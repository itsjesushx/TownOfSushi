using UnityEngine.UI;

namespace TownOfSushi.Roles
{
    public class Amnesiac : Role
    {
        public readonly List<GameObject> Buttons = new List<GameObject>();
        public readonly List<bool> ListOfActives = new List<bool>();
        public bool SpawnedAs = true;
        public bool Remembered = false;
        public PlayerControl ToRemember = null;
        public List<RoleEnum> RolesToRemember = new List<RoleEnum>
        {
            RoleEnum.Investigator, RoleEnum.Mystic, RoleEnum.Detective, RoleEnum.Tracker, RoleEnum.Vigilante, RoleEnum.Veteran,
            RoleEnum.Engineer, RoleEnum.Medium, RoleEnum.Trapper, RoleEnum.Medic, RoleEnum.Vulture, RoleEnum.Oracle,
            RoleEnum.Hunter, RoleEnum.Jester, RoleEnum.Executioner, RoleEnum.Witch, RoleEnum.Warlock, RoleEnum.Jailor,
            RoleEnum.Agent, RoleEnum.Hitman, RoleEnum.Miner, RoleEnum.Morphling, RoleEnum.Glitch, RoleEnum.Blackmailer, RoleEnum.Juggernaut,
            RoleEnum.Swapper, RoleEnum.Amnesiac, RoleEnum.GuardianAngel, RoleEnum.Undertaker, RoleEnum.Werewolf, RoleEnum.SerialKiller, RoleEnum.Arsonist,
            RoleEnum.Grenadier, RoleEnum.Crewmate, RoleEnum.Impostor, RoleEnum.Vampire, RoleEnum.Bomber, RoleEnum.Plaguebearer, RoleEnum.Pestilence, RoleEnum.Romantic, RoleEnum.Swooper,
            RoleEnum.Venerer, RoleEnum.Janitor, RoleEnum.Poisoner, RoleEnum.BountyHunter, RoleEnum.Lookout, RoleEnum.Crusader, RoleEnum.Deputy, RoleEnum.Escapist, RoleEnum.Doomsayer, RoleEnum.Seer
        };
        public Amnesiac(PlayerControl player) : base(player)
        {
            Name = "Amnesiac";
            StartText = () => "Remember a role of a deceased player";
            TaskText = () => SpawnedAs ? "Wait for a meeting to remember a role" : "Your target died. Now remember a new role";
            RoleInfo = "The Amnesiac is a Neutral role that can remember the role of a deceased player. During meetings, the Amnesiac will have a list of players who have died and can remember the role of one of them. The Amnesiac can remember any role. If the Amnesiac remembers a role, they will become that role and will have the same abilities as that role. The Amnesiac can only remember one role. If the Amnesiac remembers a role, they will no longer be able to remember a role. If the Amnesiac does not remember a role, they will still be able to remember a role in the next meeting.";
            LoreText = "A lost soul, you are haunted by the roles of those who have passed. As the Amnesiac, you have the unique ability to remember the role of a deceased player, adopting their powers and abilities. With each life lost, you gain the chance to assume a new identity, allowing you to shift allegiances and goals in a bid to survive and thrive in the chaos.";
            Color = ColorManager.Amnesiac;
            RoleType = RoleEnum.Amnesiac;
            Faction = Faction.Neutral;
            AddToRoleHistory(RoleType);
            RoleAlignment = RoleAlignment.NeutralBenign;
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class AddButtonAmnesiac
    {
        private static int _mostRecentId;
        private static Sprite ActiveSprite => TownOfSushi.ImitateSelectSprite;
        public static Sprite DisabledSprite => TownOfSushi.ImitateDeselectSprite;
        private static List<GameObject> buttonPool = new List<GameObject>();
        public static void GenButton(Amnesiac role, int index, bool isDead)
        {
            if (PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && !x.Data.Disconnected).ToList().Count <= 4) return;
            if (role.Remembered) return;
            if (!isDead)
            {
                role.Buttons.Add(null);
                role.ListOfActives.Add(false);
                return;
            }

            var confirmButton = Meeting().playerStates[index].Buttons.transform.GetChild(0).gameObject;

            GameObject newButton;
            if (buttonPool.Count > 0)
            {
                newButton = buttonPool[0];
                buttonPool.RemoveAt(0);
                newButton.SetActive(true);
            }
            else
            {
                newButton = Object.Instantiate(confirmButton, Meeting().playerStates[index].transform);
            }

            var renderer = newButton.GetComponent<SpriteRenderer>();
            var passive = newButton.GetComponent<PassiveButton>();

            renderer.sprite = DisabledSprite;
            newButton.transform.position = confirmButton.transform.position - new Vector3(0.75f, 0f, 0f);
            newButton.transform.localScale *= 0.8f;
            newButton.layer = 5;
            newButton.transform.parent = confirmButton.transform.parent.parent;
            passive.OnClick = new Button.ButtonClickedEvent();
            passive.OnClick.AddListener(SetActive(role, index));
            role.Buttons.Add(newButton);
            role.ListOfActives.Add(false);
        }

        public static void ReturnButtonToPool(GameObject button)
        {
            button.SetActive(false);
            buttonPool.Add(button);
        }

        private static Action SetActive(Amnesiac role, int index)
        {
            void Listener()
            {
                if (role.ListOfActives.Count(x => x) == 1 &&
                    role.Buttons[index].GetComponent<SpriteRenderer>().sprite == DisabledSprite)
                {
                    int active = 0;
                    for (var i = 0; i < role.ListOfActives.Count; i++) if (role.ListOfActives[i]) active = i;

                    role.Buttons[active].GetComponent<SpriteRenderer>().sprite =
                        role.ListOfActives[active] ? DisabledSprite : ActiveSprite;

                    role.ListOfActives[active] = !role.ListOfActives[active];
                }

                role.Buttons[index].GetComponent<SpriteRenderer>().sprite =
                    role.ListOfActives[index] ? DisabledSprite : ActiveSprite;

                role.ListOfActives[index] = !role.ListOfActives[index];

                _mostRecentId = index;

                SetRemember.Remember = null;
                for (var i = 0; i < role.ListOfActives.Count; i++)
                {
                    if (!role.ListOfActives[i]) continue;
                    SetRemember.Remember = Meeting().playerStates[i];
                }
            }

            return Listener;
        }

        public static void Postfix(MeetingHud __instance)
        {
            foreach (var role in GetRoles(RoleEnum.Amnesiac))
            {
                var Amnesiac = (Amnesiac)role;
                Amnesiac.ListOfActives.Clear();
                Amnesiac.Buttons.Clear();
            }

            if (IsDead()) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Amnesiac)) return;
            if (PlayerControl.LocalPlayer.IsJailed()) return;
            var AmnesiacRole = GetRole<Amnesiac>(PlayerControl.LocalPlayer);
            for (var i = 0; i < __instance.playerStates.Length; i++)
            {
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player.PlayerId == __instance.playerStates[i].TargetPlayerId)
                    {
                        var stealable = false;
                        var StolenRole = GetPlayerRole(player).RoleType;
                        if (player.Data.IsDead && !player.Data.Disconnected && AmnesiacRole.RolesToRemember.Contains(StolenRole)) stealable = true;
                        GenButton(AmnesiacRole, i, stealable);
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(MeetingHud))]
    public class SetRemember
    {
        public static PlayerVoteArea Remember;

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.VotingComplete))]
        public static class VotingComplete
        {
            public static void Postfix(MeetingHud __instance)
            {
                if (Remember == null) return;

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Amnesiac))
                {
                    var Amnesiac = GetRole<Amnesiac>(PlayerControl.LocalPlayer);
                    foreach (var button in Amnesiac.Buttons.Where(button => button != null)) button.SetActive(false);

                    foreach (var player in PlayerControl.AllPlayerControls)
                    {
                        if (player.PlayerId == Remember.TargetPlayerId) 
                        { 
                            Amnesiac.ToRemember = player;
                        }
                        if (Remember == null)
                        {
                            StartRPC(CustomRPC.Remember, Amnesiac.Player.PlayerId, sbyte.MaxValue);
                            return;
                        }

                        StartRPC(CustomRPC.Remember, Amnesiac.Player.PlayerId, player.PlayerId);
                    }
                }
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
        public static class MeetingHud_Start
        {
            public static void Postfix(MeetingHud __instance)
            {
                Remember = null;
            }
        }
    }

    public class ShowHideButtonsAmnesiac
    {
        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Confirm))]
        public static class Confirm
        {
            public static bool Prefix(MeetingHud __instance)
            {
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Amnesiac)) return true;
                var Amnesiac = GetRole<Amnesiac>(PlayerControl.LocalPlayer);
                foreach (var button in Amnesiac.Buttons.Where(button => button != null))
                {
                    if (button.GetComponent<SpriteRenderer>().sprite == AddButtonAmnesiac.DisabledSprite)
                        button.SetActive(false);

                    button.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
                }

                if (Amnesiac.ListOfActives.Count(x => x) == 1)
                {
                    for (var i = 0; i < Amnesiac.ListOfActives.Count; i++)
                    {
                        if (!Amnesiac.ListOfActives[i]) continue;
                        SetRemember.Remember = __instance.playerStates[i];
                    }
                }

                return true;
            }
        }
    }

    public static class RememberRole
    {
        public static void Remember(Amnesiac amneRole, PlayerControl other)
        {
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Lookout))
            {
                var lookout = GetRole<Lookout>(PlayerControl.LocalPlayer);
                if (lookout.Watching.ContainsKey(other.PlayerId))
                {
                    if (!lookout.Watching[other.PlayerId].Contains(RoleEnum.Amnesiac)) lookout.Watching[other.PlayerId].Add(RoleEnum.Amnesiac);
                }
            }

            var role = GetRole(other);
            var amnesiac = amneRole.Player;

            var rememberImp = true;
            var rememberNeut = true;

            Role newRole;
            switch (role)
            {
                case RoleEnum.Vigilante:
                case RoleEnum.Engineer:
                case RoleEnum.Investigator:
                case RoleEnum.Medic:
                case RoleEnum.Detective:
                case RoleEnum.Hunter:
                case RoleEnum.Veteran:
                case RoleEnum.Crewmate:
                case RoleEnum.Seer:
                case RoleEnum.Tracker:
                case RoleEnum.Deputy:
                case RoleEnum.Medium:
                case RoleEnum.Mystic:
                case RoleEnum.Swapper:
                case RoleEnum.Trapper:
                case RoleEnum.Imitator:
                case RoleEnum.Oracle:
                case RoleEnum.Lookout:
                case RoleEnum.Crusader:
                case RoleEnum.Jailor:

                    rememberImp = false;
                    rememberNeut = false;

                    break;

                case RoleEnum.Jester:
                case RoleEnum.Executioner:
                case RoleEnum.Arsonist:
                case RoleEnum.Amnesiac:
                case RoleEnum.Glitch:
                case RoleEnum.Juggernaut:
                case RoleEnum.Romantic:
                case RoleEnum.GuardianAngel:
                case RoleEnum.Vulture:
                case RoleEnum.Plaguebearer:
                case RoleEnum.Agent:
                case RoleEnum.Hitman:
                case RoleEnum.Werewolf:
                case RoleEnum.Pestilence:
                case RoleEnum.SerialKiller:
                case RoleEnum.Doomsayer:
                case RoleEnum.Vampire:

                    rememberImp = false;

                    break;
            }

            newRole = GetPlayerRole(other);
            newRole.Player = amnesiac;

            if ((role == RoleEnum.Glitch || role == RoleEnum.Juggernaut || role == RoleEnum.Hitman 
            || role == RoleEnum.Pestilence || role == RoleEnum.Agent || role == RoleEnum.Werewolf ||
                role == RoleEnum.SerialKiller) && PlayerControl.LocalPlayer == other)
            {
                HUDManager().KillButton.buttonLabelText.gameObject.SetActive(false);
            }

            RoleDictionary.Remove(amnesiac.PlayerId);
            RoleDictionary.Remove(other.PlayerId);
            RoleDictionary.Add(amnesiac.PlayerId, newRole);

            newRole.ReDoTaskText();

            var vowel = "aeiou".Contains(newRole.Name.ToLower()[0]);
            var article = vowel ? "an" : "a";
            
            if (PlayerControl.LocalPlayer == amnesiac)
            {
                ShowTextToast($"You remembered you were {article} {newRole.Name}!", 3.5f);
                Sound().PlaySound(Ship().SabotageSound, false, 1f, null);
                Flash(newRole.Color);
            }

            if (other == StartImitate.ImitatingPlayer)
            {
                StartImitate.ImitatingPlayer = amneRole.Player;
                newRole.AddToRoleHistory(RoleEnum.Imitator);
            }
            else newRole.AddToRoleHistory(newRole.RoleType);
            if (rememberImp == false)
            {
                if (rememberNeut == false)
                {
                    new Amnesiac(other);
                }
                else
                {
                    if (role == RoleEnum.Vampire)
                    {
                        var vampire = new Vampire(other);
                        vampire.ReDoTaskText();
                    }
                    else
                    {
                        var amnesiacRole = new Amnesiac(other);
                        amnesiacRole.ReDoTaskText();
                    }
                    if (role == RoleEnum.Plaguebearer || role == RoleEnum.Arsonist 
                       || role == RoleEnum.Glitch || role == RoleEnum.Pestilence 
                       || role == RoleEnum.Hitman || role == RoleEnum.Vampire
                       || role == RoleEnum.Agent || role == RoleEnum.SerialKiller || role == RoleEnum.Juggernaut)
                    {
                        //only add Assassin if the amnesiac does not have an ability already
                        if (CustomGameOptions.AmneTurnNeutAssassin && !AbilityDictionary.ContainsKey(amnesiac.PlayerId))
                        {
                            _ = new Assassin(amnesiac);
                        }
                    }
                }
            }
            else if (rememberImp == true)
            {
                new Impostor(other);
                amnesiac.Data.Role.TeamType = RoleTeamTypes.Impostor;
                RoleManager.Instance.SetRole(amnesiac, RoleTypes.Impostor);
                amnesiac.SetKillTimer(VanillaOptions().currentNormalGameOptions.KillCooldown);
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player.Data.IsImpostor() && PlayerControl.LocalPlayer.Data.IsImpostor())
                    {
                        player.nameText().color = ColorManager.Impostor;
                    }
                }
                //only add Assassin if the amnesiac does not have an ability already
                if (CustomGameOptions.AmneTurnImpAssassin && !AbilityDictionary.ContainsKey(amnesiac.PlayerId))
                {
                    var assassin = new Assassin(amnesiac);
                    assassin.ReDoTaskText();
                }
                if (amnesiac.Is(RoleEnum.Poisoner))
                {
                    if (PlayerControl.LocalPlayer == amnesiac)
                    {
                        var poisonerRole = GetRole<Poisoner>(amnesiac);
                        poisonerRole.LastPoisoned = DateTime.UtcNow;
                        HUDManager().KillButton.graphic.enabled = false;
                    }
                    else if (PlayerControl.LocalPlayer == other)
                    {
                        HUDManager().KillButton.enabled = true;
                        HUDManager().KillButton.graphic.enabled = true;
                    }
                }
            }

            if (role == RoleEnum.Romantic)
            {
                var romantic = GetRole<Romantic>(amnesiac);
                romantic.LastPick = DateTime.UtcNow;
                romantic.AlreadyPicked = false;
            }

            else if (role == RoleEnum.Werewolf)
            {
                var vigilanteRole = GetRole<Werewolf>(amnesiac);
                vigilanteRole.LastMauled = DateTime.UtcNow;
            }

            else if (role == RoleEnum.Engineer)
            {
                var engiRole = GetRole<Engineer>(amnesiac);
                engiRole.MaxUses = CustomGameOptions.MaxFixes;
            }

            else if (role == RoleEnum.Medic)
            {
                var medicRole = GetRole<Medic>(amnesiac);
                if (amnesiac != StartImitate.ImitatingPlayer) medicRole.UsedAbility = false;
                else medicRole.UsedAbility = false;
            }

            else if (role == RoleEnum.Jailor)
            {
                var jailorRole = GetRole<Jailor>(amnesiac);
                jailorRole.LastJailed = DateTime.UtcNow;
                jailorRole.Jailed = null;
                jailorRole.Executes = CustomGameOptions.MaxExecutes;
                jailorRole.CanJail = true;
            }

            else if (role == RoleEnum.Deputy)
            {
                var Deputy = GetRole<Deputy>(PlayerControl.LocalPlayer);
                Deputy.ExecuteButton.Destroy();
                Deputy.HasExectutedAlready = false;
                Deputy.RemainingKills = CustomGameOptions.DeputyKills;
            }

            else if (role == RoleEnum.Vigilante)
            {
                var vigiRole = GetRole<Vigilante>(amnesiac);
                vigiRole.RemainingKills = CustomGameOptions.VigilanteKills;
                vigiRole.LastKilled = DateTime.UtcNow;
                HUDManager().KillButton.gameObject.SetActive(false);
            }

            else if (role == RoleEnum.Veteran)
            {
                var vetRole = GetRole<Veteran>(amnesiac);
                vetRole.UsesLeft = CustomGameOptions.MaxAlerts;
                vetRole.LastAlerted = DateTime.UtcNow;
            }
            else if (role == RoleEnum.Hunter)
            {
                var hunterRole = GetRole<Hunter>(amnesiac);
                hunterRole.MaxUses = CustomGameOptions.HunterStalkUses;
                hunterRole.LastStalked = DateTime.UtcNow;
                hunterRole.LastKilled = DateTime.UtcNow;
            }

            else if (role == RoleEnum.Tracker)
            {
                var trackerRole = GetRole<Tracker>(amnesiac);
                trackerRole.TrackerArrows.Values.DestroyAll();
                trackerRole.TrackerArrows.Clear();
                trackerRole.MaxUses = CustomGameOptions.MaxTracks;
                trackerRole.LastTracked = DateTime.UtcNow;
            }

            else if (role == RoleEnum.Investigator)
            {
                var detectiveRole = GetRole<Investigator>(amnesiac);
                detectiveRole.LastExamined = DateTime.UtcNow;
                detectiveRole.CurrentTarget = null;
                detectiveRole.ExamineMode = false;
                Footprint.DestroyAll(GetRole<Investigator>(other));
            }

            else if (role == RoleEnum.Mystic)
            {
                var mysticRole = GetRole<Mystic>(amnesiac);
                mysticRole.BodyArrows.Values.DestroyAll();
                mysticRole.BodyArrows.Clear();
                mysticRole.LastExamined = DateTime.UtcNow;
                HUDManager().KillButton.gameObject.SetActive(false);
            }

            else if (role == RoleEnum.Medium)
            {
                var medRole = GetRole<Medium>(amnesiac);
                medRole.MediatedPlayers.Values.DestroyAll();
                medRole.MediatedPlayers.Clear();
                medRole.LastMediated = DateTime.UtcNow;
            }

            else if (role == RoleEnum.Detective)
            {
                var Detective = GetRole<Detective>(amnesiac);
                Detective.Investigated.RemoveRange(0, Detective.Investigated.Count);
                Detective.LastInvestigated = DateTime.UtcNow;
            }

            else if (role == RoleEnum.Lookout)
            {
                var loRole = GetRole<Lookout>(amnesiac);
                loRole.UsesLeft = CustomGameOptions.MaxWatches;
                loRole.Watching.Clear();
                loRole.LastWatched = DateTime.UtcNow;
            }

            else if (role == RoleEnum.Oracle)
            {
                var oracleRole = GetRole<Oracle>(amnesiac);
                oracleRole.Confessor = null;
                oracleRole.LastConfessed = DateTime.UtcNow;
            }

            else if (role == RoleEnum.Crusader)
            {
                var Crusader = GetRole<Crusader>(amnesiac);
                Crusader.StartingCooldown = Crusader.StartingCooldown.AddSeconds(-10f);
                Crusader.Fortified = null;
                HUDManager().KillButton.gameObject.SetActive(false);
            }

            else if (role == RoleEnum.Arsonist)
            {
                var arsoRole = GetRole<Arsonist>(amnesiac);
                arsoRole.DousedPlayers.RemoveRange(0, arsoRole.DousedPlayers.Count);
                arsoRole.LastDoused = DateTime.UtcNow;
            }

            else if (role == RoleEnum.Seer)
            {
                var Seer = GetRole<Seer>(amnesiac);
                Seer.Investigated = null;
                Seer.Investigated2 = null;
                Seer.LastInvestigated = DateTime.UtcNow;
            }

            else if (role == RoleEnum.GuardianAngel)
            {
                var gaRole = GetRole<GuardianAngel>(amnesiac);
                gaRole.LastProtected = DateTime.UtcNow;
                gaRole.MaxUses = CustomGameOptions.MaxProtects;
            }

            else if (role == RoleEnum.Glitch)
            {
                var glitchRole = GetRole<Glitch>(amnesiac);
                glitchRole.LastKill = DateTime.UtcNow;
                glitchRole.LastHack = DateTime.UtcNow;
                glitchRole.LastMimic = DateTime.UtcNow;
            }

            else if (role == RoleEnum.Juggernaut)
            {
                var juggRole = GetRole<Juggernaut>(amnesiac);
                juggRole.JuggKills = 0;
                juggRole.LastKill = DateTime.UtcNow;
            }

            else if (role == RoleEnum.Grenadier)
            {
                var grenadeRole = GetRole<Grenadier>(amnesiac);
                grenadeRole.LastFlashed = DateTime.UtcNow;
            }

            else if (role == RoleEnum.Morphling)
            {
                var morphlingRole = GetRole<Morphling>(amnesiac);
                morphlingRole.LastMorphed = DateTime.UtcNow;
            }

            else if (role == RoleEnum.Escapist)
            {
                var escapistRole = GetRole<Escapist>(amnesiac);
                escapistRole.LastEscape = DateTime.UtcNow;
            }

            else if (role == RoleEnum.Swooper)
            {
                var swooperRole = GetRole<Swooper>(amnesiac);
                swooperRole.LastSwooped = DateTime.UtcNow;
            }

            else if (role == RoleEnum.Venerer)
            {
                var venererRole = GetRole<Venerer>(amnesiac);
                venererRole.LastCamouflaged = DateTime.UtcNow;
                venererRole.KillsAtStartAbility = 0;
            }

            else if (role == RoleEnum.Blackmailer)
            {
                var blackmailerRole = GetRole<Blackmailer>(amnesiac);
                blackmailerRole.LastBlackmailed = DateTime.UtcNow;
                blackmailerRole.Blackmailed = null;
            }

            else if (role == RoleEnum.Witch)
            {
                var witchRole = GetRole<Witch>(amnesiac);
                witchRole.LastSpelled = DateTime.UtcNow;
                witchRole.SpelledPlayers.RemoveRange(0, witchRole.SpelledPlayers.Count);
            }

            else if (role == RoleEnum.Miner)
            {
                var minerRole = GetRole<Miner>(amnesiac);
                minerRole.LastMined = DateTime.UtcNow;
            }

            else if (role == RoleEnum.Undertaker)
            {
                var dienerRole = GetRole<Undertaker>(amnesiac);
                dienerRole.LastDragged = DateTime.UtcNow;
            }
            else if (role == RoleEnum.Hitman)
            {
                var hitmanRole = GetRole<Hitman>(amnesiac);
                hitmanRole.LastDrag = DateTime.UtcNow;
                hitmanRole.LastKill = DateTime.UtcNow;
                hitmanRole.LastMorph = DateTime.UtcNow;
            }

            else if (role == RoleEnum.SerialKiller)
            {
                var wwRole = GetRole<SerialKiller>(amnesiac);
                wwRole.LastStabbed = DateTime.UtcNow;
                wwRole.LastKilled = DateTime.UtcNow;
            }

            else if (role == RoleEnum.Doomsayer)
            {
                var doomRole = GetRole<Doomsayer>(amnesiac);
                doomRole.GuessedCorrectly = 0;
                doomRole.LastObserved = DateTime.UtcNow;
                doomRole.LastObservedPlayer = null;
            }

            else if (role == RoleEnum.Vulture)
            {
                var vultRole = GetRole<Vulture>(amnesiac);
                vultRole.BodyArrows.Values.DestroyAll();
                vultRole.BodyArrows.Clear();
                vultRole.EatNeed = CustomGameOptions.VultureBodyCount;
                vultRole.LastEaten = DateTime.UtcNow;
            }

            else if (role == RoleEnum.Plaguebearer)
            {
                var plagueRole = GetRole<Plaguebearer>(amnesiac);
                plagueRole.InfectedPlayers.RemoveRange(0, plagueRole.InfectedPlayers.Count);
                plagueRole.InfectedPlayers.Add(amnesiac.PlayerId);
                plagueRole.LastInfected = DateTime.UtcNow;
            }

            else if (role == RoleEnum.Pestilence)
            {
                var pestRole = GetRole<Pestilence>(amnesiac);
                pestRole.LastKill = DateTime.UtcNow;
            }

            else if (role == RoleEnum.Vampire)
            {
                var vampRole = GetRole<Vampire>(amnesiac);
                vampRole.LastBit = DateTime.UtcNow;
            }

            else if (role == RoleEnum.Trapper)
            {
                var trapperRole = GetRole<Trapper>(amnesiac);
                trapperRole.LastTrapped = DateTime.UtcNow;
                trapperRole.MaxUses = CustomGameOptions.MaxTraps;
                trapperRole.trappedPlayers.Clear();
                trapperRole.traps.ClearTraps();
            }

            else if (role == RoleEnum.Bomber)
            {
                var bomberRole = GetRole<Bomber>(amnesiac);
                bomberRole.Bomb.ClearBomb();
            }

            else if (!(amnesiac.Is(RoleEnum.Amnesiac) || amnesiac.Is(Faction.Impostors)))
            {
                HUDManager().KillButton.gameObject.SetActive(false);
            }

            var killsList = (newRole.Kills, newRole.CorrectKills,  newRole.CorrectDeputyShot, newRole.CorrectShot, newRole.IncorrectShots, newRole.CorrectVigilanteShot, newRole.CorrectAssassinKills);
            var otherRole = GetPlayerRole(other);
            otherRole.Kills = killsList.Kills;
            otherRole.CorrectVigilanteShot = killsList.CorrectVigilanteShot;
            otherRole.CorrectKills = killsList.CorrectKills;
            otherRole.IncorrectShots = killsList.IncorrectShots;
            otherRole.CorrectShot = killsList.CorrectShot;
            otherRole.CorrectDeputyShot = killsList.CorrectDeputyShot;
            otherRole.CorrectAssassinKills = killsList.CorrectAssassinKills;
        }
    }

    [HarmonyPatch(typeof(AirshipExileController), nameof(AirshipExileController.WrapUpAndSpawn))]
    public static class AmnesiacAirshipExileController_WrapUpAndSpawn
    {
        public static void Postfix(AirshipExileController __instance) => StartRemember.AmnesiacExileControllerPostfix(__instance);
    }
    
    [HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
    public static class StartRemember
    {
        public static void AmnesiacExileControllerPostfix(ExileController __instance)
        {
            var exiled = __instance.initData.networkedPlayer?.Object;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Amnesiac)) return;
            if (IsDead() || PlayerControl.LocalPlayer.Data.Disconnected) return;
            if (exiled == PlayerControl.LocalPlayer) return;
            var amnesiac = GetRole<Amnesiac>(PlayerControl.LocalPlayer);
            if (amnesiac.ToRemember == null) return;

            StartRPC(CustomRPC.StartRemember, PlayerControl.LocalPlayer.PlayerId, amnesiac.ToRemember.PlayerId);
            RememberRole.Remember(amnesiac, amnesiac.ToRemember);
            amnesiac.Remembered = true;
        }

        public static void Postfix(ExileController __instance)
        {    
            if (__instance == null) return;    
            AmnesiacExileControllerPostfix(__instance);
        }

        [HarmonyPatch(typeof(Object), nameof(Object.Destroy), new Type[] { typeof(GameObject) })]
        public static void Prefix(GameObject obj)
        {    
            if (!SubmergedLoaded || VanillaOptions()?.currentNormalGameOptions?.MapId != 6) return;
            if (obj.name?.Contains("ExileCutscene") == true && ExileControllerPatch.lastExiled != null)    
            {
                AmnesiacExileControllerPostfix(ExileControllerPatch.lastExiled);    
            }
        }
    }
}