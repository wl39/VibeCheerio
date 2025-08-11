using UnityEngine;
using System.Collections.Generic;
using Board;

namespace Systems
{
    public class MergeSystem
    {
        public List<MoveInfo> LastMoves { get; } = new();

        // Slide then merge in +1 rule; scan order = top-left -> bottom-right.
        public bool SlideAndMerge(BoardController board, Vector2Int dir)
        {
            bool movedOrMerged = false;
            LastMoves.Clear();

            // 1) Clear merge flags
            for (int y = 0; y < BoardController.H; y++)
                for (int x = 0; x < BoardController.W; x++)
                    if (board.Grid[x, y] != null) board.Grid[x, y].mergedThisStep = false;

            // 2) Compute iteration order opposite to move direction so pushing is correct
            IEnumerable<Vector2Int> order =
                dir == BoardController.Right ? board.AllCellsTopRightToBottomLeft() :
                dir == BoardController.Left ? board.AllCellsBottomLeftToTopRight() :
                dir == BoardController.Up ? board.AllCellsTopLeftToBottomRight() :
                board.AllCellsBottomRightToTopLeft();

            foreach (var cell in order)
            {
                var t = board.Grid[cell.x, cell.y];
                if (t == null || t.tag != TileTag.Basic && t.tag != TileTag.Upgrade) continue; // Only slide merge-able for v0.1

                int nx = cell.x; int ny = cell.y;
                Vector2Int start = new Vector2Int(nx, ny);
                bool merged = false;

                // Slide as far as possible
                while (true)
                {
                    int tx = nx + dir.x, ty = ny + dir.y;
                    if (!board.InBounds(tx, ty)) break;
                    var other = board.Grid[tx, ty];
                    if (other == null)
                    {
                        board.Grid[tx, ty] = t; board.Grid[nx, ny] = null; nx = tx; ny = ty; movedOrMerged = true;
                        t.x = nx; t.y = ny;
                        continue;
                    }
                    // Merge condition: same value OR Upgrade tile allows any-value merge (v0.1 rule)
                    bool canMerge = !t.mergedThisStep && !other.mergedThisStep &&
                                    ((t.value == other.value) || t.tag == TileTag.Upgrade || other.tag == TileTag.Upgrade);

                    if (canMerge)
                    {
                        // +1 rule for base v0.1 (Upgrade effects hook later)
                        other.value = Mathf.Max(t.value, other.value) + 1;
                        other.tag = (t.tag == TileTag.Upgrade || other.tag == TileTag.Upgrade) ? TileTag.Upgrade : other.tag;
                        other.mergedThisStep = true;

                        board.Grid[nx, ny] = null;
                        movedOrMerged = true;

                        LastMoves.Add(new MoveInfo
                        {
                            tile = t,
                            from = start,
                            to = new Vector2Int(tx, ty),
                            mergeTarget = other
                        });
                        merged = true;
                    }
                    break;
                }

                if (!merged && (start.x != nx || start.y != ny))
                {
                    LastMoves.Add(new MoveInfo
                    {
                        tile = t,
                        from = start,
                        to = new Vector2Int(nx, ny),
                        mergeTarget = null
                    });
                }
            }
            return movedOrMerged;
        }
    }
}
