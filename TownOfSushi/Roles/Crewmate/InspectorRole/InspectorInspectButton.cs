using TownOfSushi.Buttons;
using TownOfUs.Modules.Components;
using UnityEngine;

namespace TownOfSushi.Roles.Crewmate;

public sealed class InspectorInspectButton : TownOfSushiRoleButton<InspectorRole, CrimeSceneComponent>
{
    public override string Name => "Inspect";
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Inspector;
    public override float Cooldown => 1f + MapCooldown;
    public override LoadableAsset<Sprite> Sprite => TOSCrewAssets.InspectSprite;

    public override CrimeSceneComponent? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetNearestObjectOfType<CrimeSceneComponent>(Distance,
            Helpers.CreateFilter(Constants.NotShipMask));
    }

    public override void SetOutline(bool active)
    {
        // placeholder
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            return;
        }

        Role.InvestigatingScene = Target;
        Role.InvestigatedPlayers.AddRange(Target.GetScenePlayers());
        var notif1 = Helpers.CreateAndShowNotification(MiscUtils.ColorString(TownOfSushiColors.Inspector,
            $"<b>You have inspected the crime scene of {Target.DeadPlayer!.Data.PlayerName}. The killer or anyone that steps foot in the crime scene will flash red when examined.</b>"),
            Color.white, new Vector3(0f, 1f, -20f), spr: TOSRoleIcons.Inspector.LoadAsset());
        notif1.AdjustNotification();
        // TOSAudio.PlaySound(TOSAudio.QuestionSound);
    }
}