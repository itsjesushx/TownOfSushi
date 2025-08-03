using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities;
using TownOfSushi.Modifiers;
using TownOfSushi.Modifiers.Impostor;
using TownOfSushi.Modifiers.Neutral;
using TownOfSushi.Options.Roles.Impostor;
using TownOfSushi.Roles.Impostor;
using TownOfSushi.Utilities;
using UnityEngine;

namespace TownOfSushi.Buttons.Impostor;

public sealed class WitchCurseButton : TownOfSushiRoleButton<WitchRole, PlayerControl>, IAftermathablePlayerButton
{
    public override string Name => "Curse";
    public override string Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Impostor;
    public override float Cooldown => OptionGroupSingleton<WitchOptions>.Instance.SpellCooldown + MapCooldown;
    public override float EffectDuration => OptionGroupSingleton<WitchOptions>.Instance.SpellDuration;
    public override LoadableAsset<Sprite> Sprite => TOSImpAssets.CurseSprite;
    public List<PlayerControl> CursedPlayers { get; set; } = new();
    public override bool CanUse()
    {
        return base.CanUse() && !PlayerControl.LocalPlayer.HasModifier<GlitchHackedModifier>() && !PlayerControl.LocalPlayer.HasModifier<DisabledModifier>();
    }
    protected override void OnClick()
    {
        if (Target == null || Target.HasModifier<FirstDeadShield>() || Target.HasModifier<BaseShieldModifier>())
        {
            return;
        }

        if (!CursedPlayers.Contains(Target))
        {
            CursedPlayers.Add(Target);
            TOSAudio.PlaySound(TOSAudio.WitchIntro);
        }
    }
    public override void OnEffectEnd()
    {
        foreach (var cursed in CursedPlayers.ToList())
        {
            if (cursed == null || cursed.Data.IsDead) continue;

            float distance = Vector2.Distance(
                PlayerControl.LocalPlayer.GetTruePosition(),
                cursed.GetTruePosition());

            if (distance > GameOptionsManager.Instance.currentNormalGameOptions.KillDistance)
            {
                Logger<TownOfSushiPlugin>.Info($"Target {cursed.name} moved too far away ({distance:F2}), curse canceled.");
                continue;
            }

            WitchRole.RpcWiitchSetCursedPlayer(PlayerControl.LocalPlayer, cursed);

            var notif = Helpers.CreateAndShowNotification(
                $"<b>{cursed.Data.PlayerName} will die after the next meeting if they don't die before that.</b>",
                Color.white, new Vector3(0f, 1f, -20f), spr: TOSImpAssets.CurseSprite.LoadAsset());

            notif.Text.SetOutlineThickness(0.35f);
        }

        CursedPlayers.Clear();
    }
    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(
            includeImpostors: OptionGroupSingleton<WitchOptions>.Instance.WitchCanSpellImpostors, Distance, false, predicate: plr => !plr.HasModifier<WitchSpelledModifier>());
    }
}