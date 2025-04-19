namespace TownOfSushi.Roles
{
    public class Seer : Role
    {
        public PlayerControl Investigated;
        public PlayerControl Investigated2;
        public Seer(PlayerControl player) : base(player)
        {
            Name = "Seer";
            StartText = () => "Investigate the faction of other players";
            TaskText = () => "Investigate factions to find the Killers";
            RoleInfo = "The Seer is able to choose two targets, upon a meeting starts, the Seer will be notified wether the targets are on the same faction or not, in the voting screen the Seer will see a green Y if they are, else they will have a red X next to their names.";
            LoreText = "Gifted with an extraordinary insight, the Seer can peer into the factions of their crewmates. Tasked with revealing the truth hidden in the shadows, they aim to bring light to deception and uncover the impostors among the crew. Beware, for the Seer's knowledge can make them a prime target for evildoers.";
            RoleAlignment = RoleAlignment.CrewInvest;
            Color = ColorManager.Seer;
            AddToRoleHistory(RoleType);
            LastInvestigated = DateTime.UtcNow;
            RoleType = RoleEnum.Seer; 
        }

        public float SeerTimer1()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastInvestigated;
            var num = CustomGameOptions.SeerCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }
        public DateTime LastInvestigated { get; set; }
        public bool HasInvested1;
        public bool HasInvested2;
        public PlayerControl ClosestPlayer;
        public KillButton _InvestigateButton;
        public KillButton InvestigateButton
        {
            get => _InvestigateButton;
            set
            {
                _InvestigateButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }
    }

    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformSeerReveal
    {
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != HUDManager().KillButton) return true;
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Seer);
            if (!flag) return true;
            var role = GetRole<Seer>(PlayerControl.LocalPlayer);
            if (!PlayerControl.LocalPlayer.CanMove || role.ClosestPlayer == null) return false;
            var flag2 = role.SeerTimer1() == 0f;
            if (!flag2) return false;
            if (!__instance.enabled) return false;
            var maxDistance = KillDistance();
            if (Vector2.Distance(role.ClosestPlayer.GetTruePosition(),
                PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance) return false;
            if (role.ClosestPlayer == null) return false;

            var interact = Interact(PlayerControl.LocalPlayer, role.ClosestPlayer, false);
            if (!role.HasInvested1)
            {
                if (interact[3] == true)
                {
                    role.Investigated = role.ClosestPlayer;                
                    role.HasInvested1 = true;
                    role.HasInvested2 = false;
                    SoundEffectsManager.Play("knockKnock");
                }
                if (interact[0] == true)            
                {                
                    role.LastInvestigated = DateTime.UtcNow;                
                    return false;            
                }           
                else if (interact[1] == true)            
                {                
                    role.LastInvestigated = DateTime.UtcNow;                
                    role.LastInvestigated = role.LastInvestigated.AddSeconds(CustomGameOptions.ProtectKCReset - CustomGameOptions.SeerCd);
                    return false;
                }
                else if (interact[2] == true) return false;
                return false;
            }

            if (!role.HasInvested2)
            {
                if (interact[3] == true)
                {                
                    role.Investigated2 = role.ClosestPlayer;                
                    role.HasInvested2 = true;
                    SoundEffectsManager.Play("knockKnock");
                }
                if (interact[0] == true)            
                {
                    role.LastInvestigated = DateTime.UtcNow;                
                    return false;            
                }
                else if (interact[1] == true)            
                {
                    role.LastInvestigated = DateTime.UtcNow;                
                    role.LastInvestigated = role.LastInvestigated.AddSeconds(CustomGameOptions.ProtectKCReset - CustomGameOptions.SeerCd);
                    return false;
                }
                else if (interact[2] == true) return false;
                return false;
            }
            return false;
        }
    }

    [HarmonyPatch(typeof(HudManager))]
    public class HudSeerInvestigate
    {
        [HarmonyPatch(nameof(HudManager.Update))]
        public static void Postfix(HudManager __instance)
        {
            UpdateInvButton(__instance);
        }

        public static void UpdateInvButton(HudManager __instance)
        {
            var investigateButton = __instance.KillButton;
            var role = GetRole<Seer>(PlayerControl.LocalPlayer);

            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Seer)) return;
            if (role.HasInvested2) return;

            investigateButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !Meeting() && !IsDead()
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            investigateButton.SetCoolDown(role.SeerTimer1(), CustomGameOptions.SeerCd);
            investigateButton.transform.localPosition = new Vector3(-2f, -0.06f, 0);

            var notInvestigated = PlayerControl.AllPlayerControls
                .ToArray()
                .Where(x => x != role.Investigated && x != role.Investigated2)
                .ToList();

            SetTarget(ref role.ClosestPlayer, investigateButton, float.NaN, notInvestigated);

            var renderer = investigateButton.graphic;

            if (role.ClosestPlayer != null)
            {
                renderer.color = Palette.EnabledColor;
                renderer.material.SetFloat("_Desat", 0f);
            }
            else
            {
                renderer.color = Palette.DisabledClear;
                renderer.material.SetFloat("_Desat", 1f);
            }
        }
    }
    
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class SeerReport
    {
        public static void Postfix(MeetingHud __instance)
        {
            var seer = GetRole<Seer>(PlayerControl.LocalPlayer);
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Seer)) return;
            if (IsDead()) return;
            if (seer.Investigated.Data.IsDead || seer.Investigated.Data.Disconnected 
            || seer.Investigated2.Data.Disconnected || seer.Investigated2.Data.IsDead) return;

            var playerResults = SeerResults();
            if (seer.Investigated != null && seer.Investigated2 != null)
            {
                if (!string.IsNullOrWhiteSpace(playerResults)) Chat().AddChat(PlayerControl.LocalPlayer, playerResults);
                Sound().PlaySound(Ship().SabotageSound, false, 1f, null);            
                Flash(seer.Color);
            }
        }
        public static string SeerResults()
        {
            //theres probably a better way for this but this works for now
            var seer = GetRole<Seer>(PlayerControl.LocalPlayer);
            var differentFaction = false;

            if ((seer.Investigated.Is(Faction.Crewmates) && !seer.Investigated2.Is(Faction.Crewmates)) ||
                (seer.Investigated.Is(Faction.Neutral) && !seer.Investigated2.Is(Faction.Neutral)) ||
                (seer.Investigated.Is(Faction.Impostors) && !seer.Investigated2.Is(Faction.Impostors)) ||
                (seer.Investigated2.Is(Faction.Impostors) && !seer.Investigated.Is(Faction.Impostors)) ||
                (seer.Investigated2.Is(Faction.Crewmates) && !seer.Investigated.Is(Faction.Crewmates)) ||
                (seer.Investigated2.Is(Faction.Neutral) && !seer.Investigated.Is(Faction.Neutral)))
            {
                differentFaction = true;
            }

            if (differentFaction == true) return ColorString(Color.red, $"{seer.Investigated.GetDefaultOutfit().PlayerName} and {seer.Investigated2.GetDefaultOutfit().PlayerName} have a different faction!");
            else return ColorString(Color.green, $"{seer.Investigated.GetDefaultOutfit().PlayerName} and {seer.Investigated2.GetDefaultOutfit().PlayerName} are the same faction!");
        }
    }

    public class SeerMeetingUpdate
    {
        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
        public class MeetingHud_Update
        {
            public static void Postfix(MeetingHud __instance)
            {
                    foreach (var role in GetRoles(RoleEnum.Seer))
                    {
                        var seer = (Seer)role;

                        if (seer.Player.Data.IsDead) return;
                        if (seer.Investigated == null || seer.Investigated2 == null) return;
                        if (seer.Investigated.Data.IsDead || seer.Investigated2.Data.IsDead) return;

                        var differentFaction = false;
                        if ((seer.Investigated.Is(Faction.Crewmates) && !seer.Investigated2.Is(Faction.Crewmates)) || 
                        (seer.Investigated.Is(Faction.Neutral) && !seer.Investigated2.Is(Faction.Neutral)) ||
                        (seer.Investigated.Is(Faction.Impostors) && !seer.Investigated2.Is(Faction.Impostors)) ||
                        (seer.Investigated2.Is(Faction.Impostors) && !seer.Investigated.Is(Faction.Impostors)) ||
                        (seer.Investigated2.Is(Faction.Crewmates) && !seer.Investigated.Is(Faction.Crewmates)) ||
                        (seer.Investigated2.Is(Faction.Neutral) && !seer.Investigated.Is(Faction.Neutral)))            
                        {                
                            differentFaction = true;            
                        }
                        
                        if (seer.Player.Data.IsDead  || seer.Investigated.Data.IsDead  || seer.Investigated2.Data.IsDead) return;
                        if (seer.Investigated == null || seer.Investigated2 == null) return;
                        if (differentFaction == true)
                        {
                            var playerState = __instance.playerStates.FirstOrDefault(x => x.TargetPlayerId == seer.Investigated.PlayerId);
                            var playerState2 = __instance.playerStates.FirstOrDefault(x => x.TargetPlayerId == seer.Investigated2.PlayerId);
                            if (playerState != null && playerState2 != null)
                            {
                                playerState.NameText.text += " <color=#FF0000FF> [X]</color>";
                                playerState2.NameText.text += " <color=#FF0000FF> [X]</color>";
                            }
                        }
                        else 
                        {
                            var playerState = __instance.playerStates.FirstOrDefault(x => x.TargetPlayerId == seer.Investigated.PlayerId);
                            var playerState2 = __instance.playerStates.FirstOrDefault(x => x.TargetPlayerId == seer.Investigated2.PlayerId);
                            if (playerState != null && playerState2 != null)
                            {
                                playerState.NameText.text += " <color=#00FF0D> [Y]</color>";
                                playerState2.NameText.text += " <color=#00FF0D> [Y]</color>";
                            }
                        }
                    }
            }
        }
    }
}