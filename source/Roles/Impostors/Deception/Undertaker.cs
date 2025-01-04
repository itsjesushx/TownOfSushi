namespace TownOfSushi.Roles
{
    public class Undertaker : Role
    {
        public KillButton _dragDropButton;
        public Undertaker(PlayerControl player) : base(player)
        {
            var canVent = CustomGameOptions.UndertakerVentWithBody ? "can vent with a body" : "can't vent with a body";
            Name = "Undertaker";
            StartText = () => "Drag Bodies And Hide Them";
            TaskText = () => "Drag bodies around to hide them from being reported";
            RoleInfo = $"The Undertaker can drag bodies around to hide them from being reported. The Undertaker can drag a body to a location and then drop it to hide it from being reported. The Undertaker can only drag a body every {CustomGameOptions.DragCd} seconds. They {canVent}. Their speed is {CustomGameOptions.UndertakerDragSpeed}x.";
            LoreText = "A shadowy figure, you specialize in concealing the aftermath of your deeds. As the Undertaker, you can drag the bodies of the fallen and hide them from view, preventing others from uncovering your dark work. Your ability to erase evidence makes you a dangerous Impostor, leaving no trace of your actions and casting doubt on those who might suspect you.";
            Color = Colors.Impostor;
            LastDragged = DateTime.UtcNow;
            RoleType = RoleEnum.Undertaker;
            Faction = Faction.Impostors;
            AddToRoleHistory(RoleType);
            RoleAlignment = RoleAlignment.ImpDeception;
        }

        public DateTime LastDragged { get; set; }
        public DeadBody CurrentTarget { get; set; }
        public DeadBody CurrentlyDragging { get; set; }
        public KillButton DragDropButton
        {
            get => _dragDropButton;
            set
            {
                _dragDropButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }
        public float DragTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastDragged;
            var num = CustomGameOptions.DragCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class UndertakerPlayerControlUpdate
    {
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Undertaker)) return;

            var role = GetRole<Undertaker>(PlayerControl.LocalPlayer);
            if (role.DragDropButton == null)
            {
                role.DragDropButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.DragDropButton.graphic.enabled = true;
                role.DragDropButton.graphic.sprite = TownOfSushi.DragSprite;
                role.DragDropButton.gameObject.SetActive(false);
            }
            if (role.DragDropButton.graphic.sprite != TownOfSushi.DragSprite &&
                role.DragDropButton.graphic.sprite != TownOfSushi.DropSprite)
                role.DragDropButton.graphic.sprite = TownOfSushi.DragSprite;

            if (role.DragDropButton.graphic.sprite == TownOfSushi.DropSprite && role.CurrentlyDragging == null)
                role.DragDropButton.graphic.sprite = TownOfSushi.DragSprite;

            role.DragDropButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);


            if (role.DragDropButton.graphic.sprite == TownOfSushi.DragSprite)
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
                var killButton = role.DragDropButton;
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


                UndertakerKillButtonTarget.SetTarget(killButton, closestBody, role);
            }

            if (role.DragDropButton.graphic.sprite == TownOfSushi.DragSprite)
            {
                role.DragDropButton.SetCoolDown(role.DragTimer(), CustomGameOptions.DragCd);
            }
            else
            {
                role.DragDropButton.SetCoolDown(0f, 1f);
                role.DragDropButton.graphic.color = Palette.EnabledColor;
                role.DragDropButton.graphic.material.SetFloat("_Desat", 0f);
            }
        }
    }

    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class UndertakerPerformDrag
    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Undertaker);
            if (!flag) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = GetRole<Undertaker>(PlayerControl.LocalPlayer);

            if (__instance == role.DragDropButton)
            {
                if (role.DragDropButton.graphic.sprite == TownOfSushi.DragSprite)
                {
                    if (__instance.isCoolingDown) return false;
                    if (!__instance.enabled) return false;
                    var maxDistance = KillDistance();
                    if (Vector2.Distance(role.CurrentTarget.TruePosition,
                        PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance) return false;
                    var playerId = role.CurrentTarget.ParentId;
                    var player = Utils.PlayerById(playerId);
                    var abilityUsed = Utils.AbilityUsed(PlayerControl.LocalPlayer);
                    if (!abilityUsed) return false;
                    if ((player.IsInfected() || role.Player.IsInfected()) && !player.Is(RoleEnum.Plaguebearer))
                    {
                        foreach (var pb in GetRoles(RoleEnum.Plaguebearer)) ((Plaguebearer)pb).RpcSpreadInfection(player, role.Player);
                    }

                    Utils.Rpc(CustomRPC.Drag, PlayerControl.LocalPlayer.PlayerId, playerId);

                    role.CurrentlyDragging = role.CurrentTarget;

                    UndertakerKillButtonTarget.SetTarget(__instance, null, role);
                    __instance.graphic.sprite = TownOfSushi.DropSprite;
                    return false;
                }
                else
                {
                    if (!__instance.enabled) return false;
                    var abilityUsed = Utils.AbilityUsed(PlayerControl.LocalPlayer);
                    if (!abilityUsed) return false;
                    Vector3 position = PlayerControl.LocalPlayer.transform.position;

                    if (SubmergedCompatibility.isSubmerged())
                    {
                        if (position.y > -7f)
                        {
                            position.z = 0.0208f;
                        }
                        else
                        {
                            position.z = -0.0273f;
                        }
                    }

                    position.y -= 0.3636f;

                    Utils.Rpc(CustomRPC.Drop, PlayerControl.LocalPlayer.PlayerId, position, position.z);

                    var body = role.CurrentlyDragging;
                    foreach (var body2 in role.CurrentlyDragging.bodyRenderers) body2.material.SetFloat("_Outline", 0f);
                    role.CurrentlyDragging = null;
                    __instance.graphic.sprite = TownOfSushi.DragSprite;
                    role.LastDragged = DateTime.UtcNow;

                    body.transform.position = position;

                    return false;
                }
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(KillButton), nameof(KillButton.SetTarget))]
    public class UndertakerKillButtonTarget
    {
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Undertaker)) return true;
            return __instance == DestroyableSingleton<HudManager>.Instance.KillButton;
        }

        public static void SetTarget(KillButton __instance, DeadBody target, Undertaker role)
        {
            if (role.CurrentTarget && role.CurrentTarget != target)
            {
                foreach (var body in role.CurrentTarget.bodyRenderers) body.material.SetFloat("_Outline", 0f);
            }

            role.CurrentTarget = target;
            if (role.CurrentTarget && __instance.enabled)
            {
                SpriteRenderer component = null;
                foreach (var body in role.CurrentTarget.bodyRenderers) component = body;
                component.material.SetFloat("_Outline", 1f);
                component.material.SetColor("_OutlineColor", Color.yellow);
                __instance.graphic.color = Palette.EnabledColor;
                __instance.graphic.material.SetFloat("_Desat", 0f);
                return;
            }

            __instance.graphic.color = Palette.DisabledClear;
            __instance.graphic.material.SetFloat("_Desat", 1f);
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public static class UndertakerDragBody
    {
        public static void Postfix(PlayerControl __instance)
        {
            if (!__instance.Is(RoleEnum.Undertaker)) return;
            var role = GetRole<Undertaker>(__instance);
            var body = role.CurrentlyDragging;
            if (body == null) return;
            if (__instance.Data.IsDead)
            {
                role.CurrentlyDragging = null;
                foreach (var body2 in body.bodyRenderers) body2.material.SetFloat("_Outline", 0f);
                return;
            }
            var currentPosition = __instance.transform.position;
            var velocity = __instance.gameObject.GetComponent<Rigidbody2D>().velocity.normalized;
            Vector3 newPos = ((Vector2)__instance.transform.position) - (velocity / 3) + body.myCollider.offset;
            newPos.z = currentPosition.z;

            //WHY ARE THERE DIFFERENT LOCAL Z INDEXS FOR DIFFERENT DECALS ON DIFFERENT LEVELS?!?!?!
            if (SubmergedCompatibility.isSubmerged())
            {
                if (newPos.y > -7f)
                {
                    newPos.z = 0.0208f;
                } else
                {
                    newPos.z = -0.0273f;
                }
            }

            if (!PhysicsHelpers.AnythingBetween(
                currentPosition,
                newPos,
                Constants.ShipAndObjectsMask,
                false
            )) body.transform.position = newPos;
            if (!__instance.AmOwner) return;
            foreach (var body2 in body.bodyRenderers)
            {
                body2.material.SetColor("_OutlineColor", Color.green);
                body2.material.SetFloat("_Outline", 1f);
            }
        }
    }

    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.FixedUpdate))]
    public static class UndertakerPlayerPhysics_FixedUpdate
    {
        public static void Postfix(PlayerPhysics __instance)
        {
            if (__instance.myPlayer.Is(RoleEnum.Undertaker))
            {
                var role = GetRole<Undertaker>(__instance.myPlayer);
                if (role.CurrentlyDragging != null)
                    if (__instance.AmOwner && GameData.Instance && __instance.myPlayer.CanMove)
                        __instance.body.velocity *= CustomGameOptions.UndertakerDragSpeed;
            }
        }
    }
}