using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using TownOfUs.Modules.Components;
using TownOfSushi.Roles.Crewmate;
using UnityEngine;

namespace TownOfSushi.Buttons.Crewmate;

public sealed class DetectiveInspectButton : TownOfSushiRoleButton<DetectiveRole, CrimeSceneComponent>
{
    public override string Name => "Inspect";
    public override string Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Detective;
    public override float Cooldown => 1f + MapCooldown;
    public override LoadableAsset<Sprite> Sprite => TosCrewAssets.InspectSprite;

    public override CrimeSceneComponent? GetTarget() => PlayerControl.LocalPlayer.GetNearestObjectOfType<CrimeSceneComponent>(Distance, Helpers.CreateFilter(Constants.NotShipMask));

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
        var notif1 = Helpers.CreateAndShowNotification($"<b>{TownOfSushiColors.Detective.ToTextColor()}You have inspected the crime scene of {Target.DeadPlayer!.Data.PlayerName}. The killer or anyone that steps foot in the crime scene will flash red when examined.</b></color>", Color.white, new Vector3(0f, 1f, -20f), spr: TosRoleIcons.Detective.LoadAsset());
        notif1.Text.SetOutlineThickness(0.35f);
        // TosAudio.PlaySound(TosAudio.QuestionSound);
    }
}
