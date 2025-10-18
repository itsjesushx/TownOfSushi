using HarmonyLib;
using Reactor.Utilities.Extensions;
using TownOfSushi.Modules;
using UnityEngine;

namespace TownOfSushi.Roles.Crewmate;

[HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
public static class SeerMeetingStartPatch
{
    public static bool DifferentFaction(this PlayerControl firstPlayer, PlayerControl secondPlayer)
    {
        return secondPlayer.IsCrewmate() && !firstPlayer.IsCrewmate()
        || firstPlayer.IsCrewmate() && !secondPlayer.IsCrewmate()

        || secondPlayer.IsImpostor() && !firstPlayer.IsImpostor()
        || firstPlayer.IsImpostor() && !secondPlayer.IsImpostor()

        || firstPlayer.IsNeutral() && !secondPlayer.IsNeutral()
        || secondPlayer.IsNeutral() && !firstPlayer.IsNeutral();
    }
    public static void Postfix()
    {
        var firstPlayer = PlayerControl.AllPlayerControls.ToArray().FirstOrDefault(p => !p.HasDied() && p.HasModifier<SeerFirstCheckModifier>());
        var secondPlayer = PlayerControl.AllPlayerControls.ToArray().FirstOrDefault(p => !p.HasDied() && p.HasModifier<SeerSecondCheckModifier>());

        if (firstPlayer == null || secondPlayer == null) return;

        if (PlayerControl.LocalPlayer.GetRoleWhenAlive() is not SeerRole seer) return;

        bool sameFaction = !firstPlayer.DifferentFaction(secondPlayer);

        string msg = sameFaction
            ? $"<b><color=green>{firstPlayer.Data.PlayerName} and {secondPlayer.Data.PlayerName} have the SAME faction!</color></b>"
            : $"<b><color=red>{firstPlayer.Data.PlayerName} and {secondPlayer.Data.PlayerName} are a DIFFERENT faction!</color></b>";

        var notif1 = Helpers.CreateAndShowNotification(
                $"<b>{Palette.CrewmateBlue.ToTextColor()}{msg}</color></b>",
                Color.white, spr: TOSRoleIcons.Detective.LoadAsset());
        
        notif1.AdjustNotification();

        var title = $"<color=#{TownOfSushiColors.Seer.ToHtmlStringRGBA()}>Seer Feedback</color>";
        MiscUtils.AddFakeChat(PlayerControl.LocalPlayer.Data, title, msg, false, true);

        firstPlayer.RemoveModifier<SeerFirstCheckModifier>();
        secondPlayer.RemoveModifier<SeerSecondCheckModifier>();
        seer.InvestigatedFirst = false;
    }
}