using TownOfSushi.Modules.ColorsMod;

namespace TownOfSushi.Roles.Crewmates.Investigative.TrackerMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static Sprite Sprite => TownOfSushi.Arrow;
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton) return true;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Tracker)) return true;
            var role = GetRole<Tracker>(PlayerControl.LocalPlayer);
            if (!PlayerControl.LocalPlayer.CanMove || role.ClosestPlayer == null) return false;
            var flag2 = role.TrackerTimer() == 0f;
            if (!flag2) return false;
            if (!__instance.enabled) return false;
            var maxDistance = KillDistance();
            if (Vector2.Distance(role.ClosestPlayer.GetTruePosition(),
                PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance) return false;
            if (role.ClosestPlayer == null) return false;
            var target = role.ClosestPlayer;
            if (!role.ButtonUsable) return false;

            var interact = Interact(PlayerControl.LocalPlayer, role.ClosestPlayer);
            if (interact[3] == true)
            {
                var gameObj = new GameObject();
                var arrow = gameObj.AddComponent<ArrowBehaviour>();
                gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                var renderer = gameObj.AddComponent<SpriteRenderer>();
                renderer.sprite = Sprite;
                if (!CamouflageUnCamouflagePatch.IsCamouflaged)
                {
                    if (ColorUtils.IsRainbow(target.GetDefaultOutfit().ColorId))
                        renderer.color = ColorUtils.Rainbow;
                    
                    else if (ColorUtils.IsMonochrome(target.GetDefaultOutfit().ColorId))
                        renderer.color = ColorUtils.Monochrome;
                        
                    else if (ColorUtils.IsGalaxy(target.GetDefaultOutfit().ColorId))
                        renderer.color = ColorUtils.Galaxy;
                    else
                    {
                        renderer.color = Palette.PlayerColors[target.GetDefaultOutfit().ColorId];
                    }
                }
                arrow.image = renderer;
                gameObj.layer = 5;
                arrow.target = target.transform.position;

                role.TrackerArrows.Add(target.PlayerId, arrow);
                role.MaxUses--;
            }
            if (interact[0] == true)
            {
                role.LastTracked = DateTime.UtcNow;
                return false;
            }
            else if (interact[1] == true)
            {
                role.LastTracked = DateTime.UtcNow;
                role.LastTracked = role.LastTracked.AddSeconds(CustomGameOptions.ProtectKCReset - CustomGameOptions.TrackCd);
                return false;
            }
            else if (interact[2] == true) return false;
            return false;
        }
    }
}
