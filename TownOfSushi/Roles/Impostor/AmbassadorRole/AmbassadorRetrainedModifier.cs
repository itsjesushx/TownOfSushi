﻿using AmongUs.GameOptions;

namespace TownOfSushi.Roles.Impostor;

public sealed class AmbassadorRetrainedModifier(ushort role) : BaseModifier
{
    public override string ModifierName => "Retrained";
    public override bool HideOnUi => true;
    public RoleBehaviour PreviousRole => RoleManager.Instance.GetRole((RoleTypes)role);
}