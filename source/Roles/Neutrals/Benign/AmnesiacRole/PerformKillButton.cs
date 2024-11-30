using TownOfSushi.Roles.Crewmates.Investigative.SnitchRole;
using TownOfSushi.Roles.Crewmates.Investigative.TrapperMod;
using TownOfSushi.Roles.Crewmates.Support.ImitatorRole;
using TownOfSushi.Roles.Impostors.Power.BomberRole;

using TownOfSushi.Roles.Crewmates.Investigative.InvestigatorMod;

namespace TownOfSushi.Roles.Neutral.Benign.AmnesiacRole
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKillButton
    {
        public static Sprite Sprite => TownOfSushi.Arrow;
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton) return true;
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Amnesiac);
            if (!flag) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = GetRole<Amnesiac>(PlayerControl.LocalPlayer);

            var flag2 = __instance.isCoolingDown;
            if (flag2) return false;
            if (!__instance.enabled) return false;
            var maxDistance = KillDistance();
            if (role == null)
                return false;
            if (role.CurrentTarget == null)
                return false;
            if (Vector2.Distance(role.CurrentTarget.TruePosition,
                PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance) return false;
            var playerId = role.CurrentTarget.ParentId;
            var player = PlayerById(playerId);
            var abilityUsed = AbilityUsed(PlayerControl.LocalPlayer);
            if (!abilityUsed) return false;
            if ((player.IsInfected() || role.Player.IsInfected()) && !player.Is(RoleEnum.Plaguebearer))
            {
                foreach (var pb in GetRoles(RoleEnum.Plaguebearer)) ((Plaguebearer)pb).RpcSpreadInfection(player, role.Player);
            }

            if (AmongUsClient.Instance.AmHost)
            {
                Rpc(CustomRPC.Remember, PlayerControl.LocalPlayer.PlayerId, playerId, (byte)1);
                Remember(role, player);
            }
            else Rpc(CustomRPC.Remember, PlayerControl.LocalPlayer.PlayerId, playerId, (byte)0);
            return false;
        }

        public static void Remember(Amnesiac amneRole, PlayerControl other)
        {
            var role = GetRole(other);
            var amnesiac = amneRole.Player;

            var rememberImp = true;
            var rememberNeut = true;

            Role newRole;

            if (PlayerControl.LocalPlayer == amnesiac)
            {
                var amnesiacRole = GetRole<Amnesiac>(amnesiac);
                amnesiacRole.BodyArrows.Values.DestroyAll();
                amnesiacRole.BodyArrows.Clear();
                foreach (var body in amnesiacRole.CurrentTarget.bodyRenderers) body.material.SetFloat("_Outline", 0f);
            }

            switch (role)
            {
                case RoleEnum.Vigilante:
                case RoleEnum.Engineer:
                case RoleEnum.Mayor:
                case RoleEnum.Investigator:
                case RoleEnum.Medic:
                case RoleEnum.Seer:
                case RoleEnum.Snitch:
                case RoleEnum.Veteran:
                case RoleEnum.Crewmate:
                case RoleEnum.Tracker:
                case RoleEnum.Transporter:
                case RoleEnum.Medium:
                case RoleEnum.Mystic:
                case RoleEnum.Trapper:
                case RoleEnum.Imitator:
                case RoleEnum.Prosecutor:
                case RoleEnum.Oracle:
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

            if ((role == RoleEnum.Glitch || role == RoleEnum.Juggernaut || role == RoleEnum.Pestilence ||
                role == RoleEnum.SerialKiller) && PlayerControl.LocalPlayer == other)
            {
                HudManager.Instance.KillButton.buttonLabelText.gameObject.SetActive(false);
            }

            if (role == RoleEnum.Snitch) CompleteTask.Postfix(amnesiac);

            RoleDictionary.Remove(amnesiac.PlayerId);
            RoleDictionary.Remove(other.PlayerId);
            RoleDictionary.Add(amnesiac.PlayerId, newRole);

            if (!(amnesiac.Is(RoleEnum.Crewmate) || amnesiac.Is(RoleEnum.Impostor))) newRole.RegenTask();

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
                    if (role != RoleEnum.Vampire) {
                        var romantic = new Amnesiac(other);
                        romantic.RegenTask();
                    }
                    if (role == RoleEnum.Vampire) {
                        var vampire = new Vampire(other);
                        vampire.RegenTask();
                    }
                    if (role == RoleEnum.Arsonist || role == RoleEnum.Glitch || role == RoleEnum.Plaguebearer ||
                            role == RoleEnum.Pestilence || role == RoleEnum.SerialKiller || role == RoleEnum.Juggernaut
                             || role == RoleEnum.Vampire)
                    {
                        if (CustomGameOptions.AmneTurnNeutAssassin) new Assassin(amnesiac);
                        if (other.Is(AbilityEnum.Assassin)) Ability.AbilityDictionary.Remove(other.PlayerId);
                    }
                }
            }
            else if (rememberImp == true)
            {
                new Impostor(other);
                amnesiac.Data.Role.TeamType = RoleTeamTypes.Impostor;
                RoleManager.Instance.SetRole(amnesiac, RoleTypes.Impostor);
                amnesiac.SetKillTimer(GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown);
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player.Data.IsImpostor() && PlayerControl.LocalPlayer.Data.IsImpostor())
                    {
                        player.nameText().color = Colors.Impostor;
                    }
                }
                if (CustomGameOptions.AmneTurnImpAssassin) new Assassin(amnesiac);
            }

            if (role == RoleEnum.Snitch)
            {
                var snitchRole = GetRole<Snitch>(amnesiac);
                snitchRole.ImpArrows.DestroyAll();
                snitchRole.SnitchArrows.Values.DestroyAll();
                snitchRole.SnitchArrows.Clear();
                CompleteTask.Postfix(amnesiac);
                if (other.AmOwner)
                    foreach (var player in PlayerControl.AllPlayerControls)
                        player.nameText().color = Color.white;
                DestroyableSingleton<HudManager>.Instance.KillButton.gameObject.SetActive(false);
            }

            if (role == RoleEnum.Romantic)
            {
                var snitchRole = GetRole<Romantic>(amnesiac);
                snitchRole.LastPick = DateTime.UtcNow;
            }


            else if (role == RoleEnum.Werewolf)
            {
                var vigilanteRole = GetRole<Werewolf>(amnesiac);
                vigilanteRole.LastMauled = DateTime.UtcNow;
            }

            else if (role == RoleEnum.Engineer)
            {
                var engiRole = GetRole<Engineer>(amnesiac);
                engiRole.UsesLeft = CustomGameOptions.MaxFixes;
            }

            else if (role == RoleEnum.Medic)
            {
                var medicRole = GetRole<Medic>(amnesiac);
                if (amnesiac != StartImitate.ImitatingPlayer) medicRole.UsedAbility = false;
                else medicRole.UsedAbility = true;
            }

            else if (role == RoleEnum.Mayor)
            {
                var mayorRole = GetRole<Mayor>(amnesiac);
                mayorRole.Revealed = false;
                DestroyableSingleton<HudManager>.Instance.KillButton.gameObject.SetActive(false);
            }

            else if (role == RoleEnum.Prosecutor)
            {
                var prosRole = GetRole<Prosecutor>(amnesiac);
                prosRole.Prosecuted = false;
                DestroyableSingleton<HudManager>.Instance.KillButton.gameObject.SetActive(false);
            }

            else if (role == RoleEnum.Jailor)
            {
                var jailorRole = GetRole<Jailor>(amnesiac);
                jailorRole.LastJailed = DateTime.UtcNow;
                jailorRole.Jailed = null;
                jailorRole.Executes = CustomGameOptions.MaxExecutes;
                jailorRole.CanJail = true;
            }

            else if (role == RoleEnum.Vigilante)
            {
                var vigiRole = GetRole<Vigilante>(amnesiac);
                vigiRole.RemainingKills = CustomGameOptions.VigilanteKills;
                vigiRole.LastKilled = DateTime.UtcNow;
                DestroyableSingleton<HudManager>.Instance.KillButton.gameObject.SetActive(false);
            }

            else if (role == RoleEnum.Veteran)
            {
                var vetRole = GetRole<Veteran>(amnesiac);
                vetRole.UsesLeft = CustomGameOptions.MaxAlerts;
                vetRole.LastAlerted = DateTime.UtcNow;
            }

            else if (role == RoleEnum.Tracker)
            {
                var trackerRole = GetRole<Tracker>(amnesiac);
                trackerRole.TrackerArrows.Values.DestroyAll();
                trackerRole.TrackerArrows.Clear();
                trackerRole.UsesLeft = CustomGameOptions.MaxTracks;
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
                DestroyableSingleton<HudManager>.Instance.KillButton.gameObject.SetActive(false);
            }

            else if (role == RoleEnum.Transporter)
            {
                var tpRole = GetRole<Transporter>(amnesiac);
                tpRole.TransportPlayer1 = null;
                tpRole.TransportPlayer2 = null;
                tpRole.LastTransported = DateTime.UtcNow;
                tpRole.UsesLeft = CustomGameOptions.TransportMaxUses;
            }

            else if (role == RoleEnum.Medium)
            {
                var medRole = GetRole<Medium>(amnesiac);
                medRole.MediatedPlayers.Values.DestroyAll();
                medRole.MediatedPlayers.Clear();
                medRole.LastMediated = DateTime.UtcNow;
            }

            else if (role == RoleEnum.Seer)
            {
                var seerRole = GetRole<Seer>(amnesiac);
                seerRole.Investigated.RemoveRange(0, seerRole.Investigated.Count);
                seerRole.LastInvestigated = DateTime.UtcNow;
            }

            else if (role == RoleEnum.Oracle)
            {
                var oracleRole = GetRole<Oracle>(amnesiac);
                oracleRole.Confessor = null;
                oracleRole.LastConfessed = DateTime.UtcNow;
            }

            else if (role == RoleEnum.Arsonist)
            {
                var arsoRole = GetRole<Arsonist>(amnesiac);
                arsoRole.DousedPlayers.RemoveRange(0, arsoRole.DousedPlayers.Count);
                arsoRole.LastDoused = DateTime.UtcNow;
            }

            else if (role == RoleEnum.GuardianAngel)
            {
                var gaRole = GetRole<GuardianAngel>(amnesiac);
                gaRole.LastProtected = DateTime.UtcNow;
                gaRole.UsesLeft = CustomGameOptions.MaxProtects;
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
                witchRole.Spelled = null;
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
                //plagueRole.InfectedPlayers.RemoveRange(0, plagueRole.InfectedPlayers.Count);
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
                trapperRole.UsesLeft = CustomGameOptions.MaxTraps;
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
                DestroyableSingleton<HudManager>.Instance.KillButton.gameObject.SetActive(false);
            }

            var killsList = (newRole.Kills, newRole.CorrectKills, newRole.CorrectAssassinKills, newRole.IncorrectAssassinKills);
            var otherRole = GetPlayerRole(other);
            newRole.Kills = otherRole.Kills;
            newRole.CorrectKills = otherRole.CorrectKills;
            newRole.CorrectAssassinKills = otherRole.CorrectAssassinKills;
            newRole.IncorrectAssassinKills = otherRole.IncorrectAssassinKills;
            otherRole.Kills = killsList.Kills;
            otherRole.CorrectKills = killsList.CorrectKills;
            otherRole.CorrectAssassinKills = killsList.CorrectAssassinKills;
            otherRole.IncorrectAssassinKills = killsList.IncorrectAssassinKills;

            if (amnesiac.Is(Faction.Impostors))
            {
                foreach (var snitch in GetRoles(RoleEnum.Snitch))
                {
                    var snitchRole = (Snitch)snitch;
                    if (snitchRole.TasksDone && PlayerControl.LocalPlayer.Is(RoleEnum.Snitch))
                    {
                        var gameObj = new GameObject();
                        var arrow = gameObj.AddComponent<ArrowBehaviour>();
                        gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                        var renderer = gameObj.AddComponent<SpriteRenderer>();
                        renderer.sprite = Sprite;
                        arrow.image = renderer;
                        gameObj.layer = 5;
                        snitchRole.SnitchArrows.Add(amnesiac.PlayerId, arrow);
                    }
                    else if (snitchRole.Revealed && PlayerControl.LocalPlayer == amnesiac)
                    {
                        var gameObj = new GameObject();
                        var arrow = gameObj.AddComponent<ArrowBehaviour>();
                        gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                        var renderer = gameObj.AddComponent<SpriteRenderer>();
                        renderer.sprite = Sprite;
                        arrow.image = renderer;
                        gameObj.layer = 5;
                        snitchRole.ImpArrows.Add(arrow);
                    }
                }
            }
        }
    }
}
