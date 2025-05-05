using DG.Tweening;
using TMPro;
using UnityEngine;

public class UpgradableBuilding : MonoBehaviour
{
    [SerializeField] private GameObject[] levels; // Level 1 -> 3
    [SerializeField] private int[] upgradeCosts = { 25, 50 }; // Cost to go to level 2, then level 3
    [SerializeField] private GameObject canvasUIGO;
    [SerializeField] private TextMeshProUGUI upgradeCostText;
    
    public GameObject CanvasParent {get => canvasUIGO;}

    private int currentLevel = 0;

    public bool CanUpgrade => currentLevel < levels.Length - 1;

    public int GetNextUpgradeCost()
    {
        return CanUpgrade ? upgradeCosts[currentLevel] : -1;
    }

    public void TryUpgrade()
    {
        if (!CanUpgrade) return;

        int cost = GetNextUpgradeCost();
        if (ScrapManager.Instance.CurrentScrap < cost) return;

        ScrapManager.Instance.TrySpendScrap(cost);

        levels[currentLevel].SetActive(false);
        currentLevel++;
        levels[currentLevel].SetActive(true);

        // Optionally add a small pop effect
        levels[currentLevel].transform.DOPunchScale(Vector3.one * 0.2f, 0.2f, 8, 1);
    
        // ✅ Refresh the UI with new upgrade cost (or hide it if max level)
        SetUpgradeUI();
    }

    public int CurrentLevel => currentLevel + 1;

    public void SetUpgradeUI() 
    {
        if(upgradeCostText != null && CurrentLevel < levels.Length) 
        {
            CanvasParent.SetActive(true);
            upgradeCostText.text = upgradeCosts[currentLevel].ToString();
        }else{
            CanvasParent.SetActive(false);
        }
    }

}

