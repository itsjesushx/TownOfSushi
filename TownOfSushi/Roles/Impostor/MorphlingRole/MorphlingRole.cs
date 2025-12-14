using System.Text;
using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Hud;
using MiraAPI.Patches.Stubs;
using UnityEngine;

namespace TownOfSushi.Roles.Impostor;

public sealed class MorphlingRole(IntPtr cppPtr) : ImpostorRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable, IMysticClue
{
    [HideFromIl2Cpp]
    public PlayerControl? Sampled { get; set; }
    public string RoleName => "Morphling";
    public string RoleDescription => "Transform into other players";

    public string RoleLongDescription =>
        "Sample players and morph into them to disguise yourself.\nYour sample clears at the beginning of every round.";
    public MysticClueType MysticHintType => MysticClueType.Perception;

    public Color RoleColor => TownOfSushiColors.Impostor;
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;
    public RoleAlignment RoleAlignment => RoleAlignment.ImpostorConcealing;

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = TOSRoleIcons.Morphling,
        CanUseVent = OptionGroupSingleton<MorphlingOptions>.Instance.MorphlingVent,
        IntroSound = CustomRoleUtils.GetIntroSound(RoleTypes.Shapeshifter)
    };

    public void LobbyStart()
    {
        Clear();
    }

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        var stringB = ITownOfSushiRole.SetNewTabText(this);

        if (Player.HasModifier<MorphlingMorphModifier>())
        {
            stringB.Append(TownOfSushiPlugin.Culture,
                $"\n<b>Morphed As:</b> {Sampled!.Data.Color.ToTextColor()}{Sampled.Data.PlayerName}</color>");
        }

        return stringB;
    }

    public string GetAdvancedDescription()
    {
        return $"The {RoleName} is an Impostor Concealing role that can Sample a player and Morph into it's appearance."
               + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Sample",
            "Take a DNA sample of a player to morph into them later.",
            TOSImpAssets.SampleSprite),
        new("Morph",
            "Morph into the appearance of the sampled player, which can be cancelled early.",
            TOSImpAssets.MorphSprite)
    ];

    public override void OnVotingComplete()
    {
        RoleBehaviourStubs.OnVotingComplete(this);

        Clear();
    }

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);
        CustomButtonSingleton<MorphlingMorphButton>.Instance.SetActive(false, this);
    }

    public override void Deinitialize(PlayerControl targetPlayer)
    {
        RoleBehaviourStubs.Deinitialize(this, targetPlayer);

        Clear();
    }

    public void Clear()
    {
        Sampled = null;
    }
}