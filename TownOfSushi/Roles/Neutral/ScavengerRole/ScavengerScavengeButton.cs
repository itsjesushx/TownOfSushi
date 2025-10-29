using HarmonyLib;
using Il2CppInterop.Runtime.Attributes;
using TownOfSushi.Buttons;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfSushi.Roles.Neutral;

public sealed class ScavengerButton : TownOfSushiButton
{
    public override string Name => "Scavenge";
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Scavenger;
    public override float Cooldown => OptionGroupSingleton<ScavengerOptions>.Instance.ScavengeCooldown + MapCooldown;
    public override float EffectDuration => OptionGroupSingleton<ScavengerOptions>.Instance.ScavengeDuration;
    public override int MaxUses => (int)OptionGroupSingleton<ScavengerOptions>.Instance.MaxScavengeUses;
    public override LoadableAsset<Sprite> Sprite => TOSNeutAssets.ScavengeSprite;

    public override bool Enabled(RoleBehaviour? role) => role is ScavengerRole;
    public override void CreateButton(Transform parent)
    {
        base.CreateButton(parent);

        Button!.usesRemainingSprite.sprite = TOSAssets.AbilityCounterBodySprite.LoadAsset();
    }
    protected override void OnClick()
    {
        var deadBodies = Object.FindObjectsOfType<DeadBody>().ToList();

        deadBodies.Do(x => PlayerControl.LocalPlayer.AddModifier<ScavengerArrowModifier>(x, TownOfSushiColors.Scavenger));
        if (deadBodies.Count == 0)
        {
            var notif1 = Helpers.CreateAndShowNotification($"<b>No bodies were found on the map.</b>", Color.white, new Vector3(0f, 1f, -20f), spr: TOSNeutAssets.ScavengeSprite.LoadAsset());
            notif1.AdjustNotification();
        }
    }
    public override void OnEffectEnd()
    {
        if (PlayerControl.LocalPlayer.HasModifier<ScavengerArrowModifier>())
        {
            var mods = PlayerControl.LocalPlayer.GetModifiers<ScavengerArrowModifier>();

            mods.Do([HideFromIl2Cpp](x) => PlayerControl.LocalPlayer.RemoveModifier(x.UniqueId));
        }
    }
}