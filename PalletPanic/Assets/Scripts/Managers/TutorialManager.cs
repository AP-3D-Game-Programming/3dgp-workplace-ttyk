using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    [System.Serializable]
    public class TutorialStep
    {
        public string title;
        public KeyCode[] requiredKeys;
        public string[] keyDescriptions;
        public float holdDuration = 0.5f;
        public System.Action onComplete;
    }

    [SerializeField] private RectTransform tutorialBox;
    [SerializeField] private TMP_Text tutorialTitle;
    [SerializeField] private TMP_Text tutorialInstructions;
    [SerializeField] private Outline outlineEffect;

    // Completion Screen
    [SerializeField] private Canvas completionCanvas;
    [SerializeField] private RectTransform completionBox;
    [SerializeField] private TMP_Text completionTitle;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button nextLevelButton;

    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private float maxOutlineThickness = 5f;

    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color completedColor = Color.green;
    [SerializeField] private Color activeColor = Color.yellow;

    private List<TutorialStep> steps = new List<TutorialStep>();
    private int currentStep = -1;
    private int currentKeyIndex = 0;
    private PauseManager pauseManager;
    private bool isAnimating = false;
    private float keyHoldTimer = 0f;
    private bool stepCompleted = false;

    private bool isTutorialVisible = true;
    public bool IsTutorialVisible => isTutorialVisible;

    private void Start()
    {
        pauseManager = FindObjectOfType<PauseManager>();

        // Tutorial Box setup
        RectTransform rect = tutorialBox.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(0, 500);

        if (outlineEffect == null)
        {
            outlineEffect = tutorialBox.GetComponent<Outline>();
        }

        // Completion Screen setup - CHECK REFERENCES
        if (completionCanvas == null)
        {
            Debug.LogError("TutorialManager: completionCanvas not assigned!");
        }
        else
        {
            Debug.Log("TutorialManager: completionCanvas found!");
            completionCanvas.gameObject.SetActive(false);
        }

        if (completionBox == null)
        {
            Debug.LogError("TutorialManager: completionBox not assigned!");
        }

        if (completionTitle == null)
        {
            Debug.LogError("TutorialManager: completionTitle not assigned!");
        }

        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartLevel);
            Debug.Log("TutorialManager: Restart button listener added");
        }
        else
        {
            Debug.LogError("TutorialManager: restartButton not assigned!");
        }

        if (nextLevelButton != null)
        {
            nextLevelButton.onClick.AddListener(NextLevel);
            Debug.Log("TutorialManager: Next Level button listener added");
        }
        else
        {
            Debug.LogError("TutorialManager: nextLevelButton not assigned!");
        }

        InitializeTutorialSteps();
    }

    private void Update()
    {
        if (currentStep >= 0 && currentStep < steps.Count)
        {
            CheckControlInput();
        }
    }

    private void InitializeTutorialSteps()
    {
        // Step 1: Forward
        steps.Add(new TutorialStep
        {
            title = "Naar Voren Rijden",
            requiredKeys = new[] { KeyCode.Z },
            keyDescriptions = new[] { "Z" },
            holdDuration = 1.5f,
            onComplete = () => Debug.Log("✓ Forward completed")
        });

        // Step 2: Backward
        steps.Add(new TutorialStep
        {
            title = "Achteruit Rijden",
            requiredKeys = new[] { KeyCode.S },
            keyDescriptions = new[] { "S" },
            holdDuration = 1.5f,
            onComplete = () => Debug.Log("✓ Backward completed")
        });

        // Step 3: Turn Left
        steps.Add(new TutorialStep
        {
            title = "Links Draaien",
            requiredKeys = new[] { KeyCode.Q },
            keyDescriptions = new[] { "Q" },
            holdDuration = 1.5f,
            onComplete = () => Debug.Log("✓ Turn left completed")
        });

        // Step 4: Turn Right
        steps.Add(new TutorialStep
        {
            title = "Rechts Draaien",
            requiredKeys = new[] { KeyCode.D },
            keyDescriptions = new[] { "D" },
            holdDuration = 1.5f,
            onComplete = () => Debug.Log("✓ Turn right completed")
        });

        // Step 5: Lift Up
        steps.Add(new TutorialStep
        {
            title = "Vork Omhoog Tillen",
            requiredKeys = new[] { KeyCode.E },
            keyDescriptions = new[] { "E" },
            holdDuration = 1.5f,
            onComplete = () => Debug.Log("✓ Lift up completed")
        });

        // Step 6: Lift Down
        steps.Add(new TutorialStep
        {
            title = "Vork Omlaag Tillen",
            requiredKeys = new[] { KeyCode.A },
            keyDescriptions = new[] { "A" },
            holdDuration = 1.5f,
            onComplete = () => Debug.Log("✓ Lift down completed")
        });

        ShowNextStep();
    }

    public void ShowNextStep()
    {
        if (isAnimating) return;

        currentStep++;

        if (currentStep >= steps.Count)
        {
            CompleteTutorial();
            return;
        }

        currentKeyIndex = 0;
        stepCompleted = false;
        keyHoldTimer = 0f;
        StartCoroutine(AnimateBox(true));
    }

    private void UpdateInstructionText()
    {
        TutorialStep step = steps[currentStep];
        string currentKey = step.keyDescriptions[currentKeyIndex];
        string keyIcon = $"[{currentKey}]";
        string verb = GetVerbForKey(currentKey);
        tutorialInstructions.text = $"Gebruik {keyIcon} om {verb}";
    }

    private string GetVerbForKey(string key)
    {
        return key switch
        {
            "Z" => "vooruit te gaan",
            "S" => "achteruit te gaan",
            "Q" => "links te draaien",
            "D" => "rechts te draaien",
            "E" => "de vork omhoog te tillen",
            "A" => "de vork omlaag te tillen",
            _ => "de actie uit te voeren"
        };
    }

    private void CheckControlInput()
    {
        if (stepCompleted) return;

        TutorialStep step = steps[currentStep];
        KeyCode currentKey = step.requiredKeys[currentKeyIndex];

        if (Input.GetKey(currentKey))
        {
            keyHoldTimer += Time.deltaTime;
            float progress = keyHoldTimer / step.holdDuration;

            UpdateOutline(progress);

            if (progress < 0.5f)
            {
                tutorialInstructions.color = Color.Lerp(normalColor, activeColor, progress * 2f);
            }
            else
            {
                tutorialInstructions.color = Color.Lerp(activeColor, completedColor, (progress - 0.5f) * 2f);
            }

            if (keyHoldTimer >= step.holdDuration)
            {
                CompleteKeyStep();
            }
        }
        else
        {
            keyHoldTimer = 0f;
            ResetOutline();
            tutorialInstructions.color = normalColor;
        }
    }

    private void CompleteKeyStep()
    {
        TutorialStep step = steps[currentStep];
        currentKeyIndex++;

        if (currentKeyIndex >= step.requiredKeys.Length)
        {
            CompleteControlStep();
        }
        else
        {
            keyHoldTimer = 0f;
            ResetOutline();
            tutorialInstructions.color = normalColor;
            UpdateInstructionText();
        }
    }

    private void UpdateOutline(float progress)
    {
        if (outlineEffect == null) return;

        float thickness = Mathf.Lerp(1f, maxOutlineThickness, progress);
        outlineEffect.effectDistance = new Vector2(thickness, thickness);
        outlineEffect.effectColor = Color.Lerp(activeColor, completedColor, progress);
    }

    private void ResetOutline()
    {
        if (outlineEffect == null) return;

        outlineEffect.effectDistance = new Vector2(2, 2);
        outlineEffect.effectColor = Color.yellow;
    }

    private void CompleteControlStep()
    {
        stepCompleted = true;
        tutorialInstructions.color = completedColor;
        tutorialInstructions.text = "✓ Compleet!";

        if (outlineEffect != null)
        {
            outlineEffect.effectDistance = new Vector2(maxOutlineThickness, maxOutlineThickness);
            outlineEffect.effectColor = completedColor;
        }

        Debug.Log($"✓ Step {currentStep + 1} completed!");

        Invoke(nameof(ShowNextStep), 1.5f);
    }

    private IEnumerator AnimateBox(bool slideIn, System.Action onComplete = null)
    {
        isAnimating = true;
        RectTransform rect = tutorialBox.GetComponent<RectTransform>();

        if (slideIn)
        {
            TutorialStep step = steps[currentStep];
            tutorialTitle.text = step.title;
            tutorialTitle.color = normalColor;
            ResetOutline();
            UpdateInstructionText();

            yield return SlideBox(500, 0, animationDuration);

            while (!stepCompleted)
            {
                yield return null;
            }

            yield return SlideBox(0, -500, animationDuration);
        }
        else
        {
            yield return SlideBox(0, 500, animationDuration);
        }

        isAnimating = false;
        onComplete?.Invoke();
    }

    private IEnumerator SlideBox(float startY, float endY, float duration)
    {
        RectTransform rect = tutorialBox.GetComponent<RectTransform>();
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float y = Mathf.Lerp(startY, endY, t);
            rect.anchoredPosition = new Vector2(0, y);
            yield return null;
        }

        rect.anchoredPosition = new Vector2(0, endY);
    }

    private void CompleteTutorial()
    {
        Debug.Log("CompleteTutorial() called!");

        tutorialTitle.text = "Tutorial Compleet! 🎉";
        tutorialTitle.color = completedColor;
        tutorialInstructions.text = "Je bent klaar voor de missie!";
        tutorialInstructions.color = completedColor;

        if (outlineEffect != null)
        {
            outlineEffect.effectDistance = new Vector2(maxOutlineThickness, maxOutlineThickness);
            outlineEffect.effectColor = completedColor;
        }

        // Hide tutorial box en show completion screen
        StartCoroutine(ShowCompletionScreenCoroutine());
    }

    /// <summary>
    /// Coroutine voor completion screen
    /// </summary>
    private IEnumerator ShowCompletionScreenCoroutine()
    {
        Debug.Log("ShowCompletionScreenCoroutine() started!");

        // Slide out tutorial box
        yield return SlideBox(0, -500, animationDuration);

        // Wait een moment
        yield return new WaitForSeconds(0.5f);

        // Show completion screen
        ShowCompletionScreen();
    }

    /// <summary>
    /// Toon completion screen met buttons
    /// </summary>
    private void ShowCompletionScreen()
    {
        Debug.Log("ShowCompletionScreen() called!");

        if (completionCanvas == null)
        {
            Debug.LogError("completionCanvas is NULL!");
            return;
        }

        Debug.Log("Activating completion canvas...");
        completionCanvas.gameObject.SetActive(true);

        if (completionTitle != null)
        {
            completionTitle.text = "Level Compleet! 🎉";
            Debug.Log("Completion title set");
        }

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        Time.timeScale = 0f;

        Debug.Log("✓ Completion screen shown!");
    }

    /// <summary>
    /// Restart level
    /// </summary>
    private void RestartLevel()
    {
        Debug.Log("RestartLevel() called!");
        Time.timeScale = 1f;
        SceneManager.UnloadSceneAsync("Level0");
        SceneManager.LoadScene("Level0", LoadSceneMode.Additive);
    }

    /// <summary>
    /// Load next level
    /// </summary>
    private void NextLevel()
    {
        Debug.Log("NextLevel() called!");
        Time.timeScale = 1f;
        SceneManager.UnloadSceneAsync("Level0");
        SceneManager.LoadScene("Level1", LoadSceneMode.Additive);
    }

    public void ToggleTutorial()
    {
        isTutorialVisible = !isTutorialVisible;
        tutorialBox.gameObject.SetActive(isTutorialVisible);
        Debug.Log($"Tutorial: {(isTutorialVisible ? "shown" : "hidden")}");
    }

    public int GetCurrentStep() => currentStep;
    public int GetTotalSteps() => steps.Count;
}