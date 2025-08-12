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
        public BindScope bindScope = BindScope.Tile;
        public List<TriggerTiming> triggers = new();
        public List<string> fsmGate = new();
        public RunPhase runPhase = RunPhase.Post;
        public List<ConditionData> conditions = new();
        public List<EffectData> effects = new();
        public int priority = 0;
        public int cooldown = 0;
        public int charges = -1;
        public StackPolicy stackPolicy = StackPolicy.Stack;
        public int durationTurns = 0;
        public int period = 1; // Used when triggers include OnPeriodic
        public List<string> tags = new();
    }
}
