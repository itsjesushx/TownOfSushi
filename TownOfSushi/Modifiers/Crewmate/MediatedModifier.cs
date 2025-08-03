using MiraAPI.Events;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using TownOfSushi.Buttons.Crewmate;
using TownOfSushi.Events.TosEvents;
using TownOfSushi.Options.Roles.Crewmate;
using TownOfSushi.Roles.Crewmate;
using TownOfSushi.Utilities;
using TownOfSushi.Utilities.Appearances;

namespace TownOfSushi.Modifiers.Crewmate;

public sealed class MediatedModifier(byte mediumId) : BaseModifier
{
    public override string ModifierName => "Mediated";
    public override bool HideOnUi => true;
    public byte MediumId { get; } = mediumId;

    private MediumRole? _medium;
    private PlayerControl? _mediumPlayer;
    public override void OnMeetingStart()
    {
        ModifierComponent?.RemoveModifier(this);
    }
    private ArrowBehaviour? _arrow;

    public override void OnActivate()
    {

        _medium = GameData.Instance.GetPlayerById(MediumId).Role as MediumRole;
        _mediumPlayer = _medium?.Player;

        if (_mediumPlayer == null || _medium == null || !Player.Data.IsDead)
        {
            ModifierComponent?.RemoveModifier(this);
            return;
        }

        var TosAbilityEvent = new TosAbilityEvent(AbilityType.MediumMediate, _mediumPlayer, Player);
        MiraEventManager.InvokeEvent(TosAbilityEvent);

        _medium.MediatedPlayers.Add(this);

        switch (OptionGroupSingleton<MediumOptions>.Instance.ArrowVisibility)
        {
            case MediumVisibility.Both:
                var ownerTransform = Player.AmOwner ? _mediumPlayer.transform : Player.transform;
                _arrow = MiscUtils.CreateArrow(ownerTransform, TownOfSushiColors.Medium);
                break;

            case MediumVisibility.ShowMedium when Player.AmOwner:
                _arrow = MiscUtils.CreateArrow(_mediumPlayer.transform, TownOfSushiColors.Medium);
                break;

            case MediumVisibility.ShowMediate when _mediumPlayer.AmOwner:
                _arrow = MiscUtils.CreateArrow(Player.transform, TownOfSushiColors.Medium);
                break;
        }

        if (_mediumPlayer.AmOwner && !OptionGroupSingleton<MediumOptions>.Instance.RevealMediateAppearance)
        {
            Player.SetCamouflage();
        }

        Coroutines.Start(MiscUtils.CoFlash(TownOfSushiColors.Medium, alpha: 0.5f));
    }

    public override void OnDeactivate()
    {
        if (_mediumPlayer == null)
        {
            return;
        }

        if (_medium != null)
        {
            _medium.MediatedPlayers.Remove(this);
        }

        if (_mediumPlayer.AmOwner)
        {
            CustomButtonSingleton<MediumMediateButton>.Instance.SetTimerPaused(false);
            CustomButtonSingleton<MediumMediateButton>.Instance.ResetCooldownAndOrEffect();

            if (!OptionGroupSingleton<MediumOptions>.Instance.RevealMediateAppearance)
            {
                Player.SetCamouflage(false);
            }
        }

        if (_arrow != null)
        {
            _arrow.gameObject.Destroy();
        }
    }

    public override void FixedUpdate()
    {
        if (!Player.Data.IsDead)
        {
            ModifierComponent?.RemoveModifier(this);
            return;
        }

        if (_mediumPlayer != null && _mediumPlayer.AmOwner)
        {
            Player.Visible = true;
        }

        if (_arrow != null && _arrow.target != _arrow.transform.parent.position)
        {
            _arrow.target = _arrow.transform.parent.position;
        }
    }
}
