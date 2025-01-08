namespace TownOfSushi.Patches
{
    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameEnd))]
    public class AmongUsGameEndPatch
    {
        public static void Postfix()
        {
            Il2CppSystem.Collections.Generic.List<int> losers = new Il2CppSystem.Collections.Generic.List<int>();
            foreach (var role in GetRoles(RoleEnum.Amnesiac))
            {
                var amne = (Amnesiac)role;
                losers.Add(amne.Player.GetDefaultOutfit().ColorId);
            }
            foreach (var role in GetRoles(RoleEnum.GuardianAngel))
            {
                var ga = (GuardianAngel)role;
                losers.Add(ga.Player.GetDefaultOutfit().ColorId);
            }
            foreach (var role in GetRoles(RoleEnum.Romantic))
            {
                var Romantic = (Romantic)role;
                losers.Add(Romantic.Player.GetDefaultOutfit().ColorId);
            }
            foreach (var role in GetRoles(RoleEnum.Doomsayer))
            {
                var doom = (Doomsayer)role;
                losers.Add(doom.Player.GetDefaultOutfit().ColorId);
            }
            foreach (var role in GetRoles(RoleEnum.Executioner))
            {
                var exe = (Executioner)role;
                losers.Add(exe.Player.GetDefaultOutfit().ColorId);
            }
            foreach (var role in GetRoles(RoleEnum.Jester))
            {
                var jest = (Jester)role;
                losers.Add(jest.Player.GetDefaultOutfit().ColorId);
            }
            foreach (var role in GetRoles(RoleEnum.Agent))
            {
                var agent = (Agent)role;
                losers.Add(agent.Player.GetDefaultOutfit().ColorId);
            }
            foreach (var role in GetRoles(RoleEnum.Vulture))
            {
                var sc = (Vulture)role;
                losers.Add(sc.Player.GetDefaultOutfit().ColorId);
            }
            foreach (var role in GetRoles(RoleEnum.Hitman))
            {
                var sc = (Hitman)role;
                losers.Add(sc.Player.GetDefaultOutfit().ColorId);
            }
            foreach (var role in GetRoles(RoleEnum.Arsonist))
            {
                var arso = (Arsonist)role;
                losers.Add(arso.Player.GetDefaultOutfit().ColorId);
            }
            foreach (var role in GetRoles(RoleEnum.Juggernaut))
            {
                var jugg = (Juggernaut)role;
                losers.Add(jugg.Player.GetDefaultOutfit().ColorId);
            }
            foreach (var role in GetRoles(RoleEnum.Pestilence))
            {
                var pest = (Pestilence)role;
                losers.Add(pest.Player.GetDefaultOutfit().ColorId);
            }
            foreach (var role in GetRoles(RoleEnum.Plaguebearer))
            {
                var pb = (Plaguebearer)role;
                losers.Add(pb.Player.GetDefaultOutfit().ColorId);
            }
            foreach (var role in GetRoles(RoleEnum.Framer))
            {
                var pb = (Framer)role;
                losers.Add(pb.Player.GetDefaultOutfit().ColorId);
            }
            foreach (var role in GetRoles(RoleEnum.Glitch))
            {
                var glitch = (Glitch)role;
                losers.Add(glitch.Player.GetDefaultOutfit().ColorId);
            }
            foreach (var role in GetRoles(RoleEnum.Vampire))
            {
                var vamp = (Vampire)role;
                losers.Add(vamp.Player.GetDefaultOutfit().ColorId);
            }
            foreach (var role in GetRoles(RoleEnum.Werewolf))
            {
                var ww = (Werewolf)role;
                losers.Add(ww.Player.GetDefaultOutfit().ColorId);
            }

            var toRemoveWinners = EndGameResult.CachedWinners.ToArray().Where(o => losers.Contains(o.ColorId)).ToArray();
            for (int i = 0; i < toRemoveWinners.Count(); i++) EndGameResult.CachedWinners.Remove(toRemoveWinners[i]);

            if (NobodyWins)
            {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                return;
            }

            if (JesterWin)
            {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                foreach (var role in GetRoles(RoleEnum.Jester))
                {
                    var vamp = (Jester)role;
                    var vampData = new CachedPlayerData(vamp.Player.Data);
                    if (PlayerControl.LocalPlayer != vamp.Player) vampData.IsYou = false;
                    EndGameResult.CachedWinners.Add(vampData);
                }
            }

            if (WerewolfWin)
            {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                foreach (var role in GetRoles(RoleEnum.Werewolf))
                {
                    var vamp = (Werewolf)role;
                    var vampData = new CachedPlayerData(vamp.Player.Data);
                    if (PlayerControl.LocalPlayer != vamp.Player) vampData.IsYou = false;
                    EndGameResult.CachedWinners.Add(vampData);
                }
            }

            if (VultureWin)
            {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                foreach (var role in GetRoles(RoleEnum.Vulture))
                {
                    var vamp = (Vulture)role;
                    var vampData = new CachedPlayerData(vamp.Player.Data);
                    if (PlayerControl.LocalPlayer != vamp.Player) vampData.IsYou = false;
                    EndGameResult.CachedWinners.Add(vampData);
                }
            }

            if (JuggernautWin)
            {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                foreach (var RoleGetter in GetRoles(RoleEnum.Juggernaut))
                {
                    var role = (Juggernaut)RoleGetter;
                    var roleData = new CachedPlayerData(role.Player.Data);
                    if (PlayerControl.LocalPlayer != role.Player) roleData.IsYou = false;
                    EndGameResult.CachedWinners.Add(roleData);
                }
            }

            if (PestilenceWin)
            {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                foreach (var RoleGetter in GetRoles(RoleEnum.Pestilence))
                {
                    var role = (Pestilence)RoleGetter;
                    var roleData = new CachedPlayerData(role.Player.Data);
                    if (PlayerControl.LocalPlayer != role.Player) roleData.IsYou = false;
                    EndGameResult.CachedWinners.Add(roleData);
                }
            }

            if (PestilenceWin)
            {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                foreach (var RoleGetter in GetRoles(RoleEnum.Vampire))
                {
                    var role = (Vampire)RoleGetter;
                    var roleData = new CachedPlayerData(role.Player.Data);
                    if (PlayerControl.LocalPlayer != role.Player) roleData.IsYou = false;
                    EndGameResult.CachedWinners.Add(roleData);
                }
            }

            if (PlaguebearerWin)
            {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                foreach (var RoleGetter in GetRoles(RoleEnum.Plaguebearer))
                {
                    var role = (Plaguebearer)RoleGetter;
                    var roleData = new CachedPlayerData(role.Player.Data);
                    if (PlayerControl.LocalPlayer != role.Player) roleData.IsYou = false;
                    EndGameResult.CachedWinners.Add(roleData);
                }
            }

            if (GlitchWin)
            {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                foreach (var RoleGetter in GetRoles(RoleEnum.Glitch))
                {
                    var role = (Glitch)RoleGetter;
                    var roleData = new CachedPlayerData(role.Player.Data);
                    if (PlayerControl.LocalPlayer != role.Player) roleData.IsYou = false;
                    EndGameResult.CachedWinners.Add(roleData);
                }
            }

            if (ArsonistWin)
            {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                foreach (var RoleGetter in GetRoles(RoleEnum.Arsonist))
                {
                    var role = (Arsonist)RoleGetter;
                    var roleData = new CachedPlayerData(role.Player.Data);
                    if (PlayerControl.LocalPlayer != role.Player) roleData.IsYou = false;
                    EndGameResult.CachedWinners.Add(roleData);
                }
            }

            if (AgentWin)
            {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                foreach (var RoleGetter in GetRoles(RoleEnum.Agent))
                {
                    var role = (Agent)RoleGetter;
                    var roleData = new CachedPlayerData(role.Player.Data);
                    if (PlayerControl.LocalPlayer != role.Player) roleData.IsYou = false;
                    EndGameResult.CachedWinners.Add(roleData);
                }
            }

            if (SerialKillerWin)
            {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                foreach (var RoleGetter in GetRoles(RoleEnum.SerialKiller))
                {
                    var role = (SerialKiller)RoleGetter;
                    var roleData = new CachedPlayerData(role.Player.Data);
                    if (PlayerControl.LocalPlayer != role.Player) roleData.IsYou = false;
                    EndGameResult.CachedWinners.Add(roleData);
                }
            }

            if (ExecutionerWin)
            {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                foreach (var RoleGetter in GetRoles(RoleEnum.Executioner))
                {
                    var role = (Executioner)RoleGetter;
                    var roleData = new CachedPlayerData(role.Player.Data);
                    if (PlayerControl.LocalPlayer != role.Player) roleData.IsYou = false;
                    EndGameResult.CachedWinners.Add(roleData);
                }
            }
            

            if (DoomsayerWin)
            {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                foreach (var RoleGetter in GetRoles(RoleEnum.Doomsayer))
                {
                    var role = (Doomsayer)RoleGetter;
                    var roleData = new CachedPlayerData(role.Player.Data);
                    if (PlayerControl.LocalPlayer != role.Player) roleData.IsYou = false;
                    EndGameResult.CachedWinners.Add(roleData);
                }
            }

            if (FramerWin)
            {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                foreach (var RoleGetter in GetRoles(RoleEnum.Framer))
                {
                    var role = (Framer)RoleGetter;
                    var roleData = new CachedPlayerData(role.Player.Data);
                    if (PlayerControl.LocalPlayer != role.Player) roleData.IsYou = false;
                    EndGameResult.CachedWinners.Add(roleData);
                }
            }

            if (HitmanWin)
            {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                foreach (var role in GetRoles(RoleEnum.Hitman))
                {
                    var hit = (Hitman)role;
                    var hitData = new CachedPlayerData(hit.Player.Data);
                    if (PlayerControl.LocalPlayer != hit.Player) hitData.IsYou = false;
                    EndGameResult.CachedWinners.Add(hitData);
                }
            }

            if (CrewmatesWin)
            {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                foreach (var faction in GetFactions(Faction.Crewmates))
                {
                    var crewData = new CachedPlayerData(faction.Player.Data);
                    if (PlayerControl.LocalPlayer != faction.Player) crewData.IsYou = false;
                        EndGameResult.CachedWinners.Add(crewData);
                }
            }

            if (ImpostorsWin)
            {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                foreach (var faction in GetFactions(Faction.Impostors))
                {
                    var ImpData = new CachedPlayerData(faction.Player.Data);
                    if (PlayerControl.LocalPlayer != faction.Player) ImpData.IsYou = false;
                        EndGameResult.CachedWinners.Add(ImpData);
                }
            }

            foreach (var role in GetRoles(RoleEnum.Romantic))
            {
                var rom = (Romantic)role;
                var romTargetData = new CachedPlayerData(rom.Beloved.Data);
                foreach (CachedPlayerData winner in EndGameResult.CachedWinners.ToArray())
                {
                    if (romTargetData.ColorId == winner.ColorId)
                    {
                        var isImp = EndGameResult.CachedWinners[0].IsImpostor;
                        var romWinData = new CachedPlayerData(rom.Player.Data);
                        if (isImp) romWinData.IsImpostor = true;
                        if (PlayerControl.LocalPlayer != rom.Player) romWinData.IsYou = false;
                        EndGameResult.CachedWinners.Add(romWinData);
                    }
                }
            }
            foreach (var role in GetRoles(RoleEnum.GuardianAngel))
            {
                var ga = (GuardianAngel)role;
                var gaTargetData = new CachedPlayerData(ga.target.Data);
                foreach (CachedPlayerData winner in EndGameResult.CachedWinners.ToArray())
                {
                    if (gaTargetData.ColorId == winner.ColorId)
                    {
                        var isImp = EndGameResult.CachedWinners[0].IsImpostor;
                        var gaWinData = new CachedPlayerData(ga.Player.Data);
                        if (isImp) gaWinData.IsImpostor = true;
                        if (PlayerControl.LocalPlayer != ga.Player) gaWinData.IsYou = false;
                        EndGameResult.CachedWinners.Add(gaWinData);
                    }
                }
            }
        }
    }
}