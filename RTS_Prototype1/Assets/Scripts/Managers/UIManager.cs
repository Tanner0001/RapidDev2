using UnityEngine;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private TextMeshProUGUI selectedShipStatusText; // For real-time state
    [SerializeField] private float displayTime = 3f;

    private Coroutine _displayCoroutine;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("UIManager: Duplicate UIManager found, destroying this one.", this);
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        if (statusText == null)
        {
            Debug.LogError("UIManager: StatusText is not assigned in the inspector!", this);
        }
        if (selectedShipStatusText == null)
        {
            Debug.LogError("UIManager: SelectedShipStatusText is not assigned in the inspector!", this);
        }

        if (statusText != null) statusText.text = ""; // Start with empty text
        if (selectedShipStatusText != null) selectedShipStatusText.text = "Status: None";
    }

    /// <summary>
    /// Displays a temporary status message after a hail/scan.
    /// </summary>
    public void DisplayShipStatus(Transform shipTransform, ThreatLevel threatLevel)
    {
        Debug.Log($"UIManager: Received request to display status for {shipTransform.name} with threat {threatLevel}.");
        if (_displayCoroutine != null)
        {
            StopCoroutine(_displayCoroutine);
        }
        _displayCoroutine = StartCoroutine(DisplayStatusCoroutine(shipTransform, threatLevel));
    }

    /// <summary>
    /// Updates the persistent status display for the currently selected ship.
    /// </summary>
    public void UpdateSelectedShipStatus(string shipName, UnitState currentState)
    {
        if (selectedShipStatusText != null)
        {
            selectedShipStatusText.text = $"Ship Selected: {shipName} - Status: {currentState}";
        }
    }
    
    public void ClearSelectedShipStatus()
    {
        if (selectedShipStatusText != null)
        {
            selectedShipStatusText.text = "No Ship Selected";
        }
    }

    private IEnumerator DisplayStatusCoroutine(Transform shipTransform, ThreatLevel threatLevel)
    {
        string statusMessage = $"Hailed {shipTransform.name}: Status is {threatLevel}";
        statusText.text = statusMessage;
        
        yield return new WaitForSeconds(displayTime);
        
        // If the text hasn't been changed by another call, clear it
        if (statusText.text == statusMessage)
        {
            statusText.text = "";
        }
    }
}
