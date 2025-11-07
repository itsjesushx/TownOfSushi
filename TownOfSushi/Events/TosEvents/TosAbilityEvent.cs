using MiraAPI.Events;
using UnityEngine;

namespace TownOfSushi.Events.TOSEvents;

/// <summary>
///     Event that is invoked after a player uses specific abilities. This event is not cancelable.
/// </summary>
public class TOSAbilityEvent : MiraEvent
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="TOSAbilityEvent" /> class.
    /// </summary>
    /// <param name="ability">The player's ability that was used.</param>
    /// <param name="player">The player who used the ability.</param>
    /// <param name="target">The player's target, if available.</param>
    /// <param name="target2">The player's second target, if available.</param>
    public TOSAbilityEvent(AbilityType ability, PlayerControl player, MonoBehaviour? target = null,
        MonoBehaviour? target2 = null)
    {
        AbilityType = ability;
        Player = player;
        Target = target;
        Target2 = target2;
    }

    /// <summary>
    ///     Gets the player who used the ability.
    /// </summary>
    public PlayerControl Player { get; }

    /// <summary>
    ///     Gets the target of the ability, if any.
    /// </summary>
    public MonoBehaviour? Target { get; set; }

    /// <summary>
    ///     Gets the second target of the ability, if any.
    /// </summary>
    public MonoBehaviour? Target2 { get; set; }

    /// <summary>
    ///     Gets the ability used by the player.
    /// </summary>
    public AbilityType AbilityType { get; }
}

public enum AbilityType
{
    RetributionistRevive,
    ClericBarrier,
    Poison,
    ClericCleanse,
    WarlockCurse,
    WarlockCurseKill,
    BodyGuardProtect,
    DeputyCamp,
    ConsigliereReveal,
    // InspectorExamine,
    // InspectorInspect,
    EngineerFix,

    // EngineerVent,
    HunterStalk,
    JailorJail,
    LookoutWatch,
    MedicShield,
    MediumMediate,
    OracleBless,
    OracleConfess,
    PlumberBlock,
    HitmanMorph,
    HitmanUnMorph,
    HitmanDrop,
    HitmanDrag,
    PlumberFlush,
    PoliticianCampaign,

    MonarchKnight,

    // DetectiveReveal,
    // VigilanteShoot,
    // TrackerTrack,
    TransporterTransport,
    RomanticProtect,

    // TrapperTrap,
    VeteranAlert,
    CrusaderFortify,
    BlackmailerBlackmail,
    BomberPlant,
    EclipsalBlind,
    EscapistMark,
    EscapistRecall,
    GrenadierFlash,
    HypnotistHypno,
    HypnotistHysteria,
    JanitorClean,
    MinerPlaceVent,
    MinerRevealVent,

    // MorphlingSample,
    MorphlingMorph,
    MorphlingUnmorph,
    SwooperSwoop,
    SwooperUnswoop,
    TraitorChangeRole,
    UndertakerDrag,
    UndertakerDrop,
    VenererCamoAbility,
    VenererSprintAbility,
    VenererFreezeAbility,

    // HexbladeBurstKill,
    AmnesiacPreRemember,
    AmnesiacPostRemember,
    PyromaniacDouse,

    // PyromaniacIgnite,
    GlitchInitialHack,
    GlitchHackTrigger,
    GlitchMimic,
    GlitchUnmimic,
    GuardianAngelProtect,

    ScavengerEat,
    ArsonistDouse,
    PlaguebearerInfect,
    AmnesiacVest,

    VampireBite,
    ThiefPreSteal,
    ThiefPostSteal
}