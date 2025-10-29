﻿using InnerNet;
using RCoroutines = Reactor.Utilities.Coroutines;
using System.Collections;
using UnityEngine;
using HarmonyLib;
using MiraAPI.Modifiers.Types;

namespace TownOfSushi.Modules.Debugger;

public static class InstanceControlPatches
{
    internal static Dictionary<int, ClientData> Clients = new();
    internal static Dictionary<byte, int> PlayerClientIDs = new();
    public static PlayerControl CurrentPlayerInPower { get; private set; }
    public static int AvailableId()
    {
        for (var i = 1; i < 128; i++)
        {
            if (!AmongUsClient.Instance.allClients.ToArray().Any(x => x.Id == i) && !Clients.ContainsKey(i) && PlayerControl.LocalPlayer.OwnerId != i)
                return i;
        }

        return -1;
    }

    public static void SwitchTo(byte playerId)
    {
        var savedPlayerId = PlayerControl.LocalPlayer.PlayerId;
        PlayerControl savedPlayer = MiscUtils.PlayerById(savedPlayerId)!;
        var savedPosition = savedPlayer.transform.position;

        PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(PlayerControl.LocalPlayer.transform.position);
        PlayerControl.LocalPlayer.moveable = false;

        var light = PlayerControl.LocalPlayer.lightSource;

        //Setup new player
        var newPlayer =  MiscUtils.PlayerById(playerId);

        if (newPlayer == null) return;

        var newPosition = newPlayer.transform.position;

        PlayerControl.LocalPlayer = newPlayer;
        PlayerControl.LocalPlayer.lightSource = light;
        PlayerControl.LocalPlayer.moveable = true;

        AmongUsClient.Instance.ClientId = PlayerControl.LocalPlayer.OwnerId;
        AmongUsClient.Instance.HostId = PlayerControl.LocalPlayer.OwnerId;

        DestroyableSingleton<HudManager>.Instance.SetHudActive(true);
        DestroyableSingleton<HudManager>.Instance.ShadowQuad.gameObject.SetActive(!newPlayer.Data.IsDead);

        DestroyableSingleton<HudManager>.Instance.KillButton.transform.parent.GetComponentsInChildren<Transform>().ToList().ForEach(x =>
        {
            if (x.gameObject.name == "KillButton(Clone)")
            {
                UnityEngine.Object.Destroy(x.gameObject);
            }

        });

        DestroyableSingleton<HudManager>.Instance.KillButton.transform.GetComponentsInChildren<Transform>().ToList().ForEach(x =>
        {
            if (x.gameObject.name == "KillTimer_TMP(Clone)")
            {
                UnityEngine.Object.Destroy(x.gameObject);
            }

        });

        DestroyableSingleton<HudManager>.Instance.transform.GetComponentsInChildren<Transform>().ToList().ForEach(x =>
        {
            if (x.gameObject.name == "KillButton(Clone)")
            {
                UnityEngine.Object.Destroy(x.gameObject);
            }

        });

        var modsTab = MiraAPI.Modifiers.ModifierDisplay.ModifierDisplayComponent.Instance;
        if (modsTab != null && !modsTab.IsOpen && PlayerControl.LocalPlayer.GetModifiers<GameModifier>().Any(x => !x.HideOnUi && x.GetDescription() != string.Empty))
        {
            modsTab.ToggleTab();
        }

        light.transform.SetParent(newPlayer.transform);
        light.transform.localPosition = newPlayer.Collider.offset;
        Camera.main.GetComponent<FollowerCamera>().SetTarget(newPlayer);
        newPlayer.MyPhysics.ResetMoveState(true);
        KillAnimation.SetMovement(newPlayer, true);
        newPlayer.MyPhysics.inputHandler.enabled = true;
        CurrentPlayerInPower = newPlayer;

        newPlayer.NetTransform.SnapTo(newPosition);
        savedPlayer.NetTransform.SnapTo(savedPosition);

        if (MeetingHud.Instance)
        {
            if (newPlayer.Data.IsDead)
                MeetingHud.Instance.SetForegroundForDead();
            else
                MeetingHud.Instance.SetForegroundForAlive();
        }
    }

    public static void CleanUpLoad()
    {
        if (GameData.Instance.AllPlayers.Count == 1)
        {
            Clients.Clear();
            PlayerClientIDs.Clear();
        }
    }

    public static void CreatePlayerInstance()
    {
        RCoroutines.Start(_CreatePlayerInstanceEnumerator());
    }

    internal static IEnumerator _CreatePlayerInstanceEnumerator()
    {
        var sampleId = AvailableId();
        var sampleC = new ClientData(sampleId, $"Bot-{sampleId}", new()
        {
            Platform = Platforms.StandaloneWin10,
            PlatformName = "Bot"
        }, 1, "", "robotmodeactivate");

        AmongUsClient.Instance.GetOrCreateClient(sampleC);
        yield return AmongUsClient.Instance.CreatePlayer(sampleC);

        sampleC.Character.SetName($"Bot #{sampleC.Character.PlayerId}");
        sampleC.Character.SetSkin(HatManager.Instance.allSkins[UnityEngine.Random.Range(0, HatManager.Instance.allSkins.Count)].ProdId, 0);
        sampleC.Character.SetNamePlate(HatManager.Instance.allNamePlates[UnityEngine.Random.RandomRangeInt(0, HatManager.Instance.allNamePlates.Count)].ProdId);
        sampleC.Character.SetPet(HatManager.Instance.allPets[UnityEngine.Random.RandomRangeInt(0, HatManager.Instance.allPets.Count)].ProdId);
        sampleC.Character.SetColor(UnityEngine.Random.Range(0, Palette.PlayerColors.Length));
        sampleC.Character.SetHat(HatManager.Instance.allHats[UnityEngine.Random.Range(0, HatManager.Instance.allHats.Count)].ProdId, 0);
        sampleC.Character.SetVisor(HatManager.Instance.allVisors[UnityEngine.Random.Range(0, HatManager.Instance.allVisors.Count)].ProdId, 0);

        Clients.Add(sampleId, sampleC);
        PlayerClientIDs.Add(sampleC.Character.PlayerId, sampleId);
        sampleC.Character.MyPhysics.ResetAnimState();
        sampleC.Character.MyPhysics.ResetMoveState();

        yield return sampleC.Character.MyPhysics.CoSpawnPlayer(LobbyBehaviour.Instance);
        yield break;
    }
    public static void RemovePlayer(byte id)
    {
        if (id == 0)
            return;

        var clientId = Clients.FirstOrDefault(x => x.Value.Character.PlayerId == id).Key;
        Clients.Remove(clientId, out var outputData);
        PlayerClientIDs.Remove(id);
        AmongUsClient.Instance.RemovePlayer(clientId, DisconnectReasons.Custom);
        AmongUsClient.Instance.allClients.Remove(outputData);
    }

    public static void RemoveAllPlayers()
    {
        PlayerClientIDs.Keys.ToList().ForEach(RemovePlayer);
        SwitchTo(0);
        Keyboard_Joystick.ControllingFigure = 0;
    }

    public static void SetForegroundForAlive(this MeetingHud __instance)
    {
        __instance.amDead = false;
        __instance.SkipVoteButton.gameObject.SetActive(true);
        __instance.SkipVoteButton.AmDead = false;
        __instance.Glass.gameObject.SetActive(false);
        if (CacheMeetingSprite.Cache) __instance.Glass.sprite = CacheMeetingSprite.Cache;
    }
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.SetForegroundForDead))]
    public static class CacheMeetingSprite
    {
        public static Sprite Cache;
        public static void Prefix(MeetingHud __instance) => Cache ??= __instance.Glass.sprite;
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Confirm))]
    public static class SameVoteAll
    {
        public static void Postfix(MeetingHud __instance, ref byte suspectStateIdx)
        {
            if (!Components.Debugger.IsDebuggerActive) return;

            foreach (var player in PlayerControl.AllPlayerControls)
                __instance.CmdCastVote(player.PlayerId, suspectStateIdx);
        }
    }

    [HarmonyPatch(typeof(LobbyBehaviour), nameof(LobbyBehaviour.Start))]
    public static class OnLobbyStart
    {
        public static void Postfix()
        {
            if (Components.Debugger.IsDebuggerActive && TownOfSushiPlugin.Persistence && Clients.Count != 0)
            {
                var count = Clients.Count;
                Clients.Clear();
                PlayerClientIDs.Clear();

                for (var i = 0; i < count; i++)
                    CreatePlayerInstance();
            }
        }
    }

    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.CoStartGameHost))]
    public static class OnGameStart
    {
        public static void Prefix(AmongUsClient __instance)
        {
            if (!Components.Debugger.IsDebuggerActive) return;

            foreach (var p in __instance.allClients)
            {
                p.IsReady = true;
                p.Character.gameObject.GetComponent<DummyBehaviour>().enabled = false;
            }
        }
    }

    [HarmonyPatch(typeof(SpawnInMinigame), nameof(SpawnInMinigame.Begin))]
    public static class AirshipSpawn
    {
        public static void Postfix(SpawnInMinigame __instance)
        {
            if (!Components.Debugger.IsDebuggerActive) return;

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (!player.Data.PlayerName.Contains(TownOfSushiPlugin.RobotName)) continue;

                var rand = UnityEngine.Random.Range(0, __instance.Locations.Count);
                player.gameObject.SetActive(true);
                player.NetTransform.RpcSnapTo(__instance.Locations[rand].Location);
            }
        }
    }

    [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Update))]
    public static class CountdownPatch
    {
        public static void Prefix(GameStartManager __instance) 
        {
            if (!Input.GetKeyDown(KeyCode.LeftShift)) return;
        
            __instance.countDownTimer = 0;
            if (Embedded.ReactorCoroutines.Coroutines._ourCoroutineStore.Any(x => x.Value != null)) __instance.countDownTimer = 1;
        }
    }
}
