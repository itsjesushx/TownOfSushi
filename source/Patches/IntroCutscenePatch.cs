namespace TownOfSushi.Patches
{
    [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.OnDestroy))]
    class IntroCutsceneOnDestroyPatch
    {
        public static PoolablePlayer playerPrefab;
        public static Vector3 bottomLeft;
        public static void Prefix(IntroCutscene __instance) 
        {
            // Generate and initialize player icons
            if (PlayerControl.LocalPlayer != null && DestroyableSingleton<HudManager>.Instance != null) 
            {
                float aspect = Camera.main.aspect;
                float safeOrthographicSize = CameraSafeArea.GetSafeOrthographicSize(Camera.main);
                float xpos = 1.75f - safeOrthographicSize * aspect * 1.70f;
                float ypos = 0.15f - safeOrthographicSize * 1.7f;
                bottomLeft = new Vector3(xpos / 2, ypos/2, -61f);

                foreach (PlayerControl p in PlayerControl.AllPlayerControls) 
                {
                    NetworkedPlayerInfo data = p.Data;
                    PoolablePlayer player = Object.Instantiate<PoolablePlayer>(__instance.PlayerPrefab, DestroyableSingleton<HudManager>.Instance.transform);
                    playerPrefab = __instance.PlayerPrefab;
                    p.SetPlayerMaterialColors(player.cosmetics.currentBodySprite.BodySprite);
                    player.SetSkin(data.DefaultOutfit.SkinId, data.DefaultOutfit.ColorId);
                    player.cosmetics.SetHat(data.DefaultOutfit.HatId, data.DefaultOutfit.ColorId);
                    player.cosmetics.nameText.text = data.PlayerName;
                    player.SetFlipX(true);
                    playerIcons[p.PlayerId] = player;
                    player.gameObject.SetActive(false);
                    player.transform.localPosition = bottomLeft;
                    player.transform.localScale = Vector3.one * 0.4f;
                    player.gameObject.SetActive(false);
                }
                if (GameOptionsManager.Instance.currentNormalGameOptions.MapId == 2 && CustomGameOptions.VentImprovements) 
                {
                    var list = GameObject.FindObjectsOfType<Vent>().ToList();
                    var adminVent = list.FirstOrDefault(x => x.gameObject.name == "AdminVent");
                    var bathroomVent = list.FirstOrDefault(x => x.gameObject.name == "BathroomVent");
                    BetterPolus.SpecimenVent = Object.Instantiate<Vent>(adminVent);
                    BetterPolus.SpecimenVent.gameObject.AddSubmergedComponent(SubmergedCompatibility.Classes.ElevatorMover);
                    BetterPolus.SpecimenVent.transform.position = new Vector3(36.55068f, -21.5168f, -0.0215168f);
                    BetterPolus.SpecimenVent.Left = adminVent;
                    BetterPolus.SpecimenVent.Right = bathroomVent;
                    BetterPolus.SpecimenVent.Center = null;
                    BetterPolus.SpecimenVent.Id = ShipStatus.Instance.AllVents.Select(x => x.Id).Max() + 1;
                    var allVentsList = ShipStatus.Instance.AllVents.ToList();
                    allVentsList.Add(BetterPolus.SpecimenVent);
                    ShipStatus.Instance.AllVents = allVentsList.ToArray();
                    BetterPolus.SpecimenVent.gameObject.SetActive(true);
                    BetterPolus.SpecimenVent.name = "newVent_" + BetterPolus.SpecimenVent.Id;

                    adminVent.Center = BetterPolus.SpecimenVent;
                    bathroomVent.Center = BetterPolus.SpecimenVent;
                }                
            }
        }
    }

    [HarmonyPatch]
    class IntroPatch
    {
        public static AudioClip GetIntroSound(RoleTypes roleType)
        {
            return RoleManager.Instance.AllRoles.Where((role) => role.Role == roleType).FirstOrDefault().IntroSound;
        }
        public static void SetupIntroTeamIcons(IntroCutscene __instance, ref Il2CppSystem.Collections.Generic.List<PlayerControl> yourTeam) 
        {    
            if (GameOptionsManager.Instance.currentGameMode == GameModes.Normal) 
            {
                if (PlayerControl.LocalPlayer.Is(Faction.Neutral)) 
                {
                    foreach (var role in GetRoles(RoleEnum.GuardianAngel))
                    {
                        var guardianAngel = (GuardianAngel)role;
                        if (guardianAngel.target != null)
                        {
                            var target = guardianAngel.target;
                            var player = PlayerControl.AllPlayerControls.ToArray().FirstOrDefault(x => x.PlayerId == target.PlayerId);
                            var soloTeam2 = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
                            soloTeam2.Add(player);
                            if (player != null)
                            {
                                yourTeam = soloTeam2;
                            }
                        }
                    }
                    // Intro solo teams
                    var soloTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
                    soloTeam.Add(PlayerControl.LocalPlayer);
                    yourTeam = soloTeam;

                    if (PlayerControl.LocalPlayer.Is(RoleAlignment.NeutralKilling))
                    {
                        PlayerControl.LocalPlayer.Data.Role.IntroSound = GetIntroSound(RoleTypes.Shapeshifter);
                    }
                    if (PlayerControl.LocalPlayer.Is(RoleAlignment.NeutralEvil))
                    {
                        PlayerControl.LocalPlayer.Data.Role.IntroSound = GetIntroSound(RoleTypes.Phantom);
                    }
                    if (PlayerControl.LocalPlayer.Is(RoleAlignment.NeutralBenign))
                    {
                        PlayerControl.LocalPlayer.Data.Role.IntroSound = GetIntroSound(RoleTypes.Noisemaker);
                    }
                }
            }
        }

        public static void SetupIntroTeam(IntroCutscene __instance, ref Il2CppSystem.Collections.Generic.List<PlayerControl> yourTeam) 
        {
            if (GameOptionsManager.Instance.currentGameMode == GameModes.Normal) 
            {
                if (PlayerControl.LocalPlayer.Is(Faction.Neutral) && !PlayerControl.LocalPlayer.Is(RoleEnum.GuardianAngel))
                {
                    var neutralColor = new Color32(76, 84, 78, 255);
                    __instance.ImpostorText.text = "Work alone to achieve your goal";
                    __instance.BackgroundBar.material.color = neutralColor;
                    __instance.TeamTitle.text = "Neutral";
                    __instance.TeamTitle.color = neutralColor;
                }
                if (PlayerControl.LocalPlayer.Is(Faction.Neutral) && PlayerControl.LocalPlayer.Is(RoleEnum.GuardianAngel))
                {
                    var neutralColor = new Color32(76, 84, 78, 255);
                    __instance.ImpostorText.text = $"Protect {GetRole<GuardianAngel>(PlayerControl.LocalPlayer).target.name} with your life";
                    __instance.BackgroundBar.material.color = neutralColor;
                    __instance.TeamTitle.text = "Neutral";
                    __instance.TeamTitle.color = neutralColor;
                }
                else if (PlayerControl.LocalPlayer.Is(Faction.Crewmates))
                {
                    __instance.ImpostorText.text = "Find and vote out the <color=#FF0000>Killers</color>";
                    __instance.TeamTitle.color = Palette.CrewmateBlue;
                }
            }
        }

        [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.ShowRole))]
        class ShowRolePatch
        {
            public static void Postfix(IntroCutscene __instance) 
            {
                // Don't override the intro of the vanilla roles
                Role roleInfo = GetPlayerRole(PlayerControl.LocalPlayer);
                Modifier modifierInfo = GetModifier(PlayerControl.LocalPlayer);
                Ability abilityInfo = GetAbility(PlayerControl.LocalPlayer);               

                Color color = new Color(__instance.YouAreText.color.r, __instance.YouAreText.color.g, __instance.YouAreText.color.b, 0f);
                __instance.StartCoroutine(Effects.Lerp(0.5f, new Action<float>((t) => 
                {
                    if (roleInfo != null) 
                    {
                        __instance.RoleText.text = roleInfo.Name;
                        __instance.RoleBlurbText.text = roleInfo.StartText();
                        color = roleInfo.Color;
                    }
                    if (modifierInfo != null) 
                    {
                        __instance.RoleBlurbText.text += ColorString(modifierInfo.Color, $"\n{modifierInfo.TaskText()}");
                    }
                    if (abilityInfo != null) 
                    {
                        __instance.RoleBlurbText.text += ColorString(abilityInfo.Color, $"\n{abilityInfo.TaskText()}");
                    }

                    color.a = t;
                    __instance.YouAreText.color = color;
                    __instance.RoleText.color = color;
                    __instance.RoleBlurbText.color = color;
                })));
            }
        }

        [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.BeginCrewmate))]
        class BeginCrewmatePatch
        {
            public static void Prefix(IntroCutscene __instance, ref Il2CppSystem.Collections.Generic.List<PlayerControl> teamToDisplay) 
            {
                SetupIntroTeamIcons(__instance, ref teamToDisplay);
            }

            public static void Postfix(IntroCutscene __instance, ref Il2CppSystem.Collections.Generic.List<PlayerControl> teamToDisplay) 
            {
                SetupIntroTeam(__instance, ref teamToDisplay);
            }
        }

        [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.BeginImpostor))]
        class BeginImpostorPatch
        {
            public static void Prefix(IntroCutscene __instance, ref Il2CppSystem.Collections.Generic.List<PlayerControl> yourTeam) 
            {
                SetupIntroTeamIcons(__instance, ref yourTeam);
            }

            public static void Postfix(IntroCutscene __instance, ref Il2CppSystem.Collections.Generic.List<PlayerControl> yourTeam) 
            {
                SetupIntroTeam(__instance, ref yourTeam);
            }
        }
    }
}