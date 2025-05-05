using DG.Tweening;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance;

    public GameObject[] buildPrefabs; // e.g., wall, turret
    public BuildArea[] buildAreas;
    public LayerMask nonBuildableLayers;
    private int selectedIndex = 0;
    private bool isInBuildMode = false;

    private GameObject ghostInstance;
    private bool lastValidPlacement = true;


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
        if (Input.GetKeyDown(KeyCode.B))
        {
            ToggleBuildMode();
        }


        if (!isInBuildMode || ghostInstance == null)
            return;

        Vector3 targetPos = SnapToGrid(GetBuildInputPosition());
        ghostInstance.transform.position = targetPos;

        bool isInZone = IsInBuildZone(targetPos);
        // bool isClear = !Physics2D.OverlapBox(checkPos, boxSize, 0f, nonBuildableLayers);
        // bool isClear = !Physics2D.OverlapCircle(targetPos, 0.4f, nonBuildableLayers);
        bool validNow = isInZone && IsPlacementClear();

        // Shake if just became invalid
        if (!validNow && lastValidPlacement)
        {
            ghostInstance.transform.DOKill();
            ghostInstance.transform.DOShakePosition(0.2f, 0.15f, 10, 90);
        }

        // Place object
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (validNow)
            {
                GameObject placed = Instantiate(buildPrefabs[selectedIndex], targetPos, Quaternion.identity);
                SetLayerRecursively(placed, LayerMask.NameToLayer("Buildable"));
                placed.transform.DOPunchScale(Vector3.one * 0.2f, 0.2f, 10, 1);
                // Then we destroy the box collider that is parented on the object
                // as the children will have gameobjects
                Destroy(ghostInstance.GetComponent<BoxCollider2D>());
            }
            else
            {
                ghostInstance.transform.DOKill(); // Stop any existing tweens
                ghostInstance.transform.localPosition = Vector3.zero; // Reset just in case
                // Stronger shake when placement is invalid
                ghostInstance.transform.DOShakePosition(
                    duration: 0.4f,
                    strength: 0.2f,
                    vibrato: 30,
                    randomness: 90,
                    snapping: false,
                    fadeOut: true
                ).SetEase(Ease.OutBack);
            }
        }
        
        // if (validNow && Input.GetKeyDown(KeyCode.E)) // Swap with Input System action later
        // {
        //     Debug.Log("Placing buildable at: " + targetPos);
        //     GameObject placed = Instantiate(buildPrefabs[selectedIndex], targetPos, Quaternion.identity);
            
        //     // changing the newly placed building to the buildable layer to avoid building in place
        //     SetLayerRecursively(placed, LayerMask.NameToLayer("Buildable"));
            
        //     // Little pop animation
        //     placed.transform.DOPunchScale(Vector3.one * 0.2f, 0.2f, 10, 1);
            
        //     // Then we destroy the box collider that is parented on the object
        //     // as the children will have gameobjects
        //     Destroy(ghostInstance.GetComponent<BoxCollider2D>());
        // }

        lastValidPlacement = validNow;
    }

    private bool IsPlacementClear()
    {
        BoxCollider2D ghostCollider = ghostInstance.GetComponentInChildren<BoxCollider2D>();
        Vector2 size = ghostCollider.size;
        Vector2 offset = ghostCollider.offset;
        Vector2 position = (Vector2)ghostInstance.transform.position + offset;

        return !Physics2D.OverlapBox(position, size, 0f, nonBuildableLayers);
    }


    private void CreateGhost()
    {
        DestroyGhost(); // Clean up any old ghost

        ghostInstance = Instantiate(buildPrefabs[selectedIndex]);
        ghostInstance.transform.localScale = Vector3.one * 0.95f; // slightly smaller to look ghosty  
    }

    void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
            SetLayerRecursively(child.gameObject, newLayer);
    }

    private void DestroyGhost()
    {
        if (ghostInstance != null)
        {
            Destroy(ghostInstance);
            ghostInstance = null;
        }
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
