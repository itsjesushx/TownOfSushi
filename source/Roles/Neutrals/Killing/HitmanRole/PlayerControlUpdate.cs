namespace TownOfSushi.Roles.Neutral.Killing.HitmanRole
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class PlayerControlUpdate
    {
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Hitman)) return;

            var role = GetRole<Hitman>(PlayerControl.LocalPlayer);
            if (role.DragDropButtonHitman == null)
            {
                role.DragDropButtonHitman = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.DragDropButtonHitman.graphic.enabled = true;
                role.DragDropButtonHitman.graphic.sprite = TownOfSushi.DragSprite;
                role.DragDropButtonHitman.gameObject.SetActive(false);
            }
            if (role.DragDropButtonHitman.graphic.sprite != TownOfSushi.DragSprite &&
                role.DragDropButtonHitman.graphic.sprite != TownOfSushi.DropSprite)
                role.DragDropButtonHitman.graphic.sprite = TownOfSushi.DragSprite;

            if (role.DragDropButtonHitman.graphic.sprite == TownOfSushi.DropSprite && role.CurrentlyDragging == null)
                role.DragDropButtonHitman.graphic.sprite = TownOfSushi.DragSprite;

            role.DragDropButtonHitman.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            
            role.DragDropButtonHitman.transform.localPosition = new Vector3(-1f, 1f, 0f);

            if (role.DragDropButtonHitman.graphic.sprite == TownOfSushi.DragSprite)
            {
                var data = PlayerControl.LocalPlayer.Data;
                var isDead = data.IsDead;
                var truePosition = PlayerControl.LocalPlayer.GetTruePosition();
                var maxDistance = KillDistance();
                var flag = (GameOptionsManager.Instance.currentNormalGameOptions.GhostsDoTasks || !data.IsDead) &&
                           (!AmongUsClient.Instance || !AmongUsClient.Instance.IsGameOver) &&
                           PlayerControl.LocalPlayer.CanMove;
                var allocs = Physics2D.OverlapCircleAll(truePosition, maxDistance,
                    LayerMask.GetMask(new[] {"Players", "Ghost"}));
                var killButton = role.DragDropButtonHitman;
                DeadBody closestBody = null;
                var closestDistance = float.MaxValue;

                foreach (var collider2D in allocs)
                {
                    if (!flag || isDead || collider2D.tag != "DeadBody") continue;
                    var component = collider2D.GetComponent<DeadBody>();
                    if (!(Vector2.Distance(truePosition, component.TruePosition) <=
                          maxDistance)) continue;

                    var distance = Vector2.Distance(truePosition, component.TruePosition);
                    if (!(distance < closestDistance)) continue;
                    closestBody = component;
                    closestDistance = distance;
                }


                KillButtonTarget2.SetTarget(killButton, closestBody, role);
            }

            if (role.DragDropButtonHitman.graphic.sprite == TownOfSushi.DragSprite)
            {
                role.DragDropButtonHitman.SetCoolDown(role.DragTimer(), CustomGameOptions.HitmanDragCd);
            }
            else
            {
                role.DragDropButtonHitman.SetCoolDown(0f, 1f);
                role.DragDropButtonHitman.graphic.color = Palette.EnabledColor;
                role.DragDropButtonHitman.graphic.material.SetFloat("_Desat", 0f);
            }
        }
    }
}