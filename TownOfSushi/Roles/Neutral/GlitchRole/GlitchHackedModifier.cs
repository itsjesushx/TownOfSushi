using HarmonyLib;
using MiraAPI.Events;
using MiraAPI.Hud;
using MiraAPI.Modifiers.Types;
using TownOfSushi.Buttons;
using TownOfSushi.Events.TOSEvents;
using UnityEngine;

namespace TownOfSushi.Roles.Neutral;

public sealed class GlitchHackedModifier(byte glitchId) : TimedModifier
{
    public override string ModifierName => "Hacked";
    public override float Duration => OptionGroupSingleton<GlitchOptions>.Instance.HackDuration;
    public override bool AutoStart => false;
    public override bool HideOnUi => ShouldHideHacked;
    public byte GlitchId { get; } = glitchId;

    public bool ShouldHideHacked { get; set; } = true;
    private GameObject? ReportButtonHackedSprite { get; set; }
    private GameObject? KillButtonHackedSprite { get; set; }
    private GameObject? VentButtonHackedSprite { get; set; }
    private GameObject? UseButtonHackedSprite { get; set; }
    private GameObject? SabotageButtonHackedSprite { get; set; }
    private List<GameObject> CustomButtonHackedSprites { get; } = [];

    public override void OnActivate()
    {
        var glitch = PlayerControl.AllPlayerControls.ToArray().FirstOrDefault(x => x.PlayerId == GlitchId);
        var TOSAbilityEvent = new TOSAbilityEvent(AbilityType.GlitchInitialHack, glitch!, Player);
        MiraEventManager.InvokeEvent(TOSAbilityEvent);
        if (Player.AmOwner)
        {
            ReportButtonHackedSprite = HudManager.Instance.ReportButton.CreateHackedIcon();
            KillButtonHackedSprite = HudManager.Instance.KillButton.CreateHackedIcon();
            VentButtonHackedSprite = HudManager.Instance.ImpostorVentButton.CreateHackedIcon();
            UseButtonHackedSprite = HudManager.Instance.UseButton.CreateHackedIcon();
            SabotageButtonHackedSprite = HudManager.Instance.SabotageButton.CreateHackedIcon();

            foreach (var button in CustomButtonManager.Buttons)
            {
                if (button is FakeVentButton)
                {
                    continue;
                }

                CustomButtonHackedSprites.Add(button!.Button!.CreateHackedIcon());
            }
        }
    }

    public override void OnDeath(DeathReason reason)
    {
        ModifierComponent!.RemoveModifier(this);
    }

    public void ShowHacked()
    {
        if (!ShouldHideHacked)
        {
            return;
        }

        ShouldHideHacked = false;
        var glitch = PlayerControl.AllPlayerControls.ToArray().FirstOrDefault(x => x.PlayerId == GlitchId);
        var TOSAbilityEvent = new TOSAbilityEvent(AbilityType.GlitchHackTrigger, glitch!, Player);
        MiraEventManager.InvokeEvent(TOSAbilityEvent);

        if (Player.AmOwner)
        {
            TOSAudio.PlaySound(TOSAudio.HackedSound);

            ReportButtonHackedSprite?.SetHackActive(true);
            KillButtonHackedSprite?.SetHackActive(true);
            VentButtonHackedSprite?.SetHackActive(true);
            UseButtonHackedSprite?.SetHackActive(true);
            SabotageButtonHackedSprite?.SetHackActive(true);

            CustomButtonHackedSprites.Do(x => x.SetHackActive(true));

            StartTimer();
        }
    }

    public override void OnDeactivate()
    {
        if (Player.AmOwner)
        {
            TOSAudio.PlaySound(TOSAudio.UnhackedSound);

            ReportButtonHackedSprite?.SetHackActive(false);
            KillButtonHackedSprite?.SetHackActive(false);
            VentButtonHackedSprite?.SetHackActive(false);
            UseButtonHackedSprite?.SetHackActive(false);
            SabotageButtonHackedSprite?.SetHackActive(false);

            CustomButtonHackedSprites.Do(x => x.SetHackActive(false));
        }
    }
}