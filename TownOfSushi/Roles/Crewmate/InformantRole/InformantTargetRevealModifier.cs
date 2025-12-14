﻿using TownOfSushi.Modifiers;
using UnityEngine;

namespace TownOfSushi.Roles.Crewmate;

public sealed class InformantTargetRevealModifier()
    : RevealModifier((int)ChangeRoleResult.Nothing, true, null!)
{
    public override string ModifierName => "Revealed Killer";

    public override void OnActivate()
    {
        base.OnActivate();
        SetNewInfo(false, null, null, null, Color.red);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        NameColor = Color.red;
    }
}