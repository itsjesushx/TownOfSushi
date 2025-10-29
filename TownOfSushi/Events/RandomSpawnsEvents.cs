using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using TownOfSushi.Options;
using UnityEngine;

namespace TownOfSushi.Events
{
    public static class RandomSpawnsEvents
    {
        [RegisterEvent]
        public static void RoundStartEventHandler(RoundStartEvent @event)
        {
            if (!OptionGroupSingleton<TownOfSushiMapOptions>.Instance.EnableRandomSpawns)
                return;

            SetRandomSpawns(player => !player.HasDied());
        }

        [RegisterEvent]
        public static void RoundStartEvent(RoundStartEvent @event)
        {
            if (!OptionGroupSingleton<TownOfSushiMapOptions>.Instance.EnableRandomSpawns || !@event.TriggeredByIntro)
                return;

            SetRandomSpawns(player => !(player.HasDied() || player.HasModifier<LazyModifier>()));
        }

        private static readonly System.Random rnd = new System.Random((int)DateTime.Now.Ticks);
        private static readonly Dictionary<int, List<Vector3>> SpawnPoints = new()
        {
            [0] = new()
            {
                new(-2.2f, 2.2f, 0f), new(0.7f, 2.2f, 0f), new(-2.2f, -0.2f, 0f), new(0.7f, -0.2f, 0f),
                new(10.0f, 3.0f, 0f), new(9.0f, 1.0f, 0f), new(6.5f, -3.5f, 0f), new(11.5f, -3.5f, 0f),
                new(17.0f, -3.5f, 0f), new(18.2f, -5.7f, 0f), new(11.5f, -6.5f, 0f), new(9.5f, -8.5f, 0f),
                new(9.2f, -12.2f, 0f), new(8.0f, -14.3f, 0f), new(2.5f, -16f, 0f), new(4.2f, -16.4f, 0f),
                new(5.5f, -16f, 0f), new(-1.5f, -10.0f, 0f), new(-1.5f, -15.5f, 0f), new(-4.5f, -12.5f, 0f),
                new(0.3f, -12.5f, 0f), new(4.5f, -7.5f, 0f), new(4.5f, -9.5f, 0f), new(-9.0f, -8.0f, 0f),
                new(-6.0f, -8.0f, 0f), new(-8.0f, -11.0f, 0f), new(-12.0f, -13.0f, 0f), new(-17f, -10f, 0f),
                new(-17.0f, -13.0f, 0f), new(-21.5f, -3.0f, 0f), new(-21.5f, -8.0f, 0f), new(-13.0f, -3.0f, 0f),
                new(-12.6f, -5.6f, 0f), new(-17.0f, 2.5f, 0f), new(-17.0f, -1.0f, 0f), new(-10.5f, 1.0f, 0f),
                new(-10.5f, -2.0f, 0f), new(-6.5f, -4.5f, 0f)
            },
            [1] = new()
            {
                new(-4.5f, 3.5f, 0f), new(-4.5f, -1.4f, 0f), new(8.5f, -1f, 0f), new(14f, -1.5f, 0f),
                new(16.5f, 3f, 0f), new(10f, 5f, 0f), new(6f, 1.5f, 0f), new(2.5f, 13.6f, 0f),
                new(6f, 12f, 0f), new(9.5f, 13f, 0f), new(15f, 9f, 0f), new(17.9f, 11.5f, 0f),
                new(14f, 17.3f, 0f), new(19.5f, 21f, 0f), new(14f, 24f, 0f), new(22f, 24f, 0f),
                new(21f, 8.5f, 0f), new(28f, 3f, 0f), new(22f, 3f, 0f), new(19f, 4f, 0f),
                new(22f, -2f, 0f)
            },
            [2] = new()
            {
                new(16.6f, -1f, 0f), new(16.6f, -5f, 0f), new(20f, -9f, 0f), new(22f, -7f, 0f),
                new(25.5f, -6.9f, 0f), new(29f, -9.5f, 0f), new(29.5f, -8f, 0f), new(35f, -7.6f, 0f),
                new(40.4f, -8f, 0f), new(33f, -10f, 0f), new(39f, -15f, 0f), new(36.5f, -19.5f, 0f),
                new(36.5f, -21f, 0f), new(28f, -21f, 0f), new(24f, -20.5f, 0f), new(22f, -25f, 0f),
                new(16.6f, -17.5f, 0f), new(22.5f, -16.5f, 0f), new(24f, -17f, 0f), new(27f, -16.5f, 0f),
                new(32.7f, -15.7f, 0f), new(31.5f, -12f, 0f), new(10f, -14f, 0f), new(21.5f, -12.5f, 0f),
                new(19f, -11f, 0f), new(12f, -7f, 0f), new(5f, -7.5f, 0f), new(10f, -12f, 0f),
                new(9f, -9f, 0f), new(5f, -9f, 0f), new(4f, -11.2f, 0f), new(5.5f, -16f, 0f),
                new(1f, -17.5f, 0f), new(3f, -21f, 0f), new(2f, -19f, 0f), new(1f, -24f, 0f),
                new(7f, -24f, 0f), new(9f, -20f, 0f), new(7f, -15.8f, 0f), new(11f, -17f, 0f),
                new(12.7f, -15.5f, 0f), new(13f, -24.5f, 0f), new(15f, -17f, 0f), new(17.5f, -25.7f, 0f)
            },
            [3] = new()
            {
                new(2.2f, 2.2f, 0f), new(-0.7f, 2.2f, 0f), new(2.2f, -0.2f, 0f), new(-0.7f, -0.2f, 0f),
                new(-10.0f, 3.0f, 0f), new(-9.0f, 1.0f, 0f), new(-6.5f, -3.5f, 0f), new(-11.5f, -3.5f, 0f),
                new(-17.0f, -3.5f, 0f), new(-18.2f, -5.7f, 0f), new(-11.5f, -6.5f, 0f), new(-9.5f, -8.5f, 0f),
                new(-9.2f, -12.2f, 0f), new(-8.0f, -14.3f, 0f), new(-2.5f, -16f, 0f), new(-4.2f, -16.4f, 0f),
                new(-5.5f, -16f, 0f), new(1.5f, -10.0f, 0f), new(1.5f, -15.5f, 0f), new(4.5f, -12.5f, 0f),
                new(-0.3f, -12.5f, 0f), new(-4.5f, -7.5f, 0f), new(-4.5f, -9.5f, 0f), new(9.0f, -8.0f, 0f),
                new(6.0f, -8.0f, 0f), new(8.0f, -11.0f, 0f), new(12.0f, -13.0f, 0f), new(17f, -10f, 0f),
                new(17.0f, -13.0f, 0f), new(21.5f, -3.0f, 0f), new(21.5f, -8.0f, 0f), new(13.0f, -3.0f, 0f),
                new(12.6f, -5.6f, 0f), new(17.0f, 2.5f, 0f), new(17.0f, -1.0f, 0f), new(10.5f, 1.0f, 0f),
                new(10.5f, -2.0f, 0f), new(6.5f, -4.5f, 0f)
            },
            [4] = new(), // Airship: no spawns, already random
            [5] = new()
            {
                new(-10.68f, 13.32f, 0.1f), new(-08.33f, 10.69f, 0.1f), new(-4.55f, -1.35f, 0f), new(3.19f, -1.27f, 0f),
                new(-2.85f, -11.56f, -0.1f), new(-2.79f, -9.95f, -0.1f), new(14.37f, -16.32f, -0.2f), new(-13.42f, -3.91f, 0f),
                new(-13.19f, -9.22f, -0.1f), new(-23.52f, -6.87f, -0.1f), new(-16.36f, -1.81f, 0f), new(-19.92f, -02.04f, 0f),
                new(-18.08f, 3.07f, 0f), new(-14.21f, 7.48f, 0.1f), new(-17.40f, 5.05f, 0.1f), new(02.92f, 1.91f, 0f),
                new(02.73f, 4.24f, 0f), new(0.96f, 6.80f, 0.1f), new(17.81f, 13.40f, 0.1f), new(23.12f, 14.39f, 0.1f),
                new(21.31f, 10.92f, 0.1f), new(20.00f, 7.42f, 0.1f), new(15.48f, 3.58f, 0f), new(11.81f, 10.58f, 0.1f),
                new(6.31f, 3.61f, 0f), new(22.98f, -6.94f, -0.1f), new(15.28f, -5.85f, -0.1f), new(19.01f, -10.60f, -0.1f),
                new(15.63f, -14.59f, -0.1f), new(08.73f, -11.65f, -0.1f), new(08.99f, -9.58f, -0.1f), new(11.24f, -5.70f, -0.1f),
                new(11.38f, -0.72f, -0f), new(15.99f, 0.78f, 0f), new(0.66f, -6.02f, 0.1f), new(-5.85f, -14.51f, -0.1f),
                new(-10.98f, -5.84f, -0.1f), new(-11.31f, 1.36f, 0f), new(-8.31f, 1.36f, 0f), new(-9.84f, 7.28f, 0.1f)
            }
        };
        private static void SetRandomSpawns(Func<PlayerControl, bool> playerFilter)
        {
            int map = GameOptionsManager.Instance.currentNormalGameOptions.MapId;
            if (!SpawnPoints.TryGetValue(map, out var spawnList) || spawnList.Count == 0)
                return;

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (!playerFilter(player))
                {
                    continue;
                }

                player.transform.position = spawnList[rnd.Next(spawnList.Count)];
            }
        }
    }
}
