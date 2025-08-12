using System.Collections.Generic;

namespace Upgrades
{
    // Definition data for an upgrade tile instance.
    public class UpgradeTile
    {
        public string id;
        public string displayName;
        public BindScope bindScope = BindScope.Tile;
        public List<TriggerTiming> triggers = new();
        public List<string> fsmGate = new();
        public RunPhase runPhase = RunPhase.Post;
        public int priority = 0;
        public int cooldown = 0;
        public int charges = -1;
        public bool oncePerTurn = false;
        public StackPolicy stackPolicy = StackPolicy.Stack;
        public int durationTurns = 0;
        public int period = 1;
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
