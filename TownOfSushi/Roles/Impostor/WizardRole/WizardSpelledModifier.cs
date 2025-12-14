using MiraAPI.Events;
using Reactor.Utilities.Extensions;
using TownOfSushi.Events.TOSEvents;
using TownOfSushi.Modules.Anims;
using TownOfSushi.Options;
using UnityEngine;

namespace TownOfSushi.Roles.Impostor;

public sealed class WizardSpelledModifier(byte WizardId) : BaseModifier
{
    public override string ModifierName => "Wizard Spelled";
    public override bool HideOnUi => true;
    private static Sprite[] auraAnimationSprites = new Sprite[12];
    private int animationFrame;
    private float animationTimer;
    
    public override void OnActivate()
    {
        var TOSAbilityEvent = new TOSAbilityEvent(AbilityType.WizardCastSpell, Player);
        MiraEventManager.InvokeEvent(TOSAbilityEvent);
        var body = UnityEngine.Object.FindObjectsOfType<DeadBody>().FirstOrDefault(x =>
            x.ParentId == PlayerControl.LocalPlayer.PlayerId && !TutorialManager.InstanceExists);
        
        ShowAura = PlayerControl.LocalPlayer.IsImpostor() || (PlayerControl.LocalPlayer.HasDied() && OptionGroupSingleton<GeneralOptions>.Instance.TheDeadKnow && !body);
        
        // makes a temporary GameObject with a SpriteRenderer using the aura sprite, then spawn it
        var auraPrefab = new GameObject("WizardAura_Prefab");
        var sr = auraPrefab.AddComponent<SpriteRenderer>();
        sr.sprite = GetAuraSprite(0);
        WizardAura = AnimStore.SpawnAnimBody(Player, auraPrefab, false, -1.1f, -0.35f, 1.5f)!;
        UnityEngine.Object.Destroy(auraPrefab);
    }

    private static Sprite GetAuraSprite(int index)
    {
        index = Mathf.Clamp(index, 0, auraAnimationSprites.Length - 1);
        if (auraAnimationSprites[index] == null)
            auraAnimationSprites[index] = MiscUtils.LoadSpriteFromResources($"TownOfSushi.Resources.Wizard.Aura_{index + 1}.png", 125f);
        return auraAnimationSprites[index];
    }

    public override void Update()
    {
        if (!MeetingHud.Instance && WizardAura?.gameObject != null)
        {
            WizardAura.SetActive(!Player.IsConcealed() && ShowAura);
            
            animationTimer += Time.deltaTime;
            if (animationTimer >= 0.1f)
            {
                animationFrame = (animationFrame + 1) % auraAnimationSprites.Length;
                WizardAura.GetComponent<SpriteRenderer>().sprite = GetAuraSprite(animationFrame);
                animationTimer = 0f;
            }
        }
    }
    
    public override void OnDeactivate()
    {
        if (WizardAura?.gameObject != null)
            WizardAura.gameObject.Destroy();
    }

    public byte WizardId { get; } = WizardId;
    public GameObject? WizardAura { get; set; }
    public bool ShowAura { get; set; }

    public override void OnDeath(DeathReason reason)
    {
        ModifierComponent!.RemoveModifier(this);
    }
}