namespace TownOfSushi.Roles.Neutral.Evil.PhantomRole
{
    [HarmonyPatch(typeof(AirshipExileController), nameof(AirshipExileController.WrapUpAndSpawn))]
    public static class AirshipExileController_WrapUpAndSpawn
    {
        public static void Postfix(AirshipExileController __instance) => SetPhantom.ExileControllerPostfix(__instance);
    }

    [HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
    public class SetPhantom
    {
        public static PlayerControl WillBePhantom;
        public static Vector2 StartPosition;

        public static void ExileControllerPostfix(ExileController __instance)
        {
            if (WillBePhantom == null) return;
            var exiled = __instance.initData.networkedPlayer?.Object;
            if (!WillBePhantom.Data.IsDead && exiled.Is(Faction.Neutral)) WillBePhantom = exiled;
            if (exiled == WillBePhantom && exiled.Is(RoleEnum.Jester)) return;
            var doomRole = AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Doomsayer && ((Doomsayer)x).WonByGuessing && ((Doomsayer)x).Player == WillBePhantom);
            if (doomRole != null) return;
            var exeRole = AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Executioner && ((Executioner)x).TargetVotedOut && ((Executioner)x).Player == WillBePhantom);
            if (exeRole != null) return;
            var jestRole = AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Jester && ((Jester)x).VotedOut && ((Jester)x).Player == WillBePhantom);
            if (jestRole != null) return;
            var vultRole = AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Vulture && ((Vulture)x).WonByEating && ((Vulture)x).Player == WillBePhantom);
            if (vultRole != null) return;
            if (WillBePhantom.Data.Disconnected) return;
            if (!WillBePhantom.Data.IsDead && WillBePhantom != exiled) return;

            if (!WillBePhantom.Is(RoleEnum.Phantom))
            {
                var oldRole = GetPlayerRole(WillBePhantom);
                var killsList = (oldRole.Kills, oldRole.CorrectAssassinKills, oldRole.IncorrectAssassinKills);
                RoleDictionary.Remove(WillBePhantom.PlayerId);
                if (PlayerControl.LocalPlayer == WillBePhantom)
                {
                    var role = new Phantom(PlayerControl.LocalPlayer);
                    role.formerRole = oldRole.RoleType;
                    role.Kills = killsList.Kills;
                    role.CorrectAssassinKills = killsList.CorrectAssassinKills;
                    role.IncorrectAssassinKills = killsList.IncorrectAssassinKills;
                    role.RegenTask();
                }
                else
                {
                    var role = new Phantom(WillBePhantom);
                    role.formerRole = oldRole.RoleType;
                    role.Kills = killsList.Kills;
                    role.CorrectAssassinKills = killsList.CorrectAssassinKills;
                    role.IncorrectAssassinKills = killsList.IncorrectAssassinKills;
                }

                RemoveTasks(WillBePhantom);
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Haunter)) WillBePhantom.MyPhysics.ResetMoveState();

                WillBePhantom.gameObject.layer = LayerMask.NameToLayer("Players");
            }

            WillBePhantom.gameObject.GetComponent<PassiveButton>().OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
            WillBePhantom.gameObject.GetComponent<PassiveButton>().OnClick.AddListener((Action)(() => WillBePhantom.OnClick()));
            WillBePhantom.gameObject.GetComponent<BoxCollider2D>().enabled = true;

            if (PlayerControl.LocalPlayer != WillBePhantom) return;

            if (GetRole<Phantom>(PlayerControl.LocalPlayer).Caught) return;

            List<Vent> vents = new();
            var CleanVentTasks = PlayerControl.LocalPlayer.myTasks.ToArray().Where(x => x.TaskType == TaskTypes.VentCleaning).ToList();
            if (CleanVentTasks != null)
            {
                var ids = CleanVentTasks.Where(x => !x.IsComplete)
                                        .ToList()
                                        .ConvertAll(x => x.FindConsoles()[0].ConsoleId);

                vents = ShipStatus.Instance.AllVents.Where(x => !ids.Contains(x.Id)).ToList();
            }
            else vents = ShipStatus.Instance.AllVents.ToList();

            var startingVent = vents[Random.RandomRangeInt(0, vents.Count)];

            Rpc(CustomRPC.SetPos, PlayerControl.LocalPlayer.PlayerId, startingVent.transform.position.x, startingVent.transform.position.y + 0.3636f);
            var pos = new Vector2(startingVent.transform.position.x, startingVent.transform.position.y + 0.3636f);

            PlayerControl.LocalPlayer.transform.position = pos;
            PlayerControl.LocalPlayer.NetTransform.SnapTo(pos);
            PlayerControl.LocalPlayer.MyPhysics.RpcEnterVent(startingVent.Id);
        }

        public static void Postfix(ExileController __instance) => ExileControllerPostfix(__instance);

        [HarmonyPatch(typeof(Object), nameof(Object.Destroy), new Type[] { typeof(GameObject) })]
        public static void Prefix(GameObject obj)
        {
            if (!SubmergedCompatibility.Loaded || GameOptionsManager.Instance?.currentNormalGameOptions?.MapId != 6) return;
            if (obj.name?.Contains("ExileCutscene") == true) ExileControllerPostfix(ExileControllerPatch.lastExiled);
        }
    }
}