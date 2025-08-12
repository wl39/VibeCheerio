namespace Upgrades
{
    // Stores mutable runtime data for each upgrade tile instance.
    public class RuntimeUpgradeState
    {
        public int lastTriggeredTurn = -1;
        public int nextAvailableTurn = 0;
        public int remainingCharges = -1;
        public int triggeredThisTurn = -1;
        public int creationOrder = 0;
        public int lastTriggerFrame = -1;
    }
}
