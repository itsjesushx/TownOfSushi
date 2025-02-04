using TMPro;

namespace TownOfSushi.Roles
{
    public class Trapper : Role
    {
        public static Material trapMaterial = TownOfSushi.bundledAssets.Get<Material>("trap");
        public List<Trap> traps = new List<Trap>();
        public DateTime LastTrapped { get; set; }
        public int MaxUses;
        public TextMeshPro UsesText;
        public List<RoleEnum> trappedPlayers;
        public bool ButtonUsable => MaxUses != 0;
        public Trapper(PlayerControl player) : base(player)
        {
            var playerOrPlayers = CustomGameOptions.MinAmountOfPlayersInTrap == 1 ? "player" : "players";
            Name = "Trapper";
            StartText = () => "Catch Killers In The Act";
            TaskText = () => "Place traps to find roles";
            RoleInfo = $"The Trapper is able to place traps around the map, when a player walks over the trap  for {CustomGameOptions.MinAmountOfTimeInTrap} seconds they will be caught in it. The Trapper can then see what roles were caught in their trap during the meeting if the players that walked over the trap were min {CustomGameOptions.MinAmountOfPlayersInTrap} {playerOrPlayers}.";
            LoreText = "A stealthy and strategic expert, you specialize in setting traps to catch the killers in the act. As the Trapper, you can place traps around the map to catch unsuspecting players. Your keen sense of timing and knowledge of the environment make you a crucial asset in hunting down the Impostors hiding among the crew.";
            Color = ColorManager.Trapper;
            RoleType = RoleEnum.Trapper;
            AddToRoleHistory(RoleType);
            LastTrapped = DateTime.UtcNow;
            Faction = Faction.Crewmates;
            RoleAlignment = RoleAlignment.CrewInvest;
            trappedPlayers = new List<RoleEnum>();
            MaxUses = CustomGameOptions.MaxTraps;
        }

        public float TrapTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastTrapped;
            var num = CustomGameOptions.TrapCooldown * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }

    [HarmonyPatch(typeof(HudManager))]
    public class HudTrap
    {
        [HarmonyPatch(nameof(HudManager.Update))]
        public static void Postfix(HudManager __instance)
        {
            UpdateTrapButton(__instance);
        }

        public static void UpdateTrapButton(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Trapper)) return;
            var trapButton = __instance.KillButton;

            var role = GetRole<Trapper>(PlayerControl.LocalPlayer);

            if (role.UsesText == null && role.MaxUses > 0)
            {
                role.UsesText = Object.Instantiate(trapButton.cooldownTimerText, trapButton.transform);
                role.UsesText.gameObject.SetActive(false);
                role.UsesText.transform.localPosition = new Vector3(
                    role.UsesText.transform.localPosition.x + 0.26f,
                    role.UsesText.transform.localPosition.y + 0.29f,
                    role.UsesText.transform.localPosition.z);
                role.UsesText.transform.localScale = role.UsesText.transform.localScale * 0.65f;
                role.UsesText.alignment = TMPro.TextAlignmentOptions.Right;
                role.UsesText.fontStyle = TMPro.FontStyles.Bold;
            }
            if (role.UsesText != null)
            {
                role.UsesText.text = role.MaxUses + "";
            }

            trapButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !Meeting() && !IsDead()
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            role.UsesText.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !Meeting() && !IsDead()
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            if (role.ButtonUsable) trapButton.SetCoolDown(role.TrapTimer(), CustomGameOptions.TrapCooldown);
            else trapButton.SetCoolDown(0f, CustomGameOptions.TrapCooldown);

            var renderer = trapButton.graphic;
            if (!trapButton.isCoolingDown && trapButton.gameObject.active && role.ButtonUsable)
            {
                renderer.color = Palette.EnabledColor;
                renderer.material.SetFloat("_Desat", 0f);
                role.UsesText.color = Palette.EnabledColor;
                role.UsesText.material.SetFloat("_Desat", 0f);
                return;
            }

            renderer.color = Palette.DisabledClear;
            renderer.material.SetFloat("_Desat", 1f);
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class MeetingStartTrapper
    {
        public static void Postfix(MeetingHud __instance)
        {
            if (IsDead()) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Trapper)) return;
            var trapperRole = GetRole<Trapper>(PlayerControl.LocalPlayer);
            if (trapperRole.trappedPlayers.Count == 0)
                {
                    HUDManager().Chat.AddChat(PlayerControl.LocalPlayer, ColorString(Color.red, "No players entered any of your traps"));
                }
                else if (trapperRole.trappedPlayers.Count < CustomGameOptions.MinAmountOfPlayersInTrap)
                {
                    HUDManager().Chat.AddChat(PlayerControl.LocalPlayer, ColorString(Color.red, "Not enough players triggered your traps"));
                }
            else
            {
                string message = "Roles caught in your trap:\n";
                foreach (RoleEnum role in trapperRole.trappedPlayers.OrderBy(x => Guid.NewGuid()))
                {
                    message += $" {role},";
                }
                message.Remove(message.Length - 1, 1);
                if (HUDManager())
                    HUDManager().Chat.AddChat(PlayerControl.LocalPlayer, message);
            }
        }
    }

    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformTrap
    {
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != HUDManager().KillButton) return true;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Trapper)) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (IsDead()) return false;
            var role = GetRole<Trapper>(PlayerControl.LocalPlayer);
            if (!(role.TrapTimer() == 0f)) return false;
            if (!__instance.enabled) return false;
            if (!role.ButtonUsable) return false;
            var abilityUsed = AbilityUsed(PlayerControl.LocalPlayer);
            if (!abilityUsed) return false;
            role.MaxUses--;
            role.LastTrapped = DateTime.UtcNow;
            var pos = PlayerControl.LocalPlayer.transform.position;
            pos.z += 0.001f;
            role.traps.Add(TrapExtentions.CreateTrap(pos));

            return false;
        }
    }
}
