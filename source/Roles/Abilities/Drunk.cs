namespace TownOfSushi.Roles.Abilities
{
    public class Drunk : Ability
    {
        public Drunk(PlayerControl player) : base(player)
        {
            Name = "Drunk";
            TaskText = () => "Your controls are inverted";
            Color = Colors.Drunk;
            AbilityType = AbilityEnum.Drunk;
        }
    }

    public class DrunkUpdate
    {
        [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.FixedUpdate))]
        public static class PlayerPhysics_FixedUpdate
        {
            public static void Postfix(PlayerPhysics __instance)
            {
                if (__instance.myPlayer.Is(AbilityEnum.Drunk))
                    if (__instance.AmOwner && GameData.Instance && __instance.myPlayer.CanMove && !__instance.myPlayer.Data.IsDead)
                        __instance.body.velocity *= -1;
            }
        }
    }
}