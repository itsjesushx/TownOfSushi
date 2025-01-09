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
            TaskText = () => "Cast a spell on players to kill them";
            LoreText = "A master of the arcane, you wield dark magic to curse your foes. As the Witch, you can cast a spell on players that ensures their demise after the meeting ends. Your powerful enchantments allow you to strike from afar, waiting for the perfect moment to unleash your lethal magic and eliminate your targets with a deadly curse.";
            Color = Colors.Impostor;
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

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Close))]
    class WitchExileControllerBeginPatch 
    {
        private static void Postfix(MeetingHud __instance)
        {
            var witches = AllRoles.Where(x => x.RoleType == RoleEnum.Witch && x.Player != null).Cast<Witch>();
            foreach (var role in witches)
            {
                foreach (var spelledId in role.SpelledPlayers)
                {
                    var spelledPlayer = PlayerById(spelledId);
                    var deadRole = GetPlayerRole(spelledPlayer);
                    if (spelledPlayer != null && !spelledPlayer.Data.IsDead)
                    {
                        RpcMurderPlayer(spelledPlayer, spelledPlayer);
                        deadRole.DeathReason = DeathReasonEnum.Cursed;
                        role.Kills++;
                    }
                    Rpc(CustomRPC.RemoveAllBodies);
                    var buggedBodies = Object.FindObjectsOfType<DeadBody>();
                    foreach (var body in buggedBodies)
                    {
                        body.gameObject.Destroy();
                    }
                }
            }
        }
    }
    
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
    public static class WitchMeetingHudUpdate
    {
        public static void Postfix(MeetingHud __instance)
        {
            var localPlayer = PlayerControl.LocalPlayer;
            var _role = GetPlayerRole(localPlayer);
            if (_role?.RoleType != RoleEnum.Witch) return;
            if (localPlayer.Data.IsDead) return;
            var role = (Witch)_role;
            foreach (var state in __instance.playerStates)
            {
                var targetId = state.TargetPlayerId;
                var playerData = PlayerById(targetId)?.Data;
                if (playerData == null || playerData.Disconnected)
                {
                    role.SpelledPlayers.Remove(targetId);
                    continue;
                }
                if (role.SpelledPlayers.Contains(targetId)) state.NameText.text += " <color=#FF0000FF> [†]</color>";
            }
        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]   
    public class HudCurse
    {
        public static Sprite Blackmail => TownOfSushi.BlackmailSprite;
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Witch)) return;
            var role = GetRole<Witch>(PlayerControl.LocalPlayer);            
            if (role.SpellButton == null)
            {
                role.SpellButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.SpellButton.graphic.enabled = true;
                role.SpellButton.gameObject.SetActive(false);
            }
                role.SpellButton.graphic.sprite = Blackmail;
                role.SpellButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
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
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Witch)) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = GetRole<Witch>(PlayerControl.LocalPlayer);
            if (role.SpellTimer() != 0f) return false;
            
            if (__instance == role.SpellButton)
            {
                var interact = Interact(PlayerControl.LocalPlayer, role.ClosestPlayer);
                if (interact[3] == true)
                {
                    role.SpelledPlayers.Add(role.ClosestPlayer.PlayerId);
                    Rpc(CustomRPC.Spell, PlayerControl.LocalPlayer.PlayerId, role.ClosestPlayer.PlayerId);
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