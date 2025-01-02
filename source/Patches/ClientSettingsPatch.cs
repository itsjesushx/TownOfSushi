namespace TownOfSushi.Patches
{
    [HarmonyPatch]
    public class ClientSettings
    {
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
        [HarmonyPostfix]

        public static void HideGhosts()
        {
            if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started) return;
            if (!PlayerControl.LocalPlayer.Data.IsDead) return;
            if (MeetingHud.Instance) return;

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player == PlayerControl.LocalPlayer) continue;
                if (!player.Data.IsDead) continue;
                bool show = TownOfSushi.DeadSeeGhosts.Value;
                var bodyforms = player.gameObject.transform.GetChild(1).gameObject;

                foreach (var form in bodyforms.GetAllChilds())
                {
                    if (form.activeSelf)
                    {
                        form.GetComponent<SpriteRenderer>().color = new(1f, 1f, 1f, show ? 1f : 0f);
                    }
                }

                if (player.cosmetics.HasPetEquipped()) player.cosmetics.CurrentPet.Visible = show;
                player.cosmetics.gameObject.SetActive(show);
                player.gameObject.transform.GetChild(3).gameObject.SetActive(show);
            }
        }

        // class from: https://github.com/SuperNewRoles/SuperNewRoles
        [HarmonyPatch(typeof(LobbyBehaviour))]
        public class LobbyBehaviourPatch
        {    
            [HarmonyPatch(nameof(LobbyBehaviour.Update)), HarmonyPostfix]    
            public static void Update_Postfix(LobbyBehaviour __instance)    
            {
                Func<ISoundPlayer, bool> uglyMusic = x => x.Name.Equals("MapTheme");
                ISoundPlayer LobbyUglyMusic = SoundManager.Instance.soundPlayers.Find(uglyMusic);
                if (TownOfSushi.DisableLobbyMusic.Value)
                {
                    if (LobbyUglyMusic == null) return;
                    SoundManager.Instance.StopNamedSound("MapTheme");
                }
                else
                {
                    if (LobbyUglyMusic != null) return;
                    SoundManager.Instance.CrossFadeSound("MapTheme", __instance.MapTheme, 0.5f);
                }
            }
        }
    }
}