using System.Text;
using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Events;
using MiraAPI.GameOptions;
using MiraAPI.Roles;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using TownOfSushi.Events.TosEvents;
using TownOfSushi.Modules;
using TownOfSushi.Modules.Wiki;
using TownOfSushi.Options.Roles.Impostor;
using TownOfSushi.Roles.Crewmate;
using TownOfSushi.Utilities;
using UnityEngine;

namespace TownOfSushi.Roles.Impostor;

public sealed class BomberRole(IntPtr cppPtr) : ImpostorRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable, IDoomable, ICrewVariant
{
    public string RoleName => "Bomber";
    public string RoleDescription => "Plant Bombs To Kill Multiple Crewmates At Once";
    public string RoleLongDescription => "Plant bombs to kill several crewmates at once";
    public RoleBehaviour CrewVariant => RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<TrapperRole>());
    public Color RoleColor => TownOfSushiColors.Impostor;
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;
    public RoleAlignment RoleAlignment => RoleAlignment.ImpostorKilling;
    public DoomableType DoomHintType => DoomableType.Relentless;
    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = TosRoleIcons.Bomber,
        CanUseVent = OptionGroupSingleton<BomberOptions>.Instance.BomberVent,
    };

    [HideFromIl2Cpp]
    public Bomb? Bomb { get; set; }

    [MethodRpc((uint)TownOfSushiRpc.PlantBomb, SendImmediately = true)]
    public static void RpcPlantBomb(PlayerControl player, Vector2 position)
    {
        if (player.Data.Role is not BomberRole role)
        {
            Logger<TownOfSushiPlugin>.Error("RpcPlantBomb - Invalid bomber");
            return;
        }

        var TosAbilityEvent = new TosAbilityEvent(AbilityType.BomberPlant, player);
        MiraEventManager.InvokeEvent(TosAbilityEvent);

        if (player.AmOwner)
        {
            role.Bomb = Bomb.CreateBomb(player, position);
        }
        else if (OptionGroupSingleton<BomberOptions>.Instance.AllImpsSeeBomb && PlayerControl.LocalPlayer.IsImpostor())
        {
            Bomb.BombShowTeammate(player, position);
        }
    }

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ITownOfSushiRole.SetNewTabText(this);
    }
    public string GetAdvancedDescription()
    {
        return $"The Bomber is an Impostor Killing role that can drop a bomb on the map, which detonates after {OptionGroupSingleton<BomberOptions>.Instance.DetonateDelay} second(s)" + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
        [
            new("Place", 
                $"Place a bomb, showing the radius in which it'll kill, killing up to {(int)OptionGroupSingleton<BomberOptions>.Instance.MaxKillsInDetonation} player(s)",
                TosImpAssets.PlaceSprite)
        ];
}
