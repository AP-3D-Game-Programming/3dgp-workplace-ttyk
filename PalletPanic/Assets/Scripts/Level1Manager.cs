using UnityEngine;
using TMPro; // Voor UI

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
    
    [Header("Order System")]
    [SerializeField] private TextMeshProUGUI orderText; // UI tekst
    private PalletColor[] orderSequence; // Volgorde van kleuren
    private int currentOrderIndex = 0;
    private GameObject[] spawnedPallets; // Track alle pallets
    
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
        if (winScreen != null)
        {
            winScreen.SetActive(false);
        }
        
        // TEST: Forceer tekst update
        if (orderText != null)
        {
            orderText.text = "TEST - IK WERK!";
            orderText.color = Color.white;
            Debug.Log("OrderText updated!");
        }
        else
        {
            Debug.LogError("OrderText is NULL! Niet gekoppeld!");
        }

        // Definieer volgorde
        orderSequence = new PalletColor[] { 
            PalletColor.Red, 
            PalletColor.Blue, 
            PalletColor.Green, 
            PalletColor.Yellow 
        };
        
        SpawnPallets();
        UpdateOrderUI();
        HighlightCurrentPallet();
    }
    
    void SpawnPallets()
    {
        if (palletPrefab == null || spawnPoints == null) return;
        
        spawnedPallets = new GameObject[palletsToSpawn];
        
        for (int i = 0; i < palletsToSpawn; i++)
        {
            if (spawnPoints[i] != null)
            {
                GameObject pallet = Instantiate(palletPrefab, spawnPoints[i].position, Quaternion.identity);
                
                Pallet palletScript = pallet.GetComponent<Pallet>();
                if (palletScript != null)
                {
                    palletScript.SetColor(orderSequence[i]);
                }
                
                spawnedPallets[i] = pallet;
                Debug.Log($"Pallet {i+1}: {orderSequence[i]}");
            }
        }
    }
    
    void UpdateOrderUI()
    {
        if (orderText != null && currentOrderIndex < orderSequence.Length)
        {
            PalletColor nextColor = orderSequence[currentOrderIndex];
            orderText.text = $"Haal: {GetColorName(nextColor)} Pallet";
            
            // Verander UI kleur naar pallet kleur
            orderText.color = GetColorForPallet(nextColor);
        }
    }
    
    void HighlightCurrentPallet()
    {
        // Zet alle highlights uit
        foreach (GameObject palletObj in spawnedPallets)
        {
            if (palletObj != null)
            {
                Pallet pallet = palletObj.GetComponent<Pallet>();
                if (pallet != null)
                {
                    pallet.SetHighlight(false);
                }
            }
        }
        
        // Highlight alleen de pallet die we nu nodig hebben
        if (currentOrderIndex < spawnedPallets.Length && spawnedPallets[currentOrderIndex] != null)
        {
            Pallet targetPallet = spawnedPallets[currentOrderIndex].GetComponent<Pallet>();
            if (targetPallet != null)
            {
                targetPallet.SetHighlight(true);
                Debug.Log($"Highlighting {orderSequence[currentOrderIndex]} pallet");
            }
        }
    }
    
    public bool IsPalletCorrect(Pallet pallet)
    {
        if (currentOrderIndex >= orderSequence.Length)
            return false;
        
        PalletColor expectedColor = orderSequence[currentOrderIndex];
        return pallet.palletColor == expectedColor;
    }
    
    public void OnPalletDelivered(Pallet pallet)
    {
        // Check of dit de juiste pallet is
        if (!IsPalletCorrect(pallet))
        {
            // Vermijd out-of-range toegang wanneer currentOrderIndex aan/over het einde is
            string expected = (orderSequence != null && currentOrderIndex >= 0 && currentOrderIndex < orderSequence.Length)
                ? orderSequence[currentOrderIndex].ToString()
                : "(geen verwachting)";

            Debug.Log($"Verkeerde pallet! Verwacht: {expected}, Gekregen: {pallet.palletColor}");
            return; // Telt niet!
        }
        
        // Correcte pallet!
        palletsDelivered++;
        currentOrderIndex++;
        
        ScoreManager.Instance.AddScore(pointsPerPallet);
        Debug.Log($"Correcte pallet geleverd! ({palletsDelivered}/{palletsToSpawn})");
        
        // Update UI en highlight voor volgende pallet
        if (currentOrderIndex < orderSequence.Length)
        {
            UpdateOrderUI();
            HighlightCurrentPallet();
        }
        
        // Check of level klaar is
        if (palletsDelivered >= palletsToSpawn)
        {
            OnLevelComplete();
        }
    }
    
    string GetColorName(PalletColor color)
    {
        switch (color)
        {
            case PalletColor.Red: return "Rode";
            case PalletColor.Blue: return "Blauwe";
            case PalletColor.Green: return "Groene";
            case PalletColor.Yellow: return "Gele";
            default: return "???";
        }
    }
    
    Color GetColorForPallet(PalletColor palletColor)
    {
        switch (palletColor)
        {
            case PalletColor.Red: return Color.red;
            case PalletColor.Blue: return Color.blue;
            case PalletColor.Green: return Color.green;
            case PalletColor.Yellow: return Color.yellow;
            default: return Color.white;
        }
    }
    
    void OnLevelComplete()
    {
        Debug.Log("LEVEL VOLTOOID!");
        
        if (orderText != null)
        {
            orderText.text = "LEVEL VOLTOOID!";
            orderText.color = Color.green;
        }
        
        if (LevelTimer.Instance != null)
        {
            LevelTimer.Instance.StopTimer();
            
            if (winScreen != null)
            {
                Transform timeTextTransform = winScreen.transform.Find("TimeText");
                if (timeTextTransform != null)
                {
                    TextMeshProUGUI timeText = timeTextTransform.GetComponent<TextMeshProUGUI>();
                    if (timeText != null)
                    {
                        timeText.text = "Tijd: " + LevelTimer.Instance.GetFormattedTime();
                    }
                }
                
                winScreen.SetActive(true);
            }
        }
    }
}
