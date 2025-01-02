using TownOfSushi.Modifiers.UnderdogMod;

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
            Color = Colors.Impostor;
            ModifierType = ModifierEnum.Underdog;
        }

        public float MaxTimer() => PerformKill.LastImp() ? GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown - CustomGameOptions.UnderdogKillBonus : (PerformKill.IncreasedKC() ? GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown : GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown + CustomGameOptions.UnderdogKillBonus);

        public void SetKillTimer()
        {
            Player.SetKillTimer(MaxTimer());
        }
    }
}
