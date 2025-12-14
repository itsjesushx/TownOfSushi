using System.Text;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Hud;
using MiraAPI.Patches.Stubs;
using UnityEngine;

namespace TownOfSushi.Roles.Impostor;

public sealed class VenererRole(IntPtr cppPtr) : ImpostorRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable, IMysticClue
{
    public string RoleName => "Venerer";
    public string RoleDescription => "With each kill your ability becomes stronger";
    public string RoleLongDescription => "Kill players to unlock ability perks";
    public MysticClueType MysticHintType => MysticClueType.Trickster;
    public Color RoleColor => TownOfSushiColors.Impostor;
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;
    public RoleAlignment RoleAlignment => RoleAlignment.ImpostorConcealing;

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = TOSRoleIcons.Venerer
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ITownOfSushiRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            "The Venerer is an Impostor Concealing role that can kill players to gain new abilities, preventing others from catching them! However, the ability will be used immediately as they receive it, which will stack up."
            + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Camouflage",
            "Stage 1 of the abilities.\n" +
            "You will appear as a gray bean for all players, allowing you to sneak away from kills.",
            TOSImpAssets.CamouflageSprite),
        new("Sprint",
            "Stage 2 of the abilities.\n" +
            "You will gain the speed of the Flash while hidden from camo.",
            TOSImpAssets.SprintSprite),
        new("Freeze",
            "The Final Stage of the abilities.\n" +
            "You will slow down players around you in a radius, as well as being fast and hidden from camo.",
            TOSImpAssets.FreezeSprite)
    ];

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);

        CustomButtonSingleton<VenererAbilityButton>.Instance.UpdateAbility(VenererAbility.None);
    }
}

public enum VenererAbility
{
    None,
    Camouflage,
    Sprint,
    Freeze
}