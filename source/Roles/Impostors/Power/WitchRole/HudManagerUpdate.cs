namespace TownOfSushi.Roles.Impostors.Power.WitchRole
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Close))]
    class ExileControllerBeginPatch 
    {
        private static void Postfix(MeetingHud __instance)
        {
            var role = GetRole<Witch>(PlayerControl.LocalPlayer);
            if (role.SpelledPlayers != null && !role.Player.Data.IsDead)
            {
                foreach (var spelledId in role.SpelledPlayers)
                {
                    var spelledPlayer = PlayerById(spelledId);
                    var deadRole = GetPlayerRole(spelledPlayer);
                    if (spelledPlayer != null)
                    {
                        RpcMurderPlayer(spelledPlayer, spelledPlayer);
                        deadRole.DeathReason = DeathReasonEnum.Cursed;
                        role.Kills ++;
                    }
                    Rpc(CustomRPC.RemoveAllBodies);
                    var buggedBodies = Object.FindObjectsOfType<DeadBody>();
                    foreach (var body in buggedBodies)
                    {
                        body.gameObject.Destroy();
                    }
                }
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
                    role.SpellButton.SetCoolDown(role.SpellTimer(), CustomGameOptions.SpellCd);
                
                var notSpelled = PlayerControl.AllPlayerControls
                .ToArray()
                .Where(x => !role.SpelledPlayers.Contains(x.PlayerId))
                .ToList();

                SetTarget(ref role.ClosestPlayer, role.SpellButton, float.NaN, notSpelled);
            }
        }
    }