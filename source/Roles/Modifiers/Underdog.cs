namespace TownOfSushi.Roles.Modifiers
{
    public class Underdog : Modifier
    {
        public Underdog(PlayerControl player) : base(player)
        {
            Name = "Underdog";
            TaskText = () => LastImp()
                ? "You have a shortened kill cooldown!"
                : "You have a long kill cooldown until you're alone";
            Color = ColorManager.Impostor;
            ModifierType = ModifierEnum.Underdog;
        }

        public float MaxTimer() => UnderdogPerformKill.LastImp() ? VanillaOptions().currentNormalGameOptions.KillCooldown - CustomGameOptions.UnderdogKillBonus : (UnderdogPerformKill.IncreasedKC() ? VanillaOptions().currentNormalGameOptions.KillCooldown : VanillaOptions().currentNormalGameOptions.KillCooldown + CustomGameOptions.UnderdogKillBonus);
        public void SetKillTimer()
        {
            Player.SetKillTimer(MaxTimer());
        }
    }

    [HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
    public static class UnderdogHUDClose
    {
        public static void Postfix()
        {
            var modifier = GetModifier(PlayerControl.LocalPlayer);
            if (modifier?.ModifierType == ModifierEnum.Underdog)
                ((Underdog)modifier).SetKillTimer();
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MurderPlayer))]
    public class UnderdogPerformKill
    {
        public static void Postfix(PlayerControl __instance, [HarmonyArgument(0)] PlayerControl target)
        {
            var modifier = GetModifier(__instance);
            if (modifier?.ModifierType == ModifierEnum.Underdog)
                ((Underdog)modifier).SetKillTimer();
        }

        internal static bool LastImp()
        {
            return PlayerControl.AllPlayerControls.ToArray()
                .Count(x => x.Data.IsImpostor() && !x.Data.IsDead) == 1;
        }

        internal static bool IncreasedKC()
        {
            if (CustomGameOptions.UnderdogIncreasedKC)
                return false;
            else
                return true;
        }
    }
}
