using AmongUs.GameOptions;
using System.Text;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.GameOptions;
using MiraAPI.Roles;
using TownOfSushi.Options.Roles.Impostor;
using UnityEngine;
using TownOfSushi.Modules.Wiki;
using TownOfSushi.Utilities;

namespace TownOfSushi.Roles.Impostor;

public sealed class SwooperRole(IntPtr cppPtr) : ImpostorRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable, IDoomable
{
    public string RoleName => "Swooper";
    public string RoleDescription => "Turn Invisible Temporarily";
    public string RoleLongDescription => "Turn invisible and sneakily kill";
    public Color RoleColor => TownOfSushiColors.Impostor;
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;
    public RoleAlignment RoleAlignment => RoleAlignment.ImpostorConcealing;
    public DoomableType DoomHintType => DoomableType.Hunter;
    public CustomRoleConfiguration Configuration => new(this)
    {
        CanUseVent = OptionGroupSingleton<SwooperOptions>.Instance.CanVent,
        Icon = TosRoleIcons.Swooper,
        IntroSound = CustomRoleUtils.GetIntroSound(RoleTypes.Phantom),
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ITownOfSushiRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return "The Swooper is an Impostor Concealing role that can temporarily turn invisible."
            + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } = [
        new("Swoop",
            "Turn invisible to all players except Impostors.",
            TosImpAssets.SwoopSprite),
        new("Unswoop",
            "Cancel your swoop early, or let it finish fully to make yourself visible once again.",
            TosImpAssets.UnswoopSprite)
    ];
}
