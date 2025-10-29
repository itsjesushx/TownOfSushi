using System.Text;
using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using UnityEngine;

namespace TownOfSushi.Roles.Impostor;

public sealed class HexbladeRole(IntPtr cppPtr)
    : ImpostorRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable, ICrewVariant, IMysticClue
{
    public RoleBehaviour CrewVariant => RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<VeteranRole>());
    public string RoleName => "Hexblade";
    public string RoleDescription => "Charge Up Your Kill Button To Multi Kill";
    public string RoleLongDescription => "Kill people in small bursts";
    public MysticClueType MysticHintType => MysticClueType.Relentless;
    public Color RoleColor => TownOfSushiColors.Impostor;
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;
    public RoleAlignment RoleAlignment => RoleAlignment.ImpostorKilling;

    public CustomRoleConfiguration Configuration => new(this)
    {
        UseVanillaKillButton = false,
        IntroSound = TOSAudio.HexbladeIntroSound,
        Icon = TOSRoleIcons.Hexblade
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ITownOfSushiRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            "The Hexblade is an Impostor Killing role that can charge up attacks to wipe out the crew quickly."
            + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Kill",
            "Replaces your regular kill button with three stages: On Cooldown, Uncharged, and Charged. " +
            "You cannot kill while on cooldown but can while it is charging up, however it will reset your charge. " +
            "When it is charged, you can kill in a small burst to kill multiple players in a short time.",
            TOSAssets.KillSprite)
    ];
}