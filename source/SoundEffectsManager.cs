namespace TownOfSushi
{
    // Class to preload all audio/sound effects that are contained in the embedded resources.
    // The effects are made available through the soundEffects Dict / the get and the play methods.
    public static class SoundEffectsManager
    {
        private static Dictionary<string, AudioClip> soundEffects = new();

        public static void Load()
        {
            soundEffects = new Dictionary<string, AudioClip>();
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            string[] resourceNames = assembly.GetManifestResourceNames();
            foreach (string resourceName in resourceNames)
            {
                if (resourceName.Contains("TownOfSushi.Resources.SoundEffects.") && resourceName.Contains(".raw"))
                {
                    soundEffects.Add(resourceName, LoadAudioClipFromResources(resourceName));
                }
            }
        }

        public static AudioClip Get(string path)
        {
            // Convenience: As as SoundEffects are stored in the same folder, allow using just the name as well
            if (!path.Contains(".")) path = "TownOfSushi.Resources.SoundEffects." + path + ".raw";
            AudioClip returnValue;
            return soundEffects.TryGetValue(path, out returnValue) ? returnValue : null;
        }


        public static void Play(string path, float volume = 1f, bool loop = false)
        {
            if (!TownOfSushi.EnableSoundEffects.Value) return;
            AudioClip clipToPlay = Get(path);
            Stop(path);
            if (Constants.ShouldPlaySfx() && clipToPlay != null) 
            {
                AudioSource source = SoundManager.Instance.PlaySound(clipToPlay, false, volume);
                source.loop = loop;
            }
        }

        public static void Stop(string path) 
        {
            var soundToStop = Get(path);
            if (soundToStop != null)
                if (Constants.ShouldPlaySfx()) SoundManager.Instance.StopSound(soundToStop);
        }

        public static void StopAll() 
        {
            if (soundEffects == null) return;
            foreach (var path in soundEffects.Keys) Stop(path);        
        }
    }
}