namespace TownOfSushi.Modules
{
    public static class References
    {
        public static bool IsSkeldMap() => OptionsManager().CurrentGameOptions.MapId == 0;
        public static bool IsMiraHQMap() => OptionsManager().CurrentGameOptions.MapId == 1;
        public static bool IsPolusMap() => OptionsManager().CurrentGameOptions.MapId == 2;
        public static bool IsAirshipMap() => OptionsManager().CurrentGameOptions.MapId == 4;
        public static bool IsFungleMap() => OptionsManager().CurrentGameOptions.MapId == 5;
        public static bool IsHideNSeek() => OptionsManager().CurrentGameOptions.GameMode == GameModes.HideNSeek;
        public static bool IsClassic() => OptionsManager().CurrentGameOptions.GameMode == GameModes.Normal;
        public static bool IsDead() => LocalPlayer().Data.IsDead;
        public static bool TwoPlayersAlive() => AllPlayers().Count(x => !x.Data.IsDead) == 2;
        public static bool IsFirstRound() => GameData.Instance?.AllPlayers.Count == 0;
        public static bool IsOnlineGame() => AmongUsClient.Instance.NetworkMode == NetworkModes.OnlineGame;
        public static PlayerControl LocalPlayer() => PlayerControl.LocalPlayer;
        public static bool NullLocalPlayerData() => PlayerControl.LocalPlayer.Data == null;
        public static bool NullLocalPlayer() => PlayerControl.LocalPlayer == null;
        public static PlayerControl[] AllPlayers() => PlayerControl.AllPlayerControls.ToArray();
        public static ChatController Chat() => HUDManager().Chat;
        public static ShipStatus Ship() => ShipStatus.Instance;
        public static MeetingHud Meeting() => MeetingHud.Instance;
        public static SoundManager Sound() => SoundManager.Instance;
        public static Minigame TaskPanel() => Minigame.Instance;
        public static HudManager HUDManager() => HudManager.Instance;
        public static ExileController ExiledInstance() => ExileController.Instance;
        public static MapBehaviour MapInstance() => MapBehaviour.Instance;
        public static LobbyBehaviour IsLobby() => LobbyBehaviour.Instance;
        public static GameOptionsManager OptionsManager() => GameOptionsManager.Instance;
    }
}