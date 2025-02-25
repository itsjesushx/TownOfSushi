using HarmonyLib;
using System;
using TMPro;
using UnityEngine;

namespace TownOfSushi.Patches
{
    [HarmonyPatch]
    public static class CredentialsPatch 
    {
    public static string MainScreenText =
$@"Created by <color=#B2FEFE>Jesushi</color>
<size=60%>Emotionally Helped by <color=#B2FEFE>döll</color>
Helped by <color=#B2FEFE>Cake</color>, <color=#B2FEFE>AlchlcDvl</color> & <color=#B2FEFE>50IQ</color>
Originally Coded by <color=#B2FEFE>Eisbison</color> & <color=#B2FEFE>TheOtherRoles</color></size>";

        [HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
        internal static class PingTrackerPatch
        {
            private static float DeltaTime;
            static void Postfix(PingTracker __instance)
            {
                DeltaTime += (Time.deltaTime - DeltaTime) * 0.1f;
                var FPS = Mathf.Round(1f / DeltaTime);
                __instance.text.text = $"{__instance.text.text} \n FPS: {FPS}";
                __instance.text.color = new Color32(178, 254, 254, 255);
            }
        }
        
        [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
        public static class LogoPatch
        {
            public static SpriteRenderer renderer;
            public static Sprite bannerSprite;
            private static PingTracker instance;

            static void Postfix(PingTracker __instance) 
            {
                var torLogo = new GameObject("bannerLogo_TOS");
                torLogo.transform.SetParent(GameObject.Find("RightPanel").transform, false);
                torLogo.transform.localPosition = new Vector3(-0.4f, 1f, 5f);
                renderer = torLogo.AddComponent<SpriteRenderer>();
                LoadSprites();
                renderer.sprite = Helpers.LoadSpriteFromResources("TownOfSushi.Resources.TownOfSushiBanner.png", 130f);
                instance = __instance;
                LoadSprites();
                renderer.sprite = bannerSprite;
                var CredentialObject = new GameObject("CredentialsTOS");
                var Credentials = CredentialObject.AddComponent<TextMeshPro>();
                Credentials.SetText($"v{TownOfSushiPlugin.Version.ToString()}\n<size=30f%>\n</size>{MainScreenText}\n<size=30%>\n</size>");
                Credentials.alignment = TextAlignmentOptions.Center;
                Credentials.fontSize *= 0.05f;

                Credentials.transform.SetParent(torLogo.transform);
                Credentials.transform.localPosition = Vector3.down * 1.45f;
            }

            public static void LoadSprites() 
            {
                if (bannerSprite == null) bannerSprite = Helpers.LoadSpriteFromResources("TownOfSushi.Resources.TownOfSushiBanner.png", 130f);
            }

            public static void UpdateSprite() 
            {
                LoadSprites();
                if (renderer != null) 
                {
                    float fadeDuration = 1f;
                    instance.StartCoroutine(Effects.Lerp(fadeDuration, new Action<float>((p) => 
                    {
                        renderer.color = new Color(1, 1, 1, 1 - p);
                        if (p == 1) 
                        {
                            renderer.sprite = bannerSprite;
                            instance.StartCoroutine(Effects.Lerp(fadeDuration, new Action<float>((p) => 
                            {
                                renderer.color = new Color(1, 1, 1, p);
                            })));
                        }
                    })));
                }
            }
        }
    }
}