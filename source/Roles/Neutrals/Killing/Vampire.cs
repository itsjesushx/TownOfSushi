namespace TownOfSushi.Roles
{
    public class Vampire : Role
    {
        public Vampire(PlayerControl player) : base(player)
        {
            Name = "Vampire";
            StartText = () => "Convert Crewmates and kill the rest";
            TaskText = () => "Conver players and kill everyone to win";
            RoleInfo = "The Vampire is a Neutral role with its own win condition. The Vampire can convert or kill other players by biting them. If the bitten player was a Crewmate they will turn into a Vampire (unless there are 2 Vampires alive). Else they will kill the bitten player.";
            LoreText = "As a Vampire, you walk the fine line between life and death. With your hypnotic charm, you can turn crewmates into your devoted followers, turning the tide of the game in your favor. Yet, your thirst for blood is unquenchable, and those who refuse to join you must be eliminated. Your power grows with every converted soul, and soon, you will be the one pulling the strings. Only those who embrace your dark gift will survive—everyone else must perish in the night.";
            Color = Colors.Vampire;
            LastBit = DateTime.UtcNow;
            RoleType = RoleEnum.Vampire;
            Faction = Faction.Neutral;
            AddToRoleHistory(RoleType);
            RoleAlignment = RoleAlignment.NeutralKilling;
        }
        public PlayerControl ClosestPlayer;
        public DateTime LastBit { get; set; }
        public float BiteTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastBit;
            var num = CustomGameOptions.BiteCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }
    }

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
            var aliveVamps = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(RoleEnum.Vampire) && !x.Data.IsDead && !x.Data.Disconnected).ToList();
            if ((role.ClosestPlayer.Is(Faction.Crewmates) || (role.ClosestPlayer.Is(RoleAlignment.NeutralBenign)
                && CustomGameOptions.CanBiteNeutralBenign) || !ShowRoundOneShield.FirstRoundShielded || (role.ClosestPlayer.Is(RoleAlignment.NeutralEvil)
                && CustomGameOptions.CanBiteNeutralEvil)) &&
                aliveVamps.Count == 1 && vamps.Count < CustomGameOptions.MaxVampiresPerGame)
            {
                var interact = Interact(PlayerControl.LocalPlayer, role.ClosestPlayer);
                if (interact[3] == true)
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
                else if (interact[2] == true) return false;
                return false;
            }
            else
            {
                var interact = Interact(PlayerControl.LocalPlayer, role.ClosestPlayer, true);
                if (interact[3] == true) return false;
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
                else if (interact[2] == true) return false;
                return false;
            }
        }

        public static void Convert(PlayerControl newVamp)
        {
            var oldRole = GetPlayerRole(newVamp);
            var killsList = (oldRole.CorrectKills, oldRole.IncorrectShots, oldRole.CorrectAssassinKills, oldRole.IncorrectAssassinKills);

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
                    VultureKillButtonTarget.SetTarget(HudManager.Instance.KillButton, null, GetRole<Vulture>(PlayerControl.LocalPlayer));
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
    
    [HarmonyPatch(typeof(HudManager))]
    public class HudBite
    {
        [HarmonyPatch(nameof(HudManager.Update))]
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Vampire)) return;
            var biteButton = __instance.KillButton;

            var role = GetRole<Vampire>(PlayerControl.LocalPlayer);

            biteButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            biteButton.SetCoolDown(role.BiteTimer(), CustomGameOptions.BiteCd);

            var notVampire = PlayerControl.AllPlayerControls
                .ToArray()
                .Where(x => !x.Is(RoleEnum.Vampire))
                .ToList();

            if (CamouflageUnCamouflagePatch.IsCamouflaged && CustomGameOptions.CamoCommsKillAnyone) SetTarget(ref role.ClosestPlayer, biteButton);
            else SetTarget(ref role.ClosestPlayer, biteButton, float.NaN, notVampire);
        }
    }
}