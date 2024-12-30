namespace TownOfSushi
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.OnClick))]
    public class ClickGhostRole
    {
        public static void Prefix(PlayerControl __instance)
        {
            if (MeetingHud.Instance) return;
            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (PlayerControl.LocalPlayer == null || PlayerControl.LocalPlayer.Data == null || PlayerControl.LocalPlayer.Data.Tasks == null) return;
            var taskinfos = __instance.Data.Tasks.ToArray();
            var tasksLeft = taskinfos.Count(x => !x.Complete);
            if (__instance.Is(RoleEnum.Phantom))
            {
                if (tasksLeft <= CustomGameOptions.PhantomTasksRemaining)
                {
                    var role = GetRole<Phantom>(__instance);
                    role.Caught = true;
                    role.Player.Exiled();
                    Rpc(CustomRPC.CatchPhantom, role.Player.PlayerId);
                }
            }
            else if (__instance.Is(RoleEnum.Haunter))
            {
                if (CustomGameOptions.HaunterCanBeClickedBy == HaunterCanBeClickedBy.ImpsOnly && !PlayerControl.LocalPlayer.Data.IsImpostor()) return;
                if (CustomGameOptions.HaunterCanBeClickedBy == HaunterCanBeClickedBy.NonCrew && !(PlayerControl.LocalPlayer.Data.IsImpostor() || PlayerControl.LocalPlayer.Is(RoleAlignment.NeutralKilling))) return;
                if (tasksLeft <= CustomGameOptions.HaunterTasksRemainingClicked)
                {
                    var role = GetRole<Haunter>(__instance);
                    role.Caught = true;
                    role.Player.Exiled();
                    Rpc(CustomRPC.CatchHaunter, role.Player.PlayerId);
                }
            }
            return;
        }
    }
}