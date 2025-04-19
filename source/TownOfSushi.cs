using BepInEx;
using BepInEx.Configuration;
using Reactor;
using Reactor.Networking.Attributes;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using TownOfSushi.Modules.CustomHats;
using System.IO;
using TownOfSushi.Modules.CustomColors;

namespace TownOfSushi
{
    [BepInPlugin(Id, "TownOfSushi", VersionString)]
    [BepInDependency(ReactorPlugin.Id)]
    [BepInDependency(SUBMERGED_GUID, BepInDependency.DependencyFlags.SoftDependency)]
    [ReactorModFlags(Reactor.Networking.ModFlags.RequireOnAllClients)]
    public class TownOfSushi : BasePlugin
    {
        public const string Id = "me.itsjesushx.townofsushi";
        public const string VersionString = "2.7.0";
        public static Version Version = Version.Parse(VersionString);
        public static bool IsMCI => IL2CPPChainloader.Instance.Plugins.TryGetValue("dragonbreath.au.mci", out _);
        public static bool MCILoaded => IsMCI && AmongUsClient.Instance.NetworkMode == NetworkModes.LocalGame;
        public static bool LevelImpLoaded => IL2CPPChainloader.Instance.Plugins.TryGetValue("com.DigiWorm.LevelImposter", out _);
        internal static BepInEx.Logging.ManualLogSource Logger;
        public Harmony Harmony { get; } = new Harmony(Id);
        public static int optionsPage = 2;
        public static TownOfSushi Instance;
        public static AssetLoader bundledAssets;
        public static Sprite JanitorClean;
        public static Sprite Footprint;
        public static Sprite SampleSprite;
        public static Sprite MorphSprite;
        public static Sprite Arrow;
        public static Sprite MineSprite;
        public static Sprite SwoopSprite;
        public static Sprite StalkSprite;
        public static Sprite MaulSprite;
        public static Sprite ButtonSprite;
        public static Sprite DisperseSprite;
        public static Sprite DragSprite;
        public static Sprite DropSprite;
        public static Sprite PlantSprite;
        public static Sprite DetonateSprite;
        public static Sprite InJailSprite;
        public static Sprite BlackmailSprite;
        public static Sprite ExecuteSprite;
        public static Sprite BlackmailLetterSprite;
        public static Sprite BlackmailOverlaySprite;
        public static Sprite LighterSprite;
        public static Sprite DarkerSprite;
        public static Sprite StabSprite;
        public static Sprite ExamineSprite;
        public static Sprite EscapeSprite;
        public static Sprite MarkSprite;
        public static Sprite ImitateSelectSprite;
        public static Sprite ImitateDeselectSprite;
        public static Sprite SwapperSwitch;
        public static Sprite SwapperSwitchDisabled;
        public static Sprite NoAbilitySprite;
        public static Sprite CamouflageSprite;
        public static Sprite CamoSprintSprite;
        public static Sprite CamoSprintFreezeSprite;
        public static Sprite HackSprite;
        public static Sprite MimicSprite;
        public static Sprite LockSprite;
        public static Sprite TargetIcon;
        private static DLoadImage _iCallLoadImage;

        public static ConfigEntry<bool> DeadSeeGhosts { get; set; }
        public static ConfigEntry<bool> DeadSeeRoles { get; set; }
        public static ConfigEntry<bool> DeadSeeTasks { get; set; }
        public static ConfigEntry<bool> EnableSoundEffects { get; set; }
        public static ConfigEntry<bool> DisableLevels { get; set; }
        public static ConfigEntry<bool> DisableNameplates { get; set; }
        public static ConfigEntry<bool> ShowTasks { get; set; }
        public static ConfigEntry<bool> DeadSeeVotes { get; set; }
        public static ConfigEntry<bool> DisableLobbyMusic { get; set; }
        public static string RuntimeLocation;
        public override void Load()
        {
            RuntimeLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetAssembly(typeof(TownOfSushi)).Location);
            //ReactorCredits.Register<TownOfSushi>(ReactorCredits.AlwaysShow);
            System.Console.WriteLine("000.000.000.000/000000000000000000");
            Logger = Log;
            Instance = this;
            CustomOptionHolder.Load();

            bundledAssets = new();
            JanitorClean = CreateSprite("TownOfSushi.Resources.Janitor.png");
            Footprint = CreateSprite("TownOfSushi.Resources.Footprint.png");
            SampleSprite = CreateSprite("TownOfSushi.Resources.Sample.png");
            MorphSprite = CreateSprite("TownOfSushi.Resources.Morph.png");
            Arrow = CreateSprite("TownOfSushi.Resources.Arrow.png");
            MineSprite = CreateSprite("TownOfSushi.Resources.Mine.png");
            SwoopSprite = CreateSprite("TownOfSushi.Resources.Swoop.png");
            SwapperSwitch = CreateSprite("TownOfSushi.Resources.SwapperSwitch.png");
            SwapperSwitchDisabled = CreateSprite("TownOfSushi.Resources.SwapperSwitchDisabled.png");
            ButtonSprite = CreateSprite("TownOfSushi.Resources.Button.png");
            DisperseSprite = CreateSprite("TownOfSushi.Resources.Disperse.png");
            DragSprite = CreateSprite("TownOfSushi.Resources.Drag.png");
            DropSprite = CreateSprite("TownOfSushi.Resources.Drop.png");
            MaulSprite = CreateSprite("TownOfSushi.Resources.Maul.png");
            StalkSprite = CreateSprite("TownOfSushi.Resources.Stalk.png");
            PlantSprite = CreateSprite("TownOfSushi.Resources.Plant.png");
            DetonateSprite = CreateSprite("TownOfSushi.Resources.Detonate.png");
            BlackmailSprite = CreateSprite("TownOfSushi.Resources.Blackmail.png");
            BlackmailLetterSprite = CreateSprite("TownOfSushi.Resources.BlackmailLetter.png");
            ExecuteSprite = CreateSprite("TownOfSushi.Resources.Execute.png");
            BlackmailOverlaySprite = CreateSprite("TownOfSushi.Resources.BlackmailOverlay.png");
            LighterSprite = CreateSprite("TownOfSushi.Resources.Lighter.png");
            DarkerSprite = CreateSprite("TownOfSushi.Resources.Darker.png");
            StabSprite = CreateSprite("TownOfSushi.Resources.Stab.png");
            InJailSprite = CreateSprite("TownOfSushi.Resources.InJail.png");
            ExamineSprite = CreateSprite("TownOfSushi.Resources.Examine.png");
            EscapeSprite = CreateSprite("TownOfSushi.Resources.Recall.png");
            MarkSprite = CreateSprite("TownOfSushi.Resources.Mark.png");
            ImitateSelectSprite = CreateSprite("TownOfSushi.Resources.ImitateSelect.png");
            ImitateDeselectSprite = CreateSprite("TownOfSushi.Resources.ImitateDeselect.png");
            NoAbilitySprite = CreateSprite("TownOfSushi.Resources.NoAbility.png");
            CamouflageSprite = CreateSprite("TownOfSushi.Resources.CamoButton.png");
            CamoSprintSprite = CreateSprite("TownOfSushi.Resources.CamoSprint.png");
            CamoSprintFreezeSprite = CreateSprite("TownOfSushi.Resources.CamoSprintFreeze.png");
            HackSprite = CreateSprite("TownOfSushi.Resources.Hack.png");
            MimicSprite = CreateSprite("TownOfSushi.Resources.Mimic.png");
            LockSprite = CreateSprite("TownOfSushi.Resources.Lock.png");
            TargetIcon = CreateSprite("TownOfSushi.Resources.TargetIcon.png", 150f);

            PalettePatch.Load();
            CustomHatManager.LoadHats();
            ClassInjector.RegisterTypeInIl2Cpp<ColorBehaviour>();
            Harmony.PatchAll();
            Initialize();
            ServerManager.DefaultRegions = new Il2CppReferenceArray<IRegionInfo>(new IRegionInfo[0]);
            
            DeadSeeGhosts = Config.Bind("Settings", "Dead See Other Ghosts", true, "Whether you see other dead player's ghosts while your dead");
            DeadSeeRoles = Config.Bind("Settings", "Dead See Roles", true, "Whether you see other dead player's role while your dead");
            DeadSeeTasks = Config.Bind("Settings", "Dead See Tasks", true, "Whether you see other dead player's task while you're dead");
            ShowTasks = Config.Bind("Settings", "See Tasks", true, "Whether you see  your own tasks while you are alive");
            DeadSeeVotes = Config.Bind("Settings", "Dead See Votes", true, "Whether you see other player's vote while your dead");
            DisableLobbyMusic = Config.Bind("Settings", "Disable Lobby Music", true, "Whether you hear the lobby music");
            DisableLevels = Config.Bind("Settings", "Disable Levels", true, "Whether you see the levels");
            DisableNameplates = Config.Bind("Settings", "Disable Nameplates", true, "Whether you see the nameplates");
            EnableSoundEffects = Config.Bind("Settings", "Enable Sound Effects", true, "Wether you hear the custom role effects");
        }

        public static Sprite CreateSprite(string name, float pixelsPerUnit = 100f)
        {
            var pivot = new Vector2(0.5f, 0.5f);
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var tex = AmongUsExtensions.CreateEmptyTexture();
            var imageStream = assembly.GetManifestResourceStream(name);
            var img = imageStream.ReadFully();
            LoadImage(tex, img, true);
            tex.DontDestroy();
            var sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), pivot, pixelsPerUnit);
            sprite.DontDestroy();
            return sprite;
        }

        public static void LoadImage(Texture2D tex, byte[] data, bool markNonReadable)
        {
            _iCallLoadImage ??= IL2CPP.ResolveICall<DLoadImage>("UnityEngine.ImageConversion::LoadImage");
            var il2CPPArray = (Il2CppStructArray<byte>) data;
            _iCallLoadImage.Invoke(tex.Pointer, il2CPPArray.Pointer, markNonReadable);
        }
        private delegate bool DLoadImage(IntPtr tex, IntPtr data, bool markNonReadable);
    }
}
