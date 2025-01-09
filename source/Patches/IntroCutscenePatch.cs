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

                foreach (PlayerControl p in PlayerControl.AllPlayerControls) {
                    NetworkedPlayerInfo data = p.Data;
                    PoolablePlayer player = UnityEngine.Object.Instantiate<PoolablePlayer>(__instance.PlayerPrefab, DestroyableSingleton<HudManager>.Instance.transform);
                    playerPrefab = __instance.PlayerPrefab;
                    p.SetPlayerMaterialColors(player.cosmetics.currentBodySprite.BodySprite);
                    player.SetSkin(data.DefaultOutfit.SkinId, data.DefaultOutfit.ColorId);
                    player.cosmetics.SetHat(data.DefaultOutfit.HatId, data.DefaultOutfit.ColorId);
                   // PlayerControl.SetPetImage(data.DefaultOutfit.PetId, data.DefaultOutfit.ColorId, player.PetSlot);
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
                    BetterPolus.SpecimenVent = UnityEngine.Object.Instantiate<Vent>(adminVent);
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
        public static void AddNeutralIntroIcons(IntroCutscene __instance, ref  Il2CppSystem.Collections.Generic.List<PlayerControl> yourTeam) {
            // Intro solo teams
            if (PlayerControl.LocalPlayer.Is(Faction.Neutral)) 
            {
                var soloTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
                soloTeam.Add(PlayerControl.LocalPlayer);
                yourTeam = soloTeam;
            }
        }

        public static void AddNeutralIntro(IntroCutscene __instance, ref  Il2CppSystem.Collections.Generic.List<PlayerControl> yourTeam) 
        {
            if (PlayerControl.LocalPlayer.Is(Faction.Neutral)) 
            {
                var neutralColor = new Color32(76, 84, 78, 255);
                __instance.BackgroundBar.material.color = neutralColor;
                __instance.TeamTitle.text = "Neutral";
                __instance.TeamTitle.color = neutralColor;
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
                    PlayerControl.LocalPlayer.Data.Role.IntroSound = GetIntroSound(RoleTypes.GuardianAngel);
                }
            }
        }

        public static IEnumerator<WaitForSeconds> EndShowRole(IntroCutscene __instance) {
            yield return new WaitForSeconds(5f);
            __instance.YouAreText.gameObject.SetActive(false);
            __instance.RoleText.gameObject.SetActive(false);
            __instance.RoleBlurbText.gameObject.SetActive(false);
            __instance.ourCrewmate.gameObject.SetActive(false);
           
        }

        [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.CreatePlayer))]
        class CreatePlayerPatch {
            public static void Postfix(IntroCutscene __instance, bool impostorPositioning, ref PoolablePlayer __result) {
                if (impostorPositioning) __result.SetNameColor(Palette.ImpostorRed);
            }
        }


        [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.ShowRole))]
        class SetUpRoleTextPatch 
        {
            static int seed = 0;
            static public void SetRoleTexts(IntroCutscene __instance) {
                // Don't override the intro of the vanilla roles
                Role roleInfo = GetPlayerRole(PlayerControl.LocalPlayer);
                Modifier modifierInfo = GetModifier(PlayerControl.LocalPlayer);
                Ability abilityInfo = GetAbility(PlayerControl.LocalPlayer);

                __instance.RoleBlurbText.text = "";
                if (roleInfo != null) 
                {
                    __instance.RoleText.text = roleInfo.Name;
                    __instance.RoleText.color = roleInfo.Color;
                    __instance.YouAreText.color = roleInfo.Color;
                    __instance.RoleBlurbText.text = roleInfo.StartText();
                    __instance.RoleBlurbText.color = roleInfo.Color;
                }
                if (modifierInfo != null) 
                {
                    __instance.RoleBlurbText.text += ColorString(modifierInfo.Color, $"\n{modifierInfo.TaskText()}");
                }
                if (abilityInfo != null) 
                {
                    __instance.RoleBlurbText.text += ColorString(abilityInfo.Color, $"\n{abilityInfo.TaskText()}");
                }
            }
            public static bool Prefix(IntroCutscene __instance) {
                System.Random rnd = new System.Random((int)DateTime.Now.Ticks);
                seed = rnd.Next(5000);
                DestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(1f, new Action<float>((p) => {
                    SetRoleTexts(__instance);
                })));
                return true;
            }
        }

        [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.BeginCrewmate))]
        class BeginCrewmatePatch 
        {
            public static void Prefix(IntroCutscene __instance, ref  Il2CppSystem.Collections.Generic.List<PlayerControl> teamToDisplay) 
            {
                AddNeutralIntroIcons(__instance, ref teamToDisplay);
            }

            public static void Postfix(IntroCutscene __instance, ref  Il2CppSystem.Collections.Generic.List<PlayerControl> teamToDisplay) 
            {
                AddNeutralIntro(__instance, ref teamToDisplay);
            }
        }

        [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.BeginImpostor))]
        class BeginImpostorPatch {
            public static void Prefix(IntroCutscene __instance, ref  Il2CppSystem.Collections.Generic.List<PlayerControl> yourTeam) {
                AddNeutralIntroIcons(__instance, ref yourTeam);
            }

            public static void Postfix(IntroCutscene __instance, ref  Il2CppSystem.Collections.Generic.List<PlayerControl> yourTeam) {
                AddNeutralIntro(__instance, ref yourTeam);
            }
        }
        public static AudioClip GetIntroSound(RoleTypes roleType)
        {
            return RoleManager.Instance.AllRoles.Where((role) => role.Role == roleType).FirstOrDefault().IntroSound;
        }
    }
}