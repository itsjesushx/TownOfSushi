namespace TownOfSushi
{
    public static class References
    {
        public static bool MiraHQMap() => VanillaOptions().CurrentGameOptions.MapId == 1;
        public static bool AirshipMap() => VanillaOptions().CurrentGameOptions.MapId == 4;
        public static bool SkeldMap() => VanillaOptions().CurrentGameOptions.MapId == 0;
        public static bool PolusMap() => VanillaOptions().CurrentGameOptions.MapId == 2;
        public static bool FungleMap() => VanillaOptions().CurrentGameOptions.MapId == 5;
        public static bool IsHideNSeek() => VanillaOptions().CurrentGameOptions.GameMode == GameModes.HideNSeek;
        public static bool IsClassic() => VanillaOptions().CurrentGameOptions.GameMode == GameModes.Normal;
        public static bool IsDead() => PlayerControl.LocalPlayer.Data.IsDead;
        public static ShipStatus Ship() => ShipStatus.Instance;
        public static MeetingHud Meeting() => MeetingHud.Instance;
        public static SoundManager Sound() => SoundManager.Instance;
        public static Minigame TaskPanel() => Minigame.Instance;
        public static HudManager HUDManager() => HudManager.Instance;
        public static ExileController ExiledInstance() => ExileController.Instance;
        public static MapBehaviour MapInstance() => MapBehaviour.Instance;
        public static LobbyBehaviour Lobby() => LobbyBehaviour.Instance;
        public static GameOptionsManager VanillaOptions() => GameOptionsManager.Instance;
    }
}