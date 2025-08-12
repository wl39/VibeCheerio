using System;

namespace Upgrades
{
    // Timing keys for upgrade tile triggers. Order is fixed for determinism.
    public enum TriggerTiming
    {
        TurnStart,
        SwipeStart,
        OnMove,
        OnMergePre,
        OnMergePost,
        OnSpawn,
        OnBoardFull,
        OnBoardClear,
        OnEnemyTurnStart,
        OnEnemyTurnEnd,
        OnPeriodic,
        SwipeEnd,
        TurnEnd,
        OnStageStart,
        OnStageClear,
        OnTrapTriggered,
        OnEnemySpawn,
        OnPlayerDeath,
        Passive
    }
}
