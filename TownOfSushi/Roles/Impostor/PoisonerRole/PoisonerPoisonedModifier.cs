namespace TownOfSushi.Roles.Impostor;

public sealed class PoisonerPoisonedModifier(byte witchId) : BaseModifier
{
    public override string ModifierName => "Poisoned";
    public byte WitchId { get; } = witchId;
    public override void OnDeath(DeathReason reason)
    {
        Player.RemoveModifier(this);
    }

    public override void OnMeetingStart()
    {
        var poisoner = PlayerControl.AllPlayerControls
            .ToArray().FirstOrDefault(p => !p.HasDied() && p.Data.Role is PoisonerRole);

        if (!Player.HasDied() && poisoner != null)
        {
            PoisonerRole.RpcMurderPoisoned(poisoner, Player);
        }
    }
}