// UI/BoardView.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using Board;
using Core;
using Systems;

public class BoardView : MonoBehaviour
{
    [SerializeField] private BoardController board;
    [SerializeField] private GameRunner runner;
    [SerializeField] private Transform boardGrid;   // BoardGrid RectTransform
    [SerializeField] private GameObject tileUIPrefab;
    [SerializeField] private float moveDuration = 0.15f;

    private readonly Dictionary<TileData, RectTransform> tileViews = new();
    private RectTransform boardRect;

    void Awake()
    {
        boardRect = boardGrid as RectTransform;
        var layout = boardRect.GetComponent<GridLayoutGroup>();
        if (layout != null) layout.enabled = false; // manual positioning for animation
    }

    void OnEnable()
    {
        if (runner != null) runner.OnBoardChanged += Refresh;
    }
    void OnDisable()
    {
        if (runner != null) runner.OnBoardChanged -= Refresh;
    }

    void Start()
    {
        RebuildAll();
    }

    public void Refresh()
    {
        if (runner.LastMoves.Count == 0 || tileViews.Count == 0)
            RebuildAll();
        else
            StartCoroutine(Animate(runner.LastMoves));
    }

    void RebuildAll()
    {
        StopAllCoroutines();
        foreach (var kv in tileViews) Destroy(kv.Value.gameObject);
        tileViews.Clear();

        for (int y = 0; y < BoardController.H; y++)
            for (int x = 0; x < BoardController.W; x++)
            {
                var t = board.Grid[x, y];
                if (t == null) continue;
                var rt = CreateTile(t);
                tileViews[t] = rt;
            }
    }

    RectTransform CreateTile(TileData t)
    {
        var go = Instantiate(tileUIPrefab, boardRect);
        UpdateTileVisual(t, go);
        var rt = go.GetComponent<RectTransform>();
        rt.anchoredPosition = CellToAnchoredPosition(t.x, t.y);
        rt.localScale = Vector3.one;
        return rt;
    }

    void UpdateTileVisual(TileData t, GameObject go)
    {
        var txt = go.GetComponentInChildren<TextMeshProUGUI>(true);
        var img = go.GetComponent<Image>();

        if (t.tag == TileTag.Basic || t.tag == TileTag.Upgrade)
            txt.text = t.value.ToString();
        else if (t.tag == TileTag.Wild) txt.text = "W";
        else if (t.tag == TileTag.Trap) txt.text = "T";
        else if (t.tag == TileTag.Enemy) txt.text = $"E\n{t.hp}";

        img.color = GetColorFor(t);
    }

    IEnumerator Animate(List<MoveInfo> moves)
    {
        var remove = new List<TileData>();
        var bump = new HashSet<TileData>();

        foreach (var m in moves)
        {
            if (!tileViews.TryGetValue(m.tile, out var rt)) continue;
            StartCoroutine(MoveRect(rt, CellToAnchoredPosition(m.to.x, m.to.y), moveDuration));
            if (m.mergeTarget != null)
            {
                remove.Add(m.tile);
                bump.Add(m.mergeTarget);
            }
        }

        yield return new WaitForSeconds(moveDuration);

        foreach (var t in remove)
        {
            if (tileViews.TryGetValue(t, out var rt))
            {
                Destroy(rt.gameObject);
                tileViews.Remove(t);
            }
        }

        foreach (var kv in tileViews)
        {
            kv.Value.anchoredPosition = CellToAnchoredPosition(kv.Key.x, kv.Key.y);
            UpdateTileVisual(kv.Key, kv.Value.gameObject);
        }

        foreach (var t in bump)
        {
            if (tileViews.TryGetValue(t, out var rt))
                StartCoroutine(ScaleBump(rt));
        }

        // Spawn new tiles
        for (int y = 0; y < BoardController.H; y++)
            for (int x = 0; x < BoardController.W; x++)
            {
                var t = board.Grid[x, y];
                if (t != null && !tileViews.ContainsKey(t))
                {
                    var rt = CreateTile(t);
                    rt.localScale = Vector3.zero;
                    tileViews[t] = rt;
                    StartCoroutine(ScaleIn(rt));
                }
            }
    }

    IEnumerator MoveRect(RectTransform rt, Vector2 target, float duration)
    {
        Vector2 start = rt.anchoredPosition;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            rt.anchoredPosition = Vector2.Lerp(start, target, t);
            yield return null;
        }
        rt.anchoredPosition = target;
    }

    IEnumerator ScaleIn(RectTransform rt)
    {
        Vector3 start = Vector3.zero;
        Vector3 end = Vector3.one;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / moveDuration;
            rt.localScale = Vector3.Lerp(start, end, t);
            yield return null;
        }
        rt.localScale = end;
    }

    IEnumerator ScaleBump(RectTransform rt)
    {
        Vector3 mid = Vector3.one * 1.1f;
        float half = moveDuration * 0.5f;
        float t = 0f;
        Vector3 start = Vector3.one;
        while (t < 1f)
        {
            t += Time.deltaTime / half;
            rt.localScale = Vector3.Lerp(start, mid, t);
            yield return null;
        }
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / half;
            rt.localScale = Vector3.Lerp(mid, Vector3.one, t);
            yield return null;
        }
        rt.localScale = Vector3.one;
    }

    Vector2 CellToAnchoredPosition(int x, int y)
    {
        var rect = boardRect.rect;
        float cellW = rect.width / BoardController.W;
        float cellH = rect.height / BoardController.H;
        float originX = -rect.width / 2f + cellW / 2f;
        float originY = -rect.height / 2f + cellH / 2f;
        return new Vector2(originX + x * cellW, originY + y * cellH);
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
