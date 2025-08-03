using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using TownOfSushi.Roles.Neutral;
using UnityEngine;
using MiraAPI.Hud;
using TownOfSushi.Utilities;
using TownOfSushi.Modules;

namespace TownOfSushi.Events.Neutral;

public static class AmnesiacEvents
{
    [RegisterEvent]
    public static void RoundStartEventHandler(RoundStartEvent @event)
    {
        if (@event.TriggeredByIntro) return;

        if (!PlayerControl.LocalPlayer.IsRole<AmnesiacRole>()) return;

        var playerMenu = CustomPlayerMenu.Create();
        playerMenu.transform.FindChild("PhoneUI").GetChild(0).GetComponent<SpriteRenderer>().material = PlayerControl.LocalPlayer.cosmetics.currentBodySprite.BodySprite.material;
        playerMenu.transform.FindChild("PhoneUI").GetChild(1).GetComponent<SpriteRenderer>().material = PlayerControl.LocalPlayer.cosmetics.currentBodySprite.BodySprite.material;
        playerMenu.Begin(
            plr => (plr.HasDied() || UnityEngine.Object.FindObjectsOfType<DeadBody>().FirstOrDefault(x => x.ParentId == plr.PlayerId) ||
            FakePlayer.FakePlayers.FirstOrDefault(x => x?.body?.name == $"Fake {plr.gameObject.name}")?.body) && plr != PlayerControl.LocalPlayer,
            plr =>
            {
                playerMenu.ForceClose();

                if (plr != null)
                {
                    var targetId = plr.PlayerId;
                    var targetPlayer = MiscUtils.PlayerById(targetId);

                    if (targetPlayer == null) return;

                    AmnesiacRole.RpcRemember(PlayerControl.LocalPlayer, targetPlayer);
                }
            });
        foreach (var panel in playerMenu.potentialVictims)
        {
            panel.PlayerIcon.cosmetics.SetPhantomRoleAlpha(1f);
        }
    }
}