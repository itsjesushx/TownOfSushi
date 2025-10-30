using Il2CppInterop.Runtime.Attributes;
using Reactor.Networking.Attributes;
using TownOfSushi.Modules;
using UnityEngine;

namespace TownOfSushi.Modifiers.Game.Crewmate;
public sealed class SwapperModifier : TOSGameModifier, IWikiDiscoverable
{
    private MeetingMenu meetingMenu;
    [HideFromIl2Cpp] public PlayerVoteArea? Swap1 { get; set; }
    [HideFromIl2Cpp] public PlayerVoteArea? Swap2 { get; set; }
    public override string ModifierName => "Swapper";
    public override string IntroInfo => "You will also be able to swap votes between 2 players";
    public override LoadableAsset<Sprite>? ModifierIcon => TOSModifierIcons.Swapper;
    public override Color FreeplayFileColor => new Color32(140, 255, 255, 255);

    public override ModifierFaction FactionType => ModifierFaction.CrewmateUtility;
    public string GetAdvancedDescription()
    {
        return
            "Swap Votes between 2 players mid meeting!"
            + MiscUtils.AppendOptionsText(GetType());
    }

    public List<CustomButtonWikiDescription> Abilities { get; } = [];

    public override string GetDescription()
    {
        return
            $"Swap Votes between 2 players mid meeting!";
    }

    public override int GetAssignmentChance()
    {
        return (int)OptionGroupSingleton<SwapperOptions>.Instance.SwapperChance;
    }

    public override int GetAmountPerGame()
    {
        return 1;
    }
    public override void OnActivate()
    {
        base.OnActivate();

        if (Player.AmOwner)
        {
            meetingMenu = new MeetingMenu(Player.Data.Role, SetActive, MeetingAbilityType.Toggle, TOSAssets.SwapActive,
                TOSAssets.SwapInactive, IsExempt)
            {
                Position = new Vector3(-0.40f, 0f, -3f)
            };
        }

        if (!OptionGroupSingleton<SwapperOptions>.Instance.CanButton)
        {
            Player.RemainingEmergencies = 0;
        }
    }

    public override bool IsModifierValidOn(RoleBehaviour role)
    {
        return base.IsModifierValidOn(role) && role.IsCrewmate() && role is not VigilanteRole;
    }
    public override void OnMeetingStart()
    {
        if (Player.AmOwner)
        {
            meetingMenu.GenButtons(MeetingHud.Instance,
                Player.AmOwner && !Player.HasDied() && !Player.HasModifier<JailedModifier>());
        }
    }

    public void OnVotingComplete()
    {
        if (Player.AmOwner)
        {
            meetingMenu.HideButtons();
        }
    }

    public override void OnDeactivate()
    {
        if (Player.AmOwner)
        {
            meetingMenu?.Dispose();
            meetingMenu = null!;
        }
    }

    private static bool IsExempt(PlayerVoteArea voteArea)
    {
        var player = GameData.Instance.GetPlayerById(voteArea.TargetPlayerId)?.Object;

        return !player || !player?.Data || player!.Data.Disconnected || player.Data.IsDead ||
               player.HasModifier<JailedModifier>();
    }

    private void SetActive(PlayerVoteArea voteArea, MeetingHud __instance)
    {
        if (__instance.state == MeetingHud.VoteStates.Discussion || IsExempt(voteArea))
        {
            return;
        }

        if (!Swap1)
        {
            Swap1 = voteArea;
            meetingMenu.Actives[voteArea.TargetPlayerId] = true;
        }
        else if (!Swap2)
        {
            Swap2 = voteArea;
            meetingMenu.Actives[voteArea.TargetPlayerId] = true;
        }
        else if (Swap1 == voteArea)
        {
            meetingMenu.Actives[Swap1!.TargetPlayerId] = false;
            Swap1 = null;
        }
        else if (Swap2 == voteArea)
        {
            meetingMenu.Actives[Swap2!.TargetPlayerId] = false;
            Swap2 = null;
        }
        else
        {
            meetingMenu.Actives[Swap1!.TargetPlayerId] = false;
            Swap1 = Swap2;
            Swap2 = voteArea;
            meetingMenu.Actives[voteArea.TargetPlayerId] = !meetingMenu.Actives[voteArea.TargetPlayerId];
        }

        RpcSyncSwaps(Player, Swap1?.TargetPlayerId ?? 255, Swap2?.TargetPlayerId ?? 255);
    }

    [MethodRpc((uint)TownOfSushiRpc.SetSwaps)]
    public static void RpcSyncSwaps(PlayerControl swapper, byte swap1, byte swap2)
    {
        var swapperRole = swapper.GetModifier<SwapperModifier>();
        var areas = MeetingHud.Instance.playerStates.ToList();
        swapperRole!.Swap1 = areas.Find(x => x.TargetPlayerId == swap1);
        swapperRole.Swap2 = areas.Find(x => x.TargetPlayerId == swap2);
    }
}