namespace TownOfSushi.Roles.Impostors.Power.WitchRole
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Witch)) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = GetRole<Witch>(PlayerControl.LocalPlayer);
            var target = role.ClosestPlayer;
            if (__instance == role.SpellButton)
            {
                if (!__instance.isActiveAndEnabled || role.ClosestPlayer == null) return false;
                if (__instance.isCoolingDown) return false;
                if (!__instance.isActiveAndEnabled) return false;
                if (role.SpellTimer() != 0) return false;
                var interact = Interact(PlayerControl.LocalPlayer, target);
                if (interact.AbilityUsed)
                {
                    role.Spelled?.myRend().material.SetFloat("_Outline", 0f);
                    if (role.Spelled != null && role.Spelled.Data.IsImpostor() && CustomGameOptions.SerialKillerVent)
                    {
                        if (role.Spelled.GetCustomOutfitType() != CustomPlayerOutfitType.Camouflage &&
                            role.Spelled.GetCustomOutfitType() != CustomPlayerOutfitType.Swooper)
                            role.Spelled.nameText().color = Colors.Impostor;
                        else role.Spelled.nameText().color = Color.clear;
                    }
                    role.Spelled = target;
                    Rpc(CustomRPC.Spell, PlayerControl.LocalPlayer.PlayerId, target.PlayerId);
                }
                role.SpellButton.SetCoolDown(role.SpellTimer(), CustomGameOptions.SpellCd);
                return false;
            }
            return true;
        }
    }
}