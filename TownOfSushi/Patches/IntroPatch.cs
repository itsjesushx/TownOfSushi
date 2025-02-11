using HarmonyLib;
using System;
using static TownOfSushi.TownOfSushi;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Hazel;
using TownOfSushi.Utilities;

namespace TownOfSushi.Patches 
{
    [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.OnDestroy))]
    class IntroCutsceneOnDestroyPatch
    {
        public static PoolablePlayer playerPrefab;
        public static Vector3 bottomLeft;
        public static void Prefix(IntroCutscene __instance) 
        {
            // Generate and initialize player icons
            int playerCounter = 0;
            if (PlayerControl.LocalPlayer != null && FastDestroyableSingleton<HudManager>.Instance != null) 
            {
                float aspect = Camera.main.aspect;
                float safeOrthographicSize = CameraSafeArea.GetSafeOrthographicSize(Camera.main);
                float xpos = 1.75f - safeOrthographicSize * aspect * 1.70f;
                float ypos = 0.15f - safeOrthographicSize * 1.7f;
                bottomLeft = new Vector3(xpos / 2, ypos/2, -61f);

                foreach (PlayerControl p in PlayerControl.AllPlayerControls) 
                {
                    NetworkedPlayerInfo data = p.Data;
                    PoolablePlayer player = UnityEngine.Object.Instantiate<PoolablePlayer>(__instance.PlayerPrefab, FastDestroyableSingleton<HudManager>.Instance.transform);
                    playerPrefab = __instance.PlayerPrefab;
                    p.SetPlayerMaterialColors(player.cosmetics.currentBodySprite.BodySprite);
                    player.SetSkin(data.DefaultOutfit.SkinId, data.DefaultOutfit.ColorId);
                    player.cosmetics.SetHat(data.DefaultOutfit.HatId, data.DefaultOutfit.ColorId);
                   // PlayerControl.SetPetImage(data.DefaultOutfit.PetId, data.DefaultOutfit.ColorId, player.PetSlot);
                    player.cosmetics.nameText.text = data.PlayerName;
                    player.SetFlipX(true);
                    MapOptions.playerIcons[p.PlayerId] = player;
                    player.gameObject.SetActive(false);

                    if (PlayerControl.LocalPlayer == Arsonist.arsonist && p != Arsonist.arsonist) 
                    {
                        player.transform.localPosition = bottomLeft + new Vector3(-0.25f, -0.25f, 0) + Vector3.right * playerCounter++ * 0.35f;
                        player.transform.localScale = Vector3.one * 0.2f;
                        player.setSemiTransparent(true);
                        player.gameObject.SetActive(true);
                    }
                    else 
                    {   //  This can be done for all players not just for the bounty hunter as it was before. Allows the thief to have the correct position and scaling
                        player.transform.localPosition = bottomLeft;
                        player.transform.localScale = Vector3.one * 0.4f;
                        player.gameObject.SetActive(false);
                    }
                }
            }

            // Force Bounty Hunter to load a new Bounty when the Intro is over
            if (BountyHunter.bounty != null && PlayerControl.LocalPlayer == BountyHunter.bountyHunter) 
            {
                BountyHunter.bountyUpdateTimer = 0f;
                if (FastDestroyableSingleton<HudManager>.Instance != null) 
                {
                    BountyHunter.cooldownText = UnityEngine.Object.Instantiate<TMPro.TextMeshPro>(FastDestroyableSingleton<HudManager>.Instance.KillButton.cooldownTimerText, FastDestroyableSingleton<HudManager>.Instance.transform);
                    BountyHunter.cooldownText.alignment = TMPro.TextAlignmentOptions.Center;
                    BountyHunter.cooldownText.transform.localPosition = bottomLeft + new Vector3(0f, -0.35f, -62f);
                    BountyHunter.cooldownText.transform.localScale = Vector3.one * 0.4f;
                    BountyHunter.cooldownText.gameObject.SetActive(true);
                }
            }

            // Force Reload of SoundEffectHolder
            SoundEffectsManager.Load();

            // First kill
            if (AmongUsClient.Instance.AmHost && MapOptions.shieldFirstKill && MapOptions.firstKillName != "") 
            {
                PlayerControl target = PlayerControl.AllPlayerControls.ToArray().ToList().FirstOrDefault(x => x.Data.PlayerName.Equals(MapOptions.firstKillName));
                if (target != null) {
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetFirstKill, Hazel.SendOption.Reliable, -1);
                    writer.Write(target.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.SetFirstKill(target.PlayerId);
                }
            }
            MapOptions.firstKillName = "";

            EventUtility.gameStartsUpdate();
        }
    }

    [HarmonyPatch]
    class IntroPatch 
    {
        public static void SetupIntroTeamIcons(IntroCutscene __instance, ref  Il2CppSystem.Collections.Generic.List<PlayerControl> yourTeam) {
            // Intro solo teams
            if (PlayerControl.LocalPlayer.IsNeutral() || PlayerControl.LocalPlayer.IsNeutralKiller()) 
            {
                var soloTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
                soloTeam.Add(PlayerControl.LocalPlayer);
                yourTeam = soloTeam;
            }

            // Add the Spy to the Impostor team (for the Impostors)
            if (Spy.spy != null && PlayerControl.LocalPlayer.Data.Role.IsImpostor) {
                List<PlayerControl> players = PlayerControl.AllPlayerControls.ToArray().ToList().OrderBy(x => Guid.NewGuid()).ToList();
                var fakeImpostorTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>(); // The local player always has to be the first one in the list (to be displayed in the center)
                fakeImpostorTeam.Add(PlayerControl.LocalPlayer);
                foreach (PlayerControl p in players) {
                    if (PlayerControl.LocalPlayer != p && (p == Spy.spy || p.Data.Role.IsImpostor))
                        fakeImpostorTeam.Add(p);
                }
                yourTeam = fakeImpostorTeam;
            }
        }

        public static void SetupIntroTeam(IntroCutscene __instance, ref  Il2CppSystem.Collections.Generic.List<PlayerControl> yourTeam) {
            List<RoleInfo> infos = RoleInfo.GetRoleInfoForPlayer(PlayerControl.LocalPlayer);
            RoleInfo roleInfo = infos.Where(info => info.FactionId != Factions.Modifier).FirstOrDefault();
            if (roleInfo == null) return;
            if (roleInfo.FactionId == Factions.Neutral || roleInfo.FactionId == Factions.NeutralKiller) 
            {
                var neutralColor = new Color32(76, 84, 78, 255);
                __instance.BackgroundBar.material.color = neutralColor;
                __instance.TeamTitle.text = "Neutral";
                __instance.TeamTitle.color = neutralColor;
            }
        }

        public static IEnumerator<WaitForSeconds> EndShowRole(IntroCutscene __instance) {
            yield return new WaitForSeconds(5f);
            __instance.YouAreText.gameObject.SetActive(false);
            __instance.RoleText.gameObject.SetActive(false);
            __instance.RoleBlurbText.gameObject.SetActive(false);
            __instance.ourCrewmate.gameObject.SetActive(false);
           
        }

        [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.CreatePlayer))]
        class CreatePlayerPatch {
            public static void Postfix(IntroCutscene __instance, bool impostorPositioning, ref PoolablePlayer __result) {
                if (impostorPositioning) __result.SetNameColor(Palette.ImpostorRed);
            }
        }


        [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.ShowRole))]
        class SetUpRoleTextPatch 
        {
            static int seed = 0;
            static public void SetRoleTexts(IntroCutscene __instance) 
            {
                // Don't override the intro of the vanilla roles
                List<RoleInfo> infos = RoleInfo.GetRoleInfoForPlayer(PlayerControl.LocalPlayer);
                RoleInfo roleInfo = infos.Where(info => info.FactionId != Factions.Modifier).FirstOrDefault();
                RoleInfo modifierInfo = infos.Where(info => info.FactionId == Factions.Modifier).FirstOrDefault();

                if (EventUtility.isEnabled)
                {
                    var roleInfos = RoleInfo.allRoleInfos.Where(x => x.FactionId != Factions.Modifier).ToList();
                    if (roleInfo.FactionId == Factions.Neutral) roleInfos.RemoveAll(x => x.FactionId != Factions.Neutral);
                    
                    if (roleInfo.FactionId == Factions.NeutralKiller) roleInfos.RemoveAll(x => x.FactionId != Factions.NeutralKiller);
                   
                    if (roleInfo.color == Palette.ImpostorRed) roleInfos.RemoveAll(x => x.color != Palette.ImpostorRed);
                    
                    if (roleInfo.FactionId != Factions.Neutral && roleInfo.color != Palette.ImpostorRed) roleInfos.RemoveAll(x => x.color == Palette.ImpostorRed || x.FactionId == Factions.Neutral);

                    if (roleInfo.FactionId != Factions.NeutralKiller) roleInfos.RemoveAll(x => x.FactionId == Factions.NeutralKiller);
                    var rnd = new System.Random(seed);
                    roleInfo = roleInfos[rnd.Next(roleInfos.Count)];
                }

                __instance.RoleBlurbText.text = "";
                if (roleInfo != null) 
                {
                    __instance.RoleText.text = roleInfo.name;
                    __instance.YouAreText.color = roleInfo.color;
                    __instance.RoleText.color = roleInfo.color;
                    __instance.RoleBlurbText.text = roleInfo.introDescription;
                    __instance.RoleBlurbText.color = roleInfo.color;
                }
                if (modifierInfo != null) 
                {
                    if (modifierInfo.roleId != RoleId.Lover)
                        __instance.RoleBlurbText.text += Helpers.ColorString(modifierInfo.color, $"\n{modifierInfo.introDescription}");
                    
                    else 
                    {
                        PlayerControl otherLover = PlayerControl.LocalPlayer == Lovers.lover1 ? Lovers.lover2 : Lovers.lover1;
                        __instance.RoleBlurbText.text += Helpers.ColorString(Lovers.color, $"\n♥ You are in love with {otherLover?.Data?.PlayerName ?? ""} ♥");
                    }
                }
            }
            public static bool Prefix(IntroCutscene __instance) {
                seed = rnd.Next(5000);
                FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(1f, new Action<float>((p) => {
                    SetRoleTexts(__instance);
                })));
                return true;
            }
        }

        [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.BeginCrewmate))]
        class BeginCrewmatePatch {
            public static void Prefix(IntroCutscene __instance, ref  Il2CppSystem.Collections.Generic.List<PlayerControl> teamToDisplay) {
                SetupIntroTeamIcons(__instance, ref teamToDisplay);
            }

            public static void Postfix(IntroCutscene __instance, ref  Il2CppSystem.Collections.Generic.List<PlayerControl> teamToDisplay) {
                SetupIntroTeam(__instance, ref teamToDisplay);
            }
        }

        [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.BeginImpostor))]
        class BeginImpostorPatch {
            public static void Prefix(IntroCutscene __instance, ref  Il2CppSystem.Collections.Generic.List<PlayerControl> yourTeam) {
                SetupIntroTeamIcons(__instance, ref yourTeam);
            }

            public static void Postfix(IntroCutscene __instance, ref  Il2CppSystem.Collections.Generic.List<PlayerControl> yourTeam) {
                SetupIntroTeam(__instance, ref yourTeam);
            }
        }
    }

    /* Horses are broken since 2024.3.5 - keeping this code in case they return.
     * [HarmonyPatch(typeof(AprilFoolsMode), nameof(AprilFoolsMode.ShouldHorseAround))]
    public static class ShouldAlwaysHorseAround {
        public static bool Prefix(ref bool __result) {
            __result = EventUtility.isEnabled && !EventUtility.disableEventMode;
            return false;
        }
    }*/

    [HarmonyPatch(typeof(AprilFoolsMode), nameof(AprilFoolsMode.ShouldShowAprilFoolsToggle))]
    public static class ShouldShowAprilFoolsToggle {
        public static void Postfix(ref bool __result) {
            __result = __result || EventUtility.isEventDate || EventUtility.canBeEnabled;  // Extend it to a 7 day window instead of just 1st day of the Month
        }
    }
}

