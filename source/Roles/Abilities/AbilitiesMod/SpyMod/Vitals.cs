using TownOfSushi.Roles.Crewmates.Support.MedicRole;

namespace TownOfSushi.Roles.Abilities.AbilityMod.SpyAbility
{
    [HarmonyPatch]
    public static class Vitals
    {
        private static List<TMPro.TextMeshPro> spyTexts = new List<TMPro.TextMeshPro>();

        [HarmonyPatch(typeof(VitalsMinigame), nameof(VitalsMinigame.Begin))]
        class VitalsMinigameStartPatch {
            static void Postfix(VitalsMinigame __instance) {
                if (PlayerControl.LocalPlayer.Is(AbilityEnum.Spy)) {
                    spyTexts = new List<TMPro.TextMeshPro>();
                    foreach (VitalsPanel panel in __instance.vitals) {
                        TMPro.TextMeshPro text = UnityEngine.Object.Instantiate(__instance.SabText, panel.transform);
                        spyTexts.Add(text);
                        UnityEngine.Object.DestroyImmediate(text.GetComponent<AlphaBlink>());
                        text.gameObject.SetActive(false);
                        text.transform.localScale = Vector3.one * 0.75f;
                        text.transform.localPosition = new Vector3(-0.75f, -0.23f, 0f);

                    }
                }
            }
        }

        [HarmonyPatch(typeof(VitalsMinigame), nameof(VitalsMinigame.Update))]
        class VitalsMinigameUpdatePatch {

            static void Postfix(VitalsMinigame __instance) {
                // Spy show time since death
                if (PlayerControl.LocalPlayer.Is(AbilityEnum.Spy)) {
                    for (int k = 0; k < __instance.vitals.Length; k++) {
                        VitalsPanel vitalsPanel = __instance.vitals[k];
                        NetworkedPlayerInfo player = vitalsPanel.PlayerInfo;

                        // Spy update
                        if (vitalsPanel.IsDead) {
                            DeadPlayer deadPlayer = Murder.KilledPlayers?.Where(x => x.PlayerId == player?.PlayerId)?.FirstOrDefault();
                            if (deadPlayer != null && k < spyTexts.Count && spyTexts[k] != null) {
                                float timeSinceDeath = ((float)(DateTime.UtcNow - deadPlayer.KillTime).TotalMilliseconds);
                                spyTexts[k].gameObject.SetActive(true);
                                spyTexts[k].text = Math.Round(timeSinceDeath / 1000) + "s";
                            }
                        }
                    }
                } else {
                    foreach (TMPro.TextMeshPro text in spyTexts)
                        if (text != null && text.gameObject != null)
                            text.gameObject.SetActive(false);
                }
            }
        }
    }
}