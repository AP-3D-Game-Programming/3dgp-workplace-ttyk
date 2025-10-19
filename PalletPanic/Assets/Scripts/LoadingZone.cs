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
        // Check of het object een pallet is (of zoek in parent)
        Pallet pallet = other.GetComponent<Pallet>();

        // Als niet gevonden, zoek in parent object
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
        }
    }

    private void OnTriggerExit(Collider other)
    {

        Pallet pallet = other.GetComponent<Pallet>();

        // Als niet gevonden, zoek in parent object
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
