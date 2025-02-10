using System.Collections;
using TownOfSushi.Modules.CustomColors;

namespace TownOfSushi.Roles
{
    public class Aurial : Role
    {
        public Dictionary<(Vector3, int), ArrowBehaviour> SenseArrows = new Dictionary<(Vector3, int), ArrowBehaviour>();
        public static Sprite Sprite => TownOfSushi.Arrow;
        public Aurial(PlayerControl player) : base(player)
        {
            Name = "Aurial";
            StartText = () => "Sense disturbances in your aura";
            TaskText = () => "You will sense any player ability uses inside your aura";
            Color = ColorManager.Aurial;
            RoleType = RoleEnum.Aurial;
            AddToRoleHistory(RoleType);
        }

        public IEnumerator Sense(PlayerControl player)
        {
            if (!CheckRange(player, CustomGameOptions.AuraOuterRadius)) yield break;
            var position = player.GetTruePosition();
            var gameObj = new GameObject();
            var arrow = gameObj.AddComponent<ArrowBehaviour>();
            gameObj.transform.parent = LocalPlayer().gameObject.transform;
            var renderer = gameObj.AddComponent<SpriteRenderer>();
            renderer.sprite = Sprite;
            int colourID = player.GetDefaultOutfit().ColorId;
            if (CheckRange(player, CustomGameOptions.AuraInnerRadius) && !CamouflageUnCamouflagePatch.IsCamouflaged)
            {
                if (ColorUtils.IsRainbow(colourID))
                {
                    renderer.color = ColorUtils.Rainbow;
                }
                else if (ColorUtils.IsWater(colourID))
                {
                    renderer.color = ColorUtils.Water;
                }
                else if (ColorUtils.IsFire(colourID))
                {
                    renderer.color = ColorUtils.Fire;
                }
                else if (ColorUtils.IsGalaxy(colourID))
                {
                    renderer.color = ColorUtils.Galaxy;
                }
                else if (ColorUtils.IsMonochrome(colourID))
                {
                    renderer.color = ColorUtils.Monochrome;
                }
                else
                {
                    renderer.color = Palette.PlayerColors[colourID];
                }
            }
            arrow.image = renderer;
            gameObj.layer = 5;
            arrow.target = player.transform.position;

            try { DestroyArrow(position, colourID); }
            catch { }

            SenseArrows.Add((position, colourID), arrow);

            yield return (object)new WaitForSeconds(CustomGameOptions.SenseDuration);

            try { DestroyArrow(position, colourID); }
            catch { }
        }

        public bool CheckRange(PlayerControl player, float radius)
        {
            float lightRadius = radius * ShipStatus.Instance.MaxLightRadius;
            Vector2 vector2 = new Vector2(player.GetTruePosition().x - Player.GetTruePosition().x, player.GetTruePosition().y - Player.GetTruePosition().y);
            float magnitude = vector2.magnitude;
            if (magnitude <= lightRadius) return true;
            else return false;
        }

        public void DestroyArrow(Vector3 targetArea, int colourID)
        {
            var arrow = SenseArrows.FirstOrDefault(x => x.Key == (targetArea, colourID));
            if (arrow.Value != null)
                Object.Destroy(arrow.Value);
            if (arrow.Value.gameObject != null)
                Object.Destroy(arrow.Value.gameObject);
            SenseArrows.Remove(arrow.Key);
        }
    }
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class UpdateAurialArrows
    {
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (NullLocalPlayerData()) return;
            if (NullLocalPlayer()) return;
            if (!LocalPlayer().Is(RoleEnum.Aurial)) return;

            var role = GetRole<Aurial>(LocalPlayer());

            if (IsDead())
            {
                foreach (var arrow in role.SenseArrows)
                {
                    role.DestroyArrow(arrow.Key.Item1, arrow.Key.Item2);
                }
                return;
            }

            foreach (var arrow in role.SenseArrows)
            {
                if (ColorUtils.IsRainbow(arrow.Key.Item2))
                {
                    arrow.Value.image.color = ColorUtils.Rainbow;
                }
                if (ColorUtils.IsGalaxy(arrow.Key.Item2))
                {
                    arrow.Value.image.color = ColorUtils.Galaxy;
                }
                if (ColorUtils.IsWater(arrow.Key.Item2))
                {
                    arrow.Value.image.color = ColorUtils.Water;
                }
                if (ColorUtils.IsFire(arrow.Key.Item2))
                {
                    arrow.Value.image.color = ColorUtils.Fire;
                }
                if (ColorUtils.IsMonochrome(arrow.Key.Item2))
                {
                    arrow.Value.image.color = ColorUtils.Monochrome;
                }
            }
        }
    }
}