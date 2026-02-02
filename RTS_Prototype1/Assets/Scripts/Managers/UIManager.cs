using UnityEngine;
using TMPro;
using UnityEngine.AI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Game State UI")]
    [SerializeField] private TextMeshProUGUI creditsText;
    [SerializeField] private TextMeshProUGUI nationHealthText;

    [Header("Selection & Upgrades")]
    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private TextMeshProUGUI selectedUnitText;
    [SerializeField] private TextMeshProUGUI unitHealthText;
    [SerializeField] private TextMeshProUGUI unitFireRateText;
    [SerializeField] private TextMeshProUGUI unitSpeedText;


    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverPanel;

    private GameManager _gameManager;
    private PlayerUnit _currentSelectedUnit;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Disable panels by default
        if(upgradePanel != null) upgradePanel.SetActive(false);
        if(gameOverPanel != null) gameOverPanel.SetActive(false);
    }

    public void Initialize(GameManager gameManager)
    {
        _gameManager = gameManager;
        
        _gameManager.OnCreditsChanged += UpdateCreditsText;
        _gameManager.OnNationHealthChanged += UpdateNationHealthText;
        _gameManager.OnGameOver += ShowGameOverPanel;

        // Immediately update UI with initial values
        UpdateCreditsText(_gameManager.CurrentCredits);
        UpdateNationHealthText(_gameManager.CurrentNationHealth);
    }

    private void OnDestroy()
    {
        if (_gameManager != null)
        {
            _gameManager.OnCreditsChanged -= UpdateCreditsText;
            _gameManager.OnNationHealthChanged -= UpdateNationHealthText;
            _gameManager.OnGameOver -= ShowGameOverPanel;
        }
        // Ensure we unsubscribe if the UI Manager is destroyed while a unit is selected
        if (_currentSelectedUnit != null)
        {
            _currentSelectedUnit.OnStatsChanged -= UpdateStatsPanel;
        }
    }

    private void UpdateCreditsText(int newCredits)
    {
        if (creditsText != null)
        {
            creditsText.text = $"Credits: {newCredits}";
        }
    }

    private void UpdateNationHealthText(int newHealth)
    {
        if (nationHealthText != null)
        {
            nationHealthText.text = $"Nation Health: {newHealth}%";
        }
    }

    public void ShowUpgradePanel(GameObject selectedUnitGO)
    {
        if (upgradePanel == null) return;
        
        // Unsubscribe from the old unit if there was one
        if (_currentSelectedUnit != null)
        {
            _currentSelectedUnit.OnStatsChanged -= UpdateStatsPanel;
        }

        _currentSelectedUnit = selectedUnitGO.GetComponent<PlayerUnit>();

        // Subscribe to the new unit's event and show the panel
        if (_currentSelectedUnit != null)
        {
            _currentSelectedUnit.OnStatsChanged += UpdateStatsPanel;
            upgradePanel.SetActive(true);
            UpdateStatsPanel(); // Initial update
        }
    }

    public void HideUpgradePanel()
    {
        if (upgradePanel == null) return;
        
        upgradePanel.SetActive(false);
        if (_currentSelectedUnit != null)
        {
            _currentSelectedUnit.OnStatsChanged -= UpdateStatsPanel;
            _currentSelectedUnit = null;
        }
    }

    private void UpdateStatsPanel()
    {
        if (_currentSelectedUnit == null) return;

        if (selectedUnitText != null)
        {
            selectedUnitText.text = $"Selected: {_currentSelectedUnit.name}";
        }

        // Update stats
        Health health = _currentSelectedUnit.GetComponent<Health>();
        if (unitHealthText != null && health != null)
        {
            unitHealthText.text = $"Health: {health.CurrentHealth:F0} / {health.MaxHealth:F0}";
        }

        Weapon weapon = _currentSelectedUnit.GetComponent<Weapon>();
        if (unitFireRateText != null && weapon != null)
        {
            unitFireRateText.text = $"Fire Rate: {weapon.fireRate:F2}/s";
        }

        NavMeshAgent navAgent = _currentSelectedUnit.GetComponent<NavMeshAgent>();
        if (unitSpeedText != null && navAgent != null)
        {
            unitSpeedText.text = $"Speed: {navAgent.speed:F1} m/s";
        }
    }

    private void ShowGameOverPanel()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            HideUpgradePanel(); // Hide other UI
        }
    }
}

