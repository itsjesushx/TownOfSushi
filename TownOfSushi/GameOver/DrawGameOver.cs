using MiraAPI.GameEnd;
using Reactor.Utilities.Extensions;
using TownOfSushi.Modules;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfSushi.GameOver;

public sealed class DrawGameOver : CustomGameOver
{
    public override bool VerifyCondition(PlayerControl playerControl, NetworkedPlayerInfo[] winners)
    {
        return true;
    }

    public override void AfterEndGameSetup(EndGameManager endGameManager)
    {
        endGameManager.BackgroundBar.material.SetColor(ShaderID.Color, TownOfSushiColors.Neutral);

        var text = Object.Instantiate(endGameManager.WinText);
        text.text = "Draw Game!";
        text.color = TownOfSushiColors.Neutral;
        GameHistory.WinningFaction = $"<color=#{TownOfSushiColors.Neutral.ToHtmlStringRGBA()}>Nobody</color>";

        var pos = endGameManager.WinText.transform.localPosition;
        pos.y = 1.5f;
        pos += Vector3.down * 0.15f;
        text.transform.localScale = new Vector3(1f, 1f, 1f);

        text.transform.position = pos;
        text.text = $"<size=4>{text.text}</size>";
    }
}