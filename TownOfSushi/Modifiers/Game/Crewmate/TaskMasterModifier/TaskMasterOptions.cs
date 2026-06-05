using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using UnityEngine;

namespace TownOfSushi.Modifiers.Game.Alliance;

public sealed class TaskMasterOptions : AbstractOptionGroup<TaskmasterModifier>
{
    public override string GroupName => "TaskMaster";
    public override Color GroupColor => TownOfSushiColors.Taskmaster;
    public override uint GroupPriority => 47;

    [ModdedNumberOption("Taskmaster Amount", 0, 5)]
    public float TaskmasterAmount { get; set; } = 0;

    public ModdedNumberOption TaskmasterChance { get; } =
        new("Taskmaster Chance", 50f, 0, 100f, 10f, MiraNumberSuffixes.Percent)
        {
            Visible = () => OptionGroupSingleton<TaskMasterOptions>.Instance.TaskmasterAmount > 0
        };
}