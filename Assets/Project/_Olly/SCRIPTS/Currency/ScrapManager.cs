using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ScrapManager : MonoBehaviour
{
    public static ScrapManager Instance;

    public int CurrentScrap { get; private set; } = 0;

    // Events
    public UnityEvent<int> OnScrapChanged = new UnityEvent<int>();
    public UnityEvent<int> OnScrapGained = new UnityEvent<int>();
    public UnityEvent<int> OnScrapSpent = new UnityEvent<int>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void AddScrap(int amount)
    {
        CurrentScrap += amount;
        OnScrapChanged.Invoke(CurrentScrap);
        OnScrapGained.Invoke(amount);
    }

    public bool TrySpendScrap(int amount)
    {
        if (CurrentScrap >= amount)
        {
            CurrentScrap -= amount;
            OnScrapChanged.Invoke(CurrentScrap);
            OnScrapSpent.Invoke(amount);
            return true;
        }
        return false;
    }
}