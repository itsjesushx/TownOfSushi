using MiraAPI.Events;
using TownOfSushi.Events.TOSEvents;

namespace TownOfSushi.Roles.Impostor;

public sealed class WarlockCursedModifier(byte warlockId) : BaseModifier
{
    public override string ModifierName => "Warlock Cursed";
    public override bool HideOnUi => true;
    public override void OnActivate()
    {
        var wrlck = PlayerControl.AllPlayerControls.ToArray().FirstOrDefault(x => x.PlayerId == WarlockId);
        var TOSAbilityEvent = new TOSAbilityEvent(AbilityType.WarlockCurse, wrlck!, Player);
        MiraEventManager.InvokeEvent(TOSAbilityEvent);
    }
    public byte WarlockId { get; } = warlockId;
    public override void OnDeath(DeathReason reason)
    {
        ModifierComponent!.RemoveModifier(this);
    }
}