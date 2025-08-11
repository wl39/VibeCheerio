using UnityEngine;
using Board;

namespace Systems
{
    /// <summary>
    /// Information about a tile movement in one turn.
    /// </summary>
    public class MoveInfo
    {
        public TileData tile;            // moving tile
        public Vector2Int from;          // start cell
        public Vector2Int to;            // end cell
        public TileData mergeTarget;     // null if not merged
    }
}
