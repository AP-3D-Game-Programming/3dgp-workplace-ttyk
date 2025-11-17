using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private Canvas pauseCanvas;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button quitButton;

    private TutorialManager tutorialManager;
    private bool isPaused = false;
    private bool isInLevel = false; // Check if we're in Level0

    public bool IsPaused => isPaused;

    private void Start()
    {
        if (pauseCanvas == null)
        {
            Debug.LogError("PauseManager: pauseCanvas not assigned!");
            return;
        }

        pauseCanvas.gameObject.SetActive(false);
        tutorialManager = FindObjectOfType<TutorialManager>();

        // Check welke scene actief is
        isInLevel = SceneManager.GetActiveScene().name == "Level0";
        Debug.Log($"PauseManager: isInLevel = {isInLevel}");

        // Null checks voor buttons
        if (continueButton != null)
            continueButton.onClick.AddListener(Resume);
        else
            Debug.LogWarning("PauseManager: continueButton not assigned!");

        if (restartButton != null)
            restartButton.onClick.AddListener(Restart);
        else
            Debug.LogWarning("PauseManager: restartButton not assigned!");

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(ReturnToMenu);
        else
            Debug.LogWarning("PauseManager: mainMenuButton not assigned!");

        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
        else
            Debug.LogWarning("PauseManager: quitButton not assigned!");
    }

    private void Update()
    {
        // Only allow pause in Level0
        if (!isInLevel) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Pause()
    {
        if (!isInLevel) return;

        isPaused = true;
        Time.timeScale = 0f;
        pauseCanvas.gameObject.SetActive(true);

        // Hide tutorial wanneer pause menu open gaat
        if (tutorialManager != null && tutorialManager.IsTutorialVisible)
        {
            tutorialManager.ToggleTutorial();
        }

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        Debug.Log("Game Paused");
    }

    public void Resume()
    {
        if (!isInLevel) return;

        isPaused = false;
        Time.timeScale = 1f;
        pauseCanvas.gameObject.SetActive(false);

        // Show tutorial terug wanneer resume
        if (tutorialManager != null && !tutorialManager.IsTutorialVisible)
        {
            tutorialManager.ToggleTutorial();
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Debug.Log("Game Resumed");
    }

    public void Restart()
    {
        Time.timeScale = 1f;

        // Unload Level0 en reload
        StartCoroutine(RestartLevel());
    }

    private System.Collections.IEnumerator RestartLevel()
    {
        AsyncOperation unload = SceneManager.UnloadSceneAsync("Level0");
        yield return unload;

        SceneManager.LoadScene("Level0", LoadSceneMode.Additive);
        Debug.Log("Level Restarted");
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1f;

        // Unload Level0
        StartCoroutine(LoadMenu());
    }

    private System.Collections.IEnumerator LoadMenu()
    {
        AsyncOperation unload = SceneManager.UnloadSceneAsync("Level0");
        yield return unload;

        SceneManager.LoadScene("MainMenu", LoadSceneMode.Additive);
        Debug.Log("Returning to Main Menu");
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}