namespace TownOfSushi.Roles
{
    public class Amnesiac : Role
    {
        public readonly List<GameObject> Buttons = new List<GameObject>();
        public readonly List<bool> ListOfActives = new List<bool>();
        public bool SpawnedAs = true;
        public bool Remembered = false;
        public PlayerControl ToRemember = null;
        public List<RoleEnum> RolesToRemember = new List<RoleEnum>
        {
            RoleEnum.Investigator, RoleEnum.Mystic, RoleEnum.Seer, RoleEnum.Tracker, RoleEnum.Vigilante, RoleEnum.Veteran,
            RoleEnum.Engineer, RoleEnum.Medium, RoleEnum.Transporter, RoleEnum.Trapper, RoleEnum.Medic, RoleEnum.Vulture, RoleEnum.Oracle,
            RoleEnum.Hunter, RoleEnum.Jester, RoleEnum.Executioner, RoleEnum.Witch, RoleEnum.Warlock, RoleEnum.Jailor,
            RoleEnum.Agent, RoleEnum.Hitman, RoleEnum.Miner, RoleEnum.Morphling, RoleEnum.Glitch, RoleEnum.Blackmailer, RoleEnum.Juggernaut,
            RoleEnum.Swapper, RoleEnum.Amnesiac, RoleEnum.GuardianAngel, RoleEnum.Werewolf, RoleEnum.SerialKiller, RoleEnum.Arsonist,
            RoleEnum.Grenadier, RoleEnum.Crewmate, RoleEnum.Impostor, RoleEnum.Vampire, RoleEnum.Bomber, RoleEnum.Plaguebearer, RoleEnum.Pestilence, RoleEnum.Romantic, RoleEnum.Swooper,
            RoleEnum.Venerer, RoleEnum.Janitor, RoleEnum.Escapist, RoleEnum.Doomsayer
        };
        public Amnesiac(PlayerControl player) : base(player)
        {
            Name = "Amnesiac";
            StartText = () => "Remember a role of a deceased player";
            TaskText = () => SpawnedAs ? "Wait for a meeting to remember a role" : "Your target died. Now remember a new role";
            Color = Colors.Amnesiac;
            RoleType = RoleEnum.Amnesiac;
            Faction = Faction.Neutral;
            AddToRoleHistory(RoleType);
            RoleAlignment = RoleAlignment.NeutralBenign;
        }
    }
}