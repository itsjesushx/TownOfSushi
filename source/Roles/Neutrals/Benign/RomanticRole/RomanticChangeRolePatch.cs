namespace TownOfSushi.Roles.Neutral.Benign.RomanticRole
{

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class RomanticChangeRolePatch
    {
        private static void UpdateMeeting(MeetingHud __instance, Romantic role)
        {
            if (CustomGameOptions.GAKnowsTargetRole) return;
            foreach (var player in __instance.playerStates)
                if (player.TargetPlayerId == role.Beloved.PlayerId)
                    player.NameText.text += "<color=#FF66CCFF> ♥</color>";
        }

        private static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Romantic)) return;
            if (PlayerControl.LocalPlayer.Data.IsDead) return;

            var role = GetRole<Romantic>(PlayerControl.LocalPlayer);

            if (MeetingHud.Instance != null) UpdateMeeting(MeetingHud.Instance, role);

            if (!CustomGameOptions.RomanticKnowsBelovedRole && !CamouflageUnCamouflagePatch.IsCamouflaged) role.Beloved.nameText().text += "<color=#FF66CCFF> ♥</color>";

            if (!role.Beloved.Data.IsDead && !role.Beloved.Data.Disconnected) return;

            Rpc(CustomRPC.RomanticChangeRole, PlayerControl.LocalPlayer.PlayerId);
            DestroyableSingleton<HudManager>.Instance.KillButton.gameObject.SetActive(false);

            RomanticChangeRole(PlayerControl.LocalPlayer);
        }

        public static void RomanticChangeRole(PlayerControl player)
        {
            var ga = GetRole<Romantic>(player);
            player.myTasks.RemoveAt(0);
            RoleDictionary.Remove(player.PlayerId);

            if (CustomGameOptions.RomanticOnBelovedDeath == RomanticBecomeOptions.Jester)
            {
                var jester = new Jester(player);
                jester.SpawnedAs = false;
                jester.RegenTask();
            }
            else if (CustomGameOptions.RomanticOnBelovedDeath == RomanticBecomeOptions.Amnesiac)
            {
                var amnesiac = new Amnesiac(player);
                amnesiac.SpawnedAs = false;
                amnesiac.RegenTask();
            }
            else if (CustomGameOptions.RomanticOnBelovedDeath == RomanticBecomeOptions.Repick)
            {
                var amnesiac = new Romantic(player);
                amnesiac.SpawnedAs = false;
                amnesiac.RegenTask();
                //im stupid and tried doing romantic.PickedAlready = false; but it didnt work LMAO
            }
            else
            {
                new Crewmate(player);
            }
        }
    }
}