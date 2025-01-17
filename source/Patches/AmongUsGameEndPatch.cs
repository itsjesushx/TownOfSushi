namespace TownOfSushi.Patches
{
    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameEnd))]
    public class AmongUsGameEndPatch
    {
        public static void Postfix()
        {
            try
            {
                Il2CppSystem.Collections.Generic.List<int> losers = new Il2CppSystem.Collections.Generic.List<int>();

                foreach (var roleEnum in new RoleEnum[]
                {
                    RoleEnum.Amnesiac, RoleEnum.GuardianAngel, RoleEnum.Romantic, RoleEnum.Doomsayer, RoleEnum.Executioner,
                    RoleEnum.Jester, RoleEnum.Agent, RoleEnum.Vulture, RoleEnum.Hitman, RoleEnum.Arsonist,
                    RoleEnum.Juggernaut, RoleEnum.Pestilence, RoleEnum.Plaguebearer, RoleEnum.Glitch,
                    RoleEnum.Vampire, RoleEnum.Werewolf
                })
                {
                    foreach (var role in GetRoles(roleEnum) ?? new List<Role>())
                    {
                        if (role.Player?.Data?.IsDead == false && role.Player?.GetDefaultOutfit() != null)
                        {
                            losers.Add(role.Player.GetDefaultOutfit().ColorId);
                        }
                    }
                }

                var toRemoveWinners = EndGameResult.CachedWinners.ToArray()
                    .Where(o => losers.Contains(o.ColorId))
                    .ToArray();

                foreach (var winner in toRemoveWinners)
                {
                    EndGameResult.CachedWinners.Remove(winner);
                }

                if (NobodyWins)
                {
                    EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                    return;
                }

                var winConditions = new Dictionary<Func<bool>, Action>
                {
                    { () => JesterWin, () => AddWinners(RoleEnum.Jester) },
                    { () => WerewolfWin, () => AddWinners(RoleEnum.Werewolf) },
                    { () => VultureWin, () => AddWinners(RoleEnum.Vulture) },
                    { () => JuggernautWin, () => AddWinners(RoleEnum.Juggernaut) },
                    { () => PestilenceWin, () => AddWinners(RoleEnum.Pestilence) },
                    { () => VampireWins, () => AddWinners(RoleEnum.Vampire) },
                    { () => PlaguebearerWin, () => AddWinners(RoleEnum.Plaguebearer) },
                    { () => GlitchWin, () => AddWinners(RoleEnum.Glitch) },
                    { () => ArsonistWin, () => AddWinners(RoleEnum.Arsonist) },
                    { () => AgentWin, () => AddWinners(RoleEnum.Agent) },
                    { () => SerialKillerWin, () => AddWinners(RoleEnum.SerialKiller) },
                    { () => ExecutionerWin, () => AddWinners(RoleEnum.Executioner) },
                    { () => DoomsayerWin, () => AddWinners(RoleEnum.Doomsayer) },
                    { () => HitmanWin, () => AddWinners(RoleEnum.Hitman) },
                    { () => CrewmatesWin, () => AddWinners(Faction.Crewmates) },
                    { () => ImpostorsWin, () => AddWinners(Faction.Impostors) }
                };

                foreach (var condition in winConditions)
                {
                    if (condition.Key())
                    {
                        condition.Value();
                        break;
                    }
                }

                AddSpecialWinners(RoleEnum.Amnesiac, (role) => !role.Player.Data.IsDead && !role.Player.Data.Disconnected);
                AddGuardianAngelWinners();
                AddRomanticWinners();
            }
            catch (Exception ex)
            {
                TownOfSushi.Logger.LogError($"Error in OnGameEndPatch: {ex}");
            }
        }

        private static void AddWinners(RoleEnum roleEnum)
        {
            EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
            foreach (var role in GetRoles(roleEnum) ?? new List<Role>())
            {
                var roleData = new CachedPlayerData(role.Player.Data);
                if (PlayerControl.LocalPlayer != role.Player) roleData.IsYou = false;
                EndGameResult.CachedWinners.Add(roleData);
            }
        }

        private static void AddWinners(Faction faction)
        {
            EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
            foreach (var player in GetFactions(faction) ?? new List<Role>())
            {
                var roleData = new CachedPlayerData(player.Player.Data);
                if (PlayerControl.LocalPlayer != player.Player) roleData.IsYou = false;
                EndGameResult.CachedWinners.Add(roleData);
            }
        }

        private static void AddSpecialWinners(RoleEnum roleEnum, Func<Role, bool> condition)
        {
            foreach (var role in GetRoles(roleEnum) ?? new List<Role>())
            {
                if (condition(role))
                {
                    var isImp = EndGameResult.CachedWinners.Count > 0 && EndGameResult.CachedWinners[0].IsImpostor;
                    var roleData = new CachedPlayerData(role.Player.Data) { IsImpostor = isImp };
                    if (PlayerControl.LocalPlayer != role.Player) roleData.IsYou = false;
                    EndGameResult.CachedWinners.Add(roleData);
                }
            }
        }

        private static void AddGuardianAngelWinners()
        {
            foreach (var role in GetRoles(RoleEnum.GuardianAngel) ?? new List<Role>())
            {
                var ga = (GuardianAngel)role;
                var gaTargetData = new CachedPlayerData(ga.target.Data);
                foreach (var winner in EndGameResult.CachedWinners.ToArray())
                {
                    if (gaTargetData.ColorId == winner.ColorId)
                    {
                        var isImp = EndGameResult.CachedWinners[0].IsImpostor;
                        var gaWinData = new CachedPlayerData(ga.Player.Data) { IsImpostor = isImp };
                        if (PlayerControl.LocalPlayer != ga.Player) gaWinData.IsYou = false;
                        EndGameResult.CachedWinners.Add(gaWinData);
                    }
                }
            }
        }

        private static void AddRomanticWinners()
        {
            foreach (var role in GetRoles(RoleEnum.Romantic) ?? new List<Role>())
            {
                var romantic = (Romantic)role;
                var romanticBelovedData = new CachedPlayerData(romantic.Beloved.Data);
                foreach (var winner in EndGameResult.CachedWinners.ToArray())
                {
                    if (romanticBelovedData.ColorId == winner.ColorId)
                    {
                        var isImp = EndGameResult.CachedWinners[0].IsImpostor;
                        var romanticWinData = new CachedPlayerData(romantic.Player.Data) { IsImpostor = isImp };
                        if (PlayerControl.LocalPlayer != romantic.Player) romanticWinData.IsYou = false;
                        EndGameResult.CachedWinners.Add(romanticWinData);
                    }
                }
            }
        }
    }
}
