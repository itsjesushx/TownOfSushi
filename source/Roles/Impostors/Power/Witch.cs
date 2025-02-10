namespace TownOfSushi.Roles
{
    public class Witch : Role
    {
        public KillButton _spellButton;
        public PlayerControl ClosestPlayer;
        public List<byte> SpelledPlayers = new List<byte>();
        public DateTime LastSpelled{ get; set; }
        public Witch(PlayerControl player) : base(player)
        {
            Name = "Witch";
            StartText = () => "Cast a spell upon your foes";
            TaskText = () => "Cast a spell on players";
            RoleInfo = $"The witch can cast a spell on players, doing so adds a cross next to the player name, visible to everyone announcing they've been cursed. The spelled player can not be saved, and will be die after meeting.";
            LoreText = "A master of the arcane, you wield dark magic to curse your foes. As the Witch, you can cast a spell on players that ensures their demise after the meeting ends. Your powerful enchantments allow you to strike from afar, waiting for the perfect moment to unleash your lethal magic and eliminate your targets with a deadly curse.";
            Color = ColorManager.ImpostorRed;
            LastSpelled = DateTime.UtcNow;
            RoleType = RoleEnum.Witch;
            Faction = Faction.Impostors;
            AddToRoleHistory(RoleType);
            RoleAlignment = RoleAlignment.ImpPower;
        }

        public KillButton SpellButton
        {
            get => _spellButton;
            set
            {
                _spellButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }
        public float SpellTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastSpelled;
            var num = CustomGameOptions.SpellCd * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }


    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]   
    public class HudCurse
    {
        public static Sprite Blackmail => TownOfSushi.BlackmailSprite;
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (NullLocalPlayer()) return;
            if (NullLocalPlayerData()) return;
            if (!LocalPlayer().Is(RoleEnum.Witch)) return;
            var role = GetRole<Witch>(LocalPlayer());            
            if (role.SpellButton == null)
            {
                role.SpellButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.SpellButton.graphic.enabled = true;
                role.SpellButton.gameObject.SetActive(false);
            }

            foreach (var playerId in role.SpelledPlayers)
            {
                var player = PlayerById(playerId);
                var data = player?.Data;
                if (data == null || data.Disconnected || data.IsDead || IsDead() || playerId == LocalPlayer().PlayerId)
                    continue;
                player.nameText().text += " <color=#FF0000FF> [†]</color>";
            }
                role.SpellButton.graphic.sprite = Blackmail;
                role.SpellButton.buttonLabelText.gameObject.SetActive(true);
                role.SpellButton.buttonLabelText.text = "CURSE";
                role.SpellButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                && !Meeting() && !IsDead()
                && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
                role.SpellButton.SetCoolDown(role.SpellTimer(), CustomGameOptions.SpellCd);
                
            var notSpelled = PlayerControl.AllPlayerControls
            .ToArray()
            .Where(x => !role.SpelledPlayers.Contains(x.PlayerId))
            .ToList();

            SetTarget(ref role.ClosestPlayer, role.SpellButton, float.NaN, notSpelled);
        }
    }

    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformCurse
    {
        public static bool Prefix(KillButton __instance)
        {
            if (!LocalPlayer().Is(RoleEnum.Witch)) return true;
            if (!LocalPlayer().CanMove) return false;
            if (IsDead()) return false;
            var role = GetRole<Witch>(LocalPlayer());
            if (role.SpellTimer() != 0f) return false;
            
            if (__instance == role.SpellButton)
            {
                var interact = Interact(LocalPlayer(), role.ClosestPlayer);
                if (interact[3] == true)
                {
                    role.SpelledPlayers.Add(role.ClosestPlayer.PlayerId);
                    StartRPC(CustomRPC.Spell, LocalPlayer().PlayerId, role.ClosestPlayer.PlayerId);
                }
                if (interact[0] == true)
                {
                    role.LastSpelled = DateTime.UtcNow;
                    return false;
                }
                else if (interact[1] == true)
                {
                    role.LastSpelled = DateTime.UtcNow;
                    role.LastSpelled = role.LastSpelled.AddSeconds(CustomGameOptions.ProtectKCReset - CustomGameOptions.SpellCd);
                    return false;
                }
                else if (interact[2] == true) return false;
            }
            return true;
        }
    }

    public class SpellMeetingUpdate
    {
        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
        public class MeetingHud_Update
        {
            public static void Postfix(MeetingHud __instance)
            {
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    foreach (var role in GetRoles(RoleEnum.Witch))
                    {
                        var witch = (Witch)role;
                        if (witch.Player.Data.IsDead) return;
                        if (witch.SpelledPlayers.Contains(player.PlayerId) && !player.Data.IsDead)
                        {
                            var playerState = __instance.playerStates.FirstOrDefault(x => x.TargetPlayerId == player.PlayerId);
                            if (playerState != null)
                            {
                                playerState.NameText.text += " <color=#FF0000FF> [†]</color>";
                            }
                        }
                    }
                }
            }
        }
    }
}