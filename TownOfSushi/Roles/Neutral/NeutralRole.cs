﻿using Il2CppSystem.Text;

namespace TownOfSushi.Roles.Neutral;

public abstract class NeutralRole(IntPtr cppPtr) : RoleBehaviour(cppPtr)
{
    public override bool IsDead => false; // needed because we inherit from RoleBehaviour
    public override bool IsAffectedByComms => false;

#pragma warning disable S927 // Parameter names should match base declaration and other partial definitions
#pragma warning disable CA1725 // Parameter names should match base declaration
    public override bool CanUse(IUsable usable)
#pragma warning restore CA1725 // Parameter names should match base declaration
#pragma warning restore S927 // Parameter names should match base declaration and other partial definitions
    {
        return GameManager.Instance.LogicUsables.CanUse(usable, Player);
    }

    public override void AppendTaskHint(StringBuilder taskStringBuilder)
    {
        // remove default task hint
    }
}