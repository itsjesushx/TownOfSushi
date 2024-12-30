namespace TownOfSushi.Modules.ColorsMod
{
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
}