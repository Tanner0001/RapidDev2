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
        // Find and initialize the UIManager
        UIManager uiManager = FindFirstObjectByType<UIManager>();
        if (uiManager != null)
        {
            uiManager.Initialize(this);
        }
        else
        {
            Debug.LogError("GameManager could not find a UIManager in the scene!");
        }
    }


    public void AddCredits(int amount)
    {
        if (_isGameOver) return;
        CurrentCredits += amount;
        OnCreditsChanged?.Invoke(CurrentCredits);

    }

    public bool SpendCredits(int amount)
    {
        if (_isGameOver || CurrentCredits < amount)
        {
            return false;
        }
        CurrentCredits -= amount;
        OnCreditsChanged?.Invoke(CurrentCredits);

        return true;
    }

    public void DecreaseNationHealth(int amount)
    {
        if (_isGameOver) return;

        CurrentNationHealth -= amount;
        OnNationHealthChanged?.Invoke(CurrentNationHealth);


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

        OnGameOver?.Invoke();
        // You could also pause the game here:
        // Time.timeScale = 0;
    }
}
