using System.Text;
using Il2CppInterop.Runtime.Attributes;
using UnityEngine;

namespace TownOfSushi.Roles.Impostor;

public sealed class TraitorRole(IntPtr cppPtr)
    : ImpostorRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable, ISpawnChange, IMysticClue
{
    [HideFromIl2Cpp] public List<RoleBehaviour> ChosenRoles { get; } = [];
    [HideFromIl2Cpp]
    public RoleBehaviour? RandomRole { get; set; }
    [HideFromIl2Cpp]
    public RoleBehaviour? SelectedRole { get; set; }
    public bool NoSpawn => true;
    public string RoleName => "Traitor";
    public string RoleDescription => "Betray The Crewmates!";
    public string RoleLongDescription => "Betray the Crewmates!";
    public MysticClueType MysticHintType => MysticClueType.Trickster;
    public Color RoleColor => TownOfSushiColors.Impostor;
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;
    public RoleAlignment RoleAlignment => RoleAlignment.ImpostorPower;

    public CustomRoleConfiguration Configuration => new(this)
    {
        MaxRoleCount = 1,
        Icon = TOSRoleIcons.Traitor
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ITownOfSushiRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            $"The Traitor is an Impostor Killing role that spawns after a meeting, in which the spawn conditions are suitable. The Traitor will never be a Mayor, and must be a crewmate. The Traitor sets out to win the game for the fallen Impostors, and kill off the crew. They are also able to change to a better role."
            + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Change Role",
            "The Traitor can change their role to one of the provided role cards, or gamble on the random. Once they select a role, they stay as that role until they die. However, they must still be guessed as Traitor.",
            TOSImpAssets.TraitorSelect)
    ];

    public void Clear()
    {
        ChosenRoles.Clear();
        SelectedRole = null;
    }

    public void UpdateRole()
    {
        if (!SelectedRole)
        {
            return;
        }

        var currenttime = Player.killTimer;

        var roleType = RoleId.Get(SelectedRole!.GetType());
        Player.RpcChangeRole(roleType, false);
        Player.RpcAddModifier<TraitorCacheModifier>();
        SelectedRole = null;

        Player.SetKillTimer(currenttime);
    }
}