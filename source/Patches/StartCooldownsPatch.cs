namespace TownOfSushi.Patches
{
    [HarmonyPatch(typeof(IntroCutscene._CoBegin_d__35), nameof(IntroCutscene._CoBegin_d__35.MoveNext))]
    public static class StartCooldownsPatch
    {
        public static Sprite Sprite => TownOfSushi.Arrow;
        public static void Postfix(IntroCutscene._CoBegin_d__35 __instance)
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.Is(ModifierEnum.Mini) && player.transform.localPosition.y > 4 && MiraHQMap())
                {
                    player.transform.localPosition = new Vector3(player.transform.localPosition.x, 4f, player.transform.localPosition.z);
                }
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Investigator))
            {
                var detective = GetRole<Investigator>(PlayerControl.LocalPlayer);
                detective.LastExamined = DateTime.UtcNow;
                detective.LastExamined = detective.LastExamined.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ExamineCd);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Mystic))
            {
                var mystic = GetRole<Mystic>(PlayerControl.LocalPlayer);
                mystic.LastExamined = DateTime.UtcNow;
                mystic.LastExamined = mystic.LastExamined.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.MysticExamineCd);
                mystic.LastExaminedPlayer = null;
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Vulture))
            {
                var vult = GetRole<Vulture>(PlayerControl.LocalPlayer);
                vult.LastEaten = DateTime.UtcNow;
                vult.LastEaten = vult.LastEaten.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.VultureCd);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Medium))
            {
                var medium = GetRole<Medium>(PlayerControl.LocalPlayer);
                medium.LastMediated = DateTime.UtcNow;
                medium.LastMediated = medium.LastMediated.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.MediateCooldown);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Detective))
            {
                var detective = GetRole<Detective>(PlayerControl.LocalPlayer);
                detective.LastInvestigated = DateTime.UtcNow;
                detective.LastInvestigated = detective.LastInvestigated.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.DetectiveCd);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Seer))
            {
                var Seer = GetRole<Seer>(PlayerControl.LocalPlayer);
                Seer.LastInvestigated = DateTime.UtcNow;
                Seer.LastInvestigated = Seer.LastInvestigated.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.SeerCd);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Oracle))
            {
                var oracle = GetRole<Oracle>(PlayerControl.LocalPlayer);
                oracle.LastConfessed = DateTime.UtcNow;
                oracle.LastConfessed = oracle.LastConfessed.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ConfessCd);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Vigilante))
            {
                var vigilante = GetRole<Vigilante>(PlayerControl.LocalPlayer);
                vigilante.LastKilled = DateTime.UtcNow;
                vigilante.LastKilled = vigilante.LastKilled.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.VigilanteKillCd);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Tracker))
            {
                var tracker = GetRole<Tracker>(PlayerControl.LocalPlayer);
                tracker.LastTracked = DateTime.UtcNow;
                tracker.LastTracked = tracker.LastTracked.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.TrackCd);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Trapper))
            {
                var trapper = GetRole<Trapper>(PlayerControl.LocalPlayer);
                trapper.LastTrapped = DateTime.UtcNow;
                trapper.LastTrapped = trapper.LastTrapped.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.TrapCooldown);
            }
            if (PlayerControl.LocalPlayer.Is(ModifierEnum.Disperser))
            {
                var trapper = GetModifier<Disperser>(PlayerControl.LocalPlayer);
                trapper.LastDispersed = DateTime.UtcNow;
                trapper.LastDispersed = trapper.LastDispersed.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.DisperseCooldown);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Veteran))
            {
                var veteran = GetRole<Veteran>(PlayerControl.LocalPlayer);
                veteran.LastAlerted = DateTime.UtcNow;
                veteran.LastAlerted = veteran.LastAlerted.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.AlertCd);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Witch))
            {
                var witch = GetRole<Witch>(PlayerControl.LocalPlayer);
                witch.LastSpelled = DateTime.UtcNow;
                witch.LastSpelled = witch.LastSpelled.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.SpellCd);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Blackmailer))
            {
                var blackmailer = GetRole<Blackmailer>(PlayerControl.LocalPlayer);
                blackmailer.LastBlackmailed = DateTime.UtcNow;
                blackmailer.LastBlackmailed = blackmailer.LastBlackmailed.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.BlackmailCd);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Escapist))
            {
                var escapist = GetRole<Escapist>(PlayerControl.LocalPlayer);
                escapist.LastEscape = DateTime.UtcNow;
                escapist.LastEscape = escapist.LastEscape.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.EscapeCd);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Grenadier))
            {
                var grenadier = GetRole<Grenadier>(PlayerControl.LocalPlayer);
                grenadier.LastFlashed = DateTime.UtcNow;
                grenadier.LastFlashed = grenadier.LastFlashed.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.GrenadeCd);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Miner))
            {
                var miner = GetRole<Miner>(PlayerControl.LocalPlayer);
                miner.LastMined = DateTime.UtcNow;
                miner.LastMined = miner.LastMined.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.MineCd);
                var vents = Object.FindObjectsOfType<Vent>();
                miner.VentSize =
                    Vector2.Scale(vents[0].GetComponent<BoxCollider2D>().size, vents[0].transform.localScale) * 0.75f;
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Hunter))
            {
                var hunter = Role.GetRole<Hunter>(PlayerControl.LocalPlayer);
                hunter.LastStalked = DateTime.UtcNow;
                hunter.LastStalked = hunter.LastStalked.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.HunterStalkCd);
                hunter.LastKilled = DateTime.UtcNow;
                hunter.LastKilled = hunter.LastKilled.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.HunterKillCd);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Morphling))
            {
                var morphling = GetRole<Morphling>(PlayerControl.LocalPlayer);
                morphling.LastMorphed = DateTime.UtcNow;
                morphling.LastMorphed = morphling.LastMorphed.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.MorphlingCd);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Swooper))
            {
                var swooper = GetRole<Swooper>(PlayerControl.LocalPlayer);
                swooper.LastSwooped = DateTime.UtcNow;
                swooper.LastSwooped = swooper.LastSwooped.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.SwoopCd);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Venerer))
            {
                var venerer = GetRole<Venerer>(PlayerControl.LocalPlayer);
                venerer.LastCamouflaged = DateTime.UtcNow;
                venerer.LastCamouflaged = venerer.LastCamouflaged.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.AbilityCd);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Undertaker))
            {
                var undertaker = GetRole<Undertaker>(PlayerControl.LocalPlayer);
                undertaker.LastDragged = DateTime.UtcNow;
                undertaker.LastDragged = undertaker.LastDragged.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.DragCd);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Hitman))
            {
                var hitman = GetRole<Hitman>(PlayerControl.LocalPlayer);
                hitman.LastKill = DateTime.UtcNow;
                hitman.LastDrag = DateTime.UtcNow;
                hitman.LastMorph = DateTime.UtcNow;
                hitman.LastKill = hitman.LastKill.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.HitmanKCd);
                hitman.LastDrag = hitman.LastDrag.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.HitmanDragCd);
                hitman.LastMorph = hitman.LastMorph.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.HitmanMorphCooldown);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Arsonist))
            {
                var arsonist = GetRole<Arsonist>(PlayerControl.LocalPlayer);
                arsonist.LastDoused = DateTime.UtcNow;
                arsonist.LastDoused = arsonist.LastDoused.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.DouseCd);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Doomsayer))
            {
                var doomsayer = GetRole<Doomsayer>(PlayerControl.LocalPlayer);
                doomsayer.LastObserved = DateTime.UtcNow;
                doomsayer.LastObserved = doomsayer.LastObserved.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ObserveCooldown);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Executioner))
            {
                var exe = GetRole<Executioner>(PlayerControl.LocalPlayer);
                if (exe.target == null)
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                        (byte)CustomRPC.ExecutionerToJester, SendOption.Reliable, -1);
                    writer.Write(exe.Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);

                    ExeTargetColor.ExecutionerChangeRole(exe.Player);
                }
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Glitch))
            {
                var glitch = GetRole< Glitch> (PlayerControl.LocalPlayer);
                glitch.LastKill = DateTime.UtcNow;
                glitch.LastHack = DateTime.UtcNow;
                glitch.LastMimic = DateTime.UtcNow;
                glitch.LastKill = glitch.LastKill.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.GlitchKillCooldown);
                glitch.LastHack = glitch.LastHack.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.HackCooldown);
                glitch.LastMimic = glitch.LastMimic.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.MimicCooldown);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.GuardianAngel))
            {
                var ga = GetRole<GuardianAngel>(PlayerControl.LocalPlayer);
                ga.LastProtected = DateTime.UtcNow;
                ga.LastProtected = ga.LastProtected.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ProtectCd);
                if (ga.target == null)
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                        (byte)CustomRPC.GuardianAngelChangeRole, SendOption.Reliable, -1);
                    writer.Write(ga.Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);

                    GATargetColor.GuardianAngelChangeRole(ga.Player);
                }
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Juggernaut))
            {
                var juggernaut = GetRole<Juggernaut>(PlayerControl.LocalPlayer);
                juggernaut.LastKill = DateTime.UtcNow;
                juggernaut.LastKill = juggernaut.LastKill.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.JuggKCd);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Plaguebearer))
            {
                var plaguebearer = GetRole<Plaguebearer>(PlayerControl.LocalPlayer);
                plaguebearer.LastInfected = DateTime.UtcNow;
                plaguebearer.LastInfected = plaguebearer.LastInfected.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.InfectCd);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Romantic))
            {
                var romantic = GetRole<Romantic>(PlayerControl.LocalPlayer);
                romantic.LastPick = DateTime.UtcNow;
                romantic.LastPick = romantic.LastPick.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.PickStartTimer);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Werewolf))
            {
                var surv = GetRole<Werewolf>(PlayerControl.LocalPlayer);
                surv.LastMauled = DateTime.UtcNow;
                surv.LastMauled = surv.LastMauled.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.MaulCooldown);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.SerialKiller))
            {
                var werewolf = GetRole<SerialKiller>(PlayerControl.LocalPlayer);
                werewolf.LastStabbed = DateTime.UtcNow;
                werewolf.LastKilled = DateTime.UtcNow;
                werewolf.LastStabbed = werewolf.LastStabbed.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.StabCd);
                werewolf.LastKilled = werewolf.LastKilled.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.StabKillCd);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Jailor))
            {
                var jailor = GetRole<Jailor>(PlayerControl.LocalPlayer);
                jailor.LastJailed = DateTime.UtcNow;
                jailor.LastJailed = jailor.LastJailed.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.JailCd);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Poisoner))
            {
                var Poisoner = GetRole<Poisoner>(PlayerControl.LocalPlayer);
                Poisoner.LastPoisoned = DateTime.UtcNow;
                Poisoner.LastPoisoned = Poisoner.LastPoisoned.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.PoisonCd);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Vampire))
            {
                var vamp = GetRole<Vampire>(PlayerControl.LocalPlayer);
                vamp.LastBit = DateTime.UtcNow;
                vamp.LastBit = vamp.LastBit.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.BiteCd);
            }

            if (PlayerControl.LocalPlayer.Is(AbilityEnum.Paranoiac))
            {
                var paranoiac = GetAbility<Paranoiac>(PlayerControl.LocalPlayer);
                var gameObj = new GameObject();
                var arrow = gameObj.AddComponent<ArrowBehaviour>();
                gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                var renderer = gameObj.AddComponent<SpriteRenderer>();
                renderer.sprite = Sprite;
                renderer.color = Colors.Paranoiac;
                arrow.image = renderer;
                gameObj.layer = 5;
                arrow.target = PlayerControl.LocalPlayer.transform.position;
                paranoiac.ParanoiacArrow.Add(arrow);
            }
        }
    }
}