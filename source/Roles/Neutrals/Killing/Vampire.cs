namespace TownOfSushi.Roles
{
    public class Vampire : Role
    {
        public Vampire(PlayerControl player) : base(player)
        {
            Name = "Vampire";
            StartText = () => "Convert Crewmates and kill the rest";
            TaskText = () => "Convert players and kill everyone to win";
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
                && CustomGameOptions.CanBiteNeutralBenign) || (role.ClosestPlayer.Is(RoleAlignment.NeutralEvil)
                && CustomGameOptions.CanBiteNeutralEvil)) &&
                aliveVamps.Count == 1 && vamps.Count < CustomGameOptions.MaxVampiresPerGame && !ShowRoundOneShield.FirstRoundShielded && PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && !x.Data.Disconnected).ToList().Count > 5)
            {
                var interact = Interact(PlayerControl.LocalPlayer, role.ClosestPlayer);
                if (interact[3] == true)
                {
                    VampirePerformConvert.Convert(role.ClosestPlayer);
                    StartRPC(CustomRPC.Bite, role.ClosestPlayer.PlayerId);
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
    }
    
    public class VampirePerformConvert
    {
        public static void Convert(PlayerControl target)
        {
            var role = GetPlayerRole(target);
            var killsList = (role.Kills, role.CorrectKills,  role.CorrectDeputyShot, role.CorrectShot, role.IncorrectShots, role.CorrectVigilanteShot, role.CorrectAssassinKills, role.IncorrectAssassinKills);

            if (target == StartImitate.ImitatingPlayer) StartImitate.ImitatingPlayer = null;

            if (target.Is(RoleEnum.GuardianAngel))
            {
                var ga = GetRole<GuardianAngel>(target);
                ga.UnProtect();
            }

            if (target.Is(RoleEnum.Medium))
            {
                var medRole = GetRole<Medium>(target);
                medRole.MediatedPlayers.Values.DestroyAll();
                medRole.MediatedPlayers.Clear();
            }

            if (PlayerControl.LocalPlayer == target)
            {
                if (PlayerControl.LocalPlayer.Is(RoleEnum.Investigator)) GetRole<Investigator>(PlayerControl.LocalPlayer).ExamineButton.SetTarget(null);
                else if (PlayerControl.LocalPlayer.Is(RoleEnum.Hunter)) GetRole<Hunter>(PlayerControl.LocalPlayer).StalkButton.SetTarget(null);
                else if (PlayerControl.LocalPlayer.Is(RoleEnum.Vulture)) VultureKillButtonTarget.SetTarget(HudManager.Instance.KillButton, null, GetRole<Vulture>(PlayerControl.LocalPlayer));

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Investigator)) Footprint.DestroyAll(GetRole<Investigator>(PlayerControl.LocalPlayer));
                if (PlayerControl.LocalPlayer.Is(RoleEnum.Crusader)) DestroyableSingleton<HudManager>.Instance.KillButton.gameObject.SetActive(false);

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Vigilante)) 
                {
                    HudManager.Instance.KillButton.buttonLabelText.gameObject.SetActive(false);
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Hunter))
                {
                    var hunterRole = GetRole<Hunter>(PlayerControl.LocalPlayer);
                    Object.Destroy(hunterRole.UsesText);
                    hunterRole.StalkButton.SetTarget(null);
                    hunterRole.StalkButton.gameObject.SetActive(false);
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

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Investigator))
                {
                    var InvestigatorRole = GetRole<Investigator>(PlayerControl.LocalPlayer);
                    InvestigatorRole.ExamineButton.gameObject.SetActive(false);
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.GuardianAngel))
                {
                    var gaRole = GetRole<GuardianAngel>(PlayerControl.LocalPlayer);
                    Object.Destroy(gaRole.UsesText);
                }
            }

            RoleDictionary.Remove(target.PlayerId);
            if (PlayerControl.LocalPlayer == target)
            {
                var role2 = new Vampire(PlayerControl.LocalPlayer);
                role2.Kills = killsList.Kills;
                role2.CorrectVigilanteShot = killsList.CorrectVigilanteShot;
                role2.CorrectKills = killsList.CorrectKills;
                role2.IncorrectShots = killsList.IncorrectShots;
                role2.CorrectShot = killsList.CorrectShot;
                role2.CorrectDeputyShot = killsList.CorrectDeputyShot;
                role2.CorrectAssassinKills = killsList.CorrectAssassinKills;
                role2.IncorrectAssassinKills = killsList.IncorrectAssassinKills;
                role2.ReDoTaskText();
                Utilities.UsefulMethods.ShowTextToast("You are now a Vampire!", 3.5f);
                SoundManager.Instance.PlaySound(ShipStatus.Instance.SabotageSound, false, 1f, null);
                Flash(Colors.Vampire);
            }
            else
            {
                var role3 = new Vampire(target);
                role3.Kills = killsList.Kills;
                role3.CorrectVigilanteShot = killsList.CorrectVigilanteShot;
                role3.CorrectKills = killsList.CorrectKills;
                role3.IncorrectShots = killsList.IncorrectShots;
                role3.CorrectShot = killsList.CorrectShot;
                role3.CorrectDeputyShot = killsList.CorrectDeputyShot;
                role3.CorrectAssassinKills = killsList.CorrectAssassinKills;
                role3.IncorrectAssassinKills = killsList.IncorrectAssassinKills;
            }

            if (CustomGameOptions.NewVampCanAssassin && !AbilityDictionary.ContainsKey(target.PlayerId)) 
            {
                _ = new Assassin(target);
            }
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