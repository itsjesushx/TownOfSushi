namespace TownOfSushi
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class ImpostorKillPatch
    {
        [HarmonyPriority(Priority.First)]
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != HUDManager().KillButton) return true;
            if (!PlayerControl.LocalPlayer.Data.IsImpostor()) return true;
            var target = __instance.currentTarget;
            if (target == null) return true;
            if (!__instance.isActiveAndEnabled || __instance.isCoolingDown) return true;
            if (IsHideNSeek())
            {
                if (!target.inVent) RpcMurderPlayer(PlayerControl.LocalPlayer, target);
                return false;
            }
            var interact = Interact(PlayerControl.LocalPlayer, target, true);
            if (interact[3] == true) return false;
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Warlock))
            {
                var warlock = GetRole<Warlock>(PlayerControl.LocalPlayer);
                if (warlock.Charging)
                {
                    warlock.UsingCharge = true;
                    warlock.ChargeUseDuration = warlock.ChargePercent * CustomGameOptions.ChargeUseDuration / 100f;
                    if (warlock.ChargeUseDuration == 0f) warlock.ChargeUseDuration += 0.01f;
                }
                PlayerControl.LocalPlayer.SetKillTimer(0.01f);
            }
            else if (interact[0] == true)
            {
                if (PlayerControl.LocalPlayer.Is(RoleEnum.BountyHunter))
                {
                    var bh = GetRole<BountyHunter>(PlayerControl.LocalPlayer);
                    if (bh.Bounty == target) PlayerControl.LocalPlayer.SetKillTimer(CustomGameOptions.BountyHunterCorrectCd);
                    else PlayerControl.LocalPlayer.SetKillTimer(VanillaOptions().currentNormalGameOptions.KillCooldown * CustomGameOptions.BountyHunterIncorrectCd);
                }
                else if (PlayerControl.LocalPlayer.Is(ModifierEnum.Underdog))
                {
                    var lowerKC = VanillaOptions().currentNormalGameOptions.KillCooldown - CustomGameOptions.UnderdogKillBonus;
                    var normalKC = VanillaOptions().currentNormalGameOptions.KillCooldown;
                    var upperKC = VanillaOptions().currentNormalGameOptions.KillCooldown + CustomGameOptions.UnderdogKillBonus;
                    PlayerControl.LocalPlayer.SetKillTimer(UnderdogPerformKill.LastImp() ? lowerKC : (UnderdogPerformKill.IncreasedKC() ? normalKC : upperKC));
                }
                else PlayerControl.LocalPlayer.SetKillTimer(VanillaOptions().currentNormalGameOptions.KillCooldown);
                return false;
            }
            else if (interact[1] == true)
            {
                PlayerControl.LocalPlayer.SetKillTimer(CustomGameOptions.ProtectKCReset + 0.01f);
                return false;
            }
            else if (interact[2] == true)
            {
                PlayerControl.LocalPlayer.SetKillTimer(0.01f);
                return false;
            }
            return false;
        }
    }
}