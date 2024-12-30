

namespace TownOfSushi.Roles.Neutral.Evil.PhantomRole
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class RepickPhantom
    {
        private static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (PlayerControl.LocalPlayer != SetPhantom.WillBePhantom) return;
            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (!PlayerControl.LocalPlayer.Is(Faction.Neutral))
            {
                var toChooseFromAlive = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Neutral) && !x.Data.Disconnected).ToList();
                if (toChooseFromAlive.Count == 0)
                {
                    SetPhantom.WillBePhantom = null;

                    Rpc(CustomRPC.SetPhantom, byte.MaxValue);
                }
                else
                {
                    var rand2 = Random.RandomRangeInt(0, toChooseFromAlive.Count);
                    var pc2 = toChooseFromAlive[rand2];

                    SetPhantom.WillBePhantom = pc2;

                    Rpc(CustomRPC.SetPhantom, pc2.PlayerId);
                }
                return;
            }
            var toChooseFrom = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.Crewmates) && !x.Is(Faction.Impostors) && x.Data.IsDead && !x.Data.Disconnected).ToList();
            if (toChooseFrom.Count == 0) return;
            var rand = Random.RandomRangeInt(0, toChooseFrom.Count);
            var pc = toChooseFrom[rand];

            SetPhantom.WillBePhantom = pc;

            Rpc(CustomRPC.SetPhantom, pc.PlayerId);
            return;
        }
    }
}