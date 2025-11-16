using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Additive);
    }
    // public enum GameState { MainMenu, Intro, Level1 }
    // public GameState CurrentState { get; private set; }

    // [SerializeField] private float minimumLoadTime = 2f;

    // private void Awake()
    // {
    //     if (Instance == null)
    //     {
    //         Instance = this;
    //         DontDestroyOnLoad(gameObject);
    //     }
    //     else
    //     {
    //         Destroy(gameObject);
    //         return;
    //     }

    //     CurrentState = GameState.MainMenu;
    // }

    // private void Start()
    // {
    //     string currentScene = SceneManager.GetActiveScene().name;

    //     if (currentScene != "MainMenu")
    //     {
    //         Debug.Log($"Starting from {currentScene}, loading MainMenu...");
    //         LoadMainMenu();
    //     }
    //     else
    //     {
    //         Debug.Log("Already in MainMenu");
    //     }
    // }

    // public void LoadMainMenu()
    // {
    //     CurrentState = GameState.MainMenu;
    //     SceneManager.LoadScene("MainMenu");
    //     Debug.Log("Main Menu loaded.");
    // }

    // public void LoadIntro()
    // {
    //     CurrentState = GameState.Intro;
    //     StartCoroutine(LoadSceneAsync("IntroScene"));
    // }

    // public void LoadLevel1()
    // {
    //     CurrentState = GameState.Level1;
    //     StartCoroutine(LoadSceneAsync("Level1Scene"));
    // }

    // private IEnumerator LoadSceneAsync(string sceneName)
    // {
    //     AsyncOperation loadingScreenOp = SceneManager.LoadSceneAsync("LoadingScreen");
    //     yield return loadingScreenOp;

    //     yield return null;

    //     float startTime = Time.time;

    //     AsyncOperation sceneOperation = SceneManager.LoadSceneAsync(sceneName);
    //     sceneOperation.allowSceneActivation = false; 

    //     while (!sceneOperation.isDone)
    //     {
    //         float progress = Mathf.Clamp01(sceneOperation.progress / 0.9f);

    //         if (LoadingScreen.Instance != null)
    //         {
    //             LoadingScreen.Instance.UpdateProgress(progress);
    //         }

    //         if (sceneOperation.progress >= 0.9f)
    //         {
    //             float elapsedTime = Time.time - startTime;

    //             if (elapsedTime >= minimumLoadTime)
    //             {
    //                 if (LoadingScreen.Instance != null)
    //                 {
    //                     LoadingScreen.Instance.UpdateProgress(1f);
    //                 }

    //                 yield return new WaitForSeconds(0.3f);
    //                 sceneOperation.allowSceneActivation = true;
    //             }
    //         }

    //         yield return null;
    //     }

    //     Debug.Log($"Scene {sceneName} loaded.");

    //     yield return new WaitForSeconds(0.2f);

    //     // Als we de IntroScene laden, spawn dan de pallet
    //     if (sceneName == "IntroScene")
    //     {
    //         SpawnInitialObjects();
    //     }
    // }

    // private void SpawnInitialObjects()
    // {
    //     // Zoek de PalletSpawner in de geladen scene
    //     if (SpawnManager.Instance != null)
    //     {
    //         SpawnManager.Instance.SpawnPallet();
    //         Debug.Log("Pallet spawn requested in IntroScene");
    //     }
    //     else
    //     {
    //         Debug.LogWarning("PalletSpawner not found! Make sure it exists in the IntroScene.");
    //     }
    // }

    // public void OnStartButtonPressed()
    // {
    //     Debug.Log("Start button pressed!");
    //     LoadIntro();
    // }

    // public void OnIntroComplete()
    // {
    //     LoadLevel1();
    // }

    // private void OnEnable()
    // {
    //     SceneManager.sceneLoaded += OnSceneLoaded;
    // }

    // private void OnDisable()
    // {
    //     SceneManager.sceneLoaded -= OnSceneLoaded;
    // }

    // private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    // {
    //     Debug.Log($"Scene loaded: {scene.name}");
    // }
}