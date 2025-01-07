using System.Collections;
using Discord;
using Il2CppInterop.Runtime.InteropTypes;
using Reactor.Networking;
using System.IO;
using static TownOfSushi.Roles.Glitch;
using TownOfSushi.Utilities;

namespace TownOfSushi
{
    [HarmonyPatch]
    public static class Utils
    {
        private static NetworkedPlayerInfo voteTarget = null;
        public static Dictionary<byte, PoolablePlayer> playerIcons = new Dictionary<byte, PoolablePlayer>();
        public static void Morph(PlayerControl player, PlayerControl MorphedPlayer, bool resetAnim = false)
        {
            if (CamouflageUnCamouflagePatch.IsCamouflaged) return;
            if (player.GetCustomOutfitType() != CustomPlayerOutfitType.Morph)
                player.SetOutfit(CustomPlayerOutfitType.Morph, MorphedPlayer.Data.DefaultOutfit);
        }

        public static void Unmorph(PlayerControl player)
        {
            player.SetOutfit(CustomPlayerOutfitType.Default);
        }

        public class VisualAppearance    
        {        
            public float SpeedFactor { get; set; } = 1.0f;        
            public Vector3 SizeFactor { get; set; } = new Vector3(0.7f, 0.7f, 1.0f);
        }

        public static bool IsHacked(this PlayerControl player)
        {
            return GetRoles(RoleEnum.Glitch).Any(role =>
            {
                var glitch = (Glitch)role;
                var hackedPlayer = glitch.Hacked;
                return hackedPlayer != null && player.PlayerId == hackedPlayer.PlayerId && !hackedPlayer.Data.IsDead && !glitch.Player.Data.IsDead;
            });
        }
        public static bool AbilityUsed(PlayerControl player, PlayerControl target = null)
        {
            if (player.IsHacked())
            {
                Coroutines.Start(AbilityCoroutine.Hack(player));
                return false;
            }
            var targetId = byte.MaxValue;
            if (target != null) targetId = target.PlayerId;
            Rpc(CustomRPC.AbilityTrigger, player.PlayerId, targetId);
            foreach (Role hunterRole in GetRoles(RoleEnum.Hunter))
            {
                Hunter hunter = (Hunter)hunterRole;
                if (hunter.StalkedPlayer == player) hunter.RpcCatchPlayer(player);
            }
            return true;
        }

        public interface IVisualAlteration
        {
            bool TryGetModifiedAppearance(out VisualAppearance appearance);
        }

        public static void GroupCamouflage()
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                Camouflage(player);
            }
        }
        public static bool IsJailed(this PlayerControl player)
        {
            return GetRoles(RoleEnum.Jailor).Any(role =>
            {
                var jailor = (Jailor)role;
                return jailor.Jailed == player && !player.Data.IsDead && !player.Data.Disconnected;
            });
        }

        public static void Camouflage(PlayerControl player)
        {
            if (player.GetCustomOutfitType() != CustomPlayerOutfitType.Camouflage &&
                    player.GetCustomOutfitType() != CustomPlayerOutfitType.Swooper &&
                    player.GetCustomOutfitType() != CustomPlayerOutfitType.PlayerNameOnly)
            {
                player.SetOutfit(CustomPlayerOutfitType.Camouflage, new NetworkedPlayerInfo.PlayerOutfit()
                {
                    ColorId = player.GetDefaultOutfit().ColorId,
                    HatId = "",
                    SkinId = "",
                    VisorId = "",
                    PlayerName = " ",
                    PetId = ""
                });
                PlayerMaterial.SetColors(Color.grey, player.myRend());
                player.nameText().color = Color.clear;
                player.cosmetics.colorBlindText.color = Color.clear;
            }
        }

        public static void UnCamouflage()
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.Is(RoleEnum.Swooper))
                {
                    var swooper = GetRole<Swooper>(player);
                    if (swooper.IsSwooped) continue;
                }
                else if (player.Is(RoleEnum.Venerer))
                {
                    var venerer = GetRole<Venerer>(player);
                    if (venerer.IsCamouflaged) continue;
                }
                else if (player.Is(RoleEnum.Morphling))
                {
                    var morphling = GetRole<Morphling>(player);
                    if (morphling.Morphed) continue;
                }
                else if (player.Is(RoleEnum.Glitch))
                {
                    var glitch = GetRole<Glitch>(player);
                    if (glitch.IsUsingMimic) continue;
                }
                else if (player.Is(RoleEnum.Hitman))
                {
                    var Hitman = GetRole<Hitman>(player);
                    if (Hitman.IsUsingMorph) continue;
                }
                else if (CamouflageUnCamouflagePatch.IsCamouflaged) continue;
                Unmorph(player);
            }
        }

        public static void AddUnique<T>(this Il2CppSystem.Collections.Generic.List<T> self, T item)
            where T : IDisconnectHandler
        {
            if (!self.Contains(item)) self.Add(item);
        }

        [HarmonyPatch(typeof(ActivityManager), nameof(ActivityManager.UpdateActivity))]
        [HarmonyPrefix]
        public static void Prefix([HarmonyArgument(0)] Activity activity)
        {
            activity.Details += $" TownOfSushi v" + TownOfSushi.VersionString;
        }

        public static bool RomanticCoupleChat(this PlayerControl player, PlayerControl source)
        {
            bool beloved = source.Is(RoleEnum.Romantic) && GetRole<Romantic>(source).Beloved.PlayerId == player.PlayerId;
            bool romantic = player.Is(RoleEnum.Romantic) && GetRole<Romantic>(player).Beloved.PlayerId == source.PlayerId;
            return romantic || beloved;
        }
        public static bool HasRomanticCouple(this PlayerControl player)
        {
            bool beloved = false;
            foreach (var role in GetRoles(RoleEnum.Romantic))
                if (((Romantic)role).Beloved != null && ((Romantic)role).Beloved.PlayerId == player.PlayerId)
                    beloved = true;
            return player.Is(RoleEnum.Romantic) || beloved;
        }

        public static bool LastImp()
        {
            return PlayerControl.AllPlayerControls.ToArray().Count(x => x.Is(Faction.Impostors) && !(x.Data.IsDead || x.Data.Disconnected)) == 1;
        }

        //Code from TOUReworked
        public static bool HasTasks(this PlayerControl player)
        {
            if (player == null)
                return false;
            
            var taskersFlag = player.Is(Faction.Crewmates) || player.Is(RoleEnum.Agent) || player.Is(RoleAlignment.NeutralBenign);
            var noTaskers = player.Is(Faction.Neutral) || player.Is(Faction.Impostors);
            var flag1 = taskersFlag;
            var flag2 = noTaskers;
            var flag = flag1 || !flag2;
            return flag;
        }

        public static bool Is(this PlayerControl player, RoleEnum roleType)
        {
            return GetPlayerRole(player)?.RoleType == roleType;
        }
        public static bool Is(this PlayerControl player, RoleAlignment alignment)
        {
            return GetPlayerRole(player)?.RoleAlignment == alignment;
        }
        public static bool Is(this PlayerControl player, ModifierEnum modifierType)
        {
            return GetModifier(player)?.ModifierType == modifierType;
        }
        public static bool Is(this PlayerControl player, AbilityEnum abilityType)
        {
            return GetAbility(player)?.AbilityType == abilityType;
        }
        public static bool Is(this PlayerControl player, Faction faction)
        {
            return GetPlayerRole(player)?.Faction == faction;
        }
        public static float KillDistance()
        {
            return GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
        }

        public static List<PlayerControl> GetCrewmates(List<PlayerControl> impostors)
        {
            return PlayerControl.AllPlayerControls.ToArray().Where(
                player => !impostors.Any(imp => imp.PlayerId == player.PlayerId)
            ).ToList();
        }

        public static List<PlayerControl> GetImpostors(List<NetworkedPlayerInfo> infected)
        {
            var impostors = new List<PlayerControl>();
            foreach (var impData in infected)
                impostors.Add(impData.Object);

            return impostors;
        }

        public static RoleEnum GetRole(PlayerControl player)
        {
            if (player == null) return RoleEnum.None;
            if (player.Data == null) return RoleEnum.None;

            var role = GetPlayerRole(player);
            if (role != null) return role.RoleType;

            return player.Data.IsImpostor() ? RoleEnum.Impostor : RoleEnum.Crewmate;
        }

        public static PlayerControl PlayerById(byte id)
        {
            foreach (var player in PlayerControl.AllPlayerControls)
                if (player.PlayerId == id)
                    return player;

            return null;
        }

        public static bool IsExeTarget(this PlayerControl player)
        {
            return GetRoles(RoleEnum.Executioner).Any(role =>
            {
                var exeTarget = ((Executioner)role).target;
                return exeTarget != null && player.PlayerId == exeTarget.PlayerId;
            });
        }
        public static bool IsBeloved(this PlayerControl player)
        {
            return GetRoles(RoleEnum.Romantic).Any(role =>
            {
                var beloved = ((Romantic)role).Beloved;
                return beloved != null && player.PlayerId == beloved.PlayerId;
            });
        }
        public static bool IsRomantic(this PlayerControl player)
        {
            return GetRoles(RoleEnum.Romantic).Any(role =>
            {
                var romantic = ((Romantic)role).Player;
                return romantic != null && player.PlayerId == romantic.PlayerId;
            });
        }
        public static bool IsGATarget(this PlayerControl player)
        {
            return GetRoles(RoleEnum.GuardianAngel).Any(role =>
            {
                var gaTarget = ((GuardianAngel)role).target;
                return gaTarget != null && player.PlayerId == gaTarget.PlayerId;
            });
        }
        public static bool IsSpelled(this PlayerControl player)
        {   
            return GetRoles(RoleEnum.Witch).Any(role =>    
            {
                var spelledPlayers = ((Witch)role).SpelledPlayers;
                return spelledPlayers != null && spelledPlayers.Contains(player.PlayerId)  && !player.Data.IsDead && !role.Player.Data.IsDead;    
            });
        }

        public static bool IsShielded(this PlayerControl player)
        {
            return GetRoles(RoleEnum.Medic).Any(role =>
            {
                var shieldedPlayer = ((Medic)role).ShieldedPlayer;
                return shieldedPlayer != null && player.PlayerId == shieldedPlayer.PlayerId;
            });
        }
        public static Medic GetMedic(this PlayerControl player)
        {
            return GetRoles(RoleEnum.Medic).FirstOrDefault(role =>
            {
                var shieldedPlayer = ((Medic)role).ShieldedPlayer;
                return shieldedPlayer != null && player.PlayerId == shieldedPlayer.PlayerId;
            }) as Medic;
        }

        public static bool IsOnAlert(this PlayerControl player)
        {
            return GetRoles(RoleEnum.Veteran).Any(role =>
            {
                var veteran = (Veteran)role;
                return veteran != null && veteran.OnAlert && player.PlayerId == veteran.Player.PlayerId;
            });
        }

        public static bool IsProtected(this PlayerControl player)
        {
            return GetRoles(RoleEnum.GuardianAngel).Any(role =>
            {
                var gaTarget = ((GuardianAngel)role).target;
                var ga = (GuardianAngel)role;
                return gaTarget != null && ga.Protecting && player.PlayerId == gaTarget.PlayerId;
            });
        }

        public static bool IsInfected(this PlayerControl player)
        {
            return GetRoles(RoleEnum.Plaguebearer).Any(role =>
            {
                var plaguebearer = (Plaguebearer)role;
                return plaguebearer != null && (plaguebearer.InfectedPlayers.Contains(player.PlayerId) || player.PlayerId == plaguebearer.Player.PlayerId);
            });
        }

        public static List<bool> Interact(PlayerControl player, PlayerControl target, bool IsBeingKilled = false)
        {
            bool fullCooldownReset = false;
            bool gaReset = false;
            bool zeroSecReset = false;
            bool abilityUsed = false;
            var checkHack = AbilityUsed(player, target);
            if (!checkHack) return new List<bool> { false, false, false, true, false };
            if (target.IsInfected() || player.IsInfected())
            {
                foreach (var pb in GetRoles(RoleEnum.Plaguebearer)) ((Plaguebearer)pb).RpcSpreadInfection(target, player);
            }
            if (target == ShowRoundOneShield.FirstRoundShielded && IsBeingKilled)
            {
                zeroSecReset = true;
            }
            
            else if (target.Is(RoleEnum.Pestilence))
            {
                if (player.IsShielded())
                {
                    var medic = player.GetMedic().Player.PlayerId;
                    Rpc(CustomRPC.AttemptSound, medic, player.PlayerId);

                    if (CustomGameOptions.ShieldBreaks) fullCooldownReset = true;
                    else zeroSecReset = true;

                    MedicStopKill.BreakShield(medic, player.PlayerId, CustomGameOptions.ShieldBreaks);
                }
                else if (player.IsProtected()) gaReset = true;
                else if (player == ShowRoundOneShield.FirstRoundShielded) zeroSecReset = true;
                else RpcMurderPlayer(target, player);
            }

           
            
            else if (target.IsOnAlert())
            {
                if (player.Is(RoleEnum.Pestilence)) zeroSecReset = true;
                else if (player.IsShielded())
                {
                    var medic = player.GetMedic().Player.PlayerId;
                    Rpc(CustomRPC.AttemptSound, medic, player.PlayerId);

                    if (CustomGameOptions.ShieldBreaks) fullCooldownReset = true;
                    else zeroSecReset = true;

                    MedicStopKill.BreakShield(medic, player.PlayerId, CustomGameOptions.ShieldBreaks);
                }
                else if (player.IsProtected()) gaReset = true;
                else if (player == ShowRoundOneShield.FirstRoundShielded) zeroSecReset = true;
                else RpcMurderPlayer(target, player);

                if (IsBeingKilled && CustomGameOptions.KilledOnAlert)
                {
                    if (target.IsShielded())
                    {
                        var medic = target.GetMedic().Player.PlayerId;
                        Rpc(CustomRPC.AttemptSound, medic, target.PlayerId);

                        if (CustomGameOptions.ShieldBreaks) fullCooldownReset = true;
                        else zeroSecReset = true;

                        MedicStopKill.BreakShield(medic, target.PlayerId, CustomGameOptions.ShieldBreaks);
                    }
                    else if (target.IsProtected()) gaReset = true;
                    else if (target == ShowRoundOneShield.FirstRoundShielded) zeroSecReset = true;
                    else
                    {
                        if (player.Is(RoleEnum.Glitch))
                        {
                            var glitch = GetRole<Glitch>(player);
                            glitch.LastKill = DateTime.UtcNow;
                        }
                        else if (player.Is(RoleEnum.Juggernaut))
                        {
                            var jugg = GetRole<Juggernaut>(player);
                            jugg.JuggKills += 1;
                            jugg.LastKill = DateTime.UtcNow;
                        }
                        else if (player.Is(RoleEnum.Pestilence))
                        {
                            var pest = GetRole<Pestilence>(player);
                            pest.LastKill = DateTime.UtcNow;
                        }
                        else if (player.Is(RoleEnum.Hitman))
                        {
                            var pest = GetRole<Hitman>(player);
                            pest.LastKill = DateTime.UtcNow;
                        }
                        else if (player.Is(RoleEnum.Vampire))
                        {
                            var vamp = GetRole<Vampire>(player);
                            vamp.LastBit = DateTime.UtcNow;
                        }
                        else if (player.Is(RoleEnum.SerialKiller))
                        {
                            var ww = GetRole<SerialKiller>(player);
                            ww.LastKilled = DateTime.UtcNow;
                        }
                        else if (player.Is(RoleEnum.Werewolf))
                        {
                            var ww = GetRole<Werewolf>(player);
                            ww.LastMauled = DateTime.UtcNow;
                        }
                        RpcMurderPlayer(player, target);
                        abilityUsed = true;
                        fullCooldownReset = true;
                        gaReset = false;
                        zeroSecReset = false;
                    }
                }
            }
            else if (target.IsShielded() && IsBeingKilled)
            {
                Rpc(CustomRPC.AttemptSound, target.GetMedic().Player.PlayerId, target.PlayerId);

                System.Console.WriteLine(CustomGameOptions.ShieldBreaks + "- shield break");
                if (CustomGameOptions.ShieldBreaks) fullCooldownReset = true;
                else zeroSecReset = true;
                MedicStopKill.BreakShield(target.GetMedic().Player.PlayerId, target.PlayerId, CustomGameOptions.ShieldBreaks);
            }
            else if (target.IsProtected() && IsBeingKilled)
            {
                gaReset = true;
            }
            else if (IsBeingKilled)
            {
                if (player.Is(RoleEnum.Glitch))
                {
                    var glitch = GetRole<Glitch>(player);
                    glitch.LastKill = DateTime.UtcNow;
                }
                else if (player.Is(RoleEnum.Juggernaut))
                {
                    var jugg = GetRole<Juggernaut>(player);
                    jugg.JuggKills += 1;
                    jugg.LastKill = DateTime.UtcNow;
                }
                else if (player.Is(RoleEnum.Pestilence))
                {
                    var pest = GetRole<Pestilence>(player);
                    pest.LastKill = DateTime.UtcNow;
                }
                else if (player.Is(RoleEnum.Hitman))
                {
                    var pest = GetRole<Hitman>(player);
                    pest.LastKill = DateTime.UtcNow;
                }
                else if (player.Is(RoleEnum.Vampire))
                {
                    var Vampire = GetRole<Vampire>(player);
                    Vampire.LastBit = DateTime.UtcNow;
                }
                else if (player.Is(RoleEnum.SerialKiller))
                {
                    var ww = GetRole<SerialKiller>(player);
                    ww.LastKilled = DateTime.UtcNow;
                }
                else if (player.Is(RoleEnum.Werewolf))
                {
                    var ww = GetRole<Werewolf>(player);
                    ww.LastMauled = DateTime.UtcNow;
                }
                RpcMurderPlayer(player, target);
                abilityUsed = true;
                fullCooldownReset = true;
            }
            else
            {
                abilityUsed = true;
                fullCooldownReset = true;
            }
            var reset = new List<bool>();
            reset.Add(fullCooldownReset);
            reset.Add(gaReset);
            reset.Add(zeroSecReset);
            reset.Add(abilityUsed);
            return reset;
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

        public static PlayerControl GetClosestPlayer(PlayerControl refPlayer, List<PlayerControl> AllPlayers)
        {
            var num = double.MaxValue;
            var refPosition = refPlayer.GetTruePosition();
            PlayerControl result = null;
            foreach (var player in AllPlayers)
            {
                if (player.Data.IsDead || player.PlayerId == refPlayer.PlayerId || !player.Collider.enabled || player.inVent) continue;
                var playerPosition = player.GetTruePosition();
                var distBetweenPlayers = Vector2.Distance(refPosition, playerPosition);
                var isClosest = distBetweenPlayers < num;
                if (!isClosest) continue;
                var vector = playerPosition - refPosition;
                if (PhysicsHelpers.AnyNonTriggersBetween(
                    refPosition, vector.normalized, vector.magnitude, Constants.ShipAndObjectsMask
                )) continue;
                num = distBetweenPlayers;
                result = player;
            }
            
            return result;
        }
        public static void SetTarget(
            ref PlayerControl closestPlayer,
            KillButton button,
            float maxDistance = float.NaN,
            List<PlayerControl> targets = null
        )
        {
            if (!button.isActiveAndEnabled) return;

            button.SetTarget(
                SetClosestPlayer(ref closestPlayer, maxDistance, targets)
            );
        }

        public static PlayerControl SetClosestPlayer(
            ref PlayerControl closestPlayer,
            float maxDistance = float.NaN,
            List<PlayerControl> targets = null
        )
        {
            if (float.IsNaN(maxDistance))
                maxDistance = KillDistance();
            var player = GetClosestPlayer(
                PlayerControl.LocalPlayer,
                targets ?? PlayerControl.AllPlayerControls.ToArray().ToList()
            );
            var closeEnough = player == null || (
                GetDistBetweenPlayers(PlayerControl.LocalPlayer, player) < maxDistance
            );
            return closestPlayer = closeEnough ? player : null;
        }

        public static double GetDistBetweenPlayers(PlayerControl player, PlayerControl refplayer)
        {
            var truePosition = refplayer.GetTruePosition();
            var truePosition2 = player.GetTruePosition();
            return Vector2.Distance(truePosition, truePosition2);
        }

        public static void RpcMurderPlayer(PlayerControl killer, PlayerControl target)
        {
            MurderPlayer(killer, target, true);
            Rpc(CustomRPC.BypassKill, killer.PlayerId, target.PlayerId);
        }

        public static void RpcMurderPlayerNoJump(PlayerControl killer, PlayerControl target)
        {
            MurderPlayer(killer, target, false);
            Rpc(CustomRPC.BypassKill2, killer.PlayerId, target.PlayerId);
        }

        public static void RpcMultiMurderPlayer(PlayerControl killer, PlayerControl target)
        {
            MurderPlayer(killer, target, false);
            Rpc(CustomRPC.BypassMultiKill, killer.PlayerId, target.PlayerId);
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.StartMeeting))]
        class StartMeetingPatch 
        {
            public static void Prefix(PlayerControl __instance, [HarmonyArgument(0)]NetworkedPlayerInfo meetingTarget) 
            {
                voteTarget = meetingTarget;

                // Close In-Game Settings Display if open
                CustomOption.HudManagerUpdate.CloseSettings();

                // Reset zoomed out ghosts
                ToggleZoom(reset: true);
            }
        }

        [HarmonyPatch(typeof (MapBehaviour), "FixedUpdate")]  
        public static class MapBehaviourPatch  
        {
            public static void Postfix(MapBehaviour __instance)    
            {      
                Role role = GetPlayerRole(PlayerControl.LocalPlayer);      
                __instance.ColorControl.baseColor = role.Color;      
                __instance.ColorControl.SetColor(role.Color);
            }
        }

        public static int LineCount(string text) 
        {
            return text.Count(c => c == '\n');
        }

        public static bool zoomOutStatus = false;
        public static void ToggleZoom(bool reset = false) 
        {
            float orthographicSize = reset || zoomOutStatus ? 3f : 12f;

            zoomOutStatus = !zoomOutStatus && !reset;
            Camera.main.orthographicSize = orthographicSize;
            foreach (var cam in Camera.allCameras) {
                if (cam != null && cam.gameObject.name == "UI Camera") cam.orthographicSize = orthographicSize;
            }

            var tzGO = GameObject.Find("TOGGLEZOOMBUTTON");
            if (tzGO != null) {
                var rend = tzGO.transform.Find("Inactive").GetComponent<SpriteRenderer>();
                var rendActive = tzGO.transform.Find("Active").GetComponent<SpriteRenderer>();
                rend.sprite = zoomOutStatus ? LoadSpriteFromResources("TownOfSushi.Resources.Plus_Button.png", 100f) : LoadSpriteFromResources("TownOfSushi.Resources.Minus_Button.png", 100f);
                rendActive.sprite = zoomOutStatus ? LoadSpriteFromResources("TownOfSushi.Resources.Plus_ButtonActive.png", 100f) : LoadSpriteFromResources("TownOfSushi.Resources.Minus_ButtonActive.png", 100f);
            }
            ResolutionManager.ResolutionChanged.Invoke((float)Screen.width / Screen.height, Screen.width, Screen.height, Screen.fullScreen); // This will move button positions to the correct position.
        }

        public static void HandleShareOptions(byte numberOfOptions, MessageReader reader) 
        {
            try {
                for (int i = 0; i < numberOfOptions; i++) {
                    uint optionId = reader.ReadPackedUInt32();
                    uint selection = reader.ReadPackedUInt32();
                    CustomOption.CustomOption option = CustomOption.CustomOption.Options.First(option => option.id == (int)optionId);
                    option.UpdateSelection((int)selection, i == numberOfOptions - 1);
                }
            } catch (Exception e) {
                TownOfSushi.Logger.LogError("Error while deserializing options: " + e.Message);
            }
        }

        public static bool NoButton(PlayerControl target, RoleEnum role)
        {
            return PlayerControl.AllPlayerControls.Count <= 1 || target == null || target.Data == null || !target.Is(role);
        }
        public static bool ButtonUsable(KillButton button)
        {
            return button.isActiveAndEnabled && !button.isCoolingDown;
        }
        public static bool SetActive(PlayerControl target, HudManager hud)
        {
            var buttonFlag = hud.UseButton.isActiveAndEnabled || hud.PetButton.isActiveAndEnabled;
            var meetingFlag = !MeetingHud.Instance;
            return !target.Data.IsDead && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started && !LobbyBehaviour.Instance && meetingFlag && buttonFlag;
        }
        public static void MurderPlayer(PlayerControl killer, PlayerControl target, bool jumpToBody)
        {
            var data = target.Data;
            var targetRole = GetPlayerRole(target);
            var killerRole = GetPlayerRole(killer);
            if (data != null && !data.IsDead)
            {
                if (ShowRoundOneShield.DiedFirst == "") ShowRoundOneShield.DiedFirst = target.GetDefaultOutfit().PlayerName;

                if (killer.Is(AbilityEnum.Chameleon))
                {
                    var shy = GetAbility<Chameleon>(killer);
                    shy.Opacity = 1f;
                    ChameleonUpdate.SetVisiblity(killer, shy.Opacity);
                    shy.Moving = true;
                }

                if (target.Is(RoleEnum.Jailor))
                {
                    var jailor = GetRole<Jailor>(target);
                    jailor.Jailed = null;
                }

                if (PlayerControl.LocalPlayer == target)
                {
                    try
                    {
                        PlayerMenu.Singleton.Menu.Close();
                    }
                    catch { }

                }

                if (killer == PlayerControl.LocalPlayer)
                    SoundManager.Instance.PlaySound(PlayerControl.LocalPlayer.KillSfx, false, 0.8f);

                if (!killer.Is(Faction.Crewmates) && killer != target
                    && GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.Normal) GetPlayerRole(killer).Kills += 1;

                if (killer.Is(RoleEnum.Vigilante))
                {
                    var vigilante = GetRole<Vigilante>(killer);
                    if (target.Is(Faction.Impostors) || target.Is(RoleAlignment.NeutralKilling) ||
                        target.Is(RoleAlignment.NeutralEvil) && CustomGameOptions.VigilanteKillsNeutralEvil ||
                        target.Is(RoleAlignment.NeutralBenign) && CustomGameOptions.VigilanteKillsNeutralBenign) vigilante.CorrectVigilanteShot += 1;
                    else if (killer == target) vigilante.Misfired = true;
                }

                if (killer.Is(RoleEnum.Veteran))
                {
                    var veteran = GetRole<Veteran>(killer);
                    if (target.Is(Faction.Impostors) || target.Is(RoleAlignment.NeutralKilling) || target.Is(RoleAlignment.NeutralEvil)) veteran.CorrectShot += 1;
                    else if (killer != target) veteran.IncorrectShots += 1;
                }

                target.gameObject.layer = LayerMask.NameToLayer("Ghost");
                target.Visible = false;

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Mystic) && !PlayerControl.LocalPlayer.Data.IsDead)
                {
                    Flash(Colors.Mystic, 3.5f);
                }

                if (target.AmOwner)
                {
                    try
                    {
                        if (Minigame.Instance)
                        {
                            Minigame.Instance.Close();
                            Minigame.Instance.Close();
                        }

                        if (MapBehaviour.Instance)
                        {
                            MapBehaviour.Instance.Close();
                            MapBehaviour.Instance.Close();
                        }
                    }
                    catch
                    {
                    }

                    DestroyableSingleton<HudManager>.Instance.KillOverlay.ShowKillAnimation(killer.Data, data);
                    DestroyableSingleton<HudManager>.Instance.ShadowQuad.gameObject.SetActive(false);
                    target.nameText().GetComponent<MeshRenderer>().material.SetInt("_Mask", 0);
                    target.RpcSetScanner(false);
                    var importantTextTask = new GameObject("_Player").AddComponent<ImportantTextTask>();
                    importantTextTask.transform.SetParent(AmongUsClient.Instance.transform, false);
                    if (!GameOptionsManager.Instance.currentNormalGameOptions.GhostsDoTasks)
                    {
                        for (var i = 0; i < target.myTasks.Count; i++)
                        {
                            var playerTask = target.myTasks.ToArray()[i];
                            playerTask.OnRemove();
                            Object.Destroy(playerTask.gameObject);
                        }

                        target.myTasks.Clear();
                        importantTextTask.Text = DestroyableSingleton<TranslationController>.Instance.GetString(
                            StringNames.GhostIgnoreTasks,
                            new Il2CppReferenceArray<Il2CppSystem.Object>(0));
                    }
                    else
                    {
                        importantTextTask.Text = DestroyableSingleton<TranslationController>.Instance.GetString(
                            StringNames.GhostDoTasks,
                            new Il2CppReferenceArray<Il2CppSystem.Object>(0));
                    }

                    target.myTasks.Insert(0, importantTextTask);
                }

                if (jumpToBody)
                {
                    killer.MyPhysics.StartCoroutine(killer.KillAnimations.Random().CoPerformKill(killer, target));
                }
                else killer.MyPhysics.StartCoroutine(killer.KillAnimations.Random().CoPerformKill(target, target));



                if (killer != target)
                {
                    targetRole.KilledBy = " By " + ColorString(killerRole.Color, killerRole.PlayerName);
                    targetRole.DeathReason = DeathReasonEnum.Killed;
                }
                else
                    targetRole.DeathReason = DeathReasonEnum.Suicide;


                if (target.Is(ModifierEnum.Frosty))
                {
                    var frosty = GetModifier<Frosty>(target);
                    frosty.Chilled = killer;
                    frosty.LastChilled = DateTime.UtcNow;
                    frosty.IsChilled = true;
                }

                var deadBody = new DeadPlayer
                {
                    PlayerId = target.PlayerId,
                    KillerId = killer.PlayerId,
                    KillTime = DateTime.UtcNow
                };

                Murder.KilledPlayers.Add(deadBody);

                if (MeetingHud.Instance) target.Exiled();

                if (!killer.AmOwner) return;

                if (target.Is(ModifierEnum.Bait))
                {
                    BaitReport(killer, target);
                }

                //UsefulMethods.ShowTextToast("", 5f);

                if (target.Is(ModifierEnum.Aftermath))
                {
                    Aftermath.ForceAbility(killer, target);
                }

                if (!jumpToBody) return;

                if (killer.Data.IsImpostor() && GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek)
                {
                    killer.SetKillTimer(GameOptionsManager.Instance.currentHideNSeekGameOptions.KillCooldown);
                    return;
                }

                if (killer == PlayerControl.LocalPlayer && killer.Is(RoleEnum.Warlock))
                {
                    var warlock = GetRole<Warlock>(killer);
                    if (warlock.Charging)
                    {
                        warlock.UsingCharge = true;
                        warlock.ChargeUseDuration = warlock.ChargePercent * CustomGameOptions.ChargeUseDuration / 100f;
                        if (warlock.ChargeUseDuration == 0f) warlock.ChargeUseDuration += 0.01f;
                    }
                    killer.SetKillTimer(0.01f);
                    return;
                }

                if (target.Is(ModifierEnum.Diseased) && killer.Is(RoleEnum.SerialKiller))
                {
                    var werewolf = GetRole<SerialKiller>(killer);
                    werewolf.LastKilled = DateTime.UtcNow.AddSeconds((CustomGameOptions.DiseasedMultiplier - 1f) * CustomGameOptions.StabKillCd);
                    werewolf.Player.SetKillTimer(CustomGameOptions.StabKillCd * CustomGameOptions.DiseasedMultiplier);
                    return;
                }

                if (target.Is(ModifierEnum.Diseased) && killer.Is(RoleEnum.Hitman))
                {
                    var Hitman = GetRole<Hitman>(killer);
                    Hitman.LastKill = DateTime.UtcNow.AddSeconds((CustomGameOptions.DiseasedMultiplier - 1f) * CustomGameOptions.HitmanKCd);
                    Hitman.Player.SetKillTimer(CustomGameOptions.HitmanKCd * CustomGameOptions.DiseasedMultiplier);
                    return;
                }

                if (target.Is(ModifierEnum.Diseased) && killer.Is(RoleEnum.Werewolf))
                {
                    var werewolf = GetRole<Werewolf>(killer);
                    werewolf.LastMauled = DateTime.UtcNow.AddSeconds((CustomGameOptions.DiseasedMultiplier - 1f) * CustomGameOptions.MaulCooldown);
                    werewolf.Player.SetKillTimer(CustomGameOptions.MaulCooldown * CustomGameOptions.DiseasedMultiplier);
                    return;
                }

                if (target.Is(ModifierEnum.Diseased) && killer.Is(RoleEnum.Vampire))
                {
                    var vampire = GetRole<Vampire>(killer);
                    vampire.LastBit = DateTime.UtcNow.AddSeconds((CustomGameOptions.DiseasedMultiplier - 1f) * CustomGameOptions.BiteCd);
                    vampire.Player.SetKillTimer(CustomGameOptions.BiteCd * CustomGameOptions.DiseasedMultiplier);
                    return;
                }

                if (target.Is(ModifierEnum.Diseased) && killer.Is(RoleEnum.Glitch))
                {
                    var glitch = GetRole<Glitch>(killer);
                    glitch.LastKill = DateTime.UtcNow.AddSeconds((CustomGameOptions.DiseasedMultiplier - 1f) * CustomGameOptions.GlitchKillCooldown);
                    glitch.Player.SetKillTimer(CustomGameOptions.GlitchKillCooldown * CustomGameOptions.DiseasedMultiplier);
                    return;
                }

                if (target.Is(ModifierEnum.Diseased) && killer.Is(RoleEnum.Juggernaut))
                {
                    var juggernaut = GetRole<Juggernaut>(killer);
                    juggernaut.LastKill = DateTime.UtcNow.AddSeconds((CustomGameOptions.DiseasedMultiplier - 1f) * (CustomGameOptions.JuggKCd - CustomGameOptions.ReducedKCdPerKill * juggernaut.JuggKills));
                    juggernaut.Player.SetKillTimer((CustomGameOptions.JuggKCd - CustomGameOptions.ReducedKCdPerKill * juggernaut.JuggKills) * CustomGameOptions.DiseasedMultiplier);
                    return;
                }

                if (target.Is(ModifierEnum.Diseased) && killer.Is(ModifierEnum.Underdog))
                {
                    var lowerKC = (GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown - CustomGameOptions.UnderdogKillBonus) * CustomGameOptions.DiseasedMultiplier;
                    var normalKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown * CustomGameOptions.DiseasedMultiplier;
                    var upperKC = (GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown + CustomGameOptions.UnderdogKillBonus) * CustomGameOptions.DiseasedMultiplier;
                    killer.SetKillTimer(UnderdogPerformKill.LastImp() ? lowerKC : (UnderdogPerformKill.IncreasedKC() ? normalKC : upperKC));
                    return;
                }

                if (target.Is(ModifierEnum.Diseased) && killer.Data.IsImpostor())
                {
                    killer.SetKillTimer(GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown * CustomGameOptions.DiseasedMultiplier);
                    return;
                }

                if (killer.Is(ModifierEnum.Underdog))
                {
                    var lowerKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown - CustomGameOptions.UnderdogKillBonus;
                    var normalKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown;
                    var upperKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown + CustomGameOptions.UnderdogKillBonus;
                    killer.SetKillTimer(UnderdogPerformKill.LastImp() ? lowerKC : (UnderdogPerformKill.IncreasedKC() ? normalKC : upperKC));
                    return;
                }

                if (killer.Data.IsImpostor())
                {
                    killer.SetKillTimer(GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown);
                    return;
                }
            }
        }

        public static void BaitReport(PlayerControl killer, PlayerControl target)
        {
            Coroutines.Start(BaitReportDelay(killer, target));
        }

        public static IEnumerator BaitReportDelay(PlayerControl killer, PlayerControl target)
        {
            var extraDelay = Random.RandomRangeInt(0, (int) (100 * (CustomGameOptions.BaitMaxDelay - CustomGameOptions.BaitMinDelay) + 1));
            if (CustomGameOptions.BaitMaxDelay <= CustomGameOptions.BaitMinDelay)
                yield return new WaitForSeconds(CustomGameOptions.BaitMaxDelay + 0.01f);
            else
                yield return new WaitForSeconds(CustomGameOptions.BaitMinDelay + 0.01f + extraDelay/100f);
            var bodies = Object.FindObjectsOfType<DeadBody>();
            if (AmongUsClient.Instance.AmHost)
            {
                foreach (var body in bodies)
                {
                    try
                    {
                        if (body.ParentId == target.PlayerId) { killer.ReportDeadBody(target.Data); break; }
                    }
                    catch
                    {
                    }
                }
            }
            else
            {
                foreach (var body in bodies)
                {
                    try
                    {
                        if (body.ParentId == target.PlayerId)
                        {
                            Rpc(CustomRPC.BaitReport, killer.PlayerId, target.PlayerId);
                            break;
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }
        public static bool MiraHQMap() {
            return GameOptionsManager.Instance.CurrentGameOptions.MapId == 1;
        }

        public static bool AirshipMap() {
            return GameOptionsManager.Instance.CurrentGameOptions.MapId == 4;
        }
        public static bool SkeldMap() {
            return GameOptionsManager.Instance.CurrentGameOptions.MapId == 0;
        }
        public static bool PolusMap() {
            return GameOptionsManager.Instance.CurrentGameOptions.MapId == 2;
        }

        public static bool FungleMap() {           
            return GameOptionsManager.Instance.CurrentGameOptions.MapId == 5;
        }
        public static bool MushroomSabotageActive() {
            return PlayerControl.LocalPlayer.myTasks.ToArray().Any((x) => x.TaskType == TaskTypes.MushroomMixupSabotage);
        }

        public static void ResetWins()
        {
            VampireWins = false;
            CrewmatesWin = false;
            ImpostorsWin = false;
            NobodyWins = false;
            NobodyWins = false;        
            VampireWins = false;
            CrewmatesWin = false;
            ImpostorsWin = false;
            HitmanWin = false;
            GlitchWin = false;
            JuggernautWin = false;
            AgentWin = false;
            WerewolfWin = false;
            PestilenceWin = false;
            PlaguebearerWin = false;
            ArsonistWin = false;
            SerialKillerWin = false;
            JesterWin = false;
            ExecutionerWin = false;
            DoomsayerWin = false;
            VultureWin = false;
        }

        public static IEnumerable<(T1, T2)> Zip<T1, T2>(List<T1> first, List<T2> second)
        {
            return first.Zip(second, (x, y) => (x, y));
        }

        public static void Flash(Color color, float duration=1f) 
        {
            if (FastDestroyableSingleton<HudManager>.Instance == null || FastDestroyableSingleton<HudManager>.Instance.FullScreen == null) return;
            FastDestroyableSingleton<HudManager>.Instance.FullScreen.gameObject.SetActive(true);
            FastDestroyableSingleton<HudManager>.Instance.FullScreen.enabled = true;
            FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(duration, new Action<float>((p) => {
                var renderer = FastDestroyableSingleton<HudManager>.Instance.FullScreen;

                if (p < 0.5) 
                {
                    if (renderer != null)
                        renderer.color = new Color(color.r, color.g, color.b, Mathf.Clamp01(p * 2 * 0.75f));
                } else {
                    if (renderer != null)
                        renderer.color = new Color(color.r, color.g, color.b, Mathf.Clamp01((1 - p) * 2 * 0.75f));
                }
                if (p == 1f && renderer != null) renderer.enabled = false;
            })));
        }

        public static void RemoveTasks(this PlayerControl player)
        {
            var totalTasks = GameOptionsManager.Instance.currentNormalGameOptions.NumCommonTasks + GameOptionsManager.Instance.currentNormalGameOptions.NumLongTasks +
                             GameOptionsManager.Instance.currentNormalGameOptions.NumShortTasks;


            foreach (var task in player.myTasks)
                if (task.TryCast<NormalPlayerTask>() != null)
                {
                    var normalPlayerTask = task.Cast<NormalPlayerTask>();

                    var updateArrow = normalPlayerTask.taskStep > 0;

                    normalPlayerTask.taskStep = 0;
                    normalPlayerTask.Initialize();
                    if (normalPlayerTask.TaskType == TaskTypes.PickUpTowels)
                        foreach (var console in Object.FindObjectsOfType<TowelTaskConsole>())
                            console.Image.color = Color.white;
                    normalPlayerTask.taskStep = 0;
                    if (normalPlayerTask.TaskType == TaskTypes.UploadData)
                        normalPlayerTask.taskStep = 1;
                    if ((normalPlayerTask.TaskType == TaskTypes.EmptyGarbage || normalPlayerTask.TaskType == TaskTypes.EmptyChute)
                        && (GameOptionsManager.Instance.currentNormalGameOptions.MapId == 0 ||
                        GameOptionsManager.Instance.currentNormalGameOptions.MapId == 3 ||
                        GameOptionsManager.Instance.currentNormalGameOptions.MapId == 4))
                        normalPlayerTask.taskStep = 1;
                    if (updateArrow)
                        normalPlayerTask.UpdateArrowAndLocation();

                    var taskInfo = player.Data.FindTaskById(task.Id);
                    taskInfo.Complete = false;
                }
        }

        public static void DestroyAll(this IEnumerable<Component> listie)
        {
            foreach (var item in listie)
            {
                if (item == null) continue;
                Object.Destroy(item);
                if (item.gameObject == null) return;
                Object.Destroy(item.gameObject);
            }
        }
        public static IEnumerable<GameObject> GetAllChilds(this GameObject Go)
        {
            for (var i = 0; i < Go.transform.childCount; i++)
            {
                yield return Go.transform.GetChild(i).gameObject;
            }
        }

        public static void Rpc(params object[] data)
        {
            if (data[0] is not CustomRPC) throw new ArgumentException($"first parameter must be a {typeof(CustomRPC).FullName}");

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                        (byte)(CustomRPC)data[0], SendOption.Reliable, -1);

            if (data.Length == 1)
            {
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                return;
            }

            foreach (var item in data[1..])
            {

                if (item is bool boolean)
                {
                    writer.Write(boolean);
                }
                else if (item is int integer)
                {
                    writer.Write(integer);
                }
                else if (item is uint uinteger)
                {
                    writer.Write(uinteger);
                }
                else if (item is float Float)
                {
                    writer.Write(Float);
                }
                else if (item is byte Byte)
                {
                    writer.Write(Byte);
                }
                else if (item is sbyte sByte)
                {
                    writer.Write(sByte);
                }
                else if (item is Vector2 vector)
                {
                    writer.Write(vector);
                }
                else if (item is Vector3 vector3)
                {
                    writer.Write(vector3);
                }
                else if (item is string str)
                {
                    writer.Write(str);
                }
                else if (item is byte[] array)
                {
                    writer.WriteBytesAndSize(array);
                }
                else
                {
                    Logger<TownOfSushi>.Error($"unknown data type entered for rpc write: item - {nameof(item)}, {item.GetType().FullName}, rpc - {data[0]}");
                }
            }
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }      

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
        class MeetingHudUpdatePatch {
            static void Postfix(MeetingHud __instance) {
                // Deactivate skip Button if skipping on emergency meetings is disabled 
                if ((voteTarget == null && CustomGameOptions.SkipButtonDisable == DisableSkipButtonMeetings.Emergency) || (CustomGameOptions.SkipButtonDisable == DisableSkipButtonMeetings.Always)) {
                    __instance.SkipVoteButton.gameObject.SetActive(false);
                }
            }
        }

        public static string AlignmentText(this PlayerControl player)
        {
            if (player == null) return "";
            var name = "";

            if (player.Is(RoleAlignment.CrewInvest)) name = "<color=#8BFDFDFF>Crew</color> (<color=#1D7CF2FF>Investigative</color>)";
            else if (player.Is(RoleAlignment.CrewKilling)) name = "<color=#8BFDFDFF>Crew</color> (<color=#1D7CF2FF>Killing</color>)";
            else if (player.Is(RoleAlignment.CrewSupport)) name = "<color=#8BFDFDFF>Crew</color> (<color=#1D7CF2FF>Support</color>)";
            else if (player.Is(RoleAlignment.CrewSpecial)) name = "<color=#8BFDFDFF>Crew</color> (<color=#1D7CF2FF>Special</color>)";
            else if (player.Is(RoleAlignment.NeutralBenign)) name = "<color=#B3B3B3FF>Neutral</color> (<color=#1D7CF2FF>Benign</color>)";
            else if (player.Is(RoleAlignment.NeutralEvil)) name = "<color=#B3B3B3FF>Neutral</color> (<color=#1D7CF2FF>Evil</color>)";
            else if (player.Is(RoleAlignment.NeutralKilling)) name = "<color=#B3B3B3FF>Neutral</color> (<color=#1D7CF2FF>Killing</color>)";
            else if (player.Is(RoleAlignment.ImpDeception)) name = "<color=#FF0000FF>Imp</color> (<color=#1D7CF2FF>Deception</color>)";
            else if (player.Is(RoleAlignment.ImpPower)) name = "<color=#FF0000FF>Imp</color> (<color=#1D7CF2FF>Power</color>)";
            else if (player.Is(RoleAlignment.ImpSpecial)) name = "<color=#FF0000FF>Imp</color> (<color=#1D7CF2FF>Special</color>)";
            else if (player.Is(RoleAlignment.ImpSupport)) name = "<color=#FF0000FF>Imp</color> (<color=#1D7CF2FF>Support</color>)";

            return name;
        }

        public static PlayerControl PlayerByName(string name)
        {
            string lowercaseName = name.ToLower();
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                if (player.Data.PlayerName.ToLower() == lowercaseName)
                    return player;
            return null;
        }
        
        public static string DeathReason(this PlayerControl player)
        {
            if (player == null)
                return "";
            
            var role = GetPlayerRole(player);

            if (role == null)
                return " ERROR";

            var reason = "";
            var killedBy = "";
            var result = "";

            if (role.DeathReason == DeathReasonEnum.Killed)
                reason = "Killed";
            else if (role.DeathReason == DeathReasonEnum.Ejected)
                reason = "Ejected";
            else if (role.DeathReason == DeathReasonEnum.Guessed)
                reason = "Guessed";
            else if (role.DeathReason == DeathReasonEnum.Alive)
                reason = "Alive";
            else if (role.DeathReason == DeathReasonEnum.Executed)
                reason = "Executed";
            else if (role.DeathReason == DeathReasonEnum.Cursed)
                reason = "Cursed";
            else if (role.DeathReason == DeathReasonEnum.Suicide)
                reason = "Suicide";
            
            if (role.DeathReason != DeathReasonEnum.Alive  && role.DeathReason != DeathReasonEnum.Ejected && role.DeathReason != DeathReasonEnum.Suicide)
                killedBy = role.KilledBy;
            
            result = reason + killedBy;

            return result;
        }

        public static void AddToRoleOutro(EndGameManager instance, RoleEnum roleType, string winText, Func<Role, bool> winCondition)
        {
            var role = AllRoles.FirstOrDefault(x => x.RoleType == roleType && winCondition(x));
            if (role == null) return;

            PoolablePlayer[] array = Object.FindObjectsOfType<PoolablePlayer>();
            foreach (var player in array)
            {
                player.NameText().text = role.ColorString + player.NameText().text + "</color>";
            }
            instance.BackgroundBar.material.color = role.Color;
            var text = Object.Instantiate(instance.WinText);
            text.text = winText;
            text.color = role.Color;
            var pos = instance.WinText.transform.localPosition;
            pos.y = 1.5f;
            text.transform.position = pos;
            text.text = $"<size=4>{text.text}</size>";
        }

        public static void EndGame(GameOverReason reason = GameOverReason.ImpostorByVote, bool showAds = false)
        {
            GameManager.Instance.RpcEndGame(reason, showAds);
        }

        public static void EndGameNoWinners(GameOverReason reason = GameOverReason.ImpostorDisconnect, bool showAds = false)
        {
            GameManager.Instance.RpcEndGame(reason, showAds);
        }

        public static void EndGameCrew(GameOverReason reason = GameOverReason.HumansByTask, bool showAds = false)
        {
            GameManager.Instance.RpcEndGame(reason, showAds);
        }

        public static bool IsKillingRole(this PlayerControl player)
        {
            if ( player.Is(Faction.Impostors) || player.Is(RoleAlignment.NeutralKilling)) return true;
            return false;
        }

        public static bool IsCrewKiller(this PlayerControl player)
        {
            if ( player.Is(RoleEnum.Swapper) || player.Is(RoleEnum.Vigilante)) return true;
            else if (player.Is(RoleEnum.Hunter))
            {
                var hunter = GetRole<Hunter>(player);
                if (hunter.MaxUses > 0 || (hunter.StalkedPlayer != null && !hunter.StalkedPlayer.Data.IsDead && !hunter.StalkedPlayer.Data.Disconnected && hunter.StalkedPlayer.Is(RoleAlignment.NeutralKilling)) ||
                hunter.CaughtPlayers.Count(player => !player.Data.IsDead && !player.Data.Disconnected && player.Is(RoleAlignment.NeutralKilling)) > 0) return true;
            }
            else if (player.Is(RoleEnum.Imitator))
            {
                if (PlayerControl.AllPlayerControls.ToArray().Count(x => x.Data.IsDead && !x.Data.Disconnected &&
                (x.Is(RoleEnum.Hunter) || x.Is(RoleEnum.Vigilante) || x.Is(RoleEnum.Veteran))) > 0) return true;
            }
            else if (player.Is(RoleEnum.Jailor))
            {
                var jailor = GetRole<Jailor>(player);
                if (jailor.Executes > 0) return true;
            }
            else if (player.Is(RoleEnum.Veteran))
            {
                var vet = GetRole<Veteran>(player);
                if (vet.UsesLeft > 0 || vet.Enabled) return true;
            }
            return false;
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

        #region Submerged Utils

        public static object TryCast(this Il2CppObjectBase self, Type type)
        {
            return AccessTools.Method(self.GetType(), nameof(Il2CppObjectBase.TryCast)).MakeGenericMethod(type).Invoke(self, Array.Empty<object>());
        }
        public static IList createList(Type myType)
        {
            Type genericListType = typeof(List<>).MakeGenericType(myType);
            return (IList)Activator.CreateInstance(genericListType);
        }

        public static void ResetCustomTimers()
        {
            #region CrewmateRoles
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Medium))
            {
                var medium = GetRole<Medium>(PlayerControl.LocalPlayer);
                medium.LastMediated = DateTime.UtcNow;
            }
            foreach (var role in GetRoles(RoleEnum.Medium))
            {
                var medium = (Medium)role;
                medium.MediatedPlayers.Values.DestroyAll();
                medium.MediatedPlayers.Clear();
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Seer))
            {
                var seer = GetRole<Seer>(PlayerControl.LocalPlayer);
                seer.LastInvestigated = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Oracle))
            {
                var oracle = GetRole<Oracle>(PlayerControl.LocalPlayer);
                oracle.LastConfessed = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Hunter))
            {
                var hunter = Role.GetRole<Hunter>(PlayerControl.LocalPlayer);
                hunter.LastKilled = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Vigilante))
            {
                var vigilante = GetRole<Vigilante>(PlayerControl.LocalPlayer);
                vigilante.LastKilled = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Tracker))
            {
                var tracker = GetRole<Tracker>(PlayerControl.LocalPlayer);
                tracker.LastTracked = DateTime.UtcNow;
                tracker.MaxUses = CustomGameOptions.MaxTracks;
                if (CustomGameOptions.ResetOnNewRound)
                {
                    tracker.TrackerArrows.Values.DestroyAll();
                    tracker.TrackerArrows.Clear();
                }
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Transporter))
            {
                var transporter = GetRole<Transporter>(PlayerControl.LocalPlayer);
                transporter.LastTransported = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Veteran))
            {
                var veteran = GetRole<Veteran>(PlayerControl.LocalPlayer);
                veteran.LastAlerted = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(ModifierEnum.Disperser))
            {
                var veteran = GetModifier<Disperser>(PlayerControl.LocalPlayer);
                veteran.LastDispersed = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Jailor))
            {
                var jailor = GetRole<Jailor>(PlayerControl.LocalPlayer);
                jailor.LastJailed = DateTime.UtcNow;
            }
            foreach (var role in GetRoles(RoleEnum.Jailor))
            {
                var jailor = (Jailor)role;
                jailor.Jailed = null;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Trapper))
            {
                var trapper = GetRole<Trapper>(PlayerControl.LocalPlayer);
                trapper.LastTrapped = DateTime.UtcNow;
                trapper.trappedPlayers.Clear();
                if (CustomGameOptions.TrapsRemoveOnNewRound) trapper.traps.ClearTraps();
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Investigator))
            {
                var detective = GetRole<Investigator>(PlayerControl.LocalPlayer);
                detective.LastExamined = DateTime.UtcNow;
                if (detective.DetectedKiller != null && (detective.DetectedKiller.Data.IsDead || detective.DetectedKiller.Data.Disconnected))
                {
                    detective.DetectedKiller = null;
                    detective.ExamineMode = false;
                    detective.ClosestPlayer = null;
                }
                detective.CurrentTarget = null;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Mystic))
            {
                var Mystic = GetRole<Mystic>(PlayerControl.LocalPlayer);
                Mystic.LastExamined = DateTime.UtcNow;
                Mystic.LastExamined = Mystic.LastExamined.AddSeconds(CustomGameOptions.InitialExamineCd - CustomGameOptions.MysticExamineCd);
                Mystic.LastExaminedPlayer = null;
            }
            #endregion
            #region NeutralRoles
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Romantic))
            {
                var surv = GetRole<Romantic>(PlayerControl.LocalPlayer);
                surv.LastPick = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.GuardianAngel))
            {
                var ga = GetRole<GuardianAngel>(PlayerControl.LocalPlayer);
                ga.LastProtected = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Arsonist))
            {
                var arsonist = GetRole<Arsonist>(PlayerControl.LocalPlayer);
                arsonist.LastDoused = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Vulture))
            {
                var vulture = GetRole<Vulture>(PlayerControl.LocalPlayer);
                vulture.LastEaten = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Glitch))
            {
                var glitch = GetRole<Glitch>(PlayerControl.LocalPlayer);
                glitch.LastKill = DateTime.UtcNow;
                glitch.LastHack = DateTime.UtcNow;
                glitch.LastMimic = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Juggernaut))
            {
                var juggernaut = GetRole<Juggernaut>(PlayerControl.LocalPlayer);
                juggernaut.LastKill = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.SerialKiller))
            {
                var werewolf = GetRole<SerialKiller>(PlayerControl.LocalPlayer);
                werewolf.LastStabbed = DateTime.UtcNow;
                werewolf.LastKilled = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Werewolf))
            {
                var werewolf = GetRole<Werewolf>(PlayerControl.LocalPlayer);
                werewolf.LastMauled = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Vampire))
            {
                var Vampire = GetRole<Vampire>(PlayerControl.LocalPlayer);
                Vampire.LastBit = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Hitman))
            {
                var hitman = GetRole<Hitman>(PlayerControl.LocalPlayer);
                hitman.LastDrag = DateTime.UtcNow;
                hitman.LastMorph = DateTime.UtcNow;
                hitman.LastKill = DateTime.UtcNow;
                hitman.DragDropButtonHitman.graphic.sprite = TownOfSushi.DragSprite;
                hitman.MorphButton.graphic.sprite = TownOfSushi.SampleSprite;
                hitman.CurrentlyDragging = null;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Plaguebearer))
            {
                var plaguebearer = GetRole<Plaguebearer>(PlayerControl.LocalPlayer);
                plaguebearer.LastInfected = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Pestilence))
            {
                var pest = GetRole<Pestilence>(PlayerControl.LocalPlayer);
                pest.LastKill = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Doomsayer))
            {
                var doom = GetRole<Doomsayer>(PlayerControl.LocalPlayer);
                doom.LastObserved = DateTime.UtcNow;
                doom.LastObservedPlayer = null;
            }
            #endregion
            #region ImposterRoles
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Escapist))
            {
                var escapist = GetRole<Escapist>(PlayerControl.LocalPlayer);
                escapist.LastEscape = DateTime.UtcNow;
                escapist.EscapeButton.graphic.sprite = TownOfSushi.MarkSprite;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Blackmailer))
            {
                var blackmailer = GetRole<Blackmailer>(PlayerControl.LocalPlayer);
                blackmailer.LastBlackmailed = DateTime.UtcNow;
                if (blackmailer.Player.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                {
                    blackmailer.Blackmailed?.myRend().material.SetFloat("_Outline", 0f);
                }
            }
            foreach (var role in GetRoles(RoleEnum.Blackmailer))
            {
                var blackmailer = (Blackmailer)role;
                blackmailer.Blackmailed = null;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Bomber))
            {
                var bomber = GetRole<Bomber>(PlayerControl.LocalPlayer);
                bomber.PlantButton.graphic.sprite = TownOfSushi.PlantSprite;
                bomber.Bomb.ClearBomb();
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Grenadier))
            {
                var grenadier = GetRole<Grenadier>(PlayerControl.LocalPlayer);
                grenadier.LastFlashed = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Miner))
            {
                var miner = GetRole<Miner>(PlayerControl.LocalPlayer);
                miner.LastMined = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Witch))
            {
                var Witch = GetRole<Witch>(PlayerControl.LocalPlayer);
                Witch.LastSpelled = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Morphling))
            {
                var morphling = GetRole<Morphling>(PlayerControl.LocalPlayer);
                morphling.LastMorphed = DateTime.UtcNow;
                morphling.MorphButton.graphic.sprite = TownOfSushi.SampleSprite;
                morphling.SampledPlayer = null;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Swooper))
            {
                var swooper = GetRole<Swooper>(PlayerControl.LocalPlayer);
                swooper.LastSwooped = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Venerer))
            {
                var venerer = GetRole<Venerer>(PlayerControl.LocalPlayer);
                venerer.LastCamouflaged = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Undertaker))
            {
                var undertaker = GetRole<Undertaker>(PlayerControl.LocalPlayer);
                undertaker.LastDragged = DateTime.UtcNow;
                undertaker.DragDropButton.graphic.sprite = TownOfSushi.DragSprite;
                undertaker.CurrentlyDragging = null;
            }
            #endregion
        }

        #endregion

        public static string ColorString(Color c, string s) 
        {
            return string.Format("<color=#{0:X2}{1:X2}{2:X2}{3:X2}>{4}</color>", ToByte(c.r), ToByte(c.g), ToByte(c.b), ToByte(c.a), s);
        }

        private static byte ToByte(float f) {
            f = Mathf.Clamp01(f);
            return (byte)(f * 255);
        }

        public static Dictionary<string, Sprite> CachedSprites = new();
        public static Sprite LoadSpriteFromResources(string path, float pixelsPerUnit, bool cache=true) 
        {
            try
            {
                if (cache && CachedSprites.TryGetValue(path + pixelsPerUnit, out var sprite)) return sprite;
                Texture2D texture = LoadTextureFromResources(path);
                sprite = Sprite.Create(texture, new UnityEngine.Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), pixelsPerUnit);
                if (cache) sprite.hideFlags |= HideFlags.HideAndDontSave | HideFlags.DontSaveInEditor;
                if (!cache) return sprite;
                return CachedSprites[path + pixelsPerUnit] = sprite;
            } catch {
                TownOfSushi.Logger.LogError("Error loading sprite from path: " + path);
            }
            return null;
        }
        
        //Code from TOH-Enhanced
        public static string GetRegionName(IRegionInfo region = null)
        {
            region ??= ServerManager.Instance.CurrentRegion;
            string name = region.Name;
            if (AmongUsClient.Instance.NetworkMode != NetworkModes.OnlineGame)
            {
                name = "Local Game";
                return name;
            }
            
            if (region.PingServer.EndsWith("among.us", StringComparison.Ordinal))
            {
                // Official server
                if (name == "North America") name = "Vanilla North America";
                else if (name == "Europe") name = "Vanilla Europe";
                else if (name == "Asia") name = "Vanilla Asia";
                
                return name;
            }
            var Ip = region.Servers.FirstOrDefault()?.Ip ?? string.Empty;
            if (Ip.Contains("aumods.us", StringComparison.Ordinal)
            || Ip.Contains("duikbo.at", StringComparison.Ordinal))
            {
                if (Ip.Contains("au-eu")) name = "Modded Europe (MEU)";
                else if (Ip.Contains("au-as")) name = "Modded Asia (MAS)"; 
                else if (Ip.Contains("www.")) name = "Modded North America (MNA)";
                return name;
            }
            return name;
        }

        public static unsafe Texture2D LoadTextureFromResources(string path) 
        {
            try {
                Texture2D texture = new Texture2D(2, 2, TextureFormat.ARGB32, true);
                Assembly assembly = Assembly.GetExecutingAssembly();
                Stream stream = assembly.GetManifestResourceStream(path);
                var length = stream.Length;
                var byteTexture = new Il2CppStructArray<byte>(length);
                stream.Read(new Span<byte>(IntPtr.Add(byteTexture.Pointer, IntPtr.Size * 4).ToPointer(), (int) length));
                ImageConversion.LoadImage(texture, byteTexture, false);
                return texture;
            } catch {
                TownOfSushi.Logger.LogError("Error loading texture from resources: " + path);
            }
            return null;
        }

        public static Texture2D loadTextureFromDisk(string path) {
            try {          
                if (File.Exists(path))     {
                    Texture2D texture = new Texture2D(2, 2, TextureFormat.ARGB32, true);
                    var byteTexture = Il2CppSystem.IO.File.ReadAllBytes(path);
                    ImageConversion.LoadImage(texture, byteTexture, false);
                    return texture;
                }
            } catch {
                TownOfSushi.Logger.LogError("Error loading texture from disk: " + path);
            }
            return null;
        }

        public static bool IsTooFar(PlayerControl player, PlayerControl target)
        {
            if (player == null || target == null || (player == null && target == null))
                return true;

            var maxDistance = KillDistance();
            return (GetDistBetweenPlayers(player, target) > maxDistance);
        }
    }
}
