using UnityEngine;

namespace TownOfSushi.Utilities.Appearances;

public sealed class VisualAppearance : NetworkedPlayerInfo.PlayerOutfit
{
    public Color RendererColor { get; set; } = Color.white;
    public Color? PlayerMaterialColor { get; set; }
    public Color? NameColor { get; set; }
    public Color ColorBlindTextColor { get; set; } = Color.white;
    public bool NameVisible { get; set; } = true;

    // CODE REVIEW 22/2/2025 AEDT (D/M/Y)
    // ---------------------------------
    // I should do this

    public float Speed { get; set; } = 1f;
    public Vector3 Size { get; set; } = new Vector3(0.7f, 0.7f, 1f);

    public TownOfSushiAppearances AppearanceType { get; set; }

    public VisualAppearance(TownOfSushiAppearances appearanceType)
    {
        AppearanceType = appearanceType;
    }

    public VisualAppearance(NetworkedPlayerInfo.PlayerOutfit outfit, TownOfSushiAppearances appearanceType)
    {
        ColorId = outfit.ColorId;
        HatId = outfit.HatId;
        SkinId = outfit.SkinId;
        VisorId = outfit.VisorId;
        PlayerName = outfit.PlayerName;
        PetId = outfit.PetId;

        AppearanceType = appearanceType;
    }

    public VisualAppearance(VisualAppearance outfit, TownOfSushiAppearances appearanceType)
    {
        ColorId = outfit.ColorId;
        HatId = outfit.HatId;
        SkinId = outfit.SkinId;
        VisorId = outfit.VisorId;
        PlayerName = outfit.PlayerName;
        PetId = outfit.PetId;

        RendererColor = outfit.RendererColor;
        PlayerMaterialColor = outfit.PlayerMaterialColor;
        NameColor = outfit.NameColor;
        ColorBlindTextColor = outfit.ColorBlindTextColor;

        Speed = outfit.Speed;
        Size = outfit.Size;
        NameVisible = outfit.NameVisible;

        AppearanceType = appearanceType;
    }
}
