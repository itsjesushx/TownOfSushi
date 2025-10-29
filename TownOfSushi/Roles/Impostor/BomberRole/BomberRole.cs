using System.Text;
using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Events;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using TownOfSushi.Events.TOSEvents;
using TownOfSushi.Modules;
using UnityEngine;

namespace TownOfSushi.Roles.Impostor;

public sealed class BomberRole(IntPtr cppPtr)
    : ImpostorRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable, ICrewVariant, IMysticClue
{
    [HideFromIl2Cpp] public Bomb? Bomb { get; set; }
    public RoleBehaviour CrewVariant => RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<TrapperRole>());

    public string RoleName => "Bomber";
    public string RoleDescription => "Plant Bombs To Kill Multiple Crewmates At Once";
    public string RoleLongDescription => "Plant bombs to kill several crewmates at once";
    public MysticClueType MysticHintType => MysticClueType.Relentless;
    public Color RoleColor => TownOfSushiColors.Impostor;
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;
    public RoleAlignment RoleAlignment => RoleAlignment.ImpostorKilling;

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = TOSRoleIcons.Bomber,
        CanUseVent = OptionGroupSingleton<BomberOptions>.Instance.BomberVent
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ITownOfSushiRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            $"The {RoleName} is an Impostor Killing role that can drop a bomb on the map, which detonates after {OptionGroupSingleton<BomberOptions>.Instance.DetonateDelay} second(s)" +
            MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Place",
            $"Place a bomb, showing the radius in which it'll kill, killing up to {(int)OptionGroupSingleton<BomberOptions>.Instance.MaxKillsInDetonation} player(s)",
            TOSImpAssets.PlaceSprite)
    ];

    [MethodRpc((uint)TownOfSushiRpc.PlantBomb, SendImmediately = true)]
    public static void RpcPlantBomb(PlayerControl player, Vector2 position)
    {
        if (player.Data.Role is not BomberRole role)
        {
            Logger<TownOfSushiPlugin>.Error("RpcPlantBomb - Invalid bomber");
            return;
        }

        var TOSAbilityEvent = new TOSAbilityEvent(AbilityType.BomberPlant, player);
        MiraEventManager.InvokeEvent(TOSAbilityEvent);

        if (player.AmOwner)
        {
            role.Bomb = Bomb.CreateBomb(player, position);
        }
        else if (OptionGroupSingleton<BomberOptions>.Instance.AllImpsSeeBomb && PlayerControl.LocalPlayer.IsImpostor())
        {
            Coroutines.Start(Bomb.BombShowTeammate(player, position));
        }
    }
}