using UnityEngine;
using Board;
using Systems;
using System;
using System.Collections.Generic;
using Data;

namespace Core
{
    public class GameRunner : MonoBehaviour
    {
        [SerializeField] BoardController board;

        [Header("Specs")]
        [SerializeField] WildSpec[] wildSpecs;        // assign in Inspector
        [SerializeField] UpgradeSpec[] upgradeSpecs;  // assign in Inspector

        // Optional: simple periodic event (assign only if you have UI)
        [Header("Events (optional)")]
        [SerializeField] EventPopup eventPopup;    // can be null
        [SerializeField] int eventEveryTurns = 10;    // used only if eventPopup != null

        TurnStateMachine fsm;
        MergeSystem mergeSystem;
        SpawnSystem spawnSystem;
        WildSystem wildSystem;
        UpgradeSystem upgradeSystem;

        System.Random rng;

        public event Action OnBoardChanged;
        public int Turn => turnCounter;
        public List<MoveInfo> LastMoves => mergeSystem.LastMoves;

        int turnCounter = 0;

        void Awake()
        {
            fsm = GetComponent<TurnStateMachine>();
            if (fsm == null) fsm = gameObject.AddComponent<TurnStateMachine>();

            rng = new System.Random(System.Environment.TickCount);

            wildSystem = new WildSystem();
            upgradeSystem = new UpgradeSystem();

            // Register specs
            if (wildSpecs != null)
                foreach (var w in wildSpecs) if (w) wildSystem.RegisterWildSpec(w);
            if (upgradeSpecs != null)
                foreach (var u in upgradeSpecs) if (u) wildSystem.RegisterUpgradeSpec(u);

            mergeSystem = new MergeSystem();
            mergeSystem.BindSystems(wildSystem, upgradeSystem, rng); // IMPORTANT

            spawnSystem = new SpawnSystem(seed: System.Environment.TickCount);
        }

        void Start()
        {
            RestartRun();
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

                OnBoardChanged?.Invoke();

                // Optional periodic event demo
                if (eventPopup != null && eventEveryTurns > 0 && wildSpecs != null && wildSpecs.Length > 0)
                {
                    if (turnCounter % eventEveryTurns == 0)
                    {
                        var w = wildSpecs[0];
                        eventPopup.OpenWild(w.id, w.displayName);
                    }
                }
            }

            fsm.ResetToAwait();
        }

        // Public API for events/cards
        public bool SpawnWildRandom(int wildSpecId)
            => wildSystem.SpawnWildRandom(board, wildSpecId, rng);
    }
}
