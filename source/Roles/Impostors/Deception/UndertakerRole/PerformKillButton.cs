namespace TownOfSushi.Roles.Impostors.Deception.UndertakerRole
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKillButton
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

                    KillButtonTarget.SetTarget(__instance, null, role);
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
}