using HarmonyLib;
using MiraAPI.Hud;
using TownOfSushi.Buttons;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfSushi.Modifiers.Game.Universal;

public sealed class SatelliteButton : TownOfSushiButton
{
    public override string Name => "Broadcast";
    public override BaseKeybind Keybind => Keybinds.ModifierAction;
    public override Color TextOutlineColor => TownOfSushiColors.Satellite;
    public override float Cooldown => OptionGroupSingleton<SatelliteOptions>.Instance.Cooldown + MapCooldown;
    public override int MaxUses => (int)OptionGroupSingleton<SatelliteOptions>.Instance.MaxNumCast;
    public override ButtonLocation Location => ButtonLocation.BottomLeft;
    public override LoadableAsset<Sprite> Sprite => TOSAssets.BroadcastSprite;

    public bool Usable { get; set; } = OptionGroupSingleton<SatelliteOptions>.Instance.FirstRoundUse ||
                                       TutorialManager.InstanceExists;

    public override bool Enabled(RoleBehaviour? role)
    {
        return PlayerControl.LocalPlayer != null &&
               PlayerControl.LocalPlayer.HasModifier<SatelliteModifier>() &&
               !PlayerControl.LocalPlayer.Data.IsDead;
    }

    public override bool CanUse()
    {
        return base.CanUse() && Usable;
    }

    public override void CreateButton(Transform parent)
    {
        base.CreateButton(parent);

        Button!.usesRemainingSprite.sprite = TOSAssets.AbilityCounterBodySprite.LoadAsset();
    }

    protected override void OnClick()
    {
        var deadBodies = Object.FindObjectsOfType<DeadBody>().ToList();

        deadBodies.Do(x => PlayerControl.LocalPlayer.AddModifier<SatelliteArrowModifier>(x, Color.white));
        if (deadBodies.Count == 0)
        {
            var notif1 = Helpers.CreateAndShowNotification("<b>No bodies were found on the map.</b>", Color.white,
                new Vector3(0f, 1f, -20f), spr: TOSModifierIcons.Satellite.LoadAsset());
            notif1.AdjustNotification();
        }

        if (OptionGroupSingleton<SatelliteOptions>.Instance.OneUsePerRound)
        {
            Usable = false;
        }
        // will return to this once i get more freetime
        //deadBodies.Do(x => PlayerControl.LocalPlayer.GetModifier<SatelliteModifier>().NewMapIcon(MiscUtils.PlayerById(x.ParentId)));
    }
}