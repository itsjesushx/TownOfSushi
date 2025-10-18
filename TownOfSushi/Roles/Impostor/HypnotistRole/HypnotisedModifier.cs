using MiraAPI.Events;
using TownOfSushi.Events.TOSEvents;
using TownOfSushi.Utilities.Appearances;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TownOfSushi.Roles.Impostor;

public sealed class HypnotisedModifier(PlayerControl hypnotist) : BaseModifier
{
    private List<PlayerControl> players = [];
    public override string ModifierName => "Hypnotised";
    public override bool HideOnUi => true;
    public PlayerControl Hypnotist { get; } = hypnotist;

    public bool HysteriaActive { get; set; }

    public override void OnDeath(DeathReason reason)
    {
        ModifierComponent!.RemoveModifier(this);
    }

    public override void OnActivate()
    {
        base.OnActivate();
        var TOSAbilityEvent = new TOSAbilityEvent(AbilityType.HypnotistHypno, Hypnotist, Player);
        MiraEventManager.InvokeEvent(TOSAbilityEvent);
    }

    public override void OnDeactivate()
    {
        UnHysteria();

        players.Clear();
    }

    public void Hysteria()
    {
        if (Player.HasDied())
        {
            return;
        }

        var TOSAbilityEvent = new TOSAbilityEvent(AbilityType.HypnotistHysteria, Hypnotist, Player);
        MiraEventManager.InvokeEvent(TOSAbilityEvent);
        if (!Player.AmOwner)
        {
            return;
        }

        if (HysteriaActive)
        {
            return;
        }

        // Logger<TownOfSushiPlugin>.Message($"HypnotisedModifier.Hysteria - {Player.Data.PlayerName}");
        players = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.HasDied() && x != Player).ToList();

        foreach (var player in players)
        {
            var hidden = Random.RandomRangeInt(0, 3);
            if (hidden == 0)
            {
                var morph = new VisualAppearance(Player.GetDefaultModifiedAppearance(), TownOfSushiAppearances.Morph);

                player?.RawSetAppearance(morph);
            }
            else if (hidden == 1)
            {
                player.SetCamouflage();
            }
            else
            {
                var swoop = new VisualAppearance(player.GetDefaultModifiedAppearance(), TownOfSushiAppearances.Swooper)
                {
                    HatId = string.Empty,
                    SkinId = string.Empty,
                    VisorId = string.Empty,
                    PlayerName = string.Empty,
                    PetId = string.Empty,
                    RendererColor = Color.clear,
                    NameColor = Color.clear,
                    ColorBlindTextColor = Color.clear
                };

                player.RawSetAppearance(swoop);
            }

            player?.cosmetics.ToggleNameVisible(false);
        }

        if (Player.AmOwner)
        {
            var notif1 = Helpers.CreateAndShowNotification(MiscUtils.ColorString(TownOfSushiColors.ImpSoft,
                $"<b>You are under a Mass Hysteria!</b>"), Color.white,
                spr: TOSRoleIcons.Hypnotist.LoadAsset());

            
            notif1.AdjustNotification();
        }

        HysteriaActive = true;
    }

    public void UnHysteria()
    {
        if (!Player.AmOwner)
        {
            return;
        }

        if (!HysteriaActive)
        {
            return;
        }

        // Logger<TownOfSushiPlugin>.Message($"HypnotisedModifier.UnHysteria - {Player.Data.PlayerName}");
        foreach (var player in players)
        {
            Player.ResetAppearance();
            player?.cosmetics.ToggleNameVisible(true);
        }

        HysteriaActive = false;
    }
}