namespace TownOfSushi
{
    public enum RoleEnum
    {
        Jester,
        Vulture,
        Engineer,
        Investigator,
        Medic,
        Detective,
        Witch,
        Seer,
        Executioner,
        Glitch,
        Arsonist,
        Posioner,
        Jailor,
        Hunter,
        Swapper,
        Vigilante,
        Werewolf,
        Veteran,
        Amnesiac,
        Juggernaut,
        Poisoner,
        Tracker,
        Framer,
        Medium,
        Deputy,
        Trapper,
        Romantic,
        GuardianAngel,
        Mystic,
        Plaguebearer,
        Crusader,
        Pestilence,
        SerialKiller,
        Hitman,
        Imitator,
        Doomsayer,
        Vampire,
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
        Diseased,
        Giant,
        Aftermath,
        Disperser,
        Mini,
        DoubleShot,
        Bait,
        Underdog,
        Frosty,
        None
    }

    public enum AbilityEnum
    {
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
        ButtonBarry,
        None
    }
    public enum Faction
    {
        Crewmates,
        Impostors,
        Neutral
    }
    public enum RoleAlignment
    {
        CrewKilling,
        CrewSupport,
        CrewSpecial,
        CrewProtect,
        CrewInvest,
        NeutralBenign,
        NeutralEvil,
        NeutralKilling,
        ImpPower,
        ImpSpecial,
        ImpDeception,
        ImpSupport,
        None
    }
    public enum CustomRPC
    {
        SetRole = 100,
        SetModifier,
        SetAbility,
        SetAssassin,
        SetTarget,
        SetGATarget,        
        SetRomanticTarget,
        SetFramerTarget,
        Spell,

        VampireWin,
        GlitchWin,
        JuggernautWin,
        ArsonistWin,
        PlaguebearerWin,
        PestilenceWin,
        WerewolfWin,
        SerialKillerWin,
        AgentWin,
        HitmanWin,
        ImpostorWin,
        CrewmateWin,
        JesterWin,
        VultureWin,
        ExecutionerWin,
        DoomsayerWin,
        NobodyWins,
        FramerWin,


        Maul,
        JanitorClean,
        FixLights,
        VultureEat,
        EngineerFix,
        Fortify,
        FortifyKill,
        Jail,
        ExecuteDeputyKill,
        Protect,
        Plant,
        AttemptSound,
        Morph,
        HitmanMorph,
        Mine,
        Swoop,
        BarryButton,
        HunterStalk,
        Drag,
        Drop,
        HitmanDrag,
        ShareOptions,
        HitmanDrop,
        Poison,
        PoisonKill,
        AssassinKill,
        DeputyKill,
        VigilanteKill,
        DoomsayerKill,
        FlashGrenade,
        Retribution,
        Alert,
        Remember,
        StartRemember,
        BaitReport,
        Mediate,
        GAProtect,
        Blackmail,
        AbilityTrigger,
        Infect,
        TurnPestilence,
        VersionHandshake,
        Disperse,
        Escape,
        Imitate,
        SetGameStarting,
        StopStart,
        StartImitate,
        Bite,
        SetSwaps,
        Reveal,
        Confess,
        Camouflage,
        BypassKill,
        BypassKill2,
        ResetVaribles,
        BypassMultiKill,
        SetMimic,
        SetHitmanMorph,
        RpcResetAnim,
        RpcResetAnim2,
        SetHacked,
        ExecutionerToJester,
        GuardianAngelChangeRole,
        RomanticChangeRole,
        Start,
        FixAnimation,
        SetPos,
        SetSettings,
        RemoveAllBodies,
        CheckMurder,
        SubmergedFixOxygen
    }
    public enum CustomDeathReason 
    {
        Exile,
        Poisoned,
        Kill,
        Disconnect,
        Guess,
        WitchExile,
        Bombed,
        Executed,
        ExecutedByDeputy,
        Arson,
    }
    public enum DisableSkipButtonMeetings
    {
        No,
        Emergency,
        Always
    }
    public enum BecomeOptions
    {
        Crew,
        Amnesiac,
        Jester,
        Framer,
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
        Jester,
        Framer
    }
    public enum GameMode
    {
        Classic,
        AllAny
    }
    public enum AdminDeadPlayers
    {
        Nobody,
        Spy,
        EveryoneButSpy,
        Everyone
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
    public enum CustomPlayerOutfitType 
    {
        Default,
        Shapeshifted,
        Morph,
        Camouflage,
        Swooper,
        PlayerNameOnly

    }
}
