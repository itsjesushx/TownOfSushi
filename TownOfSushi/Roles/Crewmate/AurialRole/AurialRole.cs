using System.Collections;
using System.Text;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Patches.Stubs;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using UnityEngine;
using Color = UnityEngine.Color;

namespace TownOfSushi.Roles.Crewmate;

public sealed class AurialRole(IntPtr cppPtr) : CrewmateRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable, IMysticClue
{
    private readonly Dictionary<(Vector3, int), ArrowBehaviour> _senseArrows = new();
    public string RoleName => "Aurial";
    public string RoleDescription => "Sense disturbances in your Aura";
    public string RoleLongDescription => "Any player abilities used within your aura you will sense";
    public MysticClueType MysticHintType => MysticClueType.Perception;
    public Color RoleColor => TownOfSushiColors.Aurial;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.CrewmateInvestigative;

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = TOSRoleIcons.Aurial,
        IntroSound = TOSAudio.MediumIntroSound
    };

    public void LobbyStart()
    {
        _senseArrows.Values.DestroyAll();
        _senseArrows.Clear();
    }

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ITownOfSushiRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            "The Aurial is a Crewmate Investigative role that will be alerted whenever a player near them uses one of their abilities." +
            MiscUtils.AppendOptionsText(GetType());
    }

    public override void Deinitialize(PlayerControl targetPlayer)
    {
        RoleBehaviourStubs.Deinitialize(this, targetPlayer);

        _senseArrows.Values.DestroyAll();
        _senseArrows.Clear();
    }

    [HideFromIl2Cpp]
    public IEnumerator Sense(PlayerControl player)
    {
        if (!CheckRange(player, OptionGroupSingleton<AurialOptions>.Instance.AuraOuterRadius))
        {
            yield break;
        }

        var position = player.transform.position;
        var colorID = player.Data.DefaultOutfit.ColorId;
        var color = Color.white;

        if (CheckRange(player,
                OptionGroupSingleton<AurialOptions>.Instance.AuraInnerRadius) /* && !CamouflageUnCamouflage.IsCamoed*/)
        {
            color = Palette.PlayerColors[colorID];
        }

        var arrow = MiscUtils.CreateArrow(Player.transform, color);
        arrow.target = position;

        try
        {
            DestroyArrow(position, colorID);
        }
        catch
        {
            /* ignored */
        }

        _senseArrows.Add((position, colorID), arrow);

        yield return new WaitForSeconds(OptionGroupSingleton<AurialOptions>.Instance.SenseDuration);

        try
        {
            DestroyArrow(position, colorID);
        }
        catch
        {
            /* ignored */
        }
    }

    public bool CheckRange(PlayerControl player, float radius)
    {
        var lightRadius = radius * ShipStatus.Instance.MaxLightRadius;
        var vector2 = new Vector2(player.GetTruePosition().x - Player.GetTruePosition().x,
            player.GetTruePosition().y - Player.GetTruePosition().y);
        var magnitude = vector2.magnitude;

        if (magnitude <= lightRadius)
        {
            return true;
        }

        return false;
    }

    public void DestroyArrow(Vector3 targetArea, int colourID)
    {
        var arrow = _senseArrows.FirstOrDefault(x => x.Key == (targetArea, colourID));

        if (arrow.Value != null)
        {
            Destroy(arrow.Value);
        }

        if (arrow.Value?.gameObject != null)
        {
            Destroy(arrow.Value.gameObject);
        }

        _senseArrows.Remove(arrow.Key);
    }

    [MethodRpc((uint)TownOfSushiRpc.AurialSense, SendImmediately = true)]
    public static void RpcSense(PlayerControl player, PlayerControl source)
    {
        if (player.Data.Role is not AurialRole aurial)
        {
            Logger<TownOfSushiPlugin>.Error("Invalid Aurial");
            return;
        }

        if (player.AmOwner)
        {
            Coroutines.Start(aurial.Sense(source));
        }
    }
}