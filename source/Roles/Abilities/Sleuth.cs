namespace TownOfSushi.Roles.Abilities
{
    public class Sleuth : Ability
    {
        public List<byte> Reported = new List<byte>();
        public Sleuth(PlayerControl player) : base(player)
        {
            Name = "Sleuth";
            TaskText = () => "Know the roles of bodies you report";
            Color = ColorManager.Sleuth;
            AbilityType = AbilityEnum.Sleuth;
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CmdReportDeadBody))]
    public class SleuthBodyReport
    {
        private static void Postfix(PlayerControl __instance, [HarmonyArgument(0)] NetworkedPlayerInfo info)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(AbilityEnum.Sleuth)) return;

            GetAbility<Sleuth>(PlayerControl.LocalPlayer).Reported.Add(info.PlayerId);
        }
    }
}