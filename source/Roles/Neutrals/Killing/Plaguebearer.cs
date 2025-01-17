namespace TownOfSushi.Roles
{
    public class Plaguebearer : Role
    {
        public PlayerControl ClosestPlayer;
        public List<byte> InfectedPlayers = new List<byte>();
        public DateTime LastInfected;
        public int InfectedAlive => InfectedPlayers.Count(x => PlayerById(x) != null && PlayerById(x).Data != null && !PlayerById(x).Data.IsDead && !PlayerById(x).Data.Disconnected);
        public bool CanTransform => PlayerControl.AllPlayerControls.ToArray().Count(x => x != null && !x.Data.IsDead && !x.Data.Disconnected) <= InfectedAlive;

        public Plaguebearer(PlayerControl player) : base(player)
        {
            Name = "Plaguebearer";
            StartText = () => "Infect Everyone To Become Pestilence";
            TaskText = () => "Infect everyone to become Pestilence";
            RoleInfo = "The Plaguebearer is a Neutral role with its own win condition, as well as an ability to transform into another role. The Plaguebearer has one ability, which allows them to infect other players. Once infected, the infected player can go and infect other players via interacting with them. Once all players are infected, the Plaguebearer becomes Pestilence.";
            LoreText ="You are the Plaguebearer, the origin of all sickness and decay. Your task is to infect the crew with your deadly plague, spreading misery wherever you go. Each infected person brings you closer to becoming Pestilence itself, unlocking the full power of your devastating abilities. Be strategic, for every player you infect moves you one step closer to your ultimate transformation. Your mission is to make the crew fall, one by one, until you are the last one standing as the unstoppable force of death.";
            Color = Colors.Plaguebearer;
            RoleType = RoleEnum.Plaguebearer;
            Faction = Faction.Neutral;
            AddToRoleHistory(RoleType);
            RoleAlignment = RoleAlignment.NeutralKilling;
            InfectedPlayers.Add(player.PlayerId);
        }
        public float InfectTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastInfected;
            var num = CustomGameOptions.InfectCd * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public void RpcSpreadInfection(PlayerControl source, PlayerControl target)
        {
            new WaitForSeconds(1f);
            SpreadInfection(source, target);
            StartRPC(CustomRPC.Infect, Player.PlayerId, source.PlayerId, target.PlayerId);
        }

        public void SpreadInfection(PlayerControl source, PlayerControl target)
        {
            if (InfectedPlayers.Contains(source.PlayerId) && !InfectedPlayers.Contains(target.PlayerId)) InfectedPlayers.Add(target.PlayerId);
            else if (InfectedPlayers.Contains(target.PlayerId) && !InfectedPlayers.Contains(source.PlayerId)) InfectedPlayers.Add(source.PlayerId);
        }
        public void TurnPestilence()
        {
            var oldRole = GetPlayerRole(Player);
            var killsList = (oldRole.Kills, oldRole.CorrectKills,  oldRole.CorrectDeputyShot, oldRole.CorrectShot, oldRole.IncorrectShots, oldRole.CorrectVigilanteShot, oldRole.CorrectAssassinKills, oldRole.IncorrectAssassinKills);
            RoleDictionary.Remove(Player.PlayerId);
            var role = new Pestilence(Player);
            role.Kills = killsList.Kills;
            role.CorrectVigilanteShot = killsList.CorrectVigilanteShot;
            role.CorrectKills = killsList.CorrectKills;
            role.IncorrectShots = killsList.IncorrectShots;
            role.CorrectShot = killsList.CorrectShot;
            role.CorrectDeputyShot = killsList.CorrectDeputyShot;
            role.CorrectAssassinKills = killsList.CorrectAssassinKills;
            role.IncorrectAssassinKills = killsList.IncorrectAssassinKills;
            if (Player == PlayerControl.LocalPlayer)
            {
                Flash(Colors.Pestilence, 2.5f);
                SoundManager.Instance.PlaySound(ShipStatus.Instance.SabotageSound, false, 1f, null);
                role.ReDoTaskText();
            }
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CmdReportDeadBody))]
    public class PlagueBodyReport
    {
        private static void Postfix(PlayerControl __instance, [HarmonyArgument(0)] NetworkedPlayerInfo info)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (info == null) return;

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (!info.Disconnected && player.PlayerId == info.PlayerId)
                {
                    if (PlayerControl.LocalPlayer.IsInfected() || player.IsInfected())
                    {
                        foreach (var pb in GetRoles(RoleEnum.Plaguebearer)) ((Plaguebearer)pb).RpcSpreadInfection(PlayerControl.LocalPlayer, player);
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformInfect
    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Plaguebearer);
            if (!flag) return true;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (!__instance.isActiveAndEnabled || __instance.isCoolingDown) return false;
            var role = GetRole<Plaguebearer>(PlayerControl.LocalPlayer);
            if (role.InfectTimer() != 0) return false;
            if (role.ClosestPlayer == null) return false;
            if (role.InfectedPlayers.Contains(role.ClosestPlayer.PlayerId)) return false;
            var distBetweenPlayers = Utils.GetDistBetweenPlayers(PlayerControl.LocalPlayer, role.ClosestPlayer);
            var flag3 = distBetweenPlayers <
                        GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
            if (!flag3) return false;
            var interact = Interact(PlayerControl.LocalPlayer, role.ClosestPlayer);
            if (interact[0] == true)
            {
                role.LastInfected = DateTime.UtcNow;
                return false;
            }
            else if (interact[1] == true)
            {
                role.LastInfected = DateTime.UtcNow;
                role.LastInfected.AddSeconds(CustomGameOptions.ProtectKCReset - CustomGameOptions.InfectCd);
                return false;
            }
            else if (interact[2] == true) return false;
            return false;
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
    public static class PlagueMeetingHudUpdate
    {
        public static void Postfix(MeetingHud __instance)
        {
            var localPlayer = PlayerControl.LocalPlayer;
            var _role = GetPlayerRole(localPlayer);
            if (_role?.RoleType != RoleEnum.Plaguebearer) return;
            if (localPlayer.Data.IsDead) return;
            var role = (Plaguebearer)_role;
            foreach (var state in __instance.playerStates)
            {
                var targetId = state.TargetPlayerId;
                var playerData = PlayerById(targetId)?.Data;
                if (playerData == null || playerData.Disconnected)
                {
                    role.InfectedPlayers.Remove(targetId);
                    continue;
                }
                if (role.InfectedPlayers.Contains(targetId) && role.Player.PlayerId != targetId) state.NameText.text += "<color=#E6FFB3FF> [♨]</color>";
            }
        }
    }

     [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HudInfect
    {
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Plaguebearer)) return;
            var isDead = PlayerControl.LocalPlayer.Data.IsDead;
            var infectButton = __instance.KillButton;
            var role = GetRole<Plaguebearer>(PlayerControl.LocalPlayer);

            foreach (var player1 in role.InfectedPlayers)
            {
                var player = PlayerById(player1);
                var data = player?.Data;
                if ((data == null || data.Disconnected || data.IsDead || PlayerControl.LocalPlayer.Data.IsDead) && player != role.Player)
                    continue;
                var nameText = player.nameText();
                if (nameText != null)
                {
                    nameText.text += "<color=#E6FFB3FF> [♨]</color>";
                }
            }

            infectButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            infectButton.SetCoolDown(role.InfectTimer(), CustomGameOptions.InfectCd);

            var notInfected = PlayerControl.AllPlayerControls.ToArray().Where(
                player => !role.InfectedPlayers.Contains(player.PlayerId)
            ).ToList();

            SetTarget(ref role.ClosestPlayer, __instance.KillButton, float.NaN, notInfected);

            if (role.CanTransform && (PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && !x.Data.Disconnected).ToList().Count > 1) && !isDead)
            {
                var transform = false;
                var alives = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && !x.Data.Disconnected && x != PlayerControl.LocalPlayer).ToList();
                if (alives.Count <= 1)
                {
                    foreach (var player in alives)
                    {
                        if (player.Data.IsImpostor() || player.Is(RoleAlignment.NeutralKilling))
                        {
                            transform = true;
                        }
                    }
                }
                else transform = true;
                if (transform)
                {
                    role.TurnPestilence();
                    StartRPC(CustomRPC.TurnPestilence, PlayerControl.LocalPlayer.PlayerId);
                }
            }
        }
    }
}