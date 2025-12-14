using System.Text;
using Il2CppInterop.Runtime.Attributes;
using UnityEngine;

namespace TownOfSushi.Roles.Neutral;

public sealed class EclipsalRole(IntPtr cppPtr) : NeutralRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable, IMysticClue
{
    public string RoleName => "Eclipsal";
    public string RoleDescription => "Blind people and sneakily kill everyone";
    public string RoleLongDescription => "Make crewmates unable to see, slowly returning their vision to normal.";
    public Color RoleColor => TownOfSushiColors.Eclipsal;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;
    public MysticClueType MysticHintType => MysticClueType.Perception;
    public RoleAlignment RoleAlignment => RoleAlignment.NeutralKilling;

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = TOSRoleIcons.Eclipsal,
        CanUseVent = OptionGroupSingleton<EclipsalOptions>.Instance.CanUseVents,
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ITownOfSushiRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            $"The {RoleName} is a Neutral Killing role that can hinder the vision of all crewmates and neutrals alike, given that they are near the {RoleName}."
            + MiscUtils.AppendOptionsText(GetType());
    }

    public override bool DidWin(GameOverReason gameOverReason)
    {
        return WinConditionMet();
    }

    public bool WinConditionMet()
    {
        var roleCount = CustomRoleUtils.GetActiveRolesOfType<EclipsalRole>().Count(x => !x.Player.HasDied());

        if (MiscUtils.KillersAliveCount > roleCount)
        {
            return false;
        }

        return roleCount >= Helpers.GetAlivePlayers().Count - roleCount;
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Blind",
            "Blinding players causes their fog of war to overtake their screen, only letting them see the map and prevents reporting. After a while, they will regain their vision and have vision like normal.",
            TOSNeutAssets.BlindSprite)
    ];
}