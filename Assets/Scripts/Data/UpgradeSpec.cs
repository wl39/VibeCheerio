using System.Collections.Generic;
using UnityEngine;
using Upgrades;

namespace Data
{
    [CreateAssetMenu(menuName = "Vibe/Data/UpgradeSpec")]
    public class UpgradeSpec : ScriptableObject
    {
        public int id;
        public string displayName;
        public List<UpgradeEffectSpec> effects = new();
    }

    [System.Serializable]
    public class UpgradeEffectSpec
    {
        public TriggerTiming timing;
        public string type;
        public int period = 1; // Used when timing == OnPeriodicN
    }
}
