namespace TownOfSushi.Roles.Abilities
{
    public class Spy : Ability
    {
        public Spy(PlayerControl player) : base(player)
        {
            Name = "Spy";
            TaskText = () => "Gain extra information from all the devices";
            Color = ColorManager.Spy;
            AbilityType = AbilityEnum.Spy;
        }
    }

     [HarmonyPatch]
    public static class SpyVitals
    {
        private static List<TMPro.TextMeshPro> spyTexts = new List<TMPro.TextMeshPro>();

        [HarmonyPatch(typeof(VitalsMinigame), nameof(VitalsMinigame.Begin))]
        class VitalsMinigameStartPatch {
            static void Postfix(VitalsMinigame __instance) {
                if (PlayerControl.LocalPlayer.Is(AbilityEnum.Spy)) {
                    spyTexts = new List<TMPro.TextMeshPro>();
                    foreach (VitalsPanel panel in __instance.vitals) {
                        TMPro.TextMeshPro text = UnityEngine.Object.Instantiate(__instance.SabText, panel.transform);
                        spyTexts.Add(text);
                        UnityEngine.Object.DestroyImmediate(text.GetComponent<AlphaBlink>());
                        text.gameObject.SetActive(false);
                        text.transform.localScale = Vector3.one * 0.75f;
                        text.transform.localPosition = new Vector3(-0.75f, -0.23f, 0f);

                    }
                }
            }
        }

        [HarmonyPatch(typeof(VitalsMinigame), nameof(VitalsMinigame.Update))]
        class VitalsMinigameUpdatePatch {

            static void Postfix(VitalsMinigame __instance) {
                // Spy show time since death
                if (PlayerControl.LocalPlayer.Is(AbilityEnum.Spy)) {
                    for (int k = 0; k < __instance.vitals.Length; k++) {
                        VitalsPanel vitalsPanel = __instance.vitals[k];
                        NetworkedPlayerInfo player = vitalsPanel.PlayerInfo;

                        // Spy update
                        if (vitalsPanel.IsDead) {
                            DeadPlayer deadPlayer = Murder.KilledPlayers?.Where(x => x.PlayerId == player?.PlayerId)?.FirstOrDefault();
                            if (deadPlayer != null && k < spyTexts.Count && spyTexts[k] != null) {
                                float timeSinceDeath = ((float)(DateTime.UtcNow - deadPlayer.KillTime).TotalMilliseconds);
                                spyTexts[k].gameObject.SetActive(true);
                                spyTexts[k].text = Math.Round(timeSinceDeath / 1000) + "s";
                            }
                        }
                    }
                } else {
                    foreach (TMPro.TextMeshPro text in spyTexts)
                        if (text != null && text.gameObject != null)
                            text.gameObject.SetActive(false);
                }
            }
        }
    }

    [HarmonyPatch(typeof(MapCountOverlay), nameof(MapCountOverlay.Update))]
    public static class SpyAdmin
    {
        public static void SetSabotaged(MapCountOverlay __instance, bool sabotaged)
        {
            __instance.isSab = sabotaged;
            __instance.BackgroundColor.SetColor(sabotaged ? Palette.DisabledGrey : Color.green);
            __instance.SabotageText.gameObject.SetActive(sabotaged);
            if (sabotaged)
                foreach (var area in __instance.CountAreas)
                    area.UpdateCount(0);
        }

        public static void UpdateBlips(CounterArea area, List<int> colorMapping, bool isSpy)
        {
            area.UpdateCount(colorMapping.Count);
            var icons = area.myIcons.ToArray();
            colorMapping.Sort();
            for (var i = 0;i < colorMapping.Count;i++)
            {
                var icon = icons[i];
                var sprite = icon.GetComponent<SpriteRenderer>();
                if (SubmergedLoaded) sprite.color = new Color(1, 1, 1, 1);
                if (sprite != null)
                {
                    if (isSpy) PlayerMaterial.SetColors(colorMapping[i], sprite);
                    else PlayerMaterial.SetColors(new Color(0.8793f, 1, 0, 1), sprite);
                }
            }
        }

        public static void UpdateBlips(MapCountOverlay __instance, bool isSpy)
        {
            var rooms = Ship().FastRooms;
            var colorMapDuplicate = new List<int>();
            foreach (var area in __instance.CountAreas)
            {
                if (!rooms.ContainsKey(area.RoomType)) continue;
                var room = rooms[area.RoomType];
                if (room.roomArea == null) continue;
                var objectsInRoom = room.roomArea.OverlapCollider(__instance.filter, __instance.buffer);
                var colorMap = new List<int>();
                for (var i = 0; i < objectsInRoom; i++)
                {
                    var collider = __instance.buffer[i];
                    var player = collider.GetComponent<PlayerControl>();
                    var data = player?.Data;
                    if (collider.tag == "DeadBody" &&
                        (isSpy && CustomGameOptions.WhoSeesDead == AdminDeadPlayers.Spy ||
                        !isSpy && CustomGameOptions.WhoSeesDead == AdminDeadPlayers.EveryoneButSpy ||
                        CustomGameOptions.WhoSeesDead == AdminDeadPlayers.Everyone))
                    {
                        var playerId = collider.GetComponent<DeadBody>().ParentId;
                        colorMap.Add(GameData.Instance.GetPlayerById(playerId).DefaultOutfit.ColorId);
                        colorMapDuplicate.Add(GameData.Instance.GetPlayerById(playerId).DefaultOutfit.ColorId);
                        continue;
                    }
                    else
                    {
                        PlayerControl component = collider.GetComponent<PlayerControl>();
                        if (component && component.Data != null && !component.Data.Disconnected && !component.Data.IsDead && (__instance.showLivePlayerPosition || !component.AmOwner))
                        {
                            if (!colorMapDuplicate.Contains(data.DefaultOutfit.ColorId))
                            {
                                colorMap.Add(data.DefaultOutfit.ColorId);
                                colorMapDuplicate.Add(data.DefaultOutfit.ColorId);
                            }
                        }
                    }
                }
                UpdateBlips(area, colorMap, isSpy);
            }
        }

        public static bool Prefix(MapCountOverlay __instance)
        {
            if (IsHideNSeek()) return true;
            var isSpy = PlayerControl.LocalPlayer.Is(AbilityEnum.Spy);
            __instance.timer += Time.deltaTime;
            if (__instance.timer < 0.1f) return false;

            __instance.timer = 0f;

            var sabotaged = PlayerTask.PlayerHasTaskOfType<IHudOverrideTask>(PlayerControl.LocalPlayer);

            if (sabotaged != __instance.isSab)
                SetSabotaged(__instance, sabotaged);

            if (!sabotaged)
                UpdateBlips(__instance, isSpy);
            return false;
        }
    }
}