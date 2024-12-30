namespace TownOfSushi.Roles.Crewmates.Investigative.SeerRole
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class Update
    {
        private static void UpdateMeeting(MeetingHud __instance, Seer seer)
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (!seer.Investigated.Contains(player.PlayerId)) continue;
                foreach (var state in __instance.playerStates)
                {
                    if (player.PlayerId != state.TargetPlayerId) continue;
                    var roleType = GetPlayerRole(player);
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
}