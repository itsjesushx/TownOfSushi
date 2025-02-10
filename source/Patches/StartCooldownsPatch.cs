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
                if (player.Is(ModifierEnum.Mini) && player.transform.localPosition.y > 4 && IsMiraHQMap())
                {
                    player.transform.localPosition = new Vector3(player.transform.localPosition.x, 4f, player.transform.localPosition.z);
                }
            }

            if (LocalPlayer().Is(RoleEnum.Investigator))
            {
                var detective = GetRole<Investigator>(LocalPlayer());
                detective.LastExamined = DateTime.UtcNow;
                detective.LastExamined = detective.LastExamined.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ExamineCd);
            }

            if (LocalPlayer().Is(RoleEnum.Lookout))
            {
                var lo = GetRole<Lookout>(LocalPlayer());
                lo.LastWatched = DateTime.UtcNow;
                lo.LastWatched = lo.LastWatched.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.WatchCooldown);
            }

            if (LocalPlayer().Is(RoleEnum.Mystic))
            {
                var mystic = GetRole<Mystic>(LocalPlayer());
                mystic.LastExamined = DateTime.UtcNow;
                mystic.LastExamined = mystic.LastExamined.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.MysticExamineCd);
                mystic.LastExaminedPlayer = null;
            }

            if (LocalPlayer().Is(RoleEnum.Vulture))
            {
                var vult = GetRole<Vulture>(LocalPlayer());
                vult.LastEaten = DateTime.UtcNow;
                vult.LastEaten = vult.LastEaten.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.VultureCd);
            }

            if (LocalPlayer().Is(RoleEnum.Medium))
            {
                var medium = GetRole<Medium>(LocalPlayer());
                medium.LastMediated = DateTime.UtcNow;
                medium.LastMediated = medium.LastMediated.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.MediateCooldown);
            }

            if (LocalPlayer().Is(RoleEnum.Detective))
            {
                var detective = GetRole<Detective>(LocalPlayer());
                detective.LastInvestigated = DateTime.UtcNow;
                detective.LastInvestigated = detective.LastInvestigated.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.DetectiveCd);
            }

            if (LocalPlayer().Is(RoleEnum.Seer))
            {
                var Seer = GetRole<Seer>(LocalPlayer());
                Seer.LastInvestigated = DateTime.UtcNow;
                Seer.LastInvestigated = Seer.LastInvestigated.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.SeerCd);
            }

            if (LocalPlayer().Is(RoleEnum.Oracle))
            {
                var oracle = GetRole<Oracle>(LocalPlayer());
                oracle.LastConfessed = DateTime.UtcNow;
                oracle.LastConfessed = oracle.LastConfessed.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ConfessCd);
            }

            if (LocalPlayer().Is(RoleEnum.Vigilante))
            {
                var vigilante = GetRole<Vigilante>(LocalPlayer());
                vigilante.LastKilled = DateTime.UtcNow;
                vigilante.LastKilled = vigilante.LastKilled.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.VigilanteKillCd);
            }

            if (LocalPlayer().Is(RoleEnum.Tracker))
            {
                var tracker = GetRole<Tracker>(LocalPlayer());
                tracker.LastTracked = DateTime.UtcNow;
                tracker.LastTracked = tracker.LastTracked.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.TrackCd);
            }

            if (LocalPlayer().Is(RoleEnum.Trapper))
            {
                var trapper = GetRole<Trapper>(LocalPlayer());
                trapper.LastTrapped = DateTime.UtcNow;
                trapper.LastTrapped = trapper.LastTrapped.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.TrapCooldown);
            }
            if (LocalPlayer().Is(ModifierEnum.Disperser))
            {
                var trapper = GetModifier<Disperser>(LocalPlayer());
                trapper.LastDispersed = DateTime.UtcNow;
                trapper.LastDispersed = trapper.LastDispersed.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.DisperseCooldown);
            }

            if (LocalPlayer().Is(RoleEnum.Veteran))
            {
                var veteran = GetRole<Veteran>(LocalPlayer());
                veteran.LastAlerted = DateTime.UtcNow;
                veteran.LastAlerted = veteran.LastAlerted.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.AlertCd);
            }

            if (LocalPlayer().Is(RoleEnum.Witch))
            {
                var witch = GetRole<Witch>(LocalPlayer());
                witch.LastSpelled = DateTime.UtcNow;
                witch.LastSpelled = witch.LastSpelled.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.SpellCd);
            }

            if (LocalPlayer().Is(RoleEnum.Blackmailer))
            {
                var blackmailer = GetRole<Blackmailer>(LocalPlayer());
                blackmailer.LastBlackmailed = DateTime.UtcNow;
                blackmailer.LastBlackmailed = blackmailer.LastBlackmailed.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.BlackmailCd);
            }

            if (LocalPlayer().Is(RoleEnum.Escapist))
            {
                var escapist = GetRole<Escapist>(LocalPlayer());
                escapist.LastEscape = DateTime.UtcNow;
                escapist.LastEscape = escapist.LastEscape.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.EscapeCd);
            }

            if (LocalPlayer().Is(RoleEnum.Grenadier))
            {
                var grenadier = GetRole<Grenadier>(LocalPlayer());
                grenadier.LastFlashed = DateTime.UtcNow;
                grenadier.LastFlashed = grenadier.LastFlashed.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.GrenadeCd);
            }

            if (LocalPlayer().Is(RoleEnum.Miner))
            {
                var miner = GetRole<Miner>(LocalPlayer());
                miner.LastMined = DateTime.UtcNow;
                miner.LastMined = miner.LastMined.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.MineCd);
                var vents = Object.FindObjectsOfType<Vent>();
                miner.VentSize =
                    Vector2.Scale(vents[0].GetComponent<BoxCollider2D>().size, vents[0].transform.localScale) * 0.75f;
            }

            if (LocalPlayer().Is(RoleEnum.Hunter))
            {
                var hunter = Role.GetRole<Hunter>(LocalPlayer());
                hunter.LastStalked = DateTime.UtcNow;
                hunter.LastStalked = hunter.LastStalked.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.HunterStalkCd);
                hunter.LastKilled = DateTime.UtcNow;
                hunter.LastKilled = hunter.LastKilled.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.HunterKillCd);
            }

            if (LocalPlayer().Is(RoleEnum.Morphling))
            {
                var morphling = GetRole<Morphling>(LocalPlayer());
                morphling.LastMorphed = DateTime.UtcNow;
                morphling.LastMorphed = morphling.LastMorphed.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.MorphlingCd);
            }

            if (LocalPlayer().Is(RoleEnum.Swooper))
            {
                var swooper = GetRole<Swooper>(LocalPlayer());
                swooper.LastSwooped = DateTime.UtcNow;
                swooper.LastSwooped = swooper.LastSwooped.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.SwoopCd);
            }

            if (LocalPlayer().Is(RoleEnum.Venerer))
            {
                var venerer = GetRole<Venerer>(LocalPlayer());
                venerer.LastCamouflaged = DateTime.UtcNow;
                venerer.LastCamouflaged = venerer.LastCamouflaged.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.AbilityCd);
            }

            if (LocalPlayer().Is(RoleEnum.Undertaker))
            {
                var undertaker = GetRole<Undertaker>(LocalPlayer());
                undertaker.LastDragged = DateTime.UtcNow;
                undertaker.LastDragged = undertaker.LastDragged.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.DragCd);
            }

            if (LocalPlayer().Is(RoleEnum.Hitman))
            {
                var hitman = GetRole<Hitman>(LocalPlayer());
                hitman.LastKill = DateTime.UtcNow;
                hitman.LastDrag = DateTime.UtcNow;
                hitman.LastMorph = DateTime.UtcNow;
                hitman.LastKill = hitman.LastKill.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.HitmanKCd);
                hitman.LastDrag = hitman.LastDrag.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.HitmanDragCd);
                hitman.LastMorph = hitman.LastMorph.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.HitmanMorphCooldown);
            }

            if (LocalPlayer().Is(RoleEnum.Arsonist))
            {
                var arsonist = GetRole<Arsonist>(LocalPlayer());
                arsonist.LastDoused = DateTime.UtcNow;
                arsonist.LastDoused = arsonist.LastDoused.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.DouseCd);
            }

            if (LocalPlayer().Is(RoleEnum.Doomsayer))
            {
                var doomsayer = GetRole<Doomsayer>(LocalPlayer());
                doomsayer.LastObserved = DateTime.UtcNow;
                doomsayer.LastObserved = doomsayer.LastObserved.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ObserveCooldown);
            }

            if (LocalPlayer().Is(RoleEnum.Executioner))
            {
                var exe = GetRole<Executioner>(LocalPlayer());
                if (exe.target == null)
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(LocalPlayer().NetId,
                        (byte)CustomRPC.ExecutionerToJester, SendOption.Reliable, -1);
                    writer.Write(exe.Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);

                    ExeTargetColor.ExecutionerChangeRole(exe.Player);
                }
            }

            if (LocalPlayer().Is(RoleEnum.Glitch))
            {
                var glitch = GetRole< Glitch> (LocalPlayer());
                glitch.LastKill = DateTime.UtcNow;
                glitch.LastHack = DateTime.UtcNow;
                glitch.LastMimic = DateTime.UtcNow;
                glitch.LastKill = glitch.LastKill.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.GlitchKillCooldown);
                glitch.LastHack = glitch.LastHack.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.HackCooldown);
                glitch.LastMimic = glitch.LastMimic.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.MimicCooldown);
            }

            if (LocalPlayer().Is(RoleEnum.GuardianAngel))
            {
                var ga = GetRole<GuardianAngel>(LocalPlayer());
                ga.LastProtected = DateTime.UtcNow;
                ga.LastProtected = ga.LastProtected.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ProtectCd);
                if (ga.target == null)
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(LocalPlayer().NetId,
                        (byte)CustomRPC.GuardianAngelChangeRole, SendOption.Reliable, -1);
                    writer.Write(ga.Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);

                    GATargetColor.GuardianAngelChangeRole(ga.Player);
                }
            }

            if (LocalPlayer().Is(RoleEnum.Juggernaut))
            {
                var juggernaut = GetRole<Juggernaut>(LocalPlayer());
                juggernaut.LastKill = DateTime.UtcNow;
                juggernaut.LastKill = juggernaut.LastKill.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.JuggKCd);
            }

            if (LocalPlayer().Is(RoleEnum.Plaguebearer))
            {
                var plaguebearer = GetRole<Plaguebearer>(LocalPlayer());
                plaguebearer.LastInfected = DateTime.UtcNow;
                plaguebearer.LastInfected = plaguebearer.LastInfected.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.InfectCd);
            }

            if (LocalPlayer().Is(RoleEnum.Romantic))
            {
                var romantic = GetRole<Romantic>(LocalPlayer());
                romantic.LastPick = DateTime.UtcNow;
                romantic.LastPick = romantic.LastPick.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.PickStartTimer);
            }

            if (LocalPlayer().Is(RoleEnum.Werewolf))
            {
                var surv = GetRole<Werewolf>(LocalPlayer());
                surv.LastMauled = DateTime.UtcNow;
                surv.LastMauled = surv.LastMauled.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.MaulCooldown);
            }

            if (LocalPlayer().Is(RoleEnum.SerialKiller))
            {
                var werewolf = GetRole<SerialKiller>(LocalPlayer());
                werewolf.LastStabbed = DateTime.UtcNow;
                werewolf.LastKilled = DateTime.UtcNow;
                werewolf.LastStabbed = werewolf.LastStabbed.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.StabCd);
                werewolf.LastKilled = werewolf.LastKilled.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.StabKillCd);
            }

            if (LocalPlayer().Is(RoleEnum.Jailor))
            {
                var jailor = GetRole<Jailor>(LocalPlayer());
                jailor.LastJailed = DateTime.UtcNow;
                jailor.LastJailed = jailor.LastJailed.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.JailCd);
            }

            if (LocalPlayer().Is(RoleEnum.Poisoner))
            {
                var Poisoner = GetRole<Poisoner>(LocalPlayer());
                Poisoner.LastPoisoned = DateTime.UtcNow;
                Poisoner.LastPoisoned = Poisoner.LastPoisoned.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.PoisonCd);
            }

            if (LocalPlayer().Is(RoleEnum.Vampire))
            {
                var vamp = GetRole<Vampire>(LocalPlayer());
                vamp.LastBit = DateTime.UtcNow;
                vamp.LastBit = vamp.LastBit.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.BiteCd);
            }

            if (LocalPlayer().Is(AbilityEnum.Paranoiac))
            {
                var paranoiac = GetAbility<Paranoiac>(LocalPlayer());
                var gameObj = new GameObject();
                var arrow = gameObj.AddComponent<ArrowBehaviour>();
                gameObj.transform.parent = LocalPlayer().gameObject.transform;
                var renderer = gameObj.AddComponent<SpriteRenderer>();
                renderer.sprite = Sprite;
                renderer.color = ColorManager.Paranoiac;
                arrow.image = renderer;
                gameObj.layer = 5;
                arrow.target = LocalPlayer().transform.position;
                paranoiac.ParanoiacArrow.Add(arrow);
            }
        }
    }
}