namespace TownOfSushi.Roles
{
    public class Arsonist : Role
    {
        private KillButton _igniteButton;
        public PlayerControl ClosestPlayerDouse;
        public PlayerControl ClosestPlayerIgnite;
        public List<byte> DousedPlayers = new List<byte>();
        public DateTime LastDoused;
        public bool LastKiller = false;
        public int DousedAlive => DousedPlayers.Count(x => PlayerById(x) != null && PlayerById(x).Data != null && !PlayerById(x).Data.IsDead && !PlayerById(x).Data.Disconnected);
        public Arsonist(PlayerControl player) : base(player)
        {
            Name = "Arsonist";
            StartText = () => "Douse players and ignite the light";
            TaskText = () => "Douse players and ignite to kill all douses";
            RoleInfo = "The Arsonist is a Neutral role with its own win condition. They have two abilities, one is to douse other players with gasoline. The other is to ignite all doused players. The Arsonist needs to be the last killer alive to win the game.";
            LoreText = "A pyromaniac at heart, you play with fire in the dead of night. As the Arsonist, your mission is to douse players with your deadly fuel and wait for the perfect moment to strike. Once enough victims are soaked in your fire, you ignite the flames, causing an inferno that wipes out all those who are marked. But beware—if you ignite too early, you risk exposing yourself before you’re ready. Patience and precision are your greatest tools.";
            Color = ColorManager.Arsonist;
            LastDoused = DateTime.UtcNow;
            RoleType = RoleEnum.Arsonist;
            Faction = Faction.Neutral;
            AddToRoleHistory(RoleType);
            RoleAlignment = RoleAlignment.NeutralKilling;
        }

        public KillButton IgniteButton
        {
            get => _igniteButton;
            set
            {
                _igniteButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public float DouseTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastDoused;
            var num = CustomGameOptions.DouseCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public void Ignite()
        {
            foreach (var playerId in DousedPlayers)
            {
                var player = PlayerById(playerId);
                if (!player.IsShielded() && !player.IsFortified() && !player.IsProtected() && player != ShowRoundOneShield.FirstRoundShielded)
                {
                    RpcMultiMurderPlayer(Player, player);
                }
                else if (player.IsShielded())
                {
                    var medic = player.GetMedic().Player.PlayerId;
                    StartRPC(CustomRPC.AttemptSound, medic, player.PlayerId);
                    MedicStopKill.BreakShield(medic, player.PlayerId, CustomGameOptions.ShieldBreaks);
                }
                GameHistory.CreateDeathReason(player, CustomDeathReason.Arson, Player);
            }
            DousedPlayers.Clear();
        }
    }

    [HarmonyPatch(typeof(AirshipExileController), nameof(AirshipExileController.WrapUpAndSpawn))]
    public static class ArsonistAirshipExileController_WrapUpAndSpawn
    {
        public static void Postfix(AirshipExileController __instance) => ArsonistSetLastKillerBool.ExileControllerPostfix(__instance);
    }

    [HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
    public class ArsonistSetLastKillerBool
    {

        public static void ExileControllerPostfix(ExileController __instance)
        {
            var exiled = __instance.initData.networkedPlayer?.Object;
            if (!LocalPlayer().Is(RoleEnum.Arsonist)) return;
            var alives = AllPlayers()
                    .Where(x => !x.Data.IsDead && !x.Data.Disconnected && x != LocalPlayer()).ToList();
            foreach (var player in alives)
            {
                if (player.Is(Faction.Impostors) || player.Is(RoleAlignment.NeutralKilling)) return;
            }
            var role = GetRole<Arsonist>(LocalPlayer());
            role.LastKiller = true;
            return;
        }

        public static void Postfix(ExileController __instance) => ExileControllerPostfix(__instance);

        [HarmonyPatch(typeof(Object), nameof(Object.Destroy), new Type[] { typeof(GameObject) })]
        public static void Prefix(GameObject obj)
        {
            if (!SubmergedLoaded || OptionsManager()?.currentNormalGameOptions?.MapId != 6) return;
            if (obj.name?.Contains("ExileCutscene") == true) ExileControllerPostfix(ExileControllerPatch.lastExiled);
        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class ArsonistHudManagerUpdate
    {
        public static Sprite IgniteSprite => TownOfSushi.IgniteSprite;
        
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (NullLocalPlayer()) return;
            if (NullLocalPlayerData()) return;
            if (!LocalPlayer().Is(RoleEnum.Arsonist)) return;
            var role = GetRole<Arsonist>(LocalPlayer());

            if (role.IgniteButton == null)
            {
                role.IgniteButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.IgniteButton.graphic.enabled = true;
                role.IgniteButton.gameObject.SetActive(false);
            }

            foreach (var player1 in role.DousedPlayers)
            {
                var player = PlayerById(player1);
                var data = player?.Data;
                if (data == null || data.Disconnected || data.IsDead || IsDead())
                    continue;
                var nameText = player.nameText();
                if (nameText != null)
                {
                    nameText.text += "<color=#FF4D00FF> [♨]</color>";
                }
            }

            role.IgniteButton.graphic.sprite = IgniteSprite;
            role.IgniteButton.transform.localPosition = new Vector3(-2f, 0f, 0f);

            __instance.KillButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !Meeting() && !IsDead()
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            role.IgniteButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !Meeting() && !IsDead()
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            if (!role.LastKiller || !CustomGameOptions.IgniteCdRemoved) role.IgniteButton.SetCoolDown(role.DouseTimer(), CustomGameOptions.DouseCd);
            else role.IgniteButton.SetCoolDown(0f, CustomGameOptions.DouseCd);
            if (role.DousedAlive < CustomGameOptions.MaxDoused)
            {
                __instance.KillButton.SetCoolDown(role.DouseTimer(), CustomGameOptions.DouseCd);
            }

            var notDoused = AllPlayers().Where(
                player => !role.DousedPlayers.Contains(player.PlayerId)
            ).ToList();
            var doused = AllPlayers().Where(
                player => role.DousedPlayers.Contains(player.PlayerId)
            ).ToList();

            if (role.DousedAlive < CustomGameOptions.MaxDoused)
            {
                if (CamouflageUnCamouflagePatch.IsCamouflaged && CustomGameOptions.CamoCommsKillAnyone) SetTarget(ref role.ClosestPlayerDouse, __instance.KillButton, float.NaN, notDoused);
                else SetTarget(ref role.ClosestPlayerDouse, __instance.KillButton, float.NaN, notDoused);
            }
            else __instance.KillButton.SetTarget(null);

            if (role.DousedAlive > 0)
            {
                if (CamouflageUnCamouflagePatch.IsCamouflaged && CustomGameOptions.CamoCommsKillAnyone) SetTarget(ref role.ClosestPlayerIgnite, role.IgniteButton, float.NaN, doused);
                else SetTarget(ref role.ClosestPlayerIgnite, role.IgniteButton, float.NaN, doused);
            }
            else role.IgniteButton.SetTarget(null);

            return;
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
    public static class ArsonistMeetingHudUpdate
    {
        public static void Postfix(MeetingHud __instance)
        {
            var _role = GetPlayerRole(LocalPlayer());
            if (_role?.RoleType != RoleEnum.Arsonist) return;
            if (LocalPlayer().Data.IsDead) return;
            var role = (Arsonist)_role;
            foreach (var state in __instance.playerStates)
            {
                var targetId = state.TargetPlayerId;
                var playerData = PlayerById(targetId)?.Data;
                if (playerData == null || playerData.Disconnected)
                {
                    role.DousedPlayers.Remove(targetId);
                    continue;
                }
                if (role.DousedPlayers.Contains(targetId)) state.NameText.text += "<color=#FF4D00FF> [♨]</color>";
            }
        }
    }

    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformIgniteOrDouse
    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = LocalPlayer().Is(RoleEnum.Arsonist);
            if (!flag) return true;
            if (IsDead()) return false;
            if (!LocalPlayer().CanMove) return false;
            var role = GetRole<Arsonist>(LocalPlayer());
            if (!__instance.isActiveAndEnabled || __instance.isCoolingDown) return false;

            if (__instance == role.IgniteButton && role.DousedAlive > 0)
            {
                if (role.DouseTimer() == 0 || (role.LastKiller && CustomGameOptions.IgniteCdRemoved))
                {
                    if (role.ClosestPlayerIgnite == null) return false;
                    var distBetweenPlayers2 = GetDistBetweenPlayers(LocalPlayer(), role.ClosestPlayerIgnite);
                    var flag3 = distBetweenPlayers2 <
                                KillDistance();
                    if (!flag3) return false;
                    if (!role.DousedPlayers.Contains(role.ClosestPlayerIgnite.PlayerId)) return false;

                    var interact2 = Interact(LocalPlayer(), role.ClosestPlayerIgnite);
                    if (interact2[3] == true) 
                    {
                        role.Ignite();
                    }
                    if (interact2[0] == true)
                    {
                        role.LastDoused = DateTime.UtcNow;
                        return false;
                    }
                    else if (interact2[1] == true)
                    {
                        role.LastDoused = DateTime.UtcNow;
                        role.LastDoused.AddSeconds(CustomGameOptions.ProtectKCReset - CustomGameOptions.DouseCd);
                        return false;
                    }
                    else if (interact2[2] == true) return false;
                    return false;
                }
                else return false;
            }

            if (__instance != HUDManager().KillButton) return true;
            if (role.DousedAlive == CustomGameOptions.MaxDoused) return false;
            if (role.ClosestPlayerDouse == null) return false;
            var distBetweenPlayers = GetDistBetweenPlayers(LocalPlayer(), role.ClosestPlayerDouse);
            var flag2 = distBetweenPlayers <
                        KillDistance();
            if (!flag2) return false;
            if (role.DousedPlayers.Contains(role.ClosestPlayerDouse.PlayerId)) return false;
            var interact = Interact(LocalPlayer(), role.ClosestPlayerDouse);
            if (interact[3] == true) 
            {
                role.DousedPlayers.Add(role.ClosestPlayerDouse.PlayerId);
            }
            if (interact[0] == true)
            {
                role.LastDoused = DateTime.UtcNow;
                return false;
            }
            else if (interact[1] == true)
            {
                role.LastDoused = DateTime.UtcNow;
                role.LastDoused.AddSeconds(CustomGameOptions.ProtectKCReset - CustomGameOptions.DouseCd);
                return false;
            }
            else if (interact[2] == true) return false;
            return false;
        }
    }
}