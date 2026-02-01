using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Game State UI")]
    [SerializeField] private TextMeshProUGUI creditsText;
    [SerializeField] private TextMeshProUGUI nationHealthText;

    [Header("Selection & Upgrades")]
    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private TextMeshProUGUI selectedUnitText;

    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverPanel;

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

    void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnCreditsChanged += UpdateCreditsText;
            GameManager.Instance.OnNationHealthChanged += UpdateNationHealthText;
            GameManager.Instance.OnGameOver += ShowGameOverPanel;
        }
    }

    void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnCreditsChanged -= UpdateCreditsText;
            GameManager.Instance.OnNationHealthChanged -= UpdateNationHealthText;
            GameManager.Instance.OnGameOver -= ShowGameOverPanel;
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

    public void ShowUpgradePanel(GameObject selectedUnit)
    {
        if (upgradePanel != null)
        {
            upgradePanel.SetActive(true);
            if (selectedUnitText != null)
            {
                selectedUnitText.text = $"Selected: {selectedUnit.name}";
            }
        }
    }

    public void HideUpgradePanel()
    {
        if (upgradePanel != null)
        {
            upgradePanel.SetActive(false);
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

