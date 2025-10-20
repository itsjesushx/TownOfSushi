using UnityEngine;

namespace TownOfSushi.Assets;

public static class TOSAudio
{
    private const string AudioPath = "TownOfSushi.Resources.Audio";

    // THIS FILE SHOULD ONLY HOLD AUDIO
    public static LoadableAsset<AudioClip> NoisemakerDeathSound { get; } =
        new LoadableAudioResourceAsset($"{AudioPath}.NoisemakerDeath.wav");

    public static LoadableAsset<AudioClip> TrackerActivateSound { get; } =
        new LoadableAudioResourceAsset($"{AudioPath}.TrackerActivate.wav");

    public static LoadableAsset<AudioClip> TrackerDeactivateSound { get; } =
        new LoadableAudioResourceAsset($"{AudioPath}.TrackerDeactivate.wav");

    public static LoadableAsset<AudioClip> TrapperPlaceSound { get; } =
        new LoadableAudioResourceAsset($"{AudioPath}.TrapperPlace.wav");

    public static LoadableAsset<AudioClip> QuestionSound { get; } =
        new LoadableAudioResourceAsset($"{AudioPath}.Question.wav");

    public static LoadableAsset<AudioClip> ToppatIntroSound { get; } =
        new LoadableAudioResourceAsset($"{AudioPath}.ToppatIntro.wav");

    public static LoadableAsset<AudioClip> ProsIntroSound { get; } =
        new LoadableAudioResourceAsset($"{AudioPath}.ProsIntro.wav");

    public static LoadableAsset<AudioClip> AdministratorIntroSound { get; } =
        new LoadableAudioResourceAsset($"{AudioPath}.AdministratorIntro.wav");

    public static LoadableAsset<AudioClip> OtherIntroSound { get; } =
        new LoadableAudioResourceAsset($"{AudioPath}.OtherIntro.wav");

    public static LoadableAsset<AudioClip> VigiIntroSound { get; } =
        new LoadableAudioResourceAsset($"{AudioPath}.VigilanteIntro.wav");

    public static LoadableAsset<AudioClip> RetributionistReviveSound { get; } =
        new LoadableAudioResourceAsset($"{AudioPath}.Retributionist.wav");

    public static LoadableAsset<AudioClip> JailSound { get; } = new LoadableAudioResourceAsset($"{AudioPath}.Jail.wav");

    public static LoadableAsset<AudioClip> MayorRevealSound { get; } =
        new LoadableAudioResourceAsset($"{AudioPath}.MayorReveal.wav");

    public static LoadableAsset<AudioClip> MediumIntroSound { get; } =
        new LoadableAudioResourceAsset($"{AudioPath}.MediumIntro.wav");

    public static LoadableAsset<AudioClip> TimeLordIntroSound { get; } =
        new LoadableAudioResourceAsset($"{AudioPath}.TimeLordIntro.wav");

    public static LoadableAsset<AudioClip> EngiFix1 { get; } = new LoadableAudioResourceAsset($"{AudioPath}.Fix1.wav");
    public static LoadableAsset<AudioClip> EngiFix2 { get; } = new LoadableAudioResourceAsset($"{AudioPath}.Fix2.wav");
    public static LoadableAsset<AudioClip> EngiFix3 { get; } = new LoadableAudioResourceAsset($"{AudioPath}.Fix3.wav");

    public static LoadableAsset<AudioClip> GuardianAngelSound { get; } =
        new LoadableAudioResourceAsset($"{AudioPath}.GuardianAngelProtect.wav");

    public static LoadableAsset<AudioClip> DiscoveredSound { get; } =
        new LoadableAudioResourceAsset($"{AudioPath}.Discovered.wav");

    public static LoadableAsset<AudioClip> GlitchSound { get; } =
        new LoadableAudioResourceAsset($"{AudioPath}.Glitch.wav");

    public static LoadableAsset<AudioClip> HackedSound { get; } =
        new LoadableAudioResourceAsset($"{AudioPath}.Hacked.wav");

    public static LoadableAsset<AudioClip> UnhackedSound { get; } =
        new LoadableAudioResourceAsset($"{AudioPath}.Unhacked.wav");

    public static LoadableAsset<AudioClip> MimicSound { get; } =
        new LoadableAudioResourceAsset($"{AudioPath}.Mimic.wav");

    public static LoadableAsset<AudioClip> UnmimicSound { get; } =
        new LoadableAudioResourceAsset($"{AudioPath}.Unmimic.wav");

    public static LoadableAsset<AudioClip> ArsoIgniteSound { get; } =
        new LoadableAudioResourceAsset($"{AudioPath}.PyromaniacIgnite.wav");

    public static LoadableAsset<AudioClip> PredatorTerminateSound { get; } =
        new LoadableAudioResourceAsset($"{AudioPath}.WerewolfRampage.wav");

    public static LoadableAsset<AudioClip> GrenadeSound { get; } =
        new LoadableAudioResourceAsset($"{AudioPath}.GrenadeThrow.wav");

    public static LoadableAsset<AudioClip> HexbladeIntroSound { get; } =
        new LoadableAudioResourceAsset($"{AudioPath}.HexbladeIntro.wav");

    public static LoadableAsset<AudioClip> SwooperActivateSound { get; } =
        new LoadableAudioResourceAsset($"{AudioPath}.SwooperActivate.wav");

    public static LoadableAsset<AudioClip> SwooperDeactivateSound { get; } =
        new LoadableAudioResourceAsset($"{AudioPath}.SwooperDeactivate.wav");

    public static LoadableAsset<AudioClip> MineSound { get; } = new LoadableAudioResourceAsset($"{AudioPath}.Mine.wav");

    public static LoadableAsset<AudioClip> EscapistMarkSound { get; } =
        new LoadableAudioResourceAsset($"{AudioPath}.EscapistMark.wav");

    public static LoadableAsset<AudioClip> EscapistRecallSound { get; } =
        new LoadableAudioResourceAsset($"{AudioPath}.EscapistRecall.wav");

    public static LoadableAsset<AudioClip> JanitorCleanSound { get; } =
        new LoadableAudioResourceAsset($"{AudioPath}.JanitorClean.wav");
    

    public static LoadableAsset<AudioClip> WitchIntro { get; } =
        new LoadableAudioResourceAsset($"{AudioPath}.WitchIntro.wav");
    
    public static LoadableAsset<AudioClip> WarlockCurse { get; } =
        new LoadableAudioResourceAsset($"{AudioPath}.WarlockCurse.wav");

    public static void PlaySound(LoadableAsset<AudioClip> clip, float vol = 1f)
    {
        if (Constants.ShouldPlaySfx())
        {
            SoundManager.Instance.PlaySound(clip.LoadAsset(), false, vol);
        }
    }
}