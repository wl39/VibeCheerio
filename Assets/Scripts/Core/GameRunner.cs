using UnityEngine;
using Board;
using Systems;
using System;

namespace Core
{
    public class GameRunner : MonoBehaviour
    {
        [SerializeField] BoardController board;
        TurnStateMachine fsm;
        MergeSystem mergeSystem;
        SpawnSystem spawnSystem;


        public event Action OnBoardChanged;
        public int Turn => turnCounter;

        int turnCounter = 0;

        void Awake()
        {
            fsm = GetComponent<TurnStateMachine>();
            mergeSystem = new MergeSystem();
            spawnSystem = new SpawnSystem(seed: System.Environment.TickCount);
        }

        public void RestartRun()
        {
            // Clear grid
            for (int y = 0; y < BoardController.H; y++)
                for (int x = 0; x < BoardController.W; x++)
                    board.Grid[x, y] = null;

            turnCounter = 0;
            spawnSystem.SpawnOne(board, 1);
            spawnSystem.SpawnOne(board, 1);
            fsm.ResetToAwait();
            OnBoardChanged?.Invoke();
        }

        void Start()
        {
            RestartRun();
        }

        public void OnSwipe(Vector2Int dir)
        {
            if (fsm.State != TurnState.AwaitInput) return;

            fsm.StepTo(TurnState.Slide);
            bool changed = mergeSystem.SlideAndMerge(board, dir);

            fsm.StepTo(TurnState.Merge);

            if (changed)
            {
                fsm.StepTo(TurnState.Spawn);
                spawnSystem.SpawnOne(board, 1);

                fsm.StepTo(TurnState.Cleanup);
                turnCounter++;

                OnBoardChanged?.Invoke(); // UI 갱신 트리거
            }

            fsm.ResetToAwait();
        }
    }
}
