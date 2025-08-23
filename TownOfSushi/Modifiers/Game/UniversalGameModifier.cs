﻿using MiraAPI.Modifiers.Types;
using MiraAPI.PluginLoading;

namespace TownOfSushi.Modifiers.Game;

[MiraIgnore]
public abstract class UniversalGameModifier : GameModifier
{
    public virtual string IntroInfo => $"Modifier: {ModifierName}";
    public virtual float IntroSize => 3f;
    public virtual ModifierFaction FactionType => ModifierFaction.Universal;

    public virtual int CustomAmount => GetAmountPerGame();
    public virtual int CustomChance => GetAssignmentChance();

    public override bool HideOnUi => false;

    public override int GetAmountPerGame()
    {
        return 1;
    }

    public override bool IsModifierValidOn(RoleBehaviour role)
    {
        return !role.Player.GetModifierComponent().HasModifier<UniversalGameModifier>(true);
    }
}