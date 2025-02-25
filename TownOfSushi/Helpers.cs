using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using System.Linq;
using static TownOfSushi.TownOfSushi;
using HarmonyLib;
using Hazel;
using TownOfSushi.Utilities;
using Reactor.Utilities.Extensions;
using AmongUs.GameOptions;
using TownOfSushi.Patches;
using System.Text;

namespace TownOfSushi 
{
    public static class Helpers
    {

        public static Dictionary<string, Sprite> CachedSprites = new();
        public static Sprite LoadSpriteFromResources(string path, float pixelsPerUnit, bool cache=true) 
        {
            try
            {
                if (cache && CachedSprites.TryGetValue(path + pixelsPerUnit, out var sprite)) return sprite;
                Texture2D texture = LoadTextureFromResources(path);
                sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), pixelsPerUnit);
                if (cache) sprite.hideFlags |= HideFlags.HideAndDontSave | HideFlags.DontSaveInEditor;
                if (!cache) return sprite;
                return CachedSprites[path + pixelsPerUnit] = sprite;
            } 
            catch 
            {
                System.Console.WriteLine("Error loading sprite from path: " + path);
            }
            return null;
        }

        public static unsafe Texture2D LoadTextureFromResources(string path) 
        {
            try 
            {
                Texture2D texture = new Texture2D(2, 2, TextureFormat.ARGB32, true);
                Assembly assembly = Assembly.GetExecutingAssembly();
                Stream stream = assembly.GetManifestResourceStream(path);
                var length = stream.Length;
                var byteTexture = new Il2CppStructArray<byte>(length);
                stream.Read(new Span<byte>(IntPtr.Add(byteTexture.Pointer, IntPtr.Size * 4).ToPointer(), (int) length));
                if (path.Contains("HorseHats")) {
                    byteTexture = new Il2CppStructArray<byte>(byteTexture.Reverse().ToArray());
                }
                ImageConversion.LoadImage(texture, byteTexture, false);
                return texture;
            } 
            catch 
            {
                System.Console.WriteLine("Error loading texture from resources: " + path);
            }
            return null;
        }

        public static Texture2D LoadTextureFromDisk(string path) 
        {
            try 
            {
                if (File.Exists(path))     
                {
                    Texture2D texture = new Texture2D(2, 2, TextureFormat.ARGB32, true);
                    var byteTexture = Il2CppSystem.IO.File.ReadAllBytes(path);
                    ImageConversion.LoadImage(texture, byteTexture, false);
                    return texture;
                }
            } 
            catch 
            {
                TownOfSushiPlugin.Logger.LogError("Error loading texture from disk: " + path);
            }
            return null;
        }

        /* This function has been removed from TOS because we switched to assetbundles for compressed audio. leaving it here for reference - Gendelo
        public static AudioClip loadAudioClipFromResources(string path, string clipName = "UNNAMED_TOR_AUDIO_CLIP") {

            // must be "raw (headerless) 2-channel signed 32 bit pcm (le) 48kHz" (can e.g. use Audacity® to export )
            try {
                Assembly assembly = Assembly.GetExecutingAssembly();
                Stream stream = assembly.GetManifestResourceStream(path);
                var byteAudio = new byte[stream.Length];
                _ = stream.Read(byteAudio, 0, (int)stream.Length);
                float[] samples = new float[byteAudio.Length / 4]; // 4 bytes per sample
                int offset;
                for (int i = 0; i < samples.Length; i++) {
                    offset = i * 4;
                    samples[i] = (float)BitConverter.ToInt32(byteAudio, offset) / Int32.MaxValue;
                }
                int channels = 2;
                int sampleRate = 48000;
                AudioClip audioClip = AudioClip.Create(clipName, samples.Length / 2, channels, sampleRate, false);
                audioClip.hideFlags |= HideFlags.HideAndDontSave | HideFlags.DontSaveInEditor;
                audioClip.SetData(samples, 0);
                return audioClip;
            } catch {
                System.Console.WriteLine("Error loading AudioClip from resources: " + path);
            }
            return null;

            // Usage example:
            //AudioClip exampleClip = Helpers.loadAudioClipFromResources("TownOfSushi.Resources.exampleClip.raw");
            //if (Constants.ShouldPlaySfx()) SoundManager.Instance.PlaySound(exampleClip, false, 0.8f);
            
        }*/

        public static string ReadTextFromResources(string path) 
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream stream = assembly.GetManifestResourceStream(path);
            StreamReader textStreamReader = new StreamReader(stream);
            return textStreamReader.ReadToEnd();
        }

        public static string ReadTextFromFile(string path) 
        {
            Stream stream = File.OpenRead(path);
            StreamReader textStreamReader = new StreamReader(stream);
            return textStreamReader.ReadToEnd();
        }

        public static PlayerControl PlayerById(byte id)
        {
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                if (player.PlayerId == id)
                    return player;
            return null;
        }
        
        public static Dictionary<byte, PlayerControl> AllPlayersById()
        {
            Dictionary<byte, PlayerControl> res = new Dictionary<byte, PlayerControl>();
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                res.Add(player.PlayerId, player);
            return res;
        }

        public static bool TwoPlayersAlive() => PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead) == 2;

        public static void HandleVampireBiteOnBodyReport() 
        {
            // Murder the bitten player and reset bitten (regardless whether the kill was successful or not)
            Helpers.CheckMurderAttemptAndKill(Vampire.Player, Vampire.bitten, true, false);
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.VampireSetBitten, Hazel.SendOption.Reliable, -1);
            writer.Write(byte.MaxValue);
            writer.Write(byte.MaxValue);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.VampireSetBitten(byte.MaxValue, byte.MaxValue);
        }

        public static void RefreshRoleDescription(PlayerControl player) 
        {
            List<RoleInfo> infos = RoleInfo.GetRoleInfoForPlayer(player); 
            List<string> taskTexts = new(infos.Count); 

            foreach (var roleInfo in infos)
            {
                taskTexts.Add(GetRoleString(roleInfo));
            }
            
            var toRemove = new List<PlayerTask>();
            foreach (PlayerTask t in player.myTasks.GetFastEnumerator()) 
            {
                var textTask = t.TryCast<ImportantTextTask>();
                if (textTask == null) continue;
                
                var currentText = textTask.Text;
                
                if (taskTexts.Contains(currentText)) taskTexts.Remove(currentText); // TextTask for this RoleInfo does not have to be added, as it already exists
                else toRemove.Add(t); // TextTask does not have a corresponding RoleInfo and will hence be deleted
            }   

            foreach (PlayerTask t in toRemove) 
            {
                t.OnRemove();
                player.myTasks.Remove(t);
                UnityEngine.Object.Destroy(t.gameObject);
            }

            // Add TextTask for remaining RoleInfos
            foreach (string title in taskTexts) 
            {
                var task = new GameObject("RoleTask").AddComponent<ImportantTextTask>();
                task.transform.SetParent(player.transform, false);
                task.Text = title;
                player.myTasks.Insert(0, task);
            }
        }

        internal static string GetRoleString(RoleInfo roleInfo)
        {
            if (roleInfo.name == "Jackal") 
            {
                var getSidekickText = Jackal.canCreateSidekick ? " and recruit a Sidekick" : "";
                return ColorString(roleInfo.color, $"{roleInfo.name}: Kill everyone{getSidekickText}");  
            }

            if (roleInfo.name == "Invert") 
            {
                return ColorString(roleInfo.color, $"{roleInfo.name}: {roleInfo.shortDescription} ({Invert.meetings})");
            }
            
            return ColorString(roleInfo.color, $"{roleInfo.name}: {roleInfo.shortDescription}");
        }

        public static bool IsD(byte playerId) 
        {
            return playerId % 2 == 0;
        }

        public static bool IsLighterColor(PlayerControl target) 
        {
            return IsD(target.PlayerId);
        }
        public static Il2CppSystem.Collections.Generic.List<PlayerControl> GetClosestPlayers(Vector2 truePosition, float radius)
        {
            var playerControlList = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            var allPlayers = GameData.Instance.AllPlayers;
            var lightRadius = radius * ShipStatus.Instance.MaxLightRadius;

            for (var index = 0; index < allPlayers.Count; ++index)
            {
                var playerInfo = allPlayers[index];

                if (!playerInfo.Disconnected)
                {
                    var vector2 = new Vector2(playerInfo.Object.GetTruePosition().x - truePosition.x, playerInfo.Object.GetTruePosition().y - truePosition.y);
                    var magnitude = vector2.magnitude;

                    if (magnitude <= lightRadius)
                    {
                        var playerControl = playerInfo.Object;
                        playerControlList.Add(playerControl);
                    }
                }
            }

            return playerControlList;
        }

        public static bool IsCustomServer() 
        {
            if (FastDestroyableSingleton<ServerManager>.Instance == null) return false;
            StringNames n = FastDestroyableSingleton<ServerManager>.Instance.CurrentRegion.TranslateName;
            return n != StringNames.ServerNA && n != StringNames.ServerEU && n != StringNames.ServerAS;
        }

        public static bool HasFakeTasks(this PlayerControl player) 
        {
            return player == Jester.jester || player == Romantic.Player || player.IsNeutralKiller() || player == Arsonist.Player || player == Vulture.Player;
        }
        

        public static bool CanBeErased(this PlayerControl player) 
        {
            return !player.IsNeutralKiller();
        }

        public static bool ShouldShowGhostInfo() 
        {
            return PlayerControl.LocalPlayer != null && PlayerControl.LocalPlayer.Data.IsDead && MapOptions.ghostsSeeInformation || AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Ended;
        }

        public static void ClearAllTasks(this PlayerControl player) {
            if (player == null) return;
            foreach (var playerTask in player.myTasks.GetFastEnumerator())
            {
                playerTask.OnRemove();
                UnityEngine.Object.Destroy(playerTask.gameObject);
            }
            player.myTasks.Clear();
            
            if (player.Data != null && player.Data.Tasks != null)
                player.Data.Tasks.Clear();
        }

        public static void MurderPlayer(this PlayerControl player, PlayerControl target)
        {
            player.MurderPlayer(target, MurderResultFlags.Succeeded);
        }

        public static void RpcRepairSystem(this ShipStatus shipStatus, SystemTypes systemType, byte amount)
        {
            shipStatus.RpcUpdateSystem(systemType, amount);
        }

        public static bool IsMira() 
        {
            return GameOptionsManager.Instance.CurrentGameOptions.MapId == 1;
        }

        public static bool IsAirship() 
        {
            return GameOptionsManager.Instance.CurrentGameOptions.MapId == 4;
        }
        public static bool IsSkeld() 
        {
            return GameOptionsManager.Instance.CurrentGameOptions.MapId == 0;
        }
        public static bool IsPolus() 
        {
            return GameOptionsManager.Instance.CurrentGameOptions.MapId == 2;
        }

        public static bool IsFungle() {   

            return GameOptionsManager.Instance.CurrentGameOptions.MapId == 5;
        }

        public static bool MushroomSabotageActive() 
        {
            return PlayerControl.LocalPlayer.myTasks.ToArray().Any((x) => x.TaskType == TaskTypes.MushroomMixupSabotage);
        }


        public static bool SabotageActive() 
        {
            var sabSystem = ShipStatus.Instance.Systems[SystemTypes.Sabotage].CastFast<SabotageSystemType>();
            return sabSystem.AnyActive;
        }

        public static float SabotageTimer() 
        {
            var sabSystem = ShipStatus.Instance.Systems[SystemTypes.Sabotage].CastFast<SabotageSystemType>();
            return sabSystem.Timer;
        }
        public static bool CanUseSabotage() 
        {
            var sabSystem = ShipStatus.Instance.Systems[SystemTypes.Sabotage].CastFast<SabotageSystemType>();
            ISystemType systemType;
            IActivatable doors = null;
            if (ShipStatus.Instance.Systems.TryGetValue(SystemTypes.Doors, out systemType)) {
                doors = systemType.CastFast<IActivatable>();
            }
            return GameManager.Instance.SabotagesEnabled() && sabSystem.Timer <= 0f && !sabSystem.AnyActive && !(doors != null && doors.IsActive);
        }

        public static void SetSemiTransparent(this PoolablePlayer player, bool value, float alpha=0.25f) 
        {
            alpha = value ? alpha : 1f;
            foreach (SpriteRenderer r in player.gameObject.GetComponentsInChildren<SpriteRenderer>())
                r.color = new Color(r.color.r, r.color.g, r.color.b, alpha);
            player.cosmetics.nameText.color = new Color(player.cosmetics.nameText.color.r, player.cosmetics.nameText.color.g, player.cosmetics.nameText.color.b, alpha);
        }

        public static string GetString(this TranslationController t, StringNames key, params Il2CppSystem.Object[] parts) {
            return t.GetString(key, parts);
        }

        public static string ColorString(Color c, string s) 
        {
            return string.Format("<color=#{0:X2}{1:X2}{2:X2}{3:X2}>{4}</color>", ToByte(c.r), ToByte(c.g), ToByte(c.b), ToByte(c.a), s);
        }

        public static int lineCount(string text) {
            return text.Count(c => c == '\n');
        }

        private static byte ToByte(float f) {
            f = Mathf.Clamp01(f);
            return (byte)(f * 255);
        }

        public static KeyValuePair<byte, int> MaxPair(this Dictionary<byte, int> self, out bool tie) {
            tie = true;
            KeyValuePair<byte, int> result = new KeyValuePair<byte, int>(byte.MaxValue, int.MinValue);
            foreach (KeyValuePair<byte, int> keyValuePair in self)
            {
                if (keyValuePair.Value > result.Value)
                {
                    result = keyValuePair;
                    tie = false;
                }
                else if (keyValuePair.Value == result.Value)
                {
                    tie = true;
                }
            }
            return result;
        }

        public static bool HidePlayerName(PlayerControl source, PlayerControl target) 
        {
            if (Camouflager.camouflageTimer > 0f || Helpers.MushroomSabotageActive()) return true; // No names are visible
            if (Patches.SurveillanceMinigamePatch.nightVisionIsActive) return true;
            else if (Ninja.isInvisble && Ninja.ninja == target) return true;
            else if (!MapOptions.hidePlayerNames) return false; // All names are visible
            else if (source == null || target == null) return true;
            else if (source == target) return false; // Player sees his own name
            else if (source.Data.Role.IsImpostor && (target.Data.Role.IsImpostor || target == Spy.Player || target == Sidekick.Player && Sidekick.wasTeamRed || target == Jackal.Player && Jackal.wasTeamRed)) return false; // Members of team Impostors see the names of Impostors/Spies
            else if ((source == Lovers.Lover1 || source == Lovers.Lover2) && (target == Lovers.Lover1 || target == Lovers.Lover2)) return false; // Members of team Lovers see the names of each other
            else if ((source == Jackal.Player || source == Sidekick.Player) && (target == Jackal.Player || target == Sidekick.Player || target == Jackal.fakeSidekick)) return false; // Members of team Jackal see the names of each other
            return true;
        }

        public static void SetDefaultLook(this PlayerControl target, bool enforceNightVisionUpdate = true) 
        {
            if (MushroomSabotageActive()) 
            {
                var instance = ShipStatus.Instance.CastFast<FungleShipStatus>().specialSabotage;
                MushroomMixupSabotageSystem.CondensedOutfit condensedOutfit = instance.currentMixups[target.PlayerId];
                NetworkedPlayerInfo.PlayerOutfit playerOutfit = instance.ConvertToPlayerOutfit(condensedOutfit);
                target.MixUpOutfit(playerOutfit);
            } else
                target.SetLook(target.Data.PlayerName, target.Data.DefaultOutfit.ColorId, target.Data.DefaultOutfit.HatId, target.Data.DefaultOutfit.VisorId, target.Data.DefaultOutfit.SkinId, target.Data.DefaultOutfit.PetId, enforceNightVisionUpdate);
        }

        public static void SetLook(this PlayerControl target, String playerName, int colorId, string hatId, string visorId, string skinId, string petId, bool enforceNightVisionUpdate = true) 
        {
            target.RawSetColor(colorId);
            target.RawSetVisor(visorId, colorId);
            target.RawSetHat(hatId, colorId);
            target.RawSetName(HidePlayerName(PlayerControl.LocalPlayer, target) ? "" : playerName);


            SkinViewData nextSkin = null;
            try { nextSkin = ShipStatus.Instance.CosmeticsCache.GetSkin(skinId); } catch { return; };
            
            PlayerPhysics playerPhysics = target.MyPhysics;
            AnimationClip clip = null;
            var spriteAnim = playerPhysics.myPlayer.cosmetics.skin.animator;
            var currentPhysicsAnim = playerPhysics.Animations.Animator.GetCurrentAnimation();


            if (currentPhysicsAnim == playerPhysics.Animations.group.RunAnim) clip = nextSkin.RunAnim;
            else if (currentPhysicsAnim == playerPhysics.Animations.group.SpawnAnim) clip = nextSkin.SpawnAnim;
            else if (currentPhysicsAnim == playerPhysics.Animations.group.EnterVentAnim) clip = nextSkin.EnterVentAnim;
            else if (currentPhysicsAnim == playerPhysics.Animations.group.ExitVentAnim) clip = nextSkin.ExitVentAnim;
            else if (currentPhysicsAnim == playerPhysics.Animations.group.IdleAnim) clip = nextSkin.IdleAnim;
            else clip = nextSkin.IdleAnim;
            float progress = playerPhysics.Animations.Animator.m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            playerPhysics.myPlayer.cosmetics.skin.skin = nextSkin;
            playerPhysics.myPlayer.cosmetics.skin.UpdateMaterial();

            spriteAnim.Play(clip, 1f);
            spriteAnim.m_animator.Play("a", 0, progress % 1);
            spriteAnim.m_animator.Update(0f);

            target.RawSetPet(petId, colorId);

            if (enforceNightVisionUpdate) Patches.SurveillanceMinigamePatch.enforceNightVision(target);
            Chameleon.Update();  // so that morphling and camo wont make the chameleons visible
        }

        public static void ShowFlash(Color color, float duration=1f, string message="") 
        {
            if (FastDestroyableSingleton<HudManager>.Instance == null || FastDestroyableSingleton<HudManager>.Instance.FullScreen == null) return;
            FastDestroyableSingleton<HudManager>.Instance.FullScreen.gameObject.SetActive(true);
            FastDestroyableSingleton<HudManager>.Instance.FullScreen.enabled = true;
            // Message Text
            TMPro.TextMeshPro messageText = GameObject.Instantiate(FastDestroyableSingleton<HudManager>.Instance.KillButton.cooldownTimerText, FastDestroyableSingleton<HudManager>.Instance.transform);
            messageText.text = message;
            messageText.enableWordWrapping = false;
            messageText.transform.localScale = Vector3.one * 0.5f;
            messageText.transform.localPosition += new Vector3(0f, 2f, -69f);
            messageText.gameObject.SetActive(true);
            FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(duration, new Action<float>((p) => {
                var renderer = FastDestroyableSingleton<HudManager>.Instance.FullScreen;

                if (p < 0.5) {
                    if (renderer != null)
                        renderer.color = new Color(color.r, color.g, color.b, Mathf.Clamp01(p * 2 * 0.75f));
                } else {
                    if (renderer != null)
                        renderer.color = new Color(color.r, color.g, color.b, Mathf.Clamp01((1 - p) * 2 * 0.75f));
                }
                if (p == 1f && renderer != null) renderer.enabled = false;
                if (p == 1f) messageText.gameObject.Destroy();
            })));
        }
        public static bool RoleCanUseVents(this PlayerControl player) 
        {
            bool roleCouldUse = false;
            if (Engineer.Player != null && Engineer.Player == player)
                roleCouldUse = true;
            else if (Jackal.canUseVents && Jackal.Player != null && Jackal.Player == player)
                roleCouldUse = true;
            else if (VengefulRomantic.CanUseVents && VengefulRomantic.Player != null && VengefulRomantic.Player == player)
                roleCouldUse = true;
            else if (Werewolf.CanUseVents && Werewolf.Player != null && Werewolf.Player == player)
                roleCouldUse = true;
            else if (Sidekick.canUseVents && Sidekick.Player != null && Sidekick.Player == player)
                roleCouldUse = true;
            else if (Spy.canEnterVents && Spy.Player != null && Spy.Player == player)
                roleCouldUse = true;
            else if (SerialKiller.CanUseVents && SerialKiller.Player != null && SerialKiller.Player == player)
                roleCouldUse = true;
            else if (Glitch.canEnterVents && Glitch.Player != null && Glitch.Player == player)
                roleCouldUse = true;
            else if (Vulture.canUseVents && Vulture.Player != null && Vulture.Player == player)
                roleCouldUse = true;
            else if (Thief.canUseVents &&  Thief.Player != null && Thief.Player == player)
                roleCouldUse = true;
            else if (player.Data?.Role != null && player.Data.Role.CanVent)  
            {
                if (Janitor.Player != null && Janitor.Player == PlayerControl.LocalPlayer)
                    roleCouldUse = false;
                else if (Mafioso.mafioso != null && Mafioso.mafioso == PlayerControl.LocalPlayer && Godfather.Player != null && !Godfather.Player.Data.IsDead)
                    roleCouldUse = false;
                else
                    roleCouldUse = true;
            }
            return roleCouldUse;
        }

        public static bool CheckArmored(PlayerControl target, bool breakShield, bool showShield, bool additionalCondition = true)
        {
            if (target != null && Armored.Player != null && Armored.Player == target && !Armored.isBrokenArmor && additionalCondition) {
                if (breakShield) 
                {
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.BreakArmor, Hazel.SendOption.Reliable, -1);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.BreakArmor();
                }
                if (showShield) 
                {
                    target.ShowFailedMurder();
                }
                return true;
            }
            return false;
        }

        public static MurderAttemptResult CheckMuderAttempt(PlayerControl killer, PlayerControl target, bool blockRewind = false, bool ignoreBlank = false, bool ignoreIfKillerIsDead = false, bool ignoreMedic = false) 
        {
            var targetRole = RoleInfo.GetRoleInfoForPlayer(target, false).FirstOrDefault();
            // Modified vanilla checks
            if (AmongUsClient.Instance.IsGameOver) return MurderAttemptResult.SuppressKill;
            if (killer == null || killer.Data == null || (killer.Data.IsDead && !ignoreIfKillerIsDead) || killer.Data.Disconnected) return MurderAttemptResult.SuppressKill; // Allow non Impostor kills compared to vanilla code
            if (target == null || target.Data == null || target.Data.IsDead || target.Data.Disconnected) return MurderAttemptResult.SuppressKill; // Allow killing players in vents compared to vanilla code
            if (GameOptionsManager.Instance.currentGameOptions.GameMode == GameModes.HideNSeek) return MurderAttemptResult.PerformKill;

            // Handle first kill attempt
            if (MapOptions.shieldFirstKill && MapOptions.firstKillPlayer == target) return MurderAttemptResult.SuppressKill;

            // Handle blank shot
            if (!ignoreBlank && Pursuer.blankedList.Any(x => x.PlayerId == killer.PlayerId)) {
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetBlanked, Hazel.SendOption.Reliable, -1);
                writer.Write(killer.PlayerId);
                writer.Write((byte)0);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.SetBlanked(killer.PlayerId, 0);

                return MurderAttemptResult.BlankKill;
            }

            // Block impostor shielded kill
            if (!ignoreMedic && Medic.shielded != null && Medic.shielded == target) {
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(killer.NetId, (byte)CustomRPC.ShieldedMurderAttempt, Hazel.SendOption.Reliable, -1);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.ShieldedMurderAttempt();
                SoundEffectsManager.Play("fail");
                return MurderAttemptResult.SuppressKill;
            }

            // Block impostor not fully grown mini kill
            else if (Mini.Player != null && target == Mini.Player && !Mini.IsGrownUp()) 
            {
                return MurderAttemptResult.SuppressKill;
            }

            // Block Time Master with time shield kill
            else if (TimeMaster.shieldActive && TimeMaster.Player != null && TimeMaster.Player == target) {
                if (!blockRewind) { // Only rewind the attempt was not called because a meeting startet 
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(killer.NetId, (byte)CustomRPC.TimeMasterRewindTime, Hazel.SendOption.Reliable, -1);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.TimeMasterRewindTime();
                }
                return MurderAttemptResult.SuppressKill;
            }

            // Thief if hit crew only kill if setting says so, but also kill the thief.
            else if (Thief.IsFailedThiefKill(target, killer, targetRole)) 
            {
                if (!CheckArmored(killer, true, true))
                    Thief.suicideFlag = true;
                return MurderAttemptResult.SuppressKill;
            }

            //Veteran with alert active
            else if (Veteran.Player != null && Veteran.AlertActive && Veteran.Player == target) 
            {
                if (Medic.shielded != null && Medic.shielded == target)
                {
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(killer.NetId, (byte)CustomRPC.ShieldedMurderAttempt, SendOption.Reliable, -1);
                    writer.Write(target.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.ShieldedMurderAttempt();
                }
                return MurderAttemptResult.MirrorKill;
            }

            // Block Armored with armor kill
            
            else if (CheckArmored(target, true, killer == PlayerControl.LocalPlayer, Sheriff.Player == null || killer.PlayerId != Sheriff.Player.PlayerId || !target.IsCrew() && Sheriff.canKillNeutrals || IsKiller(target))) 
            {
                return MurderAttemptResult.BlankKill;
            }
            
            if (TransportationToolPatches.isUsingTransportation(target) && !blockRewind && killer == Vampire.Player) 
            {
                return MurderAttemptResult.DelayVampireKill;
            } 
            else if (TransportationToolPatches.isUsingTransportation(target)) return MurderAttemptResult.SuppressKill;
            return MurderAttemptResult.PerformKill;
        }

        public static void MurderPlayer(PlayerControl killer, PlayerControl target, bool showAnimation) 
        {
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.UncheckedMurderPlayer, Hazel.SendOption.Reliable, -1);
            writer.Write(killer.PlayerId);
            writer.Write(target.PlayerId);
            writer.Write(showAnimation ? Byte.MaxValue : 0);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.UncheckedMurderPlayer(killer.PlayerId, target.PlayerId, showAnimation ? Byte.MaxValue : (byte)0);
        }
        public static MurderAttemptResult CheckMurderAttemptAndKill(PlayerControl killer, PlayerControl target, bool isMeetingStart = false, bool showAnimation = true, bool ignoreBlank = false, bool ignoreIfKillerIsDead = false)  
        {
            // The local player checks for the validity of the kill and performs it afterwards (different to vanilla, where the host performs all the checks)
            // The kill attempt will be shared using a custom RPC, hence combining modded and unmodded versions is impossible
            MurderAttemptResult murder = CheckMuderAttempt(killer, target, isMeetingStart, ignoreBlank, ignoreIfKillerIsDead);

            if (murder == MurderAttemptResult.PerformKill) 
            {
                MurderPlayer(killer, target, showAnimation);
            }
            else if (murder == MurderAttemptResult.MirrorKill) 
            {
                MurderPlayer(target, killer, showAnimation);
            }
            else if (murder == MurderAttemptResult.DelayVampireKill) 
            {
                HudManager.Instance.StartCoroutine(Effects.Lerp(10f, new Action<float>((p) => 
                {

                    if (!TransportationToolPatches.isUsingTransportation(target) && Vampire.bitten != null) 
                    {
                        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.VampireSetBitten, Hazel.SendOption.Reliable, -1);
                        writer.Write(byte.MaxValue);
                        writer.Write(byte.MaxValue);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        RPCProcedure.VampireSetBitten(byte.MaxValue, byte.MaxValue);
                        MurderPlayer(killer, target, showAnimation);
                    }
                })));
            }
            return murder;            
        }
    
        public static void ShareGameVersion() 
        {
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.VersionHandshake, Hazel.SendOption.Reliable, -1);
            writer.Write((byte)TownOfSushiPlugin.Version.Major);
            writer.Write((byte)TownOfSushiPlugin.Version.Minor);
            writer.Write((byte)TownOfSushiPlugin.Version.Build);
            writer.Write(AmongUsClient.Instance.AmHost ? Patches.GameStartManagerPatch.timer : -1f);
            writer.WritePacked(AmongUsClient.Instance.ClientId);
            writer.Write((byte)(TownOfSushiPlugin.Version.Revision < 0 ? 0xFF : TownOfSushiPlugin.Version.Revision));
            writer.Write(Assembly.GetExecutingAssembly().ManifestModule.ModuleVersionId.ToByteArray());
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.VersionHandshake(TownOfSushiPlugin.Version.Major, TownOfSushiPlugin.Version.Minor, TownOfSushiPlugin.Version.Build, TownOfSushiPlugin.Version.Revision, Assembly.GetExecutingAssembly().ManifestModule.ModuleVersionId, AmongUsClient.Instance.ClientId);
        }

        public static List<PlayerControl> GetKillerTeamMembers(PlayerControl player) 
        {
            List<PlayerControl> team = new List<PlayerControl>();
            foreach(PlayerControl p in PlayerControl.AllPlayerControls) {
                if (player.Data.Role.IsImpostor && p.Data.Role.IsImpostor && player.PlayerId != p.PlayerId && team.All(x => x.PlayerId != p.PlayerId)) team.Add(p);
                else if (player == Jackal.Player && p == Sidekick.Player) team.Add(p); 
                else if (player == Sidekick.Player && p == Jackal.Player) team.Add(p);
            }
            
            return team;
        }

        public static void Shuffle<T>(this List<T> list)
        {
            for (var i = list.Count - 1; i > 0; --i)
            {
                var j = UnityEngine.Random.Range(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        public static bool IsWinner(this string playerName)
        {
            var flag = false;
            var winners = EndGameResult.CachedWinners;

            foreach (var win in winners)
            {
                if (win.PlayerName == playerName)
                {
                    flag = true;
                    break;
                }
            }

            return flag;
        }

        public static bool IsNeutral(this PlayerControl player) 
        {
            RoleInfo roleInfo = RoleInfo.GetRoleInfoForPlayer(player, false).FirstOrDefault();
            if (roleInfo != null)
                return roleInfo.FactionId == Factions.Neutral;
            return false;
        }
        public static bool IsNeutralKiller(this PlayerControl player) 
        {
            RoleInfo roleInfo = RoleInfo.GetRoleInfoForPlayer(player, false).FirstOrDefault();
            if (roleInfo != null)
                return roleInfo.FactionId == Factions.NeutralKiller || Jackal.formerJackals.Any(x => x == player);
            return false;
        }
        public static bool IsCrew(this PlayerControl player) 
        {
            RoleInfo roleInfo = RoleInfo.GetRoleInfoForPlayer(player, false).FirstOrDefault();
            if (roleInfo != null)
                return roleInfo.FactionId == Factions.Crewmate;
            return false;
        }
        public static bool IsImp(this PlayerControl player) 
        {
            RoleInfo roleInfo = RoleInfo.GetRoleInfoForPlayer(player, false).FirstOrDefault();
            if (roleInfo != null)
                return roleInfo.FactionId == Factions.Impostor;
            return false;
        }
        public static bool CheckVeteranAlertKill(this PlayerControl target)
        {
            bool CanKill = Veteran.Player == target && Veteran.AlertActive;
            if (CanKill)
            {
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.VeterenAlertKill, SendOption.Reliable, -1);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.VeterenAlertKill(PlayerControl.LocalPlayer.PlayerId);
            }
            return CanKill;
        }

        public static bool IsKiller(this PlayerControl player) 
        {
            return player.Data.Role.IsImpostor || player.IsNeutralKiller();
        }
        public static void ShowRoleInfo()
        {
            var role = RoleInfo.GetRoleInfoForPlayer(PlayerControl.LocalPlayer, false);
			foreach (RoleInfo roleInfo in role)
            {
                if (role == null) return;
                
                var stringb = new StringBuilder();
                stringb.Append(ColorString(roleInfo.color, $"{roleInfo.name} Description:\n"));
                stringb.Append(ColorString(roleInfo.color, $"{roleInfo.RoleDescription}\n\n"));
                
                FastDestroyableSingleton<HudManager>.Instance.ShowPopUp(stringb.ToString());
                SoundEffectsManager.Play("knockKnock");
            }
        }
        public static bool zoomOutStatus = false;
        public static void ToggleZoom(bool reset=false) 
        {
            float orthographicSize = reset || zoomOutStatus ? 3f : 12f;

            zoomOutStatus = !zoomOutStatus && !reset;
            Camera.main.orthographicSize = orthographicSize;
            foreach (var cam in Camera.allCameras) {
                if (cam != null && cam.gameObject.name == "UI Camera") cam.orthographicSize = orthographicSize;  // The UI is scaled too, else we cant click the buttons. Downside: map is super small.
            }

            var tzGO = GameObject.Find("TOGGLEZOOMBUTTON");
            if (tzGO != null) 
            {
                var rend = tzGO.transform.Find("Inactive").GetComponent<SpriteRenderer>();
                var rendActive = tzGO.transform.Find("Active").GetComponent<SpriteRenderer>();
                rend.sprite = zoomOutStatus ? LoadSpriteFromResources("TownOfSushi.Resources.Plus_Button.png", 100f) : Helpers.LoadSpriteFromResources("TownOfSushi.Resources.Minus_Button.png", 100f);
                rendActive.sprite = zoomOutStatus ? LoadSpriteFromResources("TownOfSushi.Resources.Plus_ButtonActive.png", 100f) : Helpers.LoadSpriteFromResources("TownOfSushi.Resources.Minus_ButtonActive.png", 100f);
                tzGO.transform.localScale = new Vector3(1.2f, 1.2f, 1f) * (zoomOutStatus ? 4 : 1);
            }

            ResolutionManager.ResolutionChanged.Invoke((float)Screen.width / Screen.height, Screen.width, Screen.height, Screen.fullScreen); // This will move button positions to the correct position.
        }

        private static long GetBuiltInTicks()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var builtin = assembly.GetType("Builtin");
            if (builtin == null) return 0;
            var field = builtin.GetField("CompileTime");
            if (field == null) return 0;
            var value = field.GetValue(null);
            if (value == null) return 0;
            return (long)value;
        }

        public static bool HasImpVision(NetworkedPlayerInfo player) 
        {
            return player.Role.IsImpostor
                || Jackal.Player != null && Jackal.Player.PlayerId == player.PlayerId || Jackal.formerJackals.Any(x => x.PlayerId == player.PlayerId)
                || (Sidekick.Player != null && Sidekick.Player.PlayerId == player.PlayerId)
                || (Glitch.Player != null && Glitch.Player.PlayerId == player.PlayerId)
                || (Werewolf.Player != null && Werewolf.Player.PlayerId == player.PlayerId)
                || (VengefulRomantic.Player != null && VengefulRomantic.Player.PlayerId == player.PlayerId)
                || (SerialKiller.Player != null && SerialKiller.HasImpostorVision && SerialKiller.Player.PlayerId == player.PlayerId)
                || (Spy.Player != null && Spy.Player.PlayerId == player.PlayerId && Spy.hasImpostorVision)
                || (Jester.jester != null && Jester.jester.PlayerId == player.PlayerId && Jester.hasImpostorVision)
                || (Thief.Player != null && Thief.Player.PlayerId == player.PlayerId && Thief.hasImpostorVision);
        }
        
        public static object TryCast(this Il2CppObjectBase self, Type type)
        {
            return AccessTools.Method(self.GetType(), nameof(Il2CppObjectBase.TryCast)).MakeGenericMethod(type).Invoke(self, Array.Empty<object>());
        }
    }
}