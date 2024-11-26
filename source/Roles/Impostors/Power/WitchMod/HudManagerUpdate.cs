namespace TownOfSushi.Roles.Impostors.Power.WitchRole
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Close))]
    class ExileControllerBeginPatch {
        private static void Postfix(MeetingHud __instance)
        {
            var role =GetRole<Witch>(PlayerControl.LocalPlayer);
            if (role.Spelled != null && !role.Player.Data.IsDead)
            {            
                RpcMurderPlayer(role.Spelled, role.Spelled);            
                Rpc(CustomRPC.RemoveAllBodies);            
                var buggedBodies = Object.FindObjectsOfType<DeadBody>();         
                var role2 = GetPlayerRole(role.Spelled);     
                foreach (var body in buggedBodies)            
                {                
                    body.gameObject.Destroy();            
                }            
                    role.Kills ++;
                    role2.DeathReason = DeathReasonEnum.Cursed;               
                    role2.KilledBy = " By " + ColorString(Colors.Impostor, role.PlayerName);   
                    System.Console.WriteLine("Executed Cursed player correctly");
            }
        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerUpdate
    {
        public static Sprite Blackmail => TownOfSushi.BlackmailSprite;
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Witch)) return;
            var role = GetRole<Witch>(PlayerControl.LocalPlayer);
            if (role.SpellButton == null)
            {
                role.SpellButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.SpellButton.graphic.enabled = true;
                role.SpellButton.gameObject.SetActive(false);
            }

            role.SpellButton.graphic.sprite = Blackmail;
            role.SpellButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);

            var notCursable = PlayerControl.AllPlayerControls.ToArray().Where(player => role.Spelled?.PlayerId != player.PlayerId || !player.Is(Faction.Impostors)).ToList();

            SetTarget(ref role.ClosestPlayer, role.SpellButton, GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance], notCursable);

            role.SpellButton.SetCoolDown(role.SpellTimer(), CustomGameOptions.SpellCd);

            if (role.Spelled != null && !role.Spelled.Data.Disconnected)
            {
                role.Spelled.myRend().material.SetFloat("_Outline", 1f);
                role.Spelled.myRend().material.SetColor("_OutlineColor", new Color(0.3f, 0.0f, 0.0f));
                if (role.Spelled.GetCustomOutfitType() != CustomPlayerOutfitType.Camouflage &&
                    role.Spelled.GetCustomOutfitType() != CustomPlayerOutfitType.Swooper)
                    role.Spelled.nameText().color = new Color(0.3f, 0.0f, 0.0f);
                else role.Spelled.nameText().color = Color.clear;
            }
        }
    }
}