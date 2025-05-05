using System.Collections.Generic;
using UnityEngine;

public class BuildArea : MonoBehaviour
{
    public int width;
    public int height;
    public Vector2 origin;
    public float cellSize = 1f;
    public GameObject cellVisualPrefab; // optional prefab for runtime visual

    private HashSet<Vector2Int> validCells = new();

    private List<GameObject> runtimeVisuals = new();

    private void Start()
    {
        GenerateValidCells();

        // Hide grid visuals at first
        ShowGridVisual(false);
    }

    public void GenerateValidCells()
    {
        validCells.Clear();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                validCells.Add(new Vector2Int(x, y));
            }
        }
    }

    public void ShowGridVisual(bool show)
    {
        if (cellVisualPrefab == null)
            return;

        if (show && runtimeVisuals.Count == 0)
        {
            foreach (var cell in validCells)
            {
                Vector3 worldPos = origin + new Vector2(cell.x, cell.y) * cellSize;
                var cellVisual = Instantiate(cellVisualPrefab, worldPos, Quaternion.identity, transform);
                runtimeVisuals.Add(cellVisual);
            }
        }

        foreach (var visual in runtimeVisuals)
        {
            if (visual != null)
                visual.SetActive(show);
        }
    }

    public Vector2Int WorldToLocalCell(Vector2 worldPos)
    {
        Vector2 localPos = worldPos - origin;
        int x = Mathf.FloorToInt(localPos.x / cellSize);
        int y = Mathf.FloorToInt(localPos.y / cellSize);
        return new Vector2Int(x, y);
    }

    public bool IsCellValid(Vector2Int cell)
    {
        return validCells.Contains(cell);
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 cellCenter = origin + new Vector2(x + 0.5f, y + 0.5f) * cellSize;
                Gizmos.DrawWireCube(cellCenter, Vector3.one * cellSize);
            }
        }
    }

    public List<Vector2Int> GetOccupiedCells(Bounds bounds)
    {
        List<Vector2Int> cells = new();

        Vector2 min = bounds.min;
        Vector2 max = bounds.max;

        Vector2Int minCell = WorldToLocalCell(min);
        Vector2Int maxCell = WorldToLocalCell(max);

        for (int x = minCell.x; x <= maxCell.x; x++)
        {
            for (int y = minCell.y; y <= maxCell.y; y++)
            {
                Vector2Int cell = new(x, y);
                if (IsCellValid(cell))
                    cells.Add(cell);
                else
                    return new List<Vector2Int>(); // Return empty if any part is outside
            }
        }

        return cells;
    }

    public bool AreCellsValid(List<Vector2Int> cells)
    {
        foreach (var cell in cells)
        {
            if (!IsCellValid(cell))
                return false;
        }
        return true;
    }
}
