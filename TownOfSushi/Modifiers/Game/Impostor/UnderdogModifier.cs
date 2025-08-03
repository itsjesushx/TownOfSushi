﻿using AmongUs.GameOptions;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Utilities.Assets;
using TownOfUs.Modules.Wiki;
using TownOfSushi.Options.Modifiers;
using TownOfSushi.Options.Modifiers.Impostor;
using TownOfSushi.Utilities;
using UnityEngine;

namespace TownOfSushi.Modifiers.Game.Impostor;

public sealed class UnderdogModifier : TOSGameModifier, IWikiDiscoverable
{
    public override string ModifierName => "Underdog";
    public override string IntroInfo => "Your kill cooldown is also faster when you're on your own.";
    public override Color FreeplayFileColor => new Color32(255, 25, 25, 255);

    public override LoadableAsset<Sprite>? ModifierIcon => TOSModifierIcons.Underdog;
    public override ModifierFaction FactionType => ModifierFaction.ImpostorPassive;

    private static float KillCooldownIncrease => OptionGroupSingleton<UnderdogOptions>.Instance.KillCooldownIncrease;
    private static bool ExtraImpsKillCooldown => OptionGroupSingleton<UnderdogOptions>.Instance.ExtraImpsKillCooldown;

    public string GetAdvancedDescription()
    {
        return
            "Your kill cooldown is lower if you're solo or your teammate is dead."
            + MiscUtils.AppendOptionsText(GetType());
    }

    public List<CustomButtonWikiDescription> Abilities { get; } = [];

    public override string GetDescription()
    {
        return "When you're alone your kill cooldown is shortened";
    }

    public override int GetAssignmentChance()
    {
        return (int)OptionGroupSingleton<ImpostorModifierOptions>.Instance.UnderdogChance;
    }

    public override int GetAmountPerGame()
    {
        return (int)OptionGroupSingleton<ImpostorModifierOptions>.Instance.UnderdogAmount;
    }

    public override bool IsModifierValidOn(RoleBehaviour role)
    {
        return base.IsModifierValidOn(role) && role.IsImpostor();
    }

    public static bool LastImpostor()
    {
        return MiscUtils.ImpAliveCount <= 1;
    }

    public static float GetKillCooldown(PlayerControl player)
    {
        var mod = player.GetModifier<UnderdogModifier>();

        if (mod == null)
        {
            return GameOptionsManager.Instance.CurrentGameOptions.GetFloat(FloatOptionNames.KillCooldown);
        }

        var baseKillCooldown = GameOptionsManager.Instance.CurrentGameOptions.GetFloat(FloatOptionNames.KillCooldown);

        var lowerKc = baseKillCooldown - KillCooldownIncrease;
        var upperKc = baseKillCooldown + KillCooldownIncrease;

        var kc = ExtraImpsKillCooldown ? upperKc : baseKillCooldown;
        var timer = LastImpostor() ? lowerKc : kc;

        // Logger<TownOfSushiPlugin>.Error($"GetKillCooldown({player.Data.PlayerName}) baseKillCooldown: {baseKillCooldown}, baseKillCooldown2: {baseKillCooldown2}, lowerKc {lowerKc}, upperKc {upperKc}, kc {kc}, timer {timer}");

        return timer;
    }
}