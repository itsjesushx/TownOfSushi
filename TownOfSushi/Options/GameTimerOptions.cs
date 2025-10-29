using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;

namespace TownOfSushi.Options;

public sealed class GameTimerOptions : AbstractOptionGroup
{
    public override string GroupName => "End Game Timer";
    public override uint GroupPriority => 3;

    [ModdedToggleOption("Game Timer")] public bool GameTimerEnabled { get; set; } = false;

    public ModdedEnumOption TimerEndOption { get; } =
        new("On Timer End", 1, typeof(GameTimerType), ["Impostor Win", "Game Draw"])
        {
            Visible = () => OptionGroupSingleton<GameTimerOptions>.Instance.GameTimerEnabled
        };

    public ModdedNumberOption GameTimeLimit { get; } =
        new("Game Time Limit", 15f, 1f, 30f, 0.5f, MiraNumberSuffixes.None, "0.0m")
        {
            Visible = () => OptionGroupSingleton<GameTimerOptions>.Instance.GameTimerEnabled
        };
}

public enum GameTimerType
{
    Impostors,
    GameDraw
}