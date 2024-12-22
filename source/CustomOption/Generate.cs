namespace TownOfSushi.CustomOption
{
    public class Generate
    {
        #region Header Options
        public static CustomHeaderOption CrewSpecialRoles;
        public static CustomHeaderOption CrewKillingSettings;
        public static CustomHeaderOption CrewSupportSettings;
        public static CustomHeaderOption CrewInvestSettings;
        public static CustomHeaderOption NeutralKillingRoles;
        public static CustomHeaderOption NeutralBenignRoles;
        public static CustomHeaderOption NeutralEvilRoles;
        public static CustomHeaderOption ImpostorDeceptionRoles;
        public static CustomHeaderOption ImpostorSupportRoles;

        #endregion

        #region Crewmate Roles

        public static CustomNumberOption HaunterOn;
        public static CustomHeaderOption Haunter;
        public static CustomNumberOption HaunterTasksRemainingClicked;
        public static CustomNumberOption HaunterTasksRemainingAlert;
        public static CustomToggleOption HaunterRevealsNeutrals;
        public static CustomStringOption HaunterCanBeClickedBy;

        public static CustomNumberOption VigilanteKills;
        public static CustomToggleOption VigilanteMultiKill;
        public static CustomToggleOption VigilanteGuessNeutralBenign;
        public static CustomToggleOption VigilanteGuessNeutralEvil;
        public static CustomToggleOption VigilanteGuessNeutralKilling;
        public static CustomToggleOption VigilanteAfterVoting;      

        public static CustomNumberOption MysticOn;
        public static CustomHeaderOption Mystic;
        public static CustomNumberOption MysticArrowDuration;
        public static CustomNumberOption InitialExamineCooldown;
        public static CustomNumberOption MysticExamineCooldown;
        public static CustomNumberOption RecentKill;
        public static CustomToggleOption MysticReportOn;
        public static CustomNumberOption MysticRoleDuration;
        public static CustomNumberOption MysticFactionDuration;
        public static CustomToggleOption MysticExamineReportOn;

        public static CustomNumberOption OracleOn;
        public static CustomHeaderOption Oracle;
        public static CustomNumberOption ConfessCooldown;
        public static CustomNumberOption RevealAccuracy;
        public static CustomToggleOption NeutralBenignShowsEvil;
        public static CustomToggleOption NeutralEvilShowsEvil;
        public static CustomToggleOption NeutralKillingShowsEvil;

        public static CustomNumberOption SeerOn;
        public static CustomHeaderOption Seer;
        public static CustomNumberOption SeerCooldown;
        public static CustomToggleOption CrewKillingRed;
        public static CustomToggleOption NeutBenignRed;
        public static CustomToggleOption NeutEvilRed;
        public static CustomToggleOption NeutKillingRed;

        public static CustomHeaderOption Hunter;
        public static CustomNumberOption HunterOn;
        public static CustomNumberOption HunterKillCd;
        public static CustomNumberOption HunterStalkCd;
        public static CustomNumberOption HunterStalkDuration;
        public static CustomNumberOption HunterStalkUses;
        public static CustomToggleOption RetributionOnVote;
        public static CustomToggleOption HunterBodyReport;

        public static CustomNumberOption SnitchOn;
        public static CustomHeaderOption Snitch;
        public static CustomNumberOption SnitchTasksRemaining;
        public static CustomToggleOption SnitchButton;

        
        public static CustomNumberOption TrackerOn;
        public static CustomHeaderOption Tracker;
        public static CustomNumberOption UpdateInterval;
        public static CustomNumberOption TrackCooldown;
        public static CustomToggleOption ResetOnNewRound;
        public static CustomNumberOption MaxTracks;

        public static CustomNumberOption TrapperOn;
        public static CustomHeaderOption Trapper;
        public static CustomNumberOption TrapCooldown;
        public static CustomToggleOption TrapsRemoveOnNewRound;
        public static CustomNumberOption MaxTraps;
        public static CustomNumberOption MinAmountOfTimeInTrap;
        public static CustomNumberOption TrapSize;
        public static CustomNumberOption MinAmountOfPlayersInTrap;

        public static CustomNumberOption MedicOn;
        public static CustomHeaderOption Medic;
        public static CustomStringOption ShowShielded;
        public static CustomStringOption WhoGetsNotification;
        public static CustomToggleOption ShieldBreaks;
        public static CustomToggleOption MedicReportSwitch;
        public static CustomNumberOption MedicReportNameDuration;
        public static CustomNumberOption MedicReportColorDuration;

        public static CustomNumberOption SwapperOn;
        public static CustomHeaderOption Swapper;
        public static CustomToggleOption SwapperButton;

        public static CustomNumberOption VeteranOn;
        public static CustomNumberOption VigilanteOn;
        public static CustomNumberOption JailorOn;

        public static CustomHeaderOption Vigilante;
        public static CustomHeaderOption Jailor;
        public static CustomNumberOption JailCooldown;
        public static CustomNumberOption MaxExecutes;

        public static CustomNumberOption ImitatorOn;
        public static CustomHeaderOption Imitator;

        public static CustomNumberOption MediumOn;
        public static CustomNumberOption TransporterOn;

        public static CustomToggleOption VigilanteKillOther;
        public static CustomToggleOption VigilanteKillsNeutralEvil;
        public static CustomToggleOption VigilanteKillsNeutralBenign;
        public static CustomNumberOption VigilanteKillCd;
        public static CustomToggleOption VigilanteBodyReport;

        public static CustomHeaderOption Engineer;
        public static CustomNumberOption EngineerOn;
        public static CustomNumberOption MaxFixes;

        public static CustomHeaderOption Investigator;
        public static CustomNumberOption InvestigatorOn;
        public static CustomNumberOption ExamineCooldown;
        public static CustomToggleOption InvestigatorReportOn;
        public static CustomNumberOption InvestigatorRoleDuration;
        public static CustomNumberOption InvestigatorFactionDuration;
        public static CustomNumberOption FootprintSize;
        public static CustomNumberOption FootprintInterval;
        public static CustomNumberOption FootprintDuration;
        public static CustomToggleOption AnonymousFootPrint;
        public static CustomToggleOption VentFootprintVisible;

        #endregion    

        #region Neutral Killing Roles
        public static CustomNumberOption AgentOn;
        public static CustomToggleOption HitmanVent;
        public static CustomToggleOption SkipAgent;
        public static CustomToggleOption HitmanVentWithBody;
        public static CustomHeaderOption Hitman;
        public static CustomNumberOption HitmanKillCooldown;
        public static CustomNumberOption HitmanDragCooldown;
        public static CustomNumberOption HitmanDragSpeed;
        public static CustomNumberOption HitmanMorphDuration;
        public static CustomNumberOption HitmanMorphCooldown;

        public static CustomNumberOption ArsonistOn;
        public static CustomHeaderOption Arsonist;
        public static CustomNumberOption DouseCooldown;
        public static CustomNumberOption MaxDoused;
        public static CustomToggleOption IgniteCdRemoved;

        public static CustomNumberOption PlaguebearerOn;
        public static CustomHeaderOption Plaguebearer;
        public static CustomNumberOption InfectCooldown;
        public static CustomNumberOption PestKillCooldown;
        public static CustomToggleOption PestVent;

        public static CustomNumberOption JuggernautOn;
        public static CustomHeaderOption Juggernaut;
        public static CustomNumberOption JuggKillCooldown;
        public static CustomNumberOption ReducedKCdPerKill;
        public static CustomToggleOption JuggVent;

        public static CustomNumberOption GlitchOn;
        public static CustomHeaderOption TheGlitch;
        public static CustomNumberOption MimicCooldownOption;
        public static CustomNumberOption MimicDurationOption;
        public static CustomNumberOption HackCooldownOption;
        public static CustomNumberOption HackDurationOption;
        public static CustomNumberOption GlitchKillCooldownOption;
        public static CustomStringOption GlitchHackDistanceOption;
        public static CustomToggleOption GlitchVent;

        public static CustomNumberOption VampireOn;
        public static CustomHeaderOption Vampire;
        public static CustomNumberOption BiteCooldown;
        public static CustomToggleOption VampVent;
        public static CustomToggleOption NewVampCanAssassin;
        public static CustomNumberOption MaxVampiresPerGame;
        public static CustomToggleOption CanBiteNeutralBenign;
        public static CustomToggleOption CanBiteNeutralEvil;

        public static CustomNumberOption SerialKillerOn;
        public static CustomHeaderOption SerialKiller;
        public static CustomNumberOption StabCooldown;
        public static CustomNumberOption Stabeduration;
        public static CustomNumberOption StabKillCooldown;
        public static CustomToggleOption SerialKillerVent;

        #endregion

        #region Impostor Roles
        public static CustomNumberOption EscapistOn;
        public static CustomHeaderOption Escapist;
        public static CustomNumberOption EscapeCooldown;
        public static CustomToggleOption EscapistVent;

        public static CustomNumberOption MorphlingOn;
        public static CustomHeaderOption Morphling;
        public static CustomNumberOption MorphlingCooldown;
        public static CustomNumberOption MorphlingDuration;
        public static CustomToggleOption MorphlingVent;

        public static CustomNumberOption SwooperOn;
        public static CustomHeaderOption Swooper;
        public static CustomNumberOption SwoopCooldown;
        public static CustomNumberOption SwoopDuration;
        public static CustomToggleOption SwooperVent;

        public static CustomNumberOption GrenadierOn;
        public static CustomHeaderOption Grenadier;
        public static CustomNumberOption GrenadeCooldown;
        public static CustomNumberOption GrenadeDuration;
        public static CustomToggleOption GrenadierIndicators;
        public static CustomToggleOption GrenadierVent;
        public static CustomNumberOption FlashRadius;

        public static CustomNumberOption VenererOn;
        public static CustomHeaderOption Venerer;
        public static CustomNumberOption AbilityCooldown;
        public static CustomNumberOption AbilityDuration;
        public static CustomNumberOption SprintSpeed;
        public static CustomNumberOption FreezeSpeed;

        public static CustomHeaderOption ImpostorPower;
        public static CustomNumberOption BomberOn;
        public static CustomHeaderOption Bomber;
        public static CustomNumberOption MaxKillsInDetonation;
        public static CustomNumberOption DetonateDelay;
        public static CustomNumberOption DetonateRadius;
        public static CustomToggleOption BomberVent;

        public static CustomNumberOption WarlockOn;
        public static CustomHeaderOption Warlock;
        public static CustomNumberOption ChargeUpDuration;
        public static CustomNumberOption ChargeUseDuration;

        public static CustomNumberOption WitchOn;
        public static CustomHeaderOption Witch;
        public static CustomNumberOption SpellCd;

        public static CustomNumberOption BlackmailerOn;
        public static CustomHeaderOption Blackmailer;
        public static CustomNumberOption BlackmailCooldown;
        public static CustomToggleOption BlackmailInvisible;

        public static CustomNumberOption JanitorOn;
         public static CustomHeaderOption Janitor;

        public static CustomNumberOption MinerOn;
        public static CustomHeaderOption Miner;
        public static CustomNumberOption MineCooldown;

        public static CustomNumberOption UndertakerOn;
        public static CustomHeaderOption Undertaker;
        public static CustomNumberOption DragCooldown;
        public static CustomNumberOption UndertakerDragSpeed;
        public static CustomToggleOption UndertakerVent;
        public static CustomToggleOption UndertakerVentWithBody;

        #endregion

        #region Modifiers
        public static CustomHeaderOption Modifiers;

        public static CustomNumberOption AftermathOn;
        public static CustomHeaderOption Aftermath;

        public static CustomNumberOption BaitOn;
        public static CustomHeaderOption Bait;
        public static CustomNumberOption BaitMinDelay;
        public static CustomNumberOption BaitMaxDelay;

        public static CustomNumberOption DiseasedOn;
        public static CustomHeaderOption Diseased;
        public static CustomNumberOption DiseasedKillMultiplier;

        public static CustomNumberOption FrostyOn;
        public static CustomHeaderOption Frosty;
        public static CustomNumberOption ChillDuration;
        public static CustomNumberOption ChillStartSpeed;

        public static CustomNumberOption MultitaskerOn;
        public static CustomHeaderOption Multitasker;

        #endregion

        #region Abilities
        public static CustomHeaderOption Abilities;

        public static CustomHeaderOption Assassin;
        public static CustomNumberOption NumberOfImpostorAssassins;
        public static CustomNumberOption NumberOfNeutralAssassins;
        public static CustomToggleOption AmneTurnImpAssassin;
        public static CustomToggleOption AmneTurnNeutAssassin;
        public static CustomNumberOption AssassinKills;
        public static CustomToggleOption AssassinMultiKill;
        public static CustomToggleOption AssassinCrewmateGuess;
        public static CustomToggleOption AssassinGuessNeutralBenign;
        public static CustomToggleOption AssassinGuessNeutralEvil;

        public static CustomNumberOption ButtonBarryOn;
        public static CustomHeaderOption ButtonBarry;

        public static CustomNumberOption ChameleonOn;
        public static CustomHeaderOption Chameleon;
        public static CustomNumberOption InvisDelay;
        public static CustomNumberOption TransformInvisDuration;
        public static CustomNumberOption FinalTransparency;

        public static CustomNumberOption FlashOn;
        public static CustomHeaderOption Flash;
        public static CustomNumberOption FlashSpeed;

        public static CustomNumberOption GiantOn;
        public static CustomHeaderOption Giant;
        public static CustomNumberOption GiantSlow;

        public static CustomNumberOption RadarOn;
        public static CustomHeaderOption Radar;

        public static CustomNumberOption SleuthOn;
        public static CustomHeaderOption Sleuth;

        public static CustomNumberOption TorchOn;
        public static CustomHeaderOption Torch;

        public static CustomNumberOption DrunkOn;
        public static CustomHeaderOption Drunk;

        public static CustomNumberOption TiebreakerOn;
        public static CustomHeaderOption Tiebreaker;

        #endregion

        #region Impostor Modifiers
        public static CustomHeaderOption ImpostorModifiers;
        public static CustomNumberOption DisperserOn;
        public static CustomHeaderOption Disperser;
        public static CustomNumberOption DisperseCooldown;
        public static CustomNumberOption MaxDisperses;

        public static CustomNumberOption SpyOn;
        public static CustomHeaderOption Spy;
        public static CustomStringOption WhoSeesDead;

        public static CustomNumberOption DoubleShotOn;
        public static CustomHeaderOption DoubleShot;

        public static CustomNumberOption UnderdogOn;
        public static CustomHeaderOption Underdog;
        public static CustomNumberOption UnderdogKillBonus;
        public static CustomToggleOption UnderdogIncreasedKC;

        #endregion
        public static CustomHeaderOption Transporter;
        public static CustomNumberOption TransportCooldown;
        public static CustomNumberOption TransportMaxUses;
        public static CustomToggleOption TransporterVitals;

        #region Neutral Evil Roles
        public static CustomHeaderOption Jester;
        public static CustomNumberOption JesterOn;
        public static CustomToggleOption JesterButton;
        public static CustomToggleOption JesterVent;
        public static CustomToggleOption JesterVentSwitch;
        public static CustomToggleOption JesterImpVision;

        public static CustomHeaderOption Executioner;
        public static CustomNumberOption ExecutionerOn;
        public static CustomStringOption OnTargetDead;
        public static CustomToggleOption ExecutionerButton;

        public static CustomHeaderOption Vulture;
        public static CustomNumberOption VultureOn;
        public static CustomNumberOption VultureCd;
        public static CustomNumberOption VultureBodyCount;
        public static CustomToggleOption VultureVent;
        public static CustomToggleOption VultureImpVision;
        public static CustomToggleOption EatArrows;
        public static CustomNumberOption EatArrowDelay;

        public static CustomHeaderOption Phantom;
        public static CustomNumberOption PhantomOn;
        public static CustomNumberOption PhantomTasksRemaining;

        public static CustomHeaderOption Doomsayer;
        public static CustomNumberOption DoomsayerOn;
        public static CustomNumberOption ObserveCooldown;
        public static CustomToggleOption DoomsayerGuessNeutralBenign;
        public static CustomToggleOption DoomsayerGuessNeutralEvil;
        public static CustomToggleOption DoomsayerGuessNeutralKilling;
        public static CustomToggleOption DoomsayerGuessImpostors;
        public static CustomToggleOption DoomsayerAfterVoting;
        public static CustomNumberOption DoomsayerGuessesToWin;
        
        #endregion

        public static CustomNumberOption WerewolfOn;
        public static CustomHeaderOption Werewolf;
        public static CustomNumberOption MaulCooldown;
        public static CustomNumberOption MaulRadius;
        public static CustomToggleOption WerewolfVent;

        public static CustomHeaderOption Veteran;
        public static CustomToggleOption KilledOnAlert;
        public static CustomNumberOption AlertCooldown;
        public static CustomNumberOption AlertDuration;
        public static CustomNumberOption MaxAlerts;

        

        

        public static CustomHeaderOption Medium;
        public static CustomNumberOption MediateCooldown;
        public static CustomToggleOption ShowMediatePlayer;
        public static CustomToggleOption ShowMediumToDead;
        public static CustomStringOption DeadRevealed;

        #region Neutral Benign Roles
        public static CustomHeaderOption Amnesiac;
        public static CustomNumberOption AmnesiacOn;
        public static CustomToggleOption RememberArrows;
        public static CustomNumberOption RememberArrowDelay;

        public static CustomHeaderOption Romantic;
        public static CustomNumberOption RomanticOn;
        public static CustomNumberOption PickStartTimer;
        public static CustomStringOption RomanticOnBelovedDeath;
        public static CustomToggleOption RomanticBelovedKnows;
        public static CustomToggleOption RomanticKnowsBelovedRole;

        public static CustomHeaderOption GuardianAngel;
        public static CustomNumberOption GuardianAngelOn;
        public static CustomNumberOption ProtectCd;
        public static CustomNumberOption ProtectDuration;
        public static CustomNumberOption ProtectKCReset;
        public static CustomNumberOption MaxProtects;
        public static CustomStringOption ShowProtect;
        public static CustomStringOption GaOnTargetDeath;
        public static CustomToggleOption GATargetKnows;
        public static CustomToggleOption GAKnowsTargetRole;
        public static CustomNumberOption EvilTargetPercent;

        #endregion

        #region General Mod Settings
        public static CustomHeaderOption MapSettings;
        public static CustomToggleOption RandomMapEnabled;
        public static CustomNumberOption RandomMapSkeld;
        public static CustomNumberOption RandomMapMira;
        public static CustomNumberOption RandomMapPolus;
        public static CustomNumberOption RandomMapAirship;
        public static CustomNumberOption RandomMapFungle;
        public static CustomNumberOption RandomMapSubmerged;
        public static CustomNumberOption RandomMapLevelImpostor;

        public static CustomHeaderOption CustomGameSettings;
        public static CustomToggleOption ColourblindComms;
        public static CustomToggleOption ImpostorSeeRoles;
        public static CustomToggleOption CamoCommsKillAnyone;
        public static CustomNumberOption InitialCooldowns;
        public static CustomToggleOption ParallelMedScans;
        public static CustomStringOption SkipButtonDisable;
        public static CustomToggleOption FirstDeathShield;

        public static CustomHeaderOption BetterPolusSettings;
        public static CustomToggleOption VentImprovements;
        public static CustomToggleOption VitalsLab;
        public static CustomToggleOption ColdTempDeathValley;
        public static CustomToggleOption WifiChartCourseSwap;

        public static CustomHeaderOption GameModeSettings;
        public static CustomStringOption GameMode;

        public static CustomNumberOption MinNeutralBenignRoles;
        public static CustomNumberOption MaxNeutralBenignRoles;
        public static CustomNumberOption MinNeutralEvilRoles;
        public static CustomNumberOption MaxNeutralEvilRoles;
        public static CustomNumberOption MinNeutralKillingRoles;
        public static CustomNumberOption MaxNeutralKillingRoles;

        public static CustomHeaderOption AllAnySettings;
        public static CustomToggleOption RandomNumberImps;

        public static CustomHeaderOption KillersOnlySettings;
        public static CustomNumberOption NeutralRoles;
        public static CustomNumberOption VeteranCount;
        public static CustomToggleOption AddArsonist;
        public static CustomToggleOption AddPlaguebearer;

        public static CustomHeaderOption TaskTrackingSettings;
        public static CustomToggleOption SeeTasksDuringRound;
        public static CustomToggleOption SeeTasksDuringMeeting;
        public static CustomToggleOption SeeTasksWhenDead;

        #endregion

        public static Func<object, string> PercentFormat { get; } = value => $"{value:0}%";
        private static Func<object, string> CooldownFormat { get; } = value => $"{value:0.0#}s";
        private static Func<object, string> MultiplierFormat { get; } = value => $"{value:0.0#}x";

        public static void GenerateAll()
        {
            var num = 0;
            #region  General Mod Settings
            CustomGameSettings =
                new CustomHeaderOption(num++, MultiMenu.General, "Custom Game Settings");
            CamoCommsKillAnyone = new CustomToggleOption(num++, MultiMenu.General, "Kill Anyone During Camouflaged Comms", false);
            ColourblindComms = new CustomToggleOption(num++, MultiMenu.General, "Camouflaged Comms", false);
            ImpostorSeeRoles = new CustomToggleOption(num++, MultiMenu.General, "Impostors Can See The Roles Of Their Team", false);
            InitialCooldowns =
                new CustomNumberOption(num++, MultiMenu.General, "Game Start Cooldowns", 10f, 10f, 30f, 2.5f, CooldownFormat);
            ParallelMedScans = new CustomToggleOption(num++, MultiMenu.General, "Parallel Medbay Scans", false);
            SkipButtonDisable = new CustomStringOption(num++, MultiMenu.General, "Disable Meeting Skip Button", new[] { "No", "Emergency", "Always" });
            FirstDeathShield = new CustomToggleOption(num++, MultiMenu.General, "First Death Shield Next Game", false);

            TaskTrackingSettings =
                new CustomHeaderOption(num++, MultiMenu.General, "Task Tracking Settings");
            SeeTasksDuringRound = new CustomToggleOption(num++, MultiMenu.General, "See Tasks During Round", false);
            SeeTasksDuringMeeting = new CustomToggleOption(num++, MultiMenu.General, "See Tasks During Meetings", false);
            SeeTasksWhenDead = new CustomToggleOption(num++, MultiMenu.General, "See Tasks When Dead", true);

            MapSettings = new CustomHeaderOption(num++, MultiMenu.General, "Map Settings");
            RandomMapEnabled = new CustomToggleOption(num++, MultiMenu.General, "Choose Random Map", false);
            RandomMapSkeld = new CustomNumberOption(num++, MultiMenu.General, "Skeld Chance", 0f, 0f, 100f, 10f, PercentFormat);
            RandomMapMira = new CustomNumberOption(num++, MultiMenu.General, "Mira Chance", 0f, 0f, 100f, 10f, PercentFormat);
            RandomMapPolus = new CustomNumberOption(num++, MultiMenu.General, "Polus Chance", 0f, 0f, 100f, 10f, PercentFormat);
            RandomMapAirship = new CustomNumberOption(num++, MultiMenu.General, "Airship Chance", 0f, 0f, 100f, 10f, PercentFormat);
            RandomMapFungle = new CustomNumberOption(num++, MultiMenu.General, "Fungle Chance", 0f, 0f, 100f, 10f, PercentFormat);
            RandomMapSubmerged = new CustomNumberOption(num++, MultiMenu.General, "Submerged Chance", 0f, 0f, 100f, 10f, PercentFormat);
            RandomMapLevelImpostor = new CustomNumberOption(num++, MultiMenu.General, "Level Impostor Chance", 0f, 0f, 100f, 10f, PercentFormat);

            BetterPolusSettings =
                new CustomHeaderOption(num++, MultiMenu.General, "Better Polus Settings");
            VentImprovements = new CustomToggleOption(num++, MultiMenu.General, "Better Polus Vent Layout", false);
            VitalsLab = new CustomToggleOption(num++, MultiMenu.General, "Vitals Moved To Lab", false);
            ColdTempDeathValley = new CustomToggleOption(num++, MultiMenu.General, "Cold Temp Moved To Death Valley", false);
            WifiChartCourseSwap =
                new CustomToggleOption(num++, MultiMenu.General, "Reboot Wifi And Chart Course Swapped", false);

            GameModeSettings =
                new CustomHeaderOption(num++, MultiMenu.General, "Game Mode Settings");
            GameMode = new CustomStringOption(num++, MultiMenu.General, "Game Mode", new[] {"Classic", "All Any", "Killers Only"});

            AllAnySettings =
                new CustomHeaderOption(num++, MultiMenu.General, "All Any Settings");
            RandomNumberImps = new CustomToggleOption(num++, MultiMenu.General, "Random Number Of Impostors", true);

            KillersOnlySettings =
                new CustomHeaderOption(num++, MultiMenu.General, "Killers Only Settings");
            NeutralRoles =
                new CustomNumberOption(num++, MultiMenu.General, "Neutral Roles", 1, 0, 5, 1);
            VeteranCount =
                new CustomNumberOption(num++, MultiMenu.General, "Veteran Count", 1, 0, 5, 1);
            AddArsonist = new CustomToggleOption(num++, MultiMenu.General, "Add Arsonist", true);
            AddPlaguebearer = new CustomToggleOption(num++, MultiMenu.General, "Add Plaguebearer", true);

            #endregion

            #region  Crewmate Roles

            CrewInvestSettings =
                new CustomHeaderOption(num++, MultiMenu.Crewmate, "<color=#8BFDFDFF>Crew</color> <color=#1D7CF2FF>Investigative</color> <color=#FFD700FF>Roles</color> Settings");

            Investigator =
                new CustomHeaderOption(num++, MultiMenu.Crewmate, "<color=#00B3B3FF>Investigator</color>");
            InvestigatorOn = new CustomNumberOption(num++, MultiMenu.Crewmate, "<color=#00B3B3FF>Spawn Chance</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            FootprintSize = new CustomNumberOption(num++, MultiMenu.Crewmate, "Footprint Size", 4f, 1f, 10f, 1f);
            FootprintInterval =
                new CustomNumberOption(num++, MultiMenu.Crewmate, "Footprint Interval", 0.1f, 0.05f, 1f, 0.05f, CooldownFormat);
            FootprintDuration = new CustomNumberOption(num++, MultiMenu.Crewmate, "Footprint Duration", 10f, 1f, 15f, 0.5f, CooldownFormat);
            AnonymousFootPrint = new CustomToggleOption(num++, MultiMenu.Crewmate, "Anonymous Footprint", false);
            VentFootprintVisible = new CustomToggleOption(num++, MultiMenu.Crewmate, "Footprint Vent Visible", false);
            ExamineCooldown =
                new CustomNumberOption(num++, MultiMenu.Crewmate, "Examine Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            InvestigatorReportOn = new CustomToggleOption(num++, MultiMenu.Crewmate, "Show Investigator Reports", true);
            InvestigatorRoleDuration =
                new CustomNumberOption(num++, MultiMenu.Crewmate, "Time Where Investigator Will Have Role", 15f, 0f, 60f, 2.5f,
                    CooldownFormat);
            InvestigatorFactionDuration =
                new CustomNumberOption(num++, MultiMenu.Crewmate, "Time Where Investigator Will Have Faction", 30f, 0f, 60f, 2.5f,
                    CooldownFormat);

            Medium =
                new CustomHeaderOption(num++, MultiMenu.Crewmate, "<color=#A680FFFF>Medium</color>");
            MediumOn = new CustomNumberOption(num++, MultiMenu.Crewmate, "<color=#A680FFFF>Spawn Chance</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            MediateCooldown =
                new CustomNumberOption(num++, MultiMenu.Crewmate, "Mediate Cooldown", 10f, 1f, 15f, 1f, CooldownFormat);
            ShowMediatePlayer =
                new CustomToggleOption(num++, MultiMenu.Crewmate, "Reveal Appearance Of Mediate Target", true);
            ShowMediumToDead =
                new CustomToggleOption(num++, MultiMenu.Crewmate, "Reveal The Medium To The Mediate Target", true);
            DeadRevealed =
                new CustomStringOption(num++, MultiMenu.Crewmate, "Who Is Revealed With Mediate", new[] { "Oldest Dead", "Newest Dead", "All Dead" });

            Mystic =
                new CustomHeaderOption(num++, MultiMenu.Crewmate, "<color=#4D99E6FF>Mystic</color>");
            MysticOn = new CustomNumberOption(num++, MultiMenu.Crewmate, "<color=#4D99E6FF>Spawn Chance</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            MysticArrowDuration =
                new CustomNumberOption(num++, MultiMenu.Crewmate, "Dead Body Arrow Duration", 0.1f, 0f, 1f, 0.05f, CooldownFormat);
            InitialExamineCooldown =
                new CustomNumberOption(num++, MultiMenu.Crewmate, "Initial Examine Cooldown", 30f, 10f, 60f, 2.5f, CooldownFormat);
            MysticExamineCooldown =
                new CustomNumberOption(num++, MultiMenu.Crewmate, "Examine Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            RecentKill =
                new CustomNumberOption(num++, MultiMenu.Crewmate, "How Long Players Stay Bloody For", 30f, 10f, 60f, 2.5f, CooldownFormat);
            MysticReportOn = new CustomToggleOption(num++, MultiMenu.Crewmate, "Show Mystic Reports", true);
            MysticRoleDuration =
                new CustomNumberOption(num++, MultiMenu.Crewmate, "Time Where Mystic Will Have Role", 15f, 0f, 60f, 2.5f,
                    CooldownFormat);
            MysticFactionDuration =
                new CustomNumberOption(num++, MultiMenu.Crewmate, "Time Where Mystic Will Have Faction", 30f, 0f, 60f, 2.5f,
                    CooldownFormat);
            MysticExamineReportOn = new CustomToggleOption(num++, MultiMenu.Crewmate, "Show Mystic Examine Reports", true);

            Oracle =
                new CustomHeaderOption(num++, MultiMenu.Crewmate, "<color=#344feb>Oracle</color>");
            OracleOn = new CustomNumberOption(num++, MultiMenu.Crewmate, "<color=#344feb>Spawn Chance</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            ConfessCooldown =
                new CustomNumberOption(num++, MultiMenu.Crewmate, "Confess Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            RevealAccuracy = new CustomNumberOption(num++, MultiMenu.Crewmate, "Reveal Accuracy", 80f, 0f, 100f, 10f,
                PercentFormat);
            NeutralBenignShowsEvil =
                new CustomToggleOption(num++, MultiMenu.Crewmate, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Benign</color> <color=#FFD700FF>Roles</color> Show Evil", false);
            NeutralEvilShowsEvil =
                new CustomToggleOption(num++, MultiMenu.Crewmate, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Evil</color> <color=#FFD700FF>Roles</color> Show Evil", false);
            NeutralKillingShowsEvil =
                new CustomToggleOption(num++, MultiMenu.Crewmate, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killing</color> <color=#FFD700FF>Roles</color> Show Evil", true);


            Seer =
                new CustomHeaderOption(num++, MultiMenu.Crewmate, "<color=#FFCC80FF>Seer</color>");
            SeerOn = new CustomNumberOption(num++, MultiMenu.Crewmate, "<color=#FFCC80FF>Spawn Chance</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            SeerCooldown =
                new CustomNumberOption(num++, MultiMenu.Crewmate, "<color=#FFCC80FF>Seer</color> Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            CrewKillingRed =
                new CustomToggleOption(num++, MultiMenu.Crewmate, "<color=#8BFDFDFF>Crew</color> Killing Roles Are Red", false);
            NeutBenignRed =
                new CustomToggleOption(num++, MultiMenu.Crewmate, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Benign</color> <color=#FFD700FF>Roles</color> Are Red", false);
            NeutEvilRed =
                new CustomToggleOption(num++, MultiMenu.Crewmate, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Evil</color> <color=#FFD700FF>Roles</color> Are Red", false);
            NeutKillingRed =
                new CustomToggleOption(num++, MultiMenu.Crewmate, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killing</color> <color=#FFD700FF>Roles</color> Are Red", true);

            Snitch = new CustomHeaderOption(num++, MultiMenu.Crewmate, "<color=#B34D99FF>Snitch</color>");
            SnitchOn = new CustomNumberOption(num++, MultiMenu.Crewmate, "<color=#B34D99FF>Spawn Chance</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            SnitchTasksRemaining =
                new CustomNumberOption(num++, MultiMenu.Crewmate, "Tasks Remaining When <color=#B34D99FF>Snitch</color> Is Revealed", 1, 1, 5, 1);
            SnitchButton = new CustomToggleOption(num++, MultiMenu.Crewmate, "<color=#B34D99FF>Snitch</color> Can Call An Emergency Meeting After Reveal", false);

            Tracker =
                new CustomHeaderOption(num++, MultiMenu.Crewmate, "<color=#66E666FF>Tracker</color>");
            TrackerOn = new CustomNumberOption(num++, MultiMenu.Crewmate, "<color=#66E666FF>Spawn Chance</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            UpdateInterval =
                new CustomNumberOption(num++, MultiMenu.Crewmate, "Arrow Update Interval", 5f, 0.5f, 15f, 0.5f, CooldownFormat);
            TrackCooldown =
                new CustomNumberOption(num++, MultiMenu.Crewmate, "Track Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            ResetOnNewRound = new CustomToggleOption(num++, MultiMenu.Crewmate, "Tracker Arrows Reset After Each Round", false);
            MaxTracks = new CustomNumberOption(num++, MultiMenu.Crewmate, "Maximum Number Of Tracks Per Round", 5, 1, 15, 1);

            Trapper =
                new CustomHeaderOption(num++, MultiMenu.Crewmate, "<color=#A7D1B3FF>Trapper</color>");
            TrapperOn = new CustomNumberOption(num++, MultiMenu.Crewmate, "<color=#A7D1B3FF>Spawn Chance</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            MinAmountOfTimeInTrap =
                new CustomNumberOption(num++, MultiMenu.Crewmate, "Min Amount Of Time In Trap To Register", 1f, 0f, 15f, 0.5f, CooldownFormat);
            TrapCooldown =
                new CustomNumberOption(num++, MultiMenu.Crewmate, "Trap Cooldown", 25f, 10f, 40f, 2.5f, CooldownFormat);
            TrapsRemoveOnNewRound =
                new CustomToggleOption(num++, MultiMenu.Crewmate, "Traps Removed After Each Round", true);
            MaxTraps =
                new CustomNumberOption(num++, MultiMenu.Crewmate, "Maximum Number Of Traps Per Game", 5, 1, 15, 1);
            TrapSize =
                new CustomNumberOption(num++, MultiMenu.Crewmate, "Trap Size", 0.25f, 0.05f, 1f, 0.05f, MultiplierFormat);
            MinAmountOfPlayersInTrap =
                new CustomNumberOption(num++, MultiMenu.Crewmate, "Minimum Number Of Roles Required To Trigger Trap", 3, 1, 5, 1);
            
            CrewKillingSettings =
                new CustomHeaderOption(num++, MultiMenu.Crewmate, "<color=#8BFDFDFF>Crew</color> <color=#1D7CF2FF>Killing</color> <color=#FFD700FF>Roles</color> Settings");            

                
            Hunter =
               new CustomHeaderOption(num++, MultiMenu.Crewmate, "<color=#29AB87FF>Hunter</color>");
            HunterOn = new CustomNumberOption(num++, MultiMenu.Crewmate, "<color=#29AB87FF>Spawn Chance</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            HunterKillCd =
                new CustomNumberOption(num++, MultiMenu.Crewmate, "<color=#29AB87FF>Hunter</color> Kill Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            HunterStalkCd =
                new CustomNumberOption(num++, MultiMenu.Crewmate, "<color=#29AB87FF>Hunter</color> Stalk Cooldown", 10f, 1f, 15f, 1f, CooldownFormat);
            HunterStalkDuration =
                new CustomNumberOption(num++, MultiMenu.Crewmate, "<color=#29AB87FF>Hunter</color> Stalk Duration", 25f, 5f, 60f, 2.5f, CooldownFormat);
            HunterStalkUses =
                new CustomNumberOption(num++, MultiMenu.Crewmate, "Maximum Stalk Uses", 5, 1, 15, 1);
            RetributionOnVote =
                new CustomToggleOption(num++, MultiMenu.Crewmate, "<color=#29AB87FF>Hunter</color> Kills Last Voter If Voted Out", false);
            HunterBodyReport =
                new CustomToggleOption(num++, MultiMenu.Crewmate, "<color=#29AB87FF>Hunter</color> Can Report Who They've Killed");

            Jailor =
               new CustomHeaderOption(num++, MultiMenu.Crewmate, "<color=#A6A6A6FF>Jailor</color>");
            JailorOn = new CustomNumberOption(num++, MultiMenu.Crewmate, "<color=#A6A6A6FF>Spawn Chance</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            JailCooldown =
                new CustomNumberOption(num++, MultiMenu.Crewmate, "Jail Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            MaxExecutes =
                new CustomNumberOption(num++, MultiMenu.Crewmate, "Maximum Number Of Executes", 3, 1, 5, 1);

            Veteran =
                new CustomHeaderOption(num++, MultiMenu.Crewmate, "<color=#998040FF>Veteran</color>");
            VeteranOn = new CustomNumberOption(num++, MultiMenu.Crewmate, "<color=#998040FF>Spawn Chance</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            KilledOnAlert =
                new CustomToggleOption(num++, MultiMenu.Crewmate, "Can Be Killed On Alert", false);
            AlertCooldown =
                new CustomNumberOption(num++, MultiMenu.Crewmate, "Alert Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            AlertDuration =
                new CustomNumberOption(num++, MultiMenu.Crewmate, "Alert Duration", 10f, 5f, 15f, 1f, CooldownFormat);
            MaxAlerts = new CustomNumberOption(num++, MultiMenu.Crewmate, "Maximum Number Of Alerts", 5, 1, 15, 1);

            Vigilante = new CustomHeaderOption(num++, MultiMenu.Crewmate, "<color=#FFFF99FF>Vigilante</color>");
            VigilanteOn = new CustomNumberOption(num++, MultiMenu.Crewmate, "<color=#FFFF99FF>Spawn Chance</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            VigilanteKills = new CustomNumberOption(num++, MultiMenu.Crewmate, "Number Of Vigilante Kills", 1, 1, 15, 1);
            VigilanteMultiKill = new CustomToggleOption(num++, MultiMenu.Crewmate, "Vigilante Can Kill More Than Once Per Meeting", false);
            VigilanteGuessNeutralBenign = new CustomToggleOption(num++, MultiMenu.Crewmate, "Vigilante Can Guess <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Benign</color> <color=#FFD700FF>Roles</color>", false);
            VigilanteGuessNeutralEvil = new CustomToggleOption(num++, MultiMenu.Crewmate, "Vigilante Can Guess <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Evil</color> <color=#FFD700FF>Roles</color>", false);
            VigilanteGuessNeutralKilling = new CustomToggleOption(num++, MultiMenu.Crewmate, "Vigilante Can Guess <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killing</color> <color=#FFD700FF>Roles</color>", false);
            VigilanteAfterVoting = new CustomToggleOption(num++, MultiMenu.Crewmate, "Vigilante Can Guess After Voting", false);
            VigilanteKillOther =
                new CustomToggleOption(num++, MultiMenu.Crewmate, "Vigilante Miskill Kills Crewmate", false);
            VigilanteKillsNeutralEvil =
                new CustomToggleOption(num++, MultiMenu.Crewmate, "Vigilante Kills Neutral Evil", false);
            VigilanteKillsNeutralBenign =
                new CustomToggleOption(num++, MultiMenu.Crewmate, "Vigilante Kills Neutral Benign", false);
            VigilanteKillCd =
                new CustomNumberOption(num++, MultiMenu.Crewmate, "Vigilante Kill Cooldown", 25f, 10f, 40f, 2.5f, CooldownFormat);
            VigilanteBodyReport = new CustomToggleOption(num++, MultiMenu.Crewmate, "Vigilante Can Report Who They've Killed");

            CrewSpecialRoles =
                new CustomHeaderOption(num++, MultiMenu.Crewmate, "<color=#8BFDFDFF>Crew</color> <color=#1D7CF2FF>Special</color> <color=#FFD700FF>Roles</color> Settings");

            Haunter =
                new CustomHeaderOption(num++, MultiMenu.Crewmate, "<color=#d3d3d3FF>Haunter</color>");
            HaunterOn = new CustomNumberOption(num++, MultiMenu.Crewmate, "<color=#D3D3D3FF>Spawn Chance</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            HaunterTasksRemainingClicked =
                 new CustomNumberOption(num++, MultiMenu.Crewmate, "Tasks Remaining When Haunter Can Be Clicked", 5, 1, 15, 1);
            HaunterTasksRemainingAlert =
                 new CustomNumberOption(num++, MultiMenu.Crewmate, "Tasks Remaining When Alert Is Sent", 1, 1, 5, 1);
            HaunterRevealsNeutrals = new CustomToggleOption(num++, MultiMenu.Crewmate, "Haunter Reveals Neutral Roles", false);
            HaunterCanBeClickedBy = new CustomStringOption(num++, MultiMenu.Crewmate, "Who Can Click Haunter", new[] { "All", "Non-Crew", "Imps Only" });

            CrewSupportSettings =
                new CustomHeaderOption(num++, MultiMenu.Crewmate, "<color=#8BFDFDFF>Crew</color> <color=#1D7CF2FF>Support</color> <color=#FFD700FF>Roles</color> Settings");         
            
                
            Engineer =
                new CustomHeaderOption(num++, MultiMenu.Crewmate, "<color=#FFA60AFF>Engineer</color>");
            EngineerOn = new CustomNumberOption(num++, MultiMenu.Crewmate, "<color=#FFA60AFF>Spawn Chance</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            MaxFixes =
                new CustomNumberOption(num++, MultiMenu.Crewmate, "Maximum Number Of Fixes", 5, 1, 15, 1);
            
            Imitator =
                new CustomHeaderOption(num++, MultiMenu.Crewmate, "<color=#B3D94DFF>Imitator</color>");
            ImitatorOn = new CustomNumberOption(num++, MultiMenu.Crewmate, "<color=#B3D94DFF>Spawn Chance</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            
            Medic =
                new CustomHeaderOption(num++, MultiMenu.Crewmate, "<color=#7efbc2>Medic</color>");
            MedicOn = new CustomNumberOption(num++, MultiMenu.Crewmate, "<color=#7efbc2>Spawn Chance</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            ShowShielded =
                new CustomStringOption(num++, MultiMenu.Crewmate, "Show Shielded Player",
                    new[] { "Self", "Medic", "Self+Medic", "Everyone" });
            WhoGetsNotification =
                new CustomStringOption(num++, MultiMenu.Crewmate, "Who Gets Murder Attempt Indicator",
                    new[] { "Medic", "Shielded", "Everyone", "Nobody" });
            ShieldBreaks = new CustomToggleOption(num++, MultiMenu.Crewmate, "Shield Breaks On Murder Attempt", false);
            MedicReportSwitch = new CustomToggleOption(num++, MultiMenu.Crewmate, "Show Medic Reports");
            MedicReportNameDuration =
                new CustomNumberOption(num++, MultiMenu.Crewmate, "Time Where Medic Will Have Name", 0f, 0f, 60f, 2.5f,
                    CooldownFormat);
            MedicReportColorDuration =
                new CustomNumberOption(num++, MultiMenu.Crewmate, "Time Where Medic Will Have Color Type", 15f, 0f, 60f, 2.5f,
                    CooldownFormat);

            Swapper =
                new CustomHeaderOption(num++, MultiMenu.Crewmate, "<color=#66E666FF>Swapper</color>");
            SwapperOn = new CustomNumberOption(num++, MultiMenu.Crewmate, "<color=#66E666FF>Spawn Chance</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            SwapperButton =
                new CustomToggleOption(num++, MultiMenu.Crewmate, "Swapper Can Button", true);

            Transporter =
                new CustomHeaderOption(num++, MultiMenu.Crewmate, "<color=#00EEFFFF>Transporter</color>");
            TransporterOn = new CustomNumberOption(num++, MultiMenu.Crewmate, "<color=#00EEFFFF>Spawn Chance</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            TransportCooldown =
                new CustomNumberOption(num++, MultiMenu.Crewmate, "Transport Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            TransportMaxUses =
                new CustomNumberOption(num++, MultiMenu.Crewmate, "Maximum Number Of Transports", 5, 1, 15, 1);
            TransporterVitals =
                new CustomToggleOption(num++, MultiMenu.Crewmate, "Transporter Can Use Vitals", false);

            #endregion

            #region Neutral Roles Settings
            NeutralKillingRoles = new CustomHeaderOption(num++, MultiMenu.Neutral, "<color=#B3B3B3FF>Neutral</color> <color=#FFD700FF>Roles</color> Settings");
            MinNeutralBenignRoles =
                new CustomNumberOption(num++, MultiMenu.Neutral, "Min <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Benign</color> <color=#FFD700FF>Roles</color>", 1, 0, 5, 1);
            MaxNeutralBenignRoles =
                new CustomNumberOption(num++, MultiMenu.Neutral, "Max <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Benign</color> <color=#FFD700FF>Roles</color>", 1, 0, 5, 1);
            MinNeutralEvilRoles =
                new CustomNumberOption(num++, MultiMenu.Neutral, "Min <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Evil</color> <color=#FFD700FF>Roles</color>", 1, 0, 5, 1);
            MaxNeutralEvilRoles =
                new CustomNumberOption(num++, MultiMenu.Neutral, "Max <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Evil</color> <color=#FFD700FF>Roles</color>", 1, 0, 5, 1);
            MinNeutralKillingRoles =
                new CustomNumberOption(num++, MultiMenu.Neutral, "Min <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killing</color> <color=#FFD700FF>Roles</color>", 1, 0, 5, 1);
            MaxNeutralKillingRoles =
                new CustomNumberOption(num++, MultiMenu.Neutral, "Max <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killing</color> <color=#FFD700FF>Roles</color>", 1, 0, 5, 1);
            
            #endregion

            #region Neutral Evil Roles
            NeutralEvilRoles = new CustomHeaderOption(num++, MultiMenu.Neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Evil</color> <color=#FFD700FF>Roles</color> Settings");

            Doomsayer = new CustomHeaderOption(num++, MultiMenu.Neutral, "<color=#00FF80FF>Doomsayer</color>");
            DoomsayerOn = new CustomNumberOption(num++, MultiMenu.Neutral, "<color=#00FF80FF>Spawn Chance</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            ObserveCooldown =
                new CustomNumberOption(num++, MultiMenu.Neutral, "Observe Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            DoomsayerGuessNeutralBenign = new CustomToggleOption(num++, MultiMenu.Neutral, "<color=#00FF80FF>Doomsayer</color> Can Guess <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Benign</color> <color=#FFD700FF>Roles</color>", false);
            DoomsayerGuessNeutralEvil = new CustomToggleOption(num++, MultiMenu.Neutral, "<color=#00FF80FF>Doomsayer</color> Can Guess <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Evil</color> <color=#FFD700FF>Roles</color>", false);
            DoomsayerGuessNeutralKilling = new CustomToggleOption(num++, MultiMenu.Neutral, "<color=#00FF80FF>Doomsayer</color> Can Guess <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killing</color> <color=#FFD700FF>Roles</color>", false);
            DoomsayerGuessNeutralBenign = new CustomToggleOption(num++, MultiMenu.Neutral, "<color=#00FF80FF>Doomsayer</color> Can Guess <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Benign</color> <color=#FFD700FF>Roles</color>", false);
            DoomsayerGuessImpostors = new CustomToggleOption(num++, MultiMenu.Neutral, "<color=#00FF80FF>Doomsayer</color> Can Guess <color=#FF0000FF>Impostor</color> <color=#FFD700FF>Roles</color>", false);
            DoomsayerAfterVoting = new CustomToggleOption(num++, MultiMenu.Neutral, "<color=#00FF80FF>Doomsayer</color> Can Guess After Voting", false);
            DoomsayerGuessesToWin = new CustomNumberOption(num++, MultiMenu.Neutral, "Number Of <color=#00FF80FF>Doomsayer</color> Kills To Win", 3, 1, 5, 1);
            
            Executioner =
                new CustomHeaderOption(num++, MultiMenu.Neutral, "<color=#CCCCCCFF>Executioner</color>");
            ExecutionerOn = new CustomNumberOption(num++, MultiMenu.Neutral, "<color=#CCCCCCFF>Spawn Chance</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            OnTargetDead = new CustomStringOption(num++, MultiMenu.Neutral, "<color=#CCCCCCFF>Executioner</color> Becomes On Target Dead",
                new[] { "<color=#8BFDFDFF>Crew</color>", "<color=#22FFFFFF>Amnesiac</color>", "<color=#FFBFCCFF>Jester</color>" });
            ExecutionerButton =
                new CustomToggleOption(num++, MultiMenu.Neutral, "<color=#CCCCCCFF>Executioner</color> Can Button", true);

            Jester =
                new CustomHeaderOption(num++, MultiMenu.Neutral, "<color=#FFBFCCFF>Jester</color>");
            JesterOn = new CustomNumberOption(num++, MultiMenu.Neutral, "<color=#FFBFCCFF>Spawn Chance</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            JesterButton =
                new CustomToggleOption(num++, MultiMenu.Neutral, "<color=#FFBFCCFF>Jester</color> Can Button", true);
            JesterVent =
                new CustomToggleOption(num++, MultiMenu.Neutral, "<color=#FFBFCCFF>Jester</color> Can Hide In Vents", false);
            JesterVentSwitch =
                new CustomToggleOption(num++, MultiMenu.Neutral, "<color=#FFBFCCFF>Jester</color> Can Switch Between Vents", false);
            JesterImpVision =
                new CustomToggleOption(num++, MultiMenu.Neutral, "<color=#FFBFCCFF>Jester</color> Has Impostor Vision", false);
            
            Phantom =
                new CustomHeaderOption(num++, MultiMenu.Neutral, "<color=#662962FF>Phantom</color>");
            PhantomOn = new CustomNumberOption(num++, MultiMenu.Neutral, "<color=#662962FF>Spawn Chance</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            PhantomTasksRemaining =
                new CustomNumberOption(num++, MultiMenu.Neutral, "Tasks Left When Can Be Clicked", 5, 1, 15, 1);

            Vulture = new CustomHeaderOption(num++, MultiMenu.Neutral, "<color=#8C4005FF>Vulture</color>"); 
            VultureOn = new CustomNumberOption(num++, MultiMenu.Neutral, "<color=#8C4005FF>Spawn Chance</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            VultureCd = new CustomNumberOption(num++, MultiMenu.Neutral, "Eat Cooldown", 10f, 10f, 60f, 2.5f, CooldownFormat);
            VultureBodyCount = new CustomNumberOption(num++, MultiMenu.Neutral, "Number Of Bodies To Eat", 1, 1, 5, 1);
            VultureVent = new CustomToggleOption(num++, MultiMenu.Neutral, "<color=#8C4005FF>Vulture</color> Can Vent", false);
            VultureImpVision = new CustomToggleOption(num++, MultiMenu.Neutral, "<color=#8C4005FF>Vulture</color> Has Impostor Vision", false);
            EatArrows = new CustomToggleOption(num++, MultiMenu.Neutral, "<color=#8C4005FF>Vulture</color> Gets Arrows To Dead Bodies", false);
            EatArrowDelay = new CustomNumberOption(num++, MultiMenu.Neutral, "Time After Death Arrow Appears", 5f, 0f, 15f, 1f, CooldownFormat);

            #endregion

            #region Neutral Benign Roles
            NeutralBenignRoles = new CustomHeaderOption(num++, MultiMenu.Neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Benign</color> <color=#FFD700FF>Roles</color>");
            Amnesiac = new CustomHeaderOption(num++, MultiMenu.Neutral, "<color=#22FFFFFF>Amnesiac</color>");
            AmnesiacOn = new CustomNumberOption(num++, MultiMenu.Neutral, "<color=#22FFFFFF>Spawn Chance</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            RememberArrows =
                new CustomToggleOption(num++, MultiMenu.Neutral, "<color=#22FFFFFF>Amnesiac</color> Gets Arrows Pointing To Dead Bodies", false);
            RememberArrowDelay =
                new CustomNumberOption(num++, MultiMenu.Neutral, "Time After Death Arrow Appears", 5f, 0f, 15f, 1f, CooldownFormat);
            
            GuardianAngel =
                new CustomHeaderOption(num++, MultiMenu.Neutral, "<color=#FFFFFFFF>Guardian Angel</color>");
            GuardianAngelOn = new CustomNumberOption(num++, MultiMenu.Neutral, "<color=#FFFFFFFF>Spawn Chance</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            ProtectCd =
                new CustomNumberOption(num++, MultiMenu.Neutral, "Protect Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            ProtectDuration =
                new CustomNumberOption(num++, MultiMenu.Neutral, "Protect Duration", 10f, 5f, 15f, 1f, CooldownFormat);
            ProtectKCReset =
                new CustomNumberOption(num++, MultiMenu.Neutral, "Kill Cooldown Reset When Protected", 2.5f, 0f, 15f, 0.5f, CooldownFormat);
            MaxProtects =
                new CustomNumberOption(num++, MultiMenu.Neutral, "Maximum Number Of Protects", 5, 1, 15, 1);
            ShowProtect =
                new CustomStringOption(num++, MultiMenu.Neutral, "Show Protected Player",
                    new[] { "Self", "Guardian Angel", "Self+GA", "Everyone" });
            GaOnTargetDeath = new CustomStringOption(num++, MultiMenu.Neutral, "GA Becomes On Target Dead",
                new[] { "<color=#8BFDFDFF>Crew</color>", "<color=#22FFFFFF>Amnesiac</color>", "<color=#FFBFCCFF>Jester</color>" });
            GATargetKnows =
                new CustomToggleOption(num++, MultiMenu.Neutral, "Target Knows GA Exists", false);
            GAKnowsTargetRole =
                new CustomToggleOption(num++, MultiMenu.Neutral, "GA Knows Targets Role", false);
            EvilTargetPercent = new CustomNumberOption(num++, MultiMenu.Neutral, "Odds Of Target Being Evil", 20f, 0f, 100f, 10f,
                PercentFormat);

            Romantic =
                new CustomHeaderOption(num++, MultiMenu.Neutral, "<color=#FF66CCFF>Romantic</color>");
            RomanticOn = new CustomNumberOption(num++, MultiMenu.Neutral, "<color=#FF66CCFF>Spawn Chance</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            PickStartTimer =
                new CustomNumberOption(num++, MultiMenu.Neutral, "Start Pick Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            RomanticOnBelovedDeath = new CustomStringOption(num++, MultiMenu.Neutral, "Romantic Becomes On Beloved Dead",
                new[] { "<color=#FF66CCFF>Repick Beloved</color>","<color=#8BFDFDFF>Crew</color>", "<color=#22FFFFFF>Amnesiac</color>", "<color=#FFBFCCFF>Jester</color>" });
            RomanticBelovedKnows =
                new CustomToggleOption(num++, MultiMenu.Neutral, "<color=#FF66CCFF>Beloved</color> Knows The Existence Of <color=#FF66CCFF>Romantic</color>", false);
            RomanticKnowsBelovedRole =
                new CustomToggleOption(num++, MultiMenu.Neutral, "Romantic Knows Beloved's Role", false);
            
            #endregion

            #region Neutral Killing Roles
            NeutralKillingRoles = new CustomHeaderOption(num++, MultiMenu.Neutral, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killing</color> <color=#FFD700FF>Roles</color> Settings");

            Arsonist = new CustomHeaderOption(num++, MultiMenu.Neutral, "<color=#FF4D00FF>Arsonist</color>");
            ArsonistOn = new CustomNumberOption(num++, MultiMenu.Neutral, "<color=#FF4D00FF>Spawn Chance</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            DouseCooldown =
                new CustomNumberOption(num++, MultiMenu.Neutral, "Douse Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            MaxDoused =
                new CustomNumberOption(num++, MultiMenu.Neutral, "Maximum Alive Players Doused", 5, 1, 15, 1);
            IgniteCdRemoved =
                new CustomToggleOption(num++, MultiMenu.Neutral, "Ignite Cooldown Removed When <color=#FF4D00FF>Arsonist</color> Is Last Killer", false);

            Juggernaut =
                new CustomHeaderOption(num++, MultiMenu.Neutral, "<color=#8C004DFF>Juggernaut</color>");
            JuggernautOn = new CustomNumberOption(num++, MultiMenu.Neutral, "<color=#8C004DFF>Spawn Chance</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            JuggKillCooldown = new CustomNumberOption(num++, MultiMenu.Neutral, "<color=#8C004DFF>Juggernaut</color> Initial Kill Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            ReducedKCdPerKill = new CustomNumberOption(num++, MultiMenu.Neutral, "Reduced Kill Cooldown Per Kill", 5f, 2.5f, 10f, 2.5f, CooldownFormat);
            JuggVent = new CustomToggleOption(num++, MultiMenu.Neutral, "<color=#8C004DFF>Juggernaut</color> Can Vent", false);

            TheGlitch =
                new CustomHeaderOption(num++, MultiMenu.Neutral, "<color=#00FF00FF>Glitch</color>");
            GlitchOn = new CustomNumberOption(num++, MultiMenu.Neutral, "<color=#00FF00FF>Spawn Chance</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            MimicCooldownOption = new CustomNumberOption(num++, MultiMenu.Neutral, "Mimic Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            MimicDurationOption = new CustomNumberOption(num++, MultiMenu.Neutral, "Mimic Duration", 10f, 1f, 15f, 1f, CooldownFormat);
            HackCooldownOption = new CustomNumberOption(num++, MultiMenu.Neutral, "Hack Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            HackDurationOption = new CustomNumberOption(num++, MultiMenu.Neutral, "Hack Duration", 10f, 1f, 15f, 1f, CooldownFormat);
            GlitchKillCooldownOption =
                new CustomNumberOption(num++, MultiMenu.Neutral, "<color=#00FF00FF>Glitch</color> Kill Cooldown", 25f, 10f, 120f, 2.5f, CooldownFormat);
            GlitchHackDistanceOption =
                new CustomStringOption(num++, MultiMenu.Neutral, "<color=#00FF00FF>Glitch</color> Hack Distance", new[] { "Short", "Normal", "Long" });
            GlitchVent =
                new CustomToggleOption(num++, MultiMenu.Neutral, "<color=#00FF00FF>Glitch</color> Can Vent", false);

            Plaguebearer = new CustomHeaderOption(num++, MultiMenu.Neutral, "<color=#E6FFB3FF>Plaguebearer</color>");
            PlaguebearerOn = new CustomNumberOption(num++, MultiMenu.Neutral, "<color=#E6FFB3FF>Spawn Chance</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            InfectCooldown =
                new CustomNumberOption(num++, MultiMenu.Neutral, "Infect Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            PestKillCooldown =
                new CustomNumberOption(num++, MultiMenu.Neutral, "Pestilence Kill Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            PestVent =
                new CustomToggleOption(num++, MultiMenu.Neutral, "Pestilence Can Vent", false);

            Hitman =
                new CustomHeaderOption(num++, MultiMenu.Neutral, "<color=#00b4eb>Hitman</color>");
            AgentOn = new CustomNumberOption(num++, MultiMenu.Neutral, "<color=#0000FFFF> Agent Spawn Chance</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            SkipAgent =
                new CustomToggleOption(num++, MultiMenu.Neutral, "<color=#00b4eb>Hitman</color> Spawns Without Agent \n (Uses Agent's Chance)", false);
            HitmanMorphCooldown = new CustomNumberOption(num++, MultiMenu.Neutral, "Morph Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            HitmanMorphDuration = new CustomNumberOption(num++, MultiMenu.Neutral, "Morph Duration", 10f, 1f, 15f, 1f, CooldownFormat);
            HitmanKillCooldown =
                new CustomNumberOption(num++, MultiMenu.Neutral, "<color=#00b4eb>Hitman</color> Kill Cooldown", 25f, 10f, 120f, 2.5f, CooldownFormat);
            HitmanDragCooldown = new CustomNumberOption(num++, MultiMenu.Neutral, "Drag Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            HitmanDragSpeed =
                new CustomNumberOption(num++, MultiMenu.Neutral, "<color=#00b4eb>Hitman</color> Drag Speed", 0.75f, 0.25f, 1f, 0.05f, MultiplierFormat);
            HitmanVent =
                new CustomToggleOption(num++, MultiMenu.Neutral, "<color=#00b4eb>Hitman</color> Can Vent", false);
            HitmanVentWithBody =
                new CustomToggleOption(num++, MultiMenu.Neutral, "<color=#00b4eb>Hitman</color> Can Vent While Dragging", false);


            Vampire = new CustomHeaderOption(num++, MultiMenu.Neutral, "<color=#262626FF>Vampire</color>");
            VampireOn = new CustomNumberOption(num++, MultiMenu.Neutral, "<color=#262626FF>Spawn Chance</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            BiteCooldown =
                new CustomNumberOption(num++, MultiMenu.Neutral, "<color=#262626FF>Vampire</color> Bite Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            VampVent =
                new CustomToggleOption(num++, MultiMenu.Neutral, "<color=#262626FF>Vampire</color> Can Vent", false);
            NewVampCanAssassin =
                new CustomToggleOption(num++, MultiMenu.Neutral, "New <color=#262626FF>Vampire</color> Can Assassinate", false);
            MaxVampiresPerGame =
                new CustomNumberOption(num++, MultiMenu.Neutral, "Maximum <color=#262626FF>Vampire</color> Per Game", 2, 2, 5, 1);
            CanBiteNeutralBenign =
                new CustomToggleOption(num++, MultiMenu.Neutral, "Can Convert <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Benign</color> <color=#FFD700FF>Roles</color>", false);
            CanBiteNeutralEvil =
                new CustomToggleOption(num++, MultiMenu.Neutral, "Can Convert <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Evil</color> <color=#FFD700FF>Roles</color>", false);
            CanBiteNeutralBenign =
                new CustomToggleOption(num++, MultiMenu.Neutral, "Can Convert <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Benign</color> <color=#FFD700FF>Roles</color>", false);

            
            SerialKiller = new CustomHeaderOption(num++, MultiMenu.Neutral, "<color=#336EFFFF>Serial Killer</color>");
            SerialKillerOn = new CustomNumberOption(num++, MultiMenu.Neutral, "<color=#336EFFFF>Spawn Chance</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            StabCooldown =
                new CustomNumberOption(num++, MultiMenu.Neutral, "Stab Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            Stabeduration =
                new CustomNumberOption(num++, MultiMenu.Neutral, "Stab Duration", 25f, 10f, 60f, 2.5f, CooldownFormat);
            StabKillCooldown =
                new CustomNumberOption(num++, MultiMenu.Neutral, "Stab Cooldown", 10f, 0.5f, 15f, 0.5f, CooldownFormat);
            SerialKillerVent =
                new CustomToggleOption(num++, MultiMenu.Neutral, "<color=#336EFFFF>Serial Killer</color> Can Vent When Stab Is Active", false);

            Werewolf = new CustomHeaderOption(num++, MultiMenu.Neutral, "<color=#A86629FF>Werewolf</color>");
            WerewolfOn = new CustomNumberOption(num++, MultiMenu.Neutral, "<color=#A86629FF>Spawn Chance</color>", 0, 0, 100, 10, PercentFormat);
            MaulCooldown = new CustomNumberOption(num++, MultiMenu.Neutral, "Maul Cooldown", 30f, 10f, 60f, 2.5f, CooldownFormat);
            MaulRadius = new CustomNumberOption(num++, MultiMenu.Neutral, "Maul Radius", 0.25f, 0.05f, 1f, 0.05f, MultiplierFormat);
            WerewolfVent = new CustomToggleOption(num++, MultiMenu.Neutral, "<color=#A86629FF>Werewolf</color> Can Vent", false);

            #endregion

            #region Impostor Roles
            ImpostorDeceptionRoles = new CustomHeaderOption(num++, MultiMenu.Impostor, "<color=#FF0000FF>Impostor</color> <color=#1D7CF2FF>Deception</color> <color=#FFD700FF>Roles</color>");

            Escapist =
                new CustomHeaderOption(num++, MultiMenu.Impostor, "<color=#FF0000FF>Escapist</color>");
            EscapistOn = new CustomNumberOption(num++, MultiMenu.Impostor, "<color=#FF0000FF>Spawn Chance</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            EscapeCooldown =
                new CustomNumberOption(num++, MultiMenu.Impostor, "Recall Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            EscapistVent =
                new CustomToggleOption(num++, MultiMenu.Impostor, "Escapist Can Vent", false);            

            Grenadier =
                new CustomHeaderOption(num++, MultiMenu.Impostor, "<color=#FF0000FF>Grenadier</color>");
            GrenadierOn = new CustomNumberOption(num++, MultiMenu.Impostor, "<color=#FF0000FF>Spawn Chance</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            GrenadeCooldown =
                new CustomNumberOption(num++, MultiMenu.Impostor, "Flash Grenade Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            GrenadeDuration =
                new CustomNumberOption(num++, MultiMenu.Impostor, "Flash Grenade Duration", 10f, 5f, 15f, 1f, CooldownFormat);
            FlashRadius =
                new CustomNumberOption(num++, MultiMenu.Impostor, "Flash Radius", 1f, 0.25f, 5f, 0.25f, MultiplierFormat);
            GrenadierIndicators =
                new CustomToggleOption(num++, MultiMenu.Impostor, "Indicate Flashed Crewmates", false);
            GrenadierVent =
                new CustomToggleOption(num++, MultiMenu.Impostor, "Grenadier Can Vent", false);

            Morphling =
                new CustomHeaderOption(num++, MultiMenu.Impostor, "<color=#FF0000FF>Morphling</color>");
            MorphlingOn = new CustomNumberOption(num++, MultiMenu.Impostor, "<color=#FF0000FF>Spawn Chance</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            MorphlingCooldown =
                new CustomNumberOption(num++, MultiMenu.Impostor, "Morphling Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            MorphlingDuration =
                new CustomNumberOption(num++, MultiMenu.Impostor, "Morphling Duration", 10f, 5f, 15f, 1f, CooldownFormat);
            MorphlingVent =
                new CustomToggleOption(num++, MultiMenu.Impostor, "Morphling Can Vent", false);

            Swooper = new CustomHeaderOption(num++, MultiMenu.Impostor, "<color=#FF0000FF>Swooper</color>");
            SwooperOn = new CustomNumberOption(num++, MultiMenu.Impostor, "<color=#FF0000FF>Spawn Chance</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            SwoopCooldown =
                new CustomNumberOption(num++, MultiMenu.Impostor, "Swoop Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            SwoopDuration =
                new CustomNumberOption(num++, MultiMenu.Impostor, "Swoop Duration", 10f, 5f, 15f, 1f, CooldownFormat);
            SwooperVent =
                new CustomToggleOption(num++, MultiMenu.Impostor, "Swooper Can Vent", false);

            Venerer = new CustomHeaderOption(num++, MultiMenu.Impostor, "<color=#FF0000FF>Venerer</color>");
            VenererOn = new CustomNumberOption(num++, MultiMenu.Impostor, "<color=#FF0000FF>Spawn Chance</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            AbilityCooldown =
                new CustomNumberOption(num++, MultiMenu.Impostor, "Ability Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            AbilityDuration =
                new CustomNumberOption(num++, MultiMenu.Impostor, "Ability Duration", 10f, 5f, 15f, 1f, CooldownFormat);
            SprintSpeed = new CustomNumberOption(num++, MultiMenu.Impostor, "Sprint Speed", 1.25f, 1.05f, 2.5f, 0.05f, MultiplierFormat);
            FreezeSpeed = new CustomNumberOption(num++, MultiMenu.Impostor, "Freeze Speed", 0.75f, 0.25f, 1f, 0.05f, MultiplierFormat);

            ImpostorPower = new CustomHeaderOption(num++, MultiMenu.Impostor, "<color=#FF0000FF>Impostor</color> <color=#1D7CF2FF>Power</color> <color=#FFD700FF>Roles</color>");

            Bomber =
                new CustomHeaderOption(num++, MultiMenu.Impostor, "<color=#FF0000FF>Bomber</color>");
            BomberOn = new CustomNumberOption(num++, MultiMenu.Impostor, "<color=#FF0000FF>Spawn Chance</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            DetonateDelay =
                new CustomNumberOption(num++, MultiMenu.Impostor, "Detonate Delay", 5f, 1f, 15f, 1f, CooldownFormat);
            MaxKillsInDetonation =
                new CustomNumberOption(num++, MultiMenu.Impostor, "Max Kills In Detonation", 5, 1, 15, 1);
            DetonateRadius =
                new CustomNumberOption(num++, MultiMenu.Impostor, "Detonate Radius", 0.25f, 0.05f, 1f, 0.05f, MultiplierFormat);
            BomberVent =
                new CustomToggleOption(num++, MultiMenu.Impostor, "Bomber Can Vent", false);

            Warlock = new CustomHeaderOption(num++, MultiMenu.Impostor, "<color=#FF0000FF>Warlock</color>");
            WarlockOn = new CustomNumberOption(num++, MultiMenu.Impostor, "<color=#FF0000FF>Spawn Chance</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            ChargeUpDuration =
                new CustomNumberOption(num++, MultiMenu.Impostor, "Time It Takes To Fully Charge", 25f, 10f, 60f, 2.5f, CooldownFormat);
            ChargeUseDuration =
                new CustomNumberOption(num++, MultiMenu.Impostor, "Time It Takes To Use Full Charge", 1f, 0.05f, 5f, 0.05f, CooldownFormat);

            Witch = new CustomHeaderOption(num++, MultiMenu.Impostor, "<color=#FF0000FF>Witch</color>");
            WitchOn = new CustomNumberOption(num++, MultiMenu.Impostor, "<color=#FF0000FF>Spawn Chance</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            SpellCd =
                new CustomNumberOption(num++, MultiMenu.Impostor, "Spell Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            
            ImpostorSupportRoles = new CustomHeaderOption(num++, MultiMenu.Impostor, "<color=#FF0000FF>Impostor</color> <color=#1D7CF2FF>Support</color> <color=#FFD700FF>Roles</color>");    

            Blackmailer = new CustomHeaderOption(num++, MultiMenu.Impostor, "<color=#FF0000FF>Blackmailer</color>");
            BlackmailerOn = new CustomNumberOption(num++, MultiMenu.Impostor, "<color=#FF0000FF>Spawn Chance</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            BlackmailCooldown =
                new CustomNumberOption(num++, MultiMenu.Impostor, "Initial Blackmail Cooldown", 10f, 1f, 15f, 1f, CooldownFormat);
            BlackmailInvisible =
                new CustomToggleOption(num++, MultiMenu.Impostor, "Only Target Sees Blackmail", true);

            JanitorOn = new CustomNumberOption(num++, MultiMenu.Impostor, "<color=#FF0000FF>Janitor</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            Janitor = new CustomHeaderOption(num++, MultiMenu.Impostor, "<color=#FF0000FF>Spawn Chance</color>");

            Miner = new CustomHeaderOption(num++, MultiMenu.Impostor, "<color=#FF0000FF>Miner</color>");
            MinerOn = new CustomNumberOption(num++, MultiMenu.Impostor, "<color=#FF0000FF>Spawn Chance</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            MineCooldown =
                new CustomNumberOption(num++, MultiMenu.Impostor, "Mine Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);

            Undertaker = new CustomHeaderOption(num++, MultiMenu.Impostor, "<color=#FF0000FF>Undertaker</color>");
            UndertakerOn = new CustomNumberOption(num++, MultiMenu.Impostor, "<color=#FF0000FF>Spawn Chance</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            DragCooldown = new CustomNumberOption(num++, MultiMenu.Impostor, "Drag Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            UndertakerDragSpeed =
                new CustomNumberOption(num++, MultiMenu.Impostor, "Undertaker Drag Speed", 0.75f, 0.25f, 1f, 0.05f, MultiplierFormat);
            UndertakerVent =
                new CustomToggleOption(num++, MultiMenu.Impostor, "Undertaker Can Vent", false);
            UndertakerVentWithBody =
                new CustomToggleOption(num++, MultiMenu.Impostor, "Undertaker Can Vent While Dragging", false);
            
            #endregion

            #region Modifiers
            Modifiers = new CustomHeaderOption(num++, MultiMenu.ModifierAndAbility, "<color=#7F7F7FFF>Modifiers</color>");
            AftermathOn = new CustomNumberOption(num++, MultiMenu.ModifierAndAbility, "<color=#A6FFA6FF>Aftermath</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            Aftermath = new CustomHeaderOption(num++, MultiMenu.ModifierAndAbility, "<color=#A6FFA6FF>Spawn Chance</color>");

            Bait = new CustomHeaderOption(num++, MultiMenu.ModifierAndAbility, "<color=#00B3B3FF>Bait</color>");
            BaitOn = new CustomNumberOption(num++, MultiMenu.ModifierAndAbility, "<color=#00B3B3FF>Spawn Chance</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            BaitMinDelay = new CustomNumberOption(num++, MultiMenu.ModifierAndAbility, "Minimum Delay for the Bait Report", 0f, 0f, 15f, 0.5f, CooldownFormat);
            BaitMaxDelay = new CustomNumberOption(num++, MultiMenu.ModifierAndAbility, "Maximum Delay for the Bait Report", 1f, 0f, 15f, 0.5f, CooldownFormat);

            Diseased = new CustomHeaderOption(num++, MultiMenu.ModifierAndAbility, "<color=#808080FF>Diseased</color>");
            DiseasedOn = new CustomNumberOption(num++, MultiMenu.ModifierAndAbility, "<color=#808080FF>Spawn Chance</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            DiseasedKillMultiplier = new CustomNumberOption(num++, MultiMenu.ModifierAndAbility, "Diseased Kill Multiplier", 3f, 1.5f, 5f, 0.5f, MultiplierFormat);

            Frosty = new CustomHeaderOption(num++, MultiMenu.ModifierAndAbility, "<color=#99FFFFFF>Frosty</color>");
            FrostyOn = new CustomNumberOption(num++, MultiMenu.ModifierAndAbility, "<color=#99FFFFFF>Spawn Chance</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            ChillDuration = new CustomNumberOption(num++, MultiMenu.ModifierAndAbility, "Chill Duration", 10f, 1f, 15f, 1f, CooldownFormat);
            ChillStartSpeed = new CustomNumberOption(num++, MultiMenu.ModifierAndAbility, "Chill Start Speed", 0.75f, 0.25f, 0.95f, 0.05f, MultiplierFormat);
            
            MultitaskerOn = new CustomNumberOption(num++, MultiMenu.ModifierAndAbility, "<color=#FF804DFF>Multitasker</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            Multitasker = new CustomHeaderOption(num++, MultiMenu.ModifierAndAbility, "<color=#FF804DFF>Spawn Chance</color>");

            #endregion


            #region Abilities

            Abilities = new CustomHeaderOption(num++, MultiMenu.ModifierAndAbility, "<color=#7F7FFF>Abilities</color>");
            Assassin = new CustomHeaderOption(num++, MultiMenu.ModifierAndAbility, "<color=#FF0000FF>Assassin Ability</color>");
            NumberOfImpostorAssassins = new CustomNumberOption(num++, MultiMenu.ModifierAndAbility, "Number Of <color=#FF0000FF>Impsotor</color> Assassins", 1, 0, 4, 1);
            NumberOfNeutralAssassins = new CustomNumberOption(num++, MultiMenu.ModifierAndAbility, "Number Of <color=#B3B3B3FF>Neutral</color> Assassins", 1, 0, 5, 1);
            AmneTurnImpAssassin = new CustomToggleOption(num++, MultiMenu.ModifierAndAbility, "<color=#22FFFFFF>Amnesiac</color> Turned Impostor Gets Ability", false);
            AmneTurnNeutAssassin = new CustomToggleOption(num++, MultiMenu.ModifierAndAbility, "<color=#22FFFFFF>Amnesiac</color> Turned <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killing</color> Gets Ability", false);
            AssassinKills = new CustomNumberOption(num++, MultiMenu.ModifierAndAbility, "Number Of Assassin Kills", 1, 1, 15, 1);
            AssassinMultiKill = new CustomToggleOption(num++, MultiMenu.ModifierAndAbility, "Assassin Can Kill More Than Once Per Meeting", false);
            AssassinCrewmateGuess = new CustomToggleOption(num++, MultiMenu.ModifierAndAbility, "Assassin Can Guess \"<color=#8BFDFDFF>Crewmate</color>\"", false);
            AssassinGuessNeutralBenign = new CustomToggleOption(num++, MultiMenu.ModifierAndAbility, "Assassin Can Guess <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Benign</color> <color=#FFD700FF>Roles</color>", false);
            AssassinGuessNeutralEvil = new CustomToggleOption(num++, MultiMenu.ModifierAndAbility, "Assassin Can Guess <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Evil</color> <color=#FFD700FF>Roles</color>", false);

            ButtonBarryOn = new CustomNumberOption(num++, MultiMenu.ModifierAndAbility, "<color=#E600FFFF>Button Barry</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            ButtonBarry = new CustomHeaderOption(num++, MultiMenu.ModifierAndAbility, "<color=#E600FFFF>Spawn Chance</color>");

            Chameleon = new CustomHeaderOption(num++, MultiMenu.ModifierAndAbility, "<color=#FFB3CCFF>Chameleon</color>");
            ChameleonOn = new CustomNumberOption(num++, MultiMenu.ModifierAndAbility, "<color=#FFB3CCFF>Spawn Chance</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            InvisDelay = new CustomNumberOption(num++, MultiMenu.ModifierAndAbility, "Transparency Delay", 5f, 1f, 15f, 1f, CooldownFormat);
            TransformInvisDuration = new CustomNumberOption(num++, MultiMenu.ModifierAndAbility, "Turn Transparent Duration", 5f, 1f, 15f, 1f, CooldownFormat);
            FinalTransparency = new CustomNumberOption(num++, MultiMenu.ModifierAndAbility, "Final Opacity", 20f, 0f, 80f, 10f, PercentFormat);
            
            DrunkOn = new CustomNumberOption(num++, MultiMenu.ModifierAndAbility, "<color=#758000FF>Drunk</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            Drunk = new CustomHeaderOption(num++, MultiMenu.ModifierAndAbility, "<color=#758000FF>Spawn Chance</color>");
            
            Flash = new CustomHeaderOption(num++, MultiMenu.ModifierAndAbility, "<color=#FF8080FF>Flash</color>");
            FlashOn = new CustomNumberOption(num++, MultiMenu.ModifierAndAbility, "<color=#FF8080FF>Spawn Chance</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            FlashSpeed = new CustomNumberOption(num++, MultiMenu.ModifierAndAbility, "<color=#FF8080FF>Flash</color> Speed", 1.25f, 1.05f, 2.5f, 0.05f, MultiplierFormat);

            Giant = new CustomHeaderOption(num++, MultiMenu.ModifierAndAbility, "<color=#FFB34DFF>Giant</color>");
            GiantOn = new CustomNumberOption(num++, MultiMenu.ModifierAndAbility, "<color=#FFB34DFF>Spawn Chance</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            GiantSlow = new CustomNumberOption(num++, MultiMenu.ModifierAndAbility, "<color=#FFB34DFF>Giant</color> Speed", 0.75f, 0.25f, 1f, 0.05f, MultiplierFormat);

            RadarOn = new CustomNumberOption(num++, MultiMenu.ModifierAndAbility, "<color=#FF0080FF>Radar</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            Radar = new CustomHeaderOption(num++, MultiMenu.ModifierAndAbility, "<color=#FF0080FF>Spawn Chance</color>");
            
            SleuthOn = new CustomNumberOption(num++, MultiMenu.ModifierAndAbility, "<color=#803333FF>Sleuth</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            Sleuth = new CustomHeaderOption(num++, MultiMenu.ModifierAndAbility, "<color=#803333FF>Spawn Chance</color>");
            
            Spy =
                new CustomHeaderOption(num++, MultiMenu.ModifierAndAbility, "<color=#CCA3CCFF>Spy</color>");
            SpyOn = new CustomNumberOption(num++, MultiMenu.ModifierAndAbility, "<color=#CCA3CCFF>Spawn Chance</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            WhoSeesDead = new CustomStringOption(num++, MultiMenu.ModifierAndAbility, "Who Sees Dead Bodies On Admin",
                new[] { "Nobody", "<color=#CCA3CCFF>Spy</color>", "Everyone But <color=#CCA3CCFF>Spy</color>", "Everyone" });
            
            Tiebreaker = new CustomHeaderOption(num++, MultiMenu.ModifierAndAbility, "<color=#99E699FF>Tiebreaker</color>");
            TiebreakerOn = new CustomNumberOption(num++, MultiMenu.ModifierAndAbility, "<color=#99E699FF>Spawn Chance</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            
            Torch = new CustomHeaderOption(num++, MultiMenu.ModifierAndAbility, "<color=#FFFF99FF>Torch</color>");
            TorchOn = new CustomNumberOption(num++, MultiMenu.ModifierAndAbility, "<color=#FFFF99FF>Spawn Chance</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            
            #endregion

            #region Impostor Modifiers

            ImpostorModifiers = new CustomHeaderOption(num++, MultiMenu.ModifierAndAbility, "<color=#FF0000FF>Impostor</color> <color=#7F7F7FFF>Modifiers</color>");
            Disperser =
                new CustomHeaderOption(num++, MultiMenu.ModifierAndAbility, "<color=#FF0000FF>Disperser</color>");
            DisperserOn = new CustomNumberOption(num++, MultiMenu.ModifierAndAbility, "<color=#FF0000FF>Spawn Chance</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            DisperseCooldown =
                new CustomNumberOption(num++, MultiMenu.ModifierAndAbility, "Disperse Cooldown", 25f, 10f, 40f, 2.5f, CooldownFormat);
            MaxDisperses =
                new CustomNumberOption(num++, MultiMenu.ModifierAndAbility, "Maximum Number Of Disperses Per Game", 5, 1, 15, 1);
           
            DoubleShotOn = new CustomNumberOption(num++, MultiMenu.ModifierAndAbility, "<color=#FF0000FF>Double Shot</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            DoubleShot = new CustomHeaderOption(num++, MultiMenu.ModifierAndAbility, "<color=#FF0000FF>Spawn Chance</color>");
            
            Underdog = new CustomHeaderOption(num++, MultiMenu.ModifierAndAbility, "<color=#FF0000FF>Underdog</color>");
            UnderdogOn = new CustomNumberOption(num++, MultiMenu.ModifierAndAbility, "<color=#FF0000FF>Spawn Chance</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            UnderdogKillBonus = new CustomNumberOption(num++, MultiMenu.ModifierAndAbility, "Kill Cooldown Bonus", 5f, 2.5f, 10f, 2.5f, CooldownFormat);
            UnderdogIncreasedKC = new CustomToggleOption(num++, MultiMenu.ModifierAndAbility, "Increased Kill Cooldown When 2+ Imps", true);
            
            #endregion
        }
    }
}