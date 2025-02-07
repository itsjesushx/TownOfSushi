namespace TownOfSushi.Roles.Abilities
{
    public class Multitasker : Ability
    {
        public Multitasker(PlayerControl player) : base(player)
        {
            Name = "Multitasker";
            TaskText = () => "Your task windows are transparent";
            Color = ColorManager.Multitasker;
            AbilityType = AbilityEnum.Multitasker;
        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class MultitaskerUpdate
    {
        public static void Postfix(HudManager __instance)
        {
            if (!LocalPlayer().Is(AbilityEnum.Multitasker)) return;
            if (IsDead() || LocalPlayer().Data.Disconnected) return;
            if (!TaskPanel()) return;
            var Base = TaskPanel() as MonoBehaviour;
            SpriteRenderer[] rends = Base.GetComponentsInChildren<SpriteRenderer>();
            for (int i = 0; i < rends.Length; i++)
            {
                var oldColor1 = rends[i].color[0];
                var oldColor2 = rends[i].color[1];
                var oldColor3 = rends[i].color[2];
                rends[i].color = new Color(oldColor1, oldColor2, oldColor3, 0.5f);
            }
        }
    }
}