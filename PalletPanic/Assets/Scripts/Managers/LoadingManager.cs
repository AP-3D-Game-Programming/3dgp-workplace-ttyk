using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    [SerializeField] private Image logoImage;
    [SerializeField] private float fadeInDuration = 1f;
    [SerializeField] private float displayDuration = 2f;
    [SerializeField] private float fadeOutDuration = 1f;

    private void Start()
    {
        StartCoroutine(LoadingSequence());
    }

    private IEnumerator LoadingSequence()
    {
        yield return StartCoroutine(FadeImage(0, 1, fadeInDuration));

        yield return new WaitForSeconds(displayDuration);

        yield return StartCoroutine(FadeImage(1, 0, fadeOutDuration));

        SceneManager.UnloadSceneAsync("Loading");
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Additive);
    }

    private IEnumerator FadeImage(float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;
        Color color = logoImage.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            logoImage.color = color;
            yield return null;
        }

        color.a = endAlpha;
        logoImage.color = color;
    }
}
