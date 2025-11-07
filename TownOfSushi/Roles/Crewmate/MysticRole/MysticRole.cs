using System.Text;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Patches.Stubs;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using UnityEngine;

namespace TownOfSushi.Roles.Crewmate;

public sealed class MysticRole(IntPtr cppPtr) : CrewmateRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable, IMysticClue
{
    public string RoleName => "Mystic";
    public string RoleDescription => "Know When and Where Kills Happen";
    public string RoleLongDescription => "Understand when and where kills happen";
    public Color RoleColor => TownOfSushiColors.Mystic;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.CrewmateInvestigative;
    public MysticClueType MysticHintType => MysticClueType.Perception;

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = TOSRoleIcons.Mystic,
        IntroSound = TOSAudio.MediumIntroSound
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ITownOfSushiRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return $"The {RoleName} is a Crewmate Investigative role that gets an alert when someone dies."
               + MiscUtils.AppendOptionsText(GetType());
    }
    public override void OnMeetingStart()
    {
        RoleBehaviourStubs.OnMeetingStart(this);

        GenerateReport();
    }

    private void GenerateReport()
    {
        Logger<TownOfSushiPlugin>.Info($"Generating Mystic report");

        var reportBuilder = new StringBuilder();

        if (Player == null)
        {
            return;
        }

        if (!Player.AmOwner)
        {
            return;
        }

        foreach (var player in GameData.Instance.AllPlayers.ToArray()
                     .Where(x => !x.Object.HasDied() && x.Object.HasModifier<MysticObservedModifier>()))
        {
            var role = player.Object.Data.Role;
            var doomableRole = role as IMysticClue;
            var undoomableRole = role as IUnguessable;
            var hintType = MysticClueType.Default;
            var cachedMod =
                player.Object.GetModifiers<BaseModifier>().FirstOrDefault(x => x is ICachedRole) as ICachedRole;
            if (cachedMod != null)
            {
                role = cachedMod.CachedRole;
                doomableRole = role as IMysticClue;
            }

            if (undoomableRole != null)
            {
                role = undoomableRole.AppearAs;
                doomableRole = role as IMysticClue;
            }

            if (doomableRole != null)
            {
                hintType = doomableRole.MysticHintType;
            }

            switch (hintType)
            {
                case MysticClueType.Perception:
                    reportBuilder.AppendLine(TownOfSushiPlugin.Culture,
                        $"You observe that {player.PlayerName} has an altered perception of reality\n");
                    break;
                case MysticClueType.Insight:
                    reportBuilder.AppendLine(TownOfSushiPlugin.Culture,
                        $"You observe that {player.PlayerName} has an insight for private information\n");
                    break;
                case MysticClueType.Death:
                    reportBuilder.AppendLine(TownOfSushiPlugin.Culture,
                        $"You observe that {player.PlayerName} has an unusual obsession with dead players\n");
                    break;
                case MysticClueType.Hunter:
                    reportBuilder.AppendLine(TownOfSushiPlugin.Culture,
                        $"You observe that {player.PlayerName} is well trained in hunting down prey\n");
                    break;
                case MysticClueType.Fearmonger:
                    reportBuilder.AppendLine(TownOfSushiPlugin.Culture,
                        $"You observe that {player.PlayerName} spreads fear amonst the group\n");
                    break;
                case MysticClueType.Protective:
                    reportBuilder.AppendLine(TownOfSushiPlugin.Culture,
                        $"You observe that {player.PlayerName} hides to guard themself or others\n");
                    break;
                case MysticClueType.Trickster:
                    reportBuilder.AppendLine(TownOfSushiPlugin.Culture,
                        $"You observe that {player.PlayerName} has a trick up their sleeve\n");
                    break;
                case MysticClueType.Relentless:
                    reportBuilder.AppendLine(TownOfSushiPlugin.Culture,
                        $"You observe that {player.PlayerName} is capable of performing relentless attacks\n");
                    break;
                case MysticClueType.Default:
                    // Get it? Because they're not from this "Town" of Us? heh...
                    // :skull: who wrote that lol
                    reportBuilder.AppendLine(TownOfSushiPlugin.Culture,
                        $"You observe that {player.PlayerName} is not from this town\n");
                    break;
            }

            var roles = RoleManager.Instance.AllRoles.ToArray()
                .Where(x => (x is IMysticClue doomRole && doomRole.MysticHintType == MysticClueType.Default &&
                    x is not IUnguessable || x is not IMysticClue) && !x.IsDead).ToList();
            roles = roles.OrderBy(x => x.GetRoleName()).ToList();
            var lastRole = roles[roles.Count - 1];

            if (hintType != MysticClueType.Default)
            {
                roles = MiscUtils.AllRoles
                    .Where(x => x is IMysticClue doomRole && doomRole.MysticHintType == hintType && x is not IUnguessable)
                    .OrderBy(x => x.GetRoleName()).ToList();
                lastRole = roles[roles.Count - 1];
            }

            if (roles.Count != 0)
            {
                reportBuilder.Append(TownOfSushiPlugin.Culture, $"(");
                foreach (var role2 in roles)
                {
                    if (role2 == lastRole)
                    {
                        reportBuilder.Append(TownOfSushiPlugin.Culture,
                            $"{lastRole.GetRoleName()}, ");
                    }
                    else
                    {
                        reportBuilder.Append(TownOfSushiPlugin.Culture,
                            $"{role2.GetRoleName()}, ");
                    }
                }
            }

            player.Object.RemoveModifier<MysticObservedModifier>();
        }

        var report = reportBuilder.ToString();

        if (HudManager.Instance && report.Length > 0)
        {
            var title = $"<color=#{TownOfSushiColors.Mystic.ToHtmlStringRGBA()}>Mystic Report</color>";
            MiscUtils.AddFakeChat(Player.Data, title, report, false, true);
        }
    }
}