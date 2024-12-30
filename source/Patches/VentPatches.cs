namespace TownOfSushi.Patches
{
    [HarmonyPatch(typeof(HudManager))]
    public static class HudManagerVentPatch
    {
        [HarmonyPatch(nameof(HudManager.Update))]
        public static void Postfix(HudManager __instance)
        {
            if(__instance.ImpostorVentButton == null || __instance.ImpostorVentButton.gameObject == null || __instance.ImpostorVentButton.IsNullOrDestroyed())
                return;

            bool active = PlayerControl.LocalPlayer != null && VentPatches.CanUseVents(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer.CachedPlayerData) && !MeetingHud.Instance;
            if (active != __instance.ImpostorVentButton.gameObject.active)
            __instance.ImpostorVentButton.gameObject.SetActive(active);
        }
    }

    [HarmonyPatch(typeof(Vent), nameof(Vent.CanUse))]
    public static class VentPatches
    {
        public static bool CanUseVents(PlayerControl player, NetworkedPlayerInfo playerInfo)
        {
            if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek) return false;

            if (player.inVent)
            {
                if (PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && !x.Data.Disconnected).ToList().Count <= 2 && !player.Is(RoleEnum.Haunter) && !player.Is(RoleEnum.Phantom))
                {
                    player.MyPhysics.RpcExitVent(Vent.currentVent.Id);
                    player.MyPhysics.ExitAllVents();
                }
                return true;
            }

            if (playerInfo.IsDead)
                return false;
            
            if (PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && !x.Data.Disconnected).ToList().Count <= 2) return false;

            if (
                player.Is(RoleEnum.Engineer)
                || (player.Is(Faction.Impostors) && !player.Is(RoleEnum.Undertaker) && !player.Is(RoleEnum.Swooper))
                || (player.Is(RoleEnum.Swooper) && CustomGameOptions.SwooperVent)
                || (player.Is(RoleEnum.Undertaker) && GetRole<Undertaker>(player).CurrentlyDragging == null)
                || (player.Is(RoleEnum.Undertaker) && GetRole<Undertaker>(player).CurrentlyDragging != null && CustomGameOptions.UndertakerVentWithBody)
                || (player.Is(RoleEnum.Hitman) && GetRole<Hitman>(player).CurrentlyDragging != null && CustomGameOptions.HitmanVentWithBody)
                || (player.Is(RoleEnum.Glitch) && CustomGameOptions.GlitchVent)
                || (player.Is(RoleEnum.Vulture) && CustomGameOptions.VultureVent)
                || (player.Is(RoleEnum.Hitman) && CustomGameOptions.HitmanVent)
                || (player.Is(RoleEnum.Juggernaut) && CustomGameOptions.JuggVent)
                || (player.Is(RoleEnum.Pestilence) && CustomGameOptions.PestVent)
                || (player.Is(RoleEnum.Werewolf) && CustomGameOptions.WerewolfVent)
                || (player.Is(RoleEnum.Jester) && CustomGameOptions.JesterVent)
                || (player.Is(RoleEnum.Vampire) && CustomGameOptions.VampVent))
                return true;

            if (player.Is(RoleEnum.SerialKiller) && CustomGameOptions.SerialKillerVent)
            {
                var role = GetRole<SerialKiller>(PlayerControl.LocalPlayer);
                if (role.Stabbed) return true;
            }

            return playerInfo.IsImpostor();
        }

        public static void Postfix(Vent __instance,
            [HarmonyArgument(0)] NetworkedPlayerInfo playerInfo,
            [HarmonyArgument(1)] ref bool canUse,
            [HarmonyArgument(2)] ref bool couldUse,
            ref float __result)
        {
            float num = float.MaxValue;
            PlayerControl playerControl = playerInfo.Object;

            if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.Normal) couldUse = CanUseVents(playerControl, playerInfo) && !playerControl.MustCleanVent(__instance.Id) && (!playerInfo.IsDead || playerControl.inVent) && (playerControl.CanMove || playerControl.inVent);
            else if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek && playerControl.Data.IsImpostor()) couldUse = false;
            else couldUse = canUse;

            var ventitaltionSystem = ShipStatus.Instance.Systems[SystemTypes.Ventilation].Cast<VentilationSystem>();

            if (ventitaltionSystem != null && ventitaltionSystem.IsVentCurrentlyBeingCleaned(__instance.Id))
            {
                couldUse = false;
            }

            canUse = couldUse;

            if (canUse)
            {
                Vector3 center = playerControl.Collider.bounds.center;
                Vector3 position = __instance.transform.position;
                num = Vector2.Distance((Vector2)center, (Vector2)position);

                if (__instance.Id == 14 && SubmergedCompatibility.isSubmerged())
                    canUse &= (double)num <= (double)__instance.UsableDistance;
                else
                    canUse = ((canUse ? 1 : 0) & ((double)num > (double)__instance.UsableDistance ? 0 : (!PhysicsHelpers.AnythingBetween(playerControl.Collider, (Vector2)center, (Vector2)position, Constants.ShipOnlyMask, false) ? 1 : 0))) != 0;
                
            }

            __result = num;
        }
    }
    [HarmonyPatch(typeof(Vent), nameof(Vent.SetButtons))]
    public static class EnterVentPatch
    {
        public static bool Prefix(Vent __instance)
        {
            var player = PlayerControl.LocalPlayer;

            if (player.Is(RoleEnum.Jester) && CustomGameOptions.JesterVent)
                return CustomGameOptions.JesterVentSwitch;
            else
                return true;
        }
    }
}