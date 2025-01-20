namespace TownOfSushi.Roles
{
    [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.Start))]
    public static class NeutralKillerOutro
    {
        public static void Postfix(EndGameManager __instance)
        {
            if (NeutralEvilWin()) return;
            
            AddToRoleOutro(__instance, RoleEnum.Plaguebearer, "Plaguebearer Wins!", role => PlaguebearerWin);
            AddToRoleOutro(__instance, RoleEnum.Pestilence, "Pestilence Wins!", role => PestilenceWin);
            AddToRoleOutro(__instance, RoleEnum.Agent, "Agent Wins!", role => AgentWin);
            AddToRoleOutro(__instance, RoleEnum.Hitman, "Hitman Wins!", role => HitmanWin);
            AddToRoleOutro(__instance, RoleEnum.Juggernaut, "Juggernaut Wins!", role => JuggernautWin);
            AddToRoleOutro(__instance, RoleEnum.Glitch, "Glitch Wins!", role => GlitchWin);
            AddToRoleOutro(__instance, RoleEnum.Werewolf, "Werewolf Wins!", role => WerewolfWin);
            AddToRoleOutro(__instance, RoleEnum.SerialKiller, "Serial Killer Wins!", role => SerialKillerWin);
            AddToRoleOutro(__instance, RoleEnum.Arsonist, "Arsonist Wins!", role => ArsonistWin);
            AddToRoleOutro(__instance, RoleEnum.Vampire, "Vampires Win!", role => VampireWins);

            if (NobodyWins)
            {
                var text = Object.Instantiate(__instance.WinText);
                var color = __instance.WinText.color;
                color.a = 1f;
                text.color = color;
                text.text = "Nobody Wins!";
                var pos = __instance.WinText.transform.localPosition;
                pos.y = 1.5f;
                text.transform.position = pos;
            }
        }
    }

    [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.Start))]
    public static class NeutralEvilOutros
    {
        public static void Postfix(EndGameManager __instance)
        {
            AddToRoleOutro(__instance, RoleEnum.Jester, "Jester Wins!", role => JesterWin);
            AddToRoleOutro(__instance, RoleEnum.Executioner, "Executioner Wins!", role => ExecutionerWin);
            AddToRoleOutro(__instance, RoleEnum.Vulture, "Vulture Wins!", role => VultureWin);
            AddToRoleOutro(__instance, RoleEnum.Doomsayer, "Doomsayer Wins!", role => DoomsayerWin);
        }
    }

    [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.Start))]
    public class VanillaCrewmateOutro
    {
        public static void Postfix(EndGameManager __instance)
        {
            if (NeutralEvilWin()) return;         
            if (!CrewmatesWin) return;

            __instance.BackgroundBar.material.color = Palette.CrewmateBlue;
            var text = Object.Instantiate(__instance.WinText);
            text.text = "Crewmates Win!";
            text.color = Palette.CrewmateBlue;
            var pos = __instance.WinText.transform.localPosition;
            pos.y = 1.5f;
            text.transform.position = pos;
            text.text = $"<size=4>{text.text}</size>";
        }
    }

    [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.Start))]
    public class VanillaImpostorOutro
    {
        public static void Postfix(EndGameManager __instance)
        {
            if (NeutralEvilWin()) return;
            if (!ImpostorsWin) return;

            __instance.BackgroundBar.material.color = Palette.ImpostorRed;
            var text = Object.Instantiate(__instance.WinText);
            text.text = "Impostors Win!";
            text.color = Palette.ImpostorRed;
            var pos = __instance.WinText.transform.localPosition;
            pos.y = 1.5f;
            text.transform.position = pos;
            text.text = $"<size=4>{text.text}</size>";
        }
    }
}