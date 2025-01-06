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
            RoleInfo = "The Jester is a Neutral role with its own win condition. If they are voted out after a meeting, the game finishes and they win. However, the Jester does not win if the Crewmates, Impostors or another Neutral role wins.";
            LoreText = "A trickster at heart, your mission is to deceive the crew into voting you out. As the Jester, you must play the fool, sowing confusion and suspicion to ensure that the crew believes you are an Impostor. The more they doubt you, the closer you get to your victory. But beware: if you fail to be voted out, you risk becoming a liability for the crew, and your deceit will be their undoing.";
            Color = Colors.Jester;
            RoleType = RoleEnum.Jester;
            Faction = Faction.Neutral;
            AddToRoleHistory(RoleType);
            RoleAlignment = RoleAlignment.NeutralEvil;
        }
    }

    [HarmonyPatch(typeof(ExileController), nameof(ExileController.BeginForGameplay))]
    internal class JesterMeetingExiledEnd
    {
        private static void Postfix(ExileController __instance)
        {
            var exiled = __instance.initData.networkedPlayer;
            if (exiled == null) return;
            var player = exiled.Object;

            var role = GetPlayerRole(player);
            if (role == null) return;
            if (role.RoleType == RoleEnum.Jester)
            {
                JesterWin = true;
                Rpc(CustomRPC.JesterWin);
                EndGame();
            }
        }
    }
}