﻿namespace TownOfSushi.Modules.ColorsMod
{
    [HarmonyPatch(typeof(PlayerMaterial), nameof(PlayerMaterial.SetColors), typeof(int), typeof(Renderer))]
    public class SetPlayerMaterialPatch
    {
        public static bool Prefix([HarmonyArgument(0)] int colorId, [HarmonyArgument(1)] Renderer rend)
        {
            var r = rend.gameObject.GetComponent<ColorBehaviour>();

            if (r == null)
                r = rend.gameObject.AddComponent<ColorBehaviour>();

            r.AddRend(rend, colorId);
            return !ColorUtils.IsChanging(colorId);
        }
    }

    [HarmonyPatch(typeof(PlayerMaterial), nameof(PlayerMaterial.SetColors), typeof(Color), typeof(Renderer))]
    public class SetPlayerMaterialPatch2
    {
        public static bool Prefix([HarmonyArgument(1)] Renderer rend)
        {
            var r = rend.gameObject.GetComponent<ColorBehaviour>();

            if (r == null)
                r = rend.gameObject.AddComponent<ColorBehaviour>();

            r.AddRend(rend, 0);
            return true;
        }
    }
}