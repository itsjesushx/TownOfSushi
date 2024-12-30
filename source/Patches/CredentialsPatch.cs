using System.Net.Http;
using TMPro;
using System.Threading.Tasks;
//Original code from TheOtherRoles (https://github.com/TheOtherRolesAU/TheOtherRoles/blob/main/TheOtherRoles/Patches/CredentialsPatch.cs)

namespace TownOfSushi.Patches
{
    [HarmonyPatch]
    public static class CredentialsPatch 
    {
    public static string MainScreenText =
$@"Remodded by <color=#FF0000FF>Jesushi</color>
Emotionally Helped by <color=#FF0000FF>döll</color>
Originally Coded by <color=#FF0000FF>Donners</color> & <color=#FF0000FF>MyDragonBreath</color>";
    public static string CreditsText =
$@"<size=60%> <color=#FF0000FF>Formerly: Slushiegoose & Polus.gg</color></size>";

public static string PingTrackerText =
@"<size=70%>ReModded by <color=#FF0000FF>Jesushi</color>
Emotionally Helped by <color=#FF0000FF>döll</color>
Originally Coded by <color=#FF0000FF>Donners & MyDragonBreath</color>
Formerly: <color=#FF0000FF>Polus.gg & Slushiegoose</color><size=70%>";
public static string PingTrackerText2 =
@"<size=70%>ReModded by <color=#FF0000FF>Jesushi</color>
Emotionally Helped by <color=#FF0000FF>döll</color><size=70%>";

       [HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
        internal static class PingTrackerPatch
        {
            static void Postfix(PingTracker __instance)
            {
                __instance.text.alignment = TextAlignmentOptions.Top;
                var position = __instance.GetComponent<AspectPosition>();
                position.Alignment = AspectPosition.EdgeAlignments.Top;
                if (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started) 
                {
                    __instance.text.text = $"<size=130%><color=#FF0000FF>TownOfSushi</color></size> <size=70%>v{TownOfSushi.Version.ToString()}" + TownOfSushi.VersionTag + $"\n{PingTrackerText2}\n Server: {GetRegionName()}\n Ping: {AmongUsClient.Instance.Ping}ms</size>";
                    position.DistanceFromEdge = new Vector3( 1.6f, 0.1f, 0);
                }
                else
                {
                    __instance.text.text = $"<size=130%><color=#FF0000FF>TownOfSushi</color></size> v" + TownOfSushi.Version.ToString() + TownOfSushi.VersionTag + $"\n{PingTrackerText}\nServer: {GetRegionName()}\n Ping: {AmongUsClient.Instance.Ping}ms</size>";
                    position.DistanceFromEdge = new Vector3( 0f, 0.1f, 0);
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
            public static GameObject MOTDObject;
            public static TextMeshPro ModText;

            static void Postfix(PingTracker __instance) {
                var torLogo = new GameObject("bannerLogo_TOS");
                torLogo.transform.SetParent(GameObject.Find("RightPanel").transform, false);
                torLogo.transform.localPosition = new Vector3(-0.4f, 1f, 5f);
                renderer = torLogo.AddComponent<SpriteRenderer>();
                loadSprites();
                renderer.sprite = LoadSpriteFromResources("TownOfSushi.Resources.TownOfSushiBanner.png", 130f);
                instance = __instance;
                loadSprites();
                renderer.sprite = bannerSprite;
                var CredentialObject = new GameObject("CredentialsTOS");
                var Credentials = CredentialObject.AddComponent<TextMeshPro>();
                Credentials.SetText($"v{TownOfSushi.Version.ToString()}\n<size=30f%>\n</size>{MainScreenText}\n<size=30%>\n</size>{CreditsText}");
                Credentials.alignment = TextAlignmentOptions.Center;
                Credentials.fontSize *= 0.05f;

                Credentials.transform.SetParent(torLogo.transform);
                Credentials.transform.localPosition = Vector3.down * 1.25f;
                MOTDObject = new GameObject("torMOTD");
                ModText = MOTDObject.AddComponent<TextMeshPro>();
                ModText.alignment = TMPro.TextAlignmentOptions.Center;
                ModText.fontSize *= 0.04f;
                ModText.transform.SetParent(torLogo.transform);
                ModText.enableWordWrapping = true;
                var rect = ModText.gameObject.GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(5.2f, 0.25f);
                ModText.transform.localPosition = Vector3.down * 2.25f;
                ModText.color = new Color(1, 53f / 255, 31f / 255);
                Material mat = ModText.fontSharedMaterial;
                mat.shaderKeywords = new string[] { "OUTLINE_ON" };
                ModText.SetOutlineColor(Color.white);
                ModText.SetOutlineThickness(0.025f);
            }

            public static void loadSprites() 
            {
                if (bannerSprite == null) bannerSprite = LoadSpriteFromResources("TownOfSushi.Resources.TownOfSushiBanner.png", 130f);
                if (banner2Sprite == null) banner2Sprite = LoadSpriteFromResources("TownOfSushi.Resources.TownOfSushiBanner.png", 300f);
                if (horseBannerSprite == null) horseBannerSprite = LoadSpriteFromResources("TownOfSushi.Resources.TownOfSushiBanner.png", 300f);
            }

            public static void updateSprite() 
            {
                loadSprites();
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

        [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.LateUpdate))]
        public static class MOTD 
        {
            public static List<string> motds = new List<string>();
            private static float timer = 0f;
            private static float maxTimer = 5f;
            private static int currentIndex = 0;

            public static void Postfix() 
            {
                if (motds.Count == 0) {
                    timer = maxTimer;
                    return;
                }
                if (motds.Count > currentIndex && LogoPatch.ModText != null)
                    LogoPatch.ModText.SetText(motds[currentIndex]);
                else return;

                // fade in and out:
                float alpha = Mathf.Clamp01(Mathf.Min(new float[] { timer, maxTimer - timer }));
                if (motds.Count == 1) alpha = 1;
                LogoPatch.ModText.color = LogoPatch.ModText.color.SetAlpha(alpha);
                timer -= Time.deltaTime;
                if (timer <= 0) {
                    timer = maxTimer;
                    currentIndex = (currentIndex + 1) % motds.Count;
                }
            }

            public static async Task LoadMOTDs() 
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync("https://raw.githubusercontent.com/itsjesushx/SushiAssets/main/motd.txt");
                response.EnsureSuccessStatusCode();
                string motds = await response.Content.ReadAsStringAsync();
                foreach(string line in motds.Split("\n", StringSplitOptions.RemoveEmptyEntries)) {
                        MOTD.motds.Add(line);
                }
            }
        }        
    }
}