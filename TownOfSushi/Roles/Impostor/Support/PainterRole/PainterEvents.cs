using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Meeting;
using MiraAPI.Hud;

namespace TownOfSushi.Roles.Impostor;

public static class PainterEvents
{
    [RegisterEvent]
    public static void EjectionEventEventHandler(EjectionEvent @event)
    {
        var button = CustomButtonSingleton<PainterPaintButton>.Instance;
        button.SetUses((int)OptionGroupSingleton<PainterOptions>.Instance.MaxPaints);

        if ((int)OptionGroupSingleton<PainterOptions>.Instance.MaxPaints == 0)
        {
            button.Button?.usesRemainingText.gameObject.SetActive(false);
            button.Button?.usesRemainingSprite.gameObject.SetActive(false);
        }
        else
        {
            button.Button?.usesRemainingText.gameObject.SetActive(true);
            button.Button?.usesRemainingSprite.gameObject.SetActive(true);
        }
    }
}