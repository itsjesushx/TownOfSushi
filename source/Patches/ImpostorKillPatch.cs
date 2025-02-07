namespace TownOfSushi
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class ImpostorKillPatch
    {
        [HarmonyPriority(Priority.First)]
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != HUDManager().KillButton) return true;
            if (!LocalPlayer().Data.IsImpostor()) return true;
            var target = __instance.currentTarget;
            if (target == null) return true;
            if (!__instance.isActiveAndEnabled || __instance.isCoolingDown) return true;
            if (IsHideNSeek())
            {
                if (!target.inVent) RpcMurderPlayer(LocalPlayer(), target);
                return false;
            }
            var interact = Interact(LocalPlayer(), target, true);
            if (interact[3] == true) return false;
            if (LocalPlayer().Is(RoleEnum.Warlock))
            {
                var warlock = GetRole<Warlock>(LocalPlayer());
                if (warlock.Charging)
                {
                    warlock.UsingCharge = true;
                    warlock.ChargeUseDuration = warlock.ChargePercent * CustomGameOptions.ChargeUseDuration / 100f;
                    if (warlock.ChargeUseDuration == 0f) warlock.ChargeUseDuration += 0.01f;
                }
                LocalPlayer().SetKillTimer(0.01f);
            }
            else if (interact[0] == true)
            {
                if (LocalPlayer().Is(RoleEnum.BountyHunter))
                {
                    var bh = GetRole<BountyHunter>(LocalPlayer());
                    if (bh.Bounty == target) LocalPlayer().SetKillTimer(CustomGameOptions.BountyHunterCorrectCd);
                    else LocalPlayer().SetKillTimer(OptionsManager().currentNormalGameOptions.KillCooldown * CustomGameOptions.BountyHunterIncorrectCd);
                }
                else if (LocalPlayer().Is(ModifierEnum.Underdog))
                {
                    var lowerKC = OptionsManager().currentNormalGameOptions.KillCooldown - CustomGameOptions.UnderdogKillBonus;
                    var normalKC = OptionsManager().currentNormalGameOptions.KillCooldown;
                    var upperKC = OptionsManager().currentNormalGameOptions.KillCooldown + CustomGameOptions.UnderdogKillBonus;
                    LocalPlayer().SetKillTimer(UnderdogPerformKill.LastImp() ? lowerKC : (UnderdogPerformKill.IncreasedKC() ? normalKC : upperKC));
                }
                else LocalPlayer().SetKillTimer(OptionsManager().currentNormalGameOptions.KillCooldown);
                return false;
            }
            else if (interact[1] == true)
            {
                LocalPlayer().SetKillTimer(CustomGameOptions.ProtectKCReset + 0.01f);
                return false;
            }
            else if (interact[2] == true)
            {
                LocalPlayer().SetKillTimer(0.01f);
                return false;
            }
            return false;
        }
    }
}