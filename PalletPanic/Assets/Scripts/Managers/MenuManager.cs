using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.UnloadSceneAsync("MainMenu");
        SceneManager.LoadScene("Level0");
    }

    public void ReturnToMenu()
    {
        SceneManager.UnloadSceneAsync("Level0");
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Additive);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}