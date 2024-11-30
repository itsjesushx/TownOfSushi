namespace TownOfSushi.Roles.Crewmates.Support.ImitatorRole
{
    [HarmonyPatch(typeof(AirshipExileController), nameof(AirshipExileController.WrapUpAndSpawn))]
    public static class AirshipExileController_WrapUpAndSpawn
    {
        public static void Postfix(AirshipExileController __instance) => StartImitate.ExileControllerPostfix(__instance);
    }
    
    [HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
    public class StartImitate
    {
        public static PlayerControl ImitatingPlayer;
        public static void ExileControllerPostfix(ExileController __instance)
        {
            var exiled = __instance.initData.networkedPlayer?.Object;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Imitator)) return;
            if (PlayerControl.LocalPlayer.Data.IsDead || PlayerControl.LocalPlayer.Data.Disconnected) return;
            if (exiled == PlayerControl.LocalPlayer) return;

            var imitator = GetRole<Imitator>(PlayerControl.LocalPlayer);
            if (imitator.ImitatePlayer == null) return;

            Imitate(imitator);

            Rpc(CustomRPC.StartImitate, imitator.Player.PlayerId);
        }

        public static void Postfix(ExileController __instance) => ExileControllerPostfix(__instance);

        [HarmonyPatch(typeof(Object), nameof(Object.Destroy), new Type[] { typeof(GameObject) })]
        public static void Prefix(GameObject obj)
        {
            if (!SubmergedCompatibility.Loaded || GameOptionsManager.Instance?.currentNormalGameOptions?.MapId != 6) return;
            if (obj.name?.Contains("ExileCutscene") == true) ExileControllerPostfix(ExileControllerPatch.lastExiled);
        }

        public static void Imitate(Imitator imitator)
        {
            if (imitator.ImitatePlayer == null) return;
            ImitatingPlayer = imitator.Player;
            var imitatorRole = GetPlayerRole(imitator.ImitatePlayer).RoleType;
            if (imitatorRole == RoleEnum.Haunter)
            {
                var haunter = GetRole<Haunter>(imitator.ImitatePlayer);
                imitatorRole = haunter.formerRole;
            }
            if (imitatorRole == RoleEnum.Mystic)
            {
                var Mystic = new Mystic(ImitatingPlayer);
                Mystic.LastExamined = Mystic.LastExamined.AddSeconds(CustomGameOptions.InitialExamineCd - CustomGameOptions.MysticExamineCd);
            }
            if (imitatorRole == RoleEnum.Crewmate) return;
            var role = GetPlayerRole(ImitatingPlayer);
            var killsList = (role.Kills, role.CorrectKills,  role.CorrectShot, role.IncorrectShots, role.CorrectVigilanteShot, role.CorrectAssassinKills, role.IncorrectAssassinKills);
            RoleDictionary.Remove(ImitatingPlayer.PlayerId);
            if (imitatorRole == RoleEnum.Investigator) new Investigator(ImitatingPlayer);
            if (imitatorRole == RoleEnum.Mystic) new Mystic(ImitatingPlayer);
            if (imitatorRole == RoleEnum.Seer) new Seer(ImitatingPlayer);
            if (imitatorRole == RoleEnum.Tracker) new Tracker(ImitatingPlayer);
            if (imitatorRole == RoleEnum.Veteran) new Veteran(ImitatingPlayer);
            if (imitatorRole == RoleEnum.Engineer) new Engineer(ImitatingPlayer);
            if (imitatorRole == RoleEnum.Medium) new Medium(ImitatingPlayer);
            if (imitatorRole == RoleEnum.Transporter) new Transporter(ImitatingPlayer);
            if (imitatorRole == RoleEnum.Trapper) new Trapper(ImitatingPlayer);
            if (imitatorRole == RoleEnum.Oracle) new Oracle(ImitatingPlayer);
            if (imitatorRole == RoleEnum.Medic)
            {
                var medic = new Medic(ImitatingPlayer);
                medic.UsedAbility = true;
                medic.StartingCooldown = medic.StartingCooldown.AddSeconds(-10f);
            }

            var newRole = GetPlayerRole(ImitatingPlayer);
            newRole.RemoveFromRoleHistory(newRole.RoleType);
            newRole.Kills = killsList.Kills;
            newRole.CorrectVigilanteShot = killsList.CorrectVigilanteShot;
            newRole.CorrectKills = killsList.CorrectKills;
            newRole.IncorrectShots = killsList.IncorrectShots;
            newRole.CorrectShot = killsList.CorrectShot;
            newRole.CorrectAssassinKills = killsList.CorrectAssassinKills;
            newRole.IncorrectAssassinKills = killsList.IncorrectAssassinKills;
        }
    }
}