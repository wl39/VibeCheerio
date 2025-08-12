using System.Collections.Generic;

namespace Upgrades
{
    // Processes events and executes upgrade tile effects in deterministic order.
    public class UpgradeDispatcher
    {
        private readonly UpgradeRegistry registry;
        private readonly GameEventBus bus;
        private readonly Dictionary<UpgradeTile, RuntimeUpgradeState> states = new();
        private int creationCounter = 0;
        private const int safetyLimit = 256;
        private int currentFrame = 0;

        public UpgradeDispatcher(GameEventBus bus, UpgradeRegistry registry)
        {
            this.bus = bus;
            this.registry = registry;
            bus.OnEvent += Handle;
        }

        private RuntimeUpgradeState GetState(UpgradeTile tile)
        {
            if (!states.TryGetValue(tile, out var state))
            {
                state = new RuntimeUpgradeState
                {
                    creationOrder = creationCounter++,
                    remainingCharges = tile.charges
                };
                states[tile] = state;
            }
            return state;
        }

        private void Handle(TriggerTiming timing, EventContext context)
        {
            currentFrame++;
            var candidates = registry.GetTilesForTiming(timing);
            var eval = new List<(UpgradeTile tile, RuntimeUpgradeState state)>();

            foreach (var tile in candidates)
            {
                var state = GetState(tile);
                if (!EvaluateConditions(tile, context)) continue;
                if (!CheckConstraints(tile, state, context.turnIndex)) continue;
                eval.Add((tile, state));
            }

            eval.Sort((a, b) =>
            {
                int cmp = a.tile.priority.CompareTo(b.tile.priority);
                if (cmp != 0) return cmp;
                cmp = a.tile.y.CompareTo(b.tile.y);
                if (cmp != 0) return cmp;
                cmp = a.tile.x.CompareTo(b.tile.x);
                if (cmp != 0) return cmp;
                return a.state.creationOrder.CompareTo(b.state.creationOrder);
            });

            int fireCount = 0;
            foreach (var (tile, state) in eval)
            {
                if (fireCount++ >= safetyLimit) break;
                bool changed = ExecuteEffects(tile, context);
                if (changed)
                {
                    state.lastTriggeredTurn = context.turnIndex;
                    state.triggeredThisTurn = context.turnIndex;
                    state.nextAvailableTurn = context.turnIndex + tile.cooldownTurns;
                    state.lastTriggerFrame = currentFrame;
                    if (state.remainingCharges > 0) state.remainingCharges--;
                }
            }
        }

        private bool EvaluateConditions(UpgradeTile tile, EventContext ctx)
        {
            // TODO: evaluate condition data. For now, always true.
            return true;
        }

        private bool CheckConstraints(UpgradeTile tile, RuntimeUpgradeState state, int turn)
        {
            if (state.lastTriggerFrame == currentFrame) return false; // re-entry guard
            if (tile.oncePerTurn && state.triggeredThisTurn == turn) return false;
            if (turn < state.nextAvailableTurn) return false;
            if (state.remainingCharges == 0) return false;
            return true;
        }

        private bool ExecuteEffects(UpgradeTile tile, EventContext ctx)
        {
            // TODO: implement concrete effect logic; stub assumes any effect changes state.
            bool changed = false;
            foreach (var eff in tile.effects)
                changed = true;
            return changed;
        }
    }
}
