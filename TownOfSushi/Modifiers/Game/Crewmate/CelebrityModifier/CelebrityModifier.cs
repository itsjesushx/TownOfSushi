using Reactor.Networking.Attributes;
using Reactor.Utilities;
using TownOfSushi.Utilities.Appearances;
using UnityEngine;
using TownOfSushi.Modules;

namespace TownOfSushi.Modifiers.Game.Crewmate;

public sealed class CelebrityModifier : TOSGameModifier, IWikiDiscoverable
{
    public override string ModifierName => "Celebrity";
    public override string IntroInfo => "You will also reveal info about your death in the meeting.";
    public override LoadableAsset<Sprite>? ModifierIcon => TOSModifierIcons.Celebrity;
    public override Color FreeplayFileColor => new Color32(140, 255, 255, 255);

    public override ModifierFaction FactionType => ModifierFaction.CrewmatePostmortem;

    public DateTime DeathTime { get; set; }
    public float DeathTimeMilliseconds { get; set; }
    public string DeathMessage { get; set; }
    public string AnnounceMessage { get; set; }
    public string StoredRoom { get; set; }
    public bool Announced { get; set; }

    public List<CustomButtonWikiDescription> Abilities { get; } = [];

    public string GetAdvancedDescription()
    {
        return
            "After you die, details about your death will be revealed such as where you were killed and which role killed you during the meeting.";
    }

    public override string GetDescription()
    {
        return "Announce how you died on your passing.";
    }

    public override int GetAssignmentChance()
    {
        return (int)OptionGroupSingleton<CelebrityOptions>.Instance.CelebrityChance;
    }

    public override int GetAmountPerGame()
    {
        return (int)OptionGroupSingleton<CelebrityOptions>.Instance.CelebrityAmount != 0 ? 1 : 0;
    }

    public override bool IsModifierValidOn(RoleBehaviour role)
    {
        return base.IsModifierValidOn(role) && role.IsCrewmate();
    }

    public static void CelebrityKilled(PlayerControl source, PlayerControl player, string customDeath = "")
    {
        if (!player.HasModifier<CelebrityModifier>())
        {
            Logger<TownOfSushiPlugin>.Error("RpcCelebrityKilled - Invalid Celebrity");
            return;
        }

        var room = MiscUtils.GetRoomName(player.GetTruePosition());

        var celeb = player.GetModifier<CelebrityModifier>()!;
        celeb.StoredRoom = room;
        celeb.DeathTime = DateTime.UtcNow;

        celeb.AnnounceMessage =
            $"<size=90%>The Celebrity, {player.GetDefaultAppearance().PlayerName}, has died!</size>\n<size=70%>(Details in chat)</size>";

        var cod = "killed";
        var role = source.GetRoleWhenAlive();
        if (source.Data.Role is IGhostRole)
        {
            role = source.Data.Role;
        }
        switch (role)
        {
            case VigilanteRole or VeteranRole:
                cod = "shot";
                break;
            case InquisitorRole:
                cod = "vanquished";
                break;
            case PyromaniacRole:
                cod = "ignited";
                break;
            case GlitchRole:
                cod = "bugged";
                break;
            case WarlockRole:
                cod = "cursed";
                break;
            case JuggernautRole:
                cod = "destroyed";
                break;
            case PestilenceRole:
                cod = "diseased";
                break;
            case VampireRole:
                cod = "bitten";
                break;
            case WerewolfRole:
                cod = "mauled";
                break;
            case PredatorRole:
                cod = "terminated";
                break;
            case JesterRole:
                cod = "haunted";
                break;
            case ExecutionerRole:
                cod = "tormented";
                break;
            case SpectreRole:
                cod = "spooked";
                break;
            case PoisonerRole:
                cod = "poisoned";
                break;
        }

        if (customDeath != string.Empty && customDeath != "")
        {
            cod = customDeath;
        }

        if (MeetingHud.Instance)
        {
            celeb.Announced = true;
        }

        if (source == player)
        {
            celeb.DeathMessage =
                $"The &Celebrity, {player.GetDefaultAppearance().PlayerName}, was killed! Location: {celeb.StoredRoom}, Death: By Suicide, Time: ";
        }
        else
        {
            celeb.DeathMessage =
                $"The &Celebrity, {player.GetDefaultAppearance().PlayerName}, was {cod}! Location: {celeb.StoredRoom}, Death: By the #{role.GetRoleName().ToLowerInvariant().Replace(" ", "-")}, Time: ";
        }
    }

    [MethodRpc((uint)TownOfSushiRpc.UpdateCelebrityKilled, SendImmediately = true)]
    public static void RpcUpdateCelebrityKilled(PlayerControl player, float milliseconds)
    {
        if (!player.HasModifier<CelebrityModifier>())
        {
            Logger<TownOfSushiPlugin>.Error("RpcUpdateCelebrityKilled - Invalid Celebrity");
            return;
        }

        Logger<TownOfSushiPlugin>.Error($"RpcUpdateCelebrityKilled milliseconds: {milliseconds}");

        var celeb = player.GetModifier<CelebrityModifier>()!;

        celeb.DeathTimeMilliseconds = milliseconds;
    }
}