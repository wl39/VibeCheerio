using UnityEngine;
using Board;
using Data;
using System.Collections.Generic;

namespace Systems
{
    public class WildSystem
    {
        readonly Dictionary<int, WildSpec> wilds = new();
        readonly Dictionary<int, UpgradeSpec> upgrades = new();

        public void RegisterWildSpec(WildSpec spec) { wilds[spec.id] = spec; }
        public void RegisterUpgradeSpec(UpgradeSpec s) { upgrades[s.id] = s; }

        // 보드에 와일드 스폰
        public bool SpawnWildRandom(BoardController board, int wildSpecId, System.Random rng)
        {
            var empty = board.EmptyCells();
            if (empty.Count == 0) return false;
            var p = empty[rng.Next(empty.Count)];
            var t = new TileData(p.x, p.y, 0, TileTag.Wild);
            t.wildSpecId = wildSpecId;
            board.Grid[p.x, p.y] = t;
            return true;
        }

        // "타일이 와일드를 지난 순간" 처리.
        // mover: 이동 중인 타일, wild: 그 칸의 와일드, 해당 칸을 비워주고 mover를 강화하거나 랜덤 승급 생성
        public void OnPass(BoardController board, TileData mover, TileData wild, System.Random rng)
        {
            if (!wilds.TryGetValue(wild.wildSpecId, out var wspec) || wspec.upgradeSpec == null)
            {
                // 와일드 정의가 없으면 그냥 제거
                board.Grid[wild.x, wild.y] = null;
                return;
            }

            // 와일드 제거
            board.Grid[wild.x, wild.y] = null;

            if (wspec.spawnMode == UpgradeSpawnMode.ConvertMover)
            {
                mover.tag = TileTag.Upgrade;
                mover.upgradeSpecId = wspec.upgradeSpec.id;
                // 기존이 승급이면 승급+로
                if (mover.upgradeLevel < 1) mover.upgradeLevel = 1;
                else if (wspec.allowUpgradePlus) mover.upgradeLevel = 2;
            }
            else // SpawnRandom
            {
                var empty = board.EmptyCells();
                if (empty.Count > 0)
                {
                    var p = empty[rng.Next(empty.Count)];
                    var u = new TileData(p.x, p.y, mover.value, TileTag.Upgrade);
                    u.upgradeSpecId = wspec.upgradeSpec.id;
                    u.upgradeLevel = 1;
                    board.Grid[p.x, p.y] = u;
                }
            }
        }
    }
}
