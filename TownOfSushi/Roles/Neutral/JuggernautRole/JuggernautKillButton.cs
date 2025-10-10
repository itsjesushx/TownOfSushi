using MiraAPI.Networking;
using Reactor.Utilities;
using TownOfSushi.Buttons;
using UnityEngine;

namespace TownOfSushi.Roles.Neutral;

public sealed class JuggernautKillButton : TownOfSushiRoleButton<JuggernautRole, PlayerControl>, IDiseaseableButton,
    IKillButton
{
    public override string Name => "Kill";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Juggernaut;
    public override LoadableAsset<Sprite> Sprite => TOSNeutAssets.JuggKillSprite;
    public override float Cooldown => GetCooldown();

    public static float BaseCooldown => OptionGroupSingleton<JuggernautOptions>.Instance.KillCooldown + MapCooldown;

    public void SetDiseasedTimer(float multiplier)
    {
        SetTimer(Cooldown * multiplier);
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            Logger<TownOfSushiPlugin>.Error("Juggernaut Shoot: Target is null");
            return;
        }

        PlayerControl.LocalPlayer.RpcCustomMurder(Target);
    }

    public override PlayerControl? GetTarget()
    {
        if (!OptionGroupSingleton<LoversOptions>.Instance.LoversKillEachOther && PlayerControl.LocalPlayer.IsLover())
        {
            return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance, false, x => !x.IsLover());
        }
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance);
    }

    public static float GetCooldown()
    {
        var juggernaut = PlayerControl.LocalPlayer.Data.Role as JuggernautRole;

        if (juggernaut == null)
        {
            return BaseCooldown;
        }

        var options = OptionGroupSingleton<JuggernautOptions>.Instance;

        return Math.Max(BaseCooldown - options.KillCooldownReduction * juggernaut.KillCount, 0);
    }
}