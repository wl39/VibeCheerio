using UnityEngine;
using System.Collections.Generic;
using Board;
using Data;

namespace Systems
{
    /// <summary>
    /// Slide + merge (+1 rule) with Wild pass-through and Upgrade effects.
    /// Compatible with parameterless construction; call BindSystems(...) to enable Wild/Upgrade logic.
    /// </summary>
    public class MergeSystem
    {
        // Optional dependencies (set via BindSystems)
        private WildSystem wildSystem;
        private UpgradeSystem upgradeSystem;
        private System.Random rng;

        public List<MoveInfo> LastMoves { get; } = new();

        /// <summary>
        /// Inject optional systems. Safe to call multiple times.
        /// </summary>
        public void BindSystems(WildSystem wild, UpgradeSystem up, System.Random rng)
        {
            this.wildSystem = wild;
            this.upgradeSystem = up;
            this.rng = rng;
        }

        /// <summary>
        /// Slides/merges tiles in dir. Returns true if anything changed.
        /// </summary>
        public bool SlideAndMerge(BoardController board, Vector2Int dir)
        {
            bool movedOrMerged = false;
            LastMoves.Clear();

            // 0) Clear merge flags
            for (int y = 0; y < BoardController.H; y++)
                for (int x = 0; x < BoardController.W; x++)
                    if (board.Grid[x, y] != null) board.Grid[x, y].mergedThisStep = false;

            // 1) Pick scan order as in your original code
            IEnumerable<Vector2Int> order =
                dir == BoardController.Right ? board.AllCellsTopRightToBottomLeft() :
                dir == BoardController.Left ? board.AllCellsBottomLeftToTopRight() :
                dir == BoardController.Up ? board.AllCellsTopLeftToBottomRight() :
                                               board.AllCellsBottomRightToTopLeft();

            // 2) Iterate and push
            foreach (var cell in order)
            {
                var t = board.Grid[cell.x, cell.y];
                if (t == null) continue;

                // Only Basic/Upgrade tiles slide/merge in v0.1
                if (t.tag != TileTag.Basic && t.tag != TileTag.Upgrade) continue;

                int nx = cell.x;
                int ny = cell.y;
                Vector2Int start = new Vector2Int(nx, ny);
                bool merged = false;

                while (true)
                {
                    int tx = nx + dir.x, ty = ny + dir.y;
                    if (!board.InBounds(tx, ty)) break;

                    var other = board.Grid[tx, ty];

                    // Empty: slide
                    if (other == null)
                    {
                        board.Grid[tx, ty] = t;
                        board.Grid[nx, ny] = null;
                        nx = tx; ny = ty;
                        t.x = nx; t.y = ny;
                        movedOrMerged = true;
                        continue;
                    }

                    // NEW: Wild pass-through
                    if (other.tag == TileTag.Wild)
                    {
                        // If systems not bound, treat as removing the wild and continue
                        if (wildSystem != null)
                            wildSystem.OnPass(board, t, other, rng);
                        else
                            board.Grid[tx, ty] = null; // allow pass-through anyway

                        // After handling, the cell becomes empty; continue sliding
                        continue;
                    }

                    // Merge conditions: same value OR Upgrade on either side
                    bool canMerge = !t.mergedThisStep && !other.mergedThisStep &&
                                    ((t.value == other.value) || t.tag == TileTag.Upgrade || other.tag == TileTag.Upgrade);

                    if (canMerge)
                    {
                        int a = t.value;
                        int b = other.value;
                        int result;

                        // NEW: Apply upgrade effect if any side is Upgrade
                        if (t.tag == TileTag.Upgrade)
                        {
                            var spec = ResolveSpecFromId(t.upgradeSpecId);
                            int level = Mathf.Max(1, t.upgradeLevel);
                            result = (upgradeSystem != null && spec != null)
                                ? upgradeSystem.Apply(spec, a, b, level)
                                : Mathf.Max(a, b) + 1;
                        }
                        else if (other.tag == TileTag.Upgrade)
                        {
                            var spec = ResolveSpecFromId(other.upgradeSpecId);
                            int level = Mathf.Max(1, other.upgradeLevel);
                            result = (upgradeSystem != null && spec != null)
                                ? upgradeSystem.Apply(spec, a, b, level)
                                : Mathf.Max(a, b) + 1;
                        }
                        else
                        {
                            // Default (+1) rule
                            result = Mathf.Max(a, b) + 1;
                        }

                        other.value = result;

                        // Keep/upgrade Upgrade tag & level
                        if (t.tag == TileTag.Upgrade || other.tag == TileTag.Upgrade)
                        {
                            var src = (t.tag == TileTag.Upgrade) ? t : other;
                            other.tag = TileTag.Upgrade;
                            other.upgradeSpecId = src.upgradeSpecId;
                            other.upgradeLevel = Mathf.Max(other.upgradeLevel, src.upgradeLevel);
                        }

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

                    // Stop this tile after encountering a non-empty cell
                    break;
                }

                // Record pure slide (no merge)
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

        // Lookup UpgradeSpec by id
        private UpgradeSpec ResolveSpecFromId(int id)
        {
            if (wildSystem != null && wildSystem.TryGetUpgradeSpec(id, out var spec))
                return spec;
            return null;
        }
    }
}
