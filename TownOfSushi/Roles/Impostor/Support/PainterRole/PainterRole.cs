using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using UnityEngine;

namespace TownOfSushi.Roles.Impostor;

public sealed class PainterRole(IntPtr cppPtr) : ImpostorRole(cppPtr), ITOSRole, IWikiDiscoverable, IMysticClue
{
    public string RoleName => "Painter";
    public string RoleDescription => "Transform everyone into random colors";
    public string RoleLongDescription => "Turn everyone into a random color";
    public MysticClueType MysticHintType => MysticClueType.Perception;
    public Color RoleColor => TownOfSushiColors.Impostor;
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;
    public RoleAlignment RoleAlignment => RoleAlignment.ImpostorConcealing;
    public Factions Faction => Factions.Impostor;
    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = TownOfSushiAssets.PaintSprite,
        CanUseVent = OptionGroupSingleton<PainterOptions>.Instance.PainterVent,
        IntroSound = CustomRoleUtils.GetIntroSound(RoleTypes.Shapeshifter)
    };
    public string GetAdvancedDescription()
    {
        return $"The {RoleName} is an Impostor Concealing role that can turn everyone into a random color."
               + Utils.AppendOptionsText(GetType());
    }
    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Paint",
            "Turn every single alive player into a random color (cannot be their current color).",
            TownOfSushiAssets.SampleSprite)
    ];
}