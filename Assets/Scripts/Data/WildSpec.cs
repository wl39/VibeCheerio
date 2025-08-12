using UnityEngine;

namespace Data
{
    public enum UpgradeSpawnMode { ConvertMover, SpawnRandom }

    [CreateAssetMenu(menuName = "Vibe/Data/WildSpec")]
    public class WildSpec : ScriptableObject
    {
        public int id;
        public string displayName;
        public int cooldownTurns = 5;
        public bool allowUpgradePlus = true;     // 승급+ 허용 여부
        public UpgradeSpawnMode spawnMode = UpgradeSpawnMode.ConvertMover;
        public UpgradeSpec upgradeSpec;          // 이 와일드가 부여하는 승급 효과
    }
}
