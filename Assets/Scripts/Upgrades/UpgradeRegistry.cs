using System.Collections.Generic;

namespace Upgrades
{
    // Maintains the currently "active" upgrade tiles and provides lookup by timing.
    public class UpgradeRegistry
    {
        private readonly List<UpgradeTile> active = new();
        private readonly Dictionary<TriggerTiming, List<UpgradeTile>> timingIndex = new();
        private bool dirty = true;

        public void Register(UpgradeTile tile)
        {
            active.Add(tile);
            dirty = true;
        }

        public void Unregister(UpgradeTile tile)
        {
            active.Remove(tile);
            dirty = true;
        }

        public IEnumerable<UpgradeTile> AllTiles => active;

        private void Rebuild()
        {
            timingIndex.Clear();
            foreach (var tile in active)
            {
                foreach (var t in tile.triggers)
                {
                    if (!timingIndex.TryGetValue(t, out var list))
                        timingIndex[t] = list = new List<UpgradeTile>();
                    list.Add(tile);
                }
            }
            dirty = false;
        }

        public IReadOnlyList<UpgradeTile> GetTilesForTiming(TriggerTiming timing)
        {
            if (dirty) Rebuild();
            return timingIndex.TryGetValue(timing, out var list) ? list : System.Array.Empty<UpgradeTile>();
        }
    }
}
