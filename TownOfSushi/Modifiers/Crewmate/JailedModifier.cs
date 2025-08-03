using MiraAPI.Events;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using Reactor.Utilities.Extensions;
using TownOfSushi.Events.TosEvents;
using TownOfSushi.Utilities;
using UnityEngine;

namespace TownOfSushi.Modifiers.Crewmate;

public sealed class JailedModifier(byte jailorId) : BaseModifier
{
    public override string ModifierName => "Jailed";
    public override bool HideOnUi => true;
    public byte JailorId { get; } = jailorId;
    private GameObject? jailCell;
    public override void OnActivate()
    {
        base.OnActivate();
        var jailor = PlayerControl.AllPlayerControls.ToArray().FirstOrDefault(x => x.PlayerId == JailorId);
        var TosAbilityEvent = new TosAbilityEvent(AbilityType.JailorJail, jailor!, Player);
        MiraEventManager.InvokeEvent(TosAbilityEvent);
    }
    public override void OnMeetingStart()
    {
        Clear();
        if (GameData.Instance.GetPlayerById(JailorId).Object.HasDied() || Player.HasDied() || !MeetingHud.Instance) return;

        if (Player.AmOwner)
        {
            var title = $"<color=#{TownOfSushiColors.Jailor.ToHtmlStringRGBA()}>Jailee Feedback</color>";
            var text = "You are jailed, convince the Jailor that you are Crew to avoid being executed in the private chatbox.";
            if (PlayerControl.LocalPlayer.Is(ModdedRoleTeams.Crewmate)) text = "You are jailed, provide relevant information to the Jailor to prove you are Crew in the private chatbox.";
            MiscUtils.AddFakeChat(PlayerControl.LocalPlayer.Data, title, text, false, true);

            var notif1 = Helpers.CreateAndShowNotification(
                $"<b>{TownOfSushiColors.Jailor.ToTextColor()}{text}</color></b>", Color.white, spr: TosRoleIcons.Jailor.LoadAsset());

            notif1.Text.SetOutlineThickness(0.35f);
            notif1.transform.localPosition = new Vector3(0f, 1f, -20f);
        }

        foreach (var voteArea in MeetingHud.Instance.playerStates)
        {
            if (Player.PlayerId == voteArea.TargetPlayerId)
            {
                GenCell(voteArea);
            }
        }
    }
    public void Clear()
    {
        jailCell?.Destroy();
    }
    private void GenCell(PlayerVoteArea voteArea)
    {
        var confirmButton = voteArea.Buttons.transform.GetChild(0).gameObject;
        var parent = confirmButton.transform.parent.parent;

        var jailCellObj = UnityEngine.Object.Instantiate(confirmButton, voteArea.transform);

        var cellRenderer = jailCellObj.GetComponent<SpriteRenderer>();
        cellRenderer.sprite = TosAssets.InJailSprite.LoadAsset();

        jailCellObj.transform.localPosition = new Vector3(-0.95f, 0f, -2f);
        jailCellObj.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
        jailCellObj.layer = 5;
        jailCellObj.transform.parent = parent;
        jailCellObj.transform.GetChild(0).gameObject.Destroy();

        var passive = jailCellObj.GetComponent<PassiveButton>();
        passive.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();

        jailCell = jailCellObj;
    }
    public override void OnDeath(DeathReason reason)
    {
        ModifierComponent!.RemoveModifier(this);
    }
}
