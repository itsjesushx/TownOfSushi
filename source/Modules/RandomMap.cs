using Random2 = System.Random;

namespace TownOfSushi.Modules
{
    [HarmonyPatch]
    class RandomMap
    {
        public static byte previousMap;
        public static bool LevelImpLoaded => IL2CPPChainloader.Instance.Plugins.TryGetValue("com.DigiWorm.LevelImposter", out _);
        public static byte GetRandomMap()
        {
            Random2 _rnd = new Random2();
            float totalWeight = 0;
            totalWeight += CustomGameOptions.RandomMapSkeld;
            totalWeight += CustomGameOptions.RandomMapMira;
            totalWeight += CustomGameOptions.RandomMapPolus;
            totalWeight += CustomGameOptions.RandomMapAirship;
            totalWeight += CustomGameOptions.RandomMapFungle;
            totalWeight += CustomGameOptions.RandomMapSubmerged;
            totalWeight += CustomGameOptions.RandomMapLevelImpostor;

            if (totalWeight == 0) return GameOptionsManager.Instance.currentNormalGameOptions.MapId;

            float randomNumber = _rnd.Next(0, (int)totalWeight);
            if (randomNumber < CustomGameOptions.RandomMapSkeld) return 0;
            randomNumber -= CustomGameOptions.RandomMapSkeld;
            if (randomNumber < CustomGameOptions.RandomMapMira) return 1;
            randomNumber -= CustomGameOptions.RandomMapMira;
            if (randomNumber < CustomGameOptions.RandomMapPolus) return 2;
            randomNumber -= CustomGameOptions.RandomMapPolus;
            if (randomNumber < CustomGameOptions.RandomMapAirship) return 4;
            randomNumber -= CustomGameOptions.RandomMapAirship;
            if (randomNumber < CustomGameOptions.RandomMapFungle) return 5;
            randomNumber -= CustomGameOptions.RandomMapFungle;
            if (SubmergedCompatibility.Loaded && randomNumber < CustomGameOptions.RandomMapSubmerged) return 6;
            randomNumber -= CustomGameOptions.RandomMapSubmerged;
            if (LevelImpLoaded && randomNumber < CustomGameOptions.RandomMapLevelImpostor) return 7;

            return GameOptionsManager.Instance.currentNormalGameOptions.MapId;
        }

        [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.BeginGame))]
        [HarmonyPrefix]
        public static bool Prefix()
        {
            if (AmongUsClient.Instance.AmHost)
            {
                previousMap = GameOptionsManager.Instance.currentNormalGameOptions.MapId;
                byte map = GameOptionsManager.Instance.currentNormalGameOptions.MapId;
                if (CustomGameOptions.RandomMapEnabled)
                {
                    map = GetRandomMap();
                    GameOptionsManager.Instance.currentNormalGameOptions.MapId = map;
                }
                GameOptionsManager.Instance.currentNormalGameOptions.RoleOptions.SetRoleRate(RoleTypes.Scientist, 0, 0);
                GameOptionsManager.Instance.currentNormalGameOptions.RoleOptions.SetRoleRate(RoleTypes.Engineer, 0, 0);
                GameOptionsManager.Instance.currentNormalGameOptions.RoleOptions.SetRoleRate(RoleTypes.GuardianAngel, 0, 0);
                GameOptionsManager.Instance.currentNormalGameOptions.RoleOptions.SetRoleRate(RoleTypes.Tracker, 0, 0);
                GameOptionsManager.Instance.currentNormalGameOptions.RoleOptions.SetRoleRate(RoleTypes.Noisemaker, 0, 0);
                GameOptionsManager.Instance.currentNormalGameOptions.RoleOptions.SetRoleRate(RoleTypes.Shapeshifter, 0, 0);
                GameOptionsManager.Instance.currentNormalGameOptions.RoleOptions.SetRoleRate(RoleTypes.Phantom, 0, 0);
                StartRPC(CustomRPC.SetSettings, map);
            }
            return true;
        }
    }
}