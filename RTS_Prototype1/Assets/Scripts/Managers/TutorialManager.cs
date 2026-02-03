using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Events;

/// <summary>
/// A single step in the tutorial sequence.
/// </summary>
[System.Serializable]
public class TutorialStep
{
    [Tooltip("The instructional text to be displayed for this step.")]
    [TextArea(3, 10)]
    public string tutorialText;

    [Tooltip("Optional event to trigger when this step starts. Use this to highlight UI, spawn units, etc.")]
    public UnityEvent onStepStart;
}

/// <summary>
/// A singleton manager for running a sequence of tutorial steps.
/// </summary>
public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    [Header("Tutorial Steps")]
    [Tooltip("The sequence of steps for the tutorial.")]
    [SerializeField] private List<TutorialStep> tutorialSteps = new List<TutorialStep>();

    [Header("UI References")]
    [Tooltip("The parent GameObject for the tutorial UI panel.")]
    [SerializeField] private GameObject tutorialPanel;

    [Tooltip("The TextMeshPro UGUI component to display the tutorial text.")]
    [SerializeField] private TextMeshProUGUI tutorialTextUI;

    [Tooltip("The button to advance to the next tutorial step.")]
    [SerializeField] private Button nextButton;

    private int _currentStepIndex = 0;
    private bool _isTutorialActive = false;

    void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Ensure the panel is hidden on awake
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);
        }
    }

    void Start()
    {
        if (nextButton != null)
        {
            nextButton.onClick.AddListener(ShowNextStep);
        }
    }

    /// <summary>
    /// Starts the tutorial sequence from the beginning.
    /// </summary>
    public void StartTutorial()
    {
        if (tutorialSteps.Count == 0)
        {
            Debug.LogWarning("TutorialManager: No tutorial steps have been defined.");
            return;
        }

        _isTutorialActive = true;
        _currentStepIndex = 0;
        ShowStep(_currentStepIndex);
    }

    /// <summary>
    /// Displays the next step in the tutorial sequence or ends the tutorial if it's the last step.
    /// </summary>
    public void ShowNextStep()
    {
        if (!_isTutorialActive) return;

        _currentStepIndex++;

        if (_currentStepIndex < tutorialSteps.Count)
        {
            ShowStep(_currentStepIndex);
        }
        else
        {
            EndTutorial();
        }
    }

    /// <summary>
    /// Shows a specific tutorial step by its index.
    /// </summary>
    /// <param name="stepIndex">The index of the step to show.</param>
    private void ShowStep(int stepIndex)
    {
        if (stepIndex < 0 || stepIndex >= tutorialSteps.Count)
        {
            Debug.LogError($"TutorialManager: Invalid step index {stepIndex}.");
            return;
        }

        TutorialStep currentStep = tutorialSteps[stepIndex];

        // Update UI
        if (tutorialTextUI != null)
        {
            tutorialTextUI.text = currentStep.tutorialText;
        }

        // Show the panel
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(true);
        }

        // Trigger the event for this step
        currentStep.onStepStart?.Invoke();
    }

    /// <summary>
    /// Ends the tutorial and hides the UI panel.
    /// </summary>
    public void EndTutorial()
    {
        _isTutorialActive = false;
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);
        }
        Debug.Log("Tutorial complete.");
    }

    // Example of how you might start the tutorial.
    // You could call this from a button click or another event.
    [ContextMenu("Start Tutorial Example")]
    private void StartTutorialFromEditor()
    {
        StartTutorial();
    }
}
