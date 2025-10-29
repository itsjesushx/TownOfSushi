using MiraAPI.GameEnd;
using Reactor.Utilities.Extensions;
using TownOfSushi.Modules;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfSushi.GameOver;

public sealed class LoverGameOver : CustomGameOver
{
    public override bool VerifyCondition(PlayerControl playerControl, NetworkedPlayerInfo[] winners)
    {
        return winners.All(plr => plr.Object.HasModifier<LoverModifier>());
    }

    public override void AfterEndGameSetup(EndGameManager endGameManager)
    {
        endGameManager.BackgroundBar.material.SetColor(ShaderID.Color, TownOfSushiColors.Lover);

        var text = Object.Instantiate(endGameManager.WinText);
        text.text = "Love Couple Wins!";
        text.color = TownOfSushiColors.Lover;
        GameHistory.WinningFaction = $"<color=#{TownOfSushiColors.Lover.ToHtmlStringRGBA()}>Lovers</color>";

        var pos = endGameManager.WinText.transform.localPosition;
        pos.y = 1.5f;
        pos += Vector3.down * 0.15f;
        text.transform.localScale = new Vector3(1f, 1f, 1f);

        text.transform.position = pos;
        text.text = $"<size=4>{text.text}</size>";
    }
}