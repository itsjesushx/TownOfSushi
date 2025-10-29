using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TownOfSushi.Modifiers.Game.Crewmate;

public sealed class BaitModifier : TOSGameModifier, IWikiDiscoverable
{
    public override string ModifierName => "Bait";
    public override string IntroInfo => "You will also force your killer to report your body.";
    public override LoadableAsset<Sprite>? ModifierIcon => TOSModifierIcons.Bait;
    public override Color FreeplayFileColor => new Color32(140, 255, 255, 255);

    public override ModifierFaction FactionType => ModifierFaction.CrewmatePostmortem;

    private static float MinDelay => OptionGroupSingleton<BaitOptions>.Instance.MinDelay;
    private static float MaxDelay => OptionGroupSingleton<BaitOptions>.Instance.MaxDelay;

    public string GetAdvancedDescription()
    {
        return
            "After you die, your killer will self-report, reporting your body."
            + MiscUtils.AppendOptionsText(GetType());
    }

    public List<CustomButtonWikiDescription> Abilities { get; } = [];

    public override string GetDescription()
    {
        return "Force your killer to self-report.";
    }

    public override int GetAssignmentChance()
    {
        return (int)OptionGroupSingleton<BaitOptions>.Instance.BaitChance;
    }

    public override int GetAmountPerGame()
    {
        return (int)OptionGroupSingleton<BaitOptions>.Instance.BaitAmount;
    }

    public override bool IsModifierValidOn(RoleBehaviour role)
    {
        return base.IsModifierValidOn(role) && role.IsCrewmate();
    }

    public static IEnumerator CoReportDelay(PlayerControl killer, PlayerControl target)
    {
        if (!killer || !target || killer == target)
        {
            yield break;
        }

        yield return new WaitForSeconds(Random.RandomRange(MinDelay, MaxDelay));

        if (killer.AmOwner)
        {
            killer.CmdReportDeadBody(target.Data);

            var notif1 = Helpers.CreateAndShowNotification(
                MiscUtils.ColorString(TownOfSushiColors.Bait, $"<b>{target.Data.PlayerName} was a Bait, causing you to self report.</b>"),
                Color.white, spr: TOSModifierIcons.Bait.LoadAsset());

            
            notif1.AdjustNotification();
        }
    }
}