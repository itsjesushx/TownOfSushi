using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Events.Vanilla.Meeting;

namespace TownOfSushi.Roles.Impostor;

public static class WarlockEvents
{
    [RegisterEvent]
    public static void EjectionEvent(EjectionEvent @event)
    {
        var exiled = @event.ExileController?.initData?.networkedPlayer?.Object;

        if (exiled == null)
        {
            return;
        }

        if (exiled.HasModifier<WarlockCursedModifier>())
        {
            exiled.RpcRemoveModifier<WarlockCursedModifier>();
        }
    }

    [RegisterEvent]
    public static void AfterMurderEvent(AfterMurderEvent @event)
    {
        var target = @event.Target;

        if (target == null)
        {
            return;
        }
        if (target.HasModifier<WarlockCursedModifier>())
        {
            target.RpcRemoveModifier<WarlockCursedModifier>();
        }
    }
}