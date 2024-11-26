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
                var modifier = Modifier.GetModifier(MorphTarget);
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
                    $"{__instance.ColorString}Morphking {morphPlayer.Data.PlayerName} ({CustomGameOptions.HitmanMorphDuration}s)</color>";
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
                        $"{__instance.ColorString}Morphking {morphPlayer.Data.PlayerName} ({CustomGameOptions.HitmanMorphDuration - Math.Round(totalMorphkTime)}s)</color>";
                    if (totalMorphkTime > CustomGameOptions.HitmanMorphDuration ||
                        PlayerControl.LocalPlayer.Data.IsDead ||
                        AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Ended)
                    {
                        PlayerControl.LocalPlayer.myTasks.Remove(mimicText);
                        //System.Console.WriteLine("Unsetting morph");
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
            public static void KillButtonUpdate(Hitman __gInstance, HudManager __instance)
            {
                if (!__gInstance.Player.Data.IsImpostor() && Rewired.ReInput.players.GetPlayer(0).GetButtonDown(8))
                    __instance.KillButton.DoClick();

                __instance.KillButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !__gInstance.Player.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Started);
                __instance.KillButton.SetCoolDown(
                    CustomGameOptions.HitmanKCd -
                    (float)(DateTime.UtcNow - __gInstance.LastKill).TotalSeconds,
                    CustomGameOptions.HitmanKCd);

                __instance.KillButton.SetTarget(null);
                __gInstance.KillTarget = null;

                if (__instance.KillButton.isActiveAndEnabled && __gInstance.Player.moveable)
                {
                    if ((CamouflageUnCamouflagePatch.IsCamouflaged && CustomGameOptions.CamoCommsKillAnyone)) SetTarget(ref __gInstance.ClosestPlayer, __instance.KillButton);
                    else SetTarget(ref __gInstance.ClosestPlayer, __instance.KillButton);
                    __gInstance.KillTarget = __gInstance.ClosestPlayer;
                }

                __gInstance.KillTarget?.myRend().material.SetColor("_OutlineColor", __gInstance.Color);
            }

            public static void KillButtonPress(Hitman __gInstance)
            {
                if (__gInstance.KillTarget != null)
                {
                    var interact = Interact(__gInstance.Player, __gInstance.KillTarget, true);
                    if (interact.AbilityUsed)
                    {
                        return;
                    }
                    else if (interact.FullCooldownReset)
                    {
                        __gInstance.LastKill = DateTime.UtcNow;
                        return;
                    }
                    else if (interact.GaReset)
                    {
                        __gInstance.LastKill = DateTime.UtcNow;
                        __gInstance.LastKill = __gInstance.LastKill.AddSeconds(CustomGameOptions.ProtectKCReset - CustomGameOptions.HitmanKCd);
                        return;
                    }
                    else if (interact.ZeroSecReset)
                    {
                        return;
                    }
                    return;
                }
            }
        }

        public static class MorphButtonHandler
        {
            public static void MorphButtonUpdate(Hitman __gInstance, HudManager __instance)
            {
                if (__gInstance.MorphButton == null)
                {
                    __gInstance.MorphButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                    __gInstance.MorphButton.gameObject.SetActive(true);
                    __gInstance.MorphButton.graphic.enabled = true;
                }

                __gInstance.MorphButton.graphic.sprite = MorphSprite;

                __gInstance.MorphButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !__gInstance.Player.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Started);
                if (__instance.UseButton != null)
                {
                    __gInstance.MorphButton.transform.position = new Vector3(
                        Camera.main.ScreenToWorldPoint(new Vector3(0, 0)).x + 0.75f,
                        __instance.UseButton.transform.position.y, __instance.UseButton.transform.position.z);
                }
                else
                {
                    __gInstance.MorphButton.transform.position = new Vector3(
                        Camera.main.ScreenToWorldPoint(new Vector3(0, 0)).x + 0.75f,
                        __instance.PetButton.transform.position.y, __instance.PetButton.transform.position.z);
                }

                if (__gInstance.IsUsingMorph)
                {
                    __gInstance.MorphButton.graphic.material.SetFloat("_Desat", 0f);
                    __gInstance.MorphButton.graphic.color = Palette.EnabledColor;
                }
                else if (!__gInstance.MorphButton.isCoolingDown && __gInstance.Player.moveable)
                {
                    __gInstance.MorphButton.isCoolingDown = false;
                    __gInstance.MorphButton.graphic.material.SetFloat("_Desat", 0f);
                    __gInstance.MorphButton.graphic.color = Palette.EnabledColor;
                }
                else
                {
                    __gInstance.MorphButton.isCoolingDown = true;
                    __gInstance.MorphButton.graphic.material.SetFloat("_Desat", 1f);
                    __gInstance.MorphButton.graphic.color = Palette.DisabledClear;
                }

                if (!__gInstance.IsUsingMorph)
                {
                    __gInstance.MorphButton.SetCoolDown(
                        CustomGameOptions.HitmanMorphCooldown -
                        (float)(DateTime.UtcNow - __gInstance.LastMorph).TotalSeconds,
                        CustomGameOptions.HitmanMorphCooldown);
                }
            }

            public static void MorphButtonPress(Hitman __gInstance)
            {
                List<byte> mimicTargets = new List<byte>();
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player != __gInstance.Player && !player.Data.Disconnected)
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
                    __gInstance.RpcSetMorphed(x);
                }, (y) =>
                {
                    return mimictargetIDs.Contains(y.PlayerId);
                });
                Coroutines.Start(pk.Open(0f, true));
            }
        }
    }
}