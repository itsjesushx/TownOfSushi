namespace TownOfSushi.Roles
{
    public class Juggernaut : Role
    {
        public Juggernaut(PlayerControl owner) : base(owner)
        {
            Name = "Juggernaut";
            Color = Colors.Juggernaut;
            LastKill = DateTime.UtcNow;
            RoleType = RoleEnum.Juggernaut;
            StartText = () => "Your Power Grows With Every Kill";
            TaskText = () => "With each kill your kill cooldown decreases";
            RoleInfo = "The Juggernaut is a Neutral role with its own win condition. The Juggernaut's special ability is that their kill cooldown reduces with each kill. This means in theory the Juggernaut can have a 0 second kill cooldown!. The Juggernaut needs to be the last killer alive to win the game.";
            LoreText = "A relentless force of nature, you become stronger with each life you take. As the Juggernaut, every kill fuels your power, reducing your cooldown and making you an unstoppable force. The more enemies you eliminate, the faster you can strike again, making you more dangerous with every passing moment. The crew may try to stop you, but as your strength grows, so does your ability to crush them all. Fear the Juggernaut, for once you start your rampage, there’s no stopping you.";
            Faction = Faction.Neutral;

            AddToRoleHistory(RoleType);
            RoleAlignment = RoleAlignment.NeutralKilling;
        }
        public PlayerControl ClosestPlayer;
        public DateTime LastKill { get; set; }
        public int JuggKills { get; set; } = 0;
        public float KillTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastKill;
            var num = (CustomGameOptions.JuggKCd - CustomGameOptions.ReducedKCdPerKill * JuggKills) * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }

    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class JuggernautPerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Juggernaut);
            if (!flag) return true;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            var role = GetRole<Juggernaut>(PlayerControl.LocalPlayer);
            if (role.Player.inVent) return false;
            if (role.KillTimer() != 0) return false;

            if (role.ClosestPlayer == null) return false;
            var distBetweenPlayers = GetDistBetweenPlayers(PlayerControl.LocalPlayer, role.ClosestPlayer);
            var flag3 = distBetweenPlayers <
                        KillDistance();
            if (!flag3) return false;
            var interact = Interact(PlayerControl.LocalPlayer, role.ClosestPlayer, true);
            if (interact[3] == true) return false;
            else if (interact[0] == true)
            {
                role.LastKill = DateTime.UtcNow;
                return false;
            }
            else if (interact[1] == true)
            {
                role.LastKill = DateTime.UtcNow;
                role.LastKill = role.LastKill.AddSeconds(-(CustomGameOptions.JuggKCd - CustomGameOptions.ReducedKCdPerKill * role.JuggKills) + CustomGameOptions.ProtectKCReset);
                return false;
            }
            else if (interact[2] == true) return false;
            return false;
        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class JuggernautHudManagerUpdate
    {
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Juggernaut)) return;
            var role = GetRole<Juggernaut>(PlayerControl.LocalPlayer);

            __instance.KillButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);

            if (CustomGameOptions.JuggKCd - CustomGameOptions.ReducedKCdPerKill * role.JuggKills <= 0) __instance.KillButton.SetCoolDown(role.KillTimer(), 0.01f);
            else __instance.KillButton.SetCoolDown(role.KillTimer(), CustomGameOptions.JuggKCd - CustomGameOptions.ReducedKCdPerKill * role.JuggKills);

            if (CamouflageUnCamouflagePatch.IsCamouflaged && CustomGameOptions.CamoCommsKillAnyone) SetTarget(ref role.ClosestPlayer, __instance.KillButton);
            else SetTarget(ref role.ClosestPlayer, __instance.KillButton);
        }
    }
}