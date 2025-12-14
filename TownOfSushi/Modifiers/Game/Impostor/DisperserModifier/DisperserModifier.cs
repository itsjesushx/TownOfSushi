using System.Collections;
using Il2CppInterop.Runtime.Attributes;
using Reactor.Utilities.Extensions;
using TownOfSushi.Modules;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfSushi.Modifiers.Game.Impostor;

public sealed class DisperserModifier : TOSGameModifier, IWikiDiscoverable
{
    public override string ModifierName => "Disperser";
    public override string IntroInfo => "You can disperse players to vents around the map";

    public override LoadableAsset<Sprite>? ModifierIcon => TOSModifierIcons.Disperser;
    public override ModifierFaction FactionType => ModifierFaction.ImpostorUtility;
    public override Color FreeplayFileColor => new Color32(255, 25, 25, 255);

    public string GetAdvancedDescription()
    {
        return
            "Disperse everyone on the map to a random vent, given that they are not Lazy. You cannot have any other button modifiers with Disperser.";
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Disperse",
            "You can disperse players on the map to any vents, which you can do once per game.",
            TOSAssets.DisperseSprite)
    ];

    public override string GetDescription()
    {
        return "Separate the Crew.";
    }

    public override int GetAssignmentChance()
    {
        return (int)OptionGroupSingleton<DisperserOptions>.Instance.DisperserChance;
    }

    public override int GetAmountPerGame()
    {
        return (int)OptionGroupSingleton<DisperserOptions>.Instance.DisperserAmount;
    }

    public override bool IsModifierValidOn(RoleBehaviour role)
    {
        return base.IsModifierValidOn(role) && role.IsImpostor() &&
               !role.Player.GetModifierComponent().HasModifier<SatelliteModifier>(true) &&
               !role.Player.GetModifierComponent().HasModifier<ButtonBarryModifier>(true);
    }

    public static IEnumerator CoDisperse(Dictionary<byte, Vector2> coordinates)
    {
        yield return HudManager.Instance.CoFadeFullScreen(Color.clear, new Color(0.6f, 0.1f, 0.2f, 1f), 11f / 24f);
        yield return HudManager.Instance.CoFadeFullScreen(new Color(0.6f, 0.1f, 0.2f, 1f), Color.clear);

        DispersePlayersToCoordinates(coordinates);

        var notif1 = Helpers.CreateAndShowNotification(
            MiscUtils.ColorString(TownOfSushiColors.ImpSoft, $"<b>Everyone has been dispersed to a vent!</b>"), Color.white,
            spr: TOSModifierIcons.Disperser.LoadAsset());

        
        notif1.AdjustNotification();
    }

    public static void DispersePlayersToCoordinates(Dictionary<byte, Vector2> coordinates)
    {
        if (coordinates.ContainsKey(PlayerControl.LocalPlayer.PlayerId))
        {
            if (Minigame.Instance)
            {
                try
                {
                    Minigame.Instance.Close();
                }
                catch
                {
                    /* ignored */
                }
            }

            if (PlayerControl.LocalPlayer.inVent)
            {
                PlayerControl.LocalPlayer.MyPhysics.RpcExitVent(Vent.currentVent.Id);
                PlayerControl.LocalPlayer.MyPhysics.ExitAllVents();
            }
        }

        foreach (var (key, value) in coordinates)
        {
            var player = MiscUtils.PlayerById(key)!;
            player.transform.position = value;

            if (PlayerControl.LocalPlayer == player)
            {
                PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(value);
            }
        }

        if (PlayerControl.LocalPlayer.walkingToVent)
        {
            PlayerControl.LocalPlayer.inVent = false;
            Vent.currentVent = null;
            PlayerControl.LocalPlayer.moveable = true;
            PlayerControl.LocalPlayer.MyPhysics.StopAllCoroutines();
        }

        if (ModCompatibility.SubLoaded)
        {
            ModCompatibility.ChangeFloor(PlayerControl.LocalPlayer.transform.position.y > -7f);
        }
    }

    public static Dictionary<byte, Vector2> GenerateDisperseCoordinates()
    {
        var targets = PlayerControl.AllPlayerControls.ToArray()
            .Where(player => !player.Data.IsDead && !player.Data.Disconnected).ToList();

        // players with the LazyModifier can't be dispersed
        targets.RemoveAll(x => x.HasModifier<LazyModifier>());

        var vents = Object.FindObjectsOfType<Vent>();

        var coordinates = new Dictionary<byte, Vector2>(targets.Count);

        foreach (var target in targets)
        {
            var destination = vents.Random()!.transform.position + new Vector3(0f, 0.3636f, 0f);
            coordinates.Add(target.PlayerId, destination);
        }

        return coordinates;
    }
}