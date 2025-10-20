using HarmonyLib;
using UnityEngine;

namespace TownOfSushi.Patches.Roles;

[HarmonyPatch]
public static class AdministratorMapCountOverlayPatch
{
    public static AdminDeadPlayers WhoSeesDead => OptionGroupSingleton<AdministratorOptions>.Instance.WhoSeesDead;

    public static void SetSabotaged(MapCountOverlay __instance, bool sabotaged)
    {
        __instance.isSab = sabotaged;
        __instance.BackgroundColor.SetColor(sabotaged ? Palette.DisabledGrey : Color.green);
        __instance.SabotageText.gameObject.SetActive(sabotaged);

        if (sabotaged)
        {
            foreach (var area in __instance.CountAreas)
            {
                area.UpdateCount(0);
            }
        }
    }

    public static void UpdateBlips(CounterArea area, List<int> colorMapping, bool isAdministrator)
    {
        area.UpdateCount(colorMapping.Count);
        var icons = area.myIcons.ToArray();
        colorMapping.Sort();

        for (var i = 0; i < colorMapping.Count; i++)
        {
            var icon = icons[i];
            var sprite = icon.GetComponent<SpriteRenderer>();

            // if (Modules.ModCompatibility.SubLoaded) sprite.color = new Color(1, 1, 1, 1);
            if (sprite != null)
            {
                if (isAdministrator)
                {
                    PlayerMaterial.SetColors(colorMapping[i], sprite);
                }
                else
                {
                    PlayerMaterial.SetColors(new Color(0.8793f, 1, 0, 1), sprite);
                }
            }
        }
    }

    public static void UpdateBlips(MapCountOverlay __instance, bool isAdministrator)
    {
        var rooms = ShipStatus.Instance.FastRooms;
        var colorMapDuplicate = new List<int>();

        foreach (var area in __instance.CountAreas)
        {
            if (!rooms.ContainsKey(area.RoomType))
            {
                continue;
            }

            var room = rooms[area.RoomType];
            if (room.roomArea == null)
            {
                continue;
            }

            var objectsInRoom = room.roomArea.OverlapCollider(__instance.filter, __instance.buffer);
            var colorMap = new List<int>();

            for (var i = 0; i < objectsInRoom; i++)
            {
                var collider = __instance.buffer[i];

                if (collider.tag == "DeadBody" &&
                    ((isAdministrator && WhoSeesDead == AdminDeadPlayers.Administrator) ||
                     (!isAdministrator && WhoSeesDead == AdminDeadPlayers.EveryoneButAdministrator) ||
                     WhoSeesDead == AdminDeadPlayers.Everyone))
                {
                    var playerId = collider.GetComponent<DeadBody>().ParentId;
                    colorMap.Add(GameData.Instance.GetPlayerById(playerId).DefaultOutfit.ColorId);
                    colorMapDuplicate.Add(GameData.Instance.GetPlayerById(playerId).DefaultOutfit.ColorId);
                }
                else
                {
                    var component = collider.GetComponent<PlayerControl>();
                    var data = component?.Data;

                    if (component == null || data == null || !component || component.Data == null ||
                        component.Data.Disconnected || component.Data.IsDead
                        || (!__instance.showLivePlayerPosition && component.AmOwner) ||
                        colorMapDuplicate.Contains(data.DefaultOutfit.ColorId))
                    {
                        continue;
                    }

                    colorMap.Add(data.DefaultOutfit.ColorId);
                    colorMapDuplicate.Add(data.DefaultOutfit.ColorId);
                }
            }

            UpdateBlips(area, colorMap, isAdministrator);
        }
    }

    [HarmonyPatch(typeof(MapCountOverlay), nameof(MapCountOverlay.Update))]
    [HarmonyPrefix]
    public static bool MapCountOverlayUpdatePatch(MapCountOverlay __instance)
    {
        var localPlayer = PlayerControl.LocalPlayer;
        var isAdministrator = localPlayer.HasModifier<AdministratorModifier>();

        __instance.timer += Time.deltaTime;
        if (__instance.timer < 0.1f)
        {
            return false;
        }

        __instance.timer = 0f;

        var sabotaged = PlayerTask.PlayerHasTaskOfType<IHudOverrideTask>(localPlayer);

        if (sabotaged != __instance.isSab)
        {
            SetSabotaged(__instance, sabotaged);
        }

        if (!sabotaged)
        {
            UpdateBlips(__instance, isAdministrator);
        }

        return false;
    }
}