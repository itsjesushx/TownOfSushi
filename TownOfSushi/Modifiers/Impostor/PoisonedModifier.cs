using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using TownOfSushi.Modules;
using TownOfSushi.Options.Roles.Impostor;
using TownOfSushi.Roles.Impostor;
using TownOfSushi.Utilities;

namespace TownOfSushi.Modifiers.Impostor;

public sealed class PoisonModifier : ConcealedModifier
{
    public override string ModifierName => "Poisoned";
    public override float Duration => OptionGroupSingleton<ViperOptions>.Instance.PoisonDelay;

    public override void OnDeath(DeathReason reason)
    {
        Player.RemoveModifier(this);
    }

    public override void OnMeetingStart()
    {
        var viper = PlayerControl.AllPlayerControls
            .ToArray().FirstOrDefault(p => !p.HasDied() && p.GetRoleWhenAlive() is ViperRole);

        if (viper != null)
        {
            ViperRole.RpcMurderPoisoned(viper, Player);
        }

        Player.RemoveModifier(this);
    }
}