using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities;
using TownOfSushi.Modifiers;
using TownOfSushi.Modifiers.Neutral;
using TownOfSushi.Modules;
using TownOfSushi.Options.Roles.Crewmate;
using TownOfSushi.Roles.Crewmate;
using TownOfSushi.Utilities;
using UnityEngine;

namespace TownOfSushi.Buttons.Crewmate;

public sealed class PlumberBlockButton : TownOfSushiRoleButton<PlumberRole, Vent>
{
    private static readonly ContactFilter2D Filter = Helpers.CreateFilter(Constants.Usables);
    public override string Name => "Block";
    public override string Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Plumber;
    public override float Cooldown => OptionGroupSingleton<PlumberOptions>.Instance.BlockCooldown + MapCooldown;
    public override int MaxUses => (int)OptionGroupSingleton<PlumberOptions>.Instance.MaxBarricades;
    public override LoadableAsset<Sprite> Sprite => TOSCrewAssets.BarricadeSprite;
    public int ExtraUses { get; set; }

    public override bool IsTargetValid(Vent? target)
    {
        return base.IsTargetValid(target) && !PlumberRole.VentsBlocked.Select(x => x.Key).Contains(target!.Id) &&
               !Role.FutureBlocks.Contains(target.Id);
    }

    public override Vent? GetTarget()
    {
        var vent = PlayerControl.LocalPlayer.GetNearestObjectOfType<Vent>(Distance / 4, Filter);
        if (vent == null)
        {
            vent = PlayerControl.LocalPlayer.GetNearestObjectOfType<Vent>(Distance / 3, Filter);
        }

        if (vent == null)
        {
            vent = PlayerControl.LocalPlayer.GetNearestObjectOfType<Vent>(Distance / 2, Filter);
        }

        if (vent == null)
        {
            vent = PlayerControl.LocalPlayer.GetNearestObjectOfType<Vent>(Distance, Filter);
        }

        if (ModCompatibility.IsSubmerged() && vent != null && (vent.Id == 0 || vent.Id == 14))
        {
            vent = null;
        }

        if (vent != null && PlayerControl.LocalPlayer.CanUseVent(vent))
        {
            return vent;
        }

        return null;
    }

    public override bool CanUse()
    {
        var newTarget = GetTarget();
        if (newTarget != Target)
        {
            Target?.SetOutline(false, false);
        }

        Target = IsTargetValid(newTarget) ? newTarget : null;
        SetOutline(true);

        return Timer <= 0 && Target != null
                          && !PlayerControl.LocalPlayer.HasModifier<GlitchHackedModifier>()
                          && !PlayerControl.LocalPlayer.HasModifier<DisabledModifier>()
                          && UsesLeft > 0;
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            Logger<TownOfSushiPlugin>.Error($"{Name}: Target is null");
            return;
        }

        var notif1 = Helpers.CreateAndShowNotification(
            $"<b>{TownOfSushiColors.Plumber.ToTextColor()}This vent will be blocked at the beginning of the next round.</b></color>",
            Color.white, new Vector3(0f, 1f, -20f), spr: TOSRoleIcons.Plumber.LoadAsset());
        notif1.Text.SetOutlineThickness(0.35f);

        PlumberRole.RpcPlumberBlockVent(PlayerControl.LocalPlayer, Target.Id);

        var flush = CustomButtonSingleton<PlumberFlushButton>.Instance;

        flush?.SetTimer(flush.Cooldown);
    }
}