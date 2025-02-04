namespace TownOfSushi.Roles
{
    public class BountyHunter : Role
    {
        public PlayerControl Bounty = null;
        public DateTime HuntEnd;
        public TMPro.TextMeshPro BountyCooldown;
        public bool Hunting = false;
        public bool GameStarted = false;
        public ArrowBehaviour PreyArrow;
        public BountyHunter(PlayerControl player) : base(player)
        {
            Name = "Bounty Hunter";
            StartText = () => $"Hunt your bounties down";
            TaskText = () => 
                Bounty == null ? "Kill your given targets" : "Hunt Down your bounty " + Bounty.GetDefaultOutfit().PlayerName;
            RoleInfo = $"As the Bounty Hunter, you are given a target, which your task is to eliminate them, killing your target gives you a short cooldown, else will give you a long penalty cooldown.";
            LoreText = "Driven by greed and the thrill of the chase, the Bounty Hunter roams the ship with deadly precision. Their targets are not random; they are chosen with purpose, and each kill sharpens their resolve. Fueled by a relentless pursuit of their marks, the Bounty Hunter stops at nothing to fulfill their grim contract and secure their place as the most feared among the stars.";
            Color = Palette.ImpostorRed;
            RoleType = RoleEnum.BountyHunter;
            AddToRoleHistory(RoleType);
            RoleAlignment = RoleAlignment.ImpPower;
            Faction = Faction.Impostors;
            HuntEnd = DateTime.UtcNow;
        }
        public PlayerControl AddBounty(PlayerControl toRemove = null)
        {
            var targets = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && !x.Data.Disconnected && !x.Is(Faction.Impostors) && x != toRemove && x != ShowRoundOneShield.FirstRoundShielded).ToList();
            //exclude romantic if bounty hunter is beloved
            if (Player.IsBeloved())targets = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && !x.Data.Disconnected && !x.Is(Faction.Impostors) && !x.Is(RoleEnum.Romantic) && x != PlayerControl.LocalPlayer && x != ShowRoundOneShield.FirstRoundShielded && x != toRemove).ToList();
            if (targets.Count == 0) targets = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && !x.Data.Disconnected && !x.Is(Faction.Impostors) && x != PlayerControl.LocalPlayer && x != ShowRoundOneShield.FirstRoundShielded && x != toRemove).ToList();

            PlayerControl result = null;
            foreach (var player in targets)
            {
                if (player.Data.IsDead || player.Data.Disconnected || player.PlayerId == Player.PlayerId) continue;
                result = player;
                break; // Exit the loop once a valid player is found
            }

            // If no valid player is found, return a random player from the targets list
            if (result == null && targets.Count > 0)
            {
                result = targets[Random.Range(0, targets.Count)];
            }

            return result;
        }
        public float HuntTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = HuntEnd - utcNow;
            var flag = (float)timeSpan.TotalMilliseconds < 0f;
            if (flag) return 0;
            return ((float)timeSpan.TotalMilliseconds) / 1000f;
        }
        public void StopHunt()
        {
            Hunting = false;
            Bounty = null;
            if (PreyArrow != null) Object.Destroy(PreyArrow);
            if (PreyArrow.gameObject != null) Object.Destroy(PreyArrow.gameObject);
            PreyArrow = null;
            ReDoTaskText();
        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class BountyHunterHudManagerUpdate
    {
        public static Sprite Sprite => TownOfSushi.Arrow;
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.BountyHunter)) return;
            var role = GetRole<BountyHunter>(PlayerControl.LocalPlayer);

            if (role.BountyCooldown == null)
            {
                role.BountyCooldown = Object.Instantiate(__instance.KillButton.cooldownTimerText, __instance.KillButton.transform);
                role.BountyCooldown.gameObject.SetActive(false);
                role.BountyCooldown.transform.localPosition = new Vector3(
                    role.BountyCooldown.transform.localPosition.x + 0.26f,
                    role.BountyCooldown.transform.localPosition.y + 0.29f,
                    role.BountyCooldown.transform.localPosition.z);
                role.BountyCooldown.transform.localScale *= 0.65f;
                role.BountyCooldown.alignment = TMPro.TextAlignmentOptions.Right;
                role.BountyCooldown.fontStyle = TMPro.FontStyles.Bold;
                role.BountyCooldown.enableWordWrapping = false;
            }
            if (role.BountyCooldown != null)
            {
                role.BountyCooldown.text = Convert.ToInt32(Math.Round(role.HuntTimer())).ToString();
            }
            role.BountyCooldown.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !Meeting() && !IsDead()
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started && role.Hunting);

            if (role.Hunting && PlayerControl.LocalPlayer.moveable && __instance.KillButton.currentTarget != null)
            {
                role.BountyCooldown.color = Palette.EnabledColor;
                role.BountyCooldown.material.SetFloat("_Desat", 0f);
            }
            else
            {
                role.BountyCooldown.color = Palette.DisabledClear;
                role.BountyCooldown.material.SetFloat("_Desat", 1f);
            }

            if ((role.HuntTimer() == 0f || Meeting() || IsDead()) && role.Hunting)
            {
                role.StopHunt();

                if (role.Player.Is(ModifierEnum.Underdog))
                {
                    var lowerKC = VanillaOptions().currentNormalGameOptions.KillCooldown - CustomGameOptions.UnderdogKillBonus;
                    var normalKC = VanillaOptions().currentNormalGameOptions.KillCooldown;
                    var upperKC = VanillaOptions().currentNormalGameOptions.KillCooldown + CustomGameOptions.UnderdogKillBonus;
                    role.Player.SetKillTimer(UnderdogPerformKill.LastImp() ? lowerKC : (UnderdogPerformKill.IncreasedKC() ? normalKC : upperKC));
                }
                else role.Player.SetKillTimer(VanillaOptions().currentNormalGameOptions.KillCooldown);
            }

            if (!role.GameStarted && PlayerControl.LocalPlayer.killTimer > 0f) role.GameStarted = true;

            if (PlayerControl.LocalPlayer.killTimer == 0f && !role.Hunting && role.GameStarted && !IsDead())
            {
                role.Hunting = true;
                role.HuntEnd = DateTime.UtcNow.AddSeconds(CustomGameOptions.HuntDuration);
                role.Bounty = role.AddBounty();
                role.ReDoTaskText();
            }
            if (role.Bounty != null)
            {
                if (role.PreyArrow == null)
                {
                    var gameObj = new GameObject();
                    var arrow = gameObj.AddComponent<ArrowBehaviour>();
                    gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                    var renderer = gameObj.AddComponent<SpriteRenderer>();
                    renderer.sprite = Sprite;
                    renderer.color = ColorManager.Impostor;
                    arrow.image = renderer;
                    gameObj.layer = 5;
                    arrow.target = role.Bounty.transform.position;
                    role.PreyArrow = arrow;
                }
                role.PreyArrow.target = role.Bounty.transform.position;
            }

            if (role.Bounty != null && !role.Bounty.Data.IsDead && !role.Bounty.Data.Disconnected)
            {
                if (role.Bounty.GetCustomOutfitType() != CustomPlayerOutfitType.Camouflage &&
                    role.Bounty.GetCustomOutfitType() != CustomPlayerOutfitType.Swooper)
                {
                    role.Bounty.nameText().color = new Color(0.45f, 0f, 0f);
                }
                else role.Bounty.nameText().color = Color.clear;
            }
        }
    }
}