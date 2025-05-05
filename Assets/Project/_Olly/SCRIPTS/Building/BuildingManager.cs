using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;


public class BuildManager : MonoBehaviour
{
    #region INSPECTOR VARIABLES
    public static BuildManager Instance;

    public BuildableData[] buildItems;
    
    // public GameObject[] buildPrefabs; // e.g., wall, turret
    public BuildArea[] buildAreas;
    public LayerMask nonBuildableLayers;

    public GameObject placeParticlesPrefab = null;
    public AudioClip placeSfx = null;
    #endregion

    #region PRIVATE VARIABLES
    private int selectedIndex = 0;
    private bool isInBuildMode = false;

    private GameObject ghostInstance;
    private bool lastValidPlacement = true;
    private InputController _inputController;
    private UpgradableBuilding currentlySelectedUpgradableBuilding;
    #endregion


    private void Awake() => Instance = this;

    void Start()
    {
        _inputController = Player.Instance.InputController;
        // foreach (var area in buildAreas)
        //     area.ShowGrid(false);
    }

    public void ToggleBuildMode()
    {
        isInBuildMode = !isInBuildMode;

        foreach (var area in buildAreas)
            area.ShowGridVisual(isInBuildMode);

        if (isInBuildMode){
            CreateGhost();
        }
        else
        {
            DestroyGhost();
            // if (WaveManager.Instance.IsBetweenWaves && Input.GetKeyDown(KeyCode.E))
        }

        
    }

    private void Update()
    {
        GetHoveredTower();

        if (Input.GetKeyDown(KeyCode.B))
        {
            ToggleBuildMode();
        }


        if(Input.GetKeyDown(KeyCode.R))
        {
            // var hoveredTower = GetHoveredTower();
            if (currentlySelectedUpgradableBuilding != null && currentlySelectedUpgradableBuilding.CanUpgrade)
            {
                currentlySelectedUpgradableBuilding.TryUpgrade();
            }
        }

        if (!isInBuildMode || ghostInstance == null)
            return;

        Vector3 targetPos = SnapToGrid(GetBuildInputPosition());
        ghostInstance.transform.position = targetPos;

        // bool isInZone = IsInBuildZone(targetPos);
        
        
        // bool isClear = !Physics2D.OverlapBox(checkPos, boxSize, 0f, nonBuildableLayers);
        // bool isClear = !Physics2D.OverlapCircle(targetPos, 0.4f, nonBuildableLayers);
        bool validNow = IsPlacementClear();

        // Shake if just became invalid
        if (!validNow && lastValidPlacement)
        {
            ghostInstance.transform.DOKill();
            ghostInstance.transform.DOShakePosition(0.2f, 0.15f, 10, 90);
        }

        // Place object
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            int cost = buildItems[selectedIndex].cost;
            if (validNow && ScrapManager.Instance.TrySpendScrap(cost))
            {
                GameObject placed = Instantiate(buildItems[selectedIndex].buildableGO, targetPos, Quaternion.identity);
                SetLayerRecursively(placed, LayerMask.NameToLayer("Buildable"));
                PlayPlacementFeedback(placed);
                // Gets rid of the parent collider so we have no issues with colliding etc
                Destroy(placed.GetComponent<BoxCollider2D>());
            }
            else if (!validNow)
            {
                PlayInvalidPlacementFeedback(ghostInstance);
            }
            else
            {
                // Not enough scrap
                Debug.Log("Not enough scrap!");
                PlayInvalidPlacementFeedback(ghostInstance);
            }
            #region  OLD CODE BEFORE SCRAP CURRENCY
            // if (validNow)
            // {
            //     GameObject placed = Instantiate(buildItems[selectedIndex].buildableGO, targetPos, Quaternion.identity);
            //     SetLayerRecursively(placed, LayerMask.NameToLayer("Buildable"));
            //     PlayPlacementFeedback(placed);
            //     Destroy(placed.GetComponent<BoxCollider2D>());
            // }
            // else
            // {
            //     PlayInvalidPlacementFeedback(ghostInstance);
            // }
            #endregion
        }
        
        lastValidPlacement = validNow;
        
        #region OLD CODE
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
        #endregion
    }

    // private bool IsPlacementClear()
    // {
    //     BoxCollider2D ghostCollider = ghostInstance.GetComponentInChildren<BoxCollider2D>();
    //     Vector2 size = ghostCollider.size;
    //     Vector2 offset = ghostCollider.offset;
    //     Vector2 position = (Vector2)ghostInstance.transform.position + offset;

    //     return !Physics2D.OverlapBox(position, size, 0f, nonBuildableLayers);
    // }

    private bool IsPlacementClear()
    {
        if (ghostInstance == null)
            return false;

        BoxCollider2D ghostCollider = ghostInstance.GetComponentInChildren<BoxCollider2D>();
        Bounds bounds = ghostCollider.bounds;

        foreach (var area in buildAreas)
        {
            // Get the cells occupied by the bounds within this build area
            List<Vector2Int> occupiedCells = area.GetOccupiedCells(bounds);

            if (occupiedCells.Count == 0 || !area.AreCellsValid(occupiedCells))
                continue;

            // Check for physical overlap with blocked layers
            bool blocked = Physics2D.OverlapBox(bounds.center, bounds.size, 0f, nonBuildableLayers);
            if (!blocked)
                return true;
        }

        return false;
    }


    private void CreateGhost()
    {
        DestroyGhost(); // Clean up any old ghost

        ghostInstance = Instantiate(buildItems[selectedIndex].buildableGO);
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
    private UpgradableBuilding GetHoveredTower()
    {
        Vector3 mouseWorldPos = GetBuildInputPosition();
        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero, 0f, LayerMask.GetMask("Buildable"));

        UpgradableBuilding hovered = hit.collider != null 
            ? hit.collider.GetComponentInParent<UpgradableBuilding>() 
            : null;

        if (hovered != currentlySelectedUpgradableBuilding)
        {
            // Hide the old one if it exists
            if (currentlySelectedUpgradableBuilding != null)
            {
                currentlySelectedUpgradableBuilding.CanvasParent.SetActive(false);
            }

            currentlySelectedUpgradableBuilding = hovered;

            // Show the new one
            if (currentlySelectedUpgradableBuilding != null)
            {
                if (WaveManager.Instance.IsBuildPhase())
                {
                    currentlySelectedUpgradableBuilding.SetUpgradeUI();
                }
            }
        }

        return currentlySelectedUpgradableBuilding;
    }

    private Vector3 SnapToGrid(Vector3 pos)
    {
        return new Vector3(Mathf.Round(pos.x), Mathf.Round(pos.y), 0);
    }

    private Vector3 GetBuildInputPosition()
    {
        Vector3 screenPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        screenPos.z = 0;
        return screenPos;
    }

    void PlayPlacementFeedback(GameObject placed)
    {
        placed.transform.DOPunchScale(
            punch: Vector3.one * 0.3f, // Bigger punch
            duration: 0.25f,
            vibrato: 10,
            elasticity: 0.5f
        ).SetEase(Ease.OutExpo);


        if (placeParticlesPrefab != null)
            Instantiate(placeParticlesPrefab, placed.transform.position, Quaternion.identity);

        if (placeSfx != null)
            AudioSource.PlayClipAtPoint(placeSfx, placed.transform.position);
    }

    void PlayInvalidPlacementFeedback(GameObject placed){
        ghostInstance.transform.DOKill(); // Stop any existing tweens
        // Stronger shake when placement is invalid
        ghostInstance.transform.DOShakePosition(
            duration: 0.4f,
            strength: 0.3f,
            vibrato: 30,
            randomness: 90,
            snapping: false,
            fadeOut: true
        ).SetEase(Ease.OutBack);
    }
}
