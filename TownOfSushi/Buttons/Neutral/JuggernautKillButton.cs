using MiraAPI.GameOptions;
using MiraAPI.Networking;
using TownOfSushi.Utilities;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities;
using TownOfSushi.Options.Roles.Neutral;
using TownOfSushi.Roles.Neutral;
using UnityEngine;

namespace TownOfSushi.Buttons.Neutral;

public sealed class JuggernautKillButton : TownOfSushiRoleButton<JuggernautRole, PlayerControl>, IDiseaseableButton, IKillButton
{
    public override string Name => "Kill";
    public override string Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Juggernaut;
    public override LoadableAsset<Sprite> Sprite => TosNeutAssets.JuggKillSprite;
    public override float Cooldown => GetCooldown();
    public void SetDiseasedTimer(float multiplier)
    {
        SetTimer(Cooldown * multiplier);
    }

    public static float BaseCooldown => OptionGroupSingleton<JuggernautOptions>.Instance.KillCooldown + MapCooldown;

    protected override void OnClick()
    {
        if (Target == null)
        {
            Logger<TownOfSushiPlugin>.Error("Juggernaut Shoot: Target is null");
            return;
        }

        PlayerControl.LocalPlayer.RpcCustomMurder(Target);
    }

    public override PlayerControl? GetTarget() => PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance);

    public static float GetCooldown()
    {
        var juggernaut = PlayerControl.LocalPlayer.Data.Role as JuggernautRole;

        if (juggernaut == null) return BaseCooldown;

        var options = OptionGroupSingleton<JuggernautOptions>.Instance;

        return Math.Max(BaseCooldown - (options.KillCooldownReduction * juggernaut.KillCount), 0);
    }
}
