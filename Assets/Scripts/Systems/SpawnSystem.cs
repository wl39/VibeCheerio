using UnityEngine;
using System.Collections.Generic;
using Board;

namespace Systems
{
    public class SpawnSystem
    {
        private System.Random rng;

        public SpawnSystem(int seed) { rng = new System.Random(seed); }
        public void Reseed(int seed) { rng = new System.Random(seed); }

        public bool SpawnOne(BoardController board, int value = 1)
        {
            var empty = board.EmptyCells();
            if (empty.Count == 0) return false;
            var pick = empty[rng.Next(empty.Count)];
            board.Grid[pick.x, pick.y] = new TileData(pick.x, pick.y, value, TileTag.Basic);
            return true;
        }
    }
}
