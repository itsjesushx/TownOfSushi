using TownOfSushi.Modifiers;
using TownOfSushi.Modules;
using TownOfSushi.Options;
using UnityEngine;

namespace TownOfSushi.Utilities;

public static class PlayerRoleTextExtensions
{
    public static Color UpdateTargetColor(this Color color, PlayerControl player, bool hidden = false)
    {
        if (player.HasModifier<EclipsalBlindModifier>() && PlayerControl.LocalPlayer.IsImpostor())
        {
            color = Color.black;
        }

        if (player.HasModifier<GrenadierFlashModifier>() && !player.IsImpostor() &&
            PlayerControl.LocalPlayer.IsImpostor())
        {
            color = Color.black;
        }

        if (player.HasModifier<DetectiveGoodRevealModifier>() && PlayerControl.LocalPlayer.IsRole<DetectiveRole>())
        {
            color = Color.green;
        }
        else if (player.HasModifier<DetectiveEvilRevealModifier>() && PlayerControl.LocalPlayer.IsRole<DetectiveRole>())
        {
            color = Color.red;
        }

        if (player.HasModifier<PoliticianCampaignedModifier>(x => x.Politician.AmOwner) &&
            PlayerControl.LocalPlayer.IsRole<PoliticianRole>())
        {
            color = Color.cyan;
        }

        return color;
    }

    public static string UpdateTargetSymbols(this string name, PlayerControl player, bool hidden = false)
    {
        var genOpt = OptionGroupSingleton<GeneralOptions>.Instance;
        if ((player.HasModifier<ExecutionerTargetModifier>(x => x.OwnerId == PlayerControl.LocalPlayer.PlayerId) &&
             PlayerControl.LocalPlayer.IsRole<ExecutionerRole>())
            || (player.HasModifier<ExecutionerTargetModifier>() && PlayerControl.LocalPlayer.HasDied() &&
                genOpt.TheDeadKnow && !hidden))
        {
            name += "<color=#643B1F> X</color>";
        }

        if (player.HasModifier<InquisitorHereticModifier>() && PlayerControl.LocalPlayer.HasDied() &&
            (genOpt.TheDeadKnow || PlayerControl.LocalPlayer.GetRoleWhenAlive() is InquisitorRole) && !hidden)
        {
            name += "<color=#D94291> $</color>";
        }

        if (PlayerControl.LocalPlayer.Data.Role is HunterRole &&
            player.HasModifier<HunterStalkedModifier>(x => x.Hunter.AmOwner))
        {
            name += "<color=#29AB87> &</color>";
        }

        if ((player.HasModifier<WarlockCursedModifier>() && PlayerControl.LocalPlayer.IsRole<WarlockRole>() && !PlayerControl.LocalPlayer.HasDied())
            || (player.HasModifier<WarlockCursedModifier>() && PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow && !hidden))
        {
            name += "<color=#FF0000> [C]</color>";
        }

        if (PlayerControl.LocalPlayer.Data.Role is HunterRole hunter && hunter.CaughtPlayers.Contains(player))
        {
            name += "<color=#21453B> &</color>";
        }

        return name;
    }

    public static string UpdateProtectionSymbols(this string name, PlayerControl player, bool hidden = false)
    {
        var genOpt = OptionGroupSingleton<GeneralOptions>.Instance;
        if (player.Data != null && !player.Data.Disconnected &&
            ((player.HasModifier<GuardianAngelTargetModifier>(x => x.OwnerId == PlayerControl.LocalPlayer.PlayerId) &&
              PlayerControl.LocalPlayer.IsRole<GuardianAngelTOSRole>())
             || (player.HasModifier<GuardianAngelTargetModifier>() &&
                 ((PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow && !hidden)
                  || (player.AmOwner &&
                      OptionGroupSingleton<GuardianAngelOptions>.Instance.GATargetKnows)))))
        {
            name += (player.HasModifier<GuardianAngelProtectModifier>() &&
                     OptionGroupSingleton<GuardianAngelOptions>.Instance.ShowProtect is not ProtectOptions.GA)
                ? "<color=#FFD900> ★</color>"
                : "<color=#B3FFFF> ★</color>";
        }

        if (player.Data != null && !player.Data.Disconnected &&
            ((player.HasModifier<RomanticBelovedModifier>(x => x.OwnerId == PlayerControl.LocalPlayer.PlayerId) &&
              PlayerControl.LocalPlayer.IsRole<RomanticRole>())
             || (player.HasModifier<RomanticBelovedModifier>() &&
                 ((PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow && !hidden)
                  || (player.AmOwner &&
                      OptionGroupSingleton<GuardianAngelOptions>.Instance.GATargetKnows)))))
        {
            
            name += (player.HasModifier<RomanticProtectModifier>() && OptionGroupSingleton<RomanticOptions>.Instance.ShowProtect is not RomanticProtectOptions.Romantic)
                ? "<color=#FFFFFF> ★</color>"
                : "<color=#d86bff> ★</color>";
        }

        if ((player.HasModifier<MedicShieldModifier>(x => x.Medic.AmOwner) &&
             PlayerControl.LocalPlayer.IsRole<MedicRole>())
            || (player.HasModifier<MedicShieldModifier>() &&
                ((PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow && !hidden)
                 || (player.AmOwner && player.TryGetModifier<MedicShieldModifier>(out var med) && med.VisibleSymbol))))
        {
            name += "<color=#7EFBC2> +</color>";
        }

        if ((player.HasModifier<BodyguardGuardedModifier>(x => x.Bodyguard.AmOwner) &&
             PlayerControl.LocalPlayer.IsRole<BodyguardRole>())
            || (player.HasModifier<BodyguardGuardedModifier>() &&
                ((PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow && !hidden)
                 || (player.AmOwner && player.TryGetModifier<BodyguardGuardedModifier>(out var bod) && bod.VisibleSymbol))))
        {
            name += "<color=#0D4D33> [+]</color>";
        }

        if ((player.HasModifier<MonarchKnightedModifier>(x => x.Monarch.AmOwner) &&
             PlayerControl.LocalPlayer.IsRole<MonarchRole>())
            || (player.HasModifier<MonarchKnightedModifier>() &&
                PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow && !hidden))
        {
            name += "<color=#FF8400> [★]</color>";
        }

        if ((player.HasModifier<ClericBarrierModifier>(x => x.Cleric.AmOwner) &&
             PlayerControl.LocalPlayer.IsRole<ClericRole>())
            || (player.HasModifier<ClericBarrierModifier>() &&
                ((PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow && !hidden)
                 || (player.AmOwner && player.TryGetModifier<ClericBarrierModifier>(out var cleric) &&
                     cleric.VisibleSymbol))))
        {
            name += "<color=#00FFB3> Ω</color>";
        }

        if ((player.HasModifier<CrusaderFortifiedModifier>(x => x.Crusader.AmOwner) &&
             PlayerControl.LocalPlayer.IsRole<CrusaderRole>())
            || (player.HasModifier<CrusaderFortifiedModifier>() &&
                ((PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow && !hidden)
                 || (player.AmOwner && player.TryGetModifier<CrusaderFortifiedModifier>(out var Crusader) &&
                     Crusader.VisibleSymbol))))
        {
            name += "<color=#9900FF> π</color>";
        }

        return name;
    }

    public static string UpdateAllianceSymbols(this string name, PlayerControl player, bool hidden = false)
    {
        var genOpt = OptionGroupSingleton<GeneralOptions>.Instance;

        if (player.HasModifier<LoverModifier>() && (PlayerControl.LocalPlayer.HasModifier<LoverModifier>() ||
                                                    (PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow &&
                                                     !hidden)))
        {
            name += "<color=#FF66CC> ♥</color>";
        }

        if (player.HasModifier<EgotistModifier>() && (player.AmOwner ||
                                                      (EgotistModifier.EgoVisibilityFlag(player) &&
                                                       (player.GetModifiers<RevealModifier>().Any(x => x.Visible && x.RevealRole))) ||
                                                      (PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow &&
                                                       !hidden)))
        {
            name += "<color=#FFFFFF> (<color=#669966>Egotist</color>)</color>";
        }

        return name;
    }

    public static string UpdateStatusSymbols(this string name, PlayerControl player, bool hidden = false)
    {
        var genOpt = OptionGroupSingleton<GeneralOptions>.Instance;

        if ((player.HasModifier<PlaguebearerInfectedModifier>(x =>
                 x.PlagueBearerId == PlayerControl.LocalPlayer.PlayerId) &&
             PlayerControl.LocalPlayer.IsRole<PlaguebearerRole>())
            || (player.HasModifier<PlaguebearerInfectedModifier>() && PlayerControl.LocalPlayer.HasDied() &&
                genOpt.TheDeadKnow && !hidden))
        {
            name += "<color=#E6FFB3> ¥</color>";
        }

        if ((player.HasModifier<PyromaniacDousedModifier>(x => x.PyromaniacId == PlayerControl.LocalPlayer.PlayerId) &&
             PlayerControl.LocalPlayer.IsRole<PyromaniacRole>())
            || (player.HasModifier<PyromaniacDousedModifier>() && PlayerControl.LocalPlayer.HasDied() &&
                genOpt.TheDeadKnow && !hidden))
        {
            name += "<color=#F07B48> Δ</color>";
        }

        if ((player.HasModifier<ArsonistDousedModifier>(x => x.ArsonistId == PlayerControl.LocalPlayer.PlayerId) &&
             PlayerControl.LocalPlayer.IsRole<ArsonistRole>())
            || (player.HasModifier<ArsonistDousedModifier>() && PlayerControl.LocalPlayer.HasDied() &&
                genOpt.TheDeadKnow && !hidden))
        {
            name += "<color=#FF4D00> Δ</color>";
        }

        if ((player.HasModifier<BlackmailedModifier>(x => x.BlackMailerId == PlayerControl.LocalPlayer.PlayerId) &&
             PlayerControl.LocalPlayer.IsRole<BlackmailerRole>())
            || (player.HasModifier<BlackmailedModifier>() && PlayerControl.LocalPlayer.IsImpostor() &&
                genOpt.ImpsKnowRoles)
            || (player.HasModifier<BlackmailedModifier>() && PlayerControl.LocalPlayer.HasDied() &&
                genOpt.TheDeadKnow && !hidden))
        {
            name += "<color=#2A1119> M</color>";
        }

        if ((player.HasModifier<HypnotisedModifier>(x => x.Hypnotist.AmOwner) &&
             PlayerControl.LocalPlayer.IsRole<HypnotistRole>())
            || (player.HasModifier<HypnotisedModifier>() && PlayerControl.LocalPlayer.IsImpostor() &&
                genOpt.ImpsKnowRoles)
            || (player.HasModifier<HypnotisedModifier>() && PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow &&
                !hidden))
        {
            name += "<color=#D53F42> @</color>";
        }

        return name;
    }
}