using TownOfSushi.Modules.CustomColors;

namespace TownOfSushi.Objects
{
    public class Footprint
    {
        public readonly PlayerControl Player;
        private GameObject _gameObject;
        private SpriteRenderer _spriteRenderer;
        private readonly float _time;
        private readonly Vector2 _velocity;

        public Color Color;
        public Vector3 Position;
        public Investigator Role;
        public bool IsRainbow = false;
        public bool IsGalaxy = false;
        public bool IsMonochrome = false;
        public Footprint(PlayerControl player, Investigator role)
        {
            Role = role;
            Position = player.transform.position;
            _velocity = player.gameObject.GetComponent<Rigidbody2D>().velocity;

            Player = player;
            _time = (int) Time.time;
            Color = Palette.PlayerColors[player.GetDefaultOutfit().ColorId];
            if (ColorUtils.IsRainbow(player.GetDefaultOutfit().ColorId)) IsRainbow = true;
            if (ColorUtils.IsMonochrome(player.GetDefaultOutfit().ColorId)) IsMonochrome = true;
            if (ColorUtils.IsGalaxy(player.GetDefaultOutfit().ColorId)) IsGalaxy = true;
            if (Grey || (player.Is(RoleEnum.Venerer) && GetRole<Venerer>(player).IsCamouflaged))
            {
                Color = new Color(0.2f, 0.2f, 0.2f, 1f);
                IsRainbow = false;
                IsGalaxy = false;
                IsMonochrome = false;
            }
            if (player.Is(RoleEnum.Morphling))
            {
                var morphling = GetRole<Morphling>(player);
                if (morphling.Morphed)
                {
                    Color = Palette.PlayerColors[morphling.MorphedPlayer.GetDefaultOutfit().ColorId];
                    if (ColorUtils.IsRainbow(morphling.MorphedPlayer.GetDefaultOutfit().ColorId)) IsRainbow = true;
                    else IsRainbow = false;
                }
            }
            if (player.Is(RoleEnum.Morphling))
            {
                var morphling = GetRole<Morphling>(player);
                if (morphling.Morphed)
                {
                    Color = Palette.PlayerColors[morphling.MorphedPlayer.GetDefaultOutfit().ColorId];
                    if (ColorUtils.IsMonochrome(morphling.MorphedPlayer.GetDefaultOutfit().ColorId)) IsMonochrome = true;
                    else IsMonochrome = false;
                }
            }
            if (player.Is(RoleEnum.Morphling))
            {
                var morphling = GetRole<Morphling>(player);
                if (morphling.Morphed)
                {
                    Color = Palette.PlayerColors[morphling.MorphedPlayer.GetDefaultOutfit().ColorId];
                    if (ColorUtils.IsGalaxy(morphling.MorphedPlayer.GetDefaultOutfit().ColorId)) IsGalaxy = true;
                    else IsGalaxy = false;
                }
            }
            if (player.Is(RoleEnum.Glitch))
            {
                var glitch = GetRole<Glitch>(player);
                if (glitch.IsUsingMimic)
                {
                    Color = Palette.PlayerColors[glitch.MimicTarget.GetDefaultOutfit().ColorId];
                    if (ColorUtils.IsRainbow(glitch.MimicTarget.GetDefaultOutfit().ColorId)) IsRainbow = true;
                    else IsRainbow = false;
                }
            }
            if (player.Is(RoleEnum.Glitch))
            {
                var glitch = GetRole<Glitch>(player);
                if (glitch.IsUsingMimic)
                {
                    Color = Palette.PlayerColors[glitch.MimicTarget.GetDefaultOutfit().ColorId];
                    if (ColorUtils.IsGalaxy(glitch.MimicTarget.GetDefaultOutfit().ColorId)) IsGalaxy = true;
                    else IsGalaxy = false;
                }
            }
            if (player.Is(RoleEnum.Glitch))
            {
                var glitch = GetRole<Glitch>(player);
                if (glitch.IsUsingMimic)
                {
                    Color = Palette.PlayerColors[glitch.MimicTarget.GetDefaultOutfit().ColorId];
                    if (ColorUtils.IsMonochrome(glitch.MimicTarget.GetDefaultOutfit().ColorId)) IsMonochrome = true;
                    else IsMonochrome = false;
                }
            }

            if (player.Is(RoleEnum.Hitman))
            {
                var hitman = GetRole<Hitman>(player);
                if (hitman.IsUsingMorph)
                {
                    Color = Palette.PlayerColors[hitman.MorphTarget.GetDefaultOutfit().ColorId];
                    if (ColorUtils.IsRainbow(hitman.MorphTarget.GetDefaultOutfit().ColorId)) IsRainbow = true;
                    else IsRainbow = false;
                }
            }
            if (player.Is(RoleEnum.Hitman))
            {
                var hitman = GetRole<Hitman>(player);
                if (hitman.IsUsingMorph)
                {
                    Color = Palette.PlayerColors[hitman.MorphTarget.GetDefaultOutfit().ColorId];
                    if (ColorUtils.IsGalaxy(hitman.MorphTarget.GetDefaultOutfit().ColorId)) IsGalaxy = true;
                    else IsGalaxy = false;
                }
            }
            if (player.Is(RoleEnum.Hitman))
            {
                var hitman = GetRole<Hitman>(player);
                if (hitman.IsUsingMorph)
                {
                    Color = Palette.PlayerColors[hitman.MorphTarget.GetDefaultOutfit().ColorId];
                    if (ColorUtils.IsMonochrome(hitman.MorphTarget.GetDefaultOutfit().ColorId)) IsMonochrome = true;
                    else IsMonochrome = false;
                }
            }

            Start();
            role.AllPrints.Add(this);
        }

        public static float Duration => CustomGameOptions.FootprintDuration;

        public static bool Grey =>
            CustomGameOptions.AnonymousFootPrint || CamouflageUnCamouflagePatch.IsCamouflaged;

        public static void DestroyAll(Investigator role)
        {
            while (role.AllPrints.Count != 0) role.AllPrints[0].Destroy();
        }


        private void Start()
        {
            _gameObject = new GameObject("Footprint");
            _gameObject.AddSubmergedComponent(SubmergedCompatibility.Classes.ElevatorMover);
            _gameObject.transform.position = Position;
            _gameObject.transform.Rotate(Vector3.forward * Vector2.SignedAngle(Vector2.up, _velocity));
            _gameObject.transform.SetParent(Player.transform.parent);

            _spriteRenderer = _gameObject.AddComponent<SpriteRenderer>();
            _spriteRenderer.sprite = TownOfSushi.Footprint;
            _spriteRenderer.color = Color;
            _gameObject.transform.localScale *= new Vector2(1.2f, 1f) * (CustomGameOptions.FootprintSize / 10);

            _gameObject.SetActive(true);
        }

        private void Destroy()
        {
            Object.Destroy(_gameObject);
            Role.AllPrints.Remove(this);
        }

        public bool Update()
        {
            var currentTime = Time.time;
            var alpha = Mathf.Max(1f - (currentTime - _time) / Duration, 0f);

            if (alpha < 0 || alpha > 1)
                alpha = 0;

            if (IsRainbow) Color = ColorUtils.Rainbow;
            if (IsGalaxy) Color = ColorUtils.Galaxy;
            if (IsMonochrome) Color = ColorUtils.Monochrome;

            Color = new Color(Color.r, Color.g, Color.b, alpha);
            _spriteRenderer.color = Color;

            if (_time + (int) Duration < currentTime)
            {
                Destroy();
                return true;
            }

            return false;
        }
    }
}