namespace TownOfSushi.Roles.Impostor;

public sealed class ViperPoisonedModifier(byte witchId) : BaseModifier
{
    public override string ModifierName => "Poisoned";
    public byte WitchId { get; } = witchId;
    public override void OnDeath(DeathReason reason)
    {
        Player.RemoveModifier(this);
    }

    public override void OnMeetingStart()
    {
        var viper = PlayerControl.AllPlayerControls
            .ToArray().FirstOrDefault(p => !p.HasDied() && p.Data.Role is ViperRole);

        if (!Player.HasDied() && viper != null)
        {
            ViperRole.RpcMurderPoisoned(viper, Player);
        }
    }
}