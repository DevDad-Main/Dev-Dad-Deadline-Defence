using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance;

    public GameObject[] buildPrefabs; // e.g., wall, turret
    public BuildArea[] buildAreas;
    public GameObject ghostInstance;

    private GameObject currentGhost;
    private int selectedIndex = 0;
    private bool isInBuildMode = false;

    private void Awake() => Instance = this;

    public void ToggleBuildMode()
    {
        isInBuildMode = !isInBuildMode;

        if (isInBuildMode)
            CreateGhost();
        else
            DestroyGhost();
    }

    private void Update()
    {
        if (!isInBuildMode || currentGhost == null)
            return;

        Vector3 worldPos = GetBuildInputPosition();
        Vector3 snapped = SnapToGrid(worldPos);

        currentGhost.transform.position = snapped;

        bool isInZone = IsInBuildZone(snapped);
        bool isClear = !Physics2D.OverlapCircle(snapped, 0.4f);

        var sr = currentGhost.GetComponent<SpriteRenderer>();
        sr.color = (isInZone && isClear) ? Color.green : Color.red;

        if (isInZone && isClear && Input.GetMouseButtonDown(0)) // Replace with input system later
        {
            Instantiate(buildPrefabs[selectedIndex], snapped, Quaternion.identity);
        }
    }

    private void CreateGhost()
    {
        if (ghostInstance != null)
            DestroyGhost();

        ghostInstance = Instantiate(buildPrefabs[selectedIndex]);
        var sr = ghostInstance.GetComponent<SpriteRenderer>();
        sr.color = new Color(0, 1, 0, 0.5f); // Semi-transparent green initially

        // Remove colliders from ghost
        foreach (var col in ghostInstance.GetComponents<Collider2D>())
            Destroy(col);
    }

    private void DestroyGhost()
    {
        if (ghostInstance != null)
            Destroy(ghostInstance);
    }

    private Vector3 SnapToGrid(Vector3 pos)
    {
        return new Vector3(Mathf.Round(pos.x), Mathf.Round(pos.y), 0);
    }

    private bool IsInBuildZone(Vector2 pos)
    {
        foreach (var area in buildAreas)
        {
            if (area.IsWithinBounds(pos))
                return true;
        }
        return false;
    }

    private Vector3 GetBuildInputPosition()
    {
        Vector3 screenPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        screenPos.z = 0;
        return screenPos;
    }
}
