namespace TownOfSushi.Roles.Neutral.Killing.SerialKillerRole
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public class StabUnrampage
    {
        [HarmonyPriority(Priority.Last)]
        public static void Postfix(HudManager __instance)
        {
            foreach (var role in GetRoles(RoleEnum.SerialKiller))
            {
                var werewolf = (SerialKiller) role;
                if (werewolf.Stabbed)
                    werewolf.Stab();
                else if (werewolf.Enabled) werewolf.Unrampage();
            }
        }
    }
}