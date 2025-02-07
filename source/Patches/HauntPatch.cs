namespace TownOfSushi.Patches 
{
    [HarmonyPatch]
    public static class HauntMenuMinigamePatch 
    {
        // Show the role name instead of just Crewmate / Impostor
        [HarmonyPostfix]
        [HarmonyPatch(typeof(HauntMenuMinigame), nameof(HauntMenuMinigame.SetFilterText))]
        public static bool Prefix(HauntMenuMinigame __instance)
        {
            if (IsHideNSeek()) return true;
            var role = GetPlayerRole(__instance.HauntTarget);
            var modifier = GetModifier(__instance.HauntTarget);
            var roleName = role == null ? "" : $"{role.Name} ";
            var modifierName = modifier == null ? "" : $"({modifier.Name})";
            var IsDead = __instance.HauntTarget.Data.IsDead ? $"({__instance.HauntTarget.GetDeadInfo()}) " : "(Alive) ";

            if (TownOfSushi.DeadSeeRoles.Value) __instance.FilterText.text = $"{IsDead}{roleName}{modifierName}";
            else __instance.FilterText.text = "";
            
            return false;
        }

        // The impostor includes killer roles
        [HarmonyPostfix]
        [HarmonyPatch(typeof(HauntMenuMinigame), nameof(HauntMenuMinigame.MatchesFilter))]
        public static void MatchesFilterPostfix(HauntMenuMinigame __instance, PlayerControl pc, ref bool __result) 
        {
            if (!IsClassic()) return;
            if (__instance.filterMode == HauntMenuMinigame.HauntFilters.Impostor) 
            {
                __result = (pc.Data.Role.IsImpostor || pc.Is(RoleAlignment.NeutralEvil) || pc.Is(RoleAlignment.NeutralKilling)) && !pc.Data.IsDead;
            }
        }


        // Shows the "haunt evil roles button"
        [HarmonyPrefix]
        [HarmonyPatch(typeof(HauntMenuMinigame), nameof(HauntMenuMinigame.Start))]
        public static bool StartPrefix(HauntMenuMinigame __instance) 
        {
            if (!IsClassic() || !TownOfSushi.DeadSeeRoles.Value) return true;
            __instance.FilterButtons[0].gameObject.SetActive(true);
            int numActive = 0;
            int numButtons = __instance.FilterButtons.Count((PassiveButton s) => s.isActiveAndEnabled);
            float edgeDist = 0.6f * (float)numButtons;
		    for (int i = 0; i< __instance.FilterButtons.Length; i++)
		    {
			    PassiveButton passiveButton = __instance.FilterButtons[i];
			    if (passiveButton.isActiveAndEnabled)
			    {
				    passiveButton.transform.SetLocalX(FloatRange.SpreadToEdges(-edgeDist, edgeDist, numActive, numButtons));
				    numActive++;
			    }
            }
            return false;
        }
    }
}