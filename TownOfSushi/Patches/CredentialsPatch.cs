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
$@"<size=70%><color=#B2FEFE>TownOfSushi</color> v{TownOfSushiPlugin.Version}{TownOfSushiPlugin.DevString}</size>";
        public static string CreditsText =
    $@"<size=60%>Created by <color=#B2FEFE>Jesushi</color>
Code helped by <color=#B2FEFE>whichtwix</color>, <color=#B2FEFE>50 1Q</color> & <color=#B2FEFE>lekiller</color>
Beta testing help by <color=#B2FEFE>Cake</color> & <color=#B2FEFE>50 IQ</color>
Originally Coded by <color=#B2FEFE>AU-Avengers</color></size>";

        [HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
        internal static class PingTrackerPatch
        {
            private static float DeltaTime;
            internal static void Postfix(PingTracker __instance)
            {
                __instance.text.alignment = TextAlignmentOptions.Top;
                var position = __instance.GetComponent<AspectPosition>();
                DeltaTime += (Time.deltaTime - DeltaTime) * 0.1f;

                string text;
                string pingcolor = "#FF0000";
                if (AmongUsClient.Instance.Ping < 100) pingcolor = "#44dfcc";
                else if (AmongUsClient.Instance.Ping < 200) pingcolor = "#f3920e";
                else if (AmongUsClient.Instance.Ping < 400) pingcolor = "#FF0000";
                string pingText = $"<color={pingcolor}>Ping: {AmongUsClient.Instance.Ping} ms</color>";

                if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started)
                {
                   text = $"{pingText} - FPS: {Mathf.Round(1f / DeltaTime)}\n{ModName}\n{CreditsText}";
                   position.DistanceFromEdge = MeetingHud.Instance ? new Vector3(1.25f, 0.15f, 0) : new Vector3(1.55f, 0.15f, 0);
                }
                else
                {
                    text = $"{pingText} - FPS: {Mathf.Round(1f / DeltaTime)}\n{ModName}";
                    position.DistanceFromEdge = new Vector3( 0f, 0.1f, 0);
                }

                position.Alignment = AspectPosition.EdgeAlignments.Top;
                position.DistanceFromEdge = new Vector3(0f, 0.1f, 0);

                __instance.text.text = text;

                position.AdjustPosition();
            }
        }
    }
}