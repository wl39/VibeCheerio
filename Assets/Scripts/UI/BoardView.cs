// UI/BoardView.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Board;
using Core;

public class BoardView : MonoBehaviour
{
    [SerializeField] private BoardController board;
    [SerializeField] private GameRunner runner;
    [SerializeField] private Transform boardGrid;   // BoardGrid RectTransform
    [SerializeField] private GameObject tileUIPrefab;

    private readonly System.Collections.Generic.List<GameObject> pool = new();

    void OnEnable()
    {
        if (runner != null) runner.OnBoardChanged += Refresh;
    }
    void OnDisable()
    {
        if (runner != null) runner.OnBoardChanged -= Refresh;
    }

    public void Refresh()
    {
        // Clear all (간단 버전: 풀 돌리기)
        foreach (var go in pool) go.SetActive(false);

        int used = 0;
        for (int y = BoardController.H - 1; y >= 0; y--)     // 보드의 위에서 아래로
        {
            for (int x = 0; x < BoardController.W; x++)
            {
                var t = board.Grid[x, y];
                if (t == null) continue;

                GameObject go;
                if (used < pool.Count) go = pool[used++];
                else { go = Instantiate(tileUIPrefab, boardGrid); pool.Add(go); used++; }

                go.SetActive(true);
                var txt = go.GetComponentInChildren<TextMeshProUGUI>(true);
                var img = go.GetComponent<Image>();

                // 값/태그 표기
                if (t.tag == TileTag.Basic || t.tag == TileTag.Upgrade)
                    txt.text = t.value.ToString();
                else if (t.tag == TileTag.Wild) txt.text = "W";
                else if (t.tag == TileTag.Trap) txt.text = "T";
                else if (t.tag == TileTag.Enemy) txt.text = $"E\n{t.hp}";

                // 색상 간단 매핑
                img.color = GetColorFor(t);

                // GridLayoutGroup 사용 중이므로 위치는 인덱스로만 배치
                // 셀 순서 = (위에서 아래로, 좌->우) 정렬이 필요하면 Layout Group의 Child Alignment를 Upper Left로
            }
        }

        // 빈 칸 UI 필요 없으므로 종료
        LayoutRebuilder.ForceRebuildLayoutImmediate(boardGrid as RectTransform);
    }

    Color GetColorFor(TileData t)
    {
        return t.tag switch
        {
            TileTag.Basic => Color.Lerp(new Color(0.85f, 0.85f, 0.85f), new Color(0.95f, 0.75f, 0.3f), Mathf.Clamp01(t.value / 16f)),
            TileTag.Upgrade => new Color(0.5f, 0.8f, 1f),
            TileTag.Wild => new Color(0.6f, 1f, 0.6f),
            TileTag.Trap => new Color(1f, 0.6f, 0.6f),
            TileTag.Enemy => new Color(0.8f, 0.6f, 1f),
            _ => Color.white
        };
    }
}
