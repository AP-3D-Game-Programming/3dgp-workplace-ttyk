using System.Collections.Generic;
using UnityEngine;

public class LoadingZone : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = true;

    private List<Pallet> palletsInZone = new List<Pallet>();

    private void OnTriggerEnter(Collider other)
{
    Pallet pallet = other.GetComponent<Pallet>();

    if (pallet == null)
    {
        pallet = other.GetComponentInParent<Pallet>();
    }

    if (pallet != null && !palletsInZone.Contains(pallet))
    {
        // Voeg pallet toe aan lijst (ook al is hij nog attached)
        palletsInZone.Add(pallet);

        if (showDebugInfo)
        {
            Debug.Log($"Pallet in zone: {pallet.palletColor} (IsAttached: {pallet.IsAttached})");
        }
        
        // Als pallet al los is, tel direct
        if (!pallet.IsAttached)
        {
            CheckOrder();
        }
        // Anders: wacht tot hij losgelaten wordt (zie Update)
    }
}


    private void OnTriggerExit(Collider other)
    {
        Pallet pallet = other.GetComponent<Pallet>();

        if (pallet == null)
        {
            pallet = other.GetComponentInParent<Pallet>();
        }

        if (pallet != null && palletsInZone.Contains(pallet))
        {
            palletsInZone.Remove(pallet);

            if (showDebugInfo)
            {
                Debug.Log($"Pallet verwijderd uit zone: {pallet.palletColor} (Totaal: {palletsInZone.Count})");
            }
        }
    }

    private void CheckOrder()
    {
        // MODE 1: OrderSystem (Tutorial/Complex levels)
        if (OrderSystem.Instance != null && OrderSystem.Instance.currentOrder != null)
        {
            CheckOrderSystemMode();
            return;
        }
        
        // MODE 2: Level1Manager (Simple levels)
        if (Level1Manager.Instance != null)
        {
            CheckSimpleMode();
            return;
        }
        
        if (showDebugInfo)
        {
            Debug.LogWarning("Geen OrderSystem of Level1Manager gevonden!");
        }
    }

    private void CheckOrderSystemMode()
    {
        PalletStackOrder requiredStack = OrderSystem.Instance.currentOrder.stacks[0];

        if (palletsInZone.Count != requiredStack.colors.Count)
        {
            if (showDebugInfo)
            {
                Debug.Log($"Verkeerd aantal paletten. Nodig: {requiredStack.colors.Count}, In zone: {palletsInZone.Count}");
            }
            return;
        }

        List<PalletColor> requiredColors = new List<PalletColor>(requiredStack.colors);
        List<PalletColor> deliveredColors = new List<PalletColor>();

        foreach (Pallet pallet in palletsInZone)
        {
            deliveredColors.Add(pallet.palletColor);
        }

        foreach (PalletColor color in requiredColors)
        {
            if (deliveredColors.Contains(color))
            {
                deliveredColors.Remove(color);
            }
            else
            {
                if (showDebugInfo)
                {
                    Debug.Log($"Verkeerde kleuren! Ontbreekt: {color}");
                }
                return;
            }
        }

        Debug.Log("Opdracht voltooid!");
        OrderSystem.Instance.OnOrderCompleted?.Invoke();
    }

    private void CheckSimpleMode()
    {
        List<Pallet> detachedPallets = new List<Pallet>();
        
        foreach (Pallet pallet in palletsInZone)
        {
            if (pallet != null && !pallet.IsAttached)
            {
                detachedPallets.Add(pallet);
            }
        }
        
        if (detachedPallets.Count > 0)
        {
            Pallet pallet = detachedPallets[0];
            
            // EXTRA DEBUG
            Debug.Log($"[CheckSimpleMode] Checking pallet: {pallet.palletColor}");
            
            if (Level1Manager.Instance == null)
            {
                Debug.LogError("Level1Manager.Instance is null!");
                return;
            }
            
            // EXTRA DEBUG
            Debug.Log("[CheckSimpleMode] Calling IsPalletCorrect...");
            bool isCorrect = Level1Manager.Instance.IsPalletCorrect(pallet);
            Debug.Log($"[CheckSimpleMode] Result: {isCorrect}");
            
            if (isCorrect)
            {
                palletsInZone.Remove(pallet);
                
                if (showDebugInfo)
                {
                    Debug.Log($"Correcte pallet geleverd! +{Level1Manager.Instance.pointsPerPallet} punten");
                }
                
                Level1Manager.Instance.OnPalletDelivered(pallet);
                Destroy(pallet.gameObject, 0.5f);
            }
            else
            {
                if (showDebugInfo)
                {
                    Debug.Log($"Verkeerde pallet! Deze telt niet.");
                }
            }
        }
        else
        {
            // EXTRA DEBUG
            Debug.Log("[CheckSimpleMode] No detached pallets in zone.");
        }
    }

    [ContextMenu("Show Pallets In Zone")]
    public void ShowPalletsInZone()
    {
        Debug.Log($"=== Paletten in laadzone: {palletsInZone.Count} ===");
        for (int i = 0; i < palletsInZone.Count; i++)
        {
            Debug.Log($"{i + 1}. {palletsInZone[i].palletColor}");
        }
    }

    public List<Pallet> GetPalletsInZone()
    {
        return new List<Pallet>(palletsInZone);
    }

    private void Update()
{
    // Check of pallets in zone losgelaten zijn
    for (int i = palletsInZone.Count - 1; i >= 0; i--)
    {
        Pallet pallet = palletsInZone[i];
        
        if (pallet != null && !pallet.IsAttached)
        {
            // Pallet is losgelaten! Check order
            CheckOrder();
            break; // Check alleen 1 pallet per frame
        }
    }
}

}
