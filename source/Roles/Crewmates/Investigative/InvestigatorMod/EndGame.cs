namespace TownOfSushi.Roles.Crewmates.Investigative.InvestigatorMod
{
    public class EndGame
    {
        [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.ExitGame))]
        [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.Start))]

        public static class EndGamePatch
        {
            public static void Prefix()
            {
                foreach (var role in GetRoles(RoleEnum.Investigator)) ((Investigator)role).AllPrints.Clear();
            }
        }
    }
}