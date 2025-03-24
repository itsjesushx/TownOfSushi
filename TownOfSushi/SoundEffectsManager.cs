using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using Reactor.Utilities.Extensions;

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
            Assembly assembly = Assembly.GetExecutingAssembly();
            string[] resourceNames = assembly.GetManifestResourceNames();

            /* Old way of loading .raw files. Left here for reference -Gendelo
            foreach (string resourceName in resourceNames)
            {
                if (resourceName.Contains("TownOfSushi.Resources.SoundEffects.") && (resourceName.Contains(".raw") || resourceName.Contains(".ogg")))
                {
                    soundEffects.Add(resourceName, Helpers.loadAudioClipFromResources(resourceName));
                }
            }*/

            var resourceBundle = assembly.GetManifestResourceStream("TownOfSushi.Resources.SoundEffects.toraudio");
            var assetBundle = AssetBundle.LoadFromMemory(resourceBundle.ReadFully());
            foreach (var f in assetBundle.GetAllAssetNames()) {
                soundEffects.Add(f, assetBundle.LoadAsset<AudioClip>(f).DontUnload());
            }
            assetBundle.Unload(false);

        }

        public static AudioClip Get(string path)
        {
            // Convenience: As as SoundEffects are stored in the same folder, allow using just the name as well
            //if (!path.Contains(".")) path = "TownOfSushi.Resources.SoundEffects." + path + ".raw";
            path = "assets/audio/" + path.ToLower() + ".ogg";
            AudioClip returnValue;
            return soundEffects.TryGetValue(path, out returnValue) ? returnValue : null;
        }


        public static void Play(string path, float volume=0.8f, bool loop = false, bool musicChannel = false)
        {
            if (!MapOptions.enableSoundEffects) return;
            AudioClip clipToPlay = Get(path);
            Stop(path);
            if (Constants.ShouldPlaySfx() && clipToPlay != null) 
            {
                AudioSource source = SoundManager.Instance.PlaySound(clipToPlay, false, volume, audioMixer: musicChannel ? SoundManager.Instance.MusicChannel : null);
                source.loop = loop;
            }
        }
        public static void PlayAtPosition(string path, Vector2 position, float maxDuration = 15f, float range = 5f, bool loop = false) {
            if (!MapOptions.enableSoundEffects || !Constants.ShouldPlaySfx()) return;
            AudioClip clipToPlay = Get(path);

            AudioSource source = SoundManager.Instance.PlaySound(clipToPlay, false, 1f);
            source.loop = loop;
            HudManager.Instance.StartCoroutine(Effects.Lerp(maxDuration, new Action<float>((p) => 
            {
                if (source != null) 
                {
                    if (p == 1) 
                    {
                        source.Stop();
                    }
                    float distance, volume;
                    distance = Vector2.Distance(position, PlayerControl.LocalPlayer.GetTruePosition());
                    if (distance < range)
                        volume = (1f - distance / range);
                    else
                        volume = 0f;
                    source.volume = volume;
                }
            })));
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
