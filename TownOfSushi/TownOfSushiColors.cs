using UnityEngine;

namespace TownOfSushi;

public static class TownOfSushiColors
{
    public static bool UseBasic { get; set; } = TownOfSushiPlugin.UseCrewmateTeamColor.Value;

    #region Crew Colors
    public static readonly Color32 Crewmate = Palette.CrewmateRoleBlue;

    public static Color32 Monarch => UseBasic ? Palette.CrewmateBlue : new(255, 128, 0, byte.MaxValue);
    public static Color32 Mayor => UseBasic ? Palette.CrewmateBlue : new Color32(112, 79, 168, byte.MaxValue);
    public static Color32 Engineer => UseBasic ? Palette.CrewmateBlue : new Color32(255, 166, 10, byte.MaxValue);
    public static Color32 Swapper => UseBasic ? Palette.CrewmateBlue : new Color32(102, 230, 102, byte.MaxValue);
    public static Color32 Investigator => UseBasic ? Palette.CrewmateBlue : new Color32(0, 179, 179, byte.MaxValue);
    public static Color32 Medic => UseBasic ? Palette.CrewmateBlue : new(102, 230, 179, byte.MaxValue);
    public static Color32 Detective => UseBasic ? Palette.CrewmateBlue : new Color32(255, 204, 128, byte.MaxValue);
    public static Color32 Administrator => UseBasic ? Palette.CrewmateBlue : new Color32(204, 163, 204, byte.MaxValue);
    public static Color32 Snitch => UseBasic ? Palette.CrewmateBlue : new Color32(212, 176, 56, byte.MaxValue);
    public static Color32 Retributionist => UseBasic ? Palette.CrewmateBlue : new Color32(102, 0, 0, byte.MaxValue);
    public static Color32 Vigilante => UseBasic ? Palette.CrewmateBlue : new Color32(255, 255, 153, byte.MaxValue);
    public static Color32 Veteran => UseBasic ? Palette.CrewmateBlue : new Color32(153, 128, 64, byte.MaxValue);
    public static Color32 Haunter => UseBasic ? Palette.CrewmateBlue : new Color32(212, 212, 212, byte.MaxValue);
    public static Color32 Transporter => UseBasic ? Palette.CrewmateBlue : new Color32(0, 237, 255, byte.MaxValue);
    public static Color32 Medium => UseBasic ? Palette.CrewmateBlue : new Color32(166, 128, 255, byte.MaxValue);
    public static Color32 Mystic => UseBasic ? Palette.CrewmateBlue : new Color32(76, 153, 230, byte.MaxValue);
    public static Color32 Trapper => UseBasic ? Palette.CrewmateBlue : new Color32(166, 209, 179, byte.MaxValue);
    public static Color32 Inspector => UseBasic ? Palette.CrewmateBlue : new Color32(76, 76, 255, byte.MaxValue);
    public static Color32 Imitator => UseBasic ? Palette.CrewmateBlue : new Color32(179, 217, 76, byte.MaxValue);
    public static Color32 Prosecutor => UseBasic ? Palette.CrewmateBlue : new Color32(179, 128, 0, byte.MaxValue);
    public static Color32 Oracle => UseBasic ? Palette.CrewmateBlue : new Color32(191, 0, 191, byte.MaxValue);
    public static Color32 Aurial => UseBasic ? Palette.CrewmateBlue : new Color32(179, 76, 153, byte.MaxValue);
    public static Color32 Politician => UseBasic ? Palette.CrewmateBlue : new Color32(102, 0, 153, byte.MaxValue);
    public static Color32 Operative => UseBasic ? Palette.CrewmateBlue : new(0, 199, 105, byte.MaxValue);
    public static Color32 Crusader => UseBasic ? Palette.CrewmateBlue : new Color32(153, 0, 255, byte.MaxValue);
    public static Color32 Jailor => UseBasic ? Palette.CrewmateBlue : new Color32(166, 166, 166, byte.MaxValue);
    public static Color32 Hunter => UseBasic ? Palette.CrewmateBlue : new Color32(41, 171, 135, byte.MaxValue);
    public static Color32 Tracker => UseBasic ? Palette.CrewmateBlue : new Color32(0, 153, 0, byte.MaxValue);
    public static Color32 Lookout => UseBasic ? Palette.CrewmateBlue : new Color32(51, 255, 102, byte.MaxValue);
    public static Color32 Deputy => UseBasic ? Palette.CrewmateBlue : new Color32(255, 204, 0, byte.MaxValue);
    public static Color32 Plumber => UseBasic ? Palette.CrewmateBlue : new Color32(204, 102, 0, byte.MaxValue);
    public static Color32 Cleric => UseBasic ? Palette.CrewmateBlue : new Color32(0, 255, 179, byte.MaxValue);
    public static Color32 Seer => UseBasic ? Palette.CrewmateBlue : new Color32(179, 153, 102, byte.MaxValue);

    #endregion

    #region Neutral Colors
    public static readonly Color32 Neutral = Color.gray;
    public static readonly Color32 Jester = new(255, 191, 204, byte.MaxValue);
    public static readonly Color32 Executioner = new(204, 204, 204, byte.MaxValue);
    public static readonly Color32 Glitch = Color.green;
    public static readonly Color32 Arsonist = new(255, 77, 0, byte.MaxValue);
    public static readonly Color32 Pyromaniac = new(230, 102, 51, byte.MaxValue);
    public static readonly Color32 Amnesiac = new(128, 179, 255, byte.MaxValue);
    public static readonly Color32 Juggernaut = new(140, 0, 77, byte.MaxValue);
    public static readonly Color32 Inquisitor = new(217, 66, 145, byte.MaxValue);
    public static readonly Color32 Protector = new(179, 255, 255, byte.MaxValue);
    public static readonly Color32 Plaguebearer = new(230, 255, 179, byte.MaxValue);
    public static readonly Color32 Pestilence = new(77, 77, 77, byte.MaxValue);
    public static readonly Color32 Werewolf = new(168, 102, 41, byte.MaxValue);
    public static readonly Color32 Predator = new(51, 102, 255, byte.MaxValue);
    public static readonly Color32 Doomsayer = new(0, 255, 128, byte.MaxValue);
    public static readonly Color32 Vampire = new Color32(38, 38, 38, byte.MaxValue);
    public static readonly Color32 SoulCollector = new(153, 255, 204, byte.MaxValue);
    public static readonly Color32 GuardianAngel = new(179, 255, 255, byte.MaxValue);
    public static readonly Color32 Hitman = new(0, 179, 230, byte.MaxValue);
    public static readonly Color32 Agent = new(102, 102, 204, byte.MaxValue);
    public static readonly Color32 Phantom = new(102, 41, 97, byte.MaxValue);
    public static readonly Color32 Mercenary = new(140, 102, 153, byte.MaxValue);
    public static readonly Color32 Eclipsal = new(128, 77, 77, byte.MaxValue);
    public static readonly Color32 Scavenger = new(153, 77, 51, byte.MaxValue);
    public static readonly Color32 Romantic = new(255, 0, 204, byte.MaxValue);
    public static readonly Color32 Thief = new(77, 128, 51, byte.MaxValue);
    #endregion

    #region Impostor Colors
    public static readonly Color32 Impostor = Palette.ImpostorRed;
    public static readonly Color32 ImpSoft = new(214, 64, 66, byte.MaxValue);
    #endregion

    #region Modifier Colors
    public static readonly Color32 Bait = new(51, 179, 179, byte.MaxValue);
    public static readonly Color32 Aftermath = new(166, 255, 166, byte.MaxValue);
    public static readonly Color32 Diseased = Color.grey;
    public static readonly Color32 Torch = new(255, 255, 153, byte.MaxValue);
    public static readonly Color32 ButtonBarry = new(179, 51, 204, byte.MaxValue);
    public static readonly Color32 Giant = new(255, 179, 77, byte.MaxValue);
    public static readonly Color32 Lover = new(255, 102, 204, byte.MaxValue);
    public static readonly Color32 Sleuth = new(128, 51, 51, byte.MaxValue);
    public static readonly Color32 Tiebreaker = new(153, 230, 153, byte.MaxValue);
    public static readonly Color32 Paranoiac = new(255, 0, 128, byte.MaxValue);
    public static readonly Color32 Drunk = new(26, 77, 51, byte.MaxValue);
    public static readonly Color32 Multitasker = new(255, 128, 77, byte.MaxValue);
    public static readonly Color32 Frosty = new(153, 255, 255, byte.MaxValue);
    public static readonly Color32 SixthSense = new(217, 255, 140, byte.MaxValue);
    public static readonly Color32 Shy = new(255, 179, 204, byte.MaxValue);
    public static readonly Color32 Mini = new(204, 255, 230, byte.MaxValue);
    public static readonly Color32 Camouflaged = Color.gray;
    public static readonly Color32 Satellite = new(0, 153, 204, byte.MaxValue);
    public static readonly Color32 Egotist = new(102, 153, 102, byte.MaxValue);
    public static readonly Color32 Taskmaster = new(148, 214, 237, byte.MaxValue);
    public static readonly Color32 Celebrity = new(255, 153, 153, byte.MaxValue);
    public static readonly Color32 Lazy = new(230, 230, 204, byte.MaxValue);
    public static readonly Color32 Decay = new(171, 128, 105, byte.MaxValue);
    public static readonly Color32 Noisemaker = new(232, 105, 158, byte.MaxValue);
    public static readonly Color32 Scout = new(69, 97, 87, byte.MaxValue);

    #endregion
}