using System.Text.RegularExpressions;
using Il2CppSystem.Text;
using UnityEngine;

namespace TownOfSushi.Modifiers.Game.Crewmate;

public sealed class TaskmasterModifier : TOSGameModifier, IWikiDiscoverable
{
    public override string ModifierName => "Taskmaster";
    public override string IntroInfo => "You finish a random task each round";
    public override LoadableAsset<Sprite>? ModifierIcon => TOSModifierIcons.Taskmaster;
    public override Color FreeplayFileColor => new Color32(140, 255, 255, 255);

    public override ModifierFaction FactionType => ModifierFaction.CrewmatePassive;

    public string GetAdvancedDescription()
    {
        return
            "Every time a round starts, you will automatically finish a task.";
    }

    public List<CustomButtonWikiDescription> Abilities { get; } = [];

    public override string GetDescription()
    {
        return "A random task is auto completed for you after each meeting";
    }

    public override int GetAssignmentChance()
    {
        return (int)OptionGroupSingleton<TaskMasterOptions>.Instance.TaskmasterChance;
    }

    public override int GetAmountPerGame()
    {
        return (int)OptionGroupSingleton<TaskMasterOptions>.Instance.TaskmasterAmount;
    }

    public override bool IsModifierValidOn(RoleBehaviour role)
    {
        return base.IsModifierValidOn(role) && role.IsCrewmate() &&
               !(GameOptionsManager.Instance.currentNormalGameOptions.MapId is 4 or 6);
    }

    public void OnRoundStart()
    {
        if (Player.AmOwner && Player.myTasks.Count > 0 && !Player.HasDied())
        {
            var tasks = Player.myTasks.ToArray().Where(x => x.TryCast<NormalPlayerTask>() != null && !x.IsComplete)
                .ToList();

            if (tasks.Count > 0)
            {
                tasks.Shuffle();

                var randomTask = tasks[0];

                HudManager.Instance.ShowTaskComplete();
                Player.RpcCompleteTask(randomTask.Id);

                var sb = new StringBuilder();
                randomTask.AppendTaskText(sb);

                var pattern = @" \(.*?\)";
                var query = sb.ToString();
                var taskText = Regex.Replace(query, pattern, string.Empty);
                taskText = taskText.Replace(Environment.NewLine, "");

                var notif1 = Helpers.CreateAndShowNotification(
                    MiscUtils.ColorString(TownOfSushiColors.Taskmaster, $"<b>The task '{taskText}' has been completed for you.</b>"),
                    Color.white, spr: TOSModifierIcons.Taskmaster.LoadAsset());
                
                notif1.AdjustNotification();
            }
        }
    }
}