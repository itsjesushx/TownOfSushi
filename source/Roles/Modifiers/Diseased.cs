namespace TownOfSushi.Roles.Modifiers
{
    public class Diseased : Modifier
    {
        public Diseased(PlayerControl player) : base(player)
        {
            Name = "Diseased";
            TaskText = () => "Killing you gives Impostors a high cooldown";
            Color = Colors.Diseased;
            ModifierType = ModifierEnum.Diseased;
        }
    }

    public class UpdateDiseased
    {
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MurderPlayer))]
        public class PlayerControl_MurderPlayer
        {
            public static void Postfix(PlayerControl __instance, [HarmonyArgument(0)] PlayerControl target)
            {
                if (target.Is(ModifierEnum.Diseased))
                    __instance.SetKillTimer(GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown * CustomGameOptions.DiseasedMultiplier);
            }
        }
    }
}