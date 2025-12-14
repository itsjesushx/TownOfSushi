using HarmonyLib;
using TownOfSushi.Modifiers.Game;
using TownOfSushi.Options;

namespace TownOfSushi.Patches.Roles;

[HarmonyPatch(typeof(MeetingHud))]
public static class MeetingHudTimerPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(MeetingHud.UpdateTimerText))]
    public static void TimerUpdatePostfix(MeetingHud __instance)
    {
        var newText = string.Empty;
        if (PlayerControl.LocalPlayer == null || PlayerControl.LocalPlayer.Data == null || PlayerControl.LocalPlayer.HasDied()) return;
        switch (PlayerControl.LocalPlayer.Data.Role)
        {
            case ProsecutorRole pros:
                var prosecutes = OptionGroupSingleton<ProsecutorOptions>.Instance.MaxProsecutions - pros.ProsecutionsCompleted;
                newText = $"\n{prosecutes} / {OptionGroupSingleton<ProsecutorOptions>.Instance.MaxProsecutions} Prosecutions Remaining";
                break;
            case PoliticianRole:
                newText = "\nReveal successfully if half the crewmates are campaigned!";
                break;
            case MayorRole mayor:
                newText = mayor.Revealed ? "\nYou unleash 3 votes at once!" : "\nReveal yourself to get 3 total votes!";
                break;
            case VigilanteRole vigi:
                newText = $"\n{vigi.MaxKills} / {(int)OptionGroupSingleton<VigilanteOptions>.Instance.VigilanteKills} Guesses Remaining";
                if ((int)OptionGroupSingleton<VigilanteOptions>.Instance.MultiShots > 0)
                {
                    newText += $" | {vigi.SafeShotsLeft} / {(int)OptionGroupSingleton<VigilanteOptions>.Instance.MultiShots} Safe Shots";
                }
                break;
        }

        if (PlayerControl.LocalPlayer.TryGetModifier<AssassinModifier>(out var assassinMod))
        {
            newText += $"\n{assassinMod.maxKills} / {(int)OptionGroupSingleton<AssassinOptions>.Instance.AssassinKills} Guesses Remaining";
            if ((PlayerControl.LocalPlayer.TryGetModifier<DoubleShotModifier>(out var doubleShotMod)))
            {
                newText += (doubleShotMod.Used) ? " | Double Shot Used" : " | Double Shot Available";
            }
        }

        if (newText != string.Empty) __instance.TimerText.text += $"<color=#FFFFFF>{newText}</color>";
    }
}