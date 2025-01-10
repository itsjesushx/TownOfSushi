namespace TownOfSushi.Modules.CustomColors
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
    [HarmonyPatch(typeof(ChatNotification), nameof(ChatNotification.SetUp))]
    public class ChatNotificationColors
    {
        public static bool Prefix(ChatNotification __instance, PlayerControl sender, string text) 
        {
            __instance.timeOnScreen = 5f;
            __instance.gameObject.SetActive(true);
            __instance.SetCosmetics(sender.Data);
            string str;
            Color color;
            try 
            {
                str = ColorUtility.ToHtmlStringRGB(Palette.TextColors[__instance.player.ColorId]);
                color = Palette.TextOutlineColors[__instance.player.ColorId];
            } 
            catch
            {
                Color32 c = Palette.PlayerColors[__instance.player.ColorId];
                str = ColorUtility.ToHtmlStringRGB(c);
                        
                color = c.r + c.g + c.b  > 180 ? Palette.Black : Palette.White;
                TownOfSushi.Logger.LogMessage($"{c.r}, {c.g}, {c.b}");
            }
            __instance.playerColorText.text = __instance.player.ColorBlindName;
            __instance.playerNameText.text = "<color=#" + str + ">" + (string.IsNullOrEmpty(sender.Data.PlayerName) ? "..." : sender.Data.PlayerName);
            __instance.playerNameText.outlineColor = color;
            __instance.chatText.text = text;
            return false;
        }
    }

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

    [HarmonyPatch(typeof(TranslationController), nameof(TranslationController.GetString), new[] { typeof(StringNames), typeof(Il2CppReferenceArray<Il2CppSystem.Object>) })]
    public class PatchColours
    {
        public static bool Prefix(ref string __result, [HarmonyArgument(0)] StringNames name)
        {
            var newResult = (int)name switch
            {
                999983 => "Watermelon",
                999984 => "Chocolate",
                999985 => "Sky Blue",
                999986 => "Beige",
                999987 => "Magenta",
                999988 => "Turquoise",
                999989 => "Lilac",
                999990 => "Olive",
                999991 => "Azure",
                999992 => "Plum",
                999993 => "Jungle",
                999994 => "Mint",
                999995 => "Chartreuse",
                999996 => "Macau",
                999997 => "Tawny",
                999998 => "Gold",
                999999 => "Rainbow",
                100000 => "Monochrome",
                110000 => "Galaxy",
                120000 => "Ice",
                130000 => "Sunrise",
                140000 => "Peach",
                _ => null
            };
            if (newResult != null)
            {
                __result = newResult;
                return false;
            }
            return true;
        }
    }

    public static class PalettePatch
    {
        public static void Load()
        {
            Palette.ColorNames = new[]
            {
                StringNames.ColorRed,
                StringNames.ColorBlue,
                StringNames.ColorGreen,
                StringNames.ColorPink,
                StringNames.ColorOrange,
                StringNames.ColorYellow,
                StringNames.ColorBlack,
                StringNames.ColorWhite,
                StringNames.ColorPurple,
                StringNames.ColorBrown,
                StringNames.ColorCyan,
                StringNames.ColorLime,
                StringNames.ColorMaroon,
                StringNames.ColorRose,
                StringNames.ColorBanana,
                StringNames.ColorGray,
                StringNames.ColorTan,
                StringNames.ColorCoral,
                // New colours
                (StringNames)999983,//"Watermelon",
                (StringNames)999984,//"Chocolate",
                (StringNames)999985,//"Sky Blue",
                (StringNames)999986,//"Beige",
                (StringNames)999987,//"Magenta",
                (StringNames)999988,//"Turquoise",
                (StringNames)999989,//"Lilac",
                (StringNames)999990,//"Olive",
                (StringNames)999991,//"Azure",
                (StringNames)999992,//"Plum",
                (StringNames)999993,//"Jungle",
                (StringNames)999994,//"Mint",
                (StringNames)999995,//"Chartreuse",
                (StringNames)999996,//"Macau",
                (StringNames)999997,//"Gold",
                (StringNames)999998,//"Tawny",
                (StringNames)999999,//"Rainbow",
                (StringNames)100000, //"Monochrome",
                (StringNames)110000, //"Galaxy",
                (StringNames)120000, //"Ice",
                (StringNames)130000, //"Sunrise",
                (StringNames)140000, //"Peach",
            };
            Palette.PlayerColors = new[]
            {
                new Color32(198, 17, 17, byte.MaxValue),
                new Color32(19, 46, 210, byte.MaxValue),
                new Color32(17, 128, 45, byte.MaxValue),
                new Color32(238, 84, 187, byte.MaxValue),
                new Color32(240, 125, 13, byte.MaxValue),
                new Color32(246, 246, 87, byte.MaxValue),
                new Color32(63, 71, 78, byte.MaxValue),
                new Color32(215, 225, 241, byte.MaxValue),
                new Color32(107, 47, 188, byte.MaxValue),
                new Color32(113, 73, 30, byte.MaxValue),
                new Color32(56, byte.MaxValue, 221, byte.MaxValue),
                new Color32(80, 240, 57, byte.MaxValue),
                Palette.FromHex(6233390),
                Palette.FromHex(15515859),
                Palette.FromHex(15787944),
                Palette.FromHex(7701907),
                Palette.FromHex(9537655),
                Palette.FromHex(14115940),
                // New colours
                new Color32(168, 50, 62, byte.MaxValue),
                new Color32(60, 48, 44, byte.MaxValue),
                new Color32(61, 129, 255, byte.MaxValue),
                new Color32(240, 211, 165, byte.MaxValue),
                new Color32(255, 0, 127, byte.MaxValue),
                new Color32(61, 255, 181, byte.MaxValue),
                new Color32(186, 161, 255, byte.MaxValue),
                new Color32(97, 114, 24, byte.MaxValue),
                new Color32(1, 166, 255, byte.MaxValue),
                new Color32(79, 0, 127, byte.MaxValue),
                new Color32(0, 47, 0, byte.MaxValue),
                new Color32(151, 255, 151, byte.MaxValue),
                new Color32(207, 255, 0, byte.MaxValue),
                new Color32(0, 97, 93, byte.MaxValue),
                new Color32(205, 63, 0, byte.MaxValue),
                new Color32(255, 207, 0, byte.MaxValue),
                new Color32(0, 0, 0, byte.MaxValue),
                new Color32(0, 0, 0, byte.MaxValue),
                new Color32(0, 0, 0, byte.MaxValue),
                new Color32(0xA8, 0xDF, 0xFF, byte.MaxValue),
                new Color32(0xFF, 0xCA, 0x19, byte.MaxValue),
                new Color32(255, 164, 119, byte.MaxValue), 
            };
            Palette.ShadowColors = new[]
            {
                new Color32(122, 8, 56, byte.MaxValue),
                new Color32(9, 21, 142, byte.MaxValue),
                new Color32(10, 77, 46, byte.MaxValue),
                new Color32(172, 43, 174, byte.MaxValue),
                new Color32(180, 62, 21, byte.MaxValue),
                new Color32(195, 136, 34, byte.MaxValue),
                new Color32(30, 31, 38, byte.MaxValue),
                new Color32(132, 149, 192, byte.MaxValue),
                new Color32(59, 23, 124, byte.MaxValue),
                new Color32(94, 38, 21, byte.MaxValue),
                new Color32(36, 169, 191, byte.MaxValue),
                new Color32(21, 168, 66, byte.MaxValue),
                Palette.FromHex(4263706),
                Palette.FromHex(14586547),
                Palette.FromHex(13810825),
                Palette.FromHex(4609636),
                Palette.FromHex(5325118),
                Palette.FromHex(11813730),
                // New colours
                new Color32(101, 30, 37, byte.MaxValue),
                new Color32(30, 24, 22, byte.MaxValue),
                new Color32(31, 65, 128, byte.MaxValue),
                new Color32(120, 106, 83, byte.MaxValue),
                new Color32(191, 0, 95, byte.MaxValue),
                new Color32(31, 128, 91, byte.MaxValue),
                new Color32(93, 81, 128, byte.MaxValue),
                new Color32(66, 91, 15, byte.MaxValue),
                new Color32(17, 104, 151, byte.MaxValue),
                new Color32(55, 0, 95, byte.MaxValue),
                new Color32(0, 23, 0, byte.MaxValue),
                new Color32(109, 191, 109, byte.MaxValue),
                new Color32(143, 191, 61, byte.MaxValue),
                new Color32(0, 65, 61, byte.MaxValue),
                new Color32(141, 31, 0, byte.MaxValue),
                new Color32(191, 143, 0, byte.MaxValue),
                new Color32(0, 0, 0, byte.MaxValue),
                new Color32(0, 0, 0, byte.MaxValue),
                new Color32(0, 0, 0, byte.MaxValue),
                new Color32(0x59, 0x9F, 0xC8, byte.MaxValue),
                new Color32(0xDB, 0x44, 0x42, byte.MaxValue),
                new Color32(238, 128, 100, byte.MaxValue),
            };
        }
    }

    [Serializable]
    public struct HSBColor
    {
        public float h;
        public float s;
        public float b;
        public float a;

        public HSBColor(float h, float s, float b, float a)
        {
            this.h = h;
            this.s = s;
            this.b = b;
            this.a = a;
        }

        public HSBColor(float h, float s, float b)
        {
            this.h = h;
            this.s = s;
            this.b = b;
            a = 1f;
        }

        public HSBColor(Color col)
        {
            var temp = FromColor(col);
            h = temp.h;
            s = temp.s;
            b = temp.b;
            a = temp.a;
        }

        public static HSBColor FromColor(Color color)
        {
            var ret = new HSBColor(0f, 0f, 0f, color.a);

            var r = color.r;
            var g = color.g;
            var b = color.b;

            var max = Mathf.Max(r, Mathf.Max(g, b));

            if (max <= 0)
                return ret;

            var min = Mathf.Min(r, Mathf.Min(g, b));
            var dif = max - min;

            if (max > min)
            {
                if (g == max)
                    ret.h = (b - r) / dif * 60f + 120f;
                else if (b == max)
                    ret.h = (r - g) / dif * 60f + 240f;
                else if (b > g)
                    ret.h = (g - b) / dif * 60f + 360f;
                else
                    ret.h = (g - b) / dif * 60f;

                if (ret.h < 0)
                    ret.h = ret.h + 360f;
            }
            else
                ret.h = 0;

            ret.h *= 1f / 360f;
            ret.s = (dif / max) * 1f;
            ret.b = max;

            return ret;
        }

        public static Color ToColor(HSBColor hsbColor)
        {
            var r = hsbColor.b;
            var g = hsbColor.b;
            var b = hsbColor.b;

            if (hsbColor.s != 0)
            {
                var max = hsbColor.b;
                var dif = hsbColor.b * hsbColor.s;
                var min = hsbColor.b - dif;

                var h = hsbColor.h * 360f;

                if (h < 60f)
                {
                    r = max;
                    g = h * dif / 60f + min;
                    b = min;
                }
                else if (h < 120f)
                {
                    r = -(h - 120f) * dif / 60f + min;
                    g = max;
                    b = min;
                }
                else if (h < 180f)
                {
                    r = min;
                    g = max;
                    b = (h - 120f) * dif / 60f + min;
                }
                else if (h < 240f)
                {
                    r = min;
                    g = -(h - 240f) * dif / 60f + min;
                    b = max;
                }
                else if (h < 300f)
                {
                    r = (h - 240f) * dif / 60f + min;
                    g = min;
                    b = max;
                }
                else if (h <= 360f)
                {
                    r = max;
                    g = min;
                    b = -(h - 360f) * dif / 60 + min;
                }
                else
                {
                    r = 0;
                    g = 0;
                    b = 0;
                }
            }

            return new Color(Mathf.Clamp01(r), Mathf.Clamp01(g), Mathf.Clamp01(b), hsbColor.a);
        }

        public Color ToColor() => ToColor(this);

        public override string ToString() => $"H: {h} S: {s} B: {b}";

        public static HSBColor Lerp(HSBColor a, HSBColor b, float t)
        {
            float h, s;

            //check special case black (color.b==0): interpolate neither hue nor saturation!
            //check special case grey (color.s==0): don't interpolate hue!
            if (a.b == 0)
            {
                h = b.h;
                s = b.s;
            }
            else if (b.b == 0)
            {
                h = a.h;
                s = a.s;
            }
            else
            {
                if (a.s == 0)
                    h = b.h;
                else if (b.s == 0)
                    h = a.h;
                else
                {
                    var angle = Mathf.LerpAngle(a.h * 360f, b.h * 360f, t);

                    while (angle < 0f)
                        angle += 360f;

                    while (angle > 360f)
                        angle -= 360f;
                        
                    h = angle / 360f;
                }

                s = Mathf.Lerp(a.s, b.s, t);
            }

            return new HSBColor(h, s, Mathf.Lerp(a.b, b.b, t), Mathf.Lerp(a.a, b.a, t));
        }

        public static void Test()
        {
            var color = new HSBColor(Color.red);
            Debug.Log("red: " + color);

            color = new HSBColor(Color.green);
            Debug.Log("green: " + color);

            color = new HSBColor(Color.blue);
            Debug.Log("blue: " + color);

            color = new HSBColor(Color.grey);
            Debug.Log("grey: " + color);

            color = new HSBColor(Color.white);
            Debug.Log("white: " + color);

            color = new HSBColor(new Color(0.4f, 1f, 0.84f, 1f));
            Debug.Log("0.4, 1f, 0.84: " + color);

            Debug.Log("164,82,84   .... 0.643137f, 0.321568f, 0.329411f  :" + ToColor(new HSBColor(new Color(0.643137f, 0.321568f, 0.329411f))));
        }
    }

    public class ColorUtils
    {
    private static readonly int BackColor = Shader.PropertyToID("_BackColor");
    private static readonly int BodyColor = Shader.PropertyToID("_BodyColor");
    private static readonly int VisorColor = Shader.PropertyToID("_VisorColor");
    public static Color Rainbow => new HSBColor(PP(0, 1, 0.3f), 1, 1).ToColor();
    public static Color RainbowShadow => Shadow(Rainbow);

    public static Color Monochrome => new HSBColor(1f, 0f, PP(0f, 1f, 0.8f)).ToColor();
    public static Color MonochromeShadow => Shadow(Monochrome);

    public static Color Galaxy => new HSBColor(PP(0.5f, 0.87f, 0.4f), 1, 1).ToColor();
    public static Color GalaxyShadow => Shadow(Galaxy);

    public static Color Fire => new HSBColor(PP(0f, 0.17f, 0.4f), 1, 1).ToColor();
    public static Color FireShadow => Shadow(Fire);


    public static float PP(float min, float max, float mul)
    {
        return min + Mathf.PingPong(Time.time * mul, max - min);
    }

    public static Color Shadow(Color color)
    {
        return new Color(color.r - 0.3f, color.g - 0.3f, color.b - 0.3f);
    }

    public static void SetRainbow(Renderer rend)
    {
        rend.material.SetColor(BackColor, RainbowShadow);
        rend.material.SetColor(BodyColor, Rainbow);
        rend.material.SetColor(VisorColor, Palette.VisorColor);
    }
    public static void SetMonochrome(Renderer rend)
    {
            rend.material.SetColor(BackColor, MonochromeShadow);
            rend.material.SetColor(BodyColor, Monochrome);
            rend.material.SetColor(VisorColor, Palette.VisorColor);
    }
    public static void SetGalaxy(Renderer rend)
    {
            rend.material.SetColor(BackColor, GalaxyShadow);
            rend.material.SetColor(BodyColor, Galaxy);
            rend.material.SetColor(VisorColor, Palette.VisorColor);
    }
    public static bool IsMonochrome(int id)
    {
        if (id < 0 || id >= Palette.ColorNames.Count)
            return false;

        return (int)Palette.ColorNames[id] == 100000;
    }
    public static bool IsGalaxy(int id)
    {
        if (id < 0 || id >= Palette.ColorNames.Count)
            return false;

        return (int)Palette.ColorNames[id] == 110000;
    }

    public static bool IsRainbow(int id)
    {
        try
        {
            return (int)Palette.ColorNames[id] == 999999;
        } catch
        {
            return false;
        }
    }
    public static bool IsChanging(int id) => IsRainbow(id) || IsGalaxy(id) ||  IsMonochrome(id);
    }

    public class ColorBehaviour : MonoBehaviour
    {
        public Renderer Renderer;
        public int Id;

        public void AddRend(Renderer rend, int id)
        {
            Renderer = rend;
            Id = id;
        }

        public void Update()
        {
            if (Renderer == null) return;

            if (ColorUtils.IsRainbow(Id))
                ColorUtils.SetRainbow(Renderer);

            else if (ColorUtils.IsMonochrome(Id))
                ColorUtils.SetMonochrome(Renderer);
                
            else if (ColorUtils.IsGalaxy(Id))
                ColorUtils.SetGalaxy(Renderer);
        }

        public ColorBehaviour(IntPtr ptr) : base(ptr) { }
    }
}
