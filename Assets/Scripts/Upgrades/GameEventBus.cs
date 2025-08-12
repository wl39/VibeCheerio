using System;

namespace Upgrades
{
    // Simple synchronous event bus.
    public class GameEventBus
    {
        public event Action<TriggerTiming, EventContext> OnEvent;

        public void Publish(TriggerTiming timing, EventContext context)
        {
            OnEvent?.Invoke(timing, context);
        }
    }
}
