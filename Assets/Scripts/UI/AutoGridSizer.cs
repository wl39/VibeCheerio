using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup), typeof(RectTransform))]
public class AutoGridSizer : MonoBehaviour
{
    [SerializeField] int columns = 6;
    [SerializeField] int rows = 6;
    GridLayoutGroup grid; RectTransform rt;

    void Awake() { grid = GetComponent<GridLayoutGroup>(); rt = GetComponent<RectTransform>(); UpdateCellSize(); }
    void OnRectTransformDimensionsChange() { UpdateCellSize(); }

#if UNITY_EDITOR
    void OnValidate() { if (!grid) grid = GetComponent<GridLayoutGroup>(); if (!rt) rt = GetComponent<RectTransform>(); UpdateCellSize(); }
#endif

    void UpdateCellSize()
    {
        var total = rt.rect.size; var sp = grid.spacing;
        float cellW = (total.x - sp.x * (columns - 1)) / columns;
        float cellH = (total.y - sp.y * (rows - 1)) / rows;
        float cell = Mathf.Floor(Mathf.Min(cellW, cellH));
        grid.cellSize = new Vector2(cell, cell);
    }
}
