namespace TownOfSushi.Roles.Impostors.Support.BlackmailerRole
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Blackmailer)) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = GetRole<Blackmailer>(PlayerControl.LocalPlayer);
            var target = role.ClosestPlayer;
            if (__instance == role.BlackmailButton)
            {
                if (role.Player.inVent) return false;
                if (!__instance.isActiveAndEnabled || role.ClosestPlayer == null) return false;
                if (__instance.isCoolingDown) return false;
                if (!__instance.isActiveAndEnabled) return false;
                if (role.BlackmailTimer() != 0) return false;

                var interact = Interact(PlayerControl.LocalPlayer, target);
                if (interact[3] == true)
                {
                    role.Blackmailed?.myRend().material.SetFloat("_Outline", 0f);
                    if (role.Blackmailed != null && role.Blackmailed.Data.IsImpostor())
                    {
                        if (role.Blackmailed.GetCustomOutfitType() != CustomPlayerOutfitType.Camouflage &&
                            role.Blackmailed.GetCustomOutfitType() != CustomPlayerOutfitType.Swooper)
                            role.Blackmailed.nameText().color = Colors.Impostor;
                        else role.Blackmailed.nameText().color = Color.clear;
                    }
                    role.Blackmailed = target;
                    Rpc(CustomRPC.Blackmail, PlayerControl.LocalPlayer.PlayerId, target.PlayerId);
                }
                role.BlackmailButton.SetCoolDown(0.01f, 1f);
                return false;
            }
            return true;
        }
    }
}