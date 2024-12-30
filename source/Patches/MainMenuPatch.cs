using AmongUs.Data;
using Assets.InnerNet;
using static UnityEngine.UI.Button;

//Originally from StellarRoles (https://github.com/Mr-Fluuff/StellarRolesAU/blob/main/StellarRoles/Patches/MainMenuPatch.cs)
namespace TownOfSushi.Patches
{
    [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
    public class MainMenuPatch
    {
        private static AnnouncementPopUp popUp;
        private static void Prefix(MainMenuManager __instance)
        {
            #region DiscordButton
            GameObject template = GameObject.Find("ExitGameButton");
            GameObject template2 = GameObject.Find("CreditsButton");
            if (template == null || template2 == null) return;
            template.transform.localScale = new Vector3(0.42f, 0.84f, 0.84f);
            template.GetComponent<AspectPosition>().anchorPoint = new Vector2(0.625f, 0.5f);
            template.transform.FindChild("FontPlacer").transform.localScale = new Vector3(1.8f, 0.9f, 0.9f);
            template.transform.FindChild("FontPlacer").transform.localPosition = new Vector3(-1.1f, 0f, 0f);

            template2.transform.localScale = new Vector3(0.42f, 0.84f, 0.84f);
            template2.GetComponent<AspectPosition>().anchorPoint = new Vector2(0.378f, 0.5f);
            template2.transform.FindChild("FontPlacer").transform.localScale = new Vector3(1.8f, 0.9f, 0.9f);
            template2.transform.FindChild("FontPlacer").transform.localPosition = new Vector3(-1.1f, 0f, 0f);

            GameObject buttonDiscord = Object.Instantiate(template, template.transform.parent);
            buttonDiscord.transform.localScale = new Vector3(0.42f, 0.84f, 0.84f);
            buttonDiscord.GetComponent<AspectPosition>().anchorPoint = new Vector2(0.543f, 0.5f);

            TMPro.TMP_Text textDiscord = buttonDiscord.transform.GetComponentInChildren<TMPro.TMP_Text>();
            __instance.StartCoroutine(Effects.Lerp(0.5f, new Action<float>((p) =>
            {
                textDiscord.SetText("            TOS Discord");
            })));
            PassiveButton passiveButtonDiscord = buttonDiscord.GetComponent<PassiveButton>();
            passiveButtonDiscord.OnClick = new ButtonClickedEvent();
            passiveButtonDiscord.OnClick.AddListener((Action)(() => Application.OpenURL("https://discord.gg/hMfFbU9mgk")));

            #endregion DiscordButton

            #region TOS Credits
            // TOR credits button
            if (template == null) return;

            GameObject creditsButton = Object.Instantiate(template, template.transform.parent);
            creditsButton.transform.localScale = new Vector3(0.42f, 0.84f, 0.84f);
            creditsButton.GetComponent<AspectPosition>().anchorPoint = new Vector2(0.461f, 0.5f);

            TMPro.TMP_Text textCreditsButton = creditsButton.transform.GetComponentInChildren<TMPro.TMP_Text>();
            __instance.StartCoroutine(Effects.Lerp(0.5f, new Action<float>((p) =>
            {
                textCreditsButton.SetText("         TOS Credits");
            })));

            PassiveButton passiveCreditsButton = creditsButton.GetComponent<PassiveButton>();
            passiveCreditsButton.OnClick = new ButtonClickedEvent();
            passiveCreditsButton.OnClick.AddListener((Action)delegate
            {
                // do stuff
                if (popUp != null) Object.Destroy(popUp);
                AnnouncementPopUp popUpTemplate = Object.FindObjectOfType<AnnouncementPopUp>(true);
                if (popUpTemplate == null)
                {
                    TownOfSushi.Logger.LogError("Unable to show credits as `popUpTemplate` is unexpectedly null");
                    return;
                }
                popUp = Object.Instantiate(popUpTemplate);
                popUp.gameObject.SetActive(true);
                string creditsString = @$"<align=""center"">Special Thanks:
[https://github.com/eDonnes124/Town-Of-Us]TownOfUs-Reactivated[] - Original Codebase

";
                creditsString += $@"<size=60%> Other Credits & Resources:
[https://github.com/eDonnes124/Town-Of-Us]TownOfUs-Reactivated[] - Original Codebase
[https://github.com/SubmergedAmongUs/Lifeboat]Lifeboat[] - UsefulMethods class
[https://github.com/AlchlcDvl/TownOfUsReworked]TownOfUsReworked[] - Random Spawns option, part of the End Game Summary rework code, the Death Reason code, Some of the Vulture.cs code & more
[https://github.com/SubmergedAmongUs/Submerged]Alexejhero[] - For the Submerged map.
[https://github.com/Mr-Fluuff/StellarRolesAU/]StellarRoles[] - Main Menu Patch, End Game Summary code & Custom Message code.
[https://github.com/TheOtherRolesAU/TheOtherRoles]TheOtherRoles[] - For the inspiration of the Gambler, Tracker and Spy roles, as well as the Bait modifier. MOTD code, Flash Util, Code for the Main menu and ping tracker as well.
[https://github.com/SuperNewRoles/SuperNewRoles]SuperNewRoles[]  - For the Disable Lobby Music option.
[https://github.com/NuclearPowered/Reactor]Reactor[] - The framework of the mod
[https://github.com/BepInEx]BepInEx[] - For hooking game functions
[https://github.com/Woodi-dev/Among-Us-Sheriff-Mod]Among-Us-Sheriff-Mod[] - For the Vigilante role.
[https://github.com/Woodi-dev/Among-Us-Love-Couple-Mod]Among-Us-Love-Couple-Mod[] - For the inspiration of Lovers modifier.
[https://github.com/NotHunter101/ExtraRolesAmongUs]ExtraRolesAmongUs[] - For the Engineer & Medic roles.
[https://github.com/Hardel-DW/TooManyRolesMods]TooManyRolesMods[] - For the Investigator & Time Lord roles.
[https://github.com/tomozbot/TorchMod]TorchMod[] - For the inspiration of the Nightowl modifier.
[https://twitch.tv/PhasmoFireGod]PhasmoFireGod[] , lotty and [https://www.instagram.com/ixean.studio]Ophidian[] - Button Art.
[https://www.twitch.tv/5uppp]5up[] and the Submarine Team - For the inspiration of the Grenadier role.
[https://github.com/OhMyGuus]Guus[] - For support for the old Among Us versions (v2021.11.9.5 and v2021.12.15).
[https://github.com/MyDragonBreath]MyDragonBreath[] - For Submerged Compatibility, the Trapper role, the Aftermath modifier and support for the new Among Us versions (v2022.6.21 & v2023.6.13).
[https://github.com/itsTheNumberH/Town-Of-H]ItsTheNumberH[] - For the code used for Blind, Bait, Poisoner and partially for Tracker, as well as other bug fixes.
[https://github.com/ruiner189/Town-Of-Us-Redux]Ruiner[] - For lovers changed into a modifier and Task Tracking.
[https://www.twitch.tv/termboii]Term[] - For creating Transporter, Medium, Blackmailer, Plaguebearer, Sleuth, Multitasker and porting v2.5.0 to the new Among Us version (v2021.12.15).
[https://github.com/Brybry16/BetterPolus]BryBry16[] - For the code used for Better Polus.
[https://github.com/DorCoMaNdO/Reactor-Essentials]Essentials[] - For created custom game options.";
                creditsString += "</align>";

                Announcement creditsAnnouncement = new()
                {
                    Id = "TOSCredits",
                    Language = 0,
                    Number = 502,
                    Title = "TownOfSushi Credits & Resources",
                    ShortTitle = "TownOfSushi Credits",
                    SubTitle = "",
                    PinState = false,
                    Date = "03.07.2023",
                    Text = creditsString,
                };

                __instance.StartCoroutine(Effects.Lerp(0.01f, new Action<float>((p) =>
                {
                    if (p == 1)
                    {
                        Il2CppSystem.Collections.Generic.List<Announcement> backup = DataManager.Player.Announcements.allAnnouncements;
                        DataManager.Player.Announcements.allAnnouncements = new();
                        popUp.Init(false);
                        DataManager.Player.Announcements.SetAnnouncements(new Announcement[] { creditsAnnouncement });
                        popUp.CreateAnnouncementList();
                        popUp.UpdateAnnouncementText(creditsAnnouncement.Number);
                        popUp.visibleAnnouncements[0].PassiveButton.OnClick.RemoveAllListeners();
                        DataManager.Player.Announcements.allAnnouncements = backup;
                    }
                })));
            });
            #endregion TOS Credits
        }
    }
}