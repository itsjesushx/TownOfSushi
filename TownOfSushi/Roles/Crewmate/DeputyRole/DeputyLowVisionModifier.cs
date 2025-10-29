using UnityEngine;

namespace TownOfSushi.Roles.Crewmate;

public sealed class DeputyLowVisionModifier(PlayerControl Deputy) : BaseModifier
{
    public override string ModifierName => "Low Visioned";
    public override LoadableAsset<Sprite>? ModifierIcon => TOSRoleIcons.Deputy;
    public override string GetDescription() => "Your vision is lower because you missed a shot!\nVision will go back to normal after this meeting.";
    public PlayerControl Deputy { get; } = Deputy;
}