using System.Collections;
using InnerNet;

namespace TownOfSushi.Roles
{
    public class Hitman : Role
    {
        public static Sprite MorphSprite = TownOfSushi.MorphSprite;
        public Hitman(PlayerControl player) : base(player)
        {
            Name = "Hitman";
            StartText = () => "Murder, drag bodies & morph into other players";
            TaskText = () => "Kill everyone and enjoy your abilities";
            RoleInfo = "The Hitman is a Neutral role that depending on settings, may spawn naturally or is the become option for the Agent. The Hitman can Morph into players to disguise itself from others. They can additionally drag bodies like the Undertaker. This role cannot spawn on Fungle.";
            LoreText = "A cold-blooded professional, you live for the thrill of the kill. As the Hitman, you are not only a deadly assassin but also a master of deception. You can eliminate targets with ruthless precision, drag their bodies away to hide the evidence, and even morph into other players to evade suspicion. Your objective is simple: eliminate everyone, and use your abilities to stay one step ahead of the crew. Every move you make is calculated, and your reign of terror will continue until you’ve achieved total victory.";
            RoleType = RoleEnum.Hitman;
            Color = Colors.Hitman;
            LastDrag = DateTime.UtcNow;
            KillTarget = null;
            IsUsingMorph = false;
            MorphButton = null;
            LastKill = DateTime.UtcNow;
            Faction = Faction.Neutral;
            AddToRoleHistory(RoleType);
            RoleAlignment = RoleAlignment.NeutralKilling;
        }
        public KillButton _dragDropButtonHitman;
        public DateTime LastDrag { get; set; }
        public DeadBody CurrentTarget { get; set; }
        public DeadBody CurrentlyDragging { get; set; }
        public KillButton DragDropButtonHitman
        {
            get => _dragDropButtonHitman;
            set
            {
                _dragDropButtonHitman = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public float DragTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastDrag;
            var num = CustomGameOptions.DragCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }
        public PlayerControl ClosestPlayer;
        public DateTime LastKill { get; set; }
        public float KillTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastKill;
            var num = CustomGameOptions.HitmanKCd * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
        public DateTime LastMorph { get; set; }
        public KillButton MorphButton { get; set; }
        public PlayerControl KillTarget { get; set; }
        public bool IsUsingMorph { get; set; }
        public PlayerControl MorphTarget { get; set; }
        public void Update(HudManager __instance)
        {
            if (HudManager.Instance?.Chat != null)
            {
                foreach (var bubble in HudManager.Instance.Chat.chatBubblePool.activeChildren)
                {
                    if (bubble.Cast<ChatBubble>().NameText != null &&
                        Player.Data.PlayerName == bubble.Cast<ChatBubble>().NameText.text)
                    {
                        bubble.Cast<ChatBubble>().NameText.color = Color;
                    }
                }
            }

            FixedUpdate(__instance);
        }

        public void FixedUpdate(HudManager __instance)
        {
            KillButtonHandler.KillButtonUpdate(this, __instance);

            MorphButtonHandler.MorphButtonUpdate(this, __instance);

            if (__instance.KillButton != null && Player.Data.IsDead)
                __instance.KillButton.SetTarget(null);

            if (MorphButton != null && Player.Data.IsDead)
                MorphButton.SetTarget(null);
        }

        public bool UseAbility(KillButton __instance)
        {
            if (__instance == MorphButton)
                MorphButtonHandler.MorphButtonPress(this);
            else
                KillButtonHandler.KillButtonPress(this);

            return false;
        }

        public void RpcSetMorphed(PlayerControl Morphed)
        {
            Coroutines.Start(AbilityCoroutine.Morph(this, Morphed));
        }

        public bool TryGetModifiedAppearance(out VisualAppearance appearance)
        {
            if (IsUsingMorph)
            {
                appearance = MorphTarget.GetDefaultAppearance();
                var modifier = GetModifier(MorphTarget);
                if (modifier is IVisualAlteration alteration)
                    alteration.TryGetModifiedAppearance(out appearance);
                return true;
            }

            appearance = Player.GetDefaultAppearance();
            return false;
        }

        public static class AbilityCoroutine
        {
            public static Dictionary<byte, DateTime> tickDictionary = new();
            public static IEnumerator Morph(Hitman __instance, PlayerControl morphPlayer)
            {
                Rpc(CustomRPC.SetHitmanMorph, PlayerControl.LocalPlayer.PlayerId, morphPlayer.PlayerId);

                var abilityUsed = AbilityUsed(PlayerControl.LocalPlayer);
                if (!abilityUsed) yield break;

                Utils.Morph(__instance.Player, morphPlayer);

                var mimicActivation = DateTime.UtcNow;
                var mimicText = new GameObject("_Player").AddComponent<ImportantTextTask>();
                mimicText.transform.SetParent(PlayerControl.LocalPlayer.transform, false);
                mimicText.Text =
                    $"{__instance.ColorString}Morphing as {morphPlayer.Data.PlayerName} ({CustomGameOptions.HitmanMorphDuration}s)</color>";
                PlayerControl.LocalPlayer.myTasks.Insert(0, mimicText);

                while (true)
                {
                    __instance.IsUsingMorph = true;
                    __instance.MorphTarget = morphPlayer;
                    var totalMorphkTime = (DateTime.UtcNow - mimicActivation).TotalMilliseconds / 1000;
                    if (__instance.Player.Data.IsDead)
                    {
                        totalMorphkTime = CustomGameOptions.HitmanMorphDuration;
                    }
                    mimicText.Text =
                        $"{__instance.ColorString}Morphing as {morphPlayer.Data.PlayerName} ({CustomGameOptions.HitmanMorphDuration - Math.Round(totalMorphkTime)}s)</color>";
                    if (totalMorphkTime > CustomGameOptions.HitmanMorphDuration ||
                        PlayerControl.LocalPlayer.Data.IsDead ||
                        AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Ended)
                    {
                        PlayerControl.LocalPlayer.myTasks.Remove(mimicText);
                        __instance.LastMorph = DateTime.UtcNow;
                        __instance.IsUsingMorph = false;
                        __instance.MorphTarget = null;
                        Unmorph(__instance.Player);

                        Rpc(CustomRPC.RpcResetAnim2, PlayerControl.LocalPlayer.PlayerId, morphPlayer.PlayerId);
                        yield break;
                    }

                    Utils.Morph(__instance.Player, morphPlayer);
                    __instance.MorphButton.SetCoolDown(CustomGameOptions.HitmanMorphDuration - (float)totalMorphkTime,
                        CustomGameOptions.HitmanMorphDuration);

                    yield return null;
                }
            }
        }

        public static class KillButtonHandler
        {
            public static void KillButtonUpdate(Hitman __hInstance, HudManager __instance)
            {
                if (!__hInstance.Player.Data.IsImpostor() && Rewired.ReInput.players.GetPlayer(0).GetButtonDown(8))
                    __instance.KillButton.DoClick();

                __instance.KillButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !__hInstance.Player.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Started);
                __instance.KillButton.SetCoolDown(
                    CustomGameOptions.HitmanKCd -
                    (float)(DateTime.UtcNow - __hInstance.LastKill).TotalSeconds,
                    CustomGameOptions.HitmanKCd);

                __instance.KillButton.SetTarget(null);
                __hInstance.KillTarget = null;

                if (__instance.KillButton.isActiveAndEnabled && __hInstance.Player.moveable)
                {
                    if (CamouflageUnCamouflagePatch.IsCamouflaged && CustomGameOptions.CamoCommsKillAnyone) SetTarget(ref __hInstance.ClosestPlayer, __instance.KillButton);
                    else SetTarget(ref __hInstance.ClosestPlayer, __instance.KillButton);
                    __hInstance.KillTarget = __hInstance.ClosestPlayer;
                }

                __hInstance.KillTarget?.myRend().material.SetColor("_OutlineColor", __hInstance.Color);
            }

            public static void KillButtonPress(Hitman __hInstance)
            {
                if (__hInstance.KillTarget != null)
                {
                    var interact = Interact(__hInstance.Player, __hInstance.KillTarget, true);
                    if (interact[3] == true)
                    {
                        return;
                    }
                    else if (interact[0] == true)
                    {
                        __hInstance.LastKill = DateTime.UtcNow;
                        return;
                    }
                    else if (interact[1] == true)
                    {
                        __hInstance.LastKill = DateTime.UtcNow;
                        __hInstance.LastKill = __hInstance.LastKill.AddSeconds(CustomGameOptions.ProtectKCReset - CustomGameOptions.HitmanKCd);
                        return;
                    }
                    else if (interact[2] == true)
                    {
                        return;
                    }
                    return;
                }
            }
        }

        public static class MorphButtonHandler
        {
            public static void MorphButtonUpdate(Hitman __hInstance, HudManager __instance)
            {
                if (__hInstance.MorphButton == null)
                {
                    __hInstance.MorphButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                    __hInstance.MorphButton.gameObject.SetActive(true);
                    __hInstance.MorphButton.graphic.enabled = true;
                }

                __hInstance.MorphButton.graphic.sprite = MorphSprite;

                __hInstance.MorphButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !__hInstance.Player.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Started);
                if (__instance.UseButton != null)
                {
                    __hInstance.MorphButton.transform.position = new Vector3(
                        Camera.main.ScreenToWorldPoint(new Vector3(0, 0)).x + 0.75f,
                        __instance.UseButton.transform.position.y, __instance.UseButton.transform.position.z);
                }
                else
                {
                    __hInstance.MorphButton.transform.position = new Vector3(
                        Camera.main.ScreenToWorldPoint(new Vector3(0, 0)).x + 0.75f,
                        __instance.PetButton.transform.position.y, __instance.PetButton.transform.position.z);
                }

                if (__hInstance.IsUsingMorph)
                {
                    __hInstance.MorphButton.graphic.material.SetFloat("_Desat", 0f);
                    __hInstance.MorphButton.graphic.color = Palette.EnabledColor;
                }
                else if (!__hInstance.MorphButton.isCoolingDown && __hInstance.Player.moveable)
                {
                    __hInstance.MorphButton.isCoolingDown = false;
                    __hInstance.MorphButton.graphic.material.SetFloat("_Desat", 0f);
                    __hInstance.MorphButton.graphic.color = Palette.EnabledColor;
                }
                else
                {
                    __hInstance.MorphButton.isCoolingDown = true;
                    __hInstance.MorphButton.graphic.material.SetFloat("_Desat", 1f);
                    __hInstance.MorphButton.graphic.color = Palette.DisabledClear;
                }

                if (!__hInstance.IsUsingMorph)
                {
                    __hInstance.MorphButton.SetCoolDown(
                        CustomGameOptions.HitmanMorphCooldown -
                        (float)(DateTime.UtcNow - __hInstance.LastMorph).TotalSeconds,
                        CustomGameOptions.HitmanMorphCooldown);
                }
            }

            public static void MorphButtonPress(Hitman __hInstance)
            {
                List<byte> mimicTargets = new List<byte>();
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player != __hInstance.Player && !player.Data.Disconnected)
                    {
                        if (!player.Data.IsDead) mimicTargets.Add(player.PlayerId);
                        else
                        {
                            foreach (var body in Object.FindObjectsOfType<DeadBody>())
                            {
                                if (body.ParentId == player.PlayerId) mimicTargets.Add(player.PlayerId);
                            }
                        }
                    }
                }
                byte[] mimictargetIDs = mimicTargets.ToArray();
                var pk = new PlayerMenu((x) =>
                {
                    __hInstance.RpcSetMorphed(x);
                }, (y) =>
                {
                    return mimictargetIDs.Contains(y.PlayerId);
                });
                Coroutines.Start(pk.Open(0f, true));
            }
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public static class HitmanDragBody
    {
        public static void Postfix(PlayerControl __instance)
        {
            if (!__instance.Is(RoleEnum.Hitman)) return;
            var role = GetRole<Hitman>(__instance);
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

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class MorphUnMorph
    {
        public static void Postfix(HudManager __instance)
        {
            foreach (var role in GetRoles(RoleEnum.Hitman))
            {
                var hitman = (Hitman) role;
                if (hitman.IsUsingMorph)
                    Morph(hitman.Player, hitman.MorphTarget);
                else if (hitman.MorphTarget) Unmorph(hitman.Player);
            }
        }
    }

    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    internal class HitmanPerformAbilities
    {
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Hitman)) return false;
            if (__instance == GetRole<Hitman>(PlayerControl.LocalPlayer).DragDropButtonHitman) return false;
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Hitman) && __instance.isActiveAndEnabled &&
                !__instance.isCoolingDown && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.inVent)
                return GetRole<Hitman>(PlayerControl.LocalPlayer).UseAbility(__instance);
            return true;
        }
    }

    [HarmonyPatch(typeof(KillButton), nameof(KillButton.SetTarget))]
    public class HitmanKillButtonTarget
    {
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Hitman)) return true;
            return __instance == DestroyableSingleton<HudManager>.Instance.KillButton;
        }

        public static void SetTarget(KillButton __instance, DeadBody target, Hitman role)
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

    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformDragButton
    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Hitman);
            if (!flag) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = GetRole<Hitman>(PlayerControl.LocalPlayer);

            if (__instance == role.DragDropButtonHitman)
            {
                if (role.DragDropButtonHitman.graphic.sprite == TownOfSushi.DragSprite)
                {
                    if (__instance.isCoolingDown) return false;
                    if (!__instance.enabled) return false;
                    var maxDistance = KillDistance();
                    if (Vector2.Distance(role.CurrentTarget.TruePosition,
                        PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance) return false;
                    var playerId = role.CurrentTarget.ParentId;
                    var player = PlayerById(playerId);
                    var abilityUsed = AbilityUsed(PlayerControl.LocalPlayer);
                    if (!abilityUsed) return false;
                    if ((player.IsInfected() || role.Player.IsInfected()) && !player.Is(RoleEnum.Plaguebearer))
                    {
                        foreach (var pb in GetRoles(RoleEnum.Plaguebearer)) ((Plaguebearer)pb).RpcSpreadInfection(player, role.Player);
                    }

                    Rpc(CustomRPC.HitmanDrag, PlayerControl.LocalPlayer.PlayerId, playerId);

                    role.CurrentlyDragging = role.CurrentTarget;

                    HitmanKillButtonTarget.SetTarget(__instance, null, role);
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

                    Rpc(CustomRPC.HitmanDrop, PlayerControl.LocalPlayer.PlayerId, position, position.z);

                    var body = role.CurrentlyDragging;
                    foreach (var body2 in role.CurrentlyDragging.bodyRenderers) body2.material.SetFloat("_Outline", 0f);
                    role.CurrentlyDragging = null;
                    __instance.graphic.sprite = TownOfSushi.DragSprite;
                    role.LastDrag = DateTime.UtcNow;

                    body.transform.position = position;

                    return false;
                }
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HitmanUpdate
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


                HitmanKillButtonTarget.SetTarget(killButton, closestBody, role);
            }

            if (role.DragDropButtonHitman.graphic.sprite == TownOfSushi.DragSprite)
            {
                role.DragDropButtonHitman.SetCoolDown(role.DragTimer(), CustomGameOptions.DragCd);
            }
            else
            {
                role.DragDropButtonHitman.SetCoolDown(0f, 1f);
                role.DragDropButtonHitman.graphic.color = Palette.EnabledColor;
                role.DragDropButtonHitman.graphic.material.SetFloat("_Desat", 0f);
            }
        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    internal class HitmanSecondUpdate
    {
        private static void Postfix(HudManager __instance)
        {
            var Hitman = AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Hitman);
            if (AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Started)
                if (Hitman != null)
                if (PlayerControl.LocalPlayer.Is(RoleEnum.Hitman))
                    GetRole<Hitman>(PlayerControl.LocalPlayer).Update(__instance);
        }
    }

    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.FixedUpdate))]
    public static class HitmanPlayerPhysics_FixedUpdate
    {
        public static void Postfix(PlayerPhysics __instance)
        {
            if (__instance.myPlayer.Is(RoleEnum.Hitman))
            {
                var role = GetRole<Hitman>(__instance.myPlayer);
                if (role.CurrentlyDragging != null)
                    if (__instance.AmOwner && GameData.Instance && __instance.myPlayer.CanMove)
                        __instance.body.velocity *= CustomGameOptions.HitmanDragSpeed;
            }
        }
    }
}