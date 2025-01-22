using Types = TownOfSushi.CustomOption.CustomOption.CustomOptionType;

namespace TownOfSushi.CustomOption
{
    public class CustomOptionHolder
    {
        public static string[] Presets = new string[]{"Preset 1", "Preset 2", "Preset 3", "Preset 4", "Preset 5"};
        public static string ColorString(Color c, string s)
        {
            return string.Format("<color=#{0:X2}{1:X2}{2:X2}{3:X2}>{4}</color>", ToByte(c.r), ToByte(c.g), ToByte(c.b), ToByte(c.a), s);
        }
        private static byte ToByte(float f)
        {
            f = Mathf.Clamp01(f);
            return (byte)(f * 255);
        }
        public static CustomOption PresetSelection;

        #region Crewmate Roles
        public static CustomOption VigilanteKills;
        public static CustomOption VigilanteMultiKill;
        public static CustomOption VigilanteGuessNeutralBenign;
        public static CustomOption VigilanteGuessNeutralEvil;
        public static CustomOption VigilanteGuessNeutralKilling;
        public static CustomOption VigilanteAfterVoting;

        public static CustomOption CrusaderOn;
        public static CustomOption FortifyCooldown;

        public static CustomOption DeputyOn;
        public static CustomOption DeputyKills;

        public static CustomOption MysticOn;
        public static CustomOption MysticArrowDuration;
        public static CustomOption InitialExamineCooldown;
        public static CustomOption MysticExamineCooldown;
        public static CustomOption RecentKill;
        public static CustomOption MysticReportOn;
        public static CustomOption MysticRoleDuration;
        public static CustomOption MysticFactionDuration;
        public static CustomOption MysticExamineReportOn;

        public static CustomOption OracleOn;
        public static CustomOption ConfessCooldown;
        public static CustomOption RevealAccuracy;
        public static CustomOption NeutralBenignShowsEvil;
        public static CustomOption NeutralEvilShowsEvil;
        public static CustomOption NeutralKillingShowsEvil;

        public static CustomOption SeerOn;
        public static CustomOption SeerCd;

        public static CustomOption DetectiveOn;
        public static CustomOption DetectiveCooldown;
        public static CustomOption CrewKillingRed;
        public static CustomOption NeutBenignRed;
        public static CustomOption NeutEvilRed;
        public static CustomOption NeutKillingRed;

        public static CustomOption HunterOn;
        public static CustomOption HunterKillCd;
        public static CustomOption HunterStalkCd;
        public static CustomOption HunterStalkDuration;
        public static CustomOption HunterStalkUses;
        public static CustomOption RetributionOnVote;
        public static CustomOption HunterBodyReport;
        
        public static CustomOption TrackerOn;
        public static CustomOption UpdateInterval;
        public static CustomOption TrackCooldown;
        public static CustomOption ResetOnNewRound;
        public static CustomOption MaxTracks;

        public static CustomOption TrapperOn;
        public static CustomOption TrapCooldown;
        public static CustomOption TrapsRemoveOnNewRound;
        public static CustomOption MaxTraps;
        public static CustomOption MinAmountOfTimeInTrap;
        public static CustomOption TrapSize;
        public static CustomOption MinAmountOfPlayersInTrap;

        public static CustomOption MedicOn;
        public static CustomOption ShowShielded;
        public static CustomOption WhoGetsNotification;
        public static CustomOption ShieldBreaks;
        public static CustomOption MedicReportSwitch;
        public static CustomOption MedicReportNameDuration;
        public static CustomOption MedicReportColorDuration;

        public static CustomOption SwapperOn;
        public static CustomOption SwapperButton;

        public static CustomOption VeteranOn;
        public static CustomOption KilledOnAlert;
        public static CustomOption VeteranBodyReport;
        public static CustomOption AlertCooldown;
        public static CustomOption AlertDuration;
        public static CustomOption MaxAlerts;

        public static CustomOption MediumOn;
        public static CustomOption MediateCooldown;
        public static CustomOption MediumVitals;
        public static CustomOption ShowMediatePlayer;
        public static CustomOption ShowMediumToDead;
        public static CustomOption DeadRevealed;

        public static CustomOption VigilanteOn;
        public static CustomOption JailorOn;

        public static CustomOption JailCooldown;
        public static CustomOption MaxExecutes;

        public static CustomOption ImitatorOn;

        public static CustomOption VigilanteKillOther;
        public static CustomOption VigilanteKillsNeutralEvil;
        public static CustomOption VigilanteKillsNeutralBenign;
        public static CustomOption VigilanteKillCd;
        public static CustomOption VigilanteBodyReport;

        public static CustomOption EngineerOn;
        public static CustomOption MaxFixes;

        public static CustomOption InvestigatorOn;
        public static CustomOption ExamineCooldown;
        public static CustomOption InvestigatorReportOn;
        public static CustomOption InvestigatorRoleDuration;
        public static CustomOption InvestigatorFactionDuration;
        public static CustomOption FootprintSize;
        public static CustomOption FootprintInterval;
        public static CustomOption FootprintDuration;
        public static CustomOption AnonymousFootPrint;
        public static CustomOption VentFootprintVisible;

        #endregion    

        #region Neutral Killing Roles
        public static CustomOption AgentOn;
        public static CustomOption HitmanVent;
        public static CustomOption SkipAgent;
        public static CustomOption HitmanVentWithBody;
        public static CustomOption HitmanKillCooldown;
        public static CustomOption HitmanDragCooldown;
        public static CustomOption HitmanDragSpeed;
        public static CustomOption HitmanMorphDuration;
        public static CustomOption HitmanMorphCooldown;

        public static CustomOption ArsonistOn;
        public static CustomOption ArsoVent;
        public static CustomOption DouseCooldown;
        public static CustomOption MaxDoused;
        public static CustomOption IgniteCdRemoved;

        public static CustomOption PlaguebearerOn;
        public static CustomOption InfectCooldown;
        public static CustomOption PestKillCooldown;
        public static CustomOption PestVent;

        public static CustomOption JuggernautOn;
        public static CustomOption JuggKillCooldown;
        public static CustomOption ReducedKCdPerKill;
        public static CustomOption JuggVent;

        public static CustomOption GlitchOn;
        public static CustomOption MimicCooldownOption;
        public static CustomOption MimicDurationOption;
        public static CustomOption HackCooldownOption;
        public static CustomOption HackDurationOption;
        public static CustomOption GlitchKillCooldownOption;
        public static CustomOption GlitchHackDistanceOption;
        public static CustomOption GlitchVent;

        public static CustomOption VampireOn;
        public static CustomOption BiteCooldown;
        public static CustomOption VampVent;
        public static CustomOption NewVampCanAssassin;
        public static CustomOption MaxVampiresPerGame;
        public static CustomOption CanBiteNeutralBenign;
        public static CustomOption CanBiteNeutralEvil;


        public static CustomOption SerialKillerOn;
        public static CustomOption StabCooldown;
        public static CustomOption Stabeduration;
        public static CustomOption StabKillCooldown;
        public static CustomOption SerialKillerVent;

        #endregion

        #region Impostor Roles
        public static CustomOption EscapistOn;
        public static CustomOption EscapeCooldown;

        public static CustomOption MorphlingOn;
        public static CustomOption MorphlingCooldown;
        public static CustomOption MorphlingDuration;

        public static CustomOption SwooperOn;
        public static CustomOption SwoopCooldown;
        public static CustomOption SwoopDuration;

        public static CustomOption PoisonerOn;
        public static CustomOption PoisonDelay;
        public static CustomOption PoisonCooldown;

        public static CustomOption GrenadierOn;
        public static CustomOption GrenadeCooldown;
        public static CustomOption GrenadeDuration;
        public static CustomOption GrenadierIndicators;
        public static CustomOption FlashRadius;

        public static CustomOption VenererOn;
        public static CustomOption AbilityCooldown;
        public static CustomOption AbilityDuration;
        public static CustomOption SprintSpeed;
        public static CustomOption FreezeSpeed;

        public static CustomOption BomberOn;
        public static CustomOption MaxKillsInDetonation;
        public static CustomOption DetonateDelay;
        public static CustomOption DetonateRadius;

        public static CustomOption WarlockOn;
        public static CustomOption ChargeUpDuration;
        public static CustomOption ChargeUseDuration;

        public static CustomOption WitchOn;
        public static CustomOption SpellCd;

        public static CustomOption BlackmailerOn;
        public static CustomOption BlackmailCooldown;
        public static CustomOption BlackmailInvisible;

        public static CustomOption JanitorOn;

        public static CustomOption MinerOn;
        public static CustomOption MineCooldown;

        public static CustomOption UndertakerOn;
        public static CustomOption DragCooldown;
        public static CustomOption UndertakerDragSpeed;
        public static CustomOption UndertakerVentWithBody;

        #endregion

        #region Modifiers
        public static CustomOption AftermathOn;

        public static CustomOption BaitOn;
        public static CustomOption BaitMinDelay;
        public static CustomOption BaitMaxDelay;

        public static CustomOption DiseasedOn;
        public static CustomOption DiseasedKillMultiplier;

        public static CustomOption FrostyOn;
        public static CustomOption ChillDuration;
        public static CustomOption ChillStartSpeed;

        public static CustomOption MultitaskerOn;

        #endregion

        #region Abilities
        public static CustomOption NumberOfImpostorAssassins;
        public static CustomOption NumberOfNeutralAssassins;
        public static CustomOption AmneTurnImpAssassin;
        public static CustomOption AmneTurnNeutAssassin;
        public static CustomOption AssassinKills;
        public static CustomOption AssassinMultiKill;
        public static CustomOption AssassinCrewmateGuess;
        public static CustomOption AssassinGuessNeutralBenign;
        public static CustomOption AssassinGuessNeutralEvil;

        public static CustomOption ButtonBarryOn;

        public static CustomOption FlashOn;
        public static CustomOption FlashSpeed;

        public static CustomOption GiantOn;
        public static CustomOption GiantSlow;

        public static CustomOption MiniOn;
        public static CustomOption MiniSpeed;

        public static CustomOption ParanoiacOn;

        public static CustomOption SleuthOn;

        public static CustomOption TorchOn;

        public static CustomOption DrunkOn;

        public static CustomOption TiebreakerOn;

        #endregion

        #region Impostor Modifiers
        public static CustomOption ImpostorModifiers;
        public static CustomOption DisperserOn;
        public static CustomOption DisperseCooldown;
        public static CustomOption MaxDisperses;

        public static CustomOption SpyOn;
        public static CustomOption WhoSeesDead;

        public static CustomOption DoubleShotOn;

        public static CustomOption UnderdogOn;
        public static CustomOption UnderdogKillBonus;
        public static CustomOption UnderdogIncreasedKC;

        #endregion

        #region Neutral Evil Roles
        public static CustomOption JesterOn;
        public static CustomOption JesterButton;
        public static CustomOption JesterVent;
        public static CustomOption JesterVentSwitch;
        public static CustomOption JesterImpVision;

        public static CustomOption ExecutionerOn;
        public static CustomOption OnTargetDead;
        public static CustomOption ExecutionerButton;

        public static CustomOption VultureOn;
        public static CustomOption VultureCd;
        public static CustomOption VultureBodyCount;
        public static CustomOption VultureVent;
        public static CustomOption VultureImpVision;
        public static CustomOption EatArrows;
        public static CustomOption EatArrowDelay;

        public static CustomOption DoomsayerOn;
        public static CustomOption ObserveCooldown;
        public static CustomOption DoomsayerGuessNeutralBenign;
        public static CustomOption DoomsayerGuessNeutralEvil;
        public static CustomOption DoomsayerGuessNeutralKilling;
        public static CustomOption DoomsayerGuessImpostors;
        public static CustomOption DoomsayerAfterVoting;
        public static CustomOption DoomsayerGuessesToWin;
        
        #endregion

        public static CustomOption WerewolfOn;
        public static CustomOption MaulCooldown;
        public static CustomOption MaulRadius;
        public static CustomOption WerewolfVent;

        #region Neutral Benign Roles
        public static CustomOption AmnesiacOn;

        public static CustomOption RomanticOn;
        public static CustomOption PickStartTimer;
        public static CustomOption RomanticOnBelovedDeath;
        public static CustomOption RomanticBelovedKnows;
        public static CustomOption RomanticKnowsBelovedRole;

        public static CustomOption GuardianAngelOn;
        public static CustomOption ProtectCd;
        public static CustomOption ProtectDuration;
        public static CustomOption ProtectKCReset;
        public static CustomOption MaxProtects;
        public static CustomOption ShowProtect;
        public static CustomOption GaOnTargetDeath;
        public static CustomOption GATargetKnows;
        public static CustomOption GAKnowsTargetRole;
        public static CustomOption EvilTargetPercent;

        #endregion

        #region General Mod Settings
        public static CustomOption RandomMapEnabled;
        public static CustomOption RandomMapSkeld;
        public static CustomOption RandomMapMira;
        public static CustomOption RandomMapPolus;
        public static CustomOption RandomMapAirship;
        public static CustomOption RandomMapFungle;
        public static CustomOption RandomMapSubmerged;
        public static CustomOption RandomMapLevelImpostor;

        public static CustomOption ColourblindComms;
        public static CustomOption AnyoneStopStart;
        public static CustomOption ImpostorSeeRoles;
        public static CustomOption CamoCommsKillAnyone;
        public static CustomOption InitialCooldowns;
        public static CustomOption ParallelMedScans;
        public static CustomOption SkipButtonDisable;
        public static CustomOption FirstDeathShield;

        public static CustomOption RoleListSettings;
        public static CustomOption UniqueRoles;
        public static CustomOption Slot1;
        public static CustomOption Slot2;
        public static CustomOption Slot3;
        public static CustomOption Slot4;
        public static CustomOption Slot5;
        public static CustomOption Slot6;
        public static CustomOption Slot7;
        public static CustomOption Slot8;
        public static CustomOption Slot9;
        public static CustomOption Slot10;
        public static CustomOption Slot11;
        public static CustomOption Slot12;
        public static CustomOption Slot13;
        public static CustomOption Slot14;
        public static CustomOption Slot15;

        public static CustomOption VentImprovements;
        public static CustomOption VitalsLab;
        public static CustomOption ColdTempDeathValley;
        public static CustomOption WifiChartCourseSwap;

        #endregion
        public static void Load()
        {
            CustomOption.vanillaSettings = TownOfSushi.Instance.Config.Bind("Preset0", "VanillaOptions", "");

            PresetSelection = CustomOption.Create(1, Types.General, "Preset", Presets, null, true);
            #region  General Mod Settings

            UniqueRoles = CustomOption.Create(22230, Types.General, "All Roles Are Unique", true, null, true, heading: "Role List Settings");
            Slot1 = CustomOption.Create(22234, Types.General, "Slot 1", new[] { "<color=#66FFFFFF>Crew</color> Investigative",
                "<color=#66FFFFFF>Crew</color> Killing", "<color=#66FFFFFF>Crew</color> Protective", "<color=#66FFFFFF>Crew</color> Support",
                "Common <color=#66FFFFFF>Crew</color>", "Random <color=#66FFFFFF>Crew</color>", "<color=#999999FF>Neutral</color> Benign",
                "<color=#999999FF>Neutral</color> Evil", "<color=#999999FF>Neutral</color> Killing", "Common <color=#999999FF>Neutral</color>",
                "Random <color=#999999FF>Neutral</color>", "<color=#FF0000FF>Imp</color> Concealing", "<color=#FF0000FF>Imp</color> Killing",
                "<color=#FF0000FF>Imp</color> Support", "Common <color=#FF0000FF>Imp</color>", "Random <color=#FF0000FF>Imp</color>",
                "Non-<color=#FF0000FF>Imp</color>", "Any" }, UniqueRoles);
            Slot2 = CustomOption.Create(22235, Types.General, "Slot 2", new[] { "<color=#66FFFFFF>Crew</color> Investigative",
                "<color=#66FFFFFF>Crew</color> Killing", "<color=#66FFFFFF>Crew</color> Protective", "<color=#66FFFFFF>Crew</color> Support",
                "Common <color=#66FFFFFF>Crew</color>", "Random <color=#66FFFFFF>Crew</color>", "<color=#999999FF>Neutral</color> Benign",
                "<color=#999999FF>Neutral</color> Evil", "<color=#999999FF>Neutral</color> Killing", "Common <color=#999999FF>Neutral</color>",
                "Random <color=#999999FF>Neutral</color>", "<color=#FF0000FF>Imp</color> Concealing", "<color=#FF0000FF>Imp</color> Killing",
                "<color=#FF0000FF>Imp</color> Support", "Common <color=#FF0000FF>Imp</color>", "Random <color=#FF0000FF>Imp</color>",
                "Non-<color=#FF0000FF>Imp</color>", "Any" }, UniqueRoles);
            Slot3 = CustomOption.Create(22236, Types.General, "Slot 3", new[] { "<color=#66FFFFFF>Crew</color> Investigative",
                "<color=#66FFFFFF>Crew</color> Killing", "<color=#66FFFFFF>Crew</color> Protective", "<color=#66FFFFFF>Crew</color> Support",
                "Common <color=#66FFFFFF>Crew</color>", "Random <color=#66FFFFFF>Crew</color>", "<color=#999999FF>Neutral</color> Benign",
                "<color=#999999FF>Neutral</color> Evil", "<color=#999999FF>Neutral</color> Killing", "Common <color=#999999FF>Neutral</color>",
                "Random <color=#999999FF>Neutral</color>", "<color=#FF0000FF>Imp</color> Concealing", "<color=#FF0000FF>Imp</color> Killing",
                "<color=#FF0000FF>Imp</color> Support", "Common <color=#FF0000FF>Imp</color>", "Random <color=#FF0000FF>Imp</color>",
                "Non-<color=#FF0000FF>Imp</color>", "Any" }, UniqueRoles);
            Slot4 = CustomOption.Create(22237, Types.General, "Slot 4", new[] { "<color=#66FFFFFF>Crew</color> Investigative",
                "<color=#66FFFFFF>Crew</color> Killing", "<color=#66FFFFFF>Crew</color> Protective", "<color=#66FFFFFF>Crew</color> Support",
                "Common <color=#66FFFFFF>Crew</color>", "Random <color=#66FFFFFF>Crew</color>", "<color=#999999FF>Neutral</color> Benign",
                "<color=#999999FF>Neutral</color> Evil", "<color=#999999FF>Neutral</color> Killing", "Common <color=#999999FF>Neutral</color>",
                "Random <color=#999999FF>Neutral</color>", "<color=#FF0000FF>Imp</color> Concealing", "<color=#FF0000FF>Imp</color> Killing",
                "<color=#FF0000FF>Imp</color> Support", "Common <color=#FF0000FF>Imp</color>", "Random <color=#FF0000FF>Imp</color>",
                "Non-<color=#FF0000FF>Imp</color>", "Any" }, UniqueRoles);
            Slot5 = CustomOption.Create(22238, Types.General, "Slot 5", new[] { "<color=#66FFFFFF>Crew</color> Investigative",
                "<color=#66FFFFFF>Crew</color> Killing", "<color=#66FFFFFF>Crew</color> Protective", "<color=#66FFFFFF>Crew</color> Support",
                "Common <color=#66FFFFFF>Crew</color>", "Random <color=#66FFFFFF>Crew</color>", "<color=#999999FF>Neutral</color> Benign",
                "<color=#999999FF>Neutral</color> Evil", "<color=#999999FF>Neutral</color> Killing", "Common <color=#999999FF>Neutral</color>",
                "Random <color=#999999FF>Neutral</color>", "<color=#FF0000FF>Imp</color> Concealing", "<color=#FF0000FF>Imp</color> Killing",
                "<color=#FF0000FF>Imp</color> Support", "Common <color=#FF0000FF>Imp</color>", "Random <color=#FF0000FF>Imp</color>",
                "Non-<color=#FF0000FF>Imp</color>", "Any" }, UniqueRoles);
            Slot6 = CustomOption.Create(22239, Types.General, "Slot 6", new[] { "<color=#66FFFFFF>Crew</color> Investigative",
                "<color=#66FFFFFF>Crew</color> Killing", "<color=#66FFFFFF>Crew</color> Protective", "<color=#66FFFFFF>Crew</color> Support",
                "Common <color=#66FFFFFF>Crew</color>", "Random <color=#66FFFFFF>Crew</color>", "<color=#999999FF>Neutral</color> Benign",
                "<color=#999999FF>Neutral</color> Evil", "<color=#999999FF>Neutral</color> Killing", "Common <color=#999999FF>Neutral</color>",
                "Random <color=#999999FF>Neutral</color>", "<color=#FF0000FF>Imp</color> Concealing", "<color=#FF0000FF>Imp</color> Killing",
                "<color=#FF0000FF>Imp</color> Support", "Common <color=#FF0000FF>Imp</color>", "Random <color=#FF0000FF>Imp</color>",
                "Non-<color=#FF0000FF>Imp</color>", "Any" }, UniqueRoles);
            Slot7 = CustomOption.Create(22245, Types.General, "Slot 7", new[] { "<color=#66FFFFFF>Crew</color> Investigative",
                "<color=#66FFFFFF>Crew</color> Killing", "<color=#66FFFFFF>Crew</color> Protective", "<color=#66FFFFFF>Crew</color> Support",
                "Common <color=#66FFFFFF>Crew</color>", "Random <color=#66FFFFFF>Crew</color>", "<color=#999999FF>Neutral</color> Benign",
                "<color=#999999FF>Neutral</color> Evil", "<color=#999999FF>Neutral</color> Killing", "Common <color=#999999FF>Neutral</color>",
                "Random <color=#999999FF>Neutral</color>", "<color=#FF0000FF>Imp</color> Concealing", "<color=#FF0000FF>Imp</color> Killing",
                "<color=#FF0000FF>Imp</color> Support", "Common <color=#FF0000FF>Imp</color>", "Random <color=#FF0000FF>Imp</color>",
                "Non-<color=#FF0000FF>Imp</color>", "Any" }, UniqueRoles);
            Slot8 = CustomOption.Create(22243, Types.General, "Slot 8", new[] { "<color=#66FFFFFF>Crew</color> Investigative",
                "<color=#66FFFFFF>Crew</color> Killing", "<color=#66FFFFFF>Crew</color> Protective", "<color=#66FFFFFF>Crew</color> Support",
                "Common <color=#66FFFFFF>Crew</color>", "Random <color=#66FFFFFF>Crew</color>", "<color=#999999FF>Neutral</color> Benign",
                "<color=#999999FF>Neutral</color> Evil", "<color=#999999FF>Neutral</color> Killing", "Common <color=#999999FF>Neutral</color>",
                "Random <color=#999999FF>Neutral</color>", "<color=#FF0000FF>Imp</color> Concealing", "<color=#FF0000FF>Imp</color> Killing",
                "<color=#FF0000FF>Imp</color> Support", "Common <color=#FF0000FF>Imp</color>", "Random <color=#FF0000FF>Imp</color>",
                "Non-<color=#FF0000FF>Imp</color>", "Any" }, UniqueRoles);
            Slot9 = CustomOption.Create(22244, Types.General, "Slot 9", new[] { "<color=#66FFFFFF>Crew</color> Investigative",
                "<color=#66FFFFFF>Crew</color> Killing", "<color=#66FFFFFF>Crew</color> Protective", "<color=#66FFFFFF>Crew</color> Support",
                "Common <color=#66FFFFFF>Crew</color>", "Random <color=#66FFFFFF>Crew</color>", "<color=#999999FF>Neutral</color> Benign",
                "<color=#999999FF>Neutral</color> Evil", "<color=#999999FF>Neutral</color> Killing", "Common <color=#999999FF>Neutral</color>",
                "Random <color=#999999FF>Neutral</color>", "<color=#FF0000FF>Imp</color> Concealing", "<color=#FF0000FF>Imp</color> Killing",
                "<color=#FF0000FF>Imp</color> Support", "Common <color=#FF0000FF>Imp</color>", "Random <color=#FF0000FF>Imp</color>",
                "Non-<color=#FF0000FF>Imp</color>", "Any" }, UniqueRoles);
            Slot10 = CustomOption.Create(22246, Types.General, "Slot 10", new[] { "<color=#66FFFFFF>Crew</color> Investigative",
                "<color=#66FFFFFF>Crew</color> Killing", "<color=#66FFFFFF>Crew</color> Protective", "<color=#66FFFFFF>Crew</color> Support",
                "Common <color=#66FFFFFF>Crew</color>", "Random <color=#66FFFFFF>Crew</color>", "<color=#999999FF>Neutral</color> Benign",
                "<color=#999999FF>Neutral</color> Evil", "<color=#999999FF>Neutral</color> Killing", "Common <color=#999999FF>Neutral</color>",
                "Random <color=#999999FF>Neutral</color>", "<color=#FF0000FF>Imp</color> Concealing", "<color=#FF0000FF>Imp</color> Killing",
                "<color=#FF0000FF>Imp</color> Support", "Common <color=#FF0000FF>Imp</color>", "Random <color=#FF0000FF>Imp</color>",
                "Non-<color=#FF0000FF>Imp</color>", "Any" }, UniqueRoles);
            Slot11 = CustomOption.Create(22247, Types.General, "Slot 11", new[] { "<color=#66FFFFFF>Crew</color> Investigative",
                "<color=#66FFFFFF>Crew</color> Killing", "<color=#66FFFFFF>Crew</color> Protective", "<color=#66FFFFFF>Crew</color> Support",
                "Common <color=#66FFFFFF>Crew</color>", "Random <color=#66FFFFFF>Crew</color>", "<color=#999999FF>Neutral</color> Benign",
                "<color=#999999FF>Neutral</color> Evil", "<color=#999999FF>Neutral</color> Killing", "Common <color=#999999FF>Neutral</color>",
                "Random <color=#999999FF>Neutral</color>", "<color=#FF0000FF>Imp</color> Concealing", "<color=#FF0000FF>Imp</color> Killing",
                "<color=#FF0000FF>Imp</color> Support", "Common <color=#FF0000FF>Imp</color>", "Random <color=#FF0000FF>Imp</color>",
                "Non-<color=#FF0000FF>Imp</color>", "Any" }, UniqueRoles);
            Slot12 = CustomOption.Create(22248, Types.General, "Slot 12", new[] { "<color=#66FFFFFF>Crew</color> Investigative",
                "<color=#66FFFFFF>Crew</color> Killing", "<color=#66FFFFFF>Crew</color> Protective", "<color=#66FFFFFF>Crew</color> Support",
                "Common <color=#66FFFFFF>Crew</color>", "Random <color=#66FFFFFF>Crew</color>", "<color=#999999FF>Neutral</color> Benign",
                "<color=#999999FF>Neutral</color> Evil", "<color=#999999FF>Neutral</color> Killing", "Common <color=#999999FF>Neutral</color>",
                "Random <color=#999999FF>Neutral</color>", "<color=#FF0000FF>Imp</color> Concealing", "<color=#FF0000FF>Imp</color> Killing",
                "<color=#FF0000FF>Imp</color> Support", "Common <color=#FF0000FF>Imp</color>", "Random <color=#FF0000FF>Imp</color>",
                "Non-<color=#FF0000FF>Imp</color>", "Any" }, UniqueRoles);
            Slot13 = CustomOption.Create(22249, Types.General, "Slot 13", new[] { "<color=#66FFFFFF>Crew</color> Investigative",
                "<color=#66FFFFFF>Crew</color> Killing", "<color=#66FFFFFF>Crew</color> Protective", "<color=#66FFFFFF>Crew</color> Support",
                "Common <color=#66FFFFFF>Crew</color>", "Random <color=#66FFFFFF>Crew</color>", "<color=#999999FF>Neutral</color> Benign",
                "<color=#999999FF>Neutral</color> Evil", "<color=#999999FF>Neutral</color> Killing", "Common <color=#999999FF>Neutral</color>",
                "Random <color=#999999FF>Neutral</color>", "<color=#FF0000FF>Imp</color> Concealing", "<color=#FF0000FF>Imp</color> Killing",
                "<color=#FF0000FF>Imp</color> Support", "Common <color=#FF0000FF>Imp</color>", "Random <color=#FF0000FF>Imp</color>",
                "Non-<color=#FF0000FF>Imp</color>", "Any" }, UniqueRoles);
            Slot14 = CustomOption.Create(22250, Types.General, "Slot 14", new[] { "<color=#66FFFFFF>Crew</color> Investigative",
                "<color=#66FFFFFF>Crew</color> Killing", "<color=#66FFFFFF>Crew</color> Protective", "<color=#66FFFFFF>Crew</color> Support",
                "Common <color=#66FFFFFF>Crew</color>", "Random <color=#66FFFFFF>Crew</color>", "<color=#999999FF>Neutral</color> Benign",
                "<color=#999999FF>Neutral</color> Evil", "<color=#999999FF>Neutral</color> Killing", "Common <color=#999999FF>Neutral</color>",
                "Random <color=#999999FF>Neutral</color>", "<color=#FF0000FF>Imp</color> Concealing", "<color=#FF0000FF>Imp</color> Killing",
                "<color=#FF0000FF>Imp</color> Support", "Common <color=#FF0000FF>Imp</color>", "Random <color=#FF0000FF>Imp</color>",
                "Non-<color=#FF0000FF>Imp</color>", "Any" }, UniqueRoles);
            Slot15 = CustomOption.Create(22251, Types.General, "Slot 15", new[] { "<color=#66FFFFFF>Crew</color> Investigative",
                "<color=#66FFFFFF>Crew</color> Killing", "<color=#66FFFFFF>Crew</color> Protective", "<color=#66FFFFFF>Crew</color> Support",
                "Common <color=#66FFFFFF>Crew</color>", "Random <color=#66FFFFFF>Crew</color>", "<color=#999999FF>Neutral</color> Benign",
                "<color=#999999FF>Neutral</color> Evil", "<color=#999999FF>Neutral</color> Killing", "Common <color=#999999FF>Neutral</color>",
                "Random <color=#999999FF>Neutral</color>", "<color=#FF0000FF>Imp</color> Concealing", "<color=#FF0000FF>Imp</color> Killing",
                "<color=#FF0000FF>Imp</color> Support", "Common <color=#FF0000FF>Imp</color>", "Random <color=#FF0000FF>Imp</color>",
                "Non-<color=#FF0000FF>Imp</color>", "Any" }, UniqueRoles);
                
            CamoCommsKillAnyone = CustomOption.Create(2, Types.General, "Kill Anyone During Camouflaged Comms", false, null, true, heading: "Custom Game Options");
            ColourblindComms = CustomOption.Create(3, Types.General, "Camouflaged Comms", false);
            AnyoneStopStart = CustomOption.Create(3333, Types.General, "All Players Can Stop The Start", false);
            ImpostorSeeRoles = CustomOption.Create(4, Types.General, "Impostors Can See The Roles Of Their Team", false);
            InitialCooldowns = CustomOption.Create(5, Types.General, "Game Start Cooldowns", 10f, 10f, 30f, 2.5f, format: "s");
            ParallelMedScans = CustomOption.Create(6, Types.General, "Parallel Medbay Scans", false);
            SkipButtonDisable = CustomOption.Create(7, Types.General, "Disable Meeting Skip Button", new[] { "No", "Emergency", "Always" });
            FirstDeathShield = CustomOption.Create(8, Types.General, "Shield Last Game First Kill", false);

            RandomMapEnabled = CustomOption.Create(9, Types.General, "Enable Random Map", false, null, true, heading: "Map Settings");
            RandomMapSkeld = CustomOption.Create(11, Types.General, "Skeld Chance", 0f, 0f, 100f, 10f, format: "%");
            RandomMapMira = CustomOption.Create(12, Types.General, "Mira Chance", 0f, 0f, 100f, 10f, format: "%");
            RandomMapPolus = CustomOption.Create(13, Types.General, "Polus Chance", 0f, 0f, 100f, 10f, format: "%");
            RandomMapAirship = CustomOption.Create(14, Types.General, "Airship Chance", 0f, 0f, 100f, 10f, format: "%");
            RandomMapFungle = CustomOption.Create(15, Types.General, "Fungle Chance", 0f, 0f, 100f, 10f, format: "%");
            RandomMapSubmerged = CustomOption.Create(16, Types.General, "Submerged Chance", 0f, 0f, 100f, 10f, format: "%");
            RandomMapLevelImpostor = CustomOption.Create(17, Types.General, "Level Impostor Chance", 0f, 0f, 100f, 10f, format: "%");

            VentImprovements = CustomOption.Create(18, Types.General, "Better Polus Vent Layout", false, null, true, heading: "Better Polus Settings");
            VitalsLab = CustomOption.Create(20, Types.General, "Vitals Moved To Lab", false);
            ColdTempDeathValley = CustomOption.Create(21, Types.General, "Cold Temp Moved To Death Valley", false);
            WifiChartCourseSwap = CustomOption.Create(22, Types.General, "Reboot Wifi And Chart Course Swapped", false);

            #endregion

            #region  Crewmate Roles

            InvestigatorOn = CustomOption.Create(30, Types.Crewmate, ColorString(Colors.Investigator, "Investigator"), 0f, 0f, 100f, 10f, null, true, format: "%");
            FootprintSize = CustomOption.Create(31, Types.Crewmate, "Footprint Size", 4f, 1f, 10f, 1f, InvestigatorOn);
            FootprintInterval = CustomOption.Create(32, Types.Crewmate, "Footprint Interval", 0.1f, 0.05f, 1f, 0.05f, InvestigatorOn);
            FootprintDuration = CustomOption.Create(33, Types.Crewmate, "Footprint Duration", 10f, 1f, 15f, 0.5f, InvestigatorOn, format: "s");
            AnonymousFootPrint = CustomOption.Create(34, Types.Crewmate, "Anonymous Footprint", false, InvestigatorOn);
            VentFootprintVisible = CustomOption.Create(35, Types.Crewmate, "Footprint Vent Visible", false, InvestigatorOn);
            ExamineCooldown = CustomOption.Create(36, Types.Crewmate, "Cooldown", 25f, 10f, 60f, 2.5f, InvestigatorOn, format: "s");
            InvestigatorReportOn = CustomOption.Create(37, Types.Crewmate, "Show Investigator Reports", true, InvestigatorOn);
            InvestigatorRoleDuration = CustomOption.Create(38, Types.Crewmate, "Time Where Investigator Will Have Role", 15f, 0f, 60f, 2.5f, InvestigatorOn, format: "s");
            InvestigatorFactionDuration = CustomOption.Create(39, Types.Crewmate, "Time Where Investigator Will Have Faction", 30f, 0f, 60f, 2.5f, InvestigatorOn, format: "s");


            MediumOn = CustomOption.Create(40, Types.Crewmate, ColorString(Colors.Medium, "Medium"), 0f, 0f, 100f, 10f, null, true, format: "%");
            MediateCooldown = CustomOption.Create(41, Types.Crewmate, "Cooldown", 10f, 1f, 15f, 1f, MediumOn, format: "s");
            ShowMediatePlayer = CustomOption.Create(42, Types.Crewmate, "Reveal Appearance Of Mediate Target", true, MediumOn);
            ShowMediumToDead = CustomOption.Create(43, Types.Crewmate, "Reveal The Medium To The Mediate Target", true, MediumOn);
            MediumVitals = CustomOption.Create(4311, Types.Crewmate, "Can Use Vitals", false, MediumOn);
            DeadRevealed = CustomOption.Create(44, Types.Crewmate, "Who Is Revealed With Mediate", new[] { "Oldest Dead", "Newest Dead", "All Dead" }, MediumOn);


            MysticOn = CustomOption.Create(46, Types.Crewmate, ColorString(Colors.Mystic, "Mystic"), 0f, 0f, 100f, 10f, null, true, format: "%");
            MysticArrowDuration = CustomOption.Create(47, Types.Crewmate, "Dead Body Arrow Duration", 0.1f, 0f, 1f, 0.05f, MysticOn, format: "s");
            InitialExamineCooldown = CustomOption.Create(48, Types.Crewmate, "Initial Cooldown", 30f, 10f, 60f, 2.5f, MysticOn, format: "s");
            MysticExamineCooldown = CustomOption.Create(49, Types.Crewmate, "Cooldown", 25f, 10f, 60f, 2.5f, MysticOn, format: "s");
            RecentKill = CustomOption.Create(50, Types.Crewmate, "How Long Players Stay Bloody For", 30f, 10f, 60f, 2.5f, MysticOn, format: "s");
            MysticReportOn = CustomOption.Create(51, Types.Crewmate, "Show Mystic Reports", true, MysticOn);
            MysticRoleDuration = CustomOption.Create(52, Types.Crewmate, "Time Where Mystic Will Have Role", 15f, 0f, 60f, 2.5f, MysticOn, format: "s");
            MysticFactionDuration = CustomOption.Create(53, Types.Crewmate, "Time Where Mystic Will Have Faction", 30f, 0f, 60f, 2.5f, MysticOn, format: "s");
            MysticExamineReportOn = CustomOption.Create(54, Types.Crewmate, "Show Mystic Examine Reports", true, MysticOn);


            OracleOn = CustomOption.Create(55, Types.Crewmate, ColorString(Colors.Oracle, "Oracle"), 0f, 0f, 100f, 10f, null, true, format: "%");
            ConfessCooldown = CustomOption.Create(56, Types.Crewmate, "Cooldown", 25f, 10f, 60f, 2.5f, OracleOn, format: "s");
            RevealAccuracy = CustomOption.Create(57, Types.Crewmate, "Reveal Accuracy", 80f, 0f, 100f, 10f, OracleOn, format: "%");
            NeutralBenignShowsEvil = CustomOption.Create(58, Types.Crewmate, "Neutral Benign Roles Show Evil", false, OracleOn);
            NeutralEvilShowsEvil = CustomOption.Create(59, Types.Crewmate, "Neutral Evil Roles Show Evil", false, OracleOn);
            NeutralKillingShowsEvil = CustomOption.Create(60, Types.Crewmate, "Neutral Killing Roles Show Evil", true, OracleOn);

            SeerOn = CustomOption.Create(6451, Types.Crewmate, ColorString(Colors.Seer, "Seer"), 0f, 0f, 100f, 10f, null, true, format: "%");
            SeerCd = CustomOption.Create(6452, Types.Crewmate, "Cooldown", 25f, 10f, 60f, 2.5f, SeerOn);


            DetectiveOn = CustomOption.Create(61, Types.Crewmate, ColorString(Colors.Detective, "Detective"), 0f, 0f, 100f, 10f, null, true, format: "%");
            DetectiveCooldown = CustomOption.Create(62, Types.Crewmate, "Cooldown", 25f, 10f, 60f, 2.5f, DetectiveOn);
            CrewKillingRed = CustomOption.Create(63, Types.Crewmate, "Crew Killing Roles Are Red", false, DetectiveOn);
            NeutBenignRed = CustomOption.Create(64, Types.Crewmate, "Neutral Benign Roles Are Red", false, DetectiveOn);
            NeutEvilRed = CustomOption.Create(65, Types.Crewmate, "Neutral Evil Roles Are Red", false, DetectiveOn);
            NeutKillingRed = CustomOption.Create(66, Types.Crewmate, "Neutral Killing Roles Are Red", true, DetectiveOn);


            TrackerOn =
                CustomOption.Create(71, Types.Crewmate, ColorString(Colors.Tracker, "Tracker"), 0f, 0f, 100f, 10f, null, true, format: "%");
            UpdateInterval = CustomOption.Create(72, Types.Crewmate, "Arrow Update Interval", 5f, 0.5f, 15f, 0.5f, TrackerOn, format: "s");
            TrackCooldown = CustomOption.Create(73, Types.Crewmate, "Cooldown", 25f, 10f, 60f, 2.5f, TrackerOn, format: "s");
            ResetOnNewRound = CustomOption.Create(74, Types.Crewmate, "Arrows Reset After Each Round", false, TrackerOn);
            MaxTracks = CustomOption.Create(75, Types.Crewmate, "Maximum Number Of Tracks Per Round", 5, 1, 15, 1, TrackerOn);

            TrapperOn = CustomOption.Create(76, Types.Crewmate, ColorString(Colors.Trapper, "Trapper"), 0f, 0f, 100f, 10f, null, true, format: "%");
            MinAmountOfTimeInTrap = CustomOption.Create(77, Types.Crewmate, "Min Amount Of Time In Trap To Register", 1f, 0f, 15f, 0.5f, TrapperOn, format: "s");
            TrapCooldown = CustomOption.Create(78, Types.Crewmate, "Trap Cooldown", 25f, 10f, 40f, 2.5f, TrapperOn, format: "s");
            TrapsRemoveOnNewRound = CustomOption.Create(79, Types.Crewmate, "Traps Removed After Each Round", true, TrapperOn);
            MaxTraps = CustomOption.Create(80, Types.Crewmate, "Maximum Number Of Traps Per Game", 5, 1, 15, 1, TrapperOn);
            TrapSize = CustomOption.Create(81, Types.Crewmate, "Trap Size", 0.25f, 0.05f, 1f, 0.05f, TrapperOn, format: "x");
            MinAmountOfPlayersInTrap = CustomOption.Create(82, Types.Crewmate, "Minimum Number Of Roles Required To Trigger Trap", 3, 1, 5, 1, TrapperOn);

            DeputyOn = CustomOption.Create(992222, Types.Crewmate, ColorString(Colors.Deputy, "Deputy"), 0f, 0f, 100f, 10f, null, true, format: "%");
            DeputyKills = CustomOption.Create(100222, Types.Crewmate, "Number Of Deputy Kills", 1, 1, 15, 1, DeputyOn);
                
            
            HunterOn = CustomOption.Create(84, Types.Crewmate, ColorString(Colors.Hunter, "Hunter"), 0f, 0f, 100f, 10f, null, true, format: "%");
            HunterKillCd = CustomOption.Create(85, Types.Crewmate, "Kill Cooldown", 25f, 10f, 60f, 2.5f, HunterOn, format: "s");
            HunterStalkCd = CustomOption.Create(86, Types.Crewmate, "Stalk Cooldown", 10f, 1f, 15f, 1f, HunterOn, format: "s");
            HunterStalkDuration = CustomOption.Create(87, Types.Crewmate, "Stalk Duration", 25f, 5f, 60f, 2.5f, HunterOn, format: "s");
            HunterStalkUses = CustomOption.Create(88, Types.Crewmate, "Maximum Stalk Uses", 5, 1, 15, 1, HunterOn);
            RetributionOnVote = CustomOption.Create(89, Types.Crewmate, "Kills Last Voter If Voted Out", false, HunterOn);
            HunterBodyReport = CustomOption.Create(90, Types.Crewmate, "Can Report Who They've Killed", false, HunterOn);

            JailorOn = CustomOption.Create(91, Types.Crewmate, ColorString(Colors.Jailor, "Jailor"), 0f, 0f, 100f, 10f, null, true, format: "%");
            JailCooldown = CustomOption.Create(92, Types.Crewmate, "Jail Cooldown", 25f, 10f, 60f, 2.5f, HunterOn, format: "s");
            MaxExecutes = CustomOption.Create(93, Types.Crewmate, "Maximum Number Of Executes", 3, 1, 5, 1, HunterOn);

            VeteranOn = CustomOption.Create(94, Types.Crewmate, ColorString(Colors.Veteran, "Veteran"), 0f, 0f, 100f, 10f, null, true, format: "%");
            KilledOnAlert = CustomOption.Create(95, Types.Crewmate, "Can Be Killed On Alert", false, VeteranOn);
            VeteranBodyReport = CustomOption.Create(9511, Types.Crewmate, "Can Report Who They've Killed", false, VeteranOn);
            AlertCooldown = CustomOption.Create(96, Types.Crewmate, "Alert Cooldown", 25f, 10f, 60f, 2.5f, VeteranOn, format: "s");
            AlertDuration = CustomOption.Create(97, Types.Crewmate, "Alert Duration", 10f, 5f, 15f, 1f, VeteranOn, format: "s");
            MaxAlerts = CustomOption.Create(98, Types.Crewmate, "Number Of Alerts", 5, 1, 15, 1, VeteranOn);

            VigilanteOn = CustomOption.Create(99, Types.Crewmate, ColorString(Colors.Vigilante, "Vigilante"), 0f, 0f, 100f, 10f, null, true, format: "%");
            VigilanteKills = CustomOption.Create(100, Types.Crewmate, "Number Of Vigilante Kills", 1, 1, 15, 1, VigilanteOn);
            VigilanteMultiKill = CustomOption.Create(101, Types.Crewmate, "Vigilante Can Kill More Than Once Per Meeting", false, VigilanteOn);
            VigilanteGuessNeutralBenign = CustomOption.Create(102, Types.Crewmate, "Vigilante Can Guess Neutral Benign Roles", false, VigilanteOn);
            VigilanteGuessNeutralEvil = CustomOption.Create(103, Types.Crewmate, "Vigilante Can Guess Neutral Evil Roles", false, VigilanteOn);
            VigilanteGuessNeutralKilling = CustomOption.Create(104, Types.Crewmate, "Vigilante Can Guess Neutral Killing Roles", false, VigilanteOn);
            VigilanteAfterVoting = CustomOption.Create(105, Types.Crewmate, "Vigilante Can Guess After Voting", false, VigilanteOn);
            VigilanteKillOther = CustomOption.Create(106, Types.Crewmate, "Vigilante Miskill Kills Crewmate", false, VigilanteOn);
            VigilanteKillsNeutralEvil = CustomOption.Create(107, Types.Crewmate, "Vigilante Kills Neutral Evil", false, VigilanteOn);
            VigilanteKillsNeutralBenign = CustomOption.Create(108, Types.Crewmate, "Vigilante Kills Neutral Benign", false, VigilanteOn);
            VigilanteKillCd = CustomOption.Create(109, Types.Crewmate, "Vigilante Kill Cooldown", 25f, 10f, 40f, 2.5f, VigilanteOn, format: "s");
            VigilanteBodyReport = CustomOption.Create(110, Types.Crewmate, "Can Report Who They've Killed", false, VigilanteOn);

            EngineerOn = CustomOption.Create(116, Types.Crewmate, ColorString(Colors.Engineer, "Engineer"), 0f, 0f, 100f, 10f, null, true, format: "%");
            MaxFixes = CustomOption.Create(117, Types.Crewmate, "Maximum Number Of Fixes", 5, 1, 15, 1, EngineerOn);

            ImitatorOn = CustomOption.Create(118, Types.Crewmate, ColorString(Colors.Imitator, "Imitator"), 0f, 0f, 100f, 10f, null, true, format: "%");

            CrusaderOn = CustomOption.Create(4011, Types.Crewmate, ColorString(Colors.Crusader, "Crusader"), 0f, 0f, 100f, 10f, null, true, format: "%");
            FortifyCooldown = CustomOption.Create(4111, Types.Crewmate, "Cooldown", 10f, 1f, 15f, 1f, CrusaderOn, format: "s");

            MedicOn = CustomOption.Create(119, Types.Crewmate, ColorString(Colors.Medic, "Medic"), 0f, 0f, 100f, 10f, null, true, format: "%");
            ShowShielded = CustomOption.Create(120, Types.Crewmate, "Show Shielded Player", new[] { "Self", "Medic", "Self+Medic", "Everyone" }, MedicOn);
            WhoGetsNotification = CustomOption.Create(121, Types.Crewmate, "Who Gets Murder Attempt Indicator", new[] { "Medic", "Shielded", "Everyone", "Nobody" }, MedicOn);
            ShieldBreaks = CustomOption.Create(122, Types.Crewmate, "Shield Breaks On Murder Attempt", false, MedicOn);
            MedicReportSwitch = CustomOption.Create(123, Types.Crewmate, "Show Medic Reports", true, MedicOn);
            MedicReportNameDuration = CustomOption.Create(124, Types.Crewmate, "Time Where Medic Will Have Name", 0f, 0f, 60f, 2.5f, MedicOn, format: "s");
            MedicReportColorDuration = CustomOption.Create(125, Types.Crewmate, "Time Where Medic Will Have Color Type", 15f, 0f, 60f, 2.5f, MedicOn, format: "s");

            SwapperOn = CustomOption.Create(126, Types.Crewmate, ColorString(Colors.Swapper, "Swapper"), 0f, 0f, 100f, 10f, null, true, format: "%");
            SwapperButton = CustomOption.Create(127, Types.Crewmate, "Can Button", true, SwapperOn);

            #endregion

            #region Neutral Benign Roles
            AmnesiacOn = CustomOption.Create(165, Types.Neutral, ColorString(Colors.Amnesiac, "Amnesiac"), 0f, 0f, 100f, 10f, null, true, format: "%");

            GuardianAngelOn = CustomOption.Create(168, Types.Neutral, ColorString(Colors.GuardianAngel, "Guardian Angel"), 0f, 0f, 100f, 10f, null, true, format: "%");
            ProtectCd = CustomOption.Create(169, Types.Neutral, "Protect Cooldown", 25f, 10f, 60f, 2.5f, GuardianAngelOn, format: "s");
            ProtectDuration = CustomOption.Create(170, Types.Neutral, "Protect Duration", 10f, 5f, 15f, 1f, GuardianAngelOn, format: "s");
            ProtectKCReset = CustomOption.Create(171, Types.Neutral, "Kill Cooldown Reset When Protected", 2.5f, 0f, 15f, 0.5f, GuardianAngelOn, format: "s");
            MaxProtects = CustomOption.Create(172, Types.Neutral, "Maximum Number Of Protects", 5, 1, 15, 1, GuardianAngelOn);
            ShowProtect = CustomOption.Create(173, Types.Neutral, "Show Protected Player", new[] { "Self", "Guardian Angel", "Self+GA", "Everyone" }, GuardianAngelOn);
            GaOnTargetDeath = CustomOption.Create(174, Types.Neutral, "GA Becomes On Target Dead", new[] { "Crew", "Amnesiac", "Jester"}, GuardianAngelOn);
            GATargetKnows = CustomOption.Create(175, Types.Neutral, "Target Knows GA Exists", false, GuardianAngelOn);
            GAKnowsTargetRole = CustomOption.Create(176, Types.Neutral, "GA Knows Targets Role", false, GuardianAngelOn);
            EvilTargetPercent = CustomOption.Create(177, Types.Neutral, "Evil Target Chance", 0f, 0f, 100f, 10f, GuardianAngelOn, format: "%");

            RomanticOn = CustomOption.Create(178, Types.Neutral, ColorString(Colors.Romantic, "Romantic"), 0f, 0f, 100f, 10f, null, true, format: "%");
            PickStartTimer = CustomOption.Create(179, Types.Neutral, "Pick Cooldown", 25f, 10f, 60f, 2.5f, RomanticOn, format: "s");
            RomanticOnBelovedDeath = CustomOption.Create(180, Types.Neutral, "Romantic Becomes On Beloved Dead", new[] { "Repick Lover","Crew", "Amnesiac", "Jester" }, RomanticOn);
            RomanticBelovedKnows = CustomOption.Create(181, Types.Neutral, "Beloved Knows The Existence Of Romantic", false, RomanticOn);
            RomanticKnowsBelovedRole = CustomOption.Create(182, Types.Neutral, "Romantic Knows Beloved's Role", false, RomanticOn);
            
            #endregion

            #region Neutral Evil Roles

            DoomsayerOn = CustomOption.Create(139, Types.Neutral, ColorString(Colors.Doomsayer, "Doomsayer"), 0f, 0f, 100f, 10f, null, true, format: "%");
            DoomsayerGuessesToWin = CustomOption.Create(147, Types.Neutral, "Number Of Kills To Win", 3, 1, 5, 1, DoomsayerOn);
            ObserveCooldown = CustomOption.Create(140, Types.Neutral, "Cooldown", 25f, 10f, 60f, 2.5f, DoomsayerOn, format: "s");
            DoomsayerGuessNeutralBenign = CustomOption.Create(141, Types.Neutral, "Can Guess Neutral Benign Roles", false, DoomsayerOn);
            DoomsayerGuessNeutralEvil = CustomOption.Create(142, Types.Neutral, "Can Guess Neutral Evil Roles", false, DoomsayerOn);
            DoomsayerGuessNeutralKilling = CustomOption.Create(143, Types.Neutral, "Can Guess Neutral Killing Roles", false, DoomsayerOn);
            DoomsayerGuessNeutralBenign = CustomOption.Create(144, Types.Neutral, "Can Guess Neutral Benign Roles", false, DoomsayerOn);
            DoomsayerGuessImpostors = CustomOption.Create(145, Types.Neutral, "Can Guess Impostor Roles", false, DoomsayerOn);
            DoomsayerAfterVoting = CustomOption.Create(146, Types.Neutral, "Can Guess After Voting", false, DoomsayerOn);
            
            
            ExecutionerOn = CustomOption.Create(148, Types.Neutral,  ColorString(Colors.Executioner, "Executioner"), 0f, 0f, 100f, 10f, null, true, format: "%");
            OnTargetDead = CustomOption.Create(149, Types.Neutral, "Executioner Becomes On Target Dead", new[] { "Crew", "Amnesiac", "Jester" }, ExecutionerOn);
            ExecutionerButton = CustomOption.Create(150, Types.Neutral, "Executioner Can Button", true, ExecutionerOn);

            JesterOn = CustomOption.Create(151, Types.Neutral, ColorString(Colors.Jester, "Jester"), 0f, 0f, 100f, 10f, null, true, format: "%");
            JesterButton = CustomOption.Create(152, Types.Neutral, "Can Button", true, JesterOn);
            JesterVent = CustomOption.Create(153, Types.Neutral, "Can Hide In Vents", false, JesterOn);
            JesterVentSwitch = CustomOption.Create(154, Types.Neutral, "Can Switch Between Vents", false, JesterVent);
            JesterImpVision = CustomOption.Create(155, Types.Neutral, "Has Impostor Vision", false, JesterOn);

            VultureOn = CustomOption.Create(158, Types.Neutral, ColorString(Colors.Vulture, "Vulture"), 0f, 0f, 100f, 10f, null, true, format: "%");
            VultureCd = CustomOption.Create(159, Types.Neutral, "Cooldown", 10f, 10f, 60f, 2.5f, VultureOn, format: "s");
            VultureBodyCount = CustomOption.Create(160, Types.Neutral, "Number Of Bodies To Eat", 1, 1, 5, 1, VultureOn);
            VultureVent = CustomOption.Create(161, Types.Neutral, "Can Vent", false, VultureOn);
            VultureImpVision = CustomOption.Create(162, Types.Neutral, "Has Impostor Vision", false, VultureOn);
            EatArrows = CustomOption.Create(163, Types.Neutral, "Gets Arrows To Dead Bodies", false, VultureOn);
            EatArrowDelay = CustomOption.Create(164, Types.Neutral, "Time After Death Arrows Appear", 5f, 0f, 15f, 1f, EatArrows, format: "s");

            #endregion

            #region Neutral Killing Roles
            ArsonistOn = CustomOption.Create(183, Types.NK, ColorString(Colors.Arsonist, "Arsonist"), 0f, 0f, 100f, 10f, null, true, format: "%");
            DouseCooldown = CustomOption.Create(184, Types.NK, "Douse Cooldown", 25f, 10f, 60f, 2.5f, ArsonistOn, format: "s");
            MaxDoused = CustomOption.Create(185, Types.NK, "Maximum Alive Players Doused", 5, 1, 15, 1, ArsonistOn);
            IgniteCdRemoved = CustomOption.Create(186, Types.NK, "Remove Ignite Cooldown If Last Killer", false, ArsonistOn);
            ArsoVent = CustomOption.Create(18612, Types.NK, "Can Vent", false, ArsonistOn);

            JuggernautOn = CustomOption.Create(187, Types.NK, ColorString(Colors.Juggernaut, "Juggernaut"), 0f, 0f, 100f, 10f, null, true, format: "%");
            JuggKillCooldown = CustomOption.Create(188, Types.NK, "Initial Cooldown", 25f, 10f, 60f, 2.5f, JuggernautOn, format: "s");
            ReducedKCdPerKill = CustomOption.Create(189, Types.NK, "Reduced Cooldown Per Kill", 5f, 2.5f, 10f, 2.5f, JuggernautOn, format: "s");
            JuggVent = CustomOption.Create(190, Types.NK, "Can Vent", false, JuggernautOn);

            GlitchOn = CustomOption.Create(191, Types.NK, ColorString(Colors.Glitch, "Glitch"), 0f, 0f, 100f, 10f, null, true, format: "%");
            MimicCooldownOption = CustomOption.Create(192, Types.NK, "Mimic Cooldown", 25f, 10f, 60f, 2.5f, GlitchOn, format: "s");
            MimicDurationOption = CustomOption.Create(193, Types.NK, "Mimic Duration", 10f, 1f, 15f, 1f, GlitchOn, format: "s");
            HackCooldownOption = CustomOption.Create(194, Types.NK, "Hack Cooldown", 25f, 10f, 60f, 2.5f, GlitchOn, format: "s");
            HackDurationOption = CustomOption.Create(195, Types.NK, "Hack Duration", 10f, 1f, 15f, 1f, GlitchOn, format: "s");
            GlitchKillCooldownOption = CustomOption.Create(196, Types.NK, "Kill Cooldown", 25f, 10f, 120f, 2.5f, GlitchOn, format: "s");
            GlitchHackDistanceOption = CustomOption.Create(197, Types.NK, "Hack Distance", new[] { "Short", "Normal", "Long" }, GlitchOn);
            GlitchVent = CustomOption.Create(198, Types.NK, "Can Vent", false, GlitchOn);

            PlaguebearerOn = CustomOption.Create(199, Types.NK, ColorString(Colors.Plaguebearer, "Plaguebearer"), 0f, 0f, 100f, 10f, null, true, format: "%");
            InfectCooldown = CustomOption.Create(200, Types.NK, "Infect Cooldown", 25f, 10f, 60f, 2.5f, PlaguebearerOn, format: "s");
            PestKillCooldown = CustomOption.Create(1833, Types.NK, "Pestilence Kill Cooldown", 25f, 10f, 60f, 2.5f, PlaguebearerOn, format: "s");
            PestVent = CustomOption.Create(202, Types.NK, "Pestilence Can Vent", false, PlaguebearerOn);

            AgentOn = CustomOption.Create(203, Types.NK, ColorString(Colors.Agent, "Agent"), 0f, 0f, 100f, 10f, null, true, format: "%");
            SkipAgent = CustomOption.Create(204, Types.NK, ColorString(Colors.Hitman, "Hitman") + " Spawns Without Agent", false, AgentOn);
            HitmanMorphCooldown = CustomOption.Create(205, Types.NK, "Morph Cooldown", 25f, 10f, 60f, 2.5f, AgentOn, format: "s");
            HitmanMorphDuration = CustomOption.Create(206, Types.NK, "Morph Duration", 10f, 1f, 15f, 1f, AgentOn, format: "s");
            HitmanKillCooldown = CustomOption.Create(207, Types.NK, "Kill Cooldown", 25f, 10f, 120f, 2.5f, AgentOn, format: "s");
            HitmanDragCooldown = CustomOption.Create(208, Types.NK, "Drag Cooldown", 25f, 10f, 60f, 2.5f, AgentOn, format: "s");
            HitmanDragSpeed = CustomOption.Create(209, Types.NK, "Drag Speed", 0.75f, 0.25f, 1f, 0.05f, AgentOn, format: "x");
            HitmanVent = CustomOption.Create(210, Types.NK, "Can Vent", false, AgentOn);
            HitmanVentWithBody = CustomOption.Create(211, Types.NK, "Can Vent While Dragging", false, AgentOn);

            VampireOn = CustomOption.Create(212, Types.NK, ColorString(Colors.Vampire, "Vampire"), 0f, 0f, 100f, 10f, null, true, format: "%");
            BiteCooldown = CustomOption.Create(213, Types.NK, "Cooldown", 25f, 10f, 60f, 2.5f, VampireOn, format: "s");
            VampVent = CustomOption.Create(214, Types.NK, "Can Vent", false, VampireOn);
            NewVampCanAssassin = CustomOption.Create(215, Types.NK, "New Vampire Can Assassinate", false, VampireOn);
            MaxVampiresPerGame = CustomOption.Create(216, Types.NK, "Maximum Vampies Per Game", 2, 2, 5, 1, VampireOn);
            CanBiteNeutralBenign = CustomOption.Create(217, Types.NK, "Can Convert Neutral Benign Roles", false, VampireOn);
            CanBiteNeutralEvil = CustomOption.Create(218, Types.NK, "Can Convert Neutral Evil Roles", false, VampireOn);
            CanBiteNeutralBenign = CustomOption.Create(219, Types.NK, "Can Convert Neutral Benign Roles", false, VampireOn);
            
            SerialKillerOn = CustomOption.Create(220, Types.NK, ColorString(Colors.SerialKiller, "Serial Killer"), 0f, 0f, 100f, 10f, null, true, format: "%");
            StabCooldown = CustomOption.Create(221, Types.NK, "Stab Cooldown", 25f, 10f, 60f, 2.5f, SerialKillerOn, format: "s");
            Stabeduration = CustomOption.Create(222, Types.NK, "Stab Duration", 25f, 10f, 60f, 2.5f, SerialKillerOn, format: "s");
            StabKillCooldown = CustomOption.Create(223, Types.NK, "Stab Cooldown", 10f, 0.5f, 15f, 0.5f, SerialKillerOn, format: "s");
            SerialKillerVent = CustomOption.Create(224, Types.NK, "Can Vent When Stab Is Active", false, SerialKillerOn);

            WerewolfOn = CustomOption.Create(225, Types.NK, ColorString(Colors.Werewolf, "Werewolf"), 0f, 0f, 100f, 10f, null, true, format: "%");
            MaulCooldown = CustomOption.Create(226, Types.NK, "Cooldown", 30f, 10f, 60f, 2.5f, WerewolfOn, format: "s");
            MaulRadius = CustomOption.Create(227, Types.NK, "Maul Radius", 0.25f, 0.05f, 1f, 0.05f, WerewolfOn, format: "x");
            WerewolfVent = CustomOption.Create(228, Types.NK, "Can Vent", false, WerewolfOn);

            #endregion

            #region Impostor Roles

            EscapistOn = CustomOption.Create(229, Types.Impostor, ColorString(Colors.Impostor, "Escapist"), 0f, 0f, 100f, 10f, null, true, format: "%");
            EscapeCooldown = CustomOption.Create(230, Types.Impostor, "Recall Cooldown", 25f, 10f, 60f, 2.5f, EscapistOn, format: "s");

            GrenadierOn = CustomOption.Create(231, Types.Impostor, ColorString(Colors.Impostor, "Grenadier"), 0f, 0f, 100f, 10f, null, true, format: "%");
            GrenadeCooldown = CustomOption.Create(232, Types.Impostor, "Flash Grenade Cooldown", 25f, 10f, 60f, 2.5f, GrenadierOn, format: "s");
            GrenadeDuration = CustomOption.Create(233, Types.Impostor, "Flash Grenade Duration", 10f, 5f, 15f, 1f, GrenadierOn, format: "s");
            FlashRadius = CustomOption.Create(234, Types.Impostor, "Flash Radius", 1f, 0.25f, 5f, 0.25f, GrenadierOn, format: "x");
            GrenadierIndicators = CustomOption.Create(235, Types.Impostor, "Indicate Flashed Players", false, GrenadierOn);

            MorphlingOn = CustomOption.Create(236, Types.Impostor, ColorString(Colors.Impostor, "Morphling"), 0f, 0f, 100f, 10f, null, true, format: "%");
            MorphlingCooldown = CustomOption.Create(237, Types.Impostor, "Morphling Cooldown", 25f, 10f, 60f, 2.5f, MorphlingOn, format: "s");
            MorphlingDuration = CustomOption.Create(238, Types.Impostor, "Morphling Duration", 10f, 5f, 15f, 1f, MorphlingOn, format: "s");

            SwooperOn = CustomOption.Create(239, Types.Impostor, ColorString(Colors.Impostor, "Swooper"), 0f, 0f, 100f, 10f, null, true, format: "%");
            SwoopCooldown = CustomOption.Create(241, Types.Impostor, "Swoop Cooldown", 25f, 10f, 60f, 2.5f, SwooperOn, format: "s");
            SwoopDuration = CustomOption.Create(242, Types.Impostor, "Swoop Duration", 10f, 5f, 15f, 1f, SwooperOn, format: "s");

            VenererOn = CustomOption.Create(244, Types.Impostor, ColorString(Colors.Impostor, "Venerer"), 0f, 0f, 100f, 10f, null, true, format: "%");
            AbilityCooldown = CustomOption.Create(245, Types.Impostor, "Ability Cooldown", 25f, 10f, 60f, 2.5f, VenererOn, format: "s");
            AbilityDuration = CustomOption.Create(246, Types.Impostor, "Ability Duration", 10f, 5f, 15f, 1f, VenererOn, format: "s");
            SprintSpeed = CustomOption.Create(247, Types.Impostor, "Sprint Speed", 1.25f, 1.05f, 2.5f, 0.05f, VenererOn, format: "x");
            FreezeSpeed = CustomOption.Create(248, Types.Impostor, "Freeze Speed", 0.75f, 0.25f, 1f, 0.05f, VenererOn, format: "x");

            PoisonerOn = CustomOption.Create(24422, Types.Impostor, ColorString(Colors.Impostor, "Poisoner"), 0f, 0f, 100f, 10f, null, true, format: "%");
            PoisonCooldown = CustomOption.Create(24522, Types.Impostor, "Cooldown", 25f, 10f, 60f, 2.5f, PoisonerOn, format: "s");
            PoisonDelay = CustomOption.Create(24622, Types.Impostor, "Delay", 5f, 1f, 15f, 1f, PoisonerOn, format: "s");

            BomberOn = CustomOption.Create(249, Types.Impostor, ColorString(Colors.Impostor, "Bomber"), 0f, 0f, 100f, 10f, null, true, format: "%");
            DetonateDelay = CustomOption.Create(250, Types.Impostor, "Detonate Delay", 5f, 1f, 15f, 1f, BomberOn, format: "s");
            MaxKillsInDetonation = CustomOption.Create(251, Types.Impostor, "Max Kills In Detonation", 5, 1, 15, 1, BomberOn);
            DetonateRadius = CustomOption.Create(252, Types.Impostor, "Detonate Radius", 0.25f, 0.05f, 1f, 0.05f, BomberOn, format: "x");

            WarlockOn = CustomOption.Create(253, Types.Impostor, ColorString(Colors.Impostor, "Warlock"), 0f, 0f, 100f, 10f, null, true, format: "%");
            ChargeUpDuration = CustomOption.Create(254, Types.Impostor, "Time It Takes To Fully Charge", 25f, 10f, 60f, 2.5f, WarlockOn, format: "s");
            ChargeUseDuration = CustomOption.Create(255, Types.Impostor, "Time It Takes To Use Full Charge", 1f, 0.05f, 5f, 0.05f, WarlockOn, format: "s");

            WitchOn = CustomOption.Create(256, Types.Impostor, ColorString(Colors.Impostor, "Witch"), 0f, 0f, 100f, 10f, null, true, format: "%");
            SpellCd = CustomOption.Create(257, Types.Impostor, "Spell Cooldown", 25f, 10f, 60f, 2.5f, WitchOn, format: "s");
            
            BlackmailerOn = CustomOption.Create(258, Types.Impostor, ColorString(Colors.Impostor, "Blackmailer"), 0f, 0f, 100f, 10f, null, true, format: "%");
            BlackmailCooldown = CustomOption.Create(259, Types.Impostor, "Initial Blackmail Cooldown", 10f, 1f, 15f, 1f, BlackmailerOn, format: "s");
            BlackmailInvisible = CustomOption.Create(260, Types.Impostor, "Only Target Sees Blackmail", true, BlackmailerOn);

            JanitorOn = CustomOption.Create(261, Types.Impostor, ColorString(Colors.Impostor, "Janitor"), 0f, 0f, 100f, 10f, null, true, format: "%");

            MinerOn = CustomOption.Create(262, Types.Impostor, ColorString(Colors.Impostor, "Miner"), 0f, 0f, 100f, 10f, null, true, format: "%");
            MineCooldown = CustomOption.Create(262, Types.Impostor, "Mine Cooldown", 25f, 10f, 60f, 2.5f, MinerOn, format: "s");

            UndertakerOn = CustomOption.Create(263, Types.Impostor, ColorString(Colors.Impostor, "Undertaker"), 0f, 0f, 100f, 10f, null, true, format: "%");
            DragCooldown = CustomOption.Create(264, Types.Impostor, "Cooldown", 25f, 10f, 60f, 2.5f, UndertakerOn, format: "s");
            UndertakerDragSpeed = CustomOption.Create(265, Types.Impostor, "Undertaker Drag Speed", 0.75f, 0.25f, 1f, 0.05f, UndertakerOn, format: "x");
            UndertakerVentWithBody = CustomOption.Create(266, Types.Impostor, "Undertaker Can Vent While Dragging", false, UndertakerOn);
            
            #endregion

            #region Modifiers
            AftermathOn = CustomOption.Create(267, Types.ModifierAbility, ColorString(Colors.Aftermath, "Aftermath"), 0f, 0f, 100f, 10f, null, true, format: "%");

            BaitOn = CustomOption.Create(268, Types.ModifierAbility, ColorString(Colors.Bait, "Bait"), 0f, 0f, 100f, 10f, null, true, format: "%");
            BaitMinDelay = CustomOption.Create(269, Types.ModifierAbility, "Minimum Delay for the Bait Report", 0f, 0f, 15f, 0.5f, BaitOn, format: "s");
            BaitMaxDelay = CustomOption.Create(270, Types.ModifierAbility, "Maximum Delay for the Bait Report", 1f, 0f, 15f, 0.5f, BaitOn, format: "s");

            DiseasedOn = CustomOption.Create(2701, Types.ModifierAbility, ColorString(Colors.Diseased, "Diseased"), 0f, 0f, 100f, 10f, null, true, format: "%");
            DiseasedKillMultiplier = CustomOption.Create(271, Types.ModifierAbility, "Kill Multiplier", 3f, 1.5f, 5f, 0.5f, DiseasedOn, format: "x");

            FrostyOn = CustomOption.Create(272, Types.ModifierAbility, ColorString(Colors.Frosty, "Frosty"), 0f, 0f, 100f, 10f, null, true, format: "%");
            ChillDuration = CustomOption.Create(273, Types.ModifierAbility, "Chill Duration", 10f, 1f, 15f, 1f, FrostyOn, format: "s");
            ChillStartSpeed = CustomOption.Create(274, Types.ModifierAbility, "Chill Start Speed", 0.75f, 0.25f, 0.95f, 0.05f, FrostyOn, format: "x");
            #endregion

            #region Impostor Modifiers

            DoubleShotOn = CustomOption.Create(305, Types.ModifierAbility, ColorString(Colors.Impostor, "Double Shot"), 0f, 0f, 100f, 10f, null, true, format: "%");
            
            UnderdogOn = CustomOption.Create(306, Types.ModifierAbility, ColorString(Colors.Impostor, "Underdog"), 0f, 0f, 100f, 10f, null, true, format: "%");
            UnderdogKillBonus = CustomOption.Create(307, Types.ModifierAbility, "Kill Cooldown Bonus", 5f, 2.5f, 10f, 2.5f, UnderdogOn, format: "s");
            UnderdogIncreasedKC = CustomOption.Create(308, Types.ModifierAbility, "Increased Kill Cooldown When 2+ Imps", true, UnderdogOn);

            #endregion     

            #region Abilities

            NumberOfImpostorAssassins = CustomOption.Create(276, Types.ModifierAbility, "Number Of Impostor Assassins", 1, 0, 5, 1, null, true, heading: ColorString(Colors.Impostor, "Assassin"));
            NumberOfNeutralAssassins = CustomOption.Create(277, Types.ModifierAbility, "Number Of Neutral Killer Assassins", 1, 0, 5, 1);
            AmneTurnImpAssassin = CustomOption.Create(278, Types.ModifierAbility, "Amnesiac Turned Impostor Gets Ability", false);
            AmneTurnNeutAssassin = CustomOption.Create(279, Types.ModifierAbility, "Amnesiac Turned Neutral Killer Gets Ability", false);
            AssassinKills = CustomOption.Create(280, Types.ModifierAbility, "Number Of Kills", 1, 1, 15, 1);
            AssassinMultiKill = CustomOption.Create(281, Types.ModifierAbility, "Can Kill More Than Once Per Meeting", false);
            AssassinCrewmateGuess = CustomOption.Create(282, Types.ModifierAbility, "Can Guess \"Crewmate\"", false);
            AssassinGuessNeutralBenign = CustomOption.Create(283, Types.ModifierAbility, "Can Guess Neutral Benign Roles", false);
            AssassinGuessNeutralEvil = CustomOption.Create(284, Types.ModifierAbility, "Can Guess Neutral Evil Roles", false);

            ButtonBarryOn = CustomOption.Create(285, Types.ModifierAbility, ColorString(Colors.ButtonBarry, "Button Barry"), 0f, 0f, 100f, 10f, null, true, format: "%");

            DrunkOn = CustomOption.Create(290, Types.ModifierAbility, ColorString(Colors.Drunk, "Drunk"), 0f, 0f, 100f, 10f, null, true, format: "%");
            
            FlashOn = CustomOption.Create(291, Types.ModifierAbility, ColorString(Colors.Flash, "Flash"), 0f, 0f, 100f, 10f, null, true, format: "%");
            FlashSpeed = CustomOption.Create(292, Types.ModifierAbility, "Speed Multiplier", 1.25f, 1.05f, 2.5f, 0.05f, FlashOn, format: "x");

            GiantOn = CustomOption.Create(293, Types.ModifierAbility, ColorString(Colors.Giant, "Giant"), 0f, 0f, 100f, 10f, null, true, format: "%");
            GiantSlow = CustomOption.Create(294, Types.ModifierAbility, "Speed Multiplier", 0.75f, 0.25f, 1f, 0.05f, GiantOn, format: "x");

            MiniOn = CustomOption.Create(2933, Types.ModifierAbility, ColorString(Colors.Mini, "Mini"), 0f, 0f, 100f, 10f, null, true, format: "%");
            MiniSpeed = CustomOption.Create(2944, Types.ModifierAbility, "Speed Multiplier", 1.25f, 1.05f, 2.5f, 0.05f, MiniOn, format: "x");

            MultitaskerOn = CustomOption.Create(295, Types.ModifierAbility, ColorString(Colors.Multitasker, "Multitasker"), 0f, 0f, 100f, 10f, null, true, format: "%");

            ParanoiacOn = CustomOption.Create(296, Types.ModifierAbility, ColorString(Colors.Paranoiac, "Paranoiac"), 0f, 0f, 100f, 10f, null, true, format: "%");
            
            SleuthOn = CustomOption.Create(297, Types.ModifierAbility, ColorString(Colors.Sleuth, "Sleuth"), 0f, 0f, 100f, 10f, null, true, format: "%");
            
            SpyOn = CustomOption.Create(298, Types.ModifierAbility, ColorString(Colors.Spy, "Spy"), 0f, 0f, 100f, 10f, null, true, format: "%");
            WhoSeesDead = CustomOption.Create(299, Types.ModifierAbility, "Who Sees Dead Bodies On Admin", new[] { "Nobody", "Spy", "Everyone But Spy", "Everyone" }, SpyOn);
            
            TiebreakerOn = CustomOption.Create(300, Types.ModifierAbility, ColorString(Colors.Tiebreaker, "Tie breaker"), 0f, 0f, 100f, 10f, null, true, format: "%");
            
            TorchOn = CustomOption.Create(301, Types.ModifierAbility, ColorString(Colors.Torch, "Torch"), 0f, 0f, 100f, 10f, null, true, format: "%");
            
            #endregion

            #region Impostor Abilities

            DisperserOn = CustomOption.Create(302, Types.ModifierAbility, ColorString(Colors.Impostor, "Disperser"), 0f, 0f, 100f, 10f, null, true, format: "%");
            DisperseCooldown = CustomOption.Create(303, Types.ModifierAbility, "Cooldown", 25f, 10f, 40f, 2.5f, DisperserOn, format: "s");
            MaxDisperses = CustomOption.Create(304, Types.ModifierAbility, "Maximum Number Of Disperses Per Game", 5, 1, 15, 1, DisperserOn);
            
            #endregion
        }
    }
}