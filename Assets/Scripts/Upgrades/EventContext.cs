using UnityEngine;
using Board;

namespace Upgrades
{
    // Base class for all event contexts dispatched through GameEventBus.
    public abstract class EventContext
    {
        public int turnIndex;
    }

    public class TurnContext : EventContext
    {
        public int rngSeed;
        public int playerHP;
        public int stageIndex;
    }

    public class MergeContext : EventContext
    {
        public TileData aTile;
        public TileData bTile;
        public TileData resultTile;
        public Vector2Int position;
        public int sumBeforeRule;
        public int sumAfterRule;
    }
}
