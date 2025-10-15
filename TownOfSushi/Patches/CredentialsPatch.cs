using HarmonyLib;
using TMPro;
using UnityEngine;
//Original code from TheOtherRoles (https://github.com/TheOtherRolesAU/TheOtherRoles/blob/main/TheOtherRoles/Patches/CredentialsPatch.cs)

namespace TownOfSushi.Patches
{
    [HarmonyPatch]
    public static class CredentialsPatch
    {
        public static string ModName =
$@"<color=#B2FEFE>TownOfSushi</color> v{TownOfSushiPlugin.Version}{TownOfSushiPlugin.DevString}";
        public static string MainScreenText =
    $@"Created by <color=#B2FEFE>Jesushi</color> with help of <color=#B2FEFE>50IQ</color>
<size=90%>Emotionally Helped by <color=#B2FEFE>döll</color>
Beta testing help by <color=#B2FEFE>Cake</color>
Originally Coded by <color=#B2FEFE>AU-Avengers</color>";

        [HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
        internal static class PingTrackerPatch
        {
            private static float DeltaTime;
            internal static void Postfix(PingTracker __instance)
            {
                __instance.text.alignment = TextAlignmentOptions.Top;
                var position = __instance.GetComponent<AspectPosition>();
                DeltaTime += (Time.deltaTime - DeltaTime) * 0.1f;

                string text = __instance.text.text + $" FPS: {Mathf.Round(1f / DeltaTime)}\n<size=60%>{ModName}</size>";

                
                position.Alignment = AspectPosition.EdgeAlignments.Top;
                position.DistanceFromEdge = new Vector3(0f, 0.1f, 0);

                __instance.text.text = text;

                position.AdjustPosition();
            }
        }
        [HarmonyPriority(Priority.VeryHigh)] // to show this message first, or be overrided if any plugins do
        [HarmonyPatch(typeof(VersionShower), nameof(VersionShower.Start))]
        public static class VersionShowerUpdate
        {
            public static SpriteRenderer renderer;
            internal static void Postfix()
            {
                var TOSLogo = new GameObject("bannerLogo_TOS");

                TOSLogo.transform.SetParent(GameObject.Find("RightPanel").transform, false);
                TOSLogo.transform.localPosition = new Vector3(-0.4f, 1f, 5f);

                renderer = TOSLogo.AddComponent<SpriteRenderer>();
                renderer.sprite = TOSAssets.Banner.LoadAsset();

                var CredentialObject = new GameObject("CredentialsTOS");
                var Credentials = CredentialObject.AddComponent<TextMeshPro>();
                Credentials.SetText($"v{TownOfSushiPlugin.Version}\n{MainScreenText}");
                Credentials.alignment = TextAlignmentOptions.Center;
                Credentials.fontSize *= 0.05f;

                Credentials.transform.SetParent(TOSLogo.transform);
                Credentials.transform.localPosition = Vector3.down * 1.45f;
            }
        }
    }
}