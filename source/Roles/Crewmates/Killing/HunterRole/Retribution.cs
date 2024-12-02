using TownOfSushi.Roles.Crewmates.Support.MedicRole;

namespace TownOfSushi.Roles.Crewmates.Killing.HunterRole
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.CastVote))]
    internal class CastVote
    {
        private static void Postfix(MeetingHud __instance, [HarmonyArgument(0)] byte srcPlayerId, [HarmonyArgument(1)] byte suspectPlayerId)
        {
            var votingPlayer = PlayerById(srcPlayerId);
            var suspectPlayer = PlayerById(suspectPlayerId);
            if (!suspectPlayer.Is(RoleEnum.Hunter)) return;
            var hunter = GetRole<Hunter>(suspectPlayer);
            hunter.LastVoted = votingPlayer;
        }
    }

    [HarmonyPatch(typeof(ExileController), nameof(ExileController.BeginForGameplay))]
    internal class MeetingExiledEnd
    {
        private static void Postfix(ExileController __instance)
        {
            var exiled = __instance.initData.networkedPlayer;
            if (exiled == null) return;
            var player = exiled.Object;
            if (player.Is(RoleEnum.Hunter) && CustomGameOptions.RetributionOnVote)
            {
                var hunter = GetRole<Hunter>(player);
                if (hunter.LastVoted != null && hunter.LastVoted != player && !hunter.LastVoted.Is(RoleEnum.Pestilence))
                {
                    foreach (var role in Role.AllRoles.Where(x => x.RoleType == RoleEnum.Executioner))
                    {
                        var exe = (Executioner)role;
                        if (exe.target == player) return;
                    }
                    Utils.Rpc(CustomRPC.Retribution, hunter.Player.PlayerId, hunter.LastVoted.PlayerId);
                    Retribution.MurderPlayer(hunter, hunter.LastVoted);
                }
            }
            foreach (var role in Role.AllRoles.Where(x => x.RoleType == RoleEnum.Hunter))
            {
                var hunter = (Hunter)role;
                hunter.LastVoted = null;
            }
        }
    }

    public class Retribution
    {
        public static void MurderPlayer(Hunter hunter, PlayerControl player)
        {
            if (player.Is(Faction.Crewmates)) hunter.IncorrectShots += 1;
            else hunter.CorrectKills += 1;
            MurderPlayer(player);
        }

        public static void MurderPlayer(
            PlayerControl player
        )
        {
            var hudManager = DestroyableSingleton<HudManager>.Instance;

            SoundManager.Instance.PlaySound(player.KillSfx, false, 0.8f);
            hudManager.KillOverlay.ShowKillAnimation(player.Data, player.Data);
            
            var amOwner = player.AmOwner;
            if (amOwner)
            {
                hudManager.ShadowQuad.gameObject.SetActive(false);
                player.nameText().GetComponent<MeshRenderer>().material.SetInt("_Mask", 0);
                player.RpcSetScanner(false);
                ImportantTextTask importantTextTask = new GameObject("_Player").AddComponent<ImportantTextTask>();
                importantTextTask.transform.SetParent(AmongUsClient.Instance.transform, false);
                if (!GameOptionsManager.Instance.currentNormalGameOptions.GhostsDoTasks)
                {
                    for (int i = 0; i < player.myTasks.Count; i++)
                    {
                        PlayerTask playerTask = player.myTasks.ToArray()[i];
                        playerTask.OnRemove();
                        Object.Destroy(playerTask.gameObject);
                    }

                    player.myTasks.Clear();
                    importantTextTask.Text = DestroyableSingleton<TranslationController>.Instance.GetString(
                        StringNames.GhostIgnoreTasks,
                        new Il2CppReferenceArray<Il2CppSystem.Object>(0)
                    );
                }
                else
                {
                    importantTextTask.Text = DestroyableSingleton<TranslationController>.Instance.GetString(
                        StringNames.GhostDoTasks,
                        new Il2CppReferenceArray<Il2CppSystem.Object>(0));
                }

                player.myTasks.Insert(0, importantTextTask);
            }
            player.Die(DeathReason.Kill, false);

            var role2 = GetPlayerRole(player);
            role2.DeathReason = DeathReasonEnum.Retribution;
            role2.KilledBy = " By " + ColorString(Colors.Hunter, GetRole<Hunter>(PlayerControl.LocalPlayer).PlayerName);

            var deadPlayer = new DeadPlayer
            {
                PlayerId = player.PlayerId,
                KillerId = player.PlayerId,
                KillTime = System.DateTime.UtcNow,
            };

            Murder.KilledPlayers.Add(deadPlayer);

            AddHauntPatch.AssassinatedPlayers.Add(player);
        }
    }
}