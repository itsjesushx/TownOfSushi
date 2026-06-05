using System.Text;
using Il2CppInterop.Runtime.Attributes;
using UnityEngine;

namespace TownOfSushi.Roles.Impostor;

public sealed class GrenadierRole(IntPtr cppPtr) : ImpostorRole(cppPtr), ITOSRole, IWikiDiscoverable, IMysticClue
{
    public string RoleName => "Grenadier";
    public string RoleDescription => "Hinder everyone's vision";
    public string RoleLongDescription => "Blind the crewmates to get sneaky kills";
    public MysticClueType MysticHintType => MysticClueType.Protective;
    public Color RoleColor => TownOfSushiColors.Impostor;
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;
    public RoleAlignment RoleAlignment => RoleAlignment.ImpostorConcealing;
    public Factions Faction => Factions.Impostor;

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = TownOfSushiAssets.Grenadier,
        CanUseVent = OptionGroupSingleton<GrenadierOptions>.Instance.CanVent
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ITOSRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            "The Grenadier is an Impostor Concealing role that can throw down a grenade that will blind all other players"
            + Utils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Flash",
            "Throw down a grenade flashing all players in it's radius.",
            TownOfSushiAssets.FlashSprite)
    ];
}