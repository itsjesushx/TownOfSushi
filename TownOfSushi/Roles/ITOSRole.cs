using System.Text;
using Il2CppInterop.Runtime.Attributes;

namespace TownOfSushi.Roles;

public interface ITOSRole : ICustomRole
{
    RoleAlignment RoleAlignment { get; }
    Factions Faction { get; }
    bool HasImpostorVision => false;
    public virtual bool MetWinCon => false;
    RoleOptionsGroup ICustomRole.RoleOptionsGroup
    {
        get
        {
            if (RoleAlignment == RoleAlignment.CrewmateInvestigative)
            {
                return TOSRoleGroups.CrewInvest;
            }

            if (RoleAlignment == RoleAlignment.CrewmateKilling)
            {
                return TOSRoleGroups.CrewKiller;
            }

            if (RoleAlignment == RoleAlignment.CrewmateProtective)
            {
                return TOSRoleGroups.CrewProc;
            }

            if (RoleAlignment == RoleAlignment.CrewmatePower)
            {
                return TOSRoleGroups.CrewPower;
            }

            if (RoleAlignment == RoleAlignment.ImpostorConcealing)
            {
                return TOSRoleGroups.ImpConceal;
            }

            if (RoleAlignment == RoleAlignment.ImpostorKilling)
            {
                return TOSRoleGroups.ImpKiller;
            }

            if (RoleAlignment == RoleAlignment.ImpostorPower)
            {
                return TOSRoleGroups.ImpPower;
            }

            if (RoleAlignment == RoleAlignment.NeutralEvil)
            {
                return TOSRoleGroups.NeutralEvil;
            }

            if (RoleAlignment == RoleAlignment.NeutralKilling)
            {
                return TOSRoleGroups.NeutralKiller;
            }

            return Team switch
            {
                ModdedRoleTeams.Crewmate => TOSRoleGroups.CrewSup,
                ModdedRoleTeams.Impostor => TOSRoleGroups.ImpSup,
                _ => TOSRoleGroups.NeutralBenign
            };
        }
    }

    bool WinConditionMet()
    {
        return false;
    }

    /// <summary>
    ///     LobbyStart - Called for each role when a lobby begins.
    /// </summary>
    void LobbyStart()
    {
    }

    public static StringBuilder SetNewTabText(ICustomRole role)
    {
        var alignment = Utils.GetRoleAlignment(role);
        var faction = Utils.GetFactions(role);

        var stringB = new StringBuilder();
        stringB.AppendLine(TownOfSushiPlugin.Culture,
            $"{role.RoleColor.ToTextColor()}Role:<b> {role.RoleName}</b></color>");
        stringB.AppendLine(TownOfSushiPlugin.Culture,
            $"<size=60%>Faction: <b>{Utils.GetParsedFaction(faction)}</b></size>");
        stringB.AppendLine(TownOfSushiPlugin.Culture,
            $"<size=60%>Alignment: <b>{Utils.GetParsedRoleAlignment(alignment, true)}</b></size>");
        stringB.Append("<size=70%>");
        stringB.AppendLine(TownOfSushiPlugin.Culture, $"{role.RoleLongDescription}");

        return stringB;
    }

    [HideFromIl2Cpp]
    StringBuilder SetTabText()
    {
        return SetNewTabText(this);
    }
}

public enum Factions
{
    Crewmate,
    Neutral,
    Impostor
}

public enum RoleAlignment
{
    CrewmateInvestigative,
    CrewmateKilling,
    CrewmateProtective,
    ImpostorPower,
    CrewmatePower,
    CrewmateSupport,
    ImpostorConcealing,
    ImpostorKilling,
    ImpostorSupport,
    NeutralBenign,
    NeutralEvil,
    NeutralKilling
}