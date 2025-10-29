using AmongUs.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Networking;
using Reactor.Networking.Attributes;
using TownOfSushi.Modifiers;
using TownOfSushi.Modules;
using TownOfSushi.Buttons;
using UnityEngine;
using Il2CppInterop.Runtime.Attributes;

namespace TownOfSushi.Roles.Neutral;

public sealed class PyromaniacIgniteButton : TownOfSushiRoleButton<PyromaniacRole>
{
    public PlayerControl? ClosestTarget;
    public override string Name => "Ignite";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Pyromaniac;
    public override float Cooldown => OptionGroupSingleton<PyromaniacOptions>.Instance.DouseCooldown + MapCooldown;
    public override LoadableAsset<Sprite> Sprite => TOSNeutAssets.IgniteButtonSprite;

    private static List<PlayerControl> PlayersInRange => Helpers.GetClosestPlayers(PlayerControl.LocalPlayer,
        OptionGroupSingleton<PyromaniacOptions>.Instance.IgniteRadius.Value * ShipStatus.Instance.MaxLightRadius);

    [HideFromIl2Cpp]
    public Ignite? Ignite { get; set; }

    public override bool CanUse()
    {
        if (OptionGroupSingleton<PyromaniacOptions>.Instance.LegacyPyromaniac)
        {
            return base.CanUse() && ClosestTarget != null;
        }

        var count = PlayersInRange.Count(x => x.HasModifier<PyromaniacDousedModifier>());

        if (count > 0 && !PlayerControl.LocalPlayer.HasDied() && Timer <= 0)
        {
            var pos = PlayerControl.LocalPlayer.transform.position;
            pos.z += 0.001f;

            if (Ignite == null)
            {
                Ignite = Ignite.CreateIgnite(pos);
            }
            else
            {
                Ignite.Transform.localPosition = pos;
            }
        }
        else
        {
            if (Ignite != null)
            {
                Ignite.Clear();
                Ignite = null;
            }
        }

        return base.CanUse() && count > 0;
    }

    protected override void OnClick()
    {
        PlayerControl.LocalPlayer.RpcAddModifier<IndirectAttackerModifier>(false);
        var dousedPlayers = PlayersInRange.Where(x => x.HasModifier<PyromaniacDousedModifier>()).ToList();
        if (OptionGroupSingleton<PyromaniacOptions>.Instance.LegacyPyromaniac)
        {
            dousedPlayers = PlayerControl.AllPlayerControls.ToArray()
                .Where(x => x.HasModifier<PyromaniacDousedModifier>()).ToList();
        }

        foreach (var doused in dousedPlayers)
        {
            if (doused.HasModifier<FirstDeadShield>())
            {
                continue;
            }

            if (doused.HasModifier<BaseShieldModifier>())
            {
                continue;
            }

            PlayerControl.LocalPlayer.RpcCustomMurder(doused, resetKillTimer: false, teleportMurderer: false,
                playKillSound: false);
            RpcIgniteSound(doused);
        }

        PlayerControl.LocalPlayer.RpcRemoveModifier<IndirectAttackerModifier>();

        TOSAudio.PlaySound(TOSAudio.ArsoIgniteSound);

        CustomButtonSingleton<PyromaniacDouseButton>.Instance.ResetCooldownAndOrEffect();
    }

    protected override void FixedUpdate(PlayerControl playerControl)
    {
        base.FixedUpdate(playerControl);
        if (MeetingHud.Instance || !OptionGroupSingleton<PyromaniacOptions>.Instance.LegacyPyromaniac)
        {
            return;
        }

        var killDistances =
            GameOptionsManager.Instance.currentNormalGameOptions.GetFloatArray(FloatArrayOptionNames.KillDistances);
        ClosestTarget = PlayerControl.LocalPlayer.GetClosestLivingPlayer(true,
            killDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance],
            predicate: x => x.HasModifier<PyromaniacDousedModifier>());
    }

    [MethodRpc((uint)TownOfSushiRpc.IgniteSound, SendImmediately = true)]
    public static void RpcIgniteSound(PlayerControl player)
    {
        if (player.AmOwner)
        {
            TOSAudio.PlaySound(TOSAudio.ArsoIgniteSound);
        }
    }
}