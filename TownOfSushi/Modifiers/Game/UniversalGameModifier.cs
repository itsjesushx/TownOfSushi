﻿using MiraAPI.Modifiers;
using MiraAPI.Modifiers.Types;
using MiraAPI.PluginLoading;

namespace TownOfSushi.Modifiers.Game;

[MiraIgnore]
public abstract class UniversalGameModifier : GameModifier
{
    public virtual ModifierFaction FactionType => ModifierFaction.Universal;
    
    public virtual int CustomAmount => GetAmountPerGame();
    public virtual int CustomChance => GetAssignmentChance();

    public override bool HideOnUi => false;

    public override int GetAmountPerGame() => 1;

    public override bool IsModifierValidOn(RoleBehaviour role) => !role.Player.GetModifierComponent().HasModifier<UniversalGameModifier>(true);
}
