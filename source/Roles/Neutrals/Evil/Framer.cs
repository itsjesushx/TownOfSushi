using System.Collections;

namespace TownOfSushi.Roles
{
    public class Framer : Role
    {
        public bool SpawnedAs = true;
        public PlayerControl ClosestPlayer;
        public PlayerControl Target;
        public DateTime LastFramed;
        public bool HasFrameTarget;
        public Framer(PlayerControl player) : base(player)
        {
            Name = "Framer";
            StartText = () => "Incriminate a player to win";
            TaskText = () => SpawnedAs ? "Incriminate a player!" : "Your target was killed. Now you get have to incriminate a player!";
            RoleInfo = "The Framer is a Neutral role with its own win condition. They have to suicide on a player to win. When they use their button on a player, the Framer will die and the touched player will automatically murder the framer, if the target is voted out, the framer wins.";
            LoreText = "A cunning manipulator and master of deception, the Framer weaves intricate plots to bring chaos to the ship. Their ultimate goal is to incriminate a target by sacrificing themselves in a staged act of betrayal, leaving no room for doubt. The Framer thrives in the shadows, sowing distrust and ensuring their target takes the blame, achieving victory through cunning and subterfuge.";
            Color = Colors.Framer;
            RoleType = RoleEnum.Framer;
            Faction = Faction.Neutral;
            LastFramed = DateTime.UtcNow;
            AddToRoleHistory(RoleType);
            RoleAlignment = RoleAlignment.NeutralEvil;
        }

        public float FrameTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastFramed;
            var num = CustomGameOptions.FramerCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0f;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }
    }

    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerfornFrame
    {
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton) return true;
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Framer);
            if (!flag) return true;
            var role = GetRole<Framer>(PlayerControl.LocalPlayer);
            if (!PlayerControl.LocalPlayer.CanMove || role.ClosestPlayer == null) return false;
            var flag2 = role.FrameTimer() == 0f;
            if (!flag2) return false;
            if (!__instance.enabled) return false;
            var maxDistance = KillDistance();
            if (Vector2.Distance(role.ClosestPlayer.GetTruePosition(),
                PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance) return false;
            if (role.ClosestPlayer == null) return false;
            if (role.HasFrameTarget) return false;

            var interact = Interact(PlayerControl.LocalPlayer, role.ClosestPlayer);
            if (interact[3] == true)
            {
                role.Target = role.ClosestPlayer;
                role.HasFrameTarget = true;
                RpcMurderPlayer(role.ClosestPlayer, PlayerControl.LocalPlayer);
                StartRPC(CustomRPC.SetFramerTarget, PlayerControl.LocalPlayer.PlayerId, role.ClosestPlayer.PlayerId);
            }
            if (interact[0] == true)
            {
                role.LastFramed = DateTime.UtcNow;
                return false;
            }
            else if (interact[1] == true)
            {
                role.LastFramed = DateTime.UtcNow;
                role.LastFramed = role.LastFramed.AddSeconds(CustomGameOptions.ProtectKCReset - CustomGameOptions.FramerCd);
                return false;
            }
            else if (interact[2] == true) return false;
            return false;
        }
    }

    [HarmonyPatch(typeof(HudManager))]
    public class HudManagerFramer
    {
        [HarmonyPatch(nameof(HudManager.Update))]
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Framer)) return;
            var role = GetRole<Framer>(PlayerControl.LocalPlayer);
            if (role.HasFrameTarget) return;
            
            var FramerButton = __instance.KillButton;
            FramerButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            FramerButton.SetCoolDown(role.FrameTimer(), CustomGameOptions.FramerCd);

            var NotFramer = PlayerControl.AllPlayerControls
                .ToArray()
                .Where(x => !x.Is(RoleEnum.Framer))
                .ToList();

            if (CamouflageUnCamouflagePatch.IsCamouflaged && CustomGameOptions.CamoCommsKillAnyone) SetTarget(ref role.ClosestPlayer, FramerButton);
            else SetTarget(ref role.ClosestPlayer, FramerButton, float.NaN, NotFramer);

            var renderer = FramerButton.graphic;
            if (role.ClosestPlayer != null)
            {
                renderer.color = Palette.EnabledColor;
                renderer.material.SetFloat("_Desat", 0f);
            }
            else
            {
                renderer.color = Palette.DisabledClear;
                renderer.material.SetFloat("_Desat", 1f);
            }
        }
    }

    [HarmonyPatch(typeof(ExileController), nameof(ExileController.BeginForGameplay))]
    internal class FramerMeetingExiledEnd
    {
        public static void Postfix()
        {
            if (!AmongUsClient.Instance.AmHost || GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek || AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started) return;
            Coroutines.Start(CheckFramerWin(ExileController.Instance));
        }
        private static IEnumerator CheckFramerWin(ExileController __instance)
        {
            yield return new WaitForSeconds(7f);

            var exiled = __instance.initData.networkedPlayer;
            
            if (exiled == null) yield break;

            var player = exiled.Object;

            foreach (var role in GetRoles(RoleEnum.Framer))
            if (((Framer)role).Target != null && player.PlayerId == ((Framer)role).Target.PlayerId)
            {
                FramerWin = true;
                StartRPC(CustomRPC.FramerWin);
                EndGame();
            }
        }
    }
}