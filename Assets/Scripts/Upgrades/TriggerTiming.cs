using System;

namespace Upgrades
{
    // Timing keys for upgrade tile triggers. Order is fixed for determinism.
    public enum TriggerTiming
    {
        TurnStart,
        TurnEnd,
        OnSpawn,
        OnMove,
        OnMergePre,
        OnMergePost,
        OnEnemyTurnStart,
        OnEnemyTurnEnd,
        OnTrapTriggered,
        OnEnemySpawn,
        OnBoardFull,
        OnBoardClear,
        OnStageStart,
        OnStageClear,
        OnPeriodicN,
        OnPlayerDeath,
        Passive
    }
}
