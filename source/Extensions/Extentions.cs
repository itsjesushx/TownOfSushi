namespace TownOfSushi.Extensions
{
    [HarmonyPatch]
    public static class RoleExtentions
    {
        public static List<object> AllPlayerInfo(this PlayerControl player)
        {
            var role = GetPlayerRole(player);
            var modifier = GetModifier(player);
            var ability = GetAbility(player);
            return new List<object> { role, modifier, ability};
        }
    }
}