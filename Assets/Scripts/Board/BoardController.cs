using UnityEngine;
using System.Collections.Generic;

namespace Board
{
    public class BoardController : MonoBehaviour
    {
        public const int W = 6, H = 6;
        // Grid indexed [x,y] -> TileData or null
        public TileData[,] Grid { get; private set; } = new TileData[W, H];

        // Direction vectors
        public static readonly Vector2Int Up = new Vector2Int(0, 1);
        public static readonly Vector2Int Down = new Vector2Int(0, -1);
        public static readonly Vector2Int Left = new Vector2Int(-1, 0);
        public static readonly Vector2Int Right = new Vector2Int(1, 0);

        public bool InBounds(int x, int y) => (uint)x < W && (uint)y < H;

        public IEnumerable<Vector2Int> AllCellsTopLeftToBottomRight()
        {
            for (int y = H - 1; y >= 0; y--)      // Top (H-1) to Bottom (0)
                for (int x = 0; x < W; x++)       // Left to Right
                    yield return new Vector2Int(x, y);
        }

        public IEnumerable<Vector2Int> AllCellsBottomRightToTopLeft()
        {
            for (int y = 0; y < H; y++)
                for (int x = W - 1; x >= 0; x--)
                    yield return new Vector2Int(x, y);
        }

        public List<Vector2Int> EmptyCells()
        {
            var list = new List<Vector2Int>();
            for (int y = 0; y < H; y++)
                for (int x = 0; x < W; x++)
                    if (Grid[x, y] == null) list.Add(new Vector2Int(x, y));
            return list;
        }
    }
}
