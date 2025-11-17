using UnityEngine;
using TMPro;

public class LevelTimer : MonoBehaviour
{
    public static LevelTimer Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private GameObject timerPanel;

    [Header("Settings")]
    [SerializeField] private bool startOnMovement = true;

    private float elapsedTime = 0f;
    private bool isRunning = false;
    private bool hasStarted = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (timerPanel != null)
        {
            timerPanel.SetActive(true);
        }

        UpdateTimerDisplay();

        // Als we niet wachten op beweging, start dan meteen
        if (!startOnMovement)
        {
            StartTimer();
        }
    }

    private void Update()
    {
        // Check voor eerste beweging van de speler
        if (startOnMovement && !hasStarted)
        {
            CheckForMovement();
        }

        // Update de timer als deze loopt
        if (isRunning)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimerDisplay();
        }
    }

    private void CheckForMovement()
    {
        // Check of WASD of pijltjestoetsen worden ingedrukt
        bool moving = Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0;

        if (moving)
        {
            StartTimer();
        }
    }

    public void StartTimer()
    {
        if (!hasStarted)
        {
            isRunning = true;
            hasStarted = true;
            Debug.Log("Timer gestart!");
        }
    }

    public void StopTimer()
    {
        isRunning = false;
        Debug.Log($"Timer gestopt bij: {GetFormattedTime()}");
    }

    public void PauseTimer()
    {
        isRunning = false;
    }

    public void ResumeTimer()
    {
        if (hasStarted)
        {
            isRunning = true;
        }
    }

    public void ResetTimer()
    {
        elapsedTime = 0f;
        isRunning = false;
        hasStarted = false;
        UpdateTimerDisplay();
    }

    private void UpdateTimerDisplay()
    {
        if (timerText != null)
        {
            timerText.text = GetFormattedTime();
        }
    }

    public string GetFormattedTime()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);
        int milliseconds = Mathf.FloorToInt((elapsedTime * 100f) % 100f);

        return string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
    }

    public float GetElapsedTime()
    {
        return elapsedTime;
    }

    public bool IsRunning()
    {
        return isRunning;
    }
}