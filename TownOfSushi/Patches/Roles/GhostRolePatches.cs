using HarmonyLib;
using Rewired.Utils;

namespace TownOfSushi.Patches.Roles;

[HarmonyPatch]
public static class GhostRolePatches
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.OnClick))]
    [HarmonyPrefix]
    public static void GhostRoleClickPatch(PlayerControl __instance)
    {
        if (MeetingHud.Instance)
        {
            return;
        }

        if (PlayerControl.LocalPlayer.Data.IsDead)
        {
            return;
        }

        if (PlayerControl.LocalPlayer == null || PlayerControl.LocalPlayer.Data == null)
        {
            return;
        }

        var nearGhost = !PhysicsHelpers.AnythingBetween(PlayerControl.LocalPlayer.GetTruePosition(),
            __instance.GetTruePosition(), Constants.ShipAndObjectsMask, false);

        if (__instance.Data.Role is IGhostRole { CanBeClicked: true } ghost && nearGhost && ghost.CanCatch())
        {
            __instance.RpcCatchGhost();
        }
    }

    [HarmonyPatch(typeof(SpawnInMinigame), nameof(SpawnInMinigame.Begin))]
    [HarmonyPrefix]
    public static bool NoSpawnPatch(SpawnInMinigame __instance)
    {
        if (PlayerControl.LocalPlayer.Data.Role is IGhostRole { GhostActive: true })
        {
            __instance.Close();
            return false;
        }

        return true;
    }

    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.HandleAnimation))]
    [HarmonyPrefix]
    public static void HandleAnimationPatch(PlayerPhysics __instance, [HarmonyArgument(0)] ref bool amDead)
    {
        if (__instance.myPlayer == null)
        {
            return;
        }

        if (__instance.myPlayer.Data == null)
        {
            return;
        }

        if (__instance.myPlayer.Data.Role is IGhostRole ghost)
        {
            amDead = !ghost.GhostActive;
        }
    }

    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.ResetMoveState))]
    [HarmonyPostfix]
    public static void ResetMoveStatePatch(PlayerPhysics __instance)
    {
        if (__instance.myPlayer == null)
        {
            return;
        }

        if (__instance.myPlayer.Data == null)
        {
            return;
        }

        if (__instance.myPlayer.Data.Role is IGhostRole ghost)
        {
            __instance.myPlayer.Collider.enabled = ghost.GhostActive;
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Visible), MethodType.Setter)]
    [HarmonyPrefix]
    public static void VisibleOverridePatch(PlayerControl __instance, [HarmonyArgument(0)] ref bool value)
    {
        if (__instance.Data.Role is IGhostRole { GhostActive: true })
        {
            value = !__instance.inVent;
        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPostfix]
    public static void HudManagerVentPatch(HudManager __instance)
    {
        if (__instance.ImpostorVentButton == null ||
            __instance.ImpostorVentButton.gameObject == null ||
            __instance.ImpostorVentButton.IsNullOrDestroyed() ||
            PlayerControl.LocalPlayer == null ||
            PlayerControl.LocalPlayer.Data == null)
        {
            return;
        }

        if (PlayerControl.LocalPlayer.Data.Role is IGhostRole { GhostActive: true } &&
            PlayerControl.LocalPlayer.inVent != __instance.ImpostorVentButton.gameObject.active)
        {
            __instance.ImpostorVentButton.gameObject.SetActive(PlayerControl.LocalPlayer.inVent);
        }
    }
}