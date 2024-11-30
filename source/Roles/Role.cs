namespace TownOfSushi.Roles
{
    public abstract class Role
    {
        public static readonly Dictionary<byte, Role> RoleDictionary = new Dictionary<byte, Role>();
        public static readonly List<KeyValuePair<byte, RoleEnum>> RoleHistory = new List<KeyValuePair<byte, RoleEnum>>();
        protected internal DeathReasonEnum DeathReason { get; set; } = DeathReasonEnum.Alive;
        protected internal RoleAlignment RoleAlignment { get; set; } = RoleAlignment.None;
        public Func<string> StartText;
        public Func<string> TaskText;
        protected Role(PlayerControl player)
        {
            Player = player;
            RoleDictionary.Add(player.PlayerId, this);
        }

        public static IEnumerable<Role> AllRoles => RoleDictionary.Values.ToList();
        protected internal string Name { get; set; }
        protected internal string KilledBy { get; set; } = "";
        protected internal string AlignmentName { get; set; } = "";
        private PlayerControl _player { get; set; }
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

        protected float Scale { get; set; } = 1f;
        protected internal Color Color { get; set; }
        protected internal RoleEnum RoleType { get; set; }
        protected internal int TasksLeft => Player.Data.Tasks.ToArray().Count(x => !x.Complete);
        protected internal int TotalTasks => Player.Data.Tasks.Count;
        protected internal int Kills { get; set; } = 0;
        protected internal int CorrectKills { get; set; } = 0;
        protected internal bool Misfire { get; set; } = false;
        protected internal int CorrectVigilanteShot { get; set; } = 0;
        protected internal int IncorrectShots { get; set; } = 0;
        protected internal int CorrectShot { get; set; } = 0;
        protected internal int CorrectAssassinKills { get; set; } = 0;
        protected internal int IncorrectAssassinKills { get; set; } = 0;
        public bool Local => PlayerControl.LocalPlayer.PlayerId == Player.PlayerId;
        protected internal bool Hidden { get; set; } = false;
        protected internal Faction Faction { get; set; } = Faction.Crewmates;
        protected internal RoleAlignment Alignment { get; set; }
        public static uint NetId => PlayerControl.LocalPlayer.NetId;
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
            return DeadCriteria() || ImpostorCriteria() || VampireCriteria() || SelfCriteria() || RoleCriteria() || RomanticCriteria() || GuardianAngelCriteria() || Local;
        }
        internal virtual bool ColorCriteria()
        {
            return SelfCriteria() || DeadCriteria() || ImpostorCriteria() || VampireCriteria() || RoleCriteria() || RomanticCriteria() || GuardianAngelCriteria();
        }
        internal virtual bool DeadCriteria()
        {
            if (PlayerControl.LocalPlayer.Data.IsDead && TownOfSushi.DeadSeeRoles.Value) return true;
            return false;
        }
        internal virtual bool ImpostorCriteria()
        {
            if (Faction == Faction.Impostors && PlayerControl.LocalPlayer.Data.IsImpostor() &&
                CustomGameOptions.ImpostorSeeRoles) return true;
            return false;
        }
        internal virtual bool VampireCriteria()
        {
            if (RoleType == RoleEnum.Vampire && PlayerControl.LocalPlayer.Is(RoleEnum.Vampire)) return true;
            return false;
        }
        public List<KillButton> ExtraButtons = new List<KillButton>();
        internal virtual bool SelfCriteria()
        {
            return GetPlayerRole(PlayerControl.LocalPlayer) == this;
        }
        internal virtual bool RoleCriteria()
        {
            return PlayerControl.LocalPlayer.Is(AbilityEnum.Sleuth) && Ability.GetAbility<Sleuth>(PlayerControl.LocalPlayer).Reported.Contains(Player.PlayerId);
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
                    || (PlayerControl.LocalPlayer.Data.IsDead && !ga.Player.Data.IsDead)))
                {
                    PlayerName += "<color=#FFFFFFFF> [★]</color>";
                }
            }
            foreach (var role in GetRoles(RoleEnum.Romantic))
            {
                var rom = (Romantic) role;
                if (Player == rom.Beloved && ((Player == PlayerControl.LocalPlayer && CustomGameOptions.RomanticBelovedKnows)
                    || (PlayerControl.LocalPlayer.Data.IsDead && !rom.Player.Data.IsDead) || Player == rom.Player && rom.AlreadyPicked && Player == PlayerControl.LocalPlayer))
                {
                    PlayerName += "<color=#FF66CCFF>  [♥]</color>";
                }
            }
            foreach (var role in GetRoles(RoleEnum.Executioner))
            {
                var exe = (Executioner) role;
                if (Player == exe.target && PlayerControl.LocalPlayer.Data.IsDead && !exe.Player.Data.IsDead)
                {
                    PlayerName += "<color=#CCCCCCFF> [⦿]</color>";
                }
            }
            foreach (var role in GetRoles(RoleEnum.Arsonist))
            {
                var arsonist = (Arsonist) role;
                if (arsonist.DousedPlayers.Contains(Player.PlayerId) && !Player.Data.IsDead && PlayerControl.LocalPlayer.Data.IsDead && !arsonist.Player.Data.IsDead)
                {
                    PlayerName += "<color=#FF4D00FF> [♨]</color>";
                }
            }
            foreach (var role in GetRoles(RoleEnum.Plaguebearer))
            {
                var plaguebearer = (Plaguebearer) role;
                if (plaguebearer.InfectedPlayers.Contains(Player.PlayerId) && Player != plaguebearer.Player && !Player.Data.IsDead && PlayerControl.LocalPlayer.Data.IsDead && !plaguebearer.Player.Data.IsDead)
                {
                    PlayerName += "<color=#E6FFB3FF> [♨]</color>";
                }
            }

           if (Player.HasTasks())
            {
                if (PlayerControl.LocalPlayer.Data.IsDead && TownOfSushi.DeadSeeTasks.Value || Player == PlayerControl.LocalPlayer)
                {
                    PlayerName += $" ({TotalTasks - TasksLeft}/{TotalTasks})";
                }
            }


            foreach (PlayerControl p in PlayerControl.AllPlayerControls) 
            {
                //These lines are from TheOtherRoles
                if (p.cosmetics.colorBlindText != null && p.cosmetics.showColorBlindText && p.cosmetics.colorBlindText.gameObject.active) {
                    p.cosmetics.colorBlindText.transform.localPosition = new Vector3(0, -1f, 0f);
                }
                p.cosmetics.nameText.transform.parent.SetLocalZ(-0.0001f); 
            }
            
            if (player != null && (MeetingHud.Instance.state == MeetingHud.VoteStates.Results)) return PlayerName;

            if (!revealRole) return PlayerName;

            //Player.nameText().transform.localPosition = new Vector3(0f, 0.15f, -0.5f);

            if (Player.Data.IsDead) return $"{PlayerName}\n<size=70%>{Name} - {Player.DeathReason()}</size>";
            else return $"{PlayerName}\n<size=80%>{Name}</size>";
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
        public void RegenTask()
        {
            bool createTask;
            var hasFakeTasks = Player.HasTasks() ? "" : "\nFake tasks:";

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
                var ability = Ability.GetAbility(PlayerControl.LocalPlayer);
                task.transform.SetParent(Player.transform, false);
                task.Text = $"{ColorString}Role: {Name}\n{TaskText()}\nAlignment: {Player.AlignmentName()}\nAbility: {ability.Name}{hasFakeTasks}</color>";
                Player.myTasks.Insert(0, task);
                return;
            }
            var ability2 = Ability.GetAbility(PlayerControl.LocalPlayer);
            Player.myTasks.ToArray()[0].Cast<ImportantTextTask>().Text =
                $"{ColorString}Role: {Name}\n{TaskText()}\nAlignment: {Player.AlignmentName()}\nAbility: {ability2.Name}{hasFakeTasks}</color>";
        }
        public static T Gen<T>(Type type, PlayerControl player, CustomRPC rpc)
        {
            var role = (T)Activator.CreateInstance(type, new object[] { player });
            Rpc(rpc, player.PlayerId);
            return role;
        }
        public static T GenRole<T>(Type type, PlayerControl player)
        {
            var role = (T)Activator.CreateInstance(type, new object[] { player });
            Rpc(CustomRPC.SetRole, player.PlayerId, (string)type.FullName);
            System.Console.WriteLine($"{player.Data.DefaultOutfit.PlayerName} GETS THE ROLE {(string)type.FullName}");
            return role;
        }
        public static T GenModifier<T>(Type type, PlayerControl player)
        {
            var modifier = (T)Activator.CreateInstance(type, new object[] { player });

            Rpc(CustomRPC.SetModifier, player.PlayerId, (string)type.FullName);
            System.Console.WriteLine($"{player.Data.DefaultOutfit.PlayerName} GETS THE MODIFIER {(string)type.FullName}");
            return modifier;
        }
        public static T GenAbility<T>(Type type, PlayerControl player)
        {
            var ability = (T)Activator.CreateInstance(type, new object[] { player });

            Rpc(CustomRPC.SetAbility, player.PlayerId, (string)type.FullName);
            System.Console.WriteLine($"{player.Data.DefaultOutfit.PlayerName} GETS THE ABILITY {(string)type.FullName}");
            return ability;
        }
        public static T GenRole<T>(Type type, List<PlayerControl> players)
        {
            var player = players[Random.RandomRangeInt(0, players.Count)];

            var role = GenRole<T>(type, player);
            players.Remove(player);
            return role;
        }
        public static T GenModifier<T>(Type type, List<PlayerControl> players)
        {
            var player = players[Random.RandomRangeInt(0, players.Count)];

            var modifier = GenModifier<T>(type, player);
            players.Remove(player);
            return modifier;
        }
        public static T GenAbility<T>(Type type, List<PlayerControl> players)
        {
            var player = players[Random.RandomRangeInt(0, players.Count)];

            var ability = GenAbility<T>(type, player);
            players.Remove(player);
            return ability;
        }
        public static Role GetPlayerRole(PlayerControl player)
        {
            if (player == null)
                return null;

            if (RoleDictionary.TryGetValue(player.PlayerId, out var role))
                return role;

            return null;
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
        public static IEnumerable<Role> GetAlignment(RoleAlignment ra)
        {
            return AllRoles.Where(x => x.RoleAlignment == ra);
        }
        public static IEnumerable<Role> GetFaction(Faction faction)
        {
            return AllRoles.Where(x => x.Faction == faction);
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
                var modifier = Modifier.GetModifier(player);
                var ability = Ability.GetAbility(player);
                var hasFakeTasks = player.HasTasks() ? "" : "\nFake tasks:";

                if (modifier != null)
                {
                    var modTask = new GameObject(modifier.Name + "Task").AddComponent<ImportantTextTask>();
                    modTask.transform.SetParent(player.transform, false);
                    modTask.Text =
                        $"{modifier.ColorString}Modifier: {modifier.Name}\n{modifier.TaskText()}{hasFakeTasks}</color>";
                    player.myTasks.Insert(0, modTask);
                }
                if (ability != null)
                {
                    var modTask = new GameObject(ability.Name + "Task").AddComponent<ImportantTextTask>();
                    modTask.transform.SetParent(player.transform, false);
                    modTask.Text =
                        $"{ability.ColorString}Ability: {ability.Name}\n{ability.TaskText()}{hasFakeTasks}</color>";
                    player.myTasks.Insert(0, modTask);
                }

                if (role == null || role.Hidden) return;
                if (role.RoleType == RoleEnum.Amnesiac && role.Player != PlayerControl.LocalPlayer) return;
                var task = new GameObject(role.Name + "Task").AddComponent<ImportantTextTask>();
                task.transform.SetParent(player.transform, false);
                task.Text = $"{role.ColorString}Role: {role.Name}\n{role.TaskText()}\nAlignment: {player.AlignmentName()}{hasFakeTasks}</color>";
                player.myTasks.Insert(0, task);
            }
        }
        [HarmonyPatch(typeof(LobbyBehaviour), nameof(LobbyBehaviour.Start))]
        public static class LobbyBehaviour_Start
        {
            private static void Postfix(LobbyBehaviour __instance)
            {
                foreach (var role in AllRoles.Where(x => x.RoleType == RoleEnum.Snitch))
                {
                    ((Snitch)role).ImpArrows.DestroyAll();
                    ((Snitch)role).SnitchArrows.Values.DestroyAll();
                    ((Snitch)role).SnitchArrows.Clear();
                }
                foreach (var role in AllRoles.Where(x => x.RoleType == RoleEnum.Tracker))
                {
                    ((Tracker)role).TrackerArrows.Values.DestroyAll();
                    ((Tracker)role).TrackerArrows.Clear();
                }
                foreach (var role in AllRoles.Where(x => x.RoleType == RoleEnum.Amnesiac))
                {
                    ((Amnesiac)role).BodyArrows.Values.DestroyAll();
                    ((Amnesiac)role).BodyArrows.Clear();
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

                RoleDictionary.Clear();
                RoleHistory.Clear();
                Modifier.ModifierDictionary.Clear();
                Ability.AbilityDictionary.Clear();
            }
        }
        [HarmonyPatch(typeof(TranslationController), nameof(TranslationController.GetString), typeof(StringNames),
            typeof(Il2CppReferenceArray<Il2CppSystem.Object>))]
        public static class TranslationController_GetString
        {
            public static void Postfix(ref string __result, [HarmonyArgument(0)] StringNames name)
            {
                if (ExileController.Instance == null) return;
                switch (name)
                {
                    case StringNames.NoExileTie:
                    case StringNames.ExileTextPN:
                    case StringNames.ExileTextSN:
                    case StringNames.ExileTextPP:
                    case StringNames.ExileTextSP:
                        {
                            if (ExileController.Instance.initData.networkedPlayer == null) return;
                            var info = ExileController.Instance.initData.networkedPlayer;
                            var role = GetPlayerRole(info.Object);
                            if (role == null) return;
                            __result = $"{info.PlayerName} was The {role.Name}.";
                            return;
                        }
                }
            }
        }

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        public static class HudManager_Update
        {
            private static Vector3 oldScale = Vector3.zero;
            private static Vector3 oldPosition = Vector3.zero;
            private static void UpdateMeeting(MeetingHud __instance)
            {
                foreach (var player in __instance.playerStates)
                {
                    player.ColorBlindName.transform.localPosition = new Vector3(-0.93f, -0.2f, -0.1f);

                    var role = GetRole(player);
                    if (role != null && role.Criteria())
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
                            selfFlag || deadFlag || impostorFlag || romanticFlag || vampireFlag || roleFlag || gaFlag,
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
                if (MeetingHud.Instance != null) UpdateMeeting(MeetingHud.Instance);

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
                            bool vampireFlag = role.VampireCriteria();
                            bool roleFlag = role.RoleCriteria();
                            bool gaFlag = role.GuardianAngelCriteria();
                            bool romanticFlag = role.RomanticCriteria();
                            player.nameText().text = role.NameText(
                                selfFlag || deadFlag || role.Local,
                                selfFlag || deadFlag || impostorFlag || romanticFlag|| vampireFlag || roleFlag || gaFlag,
                                selfFlag || deadFlag
                             );

                            if (role.ColorCriteria())
                                player.nameText().color = role.Color;
                        }
                    }

                    if (player.Data != null && PlayerControl.LocalPlayer.Data.IsImpostor() && player.Data.IsImpostor()) continue;
                }
            }
        }
    }
}