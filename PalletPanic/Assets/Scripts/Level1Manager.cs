using UnityEngine;

public class Level1Manager : MonoBehaviour
{
    public static Level1Manager Instance;
    
    [Header("Pallet Spawning")]
    [SerializeField] private GameObject palletPrefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private int palletsToSpawn = 4;
    
    [Header("Level Settings")]
    public int pointsPerPallet = 100;
    private int palletsDelivered = 0;
    
    [Header("UI")]
    [SerializeField] private GameObject winScreen;
    
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
        // Zorg dat win screen uit staat bij start
        if (winScreen != null)
        {
            winScreen.SetActive(false);
        }
        
        SpawnPallets();
    }
    
    void SpawnPallets()
    {
        if (palletPrefab == null)
        {
            Debug.LogError("Pallet Prefab niet ingesteld!");
            return;
        }
        
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("Geen spawn points ingesteld!");
            return;
        }
        
        int spawnCount = Mathf.Min(palletsToSpawn, spawnPoints.Length);
        
        for (int i = 0; i < spawnCount; i++)
        {
            if (spawnPoints[i] != null)
            {
                Instantiate(palletPrefab, spawnPoints[i].position, Quaternion.identity);
                Debug.Log($"Pallet {i+1} gespawned op {spawnPoints[i].name}");
            }
        }
    }
    
    public void OnPalletDelivered()
    {
        palletsDelivered++;
        ScoreManager.Instance.AddScore(pointsPerPallet);
        
        Debug.Log($"Pallet geleverd! ({palletsDelivered}/{palletsToSpawn})");
        
        if (palletsDelivered >= palletsToSpawn)
        {
            OnLevelComplete();
        }
    }
    
    void OnLevelComplete()
    {
        Debug.Log("LEVEL VOLTOOID!");
        
        if (LevelTimer.Instance != null)
        {
            LevelTimer.Instance.StopTimer();
            
            if (winScreen != null)
            {
                // Update tijd op win screen
                Transform timeTextTransform = winScreen.transform.Find("TimeText");
                if (timeTextTransform != null)
                {
                    TMPro.TextMeshProUGUI timeText = timeTextTransform.GetComponent<TMPro.TextMeshProUGUI>();
                    if (timeText != null)
                    {
                        timeText.text = "Tijd: " + LevelTimer.Instance.GetFormattedTime();
                    }
                }
                
                // Toon win screen
                winScreen.SetActive(true);
            }
        }
    }
}
