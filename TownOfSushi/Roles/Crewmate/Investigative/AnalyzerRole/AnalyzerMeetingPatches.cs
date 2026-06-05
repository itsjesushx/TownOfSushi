using HarmonyLib;
using Reactor.Utilities.Extensions;

using UnityEngine;

namespace TownOfSushi.Roles.Crewmate;

[HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
public static class AnalyzerMeetingStartPatch
{
    public static bool DifferentFaction(this PlayerControl firstPlayer, PlayerControl secondPlayer)
    {
        return secondPlayer.Is(Factions.Crewmate) && !firstPlayer.Is(Factions.Crewmate)
        || firstPlayer.Is(Factions.Crewmate) && !secondPlayer.Is(Factions.Crewmate)

        || secondPlayer.IsImpostor() && !firstPlayer.IsImpostor()
        || firstPlayer.IsImpostor() && !secondPlayer.IsImpostor()

        || firstPlayer.Is(Factions.Neutral) && !secondPlayer.Is(Factions.Neutral)
        || secondPlayer.Is(Factions.Neutral) && !firstPlayer.Is(Factions.Neutral);
    }
    public static void Postfix()
    {
        if (PlayerControl.LocalPlayer.GetRoleWhenAlive() is not AnalyzerRole Analyzer) return;

        var firstPlayer = PlayerControl.AllPlayerControls.ToArray().FirstOrDefault(p => !p.HasDied() && p.TryGetModifier<AnalyzerFirstCheckModifier>(out var analy) && analy.AnalyzerId == PlayerControl.LocalPlayer.PlayerId);
        var secondPlayer = PlayerControl.AllPlayerControls.ToArray().FirstOrDefault(p => !p.HasDied() && p.TryGetModifier<AnalyzerSecondCheckModifier>(out var analy) && analy.AnalyzerId == PlayerControl.LocalPlayer.PlayerId);

        if (firstPlayer == null || secondPlayer == null) return;

        bool sameFaction = !firstPlayer.DifferentFaction(secondPlayer);

        string msg = sameFaction
            ? $"<b><color=green>{firstPlayer.Data.PlayerName} and {secondPlayer.Data.PlayerName} have the SAME faction!</color></b>"
            : $"<b><color=red>{firstPlayer.Data.PlayerName} and {secondPlayer.Data.PlayerName} are a DIFFERENT faction!</color></b>";

        if (OptionGroupSingleton<AnalyzerOptions>.Instance.ExtraInfoType == AnalyzerOptions.AnalyzerCheckType.Faction)
        {
            string firstFaction;
            if (firstPlayer.Is(Factions.Neutral)) firstFaction = "Neutral";
            else if (firstPlayer.Is(Factions.Crewmate)) firstFaction = "Crewmate";
            else firstFaction = "Impostor";

            string secondFaction;
            if (secondPlayer.Is(Factions.Neutral)) secondFaction = "Neutral";
            else if (secondPlayer.Is(Factions.Crewmate)) secondFaction = "Crewmate";
            else secondFaction = "Impostor";

            if (sameFaction)
            {
                msg += $"\nFaction found: {firstFaction}";
            }
            else if (UnityEngine.Random.value > 0.5f)
            {
                msg += $"\nFactions found: {firstFaction} & {secondFaction}";
            }
            else
            {
                msg += $"\nFactions found: {secondFaction} & {firstFaction}";
            }
        }
        else if (OptionGroupSingleton<AnalyzerOptions>.Instance.ExtraInfoType == AnalyzerOptions.AnalyzerCheckType.Alignment)
        {
            string firstAlignment = firstPlayer.GetRoleWhenAlive().GetRoleAlignment().ToDisplayString();
            string secondAlignment = secondPlayer.GetRoleWhenAlive().GetRoleAlignment().ToDisplayString();

            if (sameFaction)
            {
                msg += $"\nAlignment found: {firstAlignment}";
            }
            else if (UnityEngine.Random.value > 0.5f)
            {
                msg += $"\nAlignments found: {firstAlignment} & {secondAlignment}";
            }
            else
            {
                msg += $"\nAlignments found: {secondAlignment} & {firstAlignment}";
            }
        }

        var notif1 = Helpers.CreateAndShowNotification(
                $"<b>{Palette.CrewmateBlue.ToTextColor()}{msg}</color></b>",
                Color.white, spr: TownOfSushiAssets.Seer.LoadAsset());
        
        notif1.AdjustNotification();
        Utils.AddFakeChat(PlayerControl.LocalPlayer.Data, 
            $"<color=#{TownOfSushiColors.Analyzer.ToHtmlStringRGBA()}>Analyzer Feedback</color>", msg, false, true);

        firstPlayer.RemoveModifier<AnalyzerFirstCheckModifier>();
        secondPlayer.RemoveModifier<AnalyzerSecondCheckModifier>();
        Analyzer.InvestigatedFirst = false;
    }
}