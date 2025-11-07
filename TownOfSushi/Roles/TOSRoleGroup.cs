using UnityEngine;

namespace TownOfSushi.Roles;

public static class TOSRoleGroups
{
    public static RoleOptionsGroup CrewInvest { get; } = new("Crewmate Investigative Roles", TownOfSushiColors.Crewmate);
    public static RoleOptionsGroup CrewKiller { get; } = new("Crewmate Killing Roles", TownOfSushiColors.Crewmate);
    public static RoleOptionsGroup CrewProc { get; } = new("Crewmate Protective Roles", TownOfSushiColors.Crewmate);
    public static RoleOptionsGroup CrewPower { get; } = new("Crewmate Power Roles", TownOfSushiColors.Crewmate);
    public static RoleOptionsGroup CrewSup { get; } = new("Crewmate Support Roles", TownOfSushiColors.Crewmate);
    public static RoleOptionsGroup NeutralBenign { get; } = new("Neutral Benign Roles", Color.gray);
    public static RoleOptionsGroup NeutralEvil { get; } = new("Neutral Evil Roles", Color.gray);
    public static RoleOptionsGroup NeutralOutlier { get; } = new("Neutral Outlier Roles", Color.gray);
    public static RoleOptionsGroup NeutralKiller { get; } = new("Neutral Killing Roles", Color.gray);
    public static RoleOptionsGroup ImpConceal { get; } = new("Impostor Concealing Roles", TownOfSushiColors.ImpSoft);
    public static RoleOptionsGroup ImpKiller { get; } = new("Impostor Killing Roles", TownOfSushiColors.ImpSoft);
    public static RoleOptionsGroup ImpSup { get; } = new("Impostor Support Roles", TownOfSushiColors.ImpSoft);
    public static RoleOptionsGroup ImpPower { get; } = new("Impostor Power Roles", TownOfSushiColors.ImpSoft);
}