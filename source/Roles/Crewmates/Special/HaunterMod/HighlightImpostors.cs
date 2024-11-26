namespace TownOfSushi.Roles.Crewmates.Special.HaunterMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HighlightImpostors
    {
        public static void UpdateMeeting(MeetingHud __instance)
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                foreach (var state in __instance.playerStates)
                {
                    if (player.PlayerId != state.TargetPlayerId) continue;
                    var role = GetPlayerRole(player);
                    if (player.Is(Faction.Impostors))
                        state.NameText.color = Palette.ImpostorRed;
                    if (player.Is(RoleAlignment.NeutralKilling) && CustomGameOptions.HaunterRevealsNeutrals)
                        state.NameText.color = role.Color;
                }
            }
        }
        public static void Postfix(HudManager __instance)
        {
            foreach (var haunter in GetRoles(RoleEnum.Haunter))
            {
                var role = (Haunter)haunter;
                if (!role.CompletedTasks || role.Caught) return;
                if (MeetingHud.Instance) UpdateMeeting(MeetingHud.Instance);
            }
        }
    }
}