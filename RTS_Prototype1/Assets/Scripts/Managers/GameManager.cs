using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game State")]
    [SerializeField] private int startingCredits = 500;
    [SerializeField] private int startingNationHealth = 100;
    
    public int CurrentCredits { get; private set; }
    public int CurrentNationHealth { get; private set; }

    public event Action<int> OnCreditsChanged;
    public event Action<int> OnNationHealthChanged;
    public event Action OnGameOver;

    private bool _isGameOver = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        CurrentCredits = startingCredits;
        CurrentNationHealth = startingNationHealth;
    }

    void Start()
    {
        // Initial UI update
        OnCreditsChanged?.Invoke(CurrentCredits);
        OnNationHealthChanged?.Invoke(CurrentNationHealth);
    }


    public void AddCredits(int amount)
    {
        if (_isGameOver) return;
        CurrentCredits += amount;
        OnCreditsChanged?.Invoke(CurrentCredits);
        Debug.Log($"Added {amount} credits. New balance: {CurrentCredits}");
    }

    public bool SpendCredits(int amount)
    {
        if (_isGameOver || CurrentCredits < amount)
        {
            return false;
        }
        CurrentCredits -= amount;
        OnCreditsChanged?.Invoke(CurrentCredits);
        Debug.Log($"Spent {amount} credits. New balance: {CurrentCredits}");
        return true;
    }

    public void DecreaseNationHealth(int amount)
    {
        if (_isGameOver) return;

        CurrentNationHealth -= amount;
        OnNationHealthChanged?.Invoke(CurrentNationHealth);
        Debug.Log($"Nation Health decreased by {amount}. New health: {CurrentNationHealth}");

        if (CurrentNationHealth <= 0)
        {
            CurrentNationHealth = 0;
            TriggerGameOver();
        }
    }

    private void TriggerGameOver()
    {
        if (_isGameOver) return;

        _isGameOver = true;
        Debug.Log("Game Over! The nation has fallen.");
        OnGameOver?.Invoke();
        // You could also pause the game here:
        // Time.timeScale = 0;
    }
}
