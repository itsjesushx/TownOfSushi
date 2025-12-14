using System.Text;
using MiraAPI.Events;
using Reactor.Utilities.Extensions;
using TownOfSushi.Events.TOSEvents;

namespace TownOfSushi.Roles.Crewmate;

public sealed class ClericCleanseModifier(PlayerControl cleric) : BaseModifier
{
    public enum EffectType : byte
    {
        ArsonistDouse,
        Hack,
        WizardSpelled,
        PyromaniacDouse,
        WarlockCursed,
        Infect,
        Blackmail,
        Blind,
        Flash,
        Hypnosis
    }

    public override string ModifierName => "Cleric Cleanse";
    public override bool HideOnUi => true;
    public PlayerControl Cleric { get; } = cleric;

    public List<EffectType> Effects { get; set; } = [];
    public bool Cleansed { get; set; }

    public override void OnActivate()
    {
        base.OnActivate();
        var TOSAbilityEvent = new TOSAbilityEvent(AbilityType.ClericCleanse, Cleric, Player);
        MiraEventManager.InvokeEvent(TOSAbilityEvent);

        Effects = FindNegativeEffects(Player);
    }

    public override void OnDeath(DeathReason reason)
    {
        ModifierComponent?.RemoveModifier(this);
    }

    public override void OnMeetingStart()
    {
        if (Cleric.AmOwner)
        {
            var text = new StringBuilder($"Cleansed effects on {Player.Data.PlayerName}:");

            foreach (var effect in Effects)
            {
                text.Append(TownOfSushiPlugin.Culture, $" {effect.ToString()},");
            }

            text = text.Remove(text.Length - 1, 1);

            if (Effects.Count == 0)
            {
                text = new StringBuilder($"No negative effects were found on {Player.Data.PlayerName}.");
            }

            var title = $"<color=#{TownOfSushiColors.Cleric.ToHtmlStringRGBA()}>Cleric Feedback</color>";
            MiscUtils.AddFakeChat(PlayerControl.LocalPlayer.Data, title, text.ToString(), false, true);
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if (!Cleansed)
        {
            CleansePlayer();
            Cleansed = true;
        }
    }

    private void CleansePlayer()
    {
        if (Effects.Contains(EffectType.ArsonistDouse))
        {
            Player.RemoveModifier<ArsonistDousedModifier>();
        }

        if (Effects.Contains(EffectType.PyromaniacDouse))
        {
            Player.RemoveModifier<PyromaniacDousedModifier>();
        }

        if (Effects.Contains(EffectType.WarlockCursed))
        {
            Player.RemoveModifier<WarlockCursedModifier>();
        }

        if (Effects.Contains(EffectType.Hack))
        {
            Player.RemoveModifier<GlitchHackedModifier>();
        }

        if (Effects.Contains(EffectType.Infect))
        {
            Player.RemoveModifier<PlaguebearerInfectedModifier>();
        }

        if (Effects.Contains(EffectType.Blackmail))
        {
            Player.RemoveModifier<BlackmailedModifier>();
        }

        if (Effects.Contains(EffectType.Blind))
        {
            Player.RpcRemoveModifier<EclipsalBlindModifier>();
        }

        if (Effects.Contains(EffectType.WizardSpelled))
        {
            Player.RpcRemoveModifier<WizardSpelledModifier>();
        }

        if (Effects.Contains(EffectType.Flash))
        {
            Player.RemoveModifier<GrenadierFlashModifier>();
        }

        if (Effects.Contains(EffectType.Hypnosis))
        {
            Player.RemoveModifier<HypnotisedModifier>();
        }
    }

    public static List<EffectType> FindNegativeEffects(PlayerControl player)
    {
        var effects = new List<EffectType>();

        if (player.HasModifier<PyromaniacDousedModifier>())
        {
            effects.Add(EffectType.PyromaniacDouse);
        }

        if (player.HasModifier<ArsonistDousedModifier>())
        {
            effects.Add(EffectType.ArsonistDouse);
        }

        if (player.HasModifier<WarlockCursedModifier>())
        {
            effects.Add(EffectType.WarlockCursed);
        }

        if (player.HasModifier<GlitchHackedModifier>())
        {
            effects.Add(EffectType.Hack);
        }

        if (player.HasModifier<PlaguebearerInfectedModifier>())
        {
            effects.Add(EffectType.Infect);
        }

        if (player.HasModifier<BlackmailedModifier>())
        {
            effects.Add(EffectType.Blackmail);
        }

        if (player.HasModifier<EclipsalBlindModifier>())
        {
            effects.Add(EffectType.Blind);
        }

        if (player.HasModifier<WizardSpelledModifier>())
        {
            effects.Add(EffectType.WizardSpelled);
        }

        if (player.HasModifier<GrenadierFlashModifier>())
        {
            effects.Add(EffectType.Flash);
        }

        if (player.HasModifier<HypnotisedModifier>())
        {
            effects.Add(EffectType.Hypnosis);
        }

        return effects;
    }
}