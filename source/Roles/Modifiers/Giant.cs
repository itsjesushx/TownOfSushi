namespace TownOfSushi.Roles.Modifiers
{
    public class Giant : Modifier, IVisualAlteration
    {
        public Giant(PlayerControl player) : base(player)
        {
            var slowText = CustomGameOptions.GiantSlow != 1? " and slow!" : "!";
            Name = "Giant";
            TaskText = () => "You are ginormous" + slowText;
            Color = Colors.Giant;
            ModifierType = ModifierEnum.Giant;
        }

        public bool TryGetModifiedAppearance(out VisualAppearance appearance)
        {
            appearance = Player.GetDefaultAppearance();
            appearance.SpeedFactor = CustomGameOptions.GiantSlow;
            appearance.SizeFactor = new Vector3(1.0f, 1.0f, 1.0f);
            return true;
        }
    }

    [HarmonyPatch]
    public static class GiantLadderFix
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerControl), "SetKinematic")]
        static bool Prefix(PlayerControl __instance, bool b)
        {
            if (__instance != PlayerControl.LocalPlayer) return true;
            if (!__instance.onLadder) return true;
            if (!__instance.Is(ModifierEnum.Giant)) return true;
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