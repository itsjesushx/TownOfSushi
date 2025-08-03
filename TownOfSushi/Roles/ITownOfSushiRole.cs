using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using System.Globalization;
using System.Text;
using TownOfSushi.Utilities;

namespace TownOfSushi.Roles;

public interface ITownOfSushiRole : ICustomRole
{
    RoleAlignment RoleAlignment { get; }

    bool HasImpostorVision => false;
    public virtual bool MetWinCon => false;
    public virtual string YouAreText
    {
        get
        {
            var prefix = " a";
            if (RoleName.StartsWithVowel()) prefix = " an";
            if (Configuration.MaxRoleCount is 0 or 1) prefix = " The";
            if (RoleName.StartsWith("the", StringComparison.OrdinalIgnoreCase)) prefix = "";
            return $"You are{prefix}";
        }
    }

    bool WinConditionMet() => false;

    /// <summary>
    /// LobbyStart - Called for each role when a lobby begins.
    /// </summary>
    void LobbyStart()
    {
    }

    public static StringBuilder SetNewTabText(ICustomRole role)
    {
        var alignment = role is ITownOfSushiRole tosRole
            ? tosRole.RoleAlignment.ToDisplayString()
            : "Custom";

        if (alignment.Contains("Crewmate"))
        {
            alignment = alignment.Replace("Crewmate", "<color=#68ACF4>Crewmate");
        }
        else if (alignment.Contains("Impostor"))
        {
            alignment = alignment.Replace("Impostor", "<color=#D63F42>Impostor");
        }
        else if (alignment.Contains("Neutral"))
        {
            alignment = alignment.Replace("Neutral", "<color=#8A8A8A>Neutral");
        }

        var prefix = " a";
        if (role.RoleName.StartsWithVowel()) prefix = " an";
        if (role.Configuration.MaxRoleCount is 0 or 1) prefix = " the";
        if (role.RoleName.StartsWith("the", StringComparison.OrdinalIgnoreCase)) prefix = "";

        var stringB = new StringBuilder();
        stringB.AppendLine(CultureInfo.InvariantCulture, $"{role.RoleColor.ToTextColor()}You are{prefix}<b> {role.RoleName}.</b></color>");
        stringB.AppendLine(CultureInfo.InvariantCulture, $"<size=60%>Alignment: <b>{alignment}</color></b></size>");
        stringB.Append("<size=70%>");
        stringB.AppendLine(CultureInfo.InvariantCulture, $"{role.RoleLongDescription}");

        return stringB;
    }
    public static StringBuilder SetDeadTabText(ICustomRole role)
    {
        var alignment = role is ITownOfSushiRole tosRole
            ? tosRole.RoleAlignment.ToDisplayString()
            : "Custom";

        if (alignment.Contains("Crewmate"))
        {
            alignment = alignment.Replace("Crewmate", "<color=#68ACF4>Crewmate");
        }
        else if (alignment.Contains("Impostor"))
        {
            alignment = alignment.Replace("Impostor", "<color=#D63F42>Impostor");
        }
        else if (alignment.Contains("Neutral"))
        {
            alignment = alignment.Replace("Neutral", "<color=#8A8A8A>Neutral");
        }

        var prefix = " a";
        if (role.RoleName.StartsWithVowel()) prefix = " an";
        if (role.Configuration.MaxRoleCount is 0 or 1) prefix = " The";
        if (role.RoleName.StartsWith("the", StringComparison.OrdinalIgnoreCase)) prefix = "";

        var stringB = new StringBuilder();
        stringB.AppendLine(CultureInfo.InvariantCulture, $"{role.RoleColor.ToTextColor()}You were{prefix}<b> {role.RoleName}.</b></color>");
        stringB.AppendLine(CultureInfo.InvariantCulture, $"<size=60%>Alignment: <b>{alignment}</color></b></size>");
        stringB.Append("<size=70%>");
        stringB.AppendLine(CultureInfo.InvariantCulture, $"{role.RoleLongDescription}");

        return stringB;
    }

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return SetNewTabText(this);
    }

    RoleOptionsGroup ICustomRole.RoleOptionsGroup
    {
        get
        {
            if (RoleAlignment == RoleAlignment.CrewmateInvestigative)
            {
                return TosRoleGroups.CrewInvest;
            }
            else if (RoleAlignment == RoleAlignment.CrewmateKilling)
            {
                return TosRoleGroups.CrewKiller;
            }
            else if (RoleAlignment == RoleAlignment.CrewmateProtective)
            {
                return TosRoleGroups.CrewProc;
            }
            else if (RoleAlignment == RoleAlignment.CrewmatePower)
            {
                return TosRoleGroups.CrewPower;
            }
            else if (RoleAlignment == RoleAlignment.ImpostorConcealing)
            {
                return TosRoleGroups.ImpConceal;
            }
            else if (RoleAlignment == RoleAlignment.ImpostorKilling)
            {
                return TosRoleGroups.ImpKiller;
            }
            else if (RoleAlignment == RoleAlignment.NeutralEvil)
            {
                return TosRoleGroups.NeutralEvil;
            }
            else if (RoleAlignment == RoleAlignment.NeutralKilling)
            {
                return TosRoleGroups.NeutralKiller;
            }
            else
            {
                return Team switch
                {
                    ModdedRoleTeams.Crewmate => TosRoleGroups.CrewSup,
                    ModdedRoleTeams.Impostor => TosRoleGroups.ImpSup,
                    _ => TosRoleGroups.NeutralBenign,
                };
            }
        }
    }
}

public enum RoleAlignment
{
    CrewmateInvestigative,
    CrewmateKilling,
    CrewmateProtective,
    CrewmatePower,
    CrewmateSupport,
    ImpostorConcealing,
    ImpostorKilling,
    ImpostorSupport,
    NeutralBenign,
    NeutralEvil,
    NeutralKilling,
}