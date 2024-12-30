namespace TownOfSushi.Roles.Crewmates.Investigative.SnitchRole
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CompleteTask))]
    public class CompleteTask
    {
        public static Sprite Sprite => TownOfSushi.Arrow;
        public static void Postfix(PlayerControl __instance)
        {
            if (!__instance.Is(RoleEnum.Snitch)) return;
            if (__instance.Data.IsDead) return;
            var taskinfos = __instance.Data.Tasks.ToArray();
            var tasksLeft = taskinfos.Count(x => !x.Complete);
            var role = GetRole<Snitch>(__instance);
            var localRole = GetPlayerRole(PlayerControl.LocalPlayer);
            switch (tasksLeft)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                    if (tasksLeft == CustomGameOptions.SnitchTasksRemaining)
                    {
                        role.RegenTask();
                        if (PlayerControl.LocalPlayer.Is(RoleEnum.Snitch))
                        {
                            Flash(role.Color, 1.5f);
                            _ = new CustomMessage("Killers are aware of your existence!", 5f, true, Colors.Snitch);
                        }
                        else if (PlayerControl.LocalPlayer.Data.IsImpostor()
                            || PlayerControl.LocalPlayer.Is(RoleAlignment.NeutralKilling))
                        {
                            Utils.Flash(role.Color, 1.5f);
                            _ = new CustomMessage("The Snitch is revealed!", 5f, true, Color.green);
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
                    break;

                case 0:
                    role.RegenTask();
                    if (PlayerControl.LocalPlayer.Is(RoleEnum.Snitch))
                    {
                        Utils.Flash(Color.green, 1.5f);
                        _ = new CustomMessage("The Killers are revealed!", 5f, true, Color.green);
                        foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                        {
                            if (player.Is(RoleAlignment.NeutralKilling))
                            {
                                var gameObj = new GameObject();
                                var arrow = gameObj.AddComponent<ArrowBehaviour>();
                                gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                                var renderer = gameObj.AddComponent<SpriteRenderer>();
                                renderer.sprite = Sprite;
                                arrow.image = renderer;
                                gameObj.layer = 5;
                                role.SnitchArrows.Add(player.PlayerId, arrow);
                            }
                        }
                        foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                        {
                            if (player.Is(RoleAlignment.NeutralKilling))
                            {
                                var gameObj = new GameObject();
                                var arrow = gameObj.AddComponent<ArrowBehaviour>();
                                gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                                var renderer = gameObj.AddComponent<SpriteRenderer>();
                                renderer.sprite = Sprite;
                                arrow.image = renderer;
                                gameObj.layer = 5;
                                role.SnitchArrows.Add(player.PlayerId, arrow);
                            }
                        }
                    }
                    else if (PlayerControl.LocalPlayer.Data.IsImpostor() || PlayerControl.LocalPlayer.Is(RoleAlignment.NeutralKilling))
                    {
                        Flash(Color.green, 1.5f);
                        _ = new CustomMessage("Get rid of the Snitch!", 5f, true, Color.green);
                    }

                    break;
            }
        }
    }
}