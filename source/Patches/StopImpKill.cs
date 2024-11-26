using TownOfSushi.Modifiers.UnderdogMod;

namespace TownOfSushi
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class StopImpKill
    {
        [HarmonyPriority(Priority.First)]
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton) return true;
            if (!PlayerControl.LocalPlayer.Data.IsImpostor()) return true;
            var target = __instance.currentTarget;
            if (target == null) return true;
            if (!__instance.isActiveAndEnabled || __instance.isCoolingDown) return true;
            if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek)
            {
                if (!target.inVent) RpcMurderPlayer(PlayerControl.LocalPlayer, target);
                return false;
            }
            var interact = Interact(PlayerControl.LocalPlayer, target, true);
            if (interact.AbilityUsed) return false;
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
            else if (interact.FullCooldownReset)
            {
                if (PlayerControl.LocalPlayer.Is(ModifierEnum.Underdog))
                {
                    var lowerKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown - CustomGameOptions.UnderdogKillBonus;
                    var normalKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown;
                    var upperKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown + CustomGameOptions.UnderdogKillBonus;
                    PlayerControl.LocalPlayer.SetKillTimer(PerformKill.LastImp() ? lowerKC : (PerformKill.IncreasedKC() ? normalKC : upperKC));
                }
                else PlayerControl.LocalPlayer.SetKillTimer(GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown);
                return false;
            }
            else if (interact.GaReset)
            {
                PlayerControl.LocalPlayer.SetKillTimer(CustomGameOptions.ProtectKCReset + 0.01f);
                return false;
            }
            else if (interact.ZeroSecReset)
            {
                PlayerControl.LocalPlayer.SetKillTimer(0.01f);
                return false;
            }
            return false;
        }
    }
}