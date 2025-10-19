using System.Collections.Generic;
using UnityEngine;

public class LoadingZone : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = true;

    // Lijst van paletten die momenteel in de zone zijn
    private List<Pallet> palletsInZone = new List<Pallet>();

    private void OnTriggerEnter(Collider other)
    {
        // Check of het object een pallet is
        Pallet pallet = other.GetComponent<Pallet>();

        // niet gevonden --> zoek in parent
        if (pallet == null)
        {
            pallet = other.GetComponentInParent<Pallet>();
        }

        if (pallet != null && !palletsInZone.Contains(pallet))
        {
            palletsInZone.Add(pallet);

            if (showDebugInfo)
            {
                Debug.Log($"Pallet toegevoegd aan zone: {pallet.palletColor} (Totaal: {palletsInZone.Count})");
            }

            //Opdracht afgerond?
            CheckOrder();
        }
    }

    private void OnTriggerExit(Collider other)
    {

        Pallet pallet = other.GetComponent<Pallet>();

        // niet gevonden --> zoek in parent
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
        if (OrderSystem.Instance == null || OrderSystem.Instance.currentOrder == null)
        {
            Debug.LogWarning("Geen actieve opdracht!");
            return;
        }

        //we hebben maar 1 stapel in de opdracht
        PalletStackOrder requiredStack = OrderSystem.Instance.currentOrder.stacks[0];

        //check aantal paletten
        if (palletsInZone.Count != requiredStack.colors.Count)
        {
            if (showDebugInfo)
            {
                Debug.Log($"Verkeerd aantal paletten. Nodig: {requiredStack.colors.Count}, In zone: {palletsInZone.Count}");
            }
            return;
        }

        // Check kleuren (simpele versie: telt alleen of de juiste kleuren aanwezig zijn)
        List<PalletColor> requiredColors = new List<PalletColor>(requiredStack.colors);
        List<PalletColor> deliveredColors = new List<PalletColor>();

        foreach (Pallet pallet in palletsInZone)
        {
            deliveredColors.Add(pallet.palletColor);
        }

        // Check of alle benodigde kleuren aanwezig zijn
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

        // Alle checks geslaagd
        Debug.Log("Opdracht voltooid!");
        OrderSystem.Instance.OnOrderCompleted?.Invoke();
    }

    // Toon alle paletten die in de zone zijn
    [ContextMenu("Show Pallets In Zone")]
    public void ShowPalletsInZone()
    {
        Debug.Log($"=== Paletten in laadzone: {palletsInZone.Count} ===");
        for (int i = 0; i < palletsInZone.Count; i++)
        {
            Debug.Log($"{i + 1}. {palletsInZone[i].palletColor}");
        }
    }


    // Later te gebruiken voor de check
    public List<Pallet> GetPalletsInZone()
    {
        return new List<Pallet>(palletsInZone);
    }
}
