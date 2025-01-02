namespace TownOfSushi.Roles
{
    public class Snitch : Role
    {
        public List<ArrowBehaviour> ImpArrows = new List<ArrowBehaviour>();
        public Dictionary<byte, ArrowBehaviour> SnitchArrows = new Dictionary<byte, ArrowBehaviour>();
        public Snitch(PlayerControl player) : base(player)
        {
            Name = "Snitch";
            StartText = () => "Complete All Your Tasks To Discover The Impostors";
            TaskText = () => TasksDone ? "Find the arrows pointing to the Impostors!" : "Complete all your tasks to discover the Impostors!";
            Color = Colors.Snitch;
            RoleType = RoleEnum.Snitch;
            Faction = Faction.Crewmates;
            AddToRoleHistory(RoleType);
            RoleAlignment = RoleAlignment.CrewInvest;
        }

        public bool Revealed => TasksLeft <= CustomGameOptions.SnitchTasksRemaining;
        public bool TasksDone => TasksLeft <= 0;
        internal override bool Criteria()
        {
            return Revealed && PlayerControl.LocalPlayer.Data.IsImpostor() && !Player.Data.IsDead 
            || Revealed && PlayerControl.LocalPlayer.Is(RoleAlignment.NeutralKilling) && !Player.Data.IsDead 
            || base.Criteria();
        }

        internal override bool RoleCriteria()
        {
            var localPlayer = PlayerControl.LocalPlayer;
            if (localPlayer.Data.IsImpostor() && !Player.Data.IsDead)
            {
                return Revealed || base.RoleCriteria();
            }
            else if (GetPlayerRole(localPlayer).RoleAlignment == RoleAlignment.NeutralKilling && !Player.Data.IsDead)
            {
                return Revealed || base.RoleCriteria();
            }
            return false || base.RoleCriteria();
        }

        public void DestroyArrow(byte targetPlayerId)
        {
            var arrow = SnitchArrows.FirstOrDefault(x => x.Key == targetPlayerId);
            if (arrow.Value != null)
                Object.Destroy(arrow.Value);
            if (arrow.Value.gameObject != null)
                Object.Destroy(arrow.Value.gameObject);
            SnitchArrows.Remove(arrow.Key);
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CompleteTask))]
    public class SnitchCompleteTask
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

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HighlightImpostorsSnitch
    {
        private static void UpdateMeeting(MeetingHud __instance)
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                foreach (var state in __instance.playerStates)
                {
                    if (player.PlayerId != state.TargetPlayerId) continue;
                    if (player.Is(Faction.Impostors))
                        state.NameText.color = Palette.ImpostorRed;
                    if (player.Is(RoleAlignment.NeutralKilling))
                        state.NameText.color = Color.gray;
                }
            }
        }

        public static void Postfix(HudManager __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Snitch)) return;
            var role = GetRole<Snitch>(PlayerControl.LocalPlayer);
            if (!role.TasksDone) return;
            if (MeetingHud.Instance) UpdateMeeting(MeetingHud.Instance);

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.Data.IsImpostor())
                {
                    var impcolour = Palette.ImpostorRed;
                    if (player.Is(AbilityEnum.Chameleon)) impcolour.a = GetAbility<Chameleon>(player).Opacity;
                    player.nameText().color = impcolour;
                }
                else if (player.Is(RoleAlignment.NeutralKilling))
                {
                    var neutColour = Color.gray;
                    if (player.Is(AbilityEnum.Chameleon)) neutColour.a = GetAbility<Chameleon>(player).Opacity;
                    player.nameText().color = neutColour;
                }
            }
        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class UpdateSnitchArrows
    {
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;

            foreach (var role in AllRoles.Where(x => x.RoleType == RoleEnum.Snitch))
            {
                var snitch = (Snitch)role;
                if (PlayerControl.LocalPlayer.Data.IsDead || snitch.Player.Data.IsDead)
                {
                    snitch.SnitchArrows.Values.DestroyAll();
                    snitch.SnitchArrows.Clear();
                    snitch.ImpArrows.DestroyAll();
                    snitch.ImpArrows.Clear();
                }

                foreach (var arrow in snitch.ImpArrows) arrow.target = snitch.Player.transform.position;

                foreach (var arrow in snitch.SnitchArrows)
                {
                    var player = PlayerById(arrow.Key);
                    if (player == null || player.Data == null || player.Data.IsDead || player.Data.Disconnected)
                    {
                        snitch.DestroyArrow(arrow.Key);
                        continue;
                    }
                    arrow.Value.target = player.transform.position;
                }
            }
        }
    }
}