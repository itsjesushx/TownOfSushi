namespace TownOfSushi
{
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.CalculateLightRadius))]
    public static class PlayerVisionPatch
    {
        public static bool Prefix(ShipStatus __instance, [HarmonyArgument(0)] NetworkedPlayerInfo player,
            ref float __result)
        {
            if (IsHideNSeek())
            {
                if (VanillaOptions().currentHideNSeekGameOptions.useFlashlight)
                {
                    if (player.IsImpostor()) __result = __instance.MaxLightRadius * VanillaOptions().currentHideNSeekGameOptions.ImpostorFlashlightSize;
                    else __result = __instance.MaxLightRadius * VanillaOptions().currentHideNSeekGameOptions.CrewmateFlashlightSize;
                }
                else
                {
                    if (player.IsImpostor()) __result = __instance.MaxLightRadius * VanillaOptions().currentHideNSeekGameOptions.ImpostorLightMod;
                    else __result = __instance.MaxLightRadius * VanillaOptions().currentHideNSeekGameOptions.CrewLightMod;
                }
                return false;
            }

            if (player == null || player.IsDead)
            {
                __result = __instance.MaxLightRadius;
                return false;
            }

            var switchSystem = VanillaOptions().currentNormalGameOptions.MapId == 5 ? null : __instance.Systems[SystemTypes.Electrical]?.TryCast<SwitchSystem>();
            if (player.IsImpostor() || player._object.Is(RoleAlignment.NeutralKilling) ||
                (player._object.Is(RoleEnum.Jester) && CustomGameOptions.JesterImpVision) ||
                (player._object.Is(RoleEnum.Vulture) && CustomGameOptions.VultureImpVision))
            {
                __result = __instance.MaxLightRadius * VanillaOptions().currentNormalGameOptions.ImpostorLightMod;
                return false;
            }

            if (IsSubmerged())
            {
                if (player._object.Is(AbilityEnum.Torch)) __result = Mathf.Lerp(__instance.MinLightRadius, __instance.MaxLightRadius, 1) * VanillaOptions().currentNormalGameOptions.CrewLightMod;
                return false;
            }

            var t = switchSystem != null ? switchSystem.Value / 255f : 1;

            if (player._object.Is(AbilityEnum.Torch)) t = 1;

            __result = Mathf.Lerp(__instance.MinLightRadius, __instance.MaxLightRadius, t) *
                       VanillaOptions().currentNormalGameOptions.CrewLightMod;
            return false;
        }
    }
}