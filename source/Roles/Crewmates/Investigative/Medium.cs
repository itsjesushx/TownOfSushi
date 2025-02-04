namespace TownOfSushi.Roles
{
    public class Medium : Role
    {
        public DateTime LastMediated { get; set; }
        public Dictionary<byte, ArrowBehaviour> MediatedPlayers = new Dictionary<byte, ArrowBehaviour>();
        public static Sprite Arrow => TownOfSushi.Arrow;
        public Medium(PlayerControl player) : base(player)
        {
            Name = "Medium";
            StartText = () => "Watch the spooky ghosts";
            TaskText = () => "Follow ghosts to get clues from them";
            RoleInfo = "The Medium is able to see ghosts during rounds, and can follow them to get clues from them. Every ghost will be revealed to you once you hit Mediate and if the ghosts were already dead when you hit your button. Ghosts dissapear after the next meeting.";
            LoreText = "A spiritual guide aboard the ship, you possess the rare ability to sense and interact with the spectral presences of fallen crewmates. By following these lingering spirits, you can uncover hidden truths and gather vital clues to expose the Impostors. Trust in the voices of the beyond to aid your mission.";
            Color = ColorManager.Medium;
            LastMediated = DateTime.UtcNow;
            RoleType = RoleEnum.Medium;
            Faction = Faction.Crewmates;
            AddToRoleHistory(RoleType);
            RoleAlignment = RoleAlignment.CrewInvest;
            MediatedPlayers = new Dictionary<byte, ArrowBehaviour>();
        }

        internal override bool RoleCriteria()
        {
            return (MediatedPlayers.ContainsKey(PlayerControl.LocalPlayer.PlayerId) && CustomGameOptions.ShowMediumToDead) || base.RoleCriteria();
        }
        public float MediateTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastMediated;
            var num = CustomGameOptions.MediateCooldown * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }
        public void AddMediatePlayer(byte playerId)
        {
            var gameObj = new GameObject();
            var arrow = gameObj.AddComponent<ArrowBehaviour>();
            if (Player.PlayerId == PlayerControl.LocalPlayer.PlayerId || CustomGameOptions.ShowMediumToDead)
            {
                gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                var renderer = gameObj.AddComponent<SpriteRenderer>();
                renderer.sprite = Arrow;
                arrow.image = renderer;
                gameObj.layer = 5;
                arrow.target = PlayerById(playerId).transform.position;
            }
            MediatedPlayers.Add(playerId, arrow);
            Flash(Color, 2.5f);
        }
    }

    [HarmonyPatch(typeof(HudManager))]
    public class HUDMediate
    {
        [HarmonyPatch(nameof(HudManager.Update))]
        public static void Postfix(HudManager __instance)
        {
            UpdateButton(__instance);
        }
        public static void UpdateButton(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            var data = PlayerControl.LocalPlayer.Data;

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Medium))
            {
                var mediateButton = __instance.KillButton;

                var role = GetRole<Medium>(PlayerControl.LocalPlayer);
                mediateButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !Meeting() && !IsDead()
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
                if (data.IsDead) return;

                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (role.MediatedPlayers.Keys.Contains(player.PlayerId))
                    {
                        role.MediatedPlayers.GetValueSafe(player.PlayerId).target = player.transform.position;
                        player.Visible = true;
                        if (!CustomGameOptions.ShowMediatePlayer)
                        {
                            player.SetOutfit(CustomPlayerOutfitType.Camouflage, new NetworkedPlayerInfo.PlayerOutfit()
                            {
                                ColorId = player.GetDefaultOutfit().ColorId,
                                HatId = "",
                                SkinId = "",
                                VisorId = "",
                                PlayerName = " ",
                                PetId = ""
                            });
                            PlayerMaterial.SetColors(Color.grey, player.myRend());
                        }
                    }
                }
                mediateButton.SetCoolDown(role.MediateTimer(), CustomGameOptions.MediateCooldown);

                var renderer = mediateButton.graphic;
                if (!mediateButton.isCoolingDown && PlayerControl.LocalPlayer.moveable)
                {
                    renderer.color = Palette.EnabledColor;
                    renderer.material.SetFloat("_Desat", 0f);
                    return;
                }

                renderer.color = Palette.DisabledClear;
                renderer.material.SetFloat("_Desat", 1f);
            }
            else if (CustomGameOptions.ShowMediumToDead && AllRoles.Any(x => x.RoleType == RoleEnum.Medium && ((Medium) x).MediatedPlayers.Keys.Contains(PlayerControl.LocalPlayer.PlayerId)))
            {
                var role = (Medium) AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Medium && ((Medium) x).MediatedPlayers.Keys.Contains(PlayerControl.LocalPlayer.PlayerId));
                role.MediatedPlayers.GetValueSafe(PlayerControl.LocalPlayer.PlayerId).target = role.Player.transform.position;
            }
        }
    }

    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformMediate
    {
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Medium)) return true;
            var role = GetRole<Medium>(PlayerControl.LocalPlayer);
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (IsDead()) return false;
            if (!__instance.enabled) return false;
            if (role.MediateTimer() != 0f) return false;
            var abilityUsed = AbilityUsed(PlayerControl.LocalPlayer);
            if (!abilityUsed) return false;
            role.LastMediated = DateTime.UtcNow;

            List<DeadPlayer> PlayersDead = Murder.KilledPlayers.GetRange(0, Murder.KilledPlayers.Count);
            if (CustomGameOptions.DeadRevealed == DeadRevealed.Newest) PlayersDead.Reverse();
            foreach (var dead in Murder.KilledPlayers)
            {
                if (Object.FindObjectsOfType<DeadBody>().Any(x => x.ParentId == dead.PlayerId && !role.MediatedPlayers.Keys.Contains(x.ParentId)))
                {
                    role.AddMediatePlayer(dead.PlayerId);
                    StartRPC(CustomRPC.Mediate, dead.PlayerId, PlayerControl.LocalPlayer.PlayerId);
                    if (CustomGameOptions.DeadRevealed != DeadRevealed.All) return false;
                }
            }
            
            return false;
        }
    }
}