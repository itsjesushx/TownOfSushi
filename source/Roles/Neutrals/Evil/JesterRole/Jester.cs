namespace TownOfSushi.Roles
{
    public class Jester : Role
    {
        public bool SpawnedAs = true;
        public bool VotedOut;
        public Jester(PlayerControl player) : base(player)
        {
            Name = "Jester";
            StartText = () => "Get Voted Out";
            TaskText = () => SpawnedAs ? "Get voted out!" : "Your target was killed. Now you get voted out!";
            Color = Colors.Jester;
            RoleType = RoleEnum.Jester;
            Faction = Faction.Neutral;
            AddToRoleHistory(RoleType);
            RoleAlignment = RoleAlignment.NeutralEvil;
        }
        public void Wins()
        {
            VotedOut = true;
        }
    }
}