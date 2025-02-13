using Hazel;
using System;
using TownOfSushi.Utilities;
using UnityEngine;

namespace TownOfSushi.Objects 
{
    public class Bomb 
    {
        public GameObject bomb;
        public GameObject background;

        private static Sprite bombSprite;
        private static Sprite backgroundSprite;
        private static Sprite defuseSprite;
        public static bool canDefuse = false;

        public static Sprite GetBombSprite() 
        {
            if (bombSprite) return bombSprite;
            bombSprite = Helpers.LoadSpriteFromResources("TownOfSushi.Resources.Bomb.png", 300f);
            return bombSprite;
        }
        public static Sprite GetBackgroundSprite() 
        {
            if (backgroundSprite) return backgroundSprite;
            backgroundSprite = Helpers.LoadSpriteFromResources("TownOfSushi.Resources.BombBackground.png", 110f / Bomber.hearRange);
            return backgroundSprite;
        }

        public static Sprite GetDefuseSprite() 
        {
            if (defuseSprite) return defuseSprite;
            defuseSprite = Helpers.LoadSpriteFromResources("TownOfSushi.Resources.Bomb_Button_Defuse.png", 115f);
            return defuseSprite;
        }

        public Bomb(Vector2 p) 
        {
            bomb = new GameObject("Bomb") { layer = 11 };
            bomb.AddSubmergedComponent(SubmergedCompatibility.Classes.ElevatorMover);
            Vector3 position = new Vector3(p.x, p.y, p.y / 1000 + 0.001f); // just behind player
            bomb.transform.position = position;

            background = new GameObject("Background") { layer = 11 };
            background.transform.SetParent(bomb.transform);
            background.transform.localPosition = new Vector3(0, 0, -1f); // before player
            background.transform.position = position;

            var bombRenderer = bomb.AddComponent<SpriteRenderer>();
            bombRenderer.sprite = GetBombSprite();
            var backgroundRenderer = background.AddComponent<SpriteRenderer>();
            backgroundRenderer.sprite = GetBackgroundSprite();

            bomb.SetActive(false);
            background.SetActive(false);
            if (PlayerControl.LocalPlayer == Bomber.bomber) {
                bomb.SetActive(true);
            }
            Bomber.bomb = this;
            Color c = Color.white;
            Color g = Color.red;
            backgroundRenderer.color = Color.white;
            Bomber.isActive = false;

            FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(Bomber.bombActiveAfter, new Action<float>((x) => {
                if (x == 1f && this != null) {
                    bomb.SetActive(true);
                    background.SetActive(true);
                    SoundEffectsManager.PlayAtPosition("bombFuseBurning", p, Bomber.destructionTime, Bomber.hearRange, true);
                    Bomber.isActive = true;

                    FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(Bomber.destructionTime, new Action<float>((x) => { // can you feel the pain?
                        Color combinedColor = Mathf.Clamp01(x) * g + Mathf.Clamp01(1 - x) * c;
                        if (backgroundRenderer) backgroundRenderer.color = combinedColor;
                        if (x == 1f && this != null) 
                        {
                            Explode(this);
                        }
                    })));
                }
            })));

        }
        public static void Explode(Bomb b) 
        {
            if (b == null) return;
            if (Bomber.bomber != null) 
            {
                var position = b.bomb.transform.position;
                var distance = Vector2.Distance(position, PlayerControl.LocalPlayer.transform.position);  // every player only checks that for their own client (desynct with positions sucks)
                if (distance < Bomber.destructionRange && !PlayerControl.LocalPlayer.Data.IsDead) 
                {
                    Helpers.CheckMurderAttemptAndKill(Bomber.bomber, PlayerControl.LocalPlayer, false, false, true, true);
                    
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.ShareGhostInfo, Hazel.SendOption.Reliable, -1);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write((byte)GhostInfoTypes.DeathReasonAndKiller);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write((byte)DeadPlayer.CustomDeathReason.Bomb);
                    writer.Write(Bomber.bomber.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    GameHistory.OverrideDeathReasonAndKiller(PlayerControl.LocalPlayer, DeadPlayer.CustomDeathReason.Bomb, killer: Bomber.bomber);
                }
                SoundEffectsManager.PlayAtPosition("bombExplosion", position, range: Bomber.hearRange) ;
            }
            Bomber.ClearBomb();
            canDefuse = false;
            Bomber.isActive = false;
        }

        public static void Update() 
        {
            if (Bomber.bomb == null || !Bomber.isActive) 
            {
                canDefuse = false;
                return;
            }
            Bomber.bomb.background.transform.Rotate(Vector3.forward * 50 * Time.fixedDeltaTime);

            if (MeetingHud.Instance && Bomber.bomb != null) 
            {
                Bomber.ClearBomb();
            }

            if (Vector2.Distance(PlayerControl.LocalPlayer.GetTruePosition(), Bomber.bomb.bomb.transform.position) > 1f) canDefuse = false;
            else canDefuse = true;
        }

        public static void ClearBackgroundSprite() 
        {
            backgroundSprite = null;
        }
    }
}