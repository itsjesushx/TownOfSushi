namespace TownOfSushi.Patches
{
    [HarmonyPatch]
    public static class SpeedPatch
    {
        [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.FixedUpdate))]
        [HarmonyPostfix]
        public static void PostfixPhysics(PlayerPhysics __instance)
        {
            if (__instance.AmOwner && GameData.Instance && __instance.myPlayer.CanMove && !__instance.myPlayer.Data.IsDead)
            {
                __instance.body.velocity *= __instance.myPlayer.GetAppearance().SpeedFactor;
                foreach (var role in GetRoles(RoleEnum.Venerer))
                {
                    var venerer = (Venerer)role;
                    if (venerer.Enabled)
                    {
                        if (venerer.KillsAtStartAbility >= 2 && venerer.Player == LocalPlayer()) __instance.body.velocity *= CustomGameOptions.SprintSpeed;
                        else if (venerer.KillsAtStartAbility >= 3) __instance.body.velocity *= CustomGameOptions.FreezeSpeed;
                    }
                }
                foreach (var modifier in GetModifiers(ModifierEnum.Frosty))
                {
                    var frosty = (Frosty)modifier;
                    if (frosty.IsChilled && frosty.Chilled == LocalPlayer())
                    {
                        var utcNow = DateTime.UtcNow;
                        var timeSpan = utcNow - frosty.LastChilled;
                        var duration = CustomGameOptions.ChillDuration * 1000f;
                        if ((float)timeSpan.TotalMilliseconds < duration)
                        {
                            __instance.body.velocity *= 1 - (duration - (float)timeSpan.TotalMilliseconds)
                                * (1 - CustomGameOptions.ChillStartSpeed) / duration;
                        }
                        else frosty.IsChilled = false;
                    }
                }
            }
        }

        [HarmonyPatch(typeof(CustomNetworkTransform), nameof(CustomNetworkTransform.FixedUpdate))]
        [HarmonyPostfix]
        public static void PostfixNetwork(CustomNetworkTransform __instance)
        {
            if (!__instance.AmOwner && GameData.Instance && __instance.gameObject.GetComponent<PlayerControl>().CanMove && !__instance.gameObject.GetComponent<PlayerControl>().Data.IsDead)
            {
                var player = __instance.gameObject.GetComponent<PlayerControl>();
                __instance.body.velocity *= player.GetAppearance().SpeedFactor;

                foreach (var role in GetRoles(RoleEnum.Venerer))
                {
                    var venerer = (Venerer)role;
                    if (venerer.Enabled)
                    {
                        if (venerer.KillsAtStartAbility >= 2 && venerer.Player == player) __instance.body.velocity *= CustomGameOptions.SprintSpeed;
                        else if (venerer.KillsAtStartAbility >= 3) __instance.body.velocity *= CustomGameOptions.FreezeSpeed;
                    }
                }
                foreach (var modifier in GetModifiers(ModifierEnum.Frosty))
                {
                    var frosty = (Frosty)modifier;
                    if (frosty.IsChilled && frosty.Chilled == player)
                    {
                        var utcNow = DateTime.UtcNow;
                        var timeSpan = utcNow - frosty.LastChilled;
                        var duration = CustomGameOptions.ChillDuration * 1000f;
                        if ((float)timeSpan.TotalMilliseconds < duration)
                        {
                            __instance.body.velocity *= 1 - (duration - (float)timeSpan.TotalMilliseconds)
                                * (1 - CustomGameOptions.ChillStartSpeed) / duration;
                        }
                        else frosty.IsChilled = false;
                    }
                }
            }
        }
    }
    
    [HarmonyPatch]
    public static class SizePatch
    {
        public static float Radius = 0.2233912f;
        public static float Offset = 0.3636057f;

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        [HarmonyPostfix]
        public static void Postfix(HudManager __instance)
        {
            foreach (var player in AllPlayers())
            {
                CircleCollider2D collider = player.Collider.Caster<CircleCollider2D>();
                if (player.Data != null && !(player.Data.IsDead || player.Data.Disconnected))
                {
                    player.transform.localScale = player.GetAppearance().SizeFactor;
                    if (player.GetAppearance().SizeFactor == new Vector3(0.4f, 0.4f, 1.0f))
                    {
                        collider.radius = Radius * 1.75f;
                        collider.offset = Offset / 1.75f * Vector2.down;
                    }
                    else
                    {
                        collider.radius = Radius;
                        collider.offset = Offset * Vector2.down;
                    }
                }
                else
                {
                    player.transform.localScale = new Vector3(0.7f, 0.7f, 1.0f);
                    collider.radius = Radius;
                    collider.offset = Offset * Vector2.down;
                }
            }

            var playerBindings = AllPlayers().ToDictionary(player => player.PlayerId);
            var bodies = Object.FindObjectsOfType<DeadBody>();
            foreach (var body in bodies)
            {
                try {
                    body.transform.localScale = playerBindings[body.ParentId].GetAppearance().SizeFactor;
                } catch {
                }
            }
        }
    }

    public static class CanMove
    {
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CanMove), MethodType.Getter)]
        internal static class CanMovePatch
        {
            public static bool Prefix(PlayerControl __instance, ref bool __result)
            {
                __result = __instance.moveable
                        && !TaskPanel()
                        && !__instance.shapeshifting
                        && (!HudManager.InstanceExists
                        || !Chat().IsOpenOrOpening
                        && !HUDManager().KillOverlay.IsOpen
                        && !HUDManager().GameMenu.IsOpen)
                        && (!MapInstance() || !MapInstance().IsOpenStopped)
                        && !Meeting()
                        && !PlayerCustomizationMenu.Instance 
                        && !ExiledInstance()
                        && !IntroCutscene.Instance;

                return false;
            }
        }
    }
}