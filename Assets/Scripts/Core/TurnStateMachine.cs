using UnityEngine;
using System;

namespace Core
{
    public enum TurnState
    {
        AwaitInput,
        Slide,
        Merge,
        Spawn,
        Enemy,
        Cleanup
    }

    public class TurnStateMachine : MonoBehaviour
    {
        public TurnState State { get; private set; } = TurnState.AwaitInput;
        public event Action<TurnState> OnStateChanged;

        public void StepTo(TurnState next)
        {
            State = next;
            OnStateChanged?.Invoke(State);
        }

        public void ResetToAwait() => StepTo(TurnState.AwaitInput);
    }
}
