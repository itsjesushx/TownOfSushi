﻿using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.Utilities;
using TownOfSushi.Roles.Impostor;

namespace TownOfSushi.Options.Roles.Impostor;

public sealed class BlackmailerOptions : AbstractOptionGroup<BlackmailerRole>
{
    public override string GroupName => "Blackmailer";

    [ModdedNumberOption("Number Of Blackmail Uses Per Game", 0f, 15f, 5f, MiraNumberSuffixes.None, "0", zeroInfinity: true)]
    public float MaxBlackmails { get; set; } = 0f;

    [ModdedNumberOption("Blackmail Cooldown", 1f, 30f, suffixType: MiraNumberSuffixes.Seconds)]
    public float BlackmailCooldown { get; set; } = 20f;

    [ModdedNumberOption("Max Players Alive Until Voting", 1f, 15f)]
    public float MaxAliveForVoting { get; set; } = 10f;
    [ModdedToggleOption("Blackmail Same Person Twice In A Row")]
    public bool BlackmailInARow { get; set; } = false;

    [ModdedToggleOption("Only Target Sees Blackmail")]
    public bool OnlyTargetSeesBlackmail { get; set; } = false;
    [ModdedToggleOption("Blackmailer Can Kill With Teammate")]
    public bool BlackmailerKill { get; set; } = true;
}
