using UnityEngine;

public class SceneTransitionButton : MonoBehaviour
{
    // Deze functie kan je koppelen aan je UI button
    public void StartGame()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStartButtonPressed();
        }
        else
        {
            Debug.LogError("GameManager niet gevonden!");
        }
    }

    public void GoToLevel1()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnIntroComplete();
        }
        else
        {
            Debug.LogError("GameManager niet gevonden!");
        }
    }

    public void GoToMainMenu()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadMainMenu();
        }
        else
        {
            Debug.LogError("GameManager niet gevonden!");
        }
    }
}