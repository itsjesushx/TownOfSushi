using MiraAPI.LocalSettings;
using Reactor.Utilities;
using UnityEngine;

namespace TownOfSushi.Assets;

public static class TownOfSushiAssets
{
    #region Consts
    private const string ShortPath = "TownOfSushi.Resources";
    private const string CounterPath = $"{ShortPath}.AbilityCounters";
    private const string CrewButtonPath = $"{ShortPath}.CrewButtons";
    private const string NeutralButtonsPath = $"{ShortPath}.NeutButtons";
    private const string ImpButtonPath = $"{ShortPath}.ImpButtons";
    private static readonly string RoleIconsPath = $"{ShortPath}.RoleIcons";
    private static readonly string ModifierIconsPath = $"{ShortPath}.ModifierIcons";
    #endregion

    #region Main Assets

    public static readonly AssetBundle MainBundle = AssetBundleManager.Load("tou-assets");

    public static readonly LoadableAsset<GameObject> RoleSelectionGame =
        new LoadableBundleAsset<GameObject>("SelectRoleGame", MainBundle);

    public static LoadableAsset<GameObject> WikiPrefab { get; } =
        new LoadableBundleAsset<GameObject>("IngameWiki", MainBundle);

    public static LoadableAsset<GameObject> MedicShield { get; } =
        new LoadableBundleAsset<GameObject>("MedicShield", MainBundle);
    
    public static LoadableAsset<GameObject> WardenFort { get; } =
        new LoadableBundleAsset<GameObject>("WardenFort", MainBundle);

    public static LoadableAsset<GameObject> EclipsedPrefab { get; } =
        new LoadableBundleAsset<GameObject>("Eclipsed", MainBundle);

    public static LoadableAsset<GameObject> EscapistMarkPrefab { get; } =
        new LoadableBundleAsset<GameObject>("EscapistMark", MainBundle);

    public static LoadableAsset<Sprite> Banner { get; } = 
        new LoadableResourceAsset($"{ShortPath}.Banner.png");

    public static LoadableAsset<Sprite> BlankSprite { get; } =
        new LoadableResourceAsset($"{ShortPath}.BlankSprite.png");

    public static LoadableAsset<Sprite> WikiButton { get; } = 
        new LoadableResourceAsset($"{ShortPath}.WikiButton.png");

    public static LoadableAsset<Sprite> WikiButtonActive { get; } =
        new LoadableResourceAsset($"{ShortPath}.WikiButtonActive.png");

    public static LoadableAsset<Sprite> ZoomPlus { get; } = 
        new LoadableResourceAsset($"{ShortPath}.Plus.png");
    public static LoadableAsset<Sprite> ZoomMinus { get; } = 
        new LoadableResourceAsset($"{ShortPath}.Minus.png");

    public static LoadableAsset<Sprite> ZoomPlusActive { get; } =
        new LoadableResourceAsset($"{ShortPath}.PlusActive.png");

    public static LoadableAsset<Sprite> ZoomMinusActive { get; } =
        new LoadableResourceAsset($"{ShortPath}.MinusActive.png");

    public static LoadableAsset<Sprite> TeamChatInactive { get; } =
        new LoadableResourceAsset($"{ShortPath}.TeamChatInactive.png");

    public static LoadableAsset<Sprite> TeamChatActive { get; } =
        new LoadableResourceAsset($"{ShortPath}.TeamChatActive.png");

    public static LoadableAsset<Sprite> TeamChatSelected { get; } =
        new LoadableResourceAsset($"{ShortPath}.TeamChatSelected.png");

    public static LoadableAsset<Sprite> BarryButtonSprite { get; } =
        new LoadableResourceAsset($"{ShortPath}.BarryButton.png");

    public static LoadableAsset<Sprite> BroadcastSprite { get; } =
        new LoadableResourceAsset($"{ShortPath}.BroadcastButton.png");

    public static LoadableAsset<Sprite> DisperseSprite { get; } =
        new LoadableResourceAsset($"{ShortPath}.DisperseButton.png");

    public static LoadableAsset<Sprite> VitalsSprite { get; } =
        new LoadableResourceAsset($"{ShortPath}.VitalsButton.png");

    public static LoadableAsset<Sprite> CameraSprite { get; } = 
        new LoadableResourceAsset($"{ShortPath}.CamButton.png");

    public static LoadableAsset<Sprite> AdminSprite { get; } =
        new LoadableResourceAsset($"{ShortPath}.AdminButton.png");

    public static LoadableAsset<Sprite> KillSprite { get; } = 
        new LoadableResourceAsset($"{ShortPath}.KillButton.png");
    public static LoadableAsset<Sprite> VentSprite { get; } = 
        new LoadableResourceAsset($"{ShortPath}.VentButton.png");

    public static LoadableAsset<Sprite> HysteriaSprite { get; } =
        new LoadableResourceAsset($"{ShortPath}.Hysteria.png", 300);

    public static LoadableAsset<Sprite> HysteriaCleanSprite { get; } =
        new LoadableResourceAsset($"{ShortPath}.HysteriaClean.png", 300);

    public static LoadableAsset<Sprite> ShootMeetingSprite { get; } =
        new LoadableResourceAsset($"{ShortPath}.Shoot.png", 300);

    public static LoadableAsset<Sprite> BlackmailLetterSprite { get; } =
        new LoadableResourceAsset($"{ShortPath}.BlackmailLetter.png");

    public static LoadableAsset<Sprite> BlackmailOverlaySprite { get; } =
        new LoadableResourceAsset($"{ShortPath}.BlackmailOverlay.png");

    public static LoadableAsset<Sprite> FootprintSprite { get; } =
        new LoadableResourceAsset($"{ShortPath}.Footprint.png");

    public static LoadableAsset<Sprite> SwapActive { get; } =
        new LoadableResourceAsset($"{ShortPath}.SwapActive.png", 300);

    public static LoadableAsset<Sprite> SwapInactive { get; } =
        new LoadableResourceAsset($"{ShortPath}.SwapDisabled.png", 300);

    public static LoadableAsset<Sprite> RevealButtonSprite { get; } =
        new LoadableResourceAsset($"{ShortPath}.Reveal.png", 300);

    public static LoadableAsset<Sprite> RevealCleanSprite { get; } =
        new LoadableResourceAsset($"{ShortPath}.RevealClean.png", 300);

    public static LoadableAsset<Sprite> Guess { get; } = 
        new LoadableResourceAsset($"{ShortPath}.Guess.png", 300);
    public static LoadableAsset<Sprite> InJailSprite { get; } = 
        new LoadableResourceAsset($"{ShortPath}.InJail.png");

    public static LoadableAsset<Sprite> JailCellSprite { get; } =
        new LoadableResourceAsset($"{ShortPath}.JailCell.png");

    public static LoadableAsset<Sprite> ImitateSelectSprite { get; } =
        new LoadableResourceAsset($"{ShortPath}.ImitateSelect.png", 300);

    public static LoadableAsset<Sprite> ImitateDeselectSprite { get; } =
        new LoadableResourceAsset($"{ShortPath}.ImitateDeselect.png", 300);

    public static LoadableAsset<Sprite> ExecuteSprite { get; } =
        new LoadableResourceAsset($"{ShortPath}.Execute.png", 254);

    public static LoadableAsset<Sprite> ExecuteCleanSprite { get; } =
        new LoadableResourceAsset($"{ShortPath}.ExecuteClean.png", 254);

    public static LoadableAsset<Sprite> Hacked { get; } = 
        new LoadableResourceAsset($"{ShortPath}.Hacked.png");

    public static LoadableAsset<Sprite> BarricadeVentSprite { get; } =
        new LoadableResourceAsset($"{ShortPath}.BarricadeVent.png", 200);

    public static LoadableAsset<Sprite> BarricadeFungleSprite { get; } =
        new LoadableResourceAsset($"{ShortPath}.BarricadePlant.png", 200);

    public static LoadableAsset<Sprite> LighterSprite { get; } = 
        new LoadableResourceAsset($"{ShortPath}.Lighter.png");
    public static LoadableAsset<Sprite> DarkerSprite { get; } = 
        new LoadableResourceAsset($"{ShortPath}.Darker.png");

    public static LoadableAsset<Sprite> SushiRollSprite { get; } =
        new LoadableResourceAsset($"{ShortPath}.SushiRoll.png", 290);

    public static LoadableAsset<Sprite> ArrowSprite
    {
        get
        {
            var sprite = ArrowBasicSprite;
            switch (LocalSettingsTabSingleton<TownOfSushiLocalSettings>.Instance.ArrowStyleEnum.Value)
            {
                case ArrowStyleType.DarkGlow:
                    sprite = ArrowDarkOutSprite;
                    break;
                case ArrowStyleType.ColorGlow:
                    sprite = ArrowLightOutSprite;
                    break;
                case ArrowStyleType.Legacy:
                    sprite = ArrowLegacySprite;
                    break;
            }
            return sprite;
        }
    }

    public static LoadableAsset<Sprite> ArrowBasicSprite { get; } =
         new LoadableResourceAsset($"{ShortPath}.Arrow.png", 110);
    public static LoadableAsset<Sprite> ArrowDarkOutSprite { get; } = 
        new LoadableResourceAsset($"{ShortPath}.ArrowDarkOut.png", 110);
    public static LoadableAsset<Sprite> ArrowLightOutSprite { get; } = 
        new LoadableResourceAsset($"{ShortPath}.ArrowLightOut.png", 110);
    
    public static LoadableAsset<Sprite> ArrowLegacySprite { get; } =
        new LoadableResourceAsset($"{ShortPath}.ArrowLegacy.png");

    public static LoadableAsset<Sprite> CrimeSceneSprite { get; } =
        new LoadableResourceAsset($"{ShortPath}.CrimeScene.png");

    public static LoadableAsset<Sprite> ScreenFlash { get; } =
        new LoadableResourceAsset($"{ShortPath}.ScreenFlash.png");

    public static LoadableAsset<Sprite> KillBG { get; } = 
        new LoadableResourceAsset($"{ShortPath}.KillBackground.png");

    public static LoadableAsset<Sprite> RetributionBG { get; } =
        new LoadableResourceAsset($"{ShortPath}.RetributionBackground.png");

    public static LoadableAsset<Sprite> AbilityCounterPlayerSprite { get; } =
        new LoadableResourceAsset($"{CounterPath}.Player.png");

    public static LoadableAsset<Sprite> AbilityCounterVentSprite { get; } =
        new LoadableResourceAsset($"{CounterPath}.Vent.png");

    public static LoadableAsset<Sprite> AbilityCounterBodySprite { get; } =
        new LoadableResourceAsset($"{CounterPath}.Body.png");

    public static LoadableAsset<Sprite> AbilityCounterBasicSprite { get; } =
        new LoadableResourceAsset($"{CounterPath}.Basic.png");

    public static LoadableAsset<Sprite> GameSummarySprite { get; } =
        new LoadableResourceAsset($"{ShortPath}.GameSummaryButton.png");

    public static LoadableAsset<Sprite> MapVentSprite { get; } =
        new LoadableResourceAsset($"{ShortPath}.MapVent.png", 350);

    public static LoadableAsset<Sprite> MapBodySprite { get; } =
        new LoadableResourceAsset($"{ShortPath}.MapBody.png", 350);

    public static LoadableAsset<Sprite> WikiBgSprite { get; } = 
        new LoadableResourceAsset($"{ShortPath}.WikiBg.png");

    public static LoadableAsset<Sprite> TimerDrawSprite { get; } =
        new LoadableResourceAsset($"{ShortPath}.TimerDraw.png", 300);

    public static LoadableAsset<Sprite> TimerImpSprite { get; } =
        new LoadableResourceAsset($"{ShortPath}.TimerImp.png", 300);
    
    #endregion
    

    #region Crew Assets
    public static LoadableAsset<Sprite> InspectSprite { get; } =
        new LoadableResourceAsset($"{CrewButtonPath}.InspectButton.png");
    
    public static LoadableAsset<Sprite> Observe { get; } =
        new LoadableResourceAsset($"{CrewButtonPath}.ObserveButton.png");

    public static LoadableAsset<Sprite> GuardSprite { get; } =
        new LoadableResourceAsset($"{CrewButtonPath}.GuardButton.png");

    public static LoadableAsset<Sprite> ExamineSprite { get; } =
        new LoadableResourceAsset($"{CrewButtonPath}.ExamineButton.png");

    public static LoadableAsset<Sprite> WatchSprite { get; } =
        new LoadableResourceAsset($"{CrewButtonPath}.WatchButton.png");
    
    public static LoadableAsset<Sprite> KnightSprite { get; } =
        new LoadableResourceAsset($"{CrewButtonPath}.KnightButton.png");

    public static LoadableAsset<Sprite> ConfessSprite { get; } =
        new LoadableResourceAsset($"{CrewButtonPath}.ConfessButton.png");

    public static LoadableAsset<Sprite> BlessSprite { get; } =
        new LoadableResourceAsset($"{CrewButtonPath}.BlessButton.png");

    public static LoadableAsset<Sprite> SeerSprite { get; } = 
        new LoadableResourceAsset($"{CrewButtonPath}.SeerButton.png");

    public static LoadableAsset<Sprite> TrackSprite { get; } =
        new LoadableResourceAsset($"{CrewButtonPath}.TrackButton.png");

    public static LoadableAsset<Sprite> AnalyzerButton { get; } =
        new LoadableResourceAsset($"{CrewButtonPath}.AnalyzerButton.png");

    public static LoadableAsset<Sprite> TrapSprite { get; } = 
        new LoadableResourceAsset($"{CrewButtonPath}.TrapButton.png");

    public static LoadableAsset<Sprite> CampButtonSprite { get; } =
        new LoadableResourceAsset($"{CrewButtonPath}.CampButton.png");

    public static LoadableAsset<Sprite> StalkButtonSprite { get; } =
        new LoadableResourceAsset($"{CrewButtonPath}.StalkButton.png");

    public static LoadableAsset<Sprite> JailSprite { get; } = 
        new LoadableResourceAsset($"{CrewButtonPath}.JailButton.png");

    public static LoadableAsset<Sprite> AlertSprite { get; } =
        new LoadableResourceAsset($"{CrewButtonPath}.AlertButton.png");

    public static LoadableAsset<Sprite> HunterKillSprite { get; } =
        new LoadableResourceAsset($"{CrewButtonPath}.HunterKillButton.png");

    public static LoadableAsset<Sprite> SheriffShootSprite { get; } =
        new LoadableResourceAsset($"{CrewButtonPath}.SheriffShootButton.png");

    public static LoadableAsset<Sprite> CleanseSprite { get; } =
        new LoadableResourceAsset($"{CrewButtonPath}.CleanseButton.png");

    public static LoadableAsset<Sprite> BarrierSprite { get; } =
        new LoadableResourceAsset($"{CrewButtonPath}.BarrierButton.png");

    public static LoadableAsset<Sprite> MedicSprite { get; } =
        new LoadableResourceAsset($"{CrewButtonPath}.MedicButton.png");

    public static LoadableAsset<Sprite> FortifySprite { get; } =
        new LoadableResourceAsset($"{CrewButtonPath}.FortifyButton.png");

    public static LoadableAsset<Sprite> FixButtonSprite { get; } =
        new LoadableResourceAsset($"{CrewButtonPath}.FixButton.png");

    public static LoadableAsset<Sprite> EngiVentSprite { get; } =
        new LoadableResourceAsset($"{CrewButtonPath}.EngiVentButton.png");

    public static LoadableAsset<Sprite> MediateSprite { get; } =
        new LoadableResourceAsset($"{CrewButtonPath}.MediateButton.png");
    
    public static LoadableAsset<Sprite> SoulSprite { get; } =
        new LoadableResourceAsset($"{CrewButtonPath}.Soul.png", 300f);

    public static LoadableAsset<Sprite> CampaignButtonSprite { get; } =
        new LoadableResourceAsset($"{CrewButtonPath}.CampaignButton.png");

    public static LoadableAsset<Sprite> FlushSprite { get; } =
        new LoadableResourceAsset($"{CrewButtonPath}.FlushButton.png");

    public static LoadableAsset<Sprite> BarricadeSprite { get; } =
        new LoadableResourceAsset($"{CrewButtonPath}.BarricadeButton.png");

    public static LoadableAsset<Sprite> Transport { get; } =
        new LoadableResourceAsset($"{CrewButtonPath}.TransportButton.png");
    
    #endregion

    #region Imp Assets
    public static LoadableAsset<Sprite> MarkSprite { get; } = 
        new LoadableResourceAsset($"{ImpButtonPath}.MarkButton.png");

    public static LoadableAsset<Sprite> RecallSprite { get; } =
        new LoadableResourceAsset($"{ImpButtonPath}.RecallButton.png");
    
    public static LoadableAsset<Sprite> ConsigliereRevealSprite { get; } =
        new LoadableResourceAsset($"{ImpButtonPath}.ConsigliereReveal.png");
    
    public static LoadableAsset<Sprite> AmbushSprite { get; } =
        new LoadableResourceAsset($"{ImpButtonPath}.AmbushButton.png");

    public static LoadableAsset<Sprite> PursueSprite { get; } =
        new LoadableResourceAsset($"{ImpButtonPath}.PursueButton.png");
    
    public static LoadableAsset<Sprite> WarlockCurseButton { get; } =
        new LoadableResourceAsset($"{ImpButtonPath}.CurseButton.png");
    
    public static LoadableAsset<Sprite> WarlockCurseKillButton { get; } =
        new LoadableResourceAsset($"{ImpButtonPath}.CurseKillButton.png", 80f);

    public static LoadableAsset<Sprite> FlashSprite { get; } =
        new LoadableResourceAsset($"{ImpButtonPath}.FlashButton.png");
    
    public static LoadableAsset<Sprite> CurseSprite { get; } =
        new LoadableResourceAsset($"{ImpButtonPath}.SpellButton.png");

    public static LoadableAsset<Sprite> SampleSprite { get; } =
        new LoadableResourceAsset($"{ImpButtonPath}.SampleButton.png");
    
    public static LoadableAsset<Sprite> PaintSprite { get; } =
        new LoadableResourceAsset($"{ImpButtonPath}.PaintSprite.png");

    public static LoadableAsset<Sprite> MorphSprite { get; } =
        new LoadableResourceAsset($"{ImpButtonPath}.MorphButton.png");

    public static LoadableAsset<Sprite> SwoopSprite { get; } =
        new LoadableResourceAsset($"{ImpButtonPath}.SwoopButton.png");

    public static LoadableAsset<Sprite> UnswoopSprite { get; } =
        new LoadableResourceAsset($"{ImpButtonPath}.UnswoopButton.png");

    public static LoadableAsset<Sprite> NoAbilitySprite { get; } =
        new LoadableResourceAsset($"{ImpButtonPath}.NoAbilityButton.png");

    public static LoadableAsset<Sprite> CamouflageSprite { get; } =
        new LoadableResourceAsset($"{ImpButtonPath}.CamouflageButton.png");

    public static LoadableAsset<Sprite> SprintSprite { get; } =
        new LoadableResourceAsset($"{ImpButtonPath}.CamoSprintButton.png");

    public static LoadableAsset<Sprite> FreezeSprite { get; } =
        new LoadableResourceAsset($"{ImpButtonPath}.CamoSprintFreezeButton.png");
    
    public static LoadableAsset<Sprite> SpellSprite { get; } =
        new LoadableResourceAsset($"{ImpButtonPath}.SpellButton.png");

    public static LoadableAsset<Sprite> PlaceSprite { get; } =
        new LoadableResourceAsset($"{ImpButtonPath}.PlaceButton.png");

    public static LoadableAsset<Sprite> DetonatingSprite { get; } =
        new LoadableResourceAsset($"{ImpButtonPath}.DetonatingButton.png");

    public static LoadableAsset<Sprite> PlantSprite { get; } =
        new LoadableResourceAsset($"{ImpButtonPath}.PlantButton.png");

    public static LoadableAsset<Sprite> BlackmailSprite { get; } =
        new LoadableResourceAsset($"{ImpButtonPath}.BlackmailButton.png");

    public static LoadableAsset<Sprite> HypnotiseButtonSprite { get; } =
        new LoadableResourceAsset($"{ImpButtonPath}.HypnotiseButton.png");
    
    public static LoadableAsset<Sprite> PoisonSprite { get; } =
        new LoadableResourceAsset($"{ImpButtonPath}.PoisonButton.png");

    public static LoadableAsset<Sprite> CleanButtonSprite { get; } =
        new LoadableResourceAsset($"{ImpButtonPath}.CleanButton.png");

    public static LoadableAsset<Sprite> MineSprite { get; } = 
        new LoadableResourceAsset($"{ImpButtonPath}.MineButton.png");
    public static LoadableAsset<Sprite> DragSprite { get; } = 
        new LoadableResourceAsset($"{ImpButtonPath}.DragButton.png");
    public static LoadableAsset<Sprite> DropSprite { get; } = 
        new LoadableResourceAsset($"{ImpButtonPath}.DropButton.png");
    #endregion

    #region Neut Assets
    public static LoadableAsset<Sprite> RememberButtonSprite { get; } =
        new LoadableResourceAsset($"{NeutralButtonsPath}.RememberButton.png");    
    public static LoadableAsset<Sprite> EatSprite { get; } =
        new LoadableResourceAsset($"{NeutralButtonsPath}.ScavengerButton.png");
    public static LoadableAsset<Sprite> ScavengeSprite { get; } =
        new LoadableResourceAsset($"{NeutralButtonsPath}.ScavengeButton.png");
    
    public static LoadableAsset<Sprite> ExeTormentSprite { get; } =
        new LoadableResourceAsset($"{NeutralButtonsPath}.ExeTormentButton.png");

    public static LoadableAsset<Sprite> ProtectSprite { get; } =
        new LoadableResourceAsset($"{NeutralButtonsPath}.ProtectButton.png");
    
    public static LoadableAsset<Sprite> BlindSprite { get; } =
        new LoadableResourceAsset($"{NeutralButtonsPath}.BlindButton.png");

    public static LoadableAsset<Sprite> BribeSprite { get; } =
        new LoadableResourceAsset($"{NeutralButtonsPath}.BribeButton.png");
    
    public static LoadableAsset<Sprite> RomanticProtect { get; } =
        new LoadableResourceAsset($"{NeutralButtonsPath}.RomanticProtect.png");
    public static LoadableAsset<Sprite> RomanticPick { get; } =
        new LoadableResourceAsset($"{NeutralButtonsPath}.RomanticPick.png");

    public static LoadableAsset<Sprite> VestSprite { get; } = 
        new LoadableResourceAsset($"{NeutralButtonsPath}.VestButton.png");

    public static LoadableAsset<Sprite> JesterHauntSprite { get; } =
        new LoadableResourceAsset($"{NeutralButtonsPath}.JesterHauntButton.png");

    public static LoadableAsset<Sprite> JesterVentSprite { get; } =
        new LoadableResourceAsset($"{NeutralButtonsPath}.JesterVentButton.png");

    public static LoadableAsset<Sprite> InquisKillSprite { get; } =
        new LoadableResourceAsset($"{NeutralButtonsPath}.InquisKillButton.png");
    
    public static LoadableAsset<Sprite> HitmanKillSprite { get; } =
        new LoadableResourceAsset($"{NeutralButtonsPath}.HitmanKillButton.png");

    public static LoadableAsset<Sprite> InquireSprite { get; } =
        new LoadableResourceAsset($"{NeutralButtonsPath}.InquireButton.png");

    public static LoadableAsset<Sprite> DouseButtonSprite { get; } =
        new LoadableResourceAsset($"{NeutralButtonsPath}.DouseButton.png");

    public static LoadableAsset<Sprite> IgniteButtonSprite { get; } =
        new LoadableResourceAsset($"{NeutralButtonsPath}.IgniteButton.png");

    public static LoadableAsset<Sprite> ArsoVentSprite { get; } =
        new LoadableResourceAsset($"{NeutralButtonsPath}.ArsoVentButton.png");

    public static LoadableAsset<Sprite> HackSprite { get; } = 
        new LoadableResourceAsset($"{NeutralButtonsPath}.HackButton.png");

    public static LoadableAsset<Sprite> MimicSprite { get; } =
        new LoadableResourceAsset($"{NeutralButtonsPath}.MimicButton.png");

    public static LoadableAsset<Sprite> GlitchVentSprite { get; } =
        new LoadableResourceAsset($"{NeutralButtonsPath}.GlitchVentButton.png");

    public static LoadableAsset<Sprite> GlitchKillSprite { get; } =
        new LoadableResourceAsset($"{NeutralButtonsPath}.GlitchKillButton.png");

    public static LoadableAsset<Sprite> JuggKillSprite { get; } =
        new LoadableResourceAsset($"{NeutralButtonsPath}.JuggKillButton.png");

    public static LoadableAsset<Sprite> JuggVentSprite { get; } =
        new LoadableResourceAsset($"{NeutralButtonsPath}.JuggVentButton.png");

    public static LoadableAsset<Sprite> InfectSprite { get; } =
        new LoadableResourceAsset($"{NeutralButtonsPath}.InfectButton.png");

    public static LoadableAsset<Sprite> PestKillSprite { get; } =
        new LoadableResourceAsset($"{NeutralButtonsPath}.PestKillButton.png");

    public static LoadableAsset<Sprite> PestVentSprite { get; } =
        new LoadableResourceAsset($"{NeutralButtonsPath}.PestVentButton.png");

    public static LoadableAsset<Sprite> ReapSprite { get; } =
        new LoadableResourceAsset($"{NeutralButtonsPath}.ReapButton.png");

    public static LoadableAsset<Sprite> ReaperVentSprite { get; } =
        new LoadableResourceAsset($"{NeutralButtonsPath}.ReaperVentButton.png");

    public static LoadableAsset<Sprite> BiteSprite { get; } = 
        new LoadableResourceAsset($"{NeutralButtonsPath}.BiteButton.png");

    public static LoadableAsset<Sprite> VampVentSprite { get; } =
        new LoadableResourceAsset($"{NeutralButtonsPath}.VampVentButton.png");

    public static LoadableAsset<Sprite> TerminateSprite { get; } =
        new LoadableResourceAsset($"{NeutralButtonsPath}.TerminateButton.png");
    
    public static LoadableAsset<Sprite> MaulSprite { get; } =
        new LoadableResourceAsset($"{NeutralButtonsPath}.MaulButton.png");

    public static LoadableAsset<Sprite> PredatorKillSprite { get; } =
        new LoadableResourceAsset($"{NeutralButtonsPath}.WolfKillButton.png");

    public static LoadableAsset<Sprite> PredatorVentSprite { get; } =
        new LoadableResourceAsset($"{NeutralButtonsPath}.WolfVentButton.png");
    #endregion
    #region Modifier Icons
    public static LoadableAsset<Sprite> Aftermath { get; } = 
        new LoadableResourceAsset($"{ModifierIconsPath}.Aftermath.png");
    public static LoadableAsset<Sprite> Bait { get; } = 
        new LoadableResourceAsset($"{ModifierIconsPath}.Bait.png");
    public static LoadableAsset<Sprite> Armored { get; } = 
        new LoadableResourceAsset($"{ModifierIconsPath}.Armored.png");
    public static LoadableAsset<Sprite> Celebrity { get; } = 
        new LoadableResourceAsset($"{ModifierIconsPath}.Celebrity.png");
    public static LoadableAsset<Sprite> Swapper { get; } = 
        new LoadableResourceAsset($"{ModifierIconsPath}.Swapper.png");
    public static LoadableAsset<Sprite> Diseased { get; } =  
        new LoadableResourceAsset($"{ModifierIconsPath}.Diseased.png");
    public static LoadableAsset<Sprite> Frosty { get; } =  
        new LoadableResourceAsset($"{ModifierIconsPath}.Frosty.png");
    public static LoadableAsset<Sprite> Multitasker { get; } =  
        new LoadableResourceAsset($"{ModifierIconsPath}.Multitasker.png");
    public static LoadableAsset<Sprite> Noisemaker { get; } =  
        new LoadableResourceAsset($"{ModifierIconsPath}.Noisemaker.png");
    public static LoadableAsset<Sprite> Decay { get; } =  
        new LoadableResourceAsset($"{ModifierIconsPath}.Decay.png");
    public static LoadableAsset<Sprite> Scientist { get; } =  
        new LoadableResourceAsset($"{ModifierIconsPath}.Scientist.png");
    public static LoadableAsset<Sprite> Scout { get; } =  
        new LoadableResourceAsset($"{ModifierIconsPath}.Scout.png");
    public static LoadableAsset<Sprite> Taskmaster { get; } =  
        new LoadableResourceAsset($"{ModifierIconsPath}.Taskmaster.png");
    public static LoadableAsset<Sprite> Torch { get; } =  
        new LoadableResourceAsset($"{ModifierIconsPath}.Torch.png");

    public static LoadableAsset<Sprite> Disperser { get; } =  
        new LoadableResourceAsset($"{ModifierIconsPath}.Disperser.png");
    public static LoadableAsset<Sprite> DoubleShot { get; } =  
        new LoadableResourceAsset($"{ModifierIconsPath}.DoubleShot.png");
    public static LoadableAsset<Sprite> Saboteur { get; } =  
        new LoadableResourceAsset($"{ModifierIconsPath}.Saboteur.png");
    public static LoadableAsset<Sprite> Telepath { get; } =  
        new LoadableResourceAsset($"{ModifierIconsPath}.Telepath.png");
    public static LoadableAsset<Sprite> Underdog { get; } =  
        new LoadableResourceAsset($"{ModifierIconsPath}.Underdog.png");
    
    public static LoadableAsset<Sprite> Giant { get; } =  
        new LoadableResourceAsset($"{ModifierIconsPath}.Giant.png");
    public static LoadableAsset<Sprite> Lazy { get; } =  
        new LoadableResourceAsset($"{ModifierIconsPath}.Lazy.png");
    public static LoadableAsset<Sprite> Lover { get; } = 
        new LoadableResourceAsset($"{ModifierIconsPath}.Lover.png");
    public static LoadableAsset<Sprite> Mini { get; } =  
        new LoadableResourceAsset($"{ModifierIconsPath}.Mini.png");
    public static LoadableAsset<Sprite> Paranoiac { get; } =  
        new LoadableResourceAsset($"{ModifierIconsPath}.Paranoiac.png");
    public static LoadableAsset<Sprite> Satellite { get; } =  
        new LoadableResourceAsset($"{ModifierIconsPath}.Satellite.png");
    public static LoadableAsset<Sprite> Shy { get; } =  
        new LoadableResourceAsset($"{ModifierIconsPath}.Shy.png");
    public static LoadableAsset<Sprite> SixthSense { get; } =  
        new LoadableResourceAsset($"{ModifierIconsPath}.SixthSense.png");
    public static LoadableAsset<Sprite> Sleuth { get; } =  
        new LoadableResourceAsset($"{ModifierIconsPath}.Sleuth.png");
    public static LoadableAsset<Sprite> Tiebreaker { get; } =  
        new LoadableResourceAsset($"{ModifierIconsPath}.Tiebreaker.png");
    public static LoadableAsset<Sprite> FirstRoundShield { get; } =
        new LoadableResourceAsset($"{ModifierIconsPath}.FirstRoundShield.png");
    
    #endregion

    #region Role Icons
    public static LoadableAsset<Sprite> Aurial { get; } = 
        new LoadableResourceAsset($"{RoleIconsPath}.Aurial.png");

    public static LoadableAsset<Sprite> Inspector { get; } = 
        new LoadableResourceAsset($"{RoleIconsPath}.Inspector.png");

    public static LoadableAsset<Sprite> Bodyguard { get; } = 
        new LoadableResourceAsset($"{RoleIconsPath}.Bodyguard.png");

    public static LoadableAsset<Sprite> Warlock { get; } = 
        new LoadableResourceAsset($"{RoleIconsPath}.Warlock.png");


    public static LoadableAsset<Sprite> Investigator { get; } =
        new LoadableResourceAsset($"{RoleIconsPath}.Investigator.png");

    public static LoadableAsset<Sprite> Lookout { get; } = 
        
        new LoadableResourceAsset($"{RoleIconsPath}.Lookout.png");

    public static LoadableAsset<Sprite> Mystic { get; } = 
        new LoadableResourceAsset($"{RoleIconsPath}.Mystic.png");

    public static LoadableAsset<Sprite> Seer { get; } = 
        new LoadableResourceAsset($"{RoleIconsPath}.Seer.png");

    public static LoadableAsset<Sprite> Administrator { get; } = 
        new LoadableResourceAsset($"{RoleIconsPath}.Administrator.png");

    public static LoadableAsset<Sprite> Trapper { get; } = 
        new LoadableResourceAsset($"{RoleIconsPath}.Trapper.png");

    public static LoadableAsset<Sprite> Deputy { get; } = 
        new LoadableResourceAsset($"{RoleIconsPath}.Deputy.png");

    public static LoadableAsset<Sprite> Hunter { get; } = 
        new LoadableResourceAsset($"{RoleIconsPath}.Hunter.png");
    
    public static LoadableAsset<Sprite> Lawyer { get; } = 
        new LoadableResourceAsset($"{RoleIconsPath}.Lawyer.png");

    public static LoadableAsset<Sprite> Jailor { get; } = 
        new LoadableResourceAsset($"{RoleIconsPath}.Jailor.png");

    public static LoadableAsset<Sprite> Veteran { get; } = 
        new LoadableResourceAsset($"{RoleIconsPath}.Veteran.png");

    public static LoadableAsset<Sprite> Sheriff { get; } = 
        new LoadableResourceAsset($"{RoleIconsPath}.Sheriff.png");
    
    public static LoadableAsset<Sprite> Medic { get; } = 
        new LoadableResourceAsset($"{RoleIconsPath}.Medic.png");

    public static LoadableAsset<Sprite> Oracle { get; } = 
        new LoadableResourceAsset($"{RoleIconsPath}.Oracle.png");

    public static LoadableAsset<Sprite> Crusader { get; } = 
        new LoadableResourceAsset($"{RoleIconsPath}.Crusader.png");

    public static LoadableAsset<Sprite> Engineer { get; } = 
        new LoadableResourceAsset($"{RoleIconsPath}.Engineer.png");

    public static LoadableAsset<Sprite> Imitator { get; } = 
        new LoadableResourceAsset($"{RoleIconsPath}.Imitator.png");

    public static LoadableAsset<Sprite> Medium { get; } = 
        new LoadableResourceAsset($"{RoleIconsPath}.Medium.png");

    public static LoadableAsset<Sprite> Plumber { get; } = 
        new LoadableResourceAsset($"{RoleIconsPath}.Plumber.png");

    public static LoadableAsset<Sprite> Romantic { get; } = 
        new LoadableResourceAsset($"{RoleIconsPath}.Romantic.png");

    public static LoadableAsset<Sprite> Prosecutor { get; } = 
        new LoadableResourceAsset($"{RoleIconsPath}.Prosecutor.png");

    public static LoadableAsset<Sprite> Transporter { get; } = 
        new LoadableResourceAsset($"{RoleIconsPath}.Transporter.png");
        
    public static LoadableAsset<Sprite> Informant { get; } = 
        new LoadableResourceAsset($"{RoleIconsPath}.Informant.png");

    public static LoadableAsset<Sprite> Amnesiac { get; } = 
        new LoadableResourceAsset($"{RoleIconsPath}.Amnesiac.png");
    
    public static LoadableAsset<Sprite> Executioner { get; } = 
        new LoadableResourceAsset($"{RoleIconsPath}.Executioner.png");

    public static LoadableAsset<Sprite> Jester { get; } = 
        new LoadableResourceAsset($"{RoleIconsPath}.Jester.png");

    public static LoadableAsset<Sprite> Pyromaniac { get; } = 
        new LoadableResourceAsset($"{RoleIconsPath}.Pyromaniac.png");

    public static LoadableAsset<Sprite> Glitch { get; } = 
        new LoadableResourceAsset($"{RoleIconsPath}.Glitch.png");

    public static LoadableAsset<Sprite> Juggernaut { get; } = 
        new LoadableResourceAsset($"{RoleIconsPath}.Juggernaut.png");

    public static LoadableAsset<Sprite> Plaguebearer { get; } =
        new LoadableResourceAsset($"{RoleIconsPath}.Plaguebearer.png");

    public static LoadableAsset<Sprite> Pestilence { get; } = 
        new LoadableResourceAsset($"{RoleIconsPath}.Pestilence.png");

    public static LoadableAsset<Sprite> Consigliere { get; } = 
        new LoadableResourceAsset($"{RoleIconsPath}.Consigliere.png");

    public static LoadableAsset<Sprite> Vampire { get; } = 
        new LoadableResourceAsset($"{RoleIconsPath}.Vampire.png");

    public static LoadableAsset<Sprite> Predator { get; } = 
        new LoadableResourceAsset($"{RoleIconsPath}.Predator.png");

    public static LoadableAsset<Sprite> Werewolf { get; } = 
        new LoadableResourceAsset($"{RoleIconsPath}.Werewolf.png");

    public static LoadableAsset<Sprite> Thief { get; } = 
        new LoadableResourceAsset($"{RoleIconsPath}.Thief.png");

    public static LoadableAsset<Sprite> Hitman { get; } = 
        new LoadableResourceAsset($"{RoleIconsPath}.Hitman.png");

    public static LoadableAsset<Sprite> Eclipsal { get; } = 
        new LoadableResourceAsset($"{RoleIconsPath}.Eclipsal.png");

    public static LoadableAsset<Sprite> Escapist { get; } = 
        new LoadableResourceAsset($"{RoleIconsPath}.Escapist.png");
        
    public static LoadableAsset<Sprite> Grenadier { get; } =
        new LoadableResourceAsset($"{RoleIconsPath}.Grenadier.png");

    public static LoadableAsset<Sprite> Morphling { get; } = 
        new LoadableResourceAsset($"{RoleIconsPath}.Morphling.png");

    public static LoadableAsset<Sprite> Swooper { get; } = 
        new LoadableResourceAsset($"{RoleIconsPath}.Swooper.png");
        
    public static LoadableAsset<Sprite> Venerer { get; } = 
        new LoadableResourceAsset($"{RoleIconsPath}.Venerer.png");

    public static LoadableAsset<Sprite> Bomber { get; } = 
        new LoadableResourceAsset($"{RoleIconsPath}.Bomber.png");

    public static LoadableAsset<Sprite> BountyHunter { get; } = 
        new LoadableResourceAsset($"{RoleIconsPath}.BountyHunter.png");
        
    public static LoadableAsset<Sprite> Hexblade { get; } = 
        new LoadableResourceAsset($"{RoleIconsPath}.Hexblade.png");

    public static LoadableAsset<Sprite> Blackmailer { get; } = 
        new LoadableResourceAsset($"{RoleIconsPath}.Blackmailer.png");

    public static LoadableAsset<Sprite> Hypnotist { get; } = 
        new LoadableResourceAsset($"{RoleIconsPath}.Hypnotist.png");

    public static LoadableAsset<Sprite> Janitor { get; } = 
        new LoadableResourceAsset($"{RoleIconsPath}.Janitor.png");

    public static LoadableAsset<Sprite> Miner { get; } = 
        new LoadableResourceAsset($"{RoleIconsPath}.Miner.png");

    public static LoadableAsset<Sprite> Undertaker { get; } = 
        new LoadableResourceAsset($"{RoleIconsPath}.Undertaker.png");

    public static LoadableAsset<Sprite> RandomAny { get; } = 
        new LoadableResourceAsset($"{RoleIconsPath}.RandomAny.png");

    public static LoadableAsset<Sprite> RandomCrew { get; } = 
        new LoadableResourceAsset($"{RoleIconsPath}.RandomCrew.png");

    public static LoadableAsset<Sprite> RandomNeut { get; } = 
        new LoadableResourceAsset($"{RoleIconsPath}.RandomNeut.png");

    public static LoadableAsset<Sprite> RandomImp { get; } = 
        new LoadableResourceAsset($"{RoleIconsPath}.RandomImp.png");

    #endregion

    #region Chat Assets
    private const string ChatPath = "TownOfSushi.Resources.Chat";

    public static LoadableAsset<Sprite> ImpBubble { get; } =  
        
        new LoadableResourceAsset($"{ChatPath}.ChatImpBubble.png");
    public static LoadableAsset<Sprite> JailBubble { get; } =  
        new LoadableResourceAsset($"{ChatPath}.ChatJailBubble.png");

    public static LoadableAsset<Sprite> VampBubble { get; } =  
        new LoadableResourceAsset($"{ChatPath}.ChatVampBubble.png");

    public static LoadableAsset<Sprite> TeamChatIdle { get; } =  
        new LoadableResourceAsset($"{ChatPath}.TeamChatIdle.png", 101f);

    public static LoadableAsset<Sprite> TeamChatHover { get; } =  
        new LoadableResourceAsset($"{ChatPath}.TeamChatHover.png", 101f);

    public static LoadableAsset<Sprite> TeamChatOpen { get; } =  
        
        new LoadableResourceAsset($"{ChatPath}.TeamChatOpen.png", 101f);

    public static LoadableAsset<Sprite> LoveBubble { get; } =  
        new LoadableResourceAsset($"{ChatPath}.ChatLoveBubble.png");

    public static LoadableAsset<Sprite> LoveChatIdle { get; } =  
        new LoadableResourceAsset($"{ChatPath}.LoveChatIdle.png", 101f);

    public static LoadableAsset<Sprite> LoveChatHover { get; } =  
        new LoadableResourceAsset($"{ChatPath}.LoveChatHover.png", 101f);

    public static LoadableAsset<Sprite> LoveChatOpen { get; } =  
        new LoadableResourceAsset($"{ChatPath}.LoveChatOpen.png", 101f);
    
    public static LoadableAsset<Sprite> NormalBubble { get; } = 
        new LoadableResourceAsset($"{ChatPath}.ChatBubble.png");

    public static LoadableAsset<Sprite> NormalChatIdle { get; } =  
        new LoadableResourceAsset($"{ChatPath}.NormalChatIdle.png", 101f);

    public static LoadableAsset<Sprite> NormalChatHover { get; } =  
        new LoadableResourceAsset($"{ChatPath}.NormalChatHover.png", 101f);

    public static LoadableAsset<Sprite> NormalChatOpen { get; } =  
        new LoadableResourceAsset($"{ChatPath}.NormalChatOpen.png", 101f);

    public static LoadableAsset<Sprite> TeamChatSwitch { get; } =
        new LoadableResourceAsset($"{ShortPath}.TeamChatSwitch.png", 105f);
    #endregion

    public static void Initialize()
    {
        AnimationMaterials.Initialize();
    }
}