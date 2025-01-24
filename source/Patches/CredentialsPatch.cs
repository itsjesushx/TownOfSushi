using TMPro;
//Original code from TheOtherRoles (https://github.com/TheOtherRolesAU/TheOtherRoles/blob/main/TheOtherRoles/Patches/CredentialsPatch.cs)

namespace TownOfSushi.Patches
{
    [HarmonyPatch]
    public static class CredentialsPatch 
    {
        public static string FllCredentialsVersion = 
$@"<size=130%><color=#B2FEFE>TownOfSushi</color></size> v{TownOfSushi.Version.ToString()}";
public static string FullCredentials =
$@"<size=60%>Created by <color=#B2FEFE>Jesushi</color>
Emotionally Helped by <color=#B2FEFE>döll</color>
Helped by <color=#B2FEFE>Cake</color>, <color=#B2FEFE>AlchlcDvl</color> & <color=#B2FEFE>50IQ</color>
Originally Coded by <color=#B2FEFE>Donners</color> & <color=#B2FEFE>MyDragonBreath</color></size>";
    public static string MainScreenText =
$@"Created by <color=#B2FEFE>Jesushi</color>
<size=60%>Emotionally Helped by <color=#B2FEFE>döll</color>
Helped by <color=#B2FEFE>Cake</color>, <color=#B2FEFE>AlchlcDvl</color> & <color=#B2FEFE>50IQ</color>
Originally Coded by <color=#B2FEFE>Donners</color> & <color=#B2FEFE>MyDragonBreath</color></size>";
    public static string CreditsText =
$@"<size=60%> <color=#B2FEFE>Formerly: Slushiegoose & Polus.gg</color></size>";

        [HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
        internal static class PingTrackerPatch
        {
            private static float DeltaTime;
            static void Postfix(PingTracker __instance)
            {
                __instance.text.alignment = TextAlignmentOptions.Top;
                var position = __instance.GetComponent<AspectPosition>();
                DeltaTime += (Time.deltaTime - DeltaTime) * 0.1f;
                var FPS = Mathf.Round(1f / DeltaTime);
                position.Alignment = AspectPosition.EdgeAlignments.Top;
                if (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started) 
                {
                    __instance.text.text = $"<size=130%><color=#B2FEFE>TownOfSushi</color></size> v{TownOfSushi.Version.ToString()} \n FPS: {FPS} " + __instance.text.text;
                    position.DistanceFromEdge = new Vector3(1.5f, 0.11f, 0);
                }
                else 
                {
                    __instance.text.text = $"{FllCredentialsVersion}\n{FullCredentials}\n FPS: {FPS} {__instance.text.text}";
                    position.DistanceFromEdge = new Vector3(0f, 0.1f, 0);
                }
                position.AdjustPosition();
            }
        }
        
        [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
        public static class LogoPatch
        {
            public static SpriteRenderer renderer;
            public static Sprite bannerSprite;
            public static Sprite horseBannerSprite;
            public static Sprite banner2Sprite;
            private static PingTracker instance;

            static void Postfix(PingTracker __instance) 
            {
                var torLogo = new GameObject("bannerLogo_TOS");
                torLogo.transform.SetParent(GameObject.Find("RightPanel").transform, false);
                torLogo.transform.localPosition = new Vector3(-0.4f, 1f, 5f);
                renderer = torLogo.AddComponent<SpriteRenderer>();
                LoadSprites();
                renderer.sprite = LoadSpriteFromResources("TownOfSushi.Resources.TownOfSushiBanner.png", 130f);
                instance = __instance;
                LoadSprites();
                renderer.sprite = bannerSprite;
                var CredentialObject = new GameObject("CredentialsTOS");
                var Credentials = CredentialObject.AddComponent<TextMeshPro>();
                Credentials.SetText($"v{TownOfSushi.Version.ToString()}\n<size=30f%>\n</size>{MainScreenText}\n<size=30%>\n</size>{CreditsText}");
                Credentials.alignment = TextAlignmentOptions.Center;
                Credentials.fontSize *= 0.05f;

                Credentials.transform.SetParent(torLogo.transform);
                Credentials.transform.localPosition = Vector3.down * 1.45f;
            }

            public static void LoadSprites() 
            {
                if (bannerSprite == null) bannerSprite = LoadSpriteFromResources("TownOfSushi.Resources.TownOfSushiBanner.png", 130f);
                if (banner2Sprite == null) banner2Sprite = LoadSpriteFromResources("TownOfSushi.Resources.TownOfSushiBanner.png", 300f);
                if (horseBannerSprite == null) horseBannerSprite = LoadSpriteFromResources("TownOfSushi.Resources.TownOfSushiBanner.png", 300f);
            }

            public static void UpdateSprite() 
            {
                LoadSprites();
                if (renderer != null) {
                    float fadeDuration = 1f;
                    instance.StartCoroutine(Effects.Lerp(fadeDuration, new Action<float>((p) => {
                        renderer.color = new Color(1, 1, 1, 1 - p);
                        if (p == 1) {
                            renderer.sprite = bannerSprite;
                            instance.StartCoroutine(Effects.Lerp(fadeDuration, new Action<float>((p) => {
                                renderer.color = new Color(1, 1, 1, p);
                            })));
                        }
                    })));
                }
            }
        }
    }
}