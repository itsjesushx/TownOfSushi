namespace TownOfSushi.Roles.Impostors.Deception.MorphlingRole
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static Sprite SampleSprite => TownOfSushi.SampleSprite;
        public static Sprite MorphSprite => TownOfSushi.MorphSprite;

        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Morphling);
            if (!flag) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (MushroomSabotageActive()) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = GetRole<Morphling>(PlayerControl.LocalPlayer);
            var target = role.ClosestPlayer;
            if (__instance == role.MorphButton)
            {
                if (!__instance.isActiveAndEnabled) return false;
                if (role.MorphButton.graphic.sprite == SampleSprite)
                {
                    if (target == null) return false;
                    var abilityUsed = AbilityUsed(PlayerControl.LocalPlayer);
                    if (!abilityUsed) return false;
                    role.SampledPlayer = target;
                    role.MorphButton.graphic.sprite = MorphSprite;
                    role.MorphButton.SetTarget(null);
                    DestroyableSingleton<HudManager>.Instance.KillButton.SetTarget(null);
                    if (role.MorphTimer() < 5f)
                        role.LastMorphed = DateTime.UtcNow.AddSeconds(5 - CustomGameOptions.MorphlingCd);
                }
                else
                {
                    if (__instance.isCoolingDown) return false;
                    if (role.MorphTimer() != 0) return false;
                    var abilityUsed = AbilityUsed(PlayerControl.LocalPlayer);
                    if (!abilityUsed) return false;
                    Rpc(CustomRPC.Morph, PlayerControl.LocalPlayer.PlayerId, role.SampledPlayer.PlayerId);
                    role.TimeRemaining = CustomGameOptions.MorphlingDuration;
                    role.MorphedPlayer = role.SampledPlayer;
                    Morph(role.Player, role.SampledPlayer);
                }

                return false;
            }

            return true;
        }
    }
}
