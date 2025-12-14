﻿using System.Collections;
using HarmonyLib;
using MiraAPI.Events;
using System.Text;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Events.Vanilla.Meeting;
using MiraAPI.Events.Vanilla.Meeting.Voting;
using MiraAPI.Events.Vanilla.Player;
using MiraAPI.Events.Vanilla.Usables;
using MiraAPI.Hud;
using MiraAPI.Modifiers.ModifierDisplay;
using MiraAPI.Modifiers.Types;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;
using TownOfSushi.Options;
using TownOfSushi.Modules;
using TownOfSushi.Buttons;
using TownOfSushi.Modifiers;
using TownOfSushi.Patches;
using TownOfSushi.Events.TOSEvents;
using TownOfSushi.Modules.Anims;
using TownOfSushi.Patches.CustomPolus;
using TownOfSushi.Modifiers.Game;

namespace TownOfSushi.Events;
public static class TownOfSushiEventHandlers
{
    internal static TextMeshPro ModifierText;
    public static void RunModChecks()
    {

            var modifiers = PlayerControl.LocalPlayer.GetModifiers<GameModifier>()
            .Where(x => x is TOSGameModifier || x is UniversalGameModifier).ToList();

        if (modifiers.Count > 0)
        {
            var modifierText = new StringBuilder();
            modifierText.Append("<size=4><color=#FFFFFF>Modifiers: </color>");

            int count = modifiers.Count;
            foreach (var modifier in modifiers)
            {
                var color = MiscUtils.GetRoleColour(modifier.ModifierName.Replace(" ", string.Empty));
                if (modifier is IColoredModifier colorMod)
                    color = colorMod.ModifierColor;

                count--;
                modifierText.Append($"{color.ToTextColor()}{modifier.ModifierName}</color>");
                modifierText.Append(count > 0 ? ", " : "");
            }

            modifierText.Append("</size>");
            ModifierText.text = modifierText.ToString();
            ModifierText.color = Color.white;
        }
        else
        {
            ModifierText.text = string.Empty;
        }
    }

    [RegisterEvent]
    public static void IntroBeginEventHandler(IntroBeginEvent @event)
    {
        var cutscene = @event.IntroCutscene;
        Coroutines.Start(CoChangeModifierText(cutscene));
    }

    public static IEnumerator CoChangeModifierText(IntroCutscene cutscene)
    {
        yield return new WaitForSeconds(0.01f);

        ModifierText =
            Object.Instantiate(cutscene.RoleText, cutscene.RoleText.transform.parent, false);

        if (PlayerControl.LocalPlayer.Data.Role is ITownOfSushiRole custom)
        {
            cutscene.RoleText.text = custom.RoleName;
            cutscene.YouAreText.text = custom.YouAreText;
            cutscene.RoleBlurbText.text = custom.RoleDescription;
        }

        var teamModifier = PlayerControl.LocalPlayer.GetModifiers<TOSGameModifier>().FirstOrDefault();
        if (teamModifier != null)
        {
            var color = MiscUtils.GetModifierColour(teamModifier);

            cutscene.RoleBlurbText.text =
                $"<size={teamModifier.IntroSize}>\n</size>{cutscene.RoleBlurbText.text}\n<size={teamModifier.IntroSize}><color=#{color.ToHtmlStringRGBA()}>{teamModifier.IntroInfo}</color></size>";
        }

        RunModChecks();

        ModifierText.transform.position =
            cutscene.transform.position - new Vector3(0f, 1.6f, -10f);
        ModifierText.gameObject.SetActive(true);
        ModifierText.color.SetAlpha(0.8f);
    }

    [RegisterEvent]
    public static void IntroEndEventHandler(IntroEndEvent @event)
    {
        HudManager.Instance.SetHudActive(false);
        HudManager.Instance.SetHudActive(true);

        foreach (var button in CustomButtonManager.Buttons.Where(x => x.Enabled(PlayerControl.LocalPlayer.Data.Role)))
        {
            if (button is FakeVentButton)
            {
                continue;
            }

            button.SetTimer(OptionGroupSingleton<GeneralOptions>.Instance.GameStartCd);
        }

        if (PlayerControl.LocalPlayer.IsImpostor())
        {
            PlayerControl.LocalPlayer.SetKillTimer(OptionGroupSingleton<GeneralOptions>.Instance.GameStartCd);
        }

        var modsTab = ModifierDisplayComponent.Instance;
        if (modsTab != null && !modsTab.IsOpen && PlayerControl.LocalPlayer.GetModifiers<GameModifier>()
                .Any(x => !x.HideOnUi && x.GetDescription() != string.Empty))
        {
            modsTab.ToggleTab();
        }

        var panelThing = HudManager.Instance.TaskStuff.transform.FindChild("RolePanel");
        if (panelThing != null)
        {
            var panel = panelThing.gameObject.GetComponent<TaskPanelBehaviour>();
            var role = PlayerControl.LocalPlayer.Data.Role as ICustomRole;
            if (role == null)
            {
                return;
            }

            panel.open = true;

            var tabText = panel.tab.gameObject.GetComponentInChildren<TextMeshPro>();
            var ogPanel = HudManager.Instance.TaskStuff.transform.FindChild("TaskPanel").gameObject
                .GetComponent<TaskPanelBehaviour>();
            if (tabText.text != role.RoleName)
            {
                tabText.text = role.RoleName;
            }

            var y = ogPanel.taskText.textBounds.size.y + 1;
            panel.closedPosition = new Vector3(ogPanel.closedPosition.x, ogPanel.open ? y + 0.2f : 2f,
                ogPanel.closedPosition.z);
            panel.openPosition = new Vector3(ogPanel.openPosition.x, ogPanel.open ? y : 2f, ogPanel.openPosition.z);

            panel.SetTaskText(role.SetTabText().ToString());
        }
        if (GameOptionsManager.Instance.currentNormalGameOptions.MapId == 2 && OptionGroupSingleton<BetterPolusOptions>.Instance.BPCustomSpeciVent)
        {
            var list = GameObject.FindObjectsOfType<Vent>().ToList();
            var adminVent = list.FirstOrDefault(x => x.gameObject.name == "AdminVent");
            var bathroomVent = list.FirstOrDefault(x => x.gameObject.name == "BathroomVent");
            BetterPolus.SpecimenVent = UnityEngine.Object.Instantiate<Vent>(adminVent!);
            BetterPolus.SpecimenVent.gameObject.AddSubmergedComponent(ModCompatibility.Classes.ElevatorMover);
            BetterPolus.SpecimenVent.transform.position = new Vector3(36.55068f, -21.5168f, -0.0215168f);
            BetterPolus.SpecimenVent.Left = adminVent;
            BetterPolus.SpecimenVent.Right = bathroomVent;
            BetterPolus.SpecimenVent.Center = null;
            BetterPolus.SpecimenVent.Id = ShipStatus.Instance.AllVents.Select(x => x.Id).Max() + 1;
            var allVentsList = ShipStatus.Instance.AllVents.ToList();
            allVentsList.Add(BetterPolus.SpecimenVent);
            ShipStatus.Instance.AllVents = allVentsList.ToArray();
            BetterPolus.SpecimenVent.gameObject.SetActive(true);
            BetterPolus.SpecimenVent.name = "newVent_" + BetterPolus.SpecimenVent.Id;

            adminVent!.Center = BetterPolus.SpecimenVent;
            bathroomVent!.Center = BetterPolus.SpecimenVent;
        }
    }

    [RegisterEvent]
    public static void StartMeetingEventHandler(StartMeetingEvent @event)
    {
        MiscUtils.Meetingtime = GameOptionsManager.Instance.currentNormalGameOptions.DiscussionTime + GameOptionsManager.Instance.currentNormalGameOptions.VotingTime + 7f;
        
        foreach (var mod in ModifierUtils.GetActiveModifiers<MisfortuneTargetModifier>())
        {
            mod.ModifierComponent?.RemoveModifier(mod);
        }

        var exeButton = CustomButtonSingleton<ExeTormentButton>.Instance;
        var jestButton = CustomButtonSingleton<JesterHauntButton>.Instance;
        var spectreButton = CustomButtonSingleton<SpectreSpookButton>.Instance;
        if (exeButton.Show || jestButton.Show || spectreButton.Show)
        {
            PlayerControl.LocalPlayer.RpcRemoveModifier<IndirectAttackerModifier>();
        }
        exeButton.Show = false;
        jestButton.Show = false;
        spectreButton.Show = false;

        if (DestroyableSingleton<HudManager>.Instance.FullScreen == null) return;
        
        DestroyableSingleton<HudManager>.Instance.FullScreen.gameObject.SetActive(true);
        DestroyableSingleton<HudManager>.Instance.FullScreen.enabled = true;
        var color = Color.black;

        DestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(0.8f, new Action<float>((p) =>
        {
            DestroyableSingleton<HudManager>.Instance.FullScreen.color = new Color(color.r, color.g, color.b, Mathf.Clamp01(1 - p));
            if (p == 1)
            {
                DestroyableSingleton<HudManager>.Instance.FullScreen.enabled = false;
            }
        })));
    }
    
    [RegisterEvent]
    public static void RoundStartHandler(RoundStartEvent @event)
    {
        if (!@event.TriggeredByIntro)
        {
            return; // Only run when game starts.
        }

        if (FirstDeadPatch.PlayerNames.Count > 0)
        {
            var stringB = new StringBuilder();
            stringB.Append(TownOfSushiPlugin.Culture, $"List Of Players That Died In Order: ");
            foreach (var playername in FirstDeadPatch.PlayerNames)
            {
                stringB.Append(TownOfSushiPlugin.Culture, $"{playername}, ");
            }
            
            stringB = stringB.Remove(stringB.Length - 2, 2);
            
            Logger<TownOfSushiPlugin>.Warning(stringB.ToString());
        }
        FirstDeadPatch.PlayerNames = [];

        HudManager.Instance.SetHudActive(false);
        HudManager.Instance.SetHudActive(true);

        CustomButtonSingleton<SonarTrackButton>.Instance.ExtraUses = 0;
        CustomButtonSingleton<SonarTrackButton>.Instance.SetUses((int)OptionGroupSingleton<SonarOptions>.Instance
            .MaxTracks);
        CustomButtonSingleton<TrapperTrapButton>.Instance.ExtraUses = 0;
        CustomButtonSingleton<TrapperTrapButton>.Instance.SetUses((int)OptionGroupSingleton<TrapperOptions>.Instance
            .MaxTraps);

        CustomButtonSingleton<HunterStalkButton>.Instance.ExtraUses = 0;
        CustomButtonSingleton<HunterStalkButton>.Instance.SetUses((int)OptionGroupSingleton<HunterOptions>.Instance
            .StalkUses);
        CustomButtonSingleton<VigilanteShootButton>.Instance.Usable =
            OptionGroupSingleton<VigilanteOptions>.Instance.FirstRoundUse;
        CustomButtonSingleton<VeteranAlertButton>.Instance.ExtraUses = 0;
        CustomButtonSingleton<VeteranAlertButton>.Instance.SetUses((int)OptionGroupSingleton<VeteranOptions>.Instance
            .MaxNumAlerts);

        CustomButtonSingleton<JailorJailButton>.Instance.ExecutedACrew = false;

        var engiVent = CustomButtonSingleton<EngineerVentButton>.Instance;
        engiVent.ExtraUses = 0;
        engiVent.SetUses((int)OptionGroupSingleton<EngineerOptions>.Instance.MaxVents);
        if ((int)OptionGroupSingleton<EngineerOptions>.Instance.MaxVents == 0)
        {
            engiVent.Button?.usesRemainingText.gameObject.SetActive(false);
            engiVent.Button?.usesRemainingSprite.gameObject.SetActive(false);
        }
        else
        {
            engiVent.Button?.usesRemainingText.gameObject.SetActive(true);
            engiVent.Button?.usesRemainingSprite.gameObject.SetActive(true);
        }
        
        var medicShield = CustomButtonSingleton<MedicShieldButton>.Instance;
        medicShield.SetUses(OptionGroupSingleton<MedicOptions>.Instance.ChangeTarget ? (int)OptionGroupSingleton<MedicOptions>.Instance.MedicShieldUses : 0);
        if ((int)OptionGroupSingleton<MedicOptions>.Instance.MedicShieldUses == 0 || !OptionGroupSingleton<MedicOptions>.Instance.ChangeTarget)
        {
            medicShield.Button?.usesRemainingText.gameObject.SetActive(false);
            medicShield.Button?.usesRemainingSprite.gameObject.SetActive(false);
        }
        else
        {
            medicShield.Button?.usesRemainingText.gameObject.SetActive(true);
            medicShield.Button?.usesRemainingSprite.gameObject.SetActive(true);
        }

        CustomButtonSingleton<PlumberBlockButton>.Instance.ExtraUses = 0;
        CustomButtonSingleton<PlumberBlockButton>.Instance.SetUses((int)OptionGroupSingleton<PlumberOptions>.Instance
            .MaxBarricades);
        CustomButtonSingleton<TransporterTransportButton>.Instance.ExtraUses = 0;
        CustomButtonSingleton<TransporterTransportButton>.Instance.SetUses((int)OptionGroupSingleton<TransporterOptions>
            .Instance.MaxNumTransports);

        CustomButtonSingleton<HexbladeKillButton>.Instance.Charge = 0f;
        CustomButtonSingleton<HexbladeKillButton>.Instance.BurstActive = false;

        CustomButtonSingleton<BarryButton>.Instance.Usable =
            OptionGroupSingleton<ButtonBarryOptions>.Instance.FirstRoundUse || TutorialManager.InstanceExists;
        CustomButtonSingleton<SatelliteButton>.Instance.Usable =
            OptionGroupSingleton<SatelliteOptions>.Instance.FirstRoundUse || TutorialManager.InstanceExists;
    }

    [RegisterEvent]
    public static void ChangeRoleHandler(ChangeRoleEvent @event)
    {
        if (!PlayerControl.LocalPlayer)
        {
            return;
        }

        var player = @event.Player;
        if (!MeetingHud.Instance && player.AmOwner)
        {
            foreach (var button in CustomButtonManager.Buttons)
            {
                if (button is TownOfSushiTargetButton<PlayerControl> touPlayerButton && touPlayerButton.Target != null)
                {
                    touPlayerButton.Target.cosmetics.currentBodySprite.BodySprite.SetOutline(null);
                }
                else if (button is TownOfSushiTargetButton<DeadBody> touBodyButton && touBodyButton.Target != null)
                {
                    touBodyButton.Target.bodyRenderers.Do(x => x.SetOutline(null));
                }
                else if (button is TownOfSushiTargetButton<Vent> touVentButton && touVentButton.Target != null)
                {
                    touVentButton.Target.SetOutline(false, true, player.Data.Role.TeamColor);
                }
            }
            HudManager.Instance.SetHudActive(false);
            HudManager.Instance.SetHudActive(true);
        }
    }

    [RegisterEvent]
    public static void SetRoleHandler(SetRoleEvent @event)
    {
        GameHistory.RegisterRole(@event.Player, @event.Player.Data.Role);
    }

    [RegisterEvent]
    public static void ClearBodiesAndResetPlayersEventHandler(RoundStartEvent @event)
    {
        Object.FindObjectsOfType<DeadBody>().ToList().ForEach(x => x.gameObject.Destroy());

        foreach (var player in PlayerControl.AllPlayerControls)
        {
            player.MyPhysics.ResetAnimState();
            player.MyPhysics.ResetMoveState();
        }
    }
    [RegisterEvent]
    public static void EjectionEventHandler(EjectionEvent @event)
    {
        var exiled = @event.ExileController?.initData?.networkedPlayer?.Object;
        if (exiled == null)
        {
            return;
        }
        foreach (var amnesiac in CustomRoleUtils.GetActiveRolesOfType<AmnesiacRole>())
        {
            if (!amnesiac.EjectedPlayers.Contains(exiled))
            {
                amnesiac.EjectedPlayers.Add(exiled);
            }
        }

        if (exiled.AmOwner)
        {
            HudManager.Instance.SetHudActive(false);

            if (!MeetingHud.Instance)
            {
                HudManager.Instance.SetHudActive(true);
            }
        }

        if (exiled.Data.Role is IAnimated animated)
        {
            animated.IsVisible = false;
            animated.SetVisible();
        }

        foreach (var button in CustomButtonManager.Buttons.Where(x => x.Enabled(exiled.Data.Role)).OfType<IAnimated>())
        {
            button.IsVisible = false;
            button.SetVisible();
        }

        foreach (var modifier in exiled.GetModifiers<GameModifier>().Where(x => x is IAnimated))
        {
            var animatedMod = modifier as IAnimated;
            if (animatedMod != null)
            {
                animatedMod.IsVisible = false;
                animatedMod.SetVisible();
            }
        }
    }

    [RegisterEvent]
    public static void AfterMurderEventHandler(AfterMurderEvent murderEvent)
    {
        var source = murderEvent.Source;
        var target = murderEvent.Target;

        GameHistory.AddMurder(source, target);

        if (target.AmOwner)
        {
            HudManager.Instance.SetHudActive(false);

            if (!MeetingHud.Instance)
            {
                HudManager.Instance.SetHudActive(true);
            }
        }

        if (target.Data.Role is IAnimated animated)
        {
            animated.IsVisible = false;
            animated.SetVisible();
        }

        foreach (var button in CustomButtonManager.Buttons.Where(x => x.Enabled(target.Data.Role)).OfType<IAnimated>())
        {
            button.IsVisible = false;
            button.SetVisible();
        }

        foreach (var modifier in target.GetModifiers<GameModifier>().Where(x => x is IAnimated))
        {
            var animatedMod = modifier as IAnimated;
            if (animatedMod != null)
            {
                animatedMod.IsVisible = false;
                animatedMod.SetVisible();
            }
        }

        if (source.IsImpostor() && source.AmOwner && source != target && !MeetingHud.Instance)
        {
            switch (source.Data.Role)
            {
                case BomberRole:
                    var bombButton = CustomButtonSingleton<BomberPlantButton>.Instance;
                    bombButton.ResetCooldownAndOrEffect();
                    break;
                case ConsigliereRole:
                    var consigButton = CustomButtonSingleton<ConsigliereRevealButton>.Instance;
                    consigButton.ResetCooldownAndOrEffect();
                    break;
                case JanitorRole:
                    if (OptionGroupSingleton<JanitorOptions>.Instance.ResetCooldowns)
                    {
                        var cleanButton = CustomButtonSingleton<JanitorCleanButton>.Instance;
                        cleanButton.ResetCooldownAndOrEffect();
                    }

                    break;
            }
        }

        // here we're adding support for kills during a meeting
        if (MeetingHud.Instance)
        {
            HandleMeetingMurder(MeetingHud.Instance, source, target);
        }
        else
        {
            var body = Object.FindObjectsOfType<DeadBody>().FirstOrDefault(x => x.ParentId == target.PlayerId);

            if (target.HasModifier<MiniModifier>() && body != null)
            {
                body.transform.localScale *= 0.7f;
            }

            if (target.HasModifier<GiantModifier>() && body != null)
            {
                body.transform.localScale /= 0.7f;
            }

            if (target.AmOwner)
            {
                if (Minigame.Instance != null)
                {
                    Minigame.Instance.Close();
                    Minigame.Instance.Close();
                }

                if (MapBehaviour.Instance != null)
                {
                    MapBehaviour.Instance.Close();
                    MapBehaviour.Instance.Close();
                }
            }
        }
    }

    [RegisterEvent]
    public static void PlayerCanUseEventHandler(PlayerCanUseEvent @event)
    {
        if (!PlayerControl.LocalPlayer || !PlayerControl.LocalPlayer.Data ||
            !PlayerControl.LocalPlayer.Data.Role)
        {
            return;
        }

        // Prevent last 2 players from venting
        if (@event.IsVent)
        {
            var aliveCount = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.HasDied());

            if (PlayerControl.LocalPlayer.inVent && aliveCount <= 2 &&
                PlayerControl.LocalPlayer.Data.Role is not IGhostRole)
            {
                PlayerControl.LocalPlayer.MyPhysics.RpcExitVent(Vent.currentVent.Id);
                PlayerControl.LocalPlayer.MyPhysics.ExitAllVents();
            }

            if (aliveCount <= 2)
            {
                @event.Cancel();
            }
        }
    }

    [RegisterEvent]
    public static void PlayerLeaveEventHandler(PlayerLeaveEvent @event)
    {
        if (!MeetingHud.Instance)
        {
            return;
        }

        var player = @event.ClientData.Character;

        if (!player)
        {
            return;
        }

        var pva = MeetingHud.Instance.playerStates.First(x => x.TargetPlayerId == player.PlayerId);

        if (!pva)
        {
            return;
        }

        pva.AmDead = true;
        pva.Overlay.gameObject.SetActive(true);
        pva.Overlay.color = Color.white;
        pva.XMark.gameObject.SetActive(false);
        pva.XMark.transform.localScale = Vector3.one;

        MeetingMenu.Instances.Do(x => x.HideSingle(player.PlayerId));
    }

    public static IEnumerator CoHideHud()
    {
        yield return new WaitForSeconds(0.01f);
        HudManager.Instance.SetHudActive(false);
    }

    public static void HandleMeetingMurder(MeetingHud instance, PlayerControl source, PlayerControl target)
    {
        if (MeetingHud.Instance.CurrentState == MeetingHud.VoteStates.Animating)
        {
            if (target.AmOwner)
            {
                MeetingMenu.Instances.Do(x => x.HideButtons());
                Coroutines.Start(CoHideHud());
            }
            // hide meeting menu button for victim
            else if (!source.AmOwner && !target.AmOwner)
            {
                MeetingMenu.Instances.Do(x => x.HideSingle(target.PlayerId));
            }
            var targetVoteAreaEarly = instance.playerStates.First(x => x.TargetPlayerId == target.PlayerId);

            if (!targetVoteAreaEarly)
            {
                return;
            }

            targetVoteAreaEarly.AmDead = true;
            targetVoteAreaEarly.Overlay.gameObject.SetActive(true);
            targetVoteAreaEarly.XMark.gameObject.SetActive(true);
            return;
        }
        var timer = (int)OptionGroupSingleton<GeneralOptions>.Instance.AddedMeetingDeathTimer;
        if (timer > 0 && timer <= 15)
        {
            instance.discussionTimer -= timer;
        }
        else if (timer >= 15)
        {
            instance.discussionTimer -= 15f;
        }
        // To handle murders during a meeting
        var targetVoteArea = instance.playerStates.First(x => x.TargetPlayerId == target.PlayerId);

        if (!targetVoteArea)
        {
            return;
        }

        if (targetVoteArea.DidVote)
        {
            targetVoteArea.UnsetVote();
        }

        targetVoteArea.AmDead = true;
        targetVoteArea.Overlay.gameObject.SetActive(true);
        targetVoteArea.Overlay.color = Color.white;
        targetVoteArea.XMark.gameObject.SetActive(false);
        targetVoteArea.XMark.transform.localScale = Vector3.one;

        if (Minigame.Instance != null)
        {
            Minigame.Instance.Close();
            Minigame.Instance.Close();
        }

        targetVoteArea.Overlay.gameObject.SetActive(false);
        if (target.GetRoleWhenAlive() is MayorRole mayor && mayor.Revealed)
        {
            MayorRole.DestroyReveal(targetVoteArea);
        }
        targetVoteArea.Overlay.gameObject.SetActive(false);
        targetVoteArea.Overlay.gameObject.SetActive(true);
        targetVoteArea.XMark.gameObject.SetActive(true);

        SoundManager.Instance.PlaySound(targetVoteArea.GetPlayer()!.KillSfx, false);
        if (target.AmOwner || PlayerControl.LocalPlayer.AmOwner && PlayerControl.LocalPlayer.Data.IsDead)
        {
            DestroyableSingleton<HudManager>.Instance.KillOverlay.ShowKillAnimation(source.Data, target.Data);
        }
        else
        {
            DestroyableSingleton<HudManager>.Instance.KillOverlay.ShowKillAnimation(target.Data, target.Data);
        }

        targetVoteArea.Overlay.gameObject.SetActive(true);

        // hide meeting menu buttons on the victim's screen
        if (target.AmOwner)
        {
            MeetingMenu.Instances.Do(x => x.HideButtons());
            Coroutines.Start(CoHideHud());
        }
        // hide meeting menu button for victim
        else if (!source.AmOwner && !target.AmOwner)
        {
            MeetingMenu.Instances.Do(x => x.HideSingle(target.PlayerId));
        }

        foreach (var pva in instance.playerStates)
        {
            if (pva.VotedFor != target.PlayerId || pva.AmDead)
            {
                continue;
            }

            pva.UnsetVote();

            var voteAreaPlayer = MiscUtils.PlayerById(pva.TargetPlayerId);

            if (voteAreaPlayer == null)
            {
                continue;
            }

            var voteData = voteAreaPlayer.GetVoteData();
            var votes = voteData.Votes.RemoveAll(x => x.Suspect == target.PlayerId);
            voteData.VotesRemaining += votes;

            if (!voteAreaPlayer.AmOwner)
            {
                continue;
            }

            instance.ClearVote();
        }

        instance.SetDirtyBit(1U);

        if (AmongUsClient.Instance.AmHost)
        {
            instance.CheckForEndVoting();
        }
    }

    [RegisterEvent]
    public static void VotingCompleteHandler(VotingCompleteEvent @event)
    {
        if (Minigame.Instance)
        {
            Minigame.Instance.Close();
            Minigame.Instance.Close();
        }
    }
}