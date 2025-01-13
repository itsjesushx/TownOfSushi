using System.Collections;

namespace TownOfSushi.Roles
{
    public class Executioner : Role
    {
        public PlayerControl target;
        public Executioner(PlayerControl player) : base(player)
        {
            Name = "Executioner";
            StartText = () =>
                target == null ? "You don't have a target for some reason... weird..." : $"Vote {target.name} Out";
            TaskText = () =>
                target == null
                    ? "You don't have a target for some reason... weird..."
                    : $"Vote {target.name} out!";
            RoleInfo = "The Executioner is a Neutral role with its own win condition. Their goal is to vote out a player, specified in the beginning of a game. If that player gets voted out, they win the game.";
            LoreText = "A relentless pursuer of chaos, you are tasked with ensuring that a specific player is voted out, no matter the cost. As the Executioner, you must manipulate the votes of others, turning the tide in your favor to eliminate your target. However, your victory is tied to the downfall of your chosen victim, and if they survive, your mission fails. Use your influence wisely, for the fate of your target—and your own—depends on the vote.";            
            Color = Colors.Executioner;
            RoleType = RoleEnum.Executioner;
            Faction = Faction.Neutral;
            AddToRoleHistory(RoleType);
            RoleAlignment = RoleAlignment.NeutralEvil;
        }
    }

    [HarmonyPatch(typeof(ExileController), nameof(ExileController.BeginForGameplay))]
    internal class ExecutionerMeetingExiledEnd
    {
        public static void Postfix()
        {
            if (!AmongUsClient.Instance.AmHost || GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek || AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started) return;
            Coroutines.Start(CheckExecutionerWin(ExileController.Instance));
        }
        private static IEnumerator CheckExecutionerWin(ExileController __instance)
        {
            yield return new WaitForSeconds(5f);

            var exiled = __instance.initData.networkedPlayer;

            if (exiled == null) yield break;

            var player = exiled.Object;

            foreach (var role in GetRoles(RoleEnum.Executioner))
            if (player.PlayerId == ((Executioner)role).target.PlayerId)
            {
                ExecutionerWin = true;
                StartRPC(CustomRPC.ExecutionerWin);
                EndGame();
            }
        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class ExeTargetColor
    {
        private static void UpdateMeeting(MeetingHud __instance, Executioner role)
        {
            foreach (var player in __instance.playerStates)
                if (player.TargetPlayerId == role.target.PlayerId)
                    player.NameText.text += "<color=#CCCCCCFF> [⦿]</color>";
        }

        private static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Executioner)) return;
            if (PlayerControl.LocalPlayer.Data.IsDead) return;

            var role = GetRole<Executioner>(PlayerControl.LocalPlayer);

            if (MeetingHud.Instance != null) UpdateMeeting(MeetingHud.Instance, role);

            if (role.target && role.target.nameText() && !CamouflageUnCamouflagePatch.IsCamouflaged) role.target.nameText().text += "<color=#CCCCCCFF> [⦿]</color>";

            if (!role.target.Data.IsDead && !role.target.Data.Disconnected && !role.target.Is(RoleEnum.Vampire)) return;
            if (ExecutionerWin) return;

            StartRPC(CustomRPC.ExecutionerToJester, PlayerControl.LocalPlayer.PlayerId);

            ExecutionerChangeRole(PlayerControl.LocalPlayer);
        }

        public static void ExecutionerChangeRole(PlayerControl player)
        {
            player.myTasks.RemoveAt(0);
            RoleDictionary.Remove(player.PlayerId);

            if (CustomGameOptions.OnTargetDead == OnTargetDead.Jester)
            {
                var jester = new Jester(player);
                jester.SpawnedAs = false;
                jester.ReDoTaskText();
            }
            else if (CustomGameOptions.OnTargetDead == OnTargetDead.Amnesiac)
            {
                var amnesiac = new Amnesiac(player);
                amnesiac.SpawnedAs = false;
                amnesiac.ReDoTaskText();
            }
            else if (CustomGameOptions.OnTargetDead == OnTargetDead.Framer)
            {
                var amnesiac = new Framer(player);
                amnesiac.SpawnedAs = false;
                amnesiac.ReDoTaskText();
            }
            else
            {
                new Crewmate(player);
            }
        }
    }
}