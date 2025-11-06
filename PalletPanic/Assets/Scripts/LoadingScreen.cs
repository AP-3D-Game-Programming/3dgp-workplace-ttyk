using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadingScreen : MonoBehaviour
{
    public static LoadingScreen Instance;

    [Header("UI Elements")]
    [SerializeField] private Slider progressBar;
    [SerializeField] private TextMeshProUGUI loadingText;
    [SerializeField] private TextMeshProUGUI percentageText;

    [Header("Loading Tips (optioneel)")]
    [SerializeField] private TextMeshProUGUI tipsText;
    [SerializeField]
    private string[] loadingTips = new string[]
    {
        "Tip: Druk op Space om te springen",
        "Tip: Verzamel power-ups voor extra punten",
        "Tip: Let op obstakels!",
        "Tip: Probeer alle levels te halen!"
    };

    private void Awake()
    {
        Instance = this;

        if (tipsText != null && loadingTips.Length > 0)
        {
            tipsText.text = loadingTips[Random.Range(0, loadingTips.Length)];
        }
    }

    public void UpdateProgress(float progress)
    {
        if (progressBar != null)
        {
            progressBar.value = progress;
        }

        if (percentageText != null)
        {
            percentageText.text = Mathf.RoundToInt(progress * 100f) + "%";
        }

        if (loadingText != null)
        {
            int dots = Mathf.FloorToInt(Time.time * 2f) % 4;
            loadingText.text = "Loading" + new string('.', dots);
        }
    }
}