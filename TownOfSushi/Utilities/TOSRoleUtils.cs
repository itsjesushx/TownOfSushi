
using System.Text;

namespace TownOfSushi.Utilities;

public static class TOSRoleUtils
{
    public static StringBuilder SetTabText(ICustomRole role)
    {
        var alignment = MiscUtils.GetRoleAlignment(role);

        var youAre = "Your role is";
        if (role is ITownOfSushiRole touRole2)
        {
            youAre = touRole2.YouAreText;
        }

        var stringB = new StringBuilder();
        stringB.AppendLine(TownOfSushiPlugin.Culture,
            $"{role.RoleColor.ToTextColor()}{youAre}<b> {role.RoleName}.</b></color>");
        stringB.AppendLine(TownOfSushiPlugin.Culture,
            $"<size=60%>Alignment: <b>{MiscUtils.GetParsedRoleAlignment(alignment, true)}</b></size>");
        stringB.Append("<size=70%>");
        stringB.AppendLine(TownOfSushiPlugin.Culture, $"{role.RoleLongDescription}");

        return stringB;
    }

    public static StringBuilder SetDeadTabText(ICustomRole role)
    {
        var alignment = MiscUtils.GetRoleAlignment(role);

        var youAre = "Your role was";
        if (role is ITownOfSushiRole touRole2)
        {
            youAre = touRole2.YouWereText;
        }

        var stringB = new StringBuilder();
        stringB.AppendLine(TownOfSushiPlugin.Culture,
            $"{role.RoleColor.ToTextColor()}{youAre}<b> {role.RoleName}.</b></color>");
        stringB.AppendLine(TownOfSushiPlugin.Culture,
            $"<size=60%>Alignment: <b>{MiscUtils.GetParsedRoleAlignment(alignment, true)}</b></size>");
        stringB.Append("<size=70%>");
        stringB.AppendLine(TownOfSushiPlugin.Culture, $"{role.RoleLongDescription}");

        return stringB;
    }
}