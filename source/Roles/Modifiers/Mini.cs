namespace TownOfSushi.Roles.Modifiers
{
    public class Mini : Modifier, IVisualAlteration
    {
        public Mini(PlayerControl player) : base(player)
        {
            var speedText = CustomGameOptions.MiniSpeed >= 1.50 ? " and fast!" : "!";
            Name = "Mini";
            TaskText = () => "You are tiny" + speedText;
            Color = Colors.Mini;
            ModifierType = ModifierEnum.Mini;
        }

        public bool TryGetModifiedAppearance(out VisualAppearance appearance)
        {
            appearance = Player.GetDefaultAppearance();
            appearance.SpeedFactor = CustomGameOptions.MiniSpeed;
            appearance.SizeFactor = new Vector3(0.40f, 0.40f, 1f);
            return true;
        }
    }

    [HarmonyPatch]
    public static class MiniLadderFix
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerControl), "SetKinematic")]
        static bool Prefix(PlayerControl __instance, bool b)
        {
            if (__instance != PlayerControl.LocalPlayer) return true;
            if (!__instance.onLadder) return true;
            if (!__instance.Is(ModifierEnum.Mini)) return true;
            if (b) return true; // Is start of ladder?
            if (!FungleMap()) return true; // is not fungle?

            var AllLadders = GameObject.FindObjectsOfType<Ladder>();
            var Ladder = AllLadders.OrderBy(x => Vector3.Distance(x.transform.position, __instance.transform.position)).ElementAt(0);


            if (!Ladder.IsTop) return true; // Are we at the bottom?
            
            __instance.NetTransform.RpcSnapTo(__instance.transform.position + new Vector3(0,0.5f));

            return true;
        }
    }
}