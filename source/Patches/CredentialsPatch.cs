using TMPro;
//Original code from TheOtherRoles (https://github.com/TheOtherRolesAU/TheOtherRoles/blob/main/TheOtherRoles/Patches/CredentialsPatch.cs)

namespace TownOfSushi.Patches
{
    [HarmonyPatch]
    public static class CredentialsPatch 
    {
    public static string MainScreenText =
$@"Remodded by <color=#FF0000FF>Jesushi</color>
<size=60%>Emotionally Helped by <color=#FF0000FF>döll</color>
Helped by <color=#FF0000FF>Cake</color>, <color=#FF0000FF>AlchlcDvl</color> & <color=#FF0000FF>50IQ</color>
Originally Coded by <color=#FF0000FF>Donners</color> & <color=#FF0000FF>MyDragonBreath</color></size>";
    public static string CreditsText =
$@"<size=60%> <color=#FF0000FF>Formerly: Slushiegoose & Polus.gg</color></size>";

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