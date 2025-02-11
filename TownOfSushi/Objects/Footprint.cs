using InnerNet;
using Reactor.Utilities.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using TownOfSushi.Utilities;
using UnityEngine;
using static TownOfSushi.TownOfSushi;

namespace TownOfSushi.Objects 
{
    public class FootprintHolder : MonoBehaviour {
        static FootprintHolder() => ClassInjector.RegisterTypeInIl2Cpp<FootprintHolder>();

        public FootprintHolder(IntPtr ptr) : base(ptr) { }

        private static FootprintHolder _instance;
        public static FootprintHolder Instance {
            get => _instance ? _instance : _instance = new GameObject("FootprintHolder").AddComponent<FootprintHolder>();
            set => _instance = value;

        }

        private static Sprite _footprintSprite;
        private static Sprite FootprintSprite => _footprintSprite ??= Helpers.LoadSpriteFromResources("TownOfSushi.Resources.Footprint.png", 600f);

        private static bool AnonymousFootprints => TownOfSushi.Detective.anonymousFootprints;
        private static float FootprintDuration => TownOfSushi.Detective.footprintDuration;

        private class Footprint {
            public GameObject GameObject;
            public Transform Transform;
            public SpriteRenderer Renderer;
            public PlayerControl Owner;
            public NetworkedPlayerInfo Data;
            public float Lifetime;

            public Footprint() {
                GameObject = new("Footprint") { layer = 8 };
                Transform = GameObject.transform;
                Renderer = GameObject.AddComponent<SpriteRenderer>();
                Renderer.sprite = FootprintSprite;
                Renderer.color = Color.clear;
                GameObject.AddSubmergedComponent(SubmergedCompatibility.Classes.ElevatorMover);
            }
        }



        private readonly ConcurrentBag<Footprint> _pool = new();
        private readonly List<Footprint> _activeFootprints = new();
        private readonly List<Footprint> _toRemove = new();

        [HideFromIl2Cpp]
        public void MakeFootprint(PlayerControl player) {
            if (!_pool.TryTake(out var print)) {
                print = new();
            }

            print.Lifetime = FootprintDuration;

            var pos = player.transform.position;
            pos.z = pos.y / 1000f + 0.001f;
            print.Transform.SetPositionAndRotation(pos, Quaternion.EulerRotation(0, 0, UnityEngine.Random.Range(0.0f, 360.0f)));
            print.GameObject.SetActive(true);
            print.Owner = player;
            print.Data = player.Data;
            _activeFootprints.Add(print);
        }

        private static float updateDt = 0.10f;

        private void Start() {
            InvokeRepeating(nameof(FootprintUpdate), updateDt, updateDt);
        }

        private void FootprintUpdate() {
            var dt = updateDt;
            _toRemove.Clear();
            foreach (var activeFootprint in _activeFootprints) {
                var p = activeFootprint.Lifetime / FootprintDuration;

                if (activeFootprint.Lifetime <= 0) {
                    _toRemove.Add(activeFootprint);
                    continue;
                }

                Color color;
                if (AnonymousFootprints || Camouflager.camouflageTimer > 0 || Helpers.MushroomSabotageActive()) {
                    color = Palette.PlayerColors[6];
                } else if (activeFootprint.Owner == Morphling.morphling && Morphling.morphTimer > 0 && Morphling.morphTarget && Morphling.morphTarget.Data != null) {
                    color = Palette.PlayerColors[Morphling.morphTarget.Data.DefaultOutfit.ColorId];
                } 
                else if (activeFootprint.Owner == Glitch.Player && Glitch.MimicTimer > 0 && Glitch.MimicTarget && Glitch.MimicTarget.Data != null) {
                    color = Palette.PlayerColors[Glitch.MimicTarget.Data.DefaultOutfit.ColorId];
                }
                    else {
                    color = Palette.PlayerColors[activeFootprint.Data.DefaultOutfit.ColorId];
                }

                color.a = Math.Clamp(p, 0f, 1f);
                activeFootprint.Renderer.color = color;

                activeFootprint.Lifetime -= dt;
            }

            foreach (var footprint in _toRemove) {
                footprint.GameObject.SetActive(false);
                _activeFootprints.Remove(footprint);
                _pool.Add(footprint);
            }
        }

        private void OnDestroy() {
            Instance = null;
        }
    }
}
