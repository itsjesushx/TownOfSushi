using HarmonyLib;

using TownOfUs.Modules.Components;

namespace TownOfSushi.Patches;

[HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
public static class MeetingMenuUpdatePatch
{
    public static void Postfix()
    {
        MeetingMenu.Instances.Do(x => x.Update());
        
        if (Utils.Meetingtime > 10f || GameOptionsManager.Instance.currentNormalGameOptions.VotingTime == 0f) 
            return;
        
        MeetingMenu.Instances.Do(x => x.HideButtons());
        GuesserMenu.Instance.Close();
    }
}