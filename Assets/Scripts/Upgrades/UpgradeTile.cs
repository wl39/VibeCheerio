using System.Collections.Generic;

namespace Upgrades
{
    // Definition data for an upgrade tile instance.
    public class UpgradeTile
    {
        public string id;
        public string displayName;
        public List<TriggerTiming> triggers = new();
        public int priority = 0;
        public int cooldownTurns = 0;
        public int charges = -1;
        public bool oncePerTurn = false;
        public bool stackable = true;
        public List<EffectData> effects = new();
        public List<ConditionData> conditions = new();
        public TargetSelectorData targetSelector;
        public Dictionary<string, float> values = new();
        public List<string> tags = new();

        // Runtime placement info for sorting (row-major order)
        public int x;
        public int y;
    }

    // Effect definition
    public class EffectData
    {
        public string type;
        public Dictionary<string, float> @params = new();
        public int repeats = 1;
    }

    // Condition definition (all AND)
    public class ConditionData
    {
        public string type;
        public Dictionary<string, float> @params = new();
    }

    // Target selector definition
    public class TargetSelectorData
    {
        public string type;
        public Dictionary<string, float> @params = new();
    }
}
