
using System.Text;
using AmongUs.GameOptions;
using HarmonyLib;
using MiraAPI.Modifiers.Types;
using TownOfSushi.Modifiers;

using TownOfSushi.Options;
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

        if (!TutorialManager.InstanceExists && body)
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
                var color = Utils.GetRoleColour(modifier.ModifierName.Replace(" ", string.Empty));
                if (modifier is IColoredModifier colorMod)
                {
                    color = colorMod.ModifierColor;
                }

                if (!first)
                {
                    modifierTextBuilder.Append(", ");
                }

                modifierTextBuilder.Append(TownOfSushiPlugin.Culture,
                    $"{color.ToTextColor()}{modifier.ModifierName}</color>");
                first = false;
            }

            modifierTextBuilder.Append(")</size></color>");
            __instance.FilterText.text = modifierTextBuilder.ToString();
        }

        var role = target.Data.Role;
        if (target.Data.IsDead && role is not GuardianAngelRole)
        {
            role = target.GetRoleWhenAlive();
        }

        var name = role.NiceName;

        var rColor = role is ICustomRole custom ? custom.RoleColor : role.TeamColor;

        if (!OptionGroupSingleton<GeneralOptions>.Instance.TheDeadKnow && !TutorialManager.InstanceExists)
        {
            if (target.Is(Factions.Neutral))
            {
                name = "Neutral";
                rColor = Color.gray;
            }
            else if (target.Is(Factions.Crewmate))
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

    // The impostor filter now includes neutral roles
    [HarmonyPostfix]
    [HarmonyPatch(typeof(HauntMenuMinigame), nameof(HauntMenuMinigame.MatchesFilter))]
    public static void MatchesFilterPostfix(HauntMenuMinigame __instance, PlayerControl pc, ref bool __result) 
    {
        if (GameOptionsManager.Instance.currentGameOptions.GameMode != GameModes.Normal) return;
        if (__instance.filterMode == HauntMenuMinigame.HauntFilters.Impostor) 
        {
            __result = pc.IsKillerRole() && !pc.Data.IsDead;
        }
    }
    // Shows the "haunt evil roles button"
    [HarmonyPrefix]
    [HarmonyPatch(typeof(HauntMenuMinigame), nameof(HauntMenuMinigame.Start))]
    public static bool StartPrefix(HauntMenuMinigame __instance) 
    {
        if (GameOptionsManager.Instance.currentGameOptions.GameMode != GameModes.Normal || !OptionGroupSingleton<GeneralOptions>.Instance.TheDeadKnow) return true;
        __instance.FilterButtons[0].gameObject.SetActive(true);
        int numActive = 0;
        int numButtons = __instance.FilterButtons.Count((PassiveButton s) => s.isActiveAndEnabled);
        float edgeDist = 0.6f * (float)numButtons;
		 for (int i = 0; i< __instance.FilterButtons.Length; i++)
		{
		    PassiveButton passiveButton = __instance.FilterButtons[i];
		    if (passiveButton.isActiveAndEnabled)
		    {
			    passiveButton.transform.SetLocalX(FloatRange.SpreadToEdges(-edgeDist, edgeDist, numActive, numButtons));
			    numActive++;
		    }
        }
        return false;
    }
}