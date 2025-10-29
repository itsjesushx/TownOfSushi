using System.Text;
using Il2CppInterop.Runtime.Attributes;

namespace TownOfSushi.Roles;

public interface ITownOfSushiRole : ICustomRole
{
    RoleAlignment RoleAlignment { get; }

    bool HasImpostorVision => false;
    public virtual bool MetWinCon => false;
    public virtual string LocaleKey => "KEY_MISS";
    public virtual string YouAreText
    {
        get
        {
            var prefix = " a";
            if (RoleName.StartsWithVowel())
            {
                prefix = " an";
            }

            if (Configuration.MaxRoleCount is 0 or 1)
            {
                prefix = " the";
            }

            if (RoleName.StartsWith("the", StringComparison.OrdinalIgnoreCase))
            {
                prefix = "";
            }

            return $"You are{prefix}";
        }
    }

    public virtual string YouWereText
    {
        get
        {
            var prefix = "A";
            if (RoleName.StartsWithVowel())
            {
                prefix = "An";
            }

            if (Configuration.MaxRoleCount is 0 or 1)
            {
                prefix = "The";
            }

            if (RoleName.StartsWith("the", StringComparison.OrdinalIgnoreCase) ||
                LocaleKey.StartsWith("the", StringComparison.OrdinalIgnoreCase))
            {
                prefix = "";
            }

            return $"You Were {prefix}";
        }
    }

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

            if (RoleAlignment == RoleAlignment.NeutralOutlier)
            {
                return TOSRoleGroups.NeutralOutlier;
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
        return TOSRoleUtils.SetTabText(role);
    }
    public static StringBuilder SetDeadTabText(ICustomRole role)
    {
        return TOSRoleUtils.SetDeadTabText(role);
    }

    [HideFromIl2Cpp]
    StringBuilder SetTabText()
    {
        return SetNewTabText(this);
    }
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
    NeutralOutlier,
    NeutralBenign,
    NeutralEvil,
    NeutralKilling
}