namespace TownOfSushi.Roles
{
    public abstract class Role
    {
        public static readonly Dictionary<byte, Role> RoleDictionary = new Dictionary<byte, Role>();
        public static readonly List<KeyValuePair<byte, RoleEnum>> RoleHistory = new List<KeyValuePair<byte, RoleEnum>>();
        protected internal RoleAlignment RoleAlignment { get; set; } = RoleAlignment.None;
        public Func<string> StartText;
        public Func<string> TaskText;
        protected internal string LoreText { get; set; }
        protected internal string RoleInfo { get; set; }
        public static IEnumerable<Role> AllRoles => RoleDictionary.Values.ToList();
        protected internal string Name { get; set; }
        protected internal string KilledBy { get; set; } = "";
        protected internal string AlignmentText { get; set; } = "";
        private PlayerControl _player { get; set; }
        protected Role(PlayerControl player)
        {
            Player = player;
            RoleDictionary.Add(player.PlayerId, this);
        }
        public PlayerControl Player
        {
            get => _player;
            set
            {
                if (_player != null) _player.nameText().color = Color.white;

                _player = value;
                PlayerName = value.Data.PlayerName;
            }
        }
        protected internal Color Color { get; set; }
        protected internal RoleEnum RoleType { get; set; }
        protected internal int TasksLeft => Player.Data.Tasks.ToArray().Count(x => !x.Complete);
        protected internal int TotalTasks => Player.Data.Tasks.Count;
        protected internal int Kills { get; set; } = 0;
        protected internal int CorrectKills { get; set; } = 0;
        protected internal int CorrectDeputyShot { get; set; } = 0;
        protected internal bool Misfired { get; set; } = false;
        protected internal int CorrectVigilanteShot { get; set; } = 0;
        protected internal int IncorrectShots { get; set; } = 0;
        protected internal int CorrectShot { get; set; } = 0;
        protected internal int CorrectAssassinKills { get; set; } = 0;
        public bool Local => PlayerControl.LocalPlayer.PlayerId == Player.PlayerId;
        protected internal Faction Faction { get; set; } = Faction.Crewmates;
        public string PlayerName { get; set; }
        public string ColorString => "<color=#" + Color.ToHtmlStringRGBA() + ">";
        private bool Equals(Role other)
        {
            return Equals(Player, other.Player) && RoleType == other.RoleType;
        }
        public void AddToRoleHistory(RoleEnum role)
        {
            RoleHistory.Add(KeyValuePair.Create(_player.PlayerId, role));
        }
        public void RemoveFromRoleHistory(RoleEnum role)
        {
            RoleHistory.Remove(KeyValuePair.Create(_player.PlayerId, role));
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(Role)) return false;
            return Equals((Role)obj);
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Player, (int)RoleType);
        }
        internal virtual bool Criteria()
        {
            return DeadCriteria() || ImpostorCriteria() || SelfCriteria() || VampireCriteria() || RoleCriteria() || RomanticCriteria() || GuardianAngelCriteria() || Local;
        }
        internal virtual bool ColorCriteria()
        {
            return SelfCriteria() || DeadCriteria() || ColourImpostorCriteria() || VampireCriteria() || RoleCriteria() || RomanticCriteria() || GuardianAngelCriteria();
        }

        public static bool NeutralEvilWin()
        {
            return JesterWin || ExecutionerWin || VultureWin || DoomsayerWin;
        }
        internal virtual bool VampireCriteria()
        {
            if (RoleType == RoleEnum.Vampire && PlayerControl.LocalPlayer.Is(RoleEnum.Vampire)) return true;
            return false;
        }

        internal virtual bool DeadCriteria()
        {
            if (IsDead() && TownOfSushi.DeadSeeRoles.Value) return true;
            return false;
        }
        internal virtual bool ColourImpostorCriteria()
        {
            if (Faction == Faction.Impostors && PlayerControl.LocalPlayer.Data.IsImpostor()) return true;
            return false;
        }

        internal virtual bool ImpostorCriteria()
        {
            if (Faction == Faction.Impostors && PlayerControl.LocalPlayer.Data.IsImpostor() &&
                CustomGameOptions.ImpostorSeeRoles) return true;
            return false;
        }
        public List<KillButton> ExtraButtons = new List<KillButton>();
        internal virtual bool SelfCriteria()
        {
            return GetPlayerRole(PlayerControl.LocalPlayer) == this;
        }
        internal virtual bool RoleCriteria()
        {
            return PlayerControl.LocalPlayer.Is(AbilityEnum.Sleuth) && GetAbility<Sleuth>(PlayerControl.LocalPlayer).Reported.Contains(Player.PlayerId);
        }
        internal virtual bool GuardianAngelCriteria()
        {
            return PlayerControl.LocalPlayer.Is(RoleEnum.GuardianAngel) && CustomGameOptions.GAKnowsTargetRole && Player == GetRole<GuardianAngel>(PlayerControl.LocalPlayer).target;
        }
        internal virtual bool RomanticCriteria()
        {
            return PlayerControl.LocalPlayer.Is(RoleEnum.Romantic) && CustomGameOptions.RomanticKnowsBelovedRole && Player == GetRole<Romantic>(PlayerControl.LocalPlayer).Beloved;
        }
        protected virtual string NameText(bool revealTasks, bool revealRole, bool revealModifier, PlayerVoteArea player = null)
        {
            if (CamouflageUnCamouflagePatch.IsCamouflaged && player == null) return "";
            if (Player == null) return "";
            String PlayerName = Player.GetDefaultOutfit().PlayerName;

            foreach (var role in GetRoles(RoleEnum.GuardianAngel))
            {
                var ga = (GuardianAngel) role;
                if (Player == ga.target && ((Player == PlayerControl.LocalPlayer && CustomGameOptions.GATargetKnows)
                    || (IsDead() && !ga.Player.Data.IsDead)))
                {
                    PlayerName += "<color=#FFFFFFFF> [★]</color>";
                }
            }
            foreach (var role in GetRoles(RoleEnum.Romantic))
            {
                var rom = (Romantic) role;
                if (Player == rom.Beloved && ((Player == PlayerControl.LocalPlayer && CustomGameOptions.RomanticBelovedKnows)
                    || (IsDead() && !rom.Player.Data.IsDead) || Player == rom.Player && rom.AlreadyPicked && Player == PlayerControl.LocalPlayer))
                {
                    PlayerName += "<color=#FF66CCFF>  [♥]</color>";
                }
            }
            foreach (var role in GetRoles(RoleEnum.Executioner))
            {
                var exe = (Executioner) role;
                if (Player == exe.target && IsDead() && !exe.Player.Data.IsDead)
                {
                    PlayerName += "<color=#CCCCCCFF> [⦿]</color>";
                }
            }
            foreach (var role in GetRoles(RoleEnum.Arsonist))
            {
                var arsonist = (Arsonist) role;
                if (arsonist.DousedPlayers.Contains(Player.PlayerId) && IsDead() && !arsonist.Player.Data.IsDead)
                {
                    PlayerName += "<color=#FF4D00FF> [♨]</color>";
                }
            }
            foreach (var role in GetRoles(RoleEnum.Plaguebearer))
            {
                var plaguebearer = (Plaguebearer) role;
                if (plaguebearer.InfectedPlayers.Contains(Player.PlayerId) && IsDead() && !plaguebearer.Player.Data.IsDead)
                {
                    PlayerName += "<color=#E6FFB3FF> [♨]</color>";
                }
            }

            foreach (var role in GetRoles(RoleEnum.Vulture))
            {
                var vulture = (Vulture) role;
                if (IsDead() && !vulture.Player.Data.IsDead && vulture.Player == Player)
                {
                    PlayerName += $" ({vulture.BodiesRemainingToWin()})";
                }
            }

           if (Player.HasTasks())
           {
                if (IsDead() && TownOfSushi.DeadSeeTasks.Value || Player == PlayerControl.LocalPlayer && Meeting() && TownOfSushi.ShowTasks.Value)
                {
                    PlayerName += $" ({TotalTasks - TasksLeft}/{TotalTasks})";
                }
                if (HUDManager().TaskPanel != null) 
                {
                    TMPro.TextMeshPro tabText = HUDManager().TaskPanel.tab.transform.FindChild("TabText_TMP").GetComponent<TMPro.TextMeshPro>();
                    tabText.SetText($"<color=#FAD934FF>Tasks ({TotalTasks - TasksLeft}/{TotalTasks})</color>");
                }
            }

            if (HUDManager().TaskPanel != null && !Player.HasTasks()) 
            {
                var role = GetPlayerRole(Player);
                TMPro.TextMeshPro tabText = HUDManager().TaskPanel.tab.transform.FindChild("TabText_TMP").GetComponent<TMPro.TextMeshPro>();
                tabText.SetText(ColorString(role.Color, "Fake Tasks"));
            }

            foreach (PlayerControl p in PlayerControl.AllPlayerControls) 
            {
                if (p.cosmetics.colorBlindText != null && p.cosmetics.showColorBlindText && p.cosmetics.colorBlindText.gameObject.active) {
                    p.cosmetics.colorBlindText.transform.localPosition = new Vector3(0, -1f, 0f);
                }
                p.cosmetics.nameText.transform.parent.SetLocalZ(-0.0001f); 
            }
            
            if (player != null && (Meeting().state == MeetingHud.VoteStates.Results)) return PlayerName;

            if (!revealRole) return PlayerName;

            if (Meeting()) return $"{PlayerName}\n<size=75%>{Name}</size>";
            else return $"<size=75%>{Name}</size>\n{PlayerName}</color>";
        }

        public static bool operator ==(Role a, Role b)
        {
            if (a is null && b is null) return true;
            if (a is null || b is null) return false;
            return a.RoleType == b.RoleType && a.Player.PlayerId == b.Player.PlayerId;
        }

        public static bool operator !=(Role a, Role b)
        {
            return !(a == b);
        }
        public void ReDoTaskText()
        {
            bool createTask;
            try
            {
                var firstText = Player.myTasks.ToArray()[0].Cast<ImportantTextTask>();
                createTask = !firstText.Text.Contains("Role:");
            }
            catch (InvalidCastException)
            {
                createTask = true;
            }

            if (createTask)
            {
                var task = new GameObject(Name + "Task").AddComponent<ImportantTextTask>();
                task.transform.SetParent(Player.transform, false);
                task.Text = $"{ColorString}Role: {Name} \n{TaskText()}\nAlignment: {Player.AlignmentText()}</color>";
                Player.myTasks.Insert(0, task);

                return;
            }
            Player.myTasks.ToArray()[0].Cast<ImportantTextTask>().Text =
                $"{ColorString}Role: {Name} \n{TaskText()}\nAlignment: {Player.AlignmentText()}</color>";
        }
        public static T Gen<T>(Type type, PlayerControl player, CustomRPC rpc)
        {
            var role = (T)Activator.CreateInstance(type, new object[] { player });
            StartRPC(rpc, player.PlayerId);
            return role;
        }
        public static T GenRole<T>(Type type, PlayerControl player)
        {
            var role = (T)Activator.CreateInstance(type, new object[] { player });
            StartRPC(CustomRPC.SetRole, player.PlayerId, (string)type.FullName);
            return role;
        }
        public static T GenRole<T>(Type type, List<PlayerControl> players)
        {
            var player = players[Random.RandomRangeInt(0, players.Count)];

            var role = GenRole<T>(type, player);
            players.Remove(player);
            return role;
        }
        public static Role GetPlayerRole(PlayerControl player)
        {
            if (player == null)
                return null;

            if (RoleDictionary.TryGetValue(player.PlayerId, out var role))
                return role;

            return null;
        }
        public static IEnumerable<Role> GetFactions(Faction faction)
        {
            return AllRoles.Where(x => x.Faction == faction);
        }
        public static T GetRole<T>(PlayerControl player) where T : Role
        {
            return GetPlayerRole(player) as T;
        }
        public static Role GetRole(PlayerVoteArea area)
        {
            var player = PlayerControl.AllPlayerControls.ToArray()
                .FirstOrDefault(x => x.PlayerId == area.TargetPlayerId);
            return player == null ? null : GetPlayerRole(player);
        }
        public static IEnumerable<Role> GetRoles(RoleEnum roletype)
        {
            return AllRoles.Where(x => x.RoleType == roletype);
        }
        public static Role GetRoleValue(RoleEnum roleEnum)
        {
            foreach (var role in AllRoles)
            {
                if (role.RoleType == roleEnum)
                    return role;
            }
            return null;
        }
        public static T GetRoleValue<T>(RoleEnum roleEnum) where T : Role => GetRoleValue(roleEnum) as T;

        [HarmonyPatch(typeof(PlayerControl._CoSetTasks_d__141), nameof(PlayerControl._CoSetTasks_d__141.MoveNext))]
        public static class PlayerControl_SetTasks
        {
            public static void Postfix(PlayerControl._CoSetTasks_d__141 __instance)
            {
                if (__instance == null) return;
                var player = __instance.__4__this;
                var role = GetPlayerRole(player);
                var modifier = GetModifier(player);
                var ability = GetAbility(player);

                if (modifier != null)
                {
                    var modTask = new GameObject(modifier.Name + "Task").AddComponent<ImportantTextTask>();
                    modTask.transform.SetParent(player.transform, false);
                    modTask.Text =
                        $"{modifier.ColorString}Modifier: {modifier.Name}: \n{modifier.TaskText()}</color>";
                    player.myTasks.Insert(0, modTask);
                }
                if (ability != null)
                {
                    var modTask = new GameObject(ability.Name + "Task").AddComponent<ImportantTextTask>();
                    modTask.transform.SetParent(player.transform, false);
                    modTask.Text =
                        $"{ability.ColorString}Ability: {ability.Name}: \n{ability.TaskText()}</color>";
                    player.myTasks.Insert(0, modTask);
                }

                if (role == null) return;
                if (role.RoleType == RoleEnum.Amnesiac && role.Player != PlayerControl.LocalPlayer) return;
                var task = new GameObject(role.Name + "Task").AddComponent<ImportantTextTask>();
                task.transform.SetParent(player.transform, false);
                task.Text = $"{role.ColorString}Role: {role.Name} \n{role.TaskText()}\nAlignment: {player.AlignmentText()}</color>";
                player.myTasks.Insert(0, task);
            }
        }
        
        [HarmonyPatch(typeof(LobbyBehaviour), nameof(LobbyBehaviour.Start))]
        public static class LobbyBehaviour_Start
        {
            private static void Postfix(LobbyBehaviour __instance)
            {
                foreach (var role in AllRoles.Where(x => x.RoleType == RoleEnum.Tracker))
                {
                    ((Tracker)role).TrackerArrows.Values.DestroyAll();
                    ((Tracker)role).TrackerArrows.Clear();
                }
                foreach (var role in AllRoles.Where(x => x.RoleType == RoleEnum.Vulture))
                {
                    ((Vulture)role).BodyArrows.Values.DestroyAll();
                    ((Vulture)role).BodyArrows.Clear();
                }
                foreach (var role in AllRoles.Where(x => x.RoleType == RoleEnum.Medium))
                {
                    ((Medium)role).MediatedPlayers.Values.DestroyAll();
                    ((Medium)role).MediatedPlayers.Clear();
                }
                foreach (var role in AllRoles.Where(x => x.RoleType == RoleEnum.Mystic))
                {
                    ((Mystic)role).BodyArrows.Values.DestroyAll();
                    ((Mystic)role).BodyArrows.Clear();
                }
                foreach (var role in AllRoles.Where(x => x.RoleType == RoleEnum.BountyHunter))
                {
                    ((BountyHunter)role).PreyArrow.Destroy();
                }

                RoleDictionary.Clear();
                RoleHistory.Clear();
                ModifierDictionary.Clear();
                AbilityDictionary.Clear();
            }
        }
       [HarmonyPatch(typeof(TranslationController), nameof(TranslationController.GetString), typeof(StringNames),
            typeof(Il2CppReferenceArray<Il2CppSystem.Object>))]
        public static class TranslationController_GetString
        {
            public static void Postfix(ref string __result, [HarmonyArgument(0)] StringNames name)
            {
                if (ExiledInstance() == null) return;
                switch (name)
                {
                    case StringNames.NoExileTie:
                    case StringNames.ExileTextPN:
                    case StringNames.ExileTextSN:
                    case StringNames.ExileTextPP:
                    case StringNames.ExileTextSP:
                        {
                            if (ExiledInstance().initData.networkedPlayer == null) return;
                            var info = ExiledInstance().initData.networkedPlayer;
                            var role = GetPlayerRole(info.Object);
                            if (role == null) return;
                            __result = $"{info.PlayerName} was The {role.Name}.";
                            return;
                        }
                }
            }
        }

        public static bool NobodyWins;
        public static bool VampireWins;
        public static bool CrewmatesWin;
        public static bool ImpostorsWin;
        public static bool HitmanWin;
        public static bool GlitchWin;
        public static bool JuggernautWin;
        public static bool AgentWin;
        public static bool WerewolfWin;
        public static bool PestilenceWin;
        public static bool PlaguebearerWin;
        public static bool ArsonistWin;
        public static bool SerialKillerWin;
        public static bool JesterWin;
        public static bool ExecutionerWin;
        public static bool DoomsayerWin;
        public static bool VultureWin;
        public static void NobodyWinsFunc()
        {
            NobodyWins = true;
        }
        internal static bool NobodyEndCriteria(LogicGameFlowNormal __instance)
        {
            bool CheckNoImpsNoCrews()
            {
                var alives = PlayerControl.AllPlayerControls.ToArray()
                    .Where(x => !x.Data.IsDead && !x.Data.Disconnected).ToList();
                if (alives.Count == 0) return false;
                var flag = alives.All(x =>
                {
                    var role = GetPlayerRole(x);
                    if (role == null) return false;
                    var flag2 = role.RoleAlignment == RoleAlignment.NeutralEvil || role.RoleAlignment == RoleAlignment.NeutralBenign;

                    return flag2;
                });

                return flag;
            }

            if (CheckNoImpsNoCrews())
            {
                StartRPC(CustomRPC.NobodyWins);
                NobodyWinsFunc();
                EndGame(GameOverReason.ImpostorDisconnect);
                return false;
            }
            return true;
        }

        [HarmonyPatch]
        public static class ShipStatus_KMPKPPGPNIH
        {
            [HarmonyPatch(typeof(LogicGameFlowNormal), nameof(LogicGameFlowNormal.CheckEndCriteria))]
            public static bool Prefix(LogicGameFlowNormal __instance)
            {
                if (IsHideNSeek()) return true;
                if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started) return false;
                if (PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && !x.Data.Disconnected).ToList().Count <= 2) return false;
                if (!AmongUsClient.Instance.AmHost) return false;
                if (Ship().Systems != null)
                {
                    if (Ship().Systems.ContainsKey(SystemTypes.LifeSupp))
                    {
                        var lifeSuppSystemType = Ship().Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>();
                        if (lifeSuppSystemType.Countdown < 0f)
                        {
                            ImpostorsWin = true;
                            StartRPC(CustomRPC.ImpostorWin);
                            EndGame();
                            return false;
                        }
                    }

                    if (Ship().Systems.ContainsKey(SystemTypes.Laboratory))
                    {
                        var reactorSystemType = Ship().Systems[SystemTypes.Laboratory].Cast<ReactorSystemType>();
                        if (reactorSystemType.Countdown < 0f)
                        {
                            ImpostorsWin = true;
                            StartRPC(CustomRPC.ImpostorWin);
                            EndGame();
                            return false;
                        }
                    }

                    if (Ship().Systems.ContainsKey(SystemTypes.Reactor))
                    {
                        var reactorSystemType = Ship().Systems[SystemTypes.Reactor].Cast<ICriticalSabotage>();
                        if (reactorSystemType.Countdown < 0f)
                        {
                            ImpostorsWin = true;
                            StartRPC(CustomRPC.ImpostorWin);
                            EndGame();
                            return false;
                        }
                    }
                }

                if (GameData.Instance.TotalTasks <= GameData.Instance.CompletedTasks)
                {
                    var crews = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Crewmates) && !x.Data.Disconnected).ToList();
                    if (crews.Count != 0)
                    {
                        CrewmatesWin = true;
                        StartRPC(CustomRPC.CrewmateWin);
                        EndGame(GameOverReason.HumansByVote);
                        return false;
                    }
                }

                if (!NobodyEndCriteria(__instance)) return false;

                return false;
            }
        }

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        public static class HudManager_Update
        {
            private static void UpdateMeeting(MeetingHud __instance)
            {
                foreach (var player in __instance.playerStates)
                {
                    player.ColorBlindName.transform.localPosition = new Vector3(-0.93f, -0.2f, -0.1f);

                    var role = GetRole(player);
                    if (role != null)
                    {
                        bool selfFlag = role.SelfCriteria();
                        bool deadFlag = role.DeadCriteria();
                        bool impostorFlag = role.ImpostorCriteria();
                        bool vampireFlag = role.VampireCriteria();
                        bool roleFlag = role.RoleCriteria();
                        bool gaFlag = role.GuardianAngelCriteria();
                        bool romanticFlag = role.RomanticCriteria();
                        player.NameText.text = role.NameText(
                            selfFlag || deadFlag || role.Local,
                            selfFlag || deadFlag || impostorFlag  || vampireFlag || romanticFlag || roleFlag || gaFlag,
                            selfFlag || deadFlag,
                            player
                        );
                        if(role.ColorCriteria())
                            player.NameText.color = role.Color;
                    }
                    else
                    {
                        try
                        {
                            player.NameText.text = role.Player.GetDefaultOutfit().PlayerName;
                        }
                        catch
                        {
                        }
                    }
                }
            }

            [HarmonyPriority(Priority.First)]
            private static void Postfix(HudManager __instance)
            {
                if (Meeting() != null) UpdateMeeting(Meeting());

                if (PlayerControl.AllPlayerControls.Count <= 1) return;
                if (PlayerControl.LocalPlayer == null) return;
                if (PlayerControl.LocalPlayer.Data == null) return;

                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (!(player.Data != null && player.Data.IsImpostor() && PlayerControl.LocalPlayer.Data.IsImpostor()))
                    {
                        player.nameText().text = player.name;
                        player.nameText().color = Color.white;
                    }

                    var role = GetPlayerRole(player);
                    if (role != null)
                    {
                        if (role.Criteria())
                        {
                            bool selfFlag = role.SelfCriteria();
                            bool deadFlag = role.DeadCriteria();
                            bool impostorFlag = role.ImpostorCriteria();
                            bool roleFlag = role.RoleCriteria();
                            bool vampireFlag = role.VampireCriteria();
                            bool gaFlag = role.GuardianAngelCriteria();
                            bool romanticFlag = role.RomanticCriteria();
                            player.nameText().text = role.NameText(
                                selfFlag || deadFlag || role.Local,
                                selfFlag || deadFlag || impostorFlag || vampireFlag || romanticFlag || roleFlag || gaFlag,
                                selfFlag || deadFlag
                             );

                            if (role.ColorCriteria())
                                player.nameText().color = role.Color;
                        }
                    }
                }
            }
        }
    }
}