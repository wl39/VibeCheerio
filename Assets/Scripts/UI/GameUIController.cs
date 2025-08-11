using UnityEngine;
using TMPro;
using Core;

public class GameUIController : MonoBehaviour
{
    [SerializeField] private GameRunner runner;
    [SerializeField] private BoardView boardView;

    [SerializeField] private TextMeshProUGUI txtStage;
    [SerializeField] private TextMeshProUGUI txtTurn;
    [SerializeField] private TextMeshProUGUI txtNextEvent;

    int stageIndex = 1;
    int nextEventEveryTurns = 10;

    void OnEnable() { runner.OnBoardChanged += UpdateHUD; }
    void OnDisable() { runner.OnBoardChanged -= UpdateHUD; }

    public void BtnRestart() { runner.RestartRun(); }
    public void BtnUp() { runner.OnSwipe(new Vector2Int(0, 1)); }
    public void BtnDown() { runner.OnSwipe(new Vector2Int(0, -1)); }
    public void BtnLeft() { runner.OnSwipe(new Vector2Int(-1, 0)); }
    public void BtnRight() { runner.OnSwipe(new Vector2Int(1, 0)); }

    void UpdateHUD()
    {
        txtStage.text = $"Stage {stageIndex}";
        txtTurn.text = $"Turn {runner.Turn}";
        int remain = nextEventEveryTurns - (runner.Turn % nextEventEveryTurns);
        if (remain == nextEventEveryTurns) remain = 0;
        txtNextEvent.text = $"Next Event in {remain}";
        boardView.Refresh();
    }
}
