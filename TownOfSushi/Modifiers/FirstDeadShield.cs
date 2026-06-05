using MiraAPI.LocalSettings;
using TownOfSushi.Modules.Anims;
using TownOfSushi.Options;
using TownOfSushi.Patches;
using UnityEngine;

namespace TownOfSushi.Modifiers;

public sealed class FirstDeadShield : ExcludedGameModifier, IAnimated
{
    public override string ModifierName => "First Death Shield";
    public override LoadableAsset<Sprite>? ModifierIcon => TownOfSushiAssets.FirstRoundShield;

    public override bool HideOnUi => !LocalSettingsTabSingleton<TownOfSushiLocalSettings>.Instance.ShowShieldHudToggle.Value;
    public override Color FreeplayFileColor => new Color32(100, 220, 100, 255);
    public bool IsVisible { get; set; } = true;

    public void SetVisible()
    {
    }
    public override void FixedUpdate()
    {
        Player?.cosmetics.SetOutline(true, new Il2CppSystem.Nullable<Color>(Color.blue));
    }

    public override void OnDeactivate()
    {
        Player.cosmetics.SetOutline(false, new Il2CppSystem.Nullable<Color>(Color.blue));
    }

    public override int GetAmountPerGame()
    {
        if (FirstDeadPatch.PlayerNames.Count == 0)
        {
            return 0;
        }

        var validPlayer = PlayerControl.AllPlayerControls.ToArray()
            .Where(x => FirstDeadPatch.PlayerNames.Contains(x.name)).AsEnumerable()
            .OrderBy(obj => FirstDeadPatch.PlayerNames.IndexOf(obj.name)).FirstOrDefault();

        return validPlayer != null && OptionGroupSingleton<GeneralOptions>.Instance.FirstDeathShield
            ? 1
            : 0;
    }

    public override int GetAssignmentChance()
    {
        if (FirstDeadPatch.PlayerNames.Count == 0)
        {
            return 0;
        }

        var validPlayer = PlayerControl.AllPlayerControls.ToArray()
            .Where(x => FirstDeadPatch.PlayerNames.Contains(x.name)).AsEnumerable()
            .OrderBy(obj => FirstDeadPatch.PlayerNames.IndexOf(obj.name)).FirstOrDefault();

        return validPlayer != null && OptionGroupSingleton<GeneralOptions>.Instance.FirstDeathShield
            ? 100
            : 0;
    }

    public override bool IsModifierValidOn(RoleBehaviour role)
    {
        if (FirstDeadPatch.PlayerNames.Count == 0)
        {
            return false;
        }

        var validPlayer = PlayerControl.AllPlayerControls.ToArray()
            .Where(x => FirstDeadPatch.PlayerNames.Contains(x.name)).AsEnumerable()
            .OrderBy(obj => FirstDeadPatch.PlayerNames.IndexOf(obj.name)).FirstOrDefault();

        return role.Player == validPlayer;
    }

    public override string GetDescription()
    {
        return !HideOnUi ? "You have protection because you died first last game" : string.Empty;
    }

    public override void OnDeath(DeathReason reason)
    {
        base.OnDeath(reason);

        ModifierComponent!.RemoveModifier(this);
    }

    public override void OnMeetingStart()
    {
        base.OnMeetingStart();
        ModifierComponent!.RemoveModifier(this);
    }
}