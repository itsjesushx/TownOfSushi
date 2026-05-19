using System.Collections.Generic;

namespace TownOfSushi.Patches.CustomHats
{
    public static class HatCache
    {
        public static SortedList<string, List<HatData>> SortedHats = null;

        public static Dictionary<int, string> StoreNames = [];
    }
}
