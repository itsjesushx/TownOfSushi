using System.Text;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Roles;
using TownOfSushi.Modules.Wiki;
using TownOfSushi.Utilities;
using UnityEngine;

namespace TownOfSushi.Roles.Impostor;

public sealed class EclipsalRole(IntPtr cppPtr) : ImpostorRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable, IDoomable
{
    public string RoleName => "Eclipsal";
    public string RoleDescription => "Block Out The Light";
    public string RoleLongDescription => "Make crewmates unable to see, slowly returning their vision to normal.";
    public Color RoleColor => TownOfSushiColors.Impostor;
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;
    public RoleAlignment RoleAlignment => RoleAlignment.ImpostorConcealing;
    public DoomableType DoomHintType => DoomableType.Perception;
    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = TosRoleIcons.Eclipsal,
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ITownOfSushiRole.SetNewTabText(this);
    }
    public string GetAdvancedDescription()
    {
        return
            "The Eclipsal is an Impostor Concealing role that can hinder the vision of all crewmates and neutrals alike, given that they are near the Eclipsal."
               + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } = [
        new("Blind",
            $"Blinding players causes their fog of war to overtake their screen, only letting them see the map and prevents reporting. After a while, they will regain their vision and have vision like normal.",
            TosImpAssets.BlindSprite),
    ];
}
