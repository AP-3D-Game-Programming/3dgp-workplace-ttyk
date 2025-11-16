using UnityEngine;
using TMPro;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private TextMeshProUGUI instructionText;
    [SerializeField] private GameObject completionCheckmark;
    [SerializeField] private GameObject completionButtons;

    [Header("References")]
    [SerializeField] private Transform forklift;
    [SerializeField] public Transform pallet;
    [SerializeField] private Transform targetShelf;

    [Header("Settings")]
    [SerializeField] private float requiredDistance = 4f;

    private enum TutorialStep
    {
        Welcome,
        CameraToggle,
        MoveForward,
        MoveBackward,
        MoveLeft,
        MoveRight,
        PickupPallet,
        PlacePallet,
        Complete
    }

    private TutorialStep currentStep = TutorialStep.Welcome;
    private bool stepCompleted = false;
    private bool palletPickedUp = false;
    private bool palletPlacedOnce = false;
    private Vector3 startMovePos;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (tutorialPanel != null) tutorialPanel.SetActive(true);
        if (completionButtons != null) completionButtons.SetActive(false);
        StartTutorial();
    }

    private void Update()
    {
        if (currentStep == TutorialStep.Welcome && Input.GetKeyDown(KeyCode.Space))
        {
            NextStep();
            return;
        }

        if (stepCompleted) return;

        switch (currentStep)
        {
            case TutorialStep.CameraToggle:
                if (Input.GetKeyDown(KeyCode.C)) CompleteStep();
                break;

            case TutorialStep.MoveForward:
                CheckDistanceMoved(Vector3.forward);
                break;

            case TutorialStep.MoveBackward:
                CheckDistanceMoved(Vector3.back);
                break;

            case TutorialStep.MoveLeft:
                CheckTurnAndForward(Vector3.left);
                break;

            case TutorialStep.MoveRight:
                CheckTurnAndForward(Vector3.right);
                break;

            case TutorialStep.PickupPallet:
                if (palletPickedUp) CompleteStep();
                break;

            case TutorialStep.PlacePallet:
                CheckPalletPlacement();
                break;
        }
    }

    private void StartTutorial()
    {
        currentStep = TutorialStep.Welcome;
        palletPlacedOnce = false;
        ShowInstruction("<color=#00FFFF>Welkom bij de Forklift Training!</color>\n\nDruk op <color=#FFFF00>SPATIE</color> om te beginnen.");
    }

    private void ShowInstruction(string text)
    {
        if (instructionText != null)
            instructionText.text = text;
    }

    private void NextStep()
    {
        currentStep++;
        stepCompleted = false;

        switch (currentStep)
        {
            case TutorialStep.CameraToggle:
                ShowInstruction("Druk op <color=#FFFF00>C</color> om de <color=#FFA500>camera</color> te wisselen tussen verschillende zichtpunten van de heftruck.");
                break;

            case TutorialStep.MoveForward:
                startMovePos = forklift.position;
                ShowInstruction("Rijd <color=#FFFF00>vooruit</color> met <color=#00FF00>W</color> of <color=#00FF00>Pijl Omhoog</color>.\nBlijf rijden tot de voortgang <color=#00FFFF>100%</color> bereikt.");
                break;

            case TutorialStep.MoveBackward:
                startMovePos = forklift.position;
                ShowInstruction("Rijd <color=#FFFF00>achteruit</color> met <color=#00FF00>S</color>.");
                break;

            case TutorialStep.MoveLeft:
                startMovePos = forklift.position;
                ShowInstruction("Draai en rijd <color=#FFFF00>naar links</color> door <color=#00FF00>W + A tegelijk</color> in te drukken.\nZo maak je een bocht naar links.");
                break;

            case TutorialStep.MoveRight:
                startMovePos = forklift.position;
                ShowInstruction("Draai en rijd <color=#FFFF00>naar rechts</color> door <color=#00FF00>W + D tegelijk</color> in te drukken.\nZo maak je een bocht naar rechts.");
                break;

            case TutorialStep.PickupPallet:
                ShowInstruction("Rijd naar het <color=#FFFF00>pallet</color> toe en gebruik <color=#FFFF00>E</color> om de vorken te heffen en <color=#FFFF00>Q</color> om te laten zakken.\nPlaats de vorken onder het pallet en til het voorzichtig op.");
                break;

            case TutorialStep.PlacePallet:
                ShowInstruction("Rijd met het pallet naar de <color=#FFFF00>shelf</color> en plaats het er netjes op.\nLaat het los met <color=#FFFF00>Q</color> als het goed staat.");
                break;

            case TutorialStep.Complete:
                ShowInstruction("<color=#00FF00>Tutorial voltooid!</color>\nJe bent nu klaar om als heftruckchauffeur aan de slag te gaan!");
                StartCoroutine(ShowCompletionButtons());
                break;
        }
    }

    private void CompleteStep()
    {
        stepCompleted = true;
        StartCoroutine(ShowCheckmark());
        StartCoroutine(AdvanceAfterDelay());
    }

    private IEnumerator ShowCheckmark()
    {
        if (completionCheckmark != null)
        {
            completionCheckmark.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            completionCheckmark.SetActive(false);
        }
    }

    private IEnumerator AdvanceAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        NextStep();
    }

    private void CheckDistanceMoved(Vector3 localDir)
    {
        if (forklift == null) return;

        Vector3 delta = forklift.position - startMovePos;
        Vector3 localMovement = forklift.InverseTransformDirection(delta);

        float distance = 0f;
        if (localDir == Vector3.forward) distance = localMovement.z;
        else if (localDir == Vector3.back) distance = -localMovement.z;

        distance = Mathf.Max(0, distance);
        float progress = Mathf.Clamp01(distance / requiredDistance);
        int percent = Mathf.RoundToInt(progress * 100f);

        string dir = (localDir == Vector3.forward) ? "vooruit" : "achteruit";
        ShowInstruction($"Rijd {dir}... <color=#00FFFF>{percent}%</color>\nGebruik <color=#00FF00>{GetKeysForDirection(dir)}</color>.");

        if (progress >= 1f)
            CompleteStep();
    }

    private void CheckTurnAndForward(Vector3 turnDir)
    {
        if (forklift == null) return;

        Vector3 delta = forklift.position - startMovePos;
        Vector3 localMovement = forklift.InverseTransformDirection(delta);

        float forwardDist = Mathf.Max(0, localMovement.z);
        float turnEffect = Mathf.Abs(localMovement.x);
        float progress = Mathf.Clamp01((forwardDist + turnEffect * 0.7f) / requiredDistance);

        int percent = Mathf.RoundToInt(progress * 100f);
        string dir = (turnDir == Vector3.left) ? "naar links" : "naar rechts";

        ShowInstruction($"Draai {dir} terwijl je vooruit rijdt... <color=#00FFFF>{percent}%</color>\nGebruik <color=#00FF00>{GetKeysForDirection(dir)}</color> tegelijk.");

        if (progress >= 1f)
            CompleteStep();
    }

    private string GetKeysForDirection(string dir)
    {
        switch (dir)
        {
            case "vooruit": return "W";
            case "achteruit": return "S";
            case "naar links": return "W + A";
            case "naar rechts": return "W + D";
            default: return "";
        }
    }

    private void CheckPalletPlacement()
    {
        if (!palletPickedUp && !palletPlacedOnce)
        {
            palletPlacedOnce = true;
            ShowInstruction("<color=#00FF00>Pallet neergezet!</color>\n\nEven controleren...");
            StartCoroutine(DelayedCompletion());
        }
    }

    public void OnPalletPickedUp() => palletPickedUp = true;
    public void OnPalletReleased() => palletPickedUp = false;

    private IEnumerator DelayedCompletion()
    {
        yield return new WaitForSeconds(1f);
        CompleteStep();
    }

    private IEnumerator ShowCompletionButtons()
    {
        yield return new WaitForSeconds(1f);

        if (completionButtons != null)
        {
            completionButtons.SetActive(true);
        }

        if (instructionText != null)
            instructionText.gameObject.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OnGoToLevel1ButtonPressed()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (completionButtons != null)
            completionButtons.SetActive(false);

        if (tutorialPanel != null)
            tutorialPanel.SetActive(false);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnIntroComplete();
        }
    }

    public void OnRestartTutorialButtonPressed()
    {

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (completionButtons != null)
            completionButtons.SetActive(false);

        if (instructionText != null)
            instructionText.gameObject.SetActive(true);

        currentStep = TutorialStep.Welcome;
        stepCompleted = false;
        palletPickedUp = false;
        palletPlacedOnce = false;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadIntro();
        }
    }
}
