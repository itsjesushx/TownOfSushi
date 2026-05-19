using System.Collections.Generic;
namespace TownOfSushi.Roles
{
    public static class Mystic 
    {
        public static bool Investigated;
        public static PlayerControl Player;
        public static Color Color = new Color32(77, 154, 230, byte.MaxValue);
        public static List<Vector3> deadBodyPositions = new List<Vector3>();
        public static PlayerControl CurrentTarget;
        public static string GetInfo(PlayerControl target)
        {
            var roles = Role.GetRoleInfoForPlayer(target);
            if (target == null || roles == null) return "";

            string message = "";

            foreach (Role Role in roles)
            {
                var id = Role.RoleType;

                if (id.In(
                    RoleEnum.Jester, RoleEnum.Mayor, RoleEnum.Agent, RoleEnum.Lawyer,
                    RoleEnum.Executioner, RoleEnum.Monarch, RoleEnum.Yoyo, RoleEnum.Landlord))
                {
                    message = "I twist truth and law with charm and deception. My games govern hearts and halls. \n\n(Jester, Mayor, Agent, Lawyer, Executioner, Monarch, Yoyo, Landlord)";
                }
                else if (id.In(
                    RoleEnum.Wraith, RoleEnum.Assassin, RoleEnum.Witch, RoleEnum.Blackmailer,
                    RoleEnum.Trickster, RoleEnum.Spy))
                {
                    message = "I cloak my steps, whisper secrets, and shift the board without a trace. \n\n(Wraith, Assassin, Witch, Blackmailer, Trickster, Spy)";
                }
                else if (id.In(
                    RoleEnum.Detective, RoleEnum.Hacker, RoleEnum.Oracle, RoleEnum.Mystic,
                    RoleEnum.Psychic, RoleEnum.Chronos, RoleEnum.Painter, RoleEnum.Morphling))
                {
                    message = "I peer through time and disguise, revealing fates and faces unknown. \n\n(Detective, Hacker, Oracle, Mystic, Psychic, Chronos, Painter, Morphling)";
                }
                else if (id.In(
                    RoleEnum.Engineer, RoleEnum.Miner, RoleEnum.Janitor, RoleEnum.Scavenger,
                    RoleEnum.Undertaker, RoleEnum.Trapper, RoleEnum.Grenadier, RoleEnum.Sheriff, RoleEnum.Deputy))
                {
                    message = "I wield tools of war and work, cleaning, building, burying, or blasting. \n\n(Engineer, Miner, Janitor, Scavenger, Deputy, Undertaker, Trapper, Grenadier, Sheriff)";
                }
                else if (id.In(
                    RoleEnum.Veteran, RoleEnum.BountyHunter, RoleEnum.Juggernaut, RoleEnum.Arsonist,
                    RoleEnum.Warlock, RoleEnum.Predator, RoleEnum.Viper, RoleEnum.Werewolf))
                {
                    message = "I am the storm. Fire, fang, and fury — nothing survives my wrath. \n\n(Veteran, Bounty Hunter, Juggernaut, Arsonist, Warlock, Predator, Viper, Werewolf)";
                }
                else if (id.In(
                    RoleEnum.Medic, RoleEnum.Crusader, RoleEnum.Romantic, RoleEnum.VengefulRomantic,
                    RoleEnum.Survivor))
                {
                    message = "I guard, love, endure — or avenge. Compassion and fury walk with me. \n\n(Medic, Crusader, Romantic, Vengeful Romantic, Survivor)";
                }
                else if (id.In(
                    RoleEnum.Plaguebearer, RoleEnum.Pestilence, RoleEnum.Glitch))
                {
                    message = "I am neither ally nor enemy — I am the anomaly, spreading decay or carving a lone path. \n\n(Plaguebearer, Pestilence, Glitch)";
                }
                else if (id.In(
                    RoleEnum.Hitman, RoleEnum.Tracker, RoleEnum.Crewmate, RoleEnum.Impostor))
                {
                    message = "From humble light to lethal aim, I walk among you — familiar or fatal. \n\n(Hitman, Tracker, Crewmate, Impostor)";
                }
                else if (id.In(
                    RoleEnum.Gatekeeper, RoleEnum.Amnesiac, RoleEnum.Painter, RoleEnum.Oracle))
                {
                    message = "Some guard the gates, others forget themselves — but all touch the threads of fate. \n\n(Gatekeeper, Amnesiac, Painter, Oracle)";
                }
                else
                {
                    message = "I am the unknown, the undefined... yet part of the great design. \n\n(Unclassified)";
                }

                if (!string.IsNullOrEmpty(message)) break;
            }
            return target.Data.PlayerName + "'s Mind:\n" + message;
        }


        public static int Charges;
        public static int RechargeTasksNumber;
        public static int RechargedTasks;

        public static void ClearAndReload() 
        {
            Player = null;
            CurrentTarget = null;
            Investigated = false;
            deadBodyPositions = new List<Vector3>();
            Charges = CustomGameOptions.MysticCharges;
            RechargeTasksNumber = CustomGameOptions.MysticRechargeTasksNumber;
            RechargedTasks = CustomGameOptions.MysticRechargeTasksNumber;
        }
    }
}