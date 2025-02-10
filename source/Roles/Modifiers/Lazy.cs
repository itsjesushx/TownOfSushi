namespace TownOfSushi.Roles.Modifiers
{
    public class Lazy : Modifier
    {
        public static Vector3 Position;
        public Lazy(PlayerControl player) : base(player)
        {
            Name = "Lazy";
            TaskText = () => "You are lazy to walk to the meeting place";
            Color = ColorManager.Lazy;
            ModifierType = ModifierEnum.Lazy;
            Position = Vector3.zero;
        }
        public void SetPosition() 
        {
            if (Position == Vector3.zero) return;  // Check if this has been set, otherwise first spawn on submerged will fail
            if (Player.PlayerId == PlayerControl.LocalPlayer.PlayerId) 
            {
                PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(Position);
                if (IsSubmerged()) 
                {
                    ChangeFloor(Position.y > -7);
                }
            }
        }
    }
    //update when using platform, zipline, or ladder
    [HarmonyPatch]
    public static class UpdateStatus 
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MovingPlatformBehaviour), nameof(MovingPlatformBehaviour.UsePlatform))]
        public static void Prefix2() 
        {
            Lazy.Position = LocalPlayer().transform.position;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.ClimbLadder))]
        public static void Prefix() 
        {
            Lazy.Position = LocalPlayer().transform.position;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ZiplineBehaviour), nameof(ZiplineBehaviour.Use), new Type[] {typeof(PlayerControl), typeof(bool)})]
        public static void prefix3(ZiplineBehaviour __instance, PlayerControl player, bool fromTop) 
        {
            Lazy.Position = LocalPlayer().transform.position;
        }
    }
}