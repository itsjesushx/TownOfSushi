using static TownOfSushi.Objects.BombExtentions;

namespace TownOfSushi
{
    public static class RPCHandling
    {
        private static readonly List<(Type, int, bool)> CrewmateRoles = new();
        private static readonly List<(Type, int, bool)> NeutralBenignRoles = new();
        private static readonly List<(Type, int, bool)> NeutralEvilRoles = new();
        private static readonly List<(Type, int, bool)> NeutralKillingRoles = new();
        private static readonly List<(Type, int, bool)> ImpostorRoles = new();
        private static readonly List<(Type, int)> Modifiers = new();
        private static readonly List<(Type, int)> ImpostorModifiers = new();
        private static readonly List<(Type, int)> ButtonAbilities = new();
        private static readonly List<(Type, int)> AssassinAbilityModifiers = new();
        private static readonly List<(Type, int)> Abilities = new();
        private static readonly List<(Type, int)> NonTaskerAbilities = new();
        private static readonly List<(Type, int)> VisionAbilities = new();
        private static readonly List<(Type, CustomRPC, int)> AssassinAbility = new();

        internal static bool Check(int probability)
        {
            if (probability == 0) return false;
            if (probability == 100) return true;
            var num = Random.RandomRangeInt(1, 101);
            return num <= probability;
        }
        
        private static int PickRoleCount(int min, int max)
        {
            if (min > max) min = max;
            return Random.RandomRangeInt(min, max + 1);
        }
        private static void SortRoles(this List<(Type, int, bool)> roles, int max)
        {
            if (max <= 0)
            {
                roles.Clear();
                return;
            }

            var chosenRoles = roles.Where(x => x.Item2 == 100).ToList();
            chosenRoles.Shuffle();
            // Truncate the list if there are more 100% roles than the max.
            chosenRoles = chosenRoles.GetRange(0, Math.Min(max, chosenRoles.Count));

            if (chosenRoles.Count < max)
            {
                // These roles MAY appear in this game, but they may not.
                var potentialRoles = roles.Where(x => x.Item2 < 100).ToList();
                // Determine which roles appear in this game.
                var optionalRoles = potentialRoles.Where(x => Check(x.Item2)).ToList();
                potentialRoles = potentialRoles.Where(x => !optionalRoles.Contains(x)).ToList();

                optionalRoles.Shuffle();
                chosenRoles.AddRange(optionalRoles.GetRange(0, Math.Min(max - chosenRoles.Count, optionalRoles.Count)));

                // If there are not enough roles after that, randomly add
                // ones which were previously eliminated, up to the max.
                if (chosenRoles.Count < max)
                {
                    potentialRoles.Shuffle();
                    chosenRoles.AddRange(potentialRoles.GetRange(0, Math.Min(max - chosenRoles.Count, potentialRoles.Count)));
                }
            }

            // This list will be shuffled later in GenEachRole.
            roles.Clear();
            roles.AddRange(chosenRoles);
        }

        private static void SortModifiers(this List<(Type, int)> roles, int max)
        {
            var newList = roles.Where(x => x.Item2 == 100).ToList();
            newList.Shuffle();

            if (roles.Count < max)
                max = roles.Count;

            var roles2 = roles.Where(x => x.Item2 < 100).ToList();
            roles2.Shuffle();
            newList.AddRange(roles2.Where(x => Check(x.Item2)));

            while (newList.Count > max)
            {
                newList.Shuffle();
                newList.RemoveAt(newList.Count - 1);
            }

            roles = newList;
            roles.Shuffle();
        }

        private static void SortAbilities(this List<(Type, int)> roles, int max)
        {
            var newList = roles.Where(x => x.Item2 == 100).ToList();
            newList.Shuffle();

            if (roles.Count < max)
                max = roles.Count;

            var roles2 = roles.Where(x => x.Item2 < 100).ToList();
            roles2.Shuffle();
            newList.AddRange(roles2.Where(x => Check(x.Item2)));

            while (newList.Count > max)
            {
                newList.Shuffle();
                newList.RemoveAt(newList.Count - 1);
            }

            roles = newList;
            roles.Shuffle();
        }

       private static void GenEachRole(List<NetworkedPlayerInfo> infected)
        {
            var impostors = GetImpostors(infected);
            var crewmates = GetCrewmates(impostors);
            var crewRoles = new List<(Type, int, bool)>();
            var neutRoles = new List<(Type, int, bool)>();
            var impRoles = new List<(Type, int, bool)>();

            if (CustomGameOptions.GameMode == GameMode.Classic)
            {
                var benign = PickRoleCount(CustomGameOptions.MinNeutralBenignRoles, Math.Min(CustomGameOptions.MaxNeutralBenignRoles, NeutralBenignRoles.Count));
                var evil = PickRoleCount(CustomGameOptions.MinNeutralEvilRoles, Math.Min(CustomGameOptions.MaxNeutralEvilRoles, NeutralEvilRoles.Count));
                var killing = PickRoleCount(CustomGameOptions.MinNeutralKillingRoles, Math.Min(CustomGameOptions.MaxNeutralKillingRoles, NeutralKillingRoles.Count));

                var canSubtract = (int faction, int minFaction) => { return faction > minFaction; };
                var factions = new List<string>() { "Benign", "Evil", "Killing" };

                // Crew must always start out outnumbering neutrals, so subtract roles until that can be guaranteed.
                while (Math.Ceiling((double)crewmates.Count) <= benign + evil + killing)
                {
                    bool canSubtractBenign = canSubtract(benign, CustomGameOptions.MinNeutralBenignRoles);
                    bool canSubtractEvil = canSubtract(evil, CustomGameOptions.MinNeutralEvilRoles);
                    bool canSubtractKilling = canSubtract(killing, CustomGameOptions.MinNeutralKillingRoles);
                    bool canSubtractNone = !canSubtractBenign && !canSubtractEvil && !canSubtractKilling;

                    factions.Shuffle();
                    switch(factions.First())
                    {
                        case "Benign":
                            if (benign > 0 && (canSubtractBenign || canSubtractNone))
                            {
                                benign -= 1;
                                break;
                            }
                            goto case "Evil";
                        case "Evil":
                            if (evil > 0 && (canSubtractEvil || canSubtractNone))
                            {
                                evil -= 1;
                                break;
                            }
                            goto case "Killing";
                        case "Killing":
                            if (killing > 0 && (canSubtractKilling || canSubtractNone))
                            {
                                killing -= 1;
                                break;
                            }
                            goto default;
                        default:
                            if (benign > 0)
                            {
                                benign -= 1;
                            }
                            else if (evil > 0)
                            {
                                evil -= 1;
                            }
                            else if (killing > 0)
                            {
                                killing -= 1;
                            }
                            break;
                    }

                    if (benign + evil + killing == 0)
                        break;
                }

                NeutralBenignRoles.SortRoles(benign);
                NeutralEvilRoles.SortRoles(evil);
                NeutralKillingRoles.SortRoles(killing);
                CrewmateRoles.SortRoles(crewmates.Count - NeutralBenignRoles.Count - NeutralEvilRoles.Count - NeutralKillingRoles.Count);
                ImpostorRoles.SortRoles(impostors.Count);

                crewRoles.AddRange(CrewmateRoles);
                impRoles.AddRange(ImpostorRoles);
            }
            neutRoles.AddRange(NeutralBenignRoles);
            neutRoles.AddRange(NeutralEvilRoles);
            neutRoles.AddRange(NeutralKillingRoles);
            // Roles are not at this point, shuffled yet.
            // In All/Any mode, there is at least one neutral and one crewmate, but duplicates are allowed and probability is ignored.
            if (CustomGameOptions.GameMode == GameMode.AllAny)
            {
                // Add one neutral role to the game, if any are enabled.
                // This guarantees at least one neutral role's presence.
                if (neutRoles.Count > 0)
                {
                    neutRoles.Shuffle();
                    crewRoles.Add(neutRoles[0]);
                    // If it's unique, remove it from the list.
                    if (neutRoles[0].Item3 == true) neutRoles.Remove(neutRoles[0]);
                }
                // Add one crewmate role to the game, or vanilla Crewmate if none are enabled.
                // This guarantees at least one crewmate role's presence.
                if (CrewmateRoles.Count > 0)
                {
                    CrewmateRoles.Shuffle();
                    crewRoles.Add(CrewmateRoles[0]);
                    if (CrewmateRoles[0].Item3 == true) CrewmateRoles.Remove(CrewmateRoles[0]);
                }
                else
                {
                    crewRoles.Add((typeof(Crewmate), 100, false));
                }
                // Now add all the roles together.
                var allAnyRoles = new List<(Type, int, bool)>();
                allAnyRoles.AddRange(CrewmateRoles);
                allAnyRoles.AddRange(neutRoles);
                // Add crew & neutral roles up to the crewmate count, including duplicates (unless defined as unique).
                while (crewRoles.Count < crewmates.Count && allAnyRoles.Count > 0)
                {
                    allAnyRoles.Shuffle();
                    crewRoles.Add(allAnyRoles[0]);
                    if (allAnyRoles[0].Item3 == true) allAnyRoles.Remove(allAnyRoles[0]);
                }
                // Add impostor roles up to the impostor count, including duplicates (unless defined as unique).
                while (impRoles.Count < impostors.Count && ImpostorRoles.Count > 0)
                {
                    ImpostorRoles.Shuffle();
                    impRoles.Add(ImpostorRoles[0]);
                    if (ImpostorRoles[0].Item3 == true) ImpostorRoles.Remove(ImpostorRoles[0]);
                }
            }
            else
            {
                // Roles have already been sorted for Classic mode.
                // So just add in the neutral roles.
                crewRoles.AddRange(neutRoles);
            }

            // Shuffle roles before handing them out.
            // This should ensure a statistically equal chance of all permutations of roles.
            crewRoles.Shuffle();
            impRoles.Shuffle();

            // Hand out appropriate roles to crewmates and impostors.
            foreach (var (type, _, unique) in crewRoles)
            {
                GenRole<Role>(type, crewmates);
            }
            foreach (var (type, _, unique) in impRoles)
            {
                GenRole<Role>(type, impostors);
            }

            // Assign vanilla roles to anyone who did not receive a role.
            foreach (var crewmate in crewmates)
                GenRole<Role>(typeof(Crewmate), crewmate);

            foreach (var impostor in impostors)
                GenRole<Role>(typeof(Impostor), impostor);

            // Hand out assassin ability to killers according to the settings.
            var canHaveAbility = PlayerControl.AllPlayerControls.ToArray().Where(player => player.Is(Faction.Impostors)).ToList();
            canHaveAbility.Shuffle();
            var canHaveAbility2 = PlayerControl.AllPlayerControls.ToArray().Where(player => player.Is(RoleAlignment.NeutralKilling)).ToList();
            canHaveAbility2.Shuffle();

            var assassinConfig = new (List<PlayerControl>, int)[]
            {
                (canHaveAbility, CustomGameOptions.NumberOfImpostorAssassins),
                (canHaveAbility2, CustomGameOptions.NumberOfNeutralAssassins)
            };
            foreach ((var abilityList, int maxNumber) in assassinConfig)
            {
                int assassinNumber = maxNumber;
                while (abilityList.Count > 0 && assassinNumber > 0)
                {
                    var (type, rpc, _) = AssassinAbility.Ability();
                    Gen<Ability>(type, abilityList.TakeFirst(), rpc);
                    assassinNumber -= 1;
                }
            }

            // Hand out assassin modifiers, if enabled, to impostor assassins.
            var canHaveAssassinAbilityifier = PlayerControl.AllPlayerControls.ToArray().Where(player => player.Is(Faction.Impostors) && player.Is(AbilityEnum.Assassin)).ToList();
            canHaveAssassinAbilityifier.Shuffle();
            AssassinAbilityModifiers.SortModifiers(canHaveAssassinAbilityifier.Count);
            AssassinAbilityModifiers.Shuffle();

            foreach (var (type, _) in AssassinAbilityModifiers)
            {
                if (canHaveAssassinAbilityifier.Count == 0) break;
                GenModifier<Modifier>(type, canHaveAssassinAbilityifier);
            }

            // Hand out impostor modifiers.
            var canHaveImpModifier = PlayerControl.AllPlayerControls.ToArray().Where(player => player.Is(Faction.Impostors) && !player.Is(ModifierEnum.DoubleShot)).ToList();
            canHaveImpModifier.Shuffle();
            ImpostorModifiers.SortModifiers(canHaveImpModifier.Count);
            ImpostorModifiers.Shuffle();

            foreach (var (type, _) in ImpostorModifiers)
            {
                if (canHaveImpModifier.Count == 0) break;
                GenModifier<Modifier>(type, canHaveImpModifier);
            }

            // Hand out global modifiers.
            var canHaveModifier = PlayerControl.AllPlayerControls.ToArray().Where(player => !player.Is(ModifierEnum.Disperser) && !player.Is(ModifierEnum.DoubleShot) && !player.Is(ModifierEnum.Underdog)).ToList();
            canHaveModifier.Shuffle();
            Modifiers.SortModifiers(canHaveModifier.Count);
            Modifiers.Shuffle();

            foreach (var (type, id) in Modifiers)
            {
                if (canHaveModifier.Count == 0) break;
                GenModifier<Modifier>(type, canHaveModifier);
            }

            var canHaveAbility3 = PlayerControl.AllPlayerControls.ToArray().Where(player => !player.Is(AbilityEnum.Assassin)).ToList();
            // Now hand out Abilities to all remaining eligible players.
            Abilities.SortAbilities(canHaveAbility3.Count);
            Abilities.Shuffle();

            foreach (var (type, id) in Abilities)
            {
                if (canHaveAbility3.Count == 0) break;
                GenAbility<Ability>(type, canHaveAbility3);
            }

             // Glitch/Agent/Hitman cannot have Button Abilities.
            canHaveAbility3.RemoveAll(player => player.Is(RoleEnum.Glitch) || player.Is(RoleEnum.Agent) || player.Is(RoleEnum.Hitman));
            ButtonAbilities.SortAbilities(canHaveAbility3.Count);

            foreach (var (type, id) in ButtonAbilities)
            {
                if (canHaveAbility3.Count == 0) break;
                GenAbility<Ability>(type, canHaveAbility3);
            }

            foreach (var (type, id) in NonTaskerAbilities)
            {
                if (canHaveAbility3.Count == 0) break;
                GenAbility<Ability>(type, canHaveAbility3);
            }

            canHaveAbility3.RemoveAll(player => !player.Data.Role.IsImpostor || !player.Is(RoleAlignment.NeutralKilling) || !player.Is(RoleEnum.Jester) && CustomGameOptions.JesterImpVision || !player.Is(RoleEnum.Vulture) && CustomGameOptions.VultureImpVision);
            VisionAbilities.SortAbilities(canHaveAbility3.Count);
            foreach (var (type, id) in VisionAbilities)
            {
                if (canHaveAbility3.Count == 0) break;
                GenAbility<Ability>(type, canHaveAbility3);
            }

            // Players with fake tasks cant have Multitasker
            canHaveAbility3.RemoveAll(player => !player.HasTasks());
            NonTaskerAbilities.SortAbilities(canHaveAbility3.Count);

            var exeTargets = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Crewmates) && !x.Is(RoleEnum.Swapper) && !x.Is(RoleEnum.Vigilante) && !x.Is(RoleEnum.Jailor)).ToList();
            foreach (var role in GetRoles(RoleEnum.Executioner))
            {
                var exe = (Executioner)role;
                if (exeTargets.Count > 0)
                {
                    exe.target = exeTargets[Random.RandomRangeInt(0, exeTargets.Count)];
                    exeTargets.Remove(exe.target);

                    StartRPC(CustomRPC.SetTarget, role.Player.PlayerId, exe.target.PlayerId);
                }
            }

            var goodGATargets = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Crewmates) ).ToList();
            var evilGATargets = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Impostors) || x.Is(RoleAlignment.NeutralKilling)).ToList();
            foreach (var role in GetRoles(RoleEnum.GuardianAngel))
            {
                var ga = (GuardianAngel)role;
                if (!(goodGATargets.Count == 0 && CustomGameOptions.EvilTargetPercent == 0) ||
                    (evilGATargets.Count == 0 && CustomGameOptions.EvilTargetPercent == 100) ||
                    goodGATargets.Count == 0 && evilGATargets.Count == 0)
                {
                    if (goodGATargets.Count == 0)
                    {
                        ga.target = evilGATargets[Random.RandomRangeInt(0, evilGATargets.Count)];
                        evilGATargets.Remove(ga.target);
                    }
                    else if (evilGATargets.Count == 0 || !Check(CustomGameOptions.EvilTargetPercent))
                    {
                        ga.target = goodGATargets[Random.RandomRangeInt(0, goodGATargets.Count)];
                        goodGATargets.Remove(ga.target);
                    }
                    else
                    {
                        ga.target = evilGATargets[Random.RandomRangeInt(0, evilGATargets.Count)];
                        evilGATargets.Remove(ga.target);
                    }

                    StartRPC(CustomRPC.SetGATarget, role.Player.PlayerId, ga.target.PlayerId);
                }
            }
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
        public static class HandleRpc
        {
            public static void Postfix([HarmonyArgument(0)] byte callId, [HarmonyArgument(1)] MessageReader reader)
            {
                Assembly asm = typeof(Role).Assembly;

                byte readByte, readByte1, readByte2;
                sbyte readSByte, readSByte2;
                switch ((CustomRPC) callId)
                {
                    case CustomRPC.SetRole:
                        var player = PlayerById(reader.ReadByte());
                        var rstring = reader.ReadString();
                        Activator.CreateInstance(asm.GetType(rstring), new object[] { player });
                        break;
                    case CustomRPC.SetModifier:
                        var player2 = PlayerById(reader.ReadByte());
                        var mstring = reader.ReadString();
                        Activator.CreateInstance(asm.GetType(mstring), new object[] { player2 });
                        break;
                    case CustomRPC.SetAbility:
                        var player22 = PlayerById(reader.ReadByte());
                        var mstring2 = reader.ReadString();
                        Activator.CreateInstance(asm.GetType(mstring2), new object[] { player22 });
                        break;
                    case CustomRPC.Fortify:
                        switch (reader.ReadByte())
                        {
                            default:
                            case 0: //set fortify
                                var crusader = PlayerById(reader.ReadByte());
                                var fortified = PlayerById(reader.ReadByte());
                                var crusaderRole = GetRole<Crusader>(crusader);
                                crusaderRole.Fortified = fortified;
                                break;
                            case 1: //fortify alert
                                var crusaderPlayer = PlayerById(reader.ReadByte());
                                var crusaderRole2 = GetRole<Crusader>(crusaderPlayer);
                                if (PlayerControl.LocalPlayer == crusaderPlayer) 
                                {
                                    Flash(Colors.Crusader, 0.7f);
                                    SoundManager.Instance.PlaySound(ShipStatus.Instance.SabotageSound, false, 1f, null);
                                }
                                break;
                        }
                        break;

                    case CustomRPC.Jail:
                        var jailor = PlayerById(reader.ReadByte());
                        var jailorRole = GetRole<Jailor>(jailor);
                        switch (reader.ReadByte())
                        {
                            default:
                            case 0: //jail
                                var jailed = PlayerById(reader.ReadByte());
                                jailorRole.Jailed = jailed;
                                break;
                            case 1: //execute
                                if (jailorRole.Jailed.Is(Faction.Crewmates))
                                {
                                    jailorRole.IncorrectShots += 1;
                                    jailorRole.Executes = 0;
                                }
                                else jailorRole.CorrectKills += 1;
                                jailorRole.JailCell.Destroy();
                                AddJailButtons.ExecuteKill(jailorRole, jailorRole.Jailed);
                                jailorRole.Jailed = null;
                                break;
                        }
                        break;
                    case CustomRPC.ExecuteDeputyKill:    
                        var deputy = PlayerById(reader.ReadByte());    
                        var targetPlayerId = reader.ReadByte(); // Read the specific target's PlayerId    
                        var deputyRole = GetRole<Deputy>(deputy);    
                        var targetPlayer2 = PlayerById(targetPlayerId);    
                        if (targetPlayer2.Is(Faction.Crewmates))    
                        {
                            deputyRole.IncorrectShots += 1;
                            deputyRole.RemainingKills = 0;
                        }
                        else
                        {
                           deputyRole.CorrectDeputyShot += 1;
                        }
                        AddExecuteButtons.ExecuteKill(deputyRole, targetPlayer2);
                        break;
                    case CustomRPC.Poison:
                        var poisoner = PlayerById(reader.ReadByte());
                        var poisoned = PlayerById(reader.ReadByte());
                        var poisonerRole = GetRole<Poisoner>(poisoner);
                        poisonerRole.PoisonedPlayer = poisoned;
                        break;
                    case CustomRPC.PoisonKill:
                        var poisoner1 = PlayerById(reader.ReadByte());
                        var poisonerRole1 = GetRole<Poisoner>(poisoner1);
                        poisonerRole1.PoisonKill();
                        break;
                    case CustomRPC.Start:
                        readByte = reader.ReadByte();
                        ShowRoundOneShield.FirstRoundShielded = readByte == byte.MaxValue ? null : PlayerById(readByte);
                        ShowRoundOneShield.DiedFirst = "";
                        Murder.KilledPlayers.Clear();
                        ToggleZoom(reset : true);
                        JailChat.JailorMessage = false;
                        ResetWins();
                        ExileControllerPatch.lastExiled = null;
                        PatchKillTimer.GameStarted = false;
                        StartImitate.ImitatingPlayer = null;
                        AssassinExileControllerPatch.AssassinatedPlayers.Clear();
                        break;
                    case CustomRPC.JanitorClean:
                        readByte1 = reader.ReadByte();
                        var janitorPlayer = PlayerById(readByte1);
                        var janitorRole = GetRole<Janitor>(janitorPlayer);
                        readByte = reader.ReadByte();
                        var deadBodies = Object.FindObjectsOfType<DeadBody>();
                        foreach (var body in deadBodies)
                            if (body.ParentId == readByte)
                                Coroutines.Start(JanitorCoroutine.CleanCoroutine(body, janitorRole));
                        break;
                    case CustomRPC.VultureEat:
                        readByte1 = reader.ReadByte();
                        var vultureP = PlayerById(readByte1);
                        var vultureR = GetRole<Vulture>(vultureP);
                        readByte = reader.ReadByte();
                        var deadbodies2 = Object.FindObjectsOfType<DeadBody>();
                        foreach (var body2243 in deadbodies2)
                            if (body2243.ParentId == readByte)
                                Coroutines.Start(VultureCoroutine.EatCoroutine(body2243, vultureR));
                        break;
                    case CustomRPC.SetSwaps:
                        readSByte = reader.ReadSByte();
                        SwapVotes.Swap1 = MeetingHud.Instance.playerStates.FirstOrDefault(x => x.TargetPlayerId == readSByte);
                        readSByte2 = reader.ReadSByte();
                        SwapVotes.Swap2 = MeetingHud.Instance.playerStates.FirstOrDefault(x => x.TargetPlayerId == readSByte2);
                        PluginSingleton<TownOfSushi>.Instance.Log.LogMessage("Bytes received - " + readSByte + " - " + readSByte2);
                        break;
                    case CustomRPC.EngineerFix:
                        var engineer = PlayerById(reader.ReadByte());
                        GetRole<Engineer>(engineer).MaxUses -= 1;
                        break;
                    case CustomRPC.FixLights:
                        var lights = ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();
                        lights.ActualSwitches = lights.ExpectedSwitches;
                        break;
                    case CustomRPC.Bite:
                        var newVamp = PlayerById(reader.ReadByte());
                        VampirePerformConvert.Convert(newVamp);
                        break;
                    case CustomRPC.Imitate:
                        var imitator = PlayerById(reader.ReadByte());
                        var imitatorRole = GetRole<Imitator>(imitator);
                        var imitateTarget = PlayerById(reader.ReadByte());
                        imitatorRole.ImitatePlayer = imitateTarget;
                        break;
                    case CustomRPC.StartImitate:
                        var imitator2 = PlayerById(reader.ReadByte());
                        var imitatorRole2 = GetRole<Imitator>(imitator2);
                        StartImitate.Imitate(imitatorRole2);
                        break;
                    case CustomRPC.StartRemember:
                        var amnesiac2 = PlayerById(reader.ReadByte());
                        var amnesiacRole2 = GetRole<Amnesiac>(amnesiac2);
                        var amnesiacTarget2 = PlayerById(reader.ReadByte());
                        amnesiacRole2.ToRemember = amnesiacTarget2;
                        RememberRole.Remember(amnesiacRole2, amnesiacTarget2);
                        break;
                    case CustomRPC.Remember:
                        var amnesiac = PlayerById(reader.ReadByte());
                        var amnesiacRole = GetRole<Amnesiac>(amnesiac);
                        var amnesiacTarget = PlayerById(reader.ReadByte());
                        amnesiacRole.ToRemember = amnesiacTarget;
                        break;
                    case CustomRPC.Protect:
                        readByte1 = reader.ReadByte();
                        readByte2 = reader.ReadByte();

                        var medic = PlayerById(readByte1);
                        var shield = PlayerById(readByte2);
                        GetRole<Medic>(medic).ShieldedPlayer = shield;
                        GetRole<Medic>(medic).UsedAbility = true;
                        break;
                    case CustomRPC.AttemptSound:
                        var medicId = reader.ReadByte();
                        readByte = reader.ReadByte();
                        MedicStopKill.BreakShield(medicId, readByte, CustomGameOptions.ShieldBreaks);
                        break;
                    case CustomRPC.BypassKill:
                        var killer = PlayerById(reader.ReadByte());
                        var target = PlayerById(reader.ReadByte());
                        MurderPlayer(killer, target, true);
                        break;
                    case CustomRPC.BypassKill2:
                        var killer22 = PlayerById(reader.ReadByte());
                        var target22 = PlayerById(reader.ReadByte());
                        MurderPlayer(killer22, target22, false);
                        break;
                    case CustomRPC.BypassMultiKill:
                        var killer2 = PlayerById(reader.ReadByte());
                        var target2 = PlayerById(reader.ReadByte());

                        MurderPlayer(killer2, target2, false);
                        break;
                    case CustomRPC.AssassinKill:
                        var toDie = PlayerById(reader.ReadByte());
                        var assassin = GetAbilityValue<Assassin>(AbilityEnum.Assassin);
                        var assassinPlayer = PlayerById(reader.ReadByte());
                        AssassinKill.MurderPlayer(assassin, toDie);
                        AssassinKill.AssassinKillCount(toDie, assassinPlayer);
                        break;
                    case CustomRPC.Spell:
                        var witch = GetRole<Witch>(PlayerById(reader.ReadByte()));
                        var targetPlayer = PlayerById(reader.ReadByte());
                        if (witch != null && targetPlayer != null)    
                        {        
                            witch.SpelledPlayers.Add(targetPlayer.PlayerId);    
                        }
                        break;
                    case CustomRPC.VigilanteKill:
                        var toDie2 = PlayerById(reader.ReadByte());
                        var vigilante = GetRoleValue<Vigilante>(RoleEnum.Vigilante);
                        var vigilantePlayer = PlayerById(reader.ReadByte());
                        VigilanteKill.MurderPlayer(vigilante, toDie2);
                        VigilanteKill.VigiKillCount(toDie2, vigilantePlayer);
                        break;
                    case CustomRPC.DoomsayerKill:
                        var toDie3 = PlayerById(reader.ReadByte());
                        var doomsayer = GetRoleValue<Doomsayer>(RoleEnum.Doomsayer);
                        var doomsayerPlayer = PlayerById(reader.ReadByte());
                        DoomsayerKill.MurderPlayer(doomsayer, toDie3);
                        DoomsayerKill.DoomKillCount(toDie3, doomsayerPlayer);
                        break;
                    case CustomRPC.SetMimic:
                        var glitchPlayer = PlayerById(reader.ReadByte());
                        var mimicPlayer = PlayerById(reader.ReadByte());
                        var glitchRole = GetRole<Glitch>(glitchPlayer);
                        glitchRole.MimicTarget = mimicPlayer;
                        glitchRole.IsUsingMimic = true;
                        Morph(glitchPlayer, mimicPlayer);
                        break;
                    case CustomRPC.SetHitmanMorph:
                        var hitmanPlayer = PlayerById(reader.ReadByte());
                        var morphPlayer = PlayerById(reader.ReadByte());
                        var hitmanRole = GetRole<Hitman>(hitmanPlayer);
                        hitmanRole.MorphTarget = morphPlayer;
                        hitmanRole.IsUsingMorph = true;
                        Morph(hitmanPlayer, morphPlayer);
                        break;
                    case CustomRPC.RpcResetAnim:
                        var animPlayer = PlayerById(reader.ReadByte());
                        var theGlitchRole = GetRole<Glitch>(animPlayer);
                        theGlitchRole.MimicTarget = null;
                        theGlitchRole.IsUsingMimic = false;
                        Unmorph(theGlitchRole.Player);
                        break;
                    case CustomRPC.RpcResetAnim2:
                        var animPlayer2 = PlayerById(reader.ReadByte());
                        var hitmanRole3 = GetRole<Hitman>(animPlayer2);
                        hitmanRole3.MorphTarget = null;
                        hitmanRole3.IsUsingMorph = false;
                        Unmorph(hitmanRole3.Player);
                        break;
                    case CustomRPC.SetHacked:
                        var hackingPlayer = PlayerById(reader.ReadByte());
                        var hackPlayer = PlayerById(reader.ReadByte());
                        var glitch = GetRole<Glitch>(hackingPlayer);
                        glitch.SetHacked(hackPlayer);
                        break;
                    case CustomRPC.Morph:
                        var morphling = PlayerById(reader.ReadByte());
                        var morphTarget = PlayerById(reader.ReadByte());
                        var morphRole = GetRole<Morphling>(morphling);
                        morphRole.TimeRemaining = CustomGameOptions.MorphlingDuration;
                        morphRole.MorphedPlayer = morphTarget;
                        break;
                    case CustomRPC.HitmanMorph:
                        var hitman = PlayerById(reader.ReadByte());
                        var hitmanTarget = PlayerById(reader.ReadByte());
                        var hitmanRole2 = GetRole<Morphling>(hitman);
                        hitmanRole2.TimeRemaining = CustomGameOptions.HitmanMorphDuration;
                        hitmanRole2.MorphedPlayer = hitmanTarget;
                        break;
                    case CustomRPC.SetTarget:
                        var exe = PlayerById(reader.ReadByte());
                        var exeTarget = PlayerById(reader.ReadByte());
                        var exeRole = GetRole<Executioner>(exe);
                        exeRole.target = exeTarget;
                        break;
                    case CustomRPC.SetGATarget:
                        var ga = PlayerById(reader.ReadByte());
                        var gaTarget = PlayerById(reader.ReadByte());
                        var gaRole = GetRole<GuardianAngel>(ga);
                        gaRole.target = gaTarget;
                        break;
                    case CustomRPC.SetRomanticTarget:
                        var surv = PlayerById(reader.ReadByte());
                        var romanticTarget = PlayerById(reader.ReadByte());
                        var romanticRole = GetRole<Romantic>(surv);
                        romanticRole.Beloved = romanticTarget;
                        break;
                    case CustomRPC.Blackmail:
                        var blackmailer = GetRole<Blackmailer>(PlayerById(reader.ReadByte()));
                        blackmailer.Blackmailed = PlayerById(reader.ReadByte());
                        break;
                    case CustomRPC.Confess:
                        var oracle = GetRole<Oracle>(PlayerById(reader.ReadByte()));
                        oracle.Confessor = PlayerById(reader.ReadByte());
                        var faction = reader.ReadInt32();
                        if (faction == 0) oracle.RevealedFaction = Faction.Crewmates;
                        else if (faction == 1) oracle.RevealedAlignment = RoleAlignment.NeutralEvil;
                        else oracle.RevealedFaction = Faction.Impostors;
                        break;
                    case CustomRPC.ExecutionerToJester:
                        ExeTargetColor.ExecutionerChangeRole(PlayerById(reader.ReadByte()));
                        break;
                    case CustomRPC.GuardianAngelChangeRole:
                        GATargetColor.GuardianAngelChangeRole(PlayerById(reader.ReadByte()));
                        break;
                    case CustomRPC.RomanticChangeRole:
                        RomanticChangeRolePatch.RomanticChangeRole(PlayerById(reader.ReadByte()));
                        break;
                    case CustomRPC.Mine:
                        var ventId = reader.ReadInt32();
                        var miner = PlayerById(reader.ReadByte());
                        var minerRole = GetRole<Miner>(miner);
                        var pos = reader.ReadVector2();
                        var zAxis = reader.ReadSingle();
                        PlaceVent.SpawnVent(ventId, minerRole, pos, zAxis);
                        break;
                    case CustomRPC.Swoop:
                        var swooper = PlayerById(reader.ReadByte());
                        var swooperRole = GetRole<Swooper>(swooper);
                        swooperRole.TimeRemaining = CustomGameOptions.SwoopDuration;
                        swooperRole.Swoop();
                        break;
                    case CustomRPC.GlitchWin:
                        GlitchWin = true;
                        break;
                    case CustomRPC.JuggernautWin:
                        JuggernautWin = true;
                        break;
                    case CustomRPC.PestilenceWin:
                        PestilenceWin = true;
                        break;
                    case CustomRPC.ArsonistWin:
                        ArsonistWin = true;
                        break;
                    case CustomRPC.WerewolfWin:
                        WerewolfWin = true;
                        break;
                    case CustomRPC.PlaguebearerWin:
                        PlaguebearerWin = true;
                        break;
                    case CustomRPC.HitmanWin:
                        HitmanWin = true;
                        break;
                    case CustomRPC.AgentWin:
                        AgentWin = true;
                        break;
                    case CustomRPC.VampireWin:
                        VampireWins = true;
                        break;
                    case CustomRPC.ImpostorWin:
                        ImpostorsWin = true;
                        break;
                    case CustomRPC.CrewmateWin:
                        CrewmatesWin = true;
                        break;
                    case CustomRPC.DoomsayerWin:
                        DoomsayerWin = true;
                        break;
                    case CustomRPC.JesterWin:
                        JesterWin = true;
                        break; 
                    case CustomRPC.VultureWin:
                        VultureWin = true;
                        break; 
                    case CustomRPC.ExecutionerWin:
                        ExecutionerWin = true;
                        break;
                    case CustomRPC.Camouflage:
                        var venerer = PlayerById(reader.ReadByte());
                        var venererRole = GetRole<Venerer>(venerer);
                        venererRole.TimeRemaining = CustomGameOptions.AbilityDuration;
                        venererRole.KillsAtStartAbility = reader.ReadInt32();
                        venererRole.Ability();
                        break;
                    case CustomRPC.Alert:
                        var veteran = PlayerById(reader.ReadByte());
                        var veteranRole = GetRole<Veteran>(veteran);
                        veteranRole.TimeRemaining = CustomGameOptions.AlertDuration;
                        veteranRole.Alert();
                        break;
                    case CustomRPC.Plant:
                        if (PlayerControl.LocalPlayer.Data.IsImpostor()) Coroutines.Start(BombTeammate.BombShowTeammate(new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle())));
                        break;
                    case CustomRPC.GAProtect:
                        var ga2 = PlayerById(reader.ReadByte());
                        var ga2Role = GetRole<GuardianAngel>(ga2);
                        ga2Role.TimeRemaining = CustomGameOptions.ProtectDuration;
                        ga2Role.Protect();
                        break;
                    case CustomRPC.Mediate:
                        var mediatedPlayer = PlayerById(reader.ReadByte());
                        var medium = GetRole<Medium>(PlayerById(reader.ReadByte()));
                        if (PlayerControl.LocalPlayer.PlayerId != mediatedPlayer.PlayerId) break;
                        medium.AddMediatePlayer(mediatedPlayer.PlayerId);
                        break;
                    case CustomRPC.FlashGrenade:
                        var grenadier = PlayerById(reader.ReadByte());
                        var grenadierRole = GetRole<Grenadier>(grenadier);
                        grenadierRole.TimeRemaining = CustomGameOptions.GrenadeDuration;
                        grenadierRole.Flash();
                        break;
                    case CustomRPC.Infect:
                        var pb = GetRole<Plaguebearer>(PlayerById(reader.ReadByte()));
                        pb.SpreadInfection(PlayerById(reader.ReadByte()), PlayerById(reader.ReadByte()));
                        break;
                    case CustomRPC.TurnPestilence:
                        GetRole<Plaguebearer>(PlayerById(reader.ReadByte())).TurnPestilence();
                        break;
                    case CustomRPC.Maul:
                        var ww = PlayerById(reader.ReadByte());
                        var wwRole = GetRole<Werewolf>(ww);
                        wwRole.Maul();
                        break;
                    case CustomRPC.FixAnimation:
                        var player3 = PlayerById(reader.ReadByte());
                        player3.MyPhysics.ResetMoveState();
                        player3.Collider.enabled = true;
                        player3.moveable = true;
                        player3.NetTransform.enabled = true;
                        break;
                    case CustomRPC.BarryButton:
                        var buttonBarry = PlayerById(reader.ReadByte());

                        if (AmongUsClient.Instance.AmHost)
                        {
                            MeetingRoomManager.Instance.reporter = buttonBarry;
                            MeetingRoomManager.Instance.target = null;
                            AmongUsClient.Instance.DisconnectHandlers.AddUnique(MeetingRoomManager.Instance
                                .Cast<IDisconnectHandler>());
                            if (GameManager.Instance.CheckTaskCompletion()) return;

                            DestroyableSingleton<HudManager>.Instance.OpenMeetingRoom(buttonBarry);
                            buttonBarry.RpcStartMeeting(null);
                        }
                        break;
                    case CustomRPC.Disperse:
                        byte teleports = reader.ReadByte();
                        Dictionary<byte, Vector2> coordinates = new Dictionary<byte, Vector2>();
                        for (int i = 0; i < teleports; i++)
                        {
                            byte playerId = reader.ReadByte();
                            Vector2 location = reader.ReadVector2();
                            coordinates.Add(playerId, location);
                        }
                        Disperser.DispersePlayersToCoordinates(coordinates);
                        break;
                    case CustomRPC.BaitReport:
                        var baitKiller = PlayerById(reader.ReadByte());
                        var bait = PlayerById(reader.ReadByte());
                        baitKiller.ReportDeadBody(bait.Data);
                        break;
                    case CustomRPC.CheckMurder:
                        var murderKiller = PlayerById(reader.ReadByte());
                        var murderTarget = PlayerById(reader.ReadByte());
                        murderKiller.CheckMurder(murderTarget);
                        break;
                    case CustomRPC.Drag:
                        readByte1 = reader.ReadByte();
                        var dienerPlayer = PlayerById(readByte1);
                        var dienerRole = GetRole<Undertaker>(dienerPlayer);
                        readByte = reader.ReadByte();
                        var dienerBodies = Object.FindObjectsOfType<DeadBody>();
                        foreach (var body in dienerBodies)
                            if (body.ParentId == readByte)
                                dienerRole.CurrentlyDragging = body;

                        break;
                    case CustomRPC.HitmanDrag:
                        readByte1 = reader.ReadByte();
                        var dienerPlayer2 = PlayerById(readByte1);
                        var dienerRole2 = GetRole<Hitman>(dienerPlayer2);
                        readByte = reader.ReadByte();
                        var dienerBodies2 = Object.FindObjectsOfType<DeadBody>();
                        foreach (var body in dienerBodies2)
                            if (body.ParentId == readByte)
                                dienerRole2.CurrentlyDragging = body;
                        break;
                    case CustomRPC.Drop:
                        readByte1 = reader.ReadByte();
                        var v2 = reader.ReadVector2();
                        var v2z = reader.ReadSingle();
                        var dienerPlayer22 = PlayerById(readByte1);
                        var dienerRole22 = GetRole<Undertaker>(dienerPlayer22);
                        var body2 = dienerRole22.CurrentlyDragging;
                        dienerRole22.CurrentlyDragging = null;

                        body2.transform.position = new Vector3(v2.x, v2.y, v2z);

                        break;
                    case CustomRPC.HitmanDrop:
                        readByte1 = reader.ReadByte();
                        var v22 = reader.ReadVector2();
                        var v2z2 = reader.ReadSingle();
                        var dienerPlayer223 = PlayerById(readByte1);
                        var dienerRole223 = GetRole<Hitman>(dienerPlayer223);
                        var body22 = dienerRole223.CurrentlyDragging;
                        dienerRole223.CurrentlyDragging = null;
                        body22.transform.position = new Vector3(v22.x, v22.y, v2z2);
                        break;
                    case CustomRPC.SetAssassin:
                        new Assassin(PlayerById(reader.ReadByte()));
                        break;
                    case CustomRPC.AbilityTrigger:
                        var abilityUser = Utils.PlayerById(reader.ReadByte());
                        var abilitytargetId = reader.ReadByte();
                        var abilitytarget = abilitytargetId == byte.MaxValue ? null : Utils.PlayerById(abilitytargetId);
                        foreach (Role hunterRole2 in GetRoles(RoleEnum.Hunter))
                        {
                            Hunter hunter = (Hunter)hunterRole2;
                            if (hunter.StalkedPlayer == abilityUser) hunter.RpcCatchPlayer(abilityUser);
                        }
                        break;
                    case CustomRPC.StopStart:
                        StopStart(reader.ReadByte());
                        break;
                    case CustomRPC.SetGameStarting:
                        SetGameStarting();
                        break;
                    case CustomRPC.VersionHandshake:
                        byte major = reader.ReadByte();
                        byte minor = reader.ReadByte();
                        byte patch = reader.ReadByte();
                        float timer = reader.ReadSingle();
                        if (!AmongUsClient.Instance.AmHost && timer >= 0f) GameStartManagerPatch.timer = timer;
                        int versionOwnerId = reader.ReadPackedInt32();
                        byte revision = 0xFF;
                        Guid guid;
                        if (reader.Length - reader.Position >= 17) 
                        { // enough bytes left to read
                            revision = reader.ReadByte();
                            // GUID
                            byte[] gbytes = reader.ReadBytes(16);
                            guid = new Guid(gbytes);
                        }     
                        else {
                            guid = new Guid(new byte[16]);
                        }
                        VersionHandshake(major, minor, patch, revision == 0xFF ? -1 : revision, guid, versionOwnerId);
                    break;
                    case CustomRPC.Retribution:
                        var hunter2 = GetRole<Hunter>(Utils.PlayerById(reader.ReadByte()));
                        var hunterLastVoted = PlayerById(reader.ReadByte());
                        Retribution.MurderPlayer(hunter2, hunterLastVoted);
                        break;
                    case CustomRPC.HunterStalk:
                        var stalker = PlayerById(reader.ReadByte());
                        var stalked = PlayerById(reader.ReadByte());
                        Hunter hunterRole = GetRole<Hunter>(stalker);
                        hunterRole.StalkDuration = CustomGameOptions.HunterStalkDuration;
                        hunterRole.StalkedPlayer = stalked;
                        hunterRole.MaxUses -= 1;
                        hunterRole.Stalk();
                        break;
                    case CustomRPC.Escape:
                        var escapist = PlayerById(reader.ReadByte());
                        var escapistRole = GetRole<Escapist>(escapist);
                        var escapePos = reader.ReadVector2();
                        escapistRole.EscapePoint = escapePos;
                        Escapist.Escape(escapist);
                        break;
                    case CustomRPC.RemoveAllBodies:
                        var buggedBodies = Object.FindObjectsOfType<DeadBody>();
                        foreach (var body in buggedBodies)
                            body.gameObject.Destroy();
                        break;
                    case CustomRPC.SubmergedFixOxygen:
                        SubmergedCompatibility.RepairOxygen();
                        break;
                    case CustomRPC.SetPos:
                        var setplayer = PlayerById(reader.ReadByte());
                        setplayer.transform.position = new Vector2(reader.ReadSingle(), reader.ReadSingle());
                        break;
                    case CustomRPC.ShareOptions:
                        HandleShareOptions(reader.ReadByte(), reader);
                        break;
                    case CustomRPC.SetSettings:
                        readByte = reader.ReadByte();
                        GameOptionsManager.Instance.currentNormalGameOptions.MapId = readByte == byte.MaxValue ? (byte)0 : readByte;
                        GameOptionsManager.Instance.currentNormalGameOptions.RoleOptions.SetRoleRate(RoleTypes.Scientist, 0, 0);
                        GameOptionsManager.Instance.currentNormalGameOptions.RoleOptions.SetRoleRate(RoleTypes.Engineer, 0, 0);
                        GameOptionsManager.Instance.currentNormalGameOptions.RoleOptions.SetRoleRate(RoleTypes.GuardianAngel, 0, 0);
                        GameOptionsManager.Instance.currentNormalGameOptions.RoleOptions.SetRoleRate(RoleTypes.Tracker, 0, 0);
                        GameOptionsManager.Instance.currentNormalGameOptions.RoleOptions.SetRoleRate(RoleTypes.Noisemaker, 0, 0);
                        GameOptionsManager.Instance.currentNormalGameOptions.RoleOptions.SetRoleRate(RoleTypes.Shapeshifter, 0, 0);
                        GameOptionsManager.Instance.currentNormalGameOptions.RoleOptions.SetRoleRate(RoleTypes.Phantom, 0, 0);
                        break;
                }
            }
        }

        [HarmonyPatch(typeof(RoleManager), nameof(RoleManager.SelectRoles))]
        public static class RpcSetRole
        {
            public static void Postfix()
            {
                PluginSingleton<TownOfSushi>.Instance.Log.LogMessage("Rpc Set Roles");
                var infected = GameData.Instance.AllPlayers.ToArray().Where(o => o.IsImpostor());
                if (ShowRoundOneShield.DiedFirst != null && CustomGameOptions.FirstDeathShield)
                {
                    var shielded = false;
                    foreach (var player in PlayerControl.AllPlayerControls)
                    {
                        if (player.name == ShowRoundOneShield.DiedFirst)
                        {
                            ShowRoundOneShield.FirstRoundShielded = player;
                            shielded = true;
                        }
                    }
                    if (!shielded) ShowRoundOneShield.FirstRoundShielded = null;
                }
                else ShowRoundOneShield.FirstRoundShielded = null;
                ShowRoundOneShield.DiedFirst = "";
                ExileControllerPatch.lastExiled = null;
                PatchKillTimer.GameStarted = false;
                ResetWins();
                JailChat.JailorMessage = false;
                ToggleZoom(reset : true);
                StartImitate.ImitatingPlayer = null;
                AssassinExileControllerPatch.AssassinatedPlayers.Clear();
                CrewmateRoles.Clear();
                NeutralBenignRoles.Clear();
                NeutralEvilRoles.Clear();
                NeutralKillingRoles.Clear();
                ImpostorRoles.Clear();
                Modifiers.Clear();
                ImpostorModifiers.Clear();
                ButtonAbilities.Clear();
                VisionAbilities.Clear();
                NonTaskerAbilities.Clear();
                Abilities.Clear();
                AssassinAbilityModifiers.Clear();
                AssassinAbility.Clear();
                Murder.KilledPlayers.Clear();

                if (ShowRoundOneShield.FirstRoundShielded != null)
                {
                    StartRPC(CustomRPC.Start, ShowRoundOneShield.FirstRoundShielded.PlayerId);
                }
                else
                {
                    StartRPC(CustomRPC.Start, byte.MaxValue);
                }

                if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek) return;

                if (CustomGameOptions.GameMode == GameMode.Classic || CustomGameOptions.GameMode == GameMode.AllAny)
                {
                    #region Crewmate Roles
                    
                    if (CustomGameOptions.VigilanteOn > 0)
                        CrewmateRoles.Add((typeof(Vigilante), CustomGameOptions.VigilanteOn, false));
                    
                    if (CustomGameOptions.EngineerOn > 0)
                        CrewmateRoles.Add((typeof(Engineer), CustomGameOptions.EngineerOn, false));

                    if (CustomGameOptions.InvestigatorOn > 0)
                        CrewmateRoles.Add((typeof(Investigator), CustomGameOptions.InvestigatorOn, false));

                    if (CustomGameOptions.MedicOn > 0)
                        CrewmateRoles.Add((typeof(Medic), CustomGameOptions.MedicOn, true));

                    if (CustomGameOptions.CrusaderOn > 0)
                        CrewmateRoles.Add((typeof(Crusader), CustomGameOptions.CrusaderOn, true));
                    
                    if (CustomGameOptions.JailorOn > 0)
                        CrewmateRoles.Add((typeof(Jailor), CustomGameOptions.JailorOn, true));

                    if (CustomGameOptions.DeputyOn > 0)
                        CrewmateRoles.Add((typeof(Deputy), CustomGameOptions.DeputyOn, true));
                    
                    if (CustomGameOptions.SwapperOn > 0)
                        CrewmateRoles.Add((typeof(Swapper), CustomGameOptions.SwapperOn, true));

                    if (CustomGameOptions.DetectiveOn > 0)
                        CrewmateRoles.Add((typeof(Detective), CustomGameOptions.DetectiveOn, false));

                    if (CustomGameOptions.VeteranOn > 0)
                        CrewmateRoles.Add((typeof(Veteran), CustomGameOptions.VeteranOn, false));

                    if (CustomGameOptions.SeerOn > 0)
                        CrewmateRoles.Add((typeof(Seer), CustomGameOptions.SeerOn, false));

                    if (CustomGameOptions.TrackerOn > 0)
                        CrewmateRoles.Add((typeof(Tracker), CustomGameOptions.TrackerOn, false));
                    
                    if (CustomGameOptions.MediumOn > 0)
                        CrewmateRoles.Add((typeof(Medium), CustomGameOptions.MediumOn, false));
                    
                    if (CustomGameOptions.HunterOn > 0)
                        CrewmateRoles.Add((typeof(Hunter), CustomGameOptions.HunterOn, false));

                    if (CustomGameOptions.MysticOn > 0)
                        CrewmateRoles.Add((typeof(Mystic), CustomGameOptions.MysticOn, false));

                    if (CustomGameOptions.TrapperOn > 0)
                        CrewmateRoles.Add((typeof(Trapper), CustomGameOptions.TrapperOn, false));

                    if (CustomGameOptions.ImitatorOn > 0)
                        CrewmateRoles.Add((typeof(Imitator), CustomGameOptions.ImitatorOn, true));

                    if (CustomGameOptions.OracleOn > 0)
                        CrewmateRoles.Add((typeof(Oracle), CustomGameOptions.OracleOn, true));

                    #endregion

                    #region Neutral Roles
                    if (CustomGameOptions.JesterOn > 0)
                        NeutralEvilRoles.Add((typeof(Jester), CustomGameOptions.JesterOn, false));
                    
                    if (CustomGameOptions.ExecutionerOn > 0)
                        NeutralEvilRoles.Add((typeof(Executioner), CustomGameOptions.ExecutionerOn, false));

                    if (CustomGameOptions.DoomsayerOn > 0)
                        NeutralEvilRoles.Add((typeof(Doomsayer), CustomGameOptions.DoomsayerOn, false));
                    
                    if (CustomGameOptions.VultureOn > 0)
                        NeutralEvilRoles.Add((typeof(Vulture), CustomGameOptions.VultureOn, true));

                    if (CustomGameOptions.RomanticOn > 0)
                        NeutralBenignRoles.Add((typeof(Romantic), CustomGameOptions.RomanticOn, false));

                    if (CustomGameOptions.AmnesiacOn > 0)
                        NeutralBenignRoles.Add((typeof(Amnesiac), CustomGameOptions.AmnesiacOn, false));
                        
                    if (CustomGameOptions.GuardianAngelOn > 0)
                        NeutralBenignRoles.Add((typeof(GuardianAngel), CustomGameOptions.GuardianAngelOn, false));

                    if (CustomGameOptions.GlitchOn > 0 && !FungleMap())
                        NeutralKillingRoles.Add((typeof(Glitch), CustomGameOptions.GlitchOn, false));

                    if (CustomGameOptions.ArsonistOn > 0)
                        NeutralKillingRoles.Add((typeof(Arsonist), CustomGameOptions.ArsonistOn, true));
                    
                    if (CustomGameOptions.PlaguebearerOn > 0)
                        NeutralKillingRoles.Add((typeof(Plaguebearer), CustomGameOptions.PlaguebearerOn, true));

                    if (CustomGameOptions.SerialKillerOn > 0)
                        NeutralKillingRoles.Add((typeof(SerialKiller), CustomGameOptions.SerialKillerOn, false));
                    
                    if (CustomGameOptions.AgentOn > 0 && !CustomGameOptions.SkipAgent && !FungleMap())
                        NeutralKillingRoles.Add((typeof(Agent), CustomGameOptions.AgentOn, true));
                    
                    if (CustomGameOptions.AgentOn > 0 && CustomGameOptions.SkipAgent && !FungleMap())
                        NeutralKillingRoles.Add((typeof(Hitman), CustomGameOptions.AgentOn, false));
                    
                    if (CustomGameOptions.WerewolfOn > 0)
                        NeutralKillingRoles.Add((typeof(Werewolf), CustomGameOptions.WerewolfOn, true));

                    if (CustomGameOptions.GameMode == GameMode.Classic && CustomGameOptions.VampireOn > 0)
                        NeutralKillingRoles.Add((typeof(Vampire), CustomGameOptions.VampireOn, true));

                    if (CustomGameOptions.JuggernautOn > 0)
                        NeutralKillingRoles.Add((typeof(Juggernaut), CustomGameOptions.JuggernautOn, false));

                    #endregion

                    #region Impostor Roles

                    if (CustomGameOptions.UndertakerOn > 0)
                        ImpostorRoles.Add((typeof(Undertaker), CustomGameOptions.UndertakerOn, false));

                    if (CustomGameOptions.MorphlingOn > 0 && !FungleMap())
                        ImpostorRoles.Add((typeof(Morphling), CustomGameOptions.MorphlingOn, false));

                    if (CustomGameOptions.BlackmailerOn > 0)
                        ImpostorRoles.Add((typeof(Blackmailer), CustomGameOptions.BlackmailerOn, true));

                    if (CustomGameOptions.MinerOn > 0 && !MiraHQMap())
                        ImpostorRoles.Add((typeof(Miner), CustomGameOptions.MinerOn, true));
                    
                    if (CustomGameOptions.PoisonerOn > 0)
                        ImpostorRoles.Add((typeof(Poisoner), CustomGameOptions.PoisonerOn, true));

                    if (CustomGameOptions.SwooperOn > 0 && !FungleMap())
                        ImpostorRoles.Add((typeof(Swooper), CustomGameOptions.SwooperOn, false));

                    if (CustomGameOptions.JanitorOn > 0)
                        ImpostorRoles.Add((typeof(Janitor), CustomGameOptions.JanitorOn, false));

                    if (CustomGameOptions.GrenadierOn > 0)
                        ImpostorRoles.Add((typeof(Grenadier), CustomGameOptions.GrenadierOn, true));

                    if (CustomGameOptions.WitchOn > 0)
                        ImpostorRoles.Add((typeof(Witch), CustomGameOptions.WitchOn, true));

                    if (CustomGameOptions.EscapistOn > 0)
                        ImpostorRoles.Add((typeof(Escapist), CustomGameOptions.EscapistOn, false));

                    if (CustomGameOptions.BomberOn > 0)
                        ImpostorRoles.Add((typeof(Bomber), CustomGameOptions.BomberOn, true));

                    if (CustomGameOptions.WarlockOn > 0)
                        ImpostorRoles.Add((typeof(Warlock), CustomGameOptions.WarlockOn, false));

                    if (CustomGameOptions.VenererOn > 0 && !FungleMap() && !CustomGameOptions.ColourblindComms)
                        ImpostorRoles.Add((typeof(Venerer), CustomGameOptions.VenererOn, true));

                    #endregion

                    #region Modifiers
                    
                    if (Check(CustomGameOptions.GiantOn))
                        Modifiers.Add((typeof(Giant), CustomGameOptions.GiantOn));
                    
                    if (Check(CustomGameOptions.MiniOn))
                        Modifiers.Add((typeof(Mini), CustomGameOptions.MiniOn));

                    if (Check(CustomGameOptions.BaitOn))
                        Modifiers.Add((typeof(Bait), CustomGameOptions.BaitOn));

                    if (Check(CustomGameOptions.DiseasedOn))
                        Modifiers.Add((typeof(Diseased), CustomGameOptions.DiseasedOn));

                    if (Check(CustomGameOptions.AftermathOn))
                        Modifiers.Add((typeof(Aftermath), CustomGameOptions.AftermathOn));

                    if (Check(CustomGameOptions.MultitaskerOn))
                        NonTaskerAbilities.Add((typeof(Multitasker), CustomGameOptions.MultitaskerOn));

                    if (Check(CustomGameOptions.FrostyOn))
                        Modifiers.Add((typeof(Frosty), CustomGameOptions.FrostyOn));

                    #endregion

                    #region Abilities
                    if (Check(CustomGameOptions.TiebreakerOn))
                        Abilities.Add((typeof(Tiebreaker), CustomGameOptions.TiebreakerOn));

                    if (Check(CustomGameOptions.FlashOn))
                        Abilities.Add((typeof(Flash), CustomGameOptions.FlashOn));
                    
                    if (Check(CustomGameOptions.TorchOn) && !FungleMap())
                        VisionAbilities.Add((typeof(Torch), CustomGameOptions.TorchOn));

                    if (Check(CustomGameOptions.ButtonBarryOn))
                        ButtonAbilities.Add((typeof(ButtonBarry), CustomGameOptions.ButtonBarryOn));
                    
                    if (Check(CustomGameOptions.DrunkOn))
                        Abilities.Add((typeof(Drunk), CustomGameOptions.DrunkOn));
                    
                    if (Check(CustomGameOptions.SpyOn))
                        Abilities.Add((typeof(Spy), CustomGameOptions.SpyOn));

                    if (Check(CustomGameOptions.SleuthOn))
                        Abilities.Add((typeof(Sleuth), CustomGameOptions.SleuthOn));

                    if (Check(CustomGameOptions.ParanoiacOn))
                        Abilities.Add((typeof(Paranoiac), CustomGameOptions.ParanoiacOn));

                    #endregion
                    #region Impostor Modifiers
                    if (Check(CustomGameOptions.DisperserOn))
                        ImpostorModifiers.Add((typeof(Disperser), CustomGameOptions.DisperserOn));

                    if (Check(CustomGameOptions.DoubleShotOn))
                        AssassinAbilityModifiers.Add((typeof(DoubleShot), CustomGameOptions.DoubleShotOn));

                    if (CustomGameOptions.UnderdogOn > 0)
                        ImpostorModifiers.Add((typeof(Underdog), CustomGameOptions.UnderdogOn));
                    #endregion
                    #region Assassin Ability
                    AssassinAbility.Add((typeof(Assassin), CustomRPC.SetAssassin, 100));
                    #endregion
                }
                GenEachRole(infected.ToList());
            }
        }
    }
}
