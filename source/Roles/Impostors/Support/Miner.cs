namespace TownOfSushi.Roles
{
    public class Miner : Role
    {
        public readonly List<Vent> Vents = new List<Vent>();

        public KillButton _mineButton;
        public DateTime LastMined;


        public Miner(PlayerControl player) : base(player)
        {
            Name = "Miner";
            StartText = () => "From The Top, Make It Drop, That's A Vent";
            TaskText = () => "Place vents";
            RoleInfo = "The Miner is an Impostor that can create new vents. These vents only connect to each other, forming a new passway.";
            LoreText = "A skilled worker underground, you have the power to shape the map itself. As the Miner, you can place vents around the map, giving you and your allies new pathways for movement. Your ability to alter the landscape allows you to sneak around unnoticed, setting traps or escaping danger while the Crewmates remain unaware of the new routes you've created beneath their feet.";
            Color = ColorManager.ImpostorRed;
            LastMined = DateTime.UtcNow;
            RoleType = RoleEnum.Miner;  
            Faction = Faction.Impostors;

            AddToRoleHistory(RoleType);
            RoleAlignment = RoleAlignment.ImpSupport;
        }

        public bool CanPlace { get; set; }
        public Vector2 VentSize { get; set; }

        public KillButton MineButton
        {
            get => _mineButton;
            set
            {
                _mineButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public float MineTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastMined;
            var num = CustomGameOptions.MineCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudMine
    {
        public static Sprite MineSprite => TownOfSushi.MineSprite;

        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (LocalPlayer()== null) return;
            if (LocalPlayer().Data == null) return;
            if (!LocalPlayer().Is(RoleEnum.Miner)) return;
            var role = GetRole<Miner>(LocalPlayer());
            if (role.MineButton == null)
            {
                role.MineButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.MineButton.graphic.enabled = true;
                role.MineButton.gameObject.SetActive(false);
            }

            role.MineButton.graphic.sprite = MineSprite;
            role.MineButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !Meeting() && !IsDead()
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);

            role.MineButton.SetCoolDown(role.MineTimer(), CustomGameOptions.MineCd);
            var hits = Physics2D.OverlapBoxAll(LocalPlayer().transform.position, role.VentSize, 0);
            hits = hits.ToArray().Where(c =>
                    (c.name.Contains("Vent") || !c.isTrigger) && c.gameObject.layer != 8 && c.gameObject.layer != 5)
                .ToArray();
            if (hits.Count == 0 && LocalPlayer().moveable == true)
            {
                role.MineButton.graphic.color = Palette.EnabledColor;
                role.MineButton.graphic.material.SetFloat("_Desat", 0f);
                role.CanPlace = true;
            }
            else
            {
                role.MineButton.graphic.color = Palette.DisabledClear;
                role.MineButton.graphic.material.SetFloat("_Desat", 1f);
                role.CanPlace = false;
            }
        }
    }

    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PlaceVent
    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = LocalPlayer().Is(RoleEnum.Miner);
            if (!flag) return true;
            if (!LocalPlayer().CanMove) return false;
            if (IsDead()) return false;
            var role = GetRole<Miner>(LocalPlayer());
            if (__instance == role.MineButton)
            {
                if (__instance.isCoolingDown) return false;
                if (!__instance.isActiveAndEnabled) return false;
                if (!role.CanPlace) return false;
                if (role.MineTimer() != 0) return false;
                if (GetPlayerElevator(LocalPlayer()).Item1) return false;
                var abilityUsed = AbilityUsed(LocalPlayer());
                if (!abilityUsed) return false;
                var position = LocalPlayer().transform.position;
                var id = GetAvailableId();
                StartRPC(CustomRPC.Mine, id, LocalPlayer().PlayerId, position, position.z + 0.001f);
                SpawnVent(id, role, position, position.z + 0.001f);
                return false;
            }

            return true;
        }

        public static void SpawnVent(int ventId, Miner role, Vector2 position, float zAxis)
        {
            var ventPrefab = Object.FindObjectOfType<Vent>();
            var vent = Object.Instantiate(ventPrefab, ventPrefab.transform.parent);
            
            vent.Id = ventId;
            vent.transform.position = new Vector3(position.x, position.y, zAxis);

            if (role.Vents.Count > 0)
            {
                var leftVent = role.Vents[^1];
                vent.Left = leftVent;
                leftVent.Right = vent;
            }
            else
            {
                vent.Left = null;
            }

            vent.Right = null;
            vent.Center = null;

            var allVents = Ship().AllVents.ToList();
            allVents.Add(vent);
            Ship().AllVents = allVents.ToArray();

            role.Vents.Add(vent);
            role.LastMined = DateTime.UtcNow;

            if (IsSubmerged())
            {
                vent.gameObject.layer = 12;
                vent.gameObject.AddSubmergedComponent(Classes.ElevatorMover); // just in case elevator vent is not blocked
                if (vent.gameObject.transform.position.y > -7) vent.gameObject.transform.position = new Vector3(vent.gameObject.transform.position.x, vent.gameObject.transform.position.y, 0.03f);
                else
                {
                    vent.gameObject.transform.position = new Vector3(vent.gameObject.transform.position.x, vent.gameObject.transform.position.y, 0.0009f);
                    vent.gameObject.transform.localPosition = new Vector3(vent.gameObject.transform.localPosition.x, vent.gameObject.transform.localPosition.y, -0.003f);
                }
            }
        }

        public static int GetAvailableId()
        {
            var id = 0;

            while (true)
            {
                if (Ship().AllVents.All(v => v.Id != id)) return id;
                id++;
            }
        }
    }
}