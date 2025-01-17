namespace TownOfSushi.Roles.Modifiers
{
    public class Disperser : Modifier
    {
        public KillButton DisperseButton;
        public DateTime LastDispersed { get; set; }
        public int MaxUses;
        public bool ButtonUsable => MaxUses != 0;
        public Disperser(PlayerControl player) : base(player)
        {
            Name = "Disperser";
            TaskText = () => "Separate the Crew";
            Color = Colors.Impostor;
            LastDispersed = DateTime.UtcNow;
            MaxUses = CustomGameOptions.MaxDisperses;
            ModifierType = ModifierEnum.Disperser;
        }
        public float DisperseTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastDispersed;
            var num = CustomGameOptions.DisperseCooldown * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public void Disperse()
        {
            Dictionary<byte, Vector2> coordinates = GenerateDisperseCoordinates();

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte)CustomRPC.Disperse,
                SendOption.Reliable,
                -1);
            writer.Write((byte)coordinates.Count);
            foreach ((byte key, Vector2 value) in coordinates)
            {
                writer.Write(key);
                writer.Write(value);
            }
            AmongUsClient.Instance.FinishRpcImmediately(writer);

            DispersePlayersToCoordinates(coordinates);
        }

        public static void DispersePlayersToCoordinates(Dictionary<byte, Vector2> coordinates)
        {
            if (coordinates.ContainsKey(PlayerControl.LocalPlayer.PlayerId))
            {
                Flash(Colors.Impostor, 2.5f);
                if (Minigame.Instance)
                {
                    try
                    {
                        Minigame.Instance.Close();
                    }
                    catch
                    {

                    }
                }

                if (PlayerControl.LocalPlayer.inVent)
                {
                    PlayerControl.LocalPlayer.MyPhysics.RpcExitVent(Vent.currentVent.Id);
                    PlayerControl.LocalPlayer.MyPhysics.ExitAllVents();
                }
            }


            foreach ((byte key, Vector2 value) in coordinates)
            {
                PlayerControl player = PlayerById(key);
                player.transform.position = value;
                if (PlayerControl.LocalPlayer == player) PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(value);
            }

            if (PlayerControl.LocalPlayer.walkingToVent)
            {
                PlayerControl.LocalPlayer.inVent = false;
                Vent.currentVent = null;
                PlayerControl.LocalPlayer.moveable = true;
                PlayerControl.LocalPlayer.MyPhysics.StopAllCoroutines();
            }

            if (SubmergedCompatibility.isSubmerged()) SubmergedCompatibility.ChangeFloor(PlayerControl.LocalPlayer.transform.position.y > -7f);
        }

        private Dictionary<byte, Vector2> GenerateDisperseCoordinates()
        {
            List<PlayerControl> targets = PlayerControl.AllPlayerControls.ToArray().Where(player => !player.Data.IsDead && !player.Data.Disconnected).ToList();

            HashSet<Vent> vents = Object.FindObjectsOfType<Vent>().ToHashSet();

            Dictionary<byte, Vector2> coordinates = new Dictionary<byte, Vector2>(targets.Count);
            foreach (PlayerControl target in targets)
            {
                Vent vent = vents.Random();

                Vector3 destination = SendPlayerToVent(vent);
                coordinates.Add(target.PlayerId, destination);
            }
            return coordinates;
        }

        public static Vector3 SendPlayerToVent(Vent vent)
        {
            Vector2 size = vent.GetComponent<BoxCollider2D>().size;
            Vector3 destination = vent.transform.position;
            destination.y += 0.3636f;
            return destination;
        }
    }

    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformDisperse
    {
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(ModifierEnum.Disperser)) return true;

            var role = GetModifier<Disperser>(PlayerControl.LocalPlayer);
            if (__instance != role.DisperseButton) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            if (role.MaxUses <= 0) return false;
            if (!(role.DisperseTimer() == 0f)) return false;
            if (!__instance.enabled) return false;
            if (!role.ButtonUsable) return false;
            role.MaxUses--;
            role.LastDispersed = DateTime.UtcNow;
            role.Disperse();

            return false;
        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class DisperserUpdate
    {
        public static Sprite DisperseButton => TownOfSushi.DisperseSprite;

        public static void Postfix(HudManager __instance)
        {
            UpdateButtonButton(__instance);
        }

        private static void UpdateButtonButton(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(ModifierEnum.Disperser)) return;

            var role = GetModifier<Disperser>(PlayerControl.LocalPlayer);

            if (role.DisperseButton == null)
            {
                role.DisperseButton = Object.Instantiate(__instance.KillButton, __instance.transform.parent);
                role.DisperseButton.GetComponentsInChildren<TMPro.TextMeshPro>()[0].text = "";
                role.DisperseButton.graphic.enabled = true;
                role.DisperseButton.graphic.sprite = DisperseButton;
            }

            role.DisperseButton.graphic.sprite = DisperseButton;

            role.DisperseButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);

            role.DisperseButton.SetCoolDown(role.DisperseTimer(), CustomGameOptions.DisperseCooldown);
            var renderer = role.DisperseButton.graphic;


            if (__instance.UseButton != null)
            {
                var position1 = __instance.UseButton.transform.position;
                role.DisperseButton.transform.position = new Vector3(
                    Camera.main.ScreenToWorldPoint(new Vector3(0, 0)).x + 0.75f, position1.y,
                    position1.z);
            }
            else
            {
                var position1 = __instance.PetButton.transform.position;
                role.DisperseButton.transform.position = new Vector3(
                    Camera.main.ScreenToWorldPoint(new Vector3(0, 0)).x + 0.75f, position1.y,
                    position1.z);
            }

            if (role.MaxUses > 0)
            {
                renderer.color = Palette.EnabledColor;
                renderer.material.SetFloat("_Desat", 0f);
                return;
            }

            renderer.color = Palette.DisabledClear;
            renderer.material.SetFloat("_Desat", 1f);
        }
    }
}