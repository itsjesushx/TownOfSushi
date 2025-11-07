using System.Collections;
using AmongUs.Data;
using Assets.InnerNet;
using BepInEx;
using BepInEx.Unity.IL2CPP.Utils;
using Il2CppInterop.Runtime.Attributes;
using Newtonsoft.Json.Linq;
using Reactor.Utilities;
using Twitch;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Action = System.Action;
using Version = SemanticVersioning.Version;

namespace TownOfSushi.Modules
{        
    // Thanks to The Other Roles for this patch https://github.com/TheOtherRolesAU/TheOtherRoles/blob/main/TheOtherRoles/Modules/ModUpdater.cs

    public class ModUpdateBehaviour : MonoBehaviour
    {
        public static bool showPopUp = true;
        public static bool updateInProgress;

        public static ModUpdateBehaviour Instance { get; private set; }
    //    public ModUpdateBehaviour(IntPtr ptr) : base(ptr) { }
        public class UpdateData
        {
            public string Content;
            public string Tag;
            public string TimeString;
            public JObject Request;
            public Version Version => Version.Parse(Tag);

            public UpdateData(JObject data) {
                Tag = data["tag_name"]?.ToString().TrimStart('v');
                Content = data["body"]?.ToString();
                TimeString = DateTime.FromBinary(((Il2CppSystem.DateTime)data["published_at"]).ToBinaryRaw()).ToString();
                Request = data;
            }

            public bool IsNewer(Version version) {
                if (!Version.TryParse(Tag, out var myVersion)) return false;
                return myVersion.BaseVersion() > version.BaseVersion();
            }
        }

        public UpdateData TOSUpdate;

        [HideFromIl2Cpp]
        public UpdateData RequiredUpdateData => TOSUpdate;

        public void Awake() {
            if (Instance) Destroy(this);
            Instance = this;

            SceneManager.add_sceneLoaded((System.Action<Scene, LoadSceneMode>)(OnSceneLoaded));
            this.StartCoroutine(CoCheckUpdates());

            foreach (var file in Directory.GetFiles(Paths.PluginPath, "*.old")) {
                File.Delete(file);
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            if (updateInProgress || scene.name != "MainMenu") return;
            if (RequiredUpdateData is null) {
                showPopUp = false;
                return;
            }

            var template = GameObject.Find("ExitGameButton");
            if (!template) return;

            var button = Instantiate(template, null);
            button.GetComponent<AspectPosition>().anchorPoint = new Vector2(0.458f, 0.124f);

            PassiveButton passiveButton = button.GetComponent<PassiveButton>();
            passiveButton.OnClick = new Button.ButtonClickedEvent();
            passiveButton.OnClick.AddListener((Action)(() => {
                this.StartCoroutine(CoUpdate());
                button.SetActive(false);
            }));

            var text = button.transform.GetComponentInChildren<TMPro.TMP_Text>();
            string t = "Update Town of Sushi";
            StartCoroutine(Effects.Lerp(0.1f, (System.Action<float>)(p => text.SetText(t))));
            passiveButton.OnMouseOut.AddListener((Action)(() => text.color = Color.cyan));
            passiveButton.OnMouseOver.AddListener((Action)(() => text.color = Color.cyan));

            var announcement = $"<size=150%>A new Town of Sushi update to {TOSUpdate.Tag} is available</size>\n{TOSUpdate.Content}";
            var mgr = FindObjectOfType<MainMenuManager>(true);            
            if (showPopUp) mgr.StartCoroutine(CoShowAnnouncement(announcement, shortTitle: "Town of Sushi Update", date: TOSUpdate.TimeString));
            showPopUp = false;
        }

        [HideFromIl2Cpp]
        public IEnumerator CoUpdate() 
        {
            updateInProgress = true;
            var updateName = "Town of Sushi";

            var popup = Instantiate(TwitchManager.Instance.TwitchPopup);
            popup.TextAreaTMP.fontSize *= 0.7f;
            popup.TextAreaTMP.enableAutoSizing = false;

            popup.Show();

            var button = popup.transform.GetChild(2).gameObject;
            button.SetActive(false);
            popup.TextAreaTMP.text = $"Updating {updateName}\nPlease wait...";

            var download = Task.Run(DownloadUpdate);
            while (!download.IsCompleted) yield return null;

            button.SetActive(true);
            popup.TextAreaTMP.text = download.Result ? $"{updateName}\nupdated successfully\nPlease restart the game." : "Update wasn't successful\nPlease use Town of Sushi Downloader\nto update manually.";
        }


        private static int announcementNumber = 501;
        [HideFromIl2Cpp]
        public static IEnumerator CoShowAnnouncement(string announcement, bool show = true, string shortTitle = "Town of Sushi Update", string title = "", string date = "") 
        {
            var mgr = FindObjectOfType<MainMenuManager>(true);
            var popUpTemplate = FindObjectOfType<AnnouncementPopUp>(true);
            if (popUpTemplate == null) {
                Logger<TownOfSushiPlugin>.Error("couldnt show credits, popUp is null");
                yield return null;
            }
            var popUp = Instantiate(popUpTemplate);

            popUp.gameObject.SetActive(true);

            Announcement creditsAnnouncement = new() 
            {
                Id = "TOSAnnouncement",
                Language = 0,
                Number = announcementNumber++,
                Title = title == "" ? "Town of Sushi Announcement" : title,
                ShortTitle = shortTitle,
                SubTitle = "",
                PinState = false,
                Date = date == "" ? DateTime.Now.Date.ToString() : date,
                Text = announcement,
            };
            mgr.StartCoroutine(Effects.Lerp(0.1f, new Action<float>((p) => {
                if (p == 1) {
                    var backup = DataManager.Player.Announcements.allAnnouncements;
                    DataManager.Player.Announcements.allAnnouncements = new();
                    popUp.Init(false);
                    DataManager.Player.Announcements.SetAnnouncements(new Announcement[] { creditsAnnouncement });
                    popUp.CreateAnnouncementList();
                    popUp.UpdateAnnouncementText(creditsAnnouncement.Number);
                    popUp.visibleAnnouncements[0].PassiveButton.OnClick.RemoveAllListeners();
                    DataManager.Player.Announcements.allAnnouncements = backup;
                }
            })));
        }

        [HideFromIl2Cpp]
        public static IEnumerator CoCheckUpdates() 
        {
            var TOSUpdateCheck = Task.Run(() => GetGithubUpdate("itsjesushx", "TownOfSushi"));
            while (!TOSUpdateCheck.IsCompleted) yield return null;
            if (TOSUpdateCheck.Result != null && TOSUpdateCheck.Result.IsNewer(Version.Parse(TownOfSushiPlugin.Version))) 
            {
                Instance.TOSUpdate = TOSUpdateCheck.Result;
            }
            Instance.OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
        }

        [HideFromIl2Cpp]
        public static async Task<UpdateData> GetGithubUpdate(string owner, string repo) 
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "Town of Sushi Updater");

            try {
                var req = await client.GetAsync($"https://api.github.com/repos/{owner}/{repo}/releases/latest", HttpCompletionOption.ResponseContentRead);

                if (!req.IsSuccessStatusCode) return null;

                var dataString = await req.Content.ReadAsStringAsync();
                JObject data = JObject.Parse(dataString);
                return new UpdateData(data);
            }
            catch (HttpRequestException) 
            {
                return null;
            }
        }


        [HideFromIl2Cpp]
        public async Task<bool> DownloadUpdate() {
            var data = TOSUpdate;

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "Town of Sushi Updater");

            JToken assets = data.Request["assets"];
            string downloadURI = "";
            for (JToken current = assets.First; current != null; current = current.Next) 
            {
                string browser_download_url = current["browser_download_url"]?.ToString();
#pragma warning disable CA1309 // Use ordinal string comparison
                if (browser_download_url != null && current["content_type"] != null && 
                current["content_type"].ToString().Equals("application/x-msdownload") &&
                        browser_download_url.EndsWith(".dll")) 
                {
                    downloadURI = browser_download_url;
                    break;
                }
#pragma warning restore CA1309 // Use ordinal string comparison
            }

            if (downloadURI.Length == 0) return false;

            var res = await client.GetAsync(downloadURI, HttpCompletionOption.ResponseContentRead);
            string filePath = Path.Combine(Paths.PluginPath, "TownOfSushi.dll");
            if (File.Exists(filePath + ".old")) File.Delete(filePath + ".old");
            if (File.Exists(filePath)) File.Move(filePath, filePath + ".old");

            await using var responseStream = await res.Content.ReadAsStreamAsync();
            await using var fileStream = File.Create(filePath);
            await responseStream.CopyToAsync(fileStream);

            return true;
        }
    }
}
