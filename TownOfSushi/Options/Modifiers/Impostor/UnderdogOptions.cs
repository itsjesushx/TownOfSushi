﻿using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.Utilities;
using TownOfSushi.Modifiers.Game.Impostor;
using UnityEngine;

namespace TownOfSushi.Options.Modifiers.Impostor;

public sealed class UnderdogOptions : AbstractOptionGroup<UnderdogModifier>
{
    public override string GroupName => "Underdog";
    public override Color GroupColor => Palette.ImpostorRoleHeaderRed;
    public override uint GroupPriority => 43;

    [ModdedNumberOption("Kill Cooldown Bonus", 2.5f, 10f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float KillCooldownIncrease { get; set; } = 5f;

    [ModdedToggleOption("Increased Kill Cooldown When 2+ Imps")]
    public bool ExtraImpsKillCooldown { get; set; } = false;
}