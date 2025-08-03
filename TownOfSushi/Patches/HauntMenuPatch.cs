using System.Globalization;
using System.Text;
using AmongUs.GameOptions;
using HarmonyLib;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Modifiers.Types;
using MiraAPI.Roles;
using TownOfSushi.Modifiers;
using TownOfSushi.Modules;
using TownOfSushi.Options;
using TownOfSushi.Roles.Crewmate;
using TownOfSushi.Roles.Neutral;
using TownOfSushi.Utilities;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfSushi.Patches;

[HarmonyPatch(typeof(HauntMenuMinigame), nameof(HauntMenuMinigame.SetHauntTarget))]
public static class HauntMenuMinigamePatch
{
    public static void Postfix(HauntMenuMinigame __instance)
    {
        if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek)
        {
            return;
        }

        var body = Object.FindObjectsOfType<DeadBody>()
            .FirstOrDefault(x => x.ParentId == PlayerControl.LocalPlayer.PlayerId);
        var fakePlayer = FakePlayer.FakePlayers.FirstOrDefault(x => x.PlayerId == PlayerControl.LocalPlayer.PlayerId);

        if (!TutorialManager.InstanceExists && (body || fakePlayer?.body))
        {
            __instance.Close();
            __instance.NameText.text = string.Empty;
            __instance.FilterText.text = string.Empty;
            return;
        }

        var target = __instance.HauntTarget;
        __instance.FilterText.text = string.Empty;

        var modifiers = target.GetModifiers<GameModifier>().Where(x => x is not ExcludedGameModifier)
            .OrderBy(x => x.ModifierName).ToList();
        __instance.FilterText.text = "<color=#FFFFFF><size=100%>(No Modifiers)</size></color>";
        if (modifiers.Count != 0)
        {
            var modifierTextBuilder = new StringBuilder("<color=#FFFFFF><size=100%>(");
            var first = true;
            foreach (var modifier in modifiers)
            {
                var color = MiscUtils.GetRoleColour(modifier.ModifierName.Replace(" ", string.Empty));
                if (modifier is IColoredModifier colorMod)
                {
                    color = colorMod.ModifierColor;
                }

                if (!first)
                {
                    modifierTextBuilder.Append(", ");
                }

                modifierTextBuilder.Append(CultureInfo.InvariantCulture,
                    $"{color.ToTextColor()}{modifier.ModifierName}</color>");
                first = false;
            }

            modifierTextBuilder.Append(")</size></color>");
            __instance.FilterText.text = modifierTextBuilder.ToString();
        }

        var role = target.Data.Role;
        if (target.Data.IsDead && role is not PhantomTOSRole or GuardianAngelRole or HaunterRole)
        {
            role = target.GetRoleWhenAlive();
        }

        var name = role.NiceName;

        var rColor = role is ICustomRole custom ? custom.RoleColor : role.TeamColor;

        if (!OptionGroupSingleton<GeneralOptions>.Instance.TheDeadKnow && !TutorialManager.InstanceExists)
        {
            if (role.IsNeutral())
            {
                name = "Neutral";
                rColor = Color.gray;
            }
            else if (role.IsCrewmate())
            {
                name = "Crewmate";
                rColor = Palette.CrewmateBlue;
            }
            else
            {
                name = "Impostor";
                rColor = Palette.ImpostorRed;
            }
        }

        __instance.NameText.text =
            $"<size=90%>{__instance.NameText.text} - {rColor.ToTextColor()}{name}</color></size>";
    }
}