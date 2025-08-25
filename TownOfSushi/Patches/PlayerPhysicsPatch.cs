using HarmonyLib;
using Il2CppInterop.Runtime.Attributes;

namespace TownOfSushi.Patches
{
    // There is probably a better way to do this, or maybe not, but this works for now.
    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.FixedUpdate))]
    public static class PlayerPhysicsFixedUpdate 
    {
        public static void Postfix(PlayerPhysics __instance)
        {
            bool invertControls = ModifierUtils.GetActiveModifiers<DrunkModifier>().Any([HideFromIl2Cpp] (x) => x.Player.PlayerId == PlayerControl.LocalPlayer.PlayerId && x.DrunkDuration > 0);
            if (__instance.AmOwner && AmongUsClient.Instance &&
            AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started &&
            !PlayerControl.LocalPlayer.Data.IsDead && invertControls && GameData.Instance &&
            __instance.myPlayer.CanMove)
            {
                __instance.body.velocity *= -1;
            }
        }
    }
}