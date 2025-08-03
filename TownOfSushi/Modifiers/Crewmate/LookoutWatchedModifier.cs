﻿using MiraAPI.Events;
using MiraAPI.Modifiers;
using Reactor.Utilities.Extensions;
using System.Text;
using TownOfSushi.Events.TosEvents;
using TownOfSushi.Utilities;
using UnityEngine;

namespace TownOfSushi.Modifiers.Crewmate;

public sealed class LookoutWatchedModifier(PlayerControl lookout) : BaseModifier
{
    public override string ModifierName => "Watched";
    public override bool HideOnUi => true;

    public PlayerControl Lookout { get; set; } = lookout;
    public List<RoleBehaviour> SeenPlayers { get; set; } = [];

    public override void OnActivate()
    {
        base.OnActivate();

        var TosAbilityEvent = new TosAbilityEvent(AbilityType.LookoutWatch, Lookout, Player);
        MiraEventManager.InvokeEvent(TosAbilityEvent);
    }
    public override void OnDeath(DeathReason reason)
    {
        ModifierComponent!.RemoveModifier(this);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if (Lookout.AmOwner)
        {
            Player?.cosmetics.SetOutline(true, new Il2CppSystem.Nullable<Color>(TownOfSushiColors.Lookout));
        }
    }

    public override void OnMeetingStart()
    {
        if (!Lookout.AmOwner) return;
        var title = $"<color=#{TownOfSushiColors.Lookout.ToHtmlStringRGBA()}>Lookout Feedback</color>";
        var msg = $"No players interacted with {Player.Data.PlayerName}";

        if (SeenPlayers.Count != 0)
        {
            var message = new StringBuilder($"Roles seen interacting with {Player.Data.PlayerName}:\n");

            SeenPlayers.Shuffle();

            foreach (var role in SeenPlayers)
            {
                message.Append(TownOfSushiPlugin.Culture, $"{role.NiceName}, ");
            }

            message = message.Remove(message.Length - 2, 2);

            var final = message.ToString();

            if (string.IsNullOrWhiteSpace(final))
                return;

            msg = final;
        }

        MiscUtils.AddFakeChat(Player.Data, title, msg, false, true);

        SeenPlayers.Clear();
    }
}
