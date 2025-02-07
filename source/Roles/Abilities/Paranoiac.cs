namespace TownOfSushi.Roles.Abilities
{
    public class Paranoiac : Ability
    {
        public List<ArrowBehaviour> ParanoiacArrow = new List<ArrowBehaviour>();
        public PlayerControl ClosestPlayer;
        public Paranoiac(PlayerControl player) : base(player)
        {
            Name = "Paranoiac";
            TaskText = () => "Be always on high alert";
            Color = ColorManager.Paranoiac;
            AbilityType = AbilityEnum.Paranoiac;
        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class ParanoiacUpdateArrow
    {
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (LocalPlayer()== null) return;
            if (LocalPlayer().Data == null) return;
            if (!LocalPlayer().Is(AbilityEnum.Paranoiac)) return;

            var paranoiac = GetAbility<Paranoiac>(LocalPlayer());
            if (paranoiac.Player.Data.IsDead)
            {
                paranoiac.ParanoiacArrow.DestroyAll();
                paranoiac.ParanoiacArrow.Clear();
            }

            foreach (var arrow in paranoiac.ParanoiacArrow)
            {
                paranoiac.ClosestPlayer = GetClosestPlayer(LocalPlayer(), AllPlayers().ToList());
                arrow.target = paranoiac.ClosestPlayer.transform.position;
            }
        }

        public static PlayerControl GetClosestPlayer(PlayerControl refPlayer, List<PlayerControl> AllPlayers)
        {
            var num = double.MaxValue;
            var refPosition = refPlayer.GetTruePosition();
            PlayerControl result = null;
            foreach (var player in AllPlayers)
            {
                if (player.Data.IsDead || player.PlayerId == refPlayer.PlayerId || !player.Collider.enabled) continue;
                var playerPosition = player.GetTruePosition();
                var distBetweenPlayers = Vector2.Distance(refPosition, playerPosition);
                var isClosest = distBetweenPlayers < num;
                if (!isClosest) continue;
                var vector = playerPosition - refPosition;
                num = distBetweenPlayers;
                result = player;
            }

            return result;
        }
    }
}