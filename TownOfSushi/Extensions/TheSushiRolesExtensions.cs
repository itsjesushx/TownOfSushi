namespace TownOfSushi.Extensions
{
    public static class RoleExtensions
    {
        public static bool IsJester(this PlayerControl player, out Jester jester) => Jester.IsJester(player.PlayerId, out jester);
        public static bool IsAmnesiac(this PlayerControl player, out Amnesiac amne) => Amnesiac.IsAmnesiac(player.PlayerId, out amne);
        public static bool IsSurvivor(this PlayerControl player, out Survivor surv) => Survivor.IsSurvivor(player.PlayerId, out surv);
    }
}