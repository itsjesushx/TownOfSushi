namespace TownOfSushi.Roles.Crewmates.Special.HaunterMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CompleteTask))]
    public class CompleteTask
    {
        public static Sprite Sprite => TownOfSushi.Arrow;
        public static void Postfix(PlayerControl __instance)
        {
            if (!__instance.Is(RoleEnum.Haunter)) return;
            var role = GetRole<Haunter>(__instance);

            var taskinfos = __instance.Data.Tasks.ToArray();

            var tasksLeft = taskinfos.Count(x => !x.Complete);

            if (tasksLeft == CustomGameOptions.HaunterTasksRemainingAlert && !role.Caught)
            {
                if (PlayerControl.LocalPlayer.Is(RoleEnum.Haunter))
                {
                    Utils.Flash(role.Color, 2.5f);
                }
                else if (PlayerControl.LocalPlayer.Data.IsImpostor() || (PlayerControl.LocalPlayer.Is(RoleAlignment.NeutralKilling) && CustomGameOptions.HaunterRevealsNeutrals))
                {
                    role.Revealed = true;
                    Utils.Flash(role.Color, 2.5f);
                    var gameObj = new GameObject();
                    var arrow = gameObj.AddComponent<ArrowBehaviour>();
                    gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                    var renderer = gameObj.AddComponent<SpriteRenderer>();
                    renderer.sprite = Sprite;
                    arrow.image = renderer;
                    gameObj.layer = 5;
                    role.ImpArrows.Add(arrow);
                }
            }

            if (tasksLeft == 0 && !role.Caught)
            {
                role.CompletedTasks = true;
                if (PlayerControl.LocalPlayer.Is(RoleEnum.Haunter))
                {
                    Utils.Flash(Color.white, 2.5f);
                }
                else if (PlayerControl.LocalPlayer.Data.IsImpostor() || (PlayerControl.LocalPlayer.Is(RoleAlignment.NeutralKilling) && CustomGameOptions.HaunterRevealsNeutrals))
                {
                    Utils.Flash(Color.white, 2.5f);
                }
            }
        }
    }
}