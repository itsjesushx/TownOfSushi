namespace TownOfSushi.Roles.Abilities.SleuthAbility
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CmdReportDeadBody))]
    public class BodyReport
    {
        private static void Postfix(PlayerControl __instance, [HarmonyArgument(0)] NetworkedPlayerInfo info)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(AbilityEnum.Sleuth)) return;

            Ability.GetAbility<Sleuth>(PlayerControl.LocalPlayer).Reported.Add(info.PlayerId);
        }
    }
}