namespace TownOfSushi
{
    public enum RoleEnum
    {
        Jester,
        Vulture,
        Engineer,
        Mayor,
        Investigator,
        Medic,
        Seer,
        Witch,
        Executioner,
        Glitch,
        Snitch,
        Arsonist,
        Jailor,
        Phantom,
        Vigilante,
        Werewolf,
        Haunter,
        Veteran,
        Amnesiac,
        Juggernaut,
        Tracker,
        Transporter,
        Medium,
        Trapper,
        Romantic,
        GuardianAngel,
        Mystic,
        Plaguebearer,
        Pestilence,
        SerialKiller,
        Hitman,
        Imitator,
        Doomsayer,
        Vampire,
        Prosecutor,
        Oracle,

        Miner,
        Swooper,
        Morphling,
        Janitor,
        Undertaker,
        Grenadier,
        Blackmailer,
        Escapist,
        Bomber,
        Warlock,
        Venerer,
        Agent,

        Crewmate,
        Impostor,

        Chameleon,
        None
    }

    public enum ModifierEnum
    {
        None,
        Diseased,
        Giant,
        Aftermath,
        Disperser,
        DoubleShot,
        Ghoul,
        Bait,
        Underdog,
        Frosty
    }
    public enum GhoulPunishment
    {
        Kill,
        LoseRole
    }

    public enum AbilityEnum
    {
        None,
        Assassin,
        Spy,
        Torch,
        Radar,
        Drunk,
        Sleuth,
        Tiebreaker,
        Multitasker,
        Chameleon,
        Flash,                
        ButtonBarry
    }
    public enum Faction
    {
        Crewmates,
        Impostors,
        Neutral,
        Modifier
    }
    public enum RoleAlignment
    {
        CrewKilling,
        CrewSupport,
        CrewPower,
        CrewSpecial,
        CrewInvest,
        NeutralBenign,
        NeutralEvil,
        NeutralKilling,
        ImpPower,
        ImpSpecial,
        ImpDeception,
        ImpSupport,

        Other,
        None
    }
    public enum CustomRPC
    {
        SetRole = 100,
        SetModifier,
        SetAbility,
        SetCouple,
        SetAssassin,
        SetTarget,
        SetGATarget,
        SetRomanticTarget,
        SetPhantom,
        CatchPhantom,
        SetHaunter,
        CatchHaunter,
        Maul,
        JanitorClean,
        FixLights,
        VultureEat,
        EngineerFix,
        Jail,
        Protect,
        Plant,
        AttemptSound,
        Morph,
        HitmanMorph,
        Mine,
        Swoop,
        BarryButton,
        Drag,
        Drop,
        HitmanDrag,
        HitmanDrop,
        AssassinKill,
        VigilanteKill,
        DoomsayerKill,
        FlashGrenade,
        Alert,
        Remember,
        BaitReport,
        Transport,
        SetUntransportable,
        Mediate,
        GAProtect,
        Blackmail,
        AbilityTrigger,
        Infect,
        TurnPestilence,
        Disperse,
        Escape,
        Revive,
        Convert,
        ChameleonSwoop,
        Imitate,
        StartImitate,
        Bite,
        Reveal,
        Prosecute,
        Confess,
        Camouflage,
        Spell,
        Shift,
        BypassKill,
        ResetVaribles,
        BypassMultiKill,
        SetMimic,
        SetHitmanMorph,
        RpcResetAnim,
        RpcResetAnim2,
        SetHacked,
        ExecutionerToJester,
        GAToSurv,
        RomanticChangeRole,
        Start,
        SyncCustomSettings,
        FixAnimation,
        SetPos,
        SetSettings,
        RemoveAllBodies,
        CheckMurder,
        SubmergedFixOxygen
    }
    public enum DeathReasonEnum
    {
        Alive,
        Ejected,
        Guessed,
        Killed,
        Executed,
        Cursed,
        Suicide
    }
    public enum DisableSkipButtonMeetings
    {
        No,
        Emergency,
        Always
    }
    public enum HaunterCanBeClickedBy
    {
        All,
        NonCrew,
        ImpsOnly
    }
    public enum BecomeOptions
    {
        Crew,
        Amnesiac,
        Jester
    }
    public enum RomanticBecomeOptions
    {
        Repick,
        Crew,
        Amnesiac,
        Jester
    }
    public enum DeadRevealed
    {
        Oldest = 0,
        Newest,
        All
    }
    public enum ProtectOptions
    {
        Self = 0,
        GA = 1,
        SelfAndGA = 2,
        Everyone = 3
    }
    public enum OnTargetDead
    {
        Crew,
        Amnesiac,
        Jester
    }
    public enum BecomeEnum
    {
        Crewmate = 0,
        Veteran,
        Vigilante
    }
    public enum GameMode
    {
        Classic,
        AllAny,
        KillersOnly
    }
    public enum AdminDeadPlayers
    {
        Nobody,
        Spy,
        EveryoneButSpy,
        Everyone
    }
    public enum MultiMenu
    {
        General,
        Crewmate,
        Neutral,
        Impostor,
        ModifierAndAbility,
        External
    }
    public enum CustomOptionType
    {
        Header,
        Toggle,
        Number,
        String,
        Button,
        Menu
    }
    public enum ShieldOptions
    {
        Self = 0,
        Medic = 1,
        SelfAndMedic = 2,
        Everyone = 3
    }

    public enum NotificationOptions
    {
        Medic = 0,
        Shielded = 1,
        Everyone = 2,
        Nobody = 3
    }
    internal enum RPC
    {
        PlayAnimation, //0
        CompleteTask, //1
        SyncSettings, //2
        SetRole, //3
        Exiled, //4
        CheckName, //5
        SetName, //6
        CheckColor, //7
        SetColor, //8
        SetHat, //9
        SetSkin, //10
        ReportDeadBody, //11
        MurderPlayer, //12
        SendChat, //13
        StartMeeting, //14
        SetScanner, //15
        SendChatNote, //16
        SetPet, //17
        SetStartCounter, //18
        EnterVent, //19
        ExitVent, //20
        SnapTo, //21
        Close, //22
        BBFDNCCEJHI, //23
        CastVote, //24
        ClearVote, //25
        AddVote, //26
        CloseDoorsOfType, //27
        RepairSystem, //28
        SetTasks, //29
        UpdateGameData //30
    }
    public enum CustomGameOverReason 
    {
        JesterWin = 11,
        ExecutionerWin = 12,
        DoomsayerWin = 13,
        VultureWin = 14,
        PhantomWin = 15,
        TeamVampiresWin = 16,
        GlitchWin = 17,
        JuggernautWin = 18,
        ArsonistWin = 19,
        PlaguebearerWin = 20,
        PestilenceWin = 21,
        WerewolfWin = 22,
        SerialKillerWin = 23,
        AgentWin = 24,
        HitmanWin = 25,
        ImpostorWin = 26,
        CrewmateWin = 27
    }

    public enum WinCondition 
    {
        Default,
        JesterWin,
        ExecutionerWin,
        DoomsayerWin,
        VultureWin,
        PhantomWin,
        TeamVampiresWin,
        GlitchWin,
        JuggernautWin,
        ArsonistWin,
        PlaguebearerWin,
        PestilenceWin,
        WerewolfWin,
        AdditionalGuardianAngelWin,
        AdditionalRomanticWin,
        SerialKillerWin,
        AgentWin,
        HitmanWin,
        ImpostorWin,
        CrewmateWin
    }
}
