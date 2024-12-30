namespace TownOfSushi.Modules.ColorsMod
{
    [HarmonyPatch(typeof(PlayerTab))]
    public static class PlayerTabPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(PlayerTab.OnEnable))]
        public static void OnEnablePostfix(PlayerTab __instance)
        {
            for (int i = 0; i < __instance.ColorChips.Count; i++)
            {
                var colorChip = __instance.ColorChips[i];
                colorChip.transform.localScale *= 0.6f;
                var x = __instance.XRange.Lerp((i % 6) / 6f) + 0.25f;
                var y = __instance.YStart - (i / 6) * 0.35f;
                colorChip.transform.localPosition = new Vector3(x, y, 2f);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(PlayerTab.Update))]
        public static void UpdatePostfix(PlayerTab __instance)
        {
            for (int i = 0; i < __instance.ColorChips.Count; i++)
            {
                if (ColorUtils.IsRainbow(i))
                    __instance.ColorChips[i].Inner.SpriteColor = ColorUtils.Rainbow;
                else if (ColorUtils.IsMonochrome(i))
                    __instance.ColorChips[i].Inner.SpriteColor = ColorUtils.Monochrome;
                else if (ColorUtils.IsGalaxy(i))
                    __instance.ColorChips[i].Inner.SpriteColor = ColorUtils.Galaxy;
            }

        }
    }
}
