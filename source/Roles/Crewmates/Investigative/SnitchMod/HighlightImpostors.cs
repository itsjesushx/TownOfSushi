namespace TownOfSushi.Roles.Crewmates.Investigative.SnitchRole
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HighlightImpostors
    {
        private static void UpdateMeeting(MeetingHud __instance)
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                foreach (var state in __instance.playerStates)
                {
                    if (player.PlayerId != state.TargetPlayerId) continue;
                    var role = GetPlayerRole(player);
                    if (player.Is(Faction.Impostors))
                        state.NameText.color = Palette.ImpostorRed;
                    if (player.Is(RoleAlignment.NeutralKilling))
                        state.NameText.color = Color.gray;
                }
            }
        }

        public static void Postfix(HudManager __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Snitch)) return;
            var role = GetRole<Snitch>(PlayerControl.LocalPlayer);
            if (!role.TasksDone) return;
            if (MeetingHud.Instance) UpdateMeeting(MeetingHud.Instance);

            foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player.Data.IsImpostor())
                    {
                        var impcolour = Palette.ImpostorRed;
                        if (player.Is(AbilityEnum.Chameleon)) impcolour.a = GetAbility<Chameleon>(player).Opacity;
                        player.nameText().color = impcolour;
                    }
                    else if (player.Is(RoleAlignment.NeutralKilling))
                    {
                        var neutColour = Color.gray;
                        if (player.Is(AbilityEnum.Chameleon)) neutColour.a = GetAbility<Chameleon>(player).Opacity;
                        player.nameText().color = neutColour;
                    }
                }
        }
    }
}