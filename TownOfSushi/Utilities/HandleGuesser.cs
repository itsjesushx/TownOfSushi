using System.Collections.Generic;
using UnityEngine;

namespace TownOfSushi.Utilities 
{
    public static class HandleGuesser 
    {
        private static Sprite targetSprite;
        public static bool hasMultipleShotsPerMeeting = false;
        public static bool killsThroughShield = true;
        public static bool evilGuesserCanGuessSpy = true;
        public static bool guesserCantGuessSnitch = false;
        public static int tasksToUnlock = Mathf.RoundToInt(CustomOptionHolder.GuesserCrewGuesserNumberOfTasks.GetFloat());

        public static Sprite GetTargetSprite() 
        {
            if (targetSprite) return targetSprite;
            targetSprite = Helpers.LoadSpriteFromResources("TownOfSushi.Resources.TargetIcon.png", 150f);
            return targetSprite;
        }

        public static bool IsGuesser(byte playerId) 
        {
            return Guesser.IsGuesser(playerId);
        }

        public static void Clear(byte playerId) 
        {
            Guesser.Clear(playerId);
        }

        public static int RemainingShots(byte playerId, bool shoot = false) 
        {
            return Guesser.RemainingShots(playerId, shoot);
        }

        public static void ClearAndReload() 
        {
            Guesser.ClearAndReload();
            guesserCantGuessSnitch = CustomOptionHolder.GuesserCantGuessSnitchIfTaksDone.GetBool();
            hasMultipleShotsPerMeeting = CustomOptionHolder.GuesserHasMultipleShotsPerMeeting.GetBool();
            killsThroughShield = CustomOptionHolder.GuesserKillsThroughShield.GetBool();
            evilGuesserCanGuessSpy = CustomOptionHolder.GuesserEvilCanKillSpy.GetBool();
            tasksToUnlock = Mathf.RoundToInt(CustomOptionHolder.GuesserCrewGuesserNumberOfTasks.GetFloat());
        }
    }
    class Guesser
    {        
        public static List<Guesser> guessers = new List<Guesser>();
        public static Color color = new Color32(255, 255, 0, byte.MaxValue);
        public PlayerControl guesser = null;
        public int shots = Mathf.RoundToInt(CustomOptionHolder.GuesserNumberOfShots.GetFloat());
        public int tasksToUnlock = Mathf.RoundToInt(CustomOptionHolder.GuesserCrewGuesserNumberOfTasks.GetFloat());
        public Guesser(PlayerControl player) 
        {
            guesser = player;
            guessers.Add(this);
        }

        public static int RemainingShots(byte playerId, bool shoot = false) 
        {
            var g = guessers.FindLast(x => x.guesser.PlayerId == playerId);
            if (g == null) return 0;
            if (shoot) g.shots--;
            return g.shots;
        }

        public static void Clear(byte playerId) 
        {
            var g = guessers.FindLast(x => x.guesser.PlayerId == playerId);
            if (g == null) return;
            g.guesser = null;
            g.shots = Mathf.RoundToInt(CustomOptionHolder.GuesserNumberOfShots.GetFloat());
            g.tasksToUnlock = Mathf.RoundToInt(CustomOptionHolder.GuesserCrewGuesserNumberOfTasks.GetFloat());

            guessers.Remove(g);
        }

        public static void ClearAndReload() 
        {
            guessers = new List<Guesser>();
        }

        public static bool IsGuesser(byte playerId) 
        {
            return guessers.FindAll(x => x.guesser.PlayerId == playerId).Count > 0;
        }
    }
}
