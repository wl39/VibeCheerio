using UnityEngine;

namespace Data
{
    public enum UpgradeEffect { Add, Multiply, Max, Abs, DoubleMerge }

    [CreateAssetMenu(menuName = "Vibe/Data/UpgradeSpec")]
    public class UpgradeSpec : ScriptableObject
    {
        public int id;
        public string displayName;
        public UpgradeEffect effect;
    }
}
