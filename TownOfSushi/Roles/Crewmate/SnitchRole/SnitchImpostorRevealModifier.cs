using TownOfSushi.Modifiers;
using UnityEngine;

namespace TownOfSushi.Roles.Crewmate;

public sealed class SnitchImpostorRevealModifier()
    : RevealModifier((int)ChangeRoleResult.Nothing, true, null!)
{
    public override string ModifierName => "Revealed Impostor";
    
    public override void OnActivate()
    {
        base.OnActivate();
        SetNewInfo(false, null,null, null, Color.red);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        if (!OptionGroupSingleton<SnitchOptions>.Instance.SnitchSeesImpostorsMeetings)
        {
            Visible = !MeetingHud.Instance;
        }

        if (Player.IsImpostor())
        {
            NameColor = Color.red;
        }
        else if (!Player.HasDied())
        {
            NameColor = Player.Data.Role.TeamColor;
        }
    }
}