using TownOfSushi.Roles.Crewmates.Investigative.TrapperMod;
using TownOfSushi.Roles.Crewmates.Support.ImitatorRole;
using TownOfSushi.Roles.Crewmates.Investigative.InvestigatorMod;

namespace TownOfSushi.NeutralRoles.VampireRole
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class Bite
    {
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton) return true;
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Vampire);
            if (!flag) return true;
            var role = GetRole<Vampire>(PlayerControl.LocalPlayer);
            if (!PlayerControl.LocalPlayer.CanMove || role.ClosestPlayer == null) return false;
            var flag2 = role.BiteTimer() == 0f;
            if (!flag2) return false;
            if (!__instance.enabled) return false;
            var maxDistance = GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
            if (Vector2.Distance(role.ClosestPlayer.GetTruePosition(),
                PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance) return false;
            if (role.ClosestPlayer == null) return false;

            var vamps = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(RoleEnum.Vampire)).ToList();
            foreach (var phantom in GetRoles(RoleEnum.Phantom))
            {
                var phantomRole = (Phantom)phantom;
                if (phantomRole.formerRole == RoleEnum.Vampire) vamps.Add(phantomRole.Player);
            }
            var aliveVamps = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(RoleEnum.Vampire) && !x.Data.IsDead && !x.Data.Disconnected).ToList();
            if ((role.ClosestPlayer.Is(Faction.Crewmates) || (role.ClosestPlayer.Is(RoleAlignment.NeutralBenign)
                && CustomGameOptions.CanBiteNeutralBenign) || !ShowRoundOneShield.FirstRoundShielded || (role.ClosestPlayer.Is(RoleAlignment.NeutralEvil)
                && CustomGameOptions.CanBiteNeutralEvil)) &&
                aliveVamps.Count == 1 && vamps.Count < CustomGameOptions.MaxVampiresPerGame)
            {
                var interact = Interact(PlayerControl.LocalPlayer, role.ClosestPlayer);
                if (interact[4] == true)
                {
                    Convert(role.ClosestPlayer);
                    Rpc(CustomRPC.Bite, role.ClosestPlayer.PlayerId);
                }
                if (interact[0] == true)
                {
                    role.LastBit = DateTime.UtcNow;
                    return false;
                }
                else if (interact[1] == true)
                {
                    role.LastBit = DateTime.UtcNow;
                    role.LastBit = role.LastBit.AddSeconds(CustomGameOptions.ProtectKCReset - CustomGameOptions.BiteCd);
                    return false;
                }
                else if (interact[3] == true) return false;
                return false;
            }
            else
            {
                var interact = Interact(PlayerControl.LocalPlayer, role.ClosestPlayer, true);
                if (interact[4] == true) return false;
                if (interact[0] == true)
                {
                    role.LastBit = DateTime.UtcNow;
                    return false;
                }
                else if (interact[1] == true)
                {
                    role.LastBit = DateTime.UtcNow;
                    role.LastBit = role.LastBit.AddSeconds(CustomGameOptions.ProtectKCReset - CustomGameOptions.BiteCd);
                    return false;
                }
                else if (interact[3] == true) return false;
                return false;
            }
        }

        public static void Convert(PlayerControl newVamp)
        {
            var oldRole = GetPlayerRole(newVamp);
            var killsList = (oldRole.CorrectKills, oldRole.IncorrectShots, oldRole.CorrectAssassinKills, oldRole.IncorrectAssassinKills);

            if (newVamp.Is(RoleEnum.Snitch))
            {
                var snitch = GetRole<Snitch>(newVamp);
                snitch.SnitchArrows.Values.DestroyAll();
                snitch.SnitchArrows.Clear();
                snitch.ImpArrows.DestroyAll();
                snitch.ImpArrows.Clear();
            }

            if (newVamp == StartImitate.ImitatingPlayer) StartImitate.ImitatingPlayer = null;

            if (newVamp.Is(RoleEnum.GuardianAngel))
            {
                var ga = GetRole<GuardianAngel>(newVamp);
                ga.UnProtect();
            }

            if (newVamp.Is(RoleEnum.Medium))
            {
                var medRole = GetRole<Medium>(newVamp);
                medRole.MediatedPlayers.Values.DestroyAll();
                medRole.MediatedPlayers.Clear();
            }

            if (PlayerControl.LocalPlayer == newVamp)
            {
                if (PlayerControl.LocalPlayer.Is(RoleEnum.Investigator))
                {
                    var InvestigatorRole = GetRole<Investigator>(PlayerControl.LocalPlayer);
                    GetRole<Investigator>(PlayerControl.LocalPlayer).ExamineButton.SetTarget(null); 
                    Footprint.DestroyAll(GetRole<Investigator>(PlayerControl.LocalPlayer));
                    InvestigatorRole.ExamineButton.gameObject.SetActive(false);
                }
                else if (PlayerControl.LocalPlayer.Is(RoleEnum.Amnesiac)) 
                {
                    Roles.Neutral.Benign.AmnesiacRole.KillButtonTarget.SetTarget(HudManager.Instance.KillButton, null, GetRole<Amnesiac>(PlayerControl.LocalPlayer));
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Vigilante)) 
                {
                    HudManager.Instance.KillButton.buttonLabelText.gameObject.SetActive(false);
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Engineer))
                {
                    var engineerRole = GetRole<Engineer>(PlayerControl.LocalPlayer);
                    Object.Destroy(engineerRole.UsesText);
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Tracker))
                {
                    var trackerRole = GetRole<Tracker>(PlayerControl.LocalPlayer);
                    trackerRole.TrackerArrows.Values.DestroyAll();
                    trackerRole.TrackerArrows.Clear();
                    Object.Destroy(trackerRole.UsesText);
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Mystic))
                {
                    var mysticRole = GetRole<Mystic>(PlayerControl.LocalPlayer);
                    mysticRole.BodyArrows.Values.DestroyAll();
                    mysticRole.BodyArrows.Clear();
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Vulture))
                {
                    var Vulture = GetRole<Vulture>(PlayerControl.LocalPlayer);
                    Vulture.BodyArrows.Values.DestroyAll();
                    Vulture.BodyArrows.Clear();
                    Roles.Neutral.Evil.VultureRole.KillButtonTarget.SetTarget(HudManager.Instance.KillButton, null, GetRole<Vulture>(PlayerControl.LocalPlayer));
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Transporter))
                {
                    var transporterRole = GetRole<Transporter>(PlayerControl.LocalPlayer);
                    Object.Destroy(transporterRole.UsesText);
                    try
                    {
                        PlayerMenu.singleton.Menu.Close();
                    }
                    catch { }
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Veteran))
                {
                    var veteranRole = GetRole<Veteran>(PlayerControl.LocalPlayer);
                    Object.Destroy(veteranRole.UsesText);
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Trapper))
                {
                    var trapperRole = GetRole<Trapper>(PlayerControl.LocalPlayer);
                    Object.Destroy(trapperRole.UsesText);
                    trapperRole.traps.ClearTraps();
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.GuardianAngel))
                {
                    var gaRole = GetRole<GuardianAngel>(PlayerControl.LocalPlayer);
                    Object.Destroy(gaRole.UsesText);
                }
            }

            RoleDictionary.Remove(newVamp.PlayerId);

            if (PlayerControl.LocalPlayer == newVamp)
            {
                var role = new Vampire(PlayerControl.LocalPlayer);
                role.CorrectKills = killsList.CorrectKills;
                role.IncorrectShots = killsList.IncorrectShots;
                role.CorrectAssassinKills = killsList.CorrectAssassinKills;
                role.IncorrectAssassinKills = killsList.IncorrectAssassinKills;
                role.RegenTask();
            }
            else
            {
                var role = new Vampire(newVamp);
                role.CorrectKills = killsList.CorrectKills;
                role.IncorrectShots = killsList.IncorrectShots;
                role.CorrectAssassinKills = killsList.CorrectAssassinKills;
                role.IncorrectAssassinKills = killsList.IncorrectAssassinKills;
            }

            if (CustomGameOptions.NewVampCanAssassin) new Assassin(newVamp);
        }
    }
}