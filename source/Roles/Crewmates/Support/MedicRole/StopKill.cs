namespace TownOfSushi.Roles.Crewmates.Support.MedicRole
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class StopKill
    {
        public static void BreakShield(byte medicId, byte playerId, bool flag)
        {
            if (PlayerControl.LocalPlayer.PlayerId == playerId && CustomGameOptions.NotificationShield == NotificationOptions.Shielded) 
            {
                Flash(Color.red, 0.5f);
            }

            if (PlayerControl.LocalPlayer.PlayerId == medicId && CustomGameOptions.NotificationShield == NotificationOptions.Medic) 
            {
                Flash(Color.red, 0.5f);
            }

            if (CustomGameOptions.NotificationShield == NotificationOptions.Everyone) 
            {
                Flash(Color.red, 0.5f);
            }

            if (!flag)
                return;

            var player = PlayerById(playerId);
            foreach (var role in GetRoles(RoleEnum.Medic))
                if (((Medic) role).ShieldedPlayer.PlayerId == playerId)
                {
                    ((Medic) role).ShieldedPlayer = null;
                    ((Medic) role).exShielded = player;
                    System.Console.WriteLine(player.name + " Is Ex-Shielded");
                }

            player.myRend().material.SetColor("_VisorColor", Palette.VisorColor);
            player.myRend().material.SetFloat("_Outline", 0f);
        }
    }
}