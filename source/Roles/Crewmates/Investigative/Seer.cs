namespace TownOfSushi.Roles
{
    public class Seer : Role
    {
        public List<byte> Investigated = new List<byte>();

        public Seer(PlayerControl player) : base(player)
        {
            var RedOrGreen = CustomGameOptions.NeutEvilRed ? "red" : "green";
            var RedOrGreen2 = CustomGameOptions.NeutBenignRed ? "red" : "green";
            var RedOrGreen3 = CustomGameOptions.NeutKillingRed ? "red" : "green";
            Name = "Seer";
            StartText = () => "Reveal The Alliance Of Other Players";
            TaskText = () => "Reveal alliances of other players to find the Impostors";
            RoleInfo = $"The Seer is able to investigate the alignment of other players. If the player is a Crewmate, their name will be green, if they are an Impostor, their name will be red. If the player is a Neutral Evil role, their name will be {RedOrGreen}, and {RedOrGreen2} if they are a Neutral Benign Role. Neutral Killing Roles Will appear {RedOrGreen3} The Seer can only investigate one player every {CustomGameOptions.SeerCd} seconds.";
            LoreText = "Endowed with the power of insight, you possess the ability to unveil the true alliances of those around you. As the Seer, your gift allows you to discern the loyalties of your crewmates, shining a light on potential Impostors. Use your abilities wisely to protect the crew and expose deception.";
            RoleAlignment = RoleAlignment.CrewInvest;
            Color = Colors.Seer;
            LastInvestigated = DateTime.UtcNow;
            RoleType = RoleEnum.Seer; 
        }
        public PlayerControl ClosestPlayer;
        public DateTime LastInvestigated { get; set; }
        public float SeerTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastInvestigated;
            var num = CustomGameOptions.SeerCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class UpdateSeer
    {
        private static void UpdateMeeting(MeetingHud __instance, Seer seer)
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (!seer.Investigated.Contains(player.PlayerId)) continue;
                foreach (var state in __instance.playerStates)
                {
                    if (player.PlayerId != state.TargetPlayerId) continue;
                    var roleType = GetRole(player);
                    switch (roleType)
                    {
                        default:
                        if ((player.Is(Faction.Crewmates) && !(player.Is(RoleEnum.Veteran) || player.Is(RoleEnum.Hunter) || player.Is(RoleEnum.Vigilante))) ||
                        (( player.Is(RoleEnum.Veteran) || player.Is(RoleEnum.Hunter) || player.Is(RoleEnum.Vigilante)) && !CustomGameOptions.CrewKillingRed) ||
                        (player.Is(RoleAlignment.NeutralBenign) && !CustomGameOptions.NeutBenignRed) ||
                        (player.Is(RoleAlignment.NeutralEvil) && !CustomGameOptions.NeutEvilRed) ||
                        (player.Is(RoleAlignment.NeutralKilling) && !CustomGameOptions.NeutKillingRed))
                        {
                            state.NameText.color = Color.green;
                        }
                        else
                        {
                            state.NameText.color = Color.red;
                        }
                        break;
                    }
                }
            }
        }

        [HarmonyPriority(Priority.Last)]
        private static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (PlayerControl.LocalPlayer.Data.IsDead) return;

            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Seer)) return;
            var seer = GetRole<Seer>(PlayerControl.LocalPlayer);
            if (MeetingHud.Instance != null) UpdateMeeting(MeetingHud.Instance, seer);
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (!seer.Investigated.Contains(player.PlayerId)) continue;
                var roleType = GetPlayerRole(player);
                switch (roleType)
                {
                    default:
                        var colour = Color.red;
                        if ((player.Is(Faction.Crewmates) && !(player.Is(RoleEnum.Veteran) || player.Is(RoleEnum.Vigilante))) ||
                            ((player.Is(RoleEnum.Veteran) || player.Is(RoleEnum.Hunter)  || player.Is(RoleEnum.Vigilante)) && !CustomGameOptions.CrewKillingRed) ||
                            (player.Is(RoleAlignment.NeutralBenign) && !CustomGameOptions.NeutBenignRed) ||
                            (player.Is(RoleAlignment.NeutralEvil) && !CustomGameOptions.NeutEvilRed) ||
                            (player.Is(RoleAlignment.NeutralKilling) && !CustomGameOptions.NeutKillingRed))
                        {
                            colour = Color.green;
                        }
                        if (player.Is(AbilityEnum.Chameleon)) colour.a = GetAbility<Chameleon>(player).Opacity;
                        player.nameText().color = colour;
                    break;
                }
            }
        }
    }

    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformSeerReveal
    {
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton) return true;
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Seer);
            if (!flag) return true;
            var role = GetRole<Seer>(PlayerControl.LocalPlayer);
            if (!PlayerControl.LocalPlayer.CanMove || role.ClosestPlayer == null) return false;
            var flag2 = role.SeerTimer() == 0f;
            if (!flag2) return false;
            if (!__instance.enabled) return false;
            var maxDistance = KillDistance();
            if (Vector2.Distance(role.ClosestPlayer.GetTruePosition(),
                PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance) return false;
            if (role.ClosestPlayer == null) return false;

            var interact = Interact(PlayerControl.LocalPlayer, role.ClosestPlayer, false);
            if (interact[3] == true)
            {
                role.Investigated.Add(role.ClosestPlayer.PlayerId);
                
            }
            if (interact[0] == true)
            {
                role.LastInvestigated = DateTime.UtcNow;
                return false;
            }
            else if (interact[1] == true)
            {
                role.LastInvestigated = DateTime.UtcNow;
                role.LastInvestigated = role.LastInvestigated.AddSeconds(CustomGameOptions.ProtectKCReset - CustomGameOptions.SeerCd);
                return false;
            }
            else if (interact[2] == true) return false;
            return false;
        }
    }

    [HarmonyPatch(typeof(HudManager))]
    public class HudSeerInvestigate
    {
        [HarmonyPatch(nameof(HudManager.Update))]
        public static void Postfix(HudManager __instance)
        {
            UpdateInvButton(__instance);
        }

        public static void UpdateInvButton(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Seer)) return;
            var investigateButton = __instance.KillButton;

            var role = GetRole<Seer>(PlayerControl.LocalPlayer);

            investigateButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            investigateButton.SetCoolDown(role.SeerTimer(), CustomGameOptions.SeerCd);

            var notInvestigated = PlayerControl.AllPlayerControls
                .ToArray()
                .Where(x => !role.Investigated.Contains(x.PlayerId))
                .ToList();

            SetTarget(ref role.ClosestPlayer, investigateButton, float.NaN, notInvestigated);

            var renderer = investigateButton.graphic;

            if (role.ClosestPlayer != null)
            {
                renderer.color = Palette.EnabledColor;
                renderer.material.SetFloat("_Desat", 0f);
            }
            else
            {
                renderer.color = Palette.DisabledClear;
                renderer.material.SetFloat("_Desat", 1f);
            }
        }
    }
}