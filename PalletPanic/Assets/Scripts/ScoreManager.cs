using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    
    [SerializeField] private TextMeshProUGUI scoreText;
    private int currentScore = 0;
    
    void Awake()
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
    
    void Start()
    {
        UpdateScoreDisplay();
    }
    
    public void AddScore(int points)
    {
        currentScore += points;
        UpdateScoreDisplay();
        Debug.Log("Score: " + currentScore);
    }
    
    void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + currentScore;
        }
    }
    
    public int GetScore()
    {
        return currentScore;
    }
}
