using System.Text;
using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using UnityEngine;

namespace TownOfSushi.Roles.Impostor;

public sealed class SwooperRole(IntPtr cppPtr) : ImpostorRole(cppPtr), ITOSRole, IWikiDiscoverable, IMysticClue
{
    public string RoleName => "Swooper";
    public string RoleDescription => "Turn invisible temporarily";
    public string RoleLongDescription => "Turn invisible and sneakily kill";
    public MysticClueType MysticHintType => MysticClueType.Hunter;
    public Color RoleColor => TownOfSushiColors.Impostor;
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;
    public RoleAlignment RoleAlignment => RoleAlignment.ImpostorConcealing;
    public Factions Faction => Factions.Impostor;

    public CustomRoleConfiguration Configuration => new(this)
    {
        CanUseVent = OptionGroupSingleton<SwooperOptions>.Instance.CanVent,
        Icon = TownOfSushiAssets.Swooper,
        IntroSound = CustomRoleUtils.GetIntroSound(RoleTypes.Phantom)
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ITOSRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return $"The {RoleName} is an Impostor Concealing role that can temporarily turn invisible."
               + Utils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Swoop",
            "Turn invisible to all players except Impostors.",
            TownOfSushiAssets.SwoopSprite),
        new("Unswoop",
            "Cancel your swoop early, or let it finish fully to make yourself visible once again.",
            TownOfSushiAssets.UnswoopSprite)
    ];
}