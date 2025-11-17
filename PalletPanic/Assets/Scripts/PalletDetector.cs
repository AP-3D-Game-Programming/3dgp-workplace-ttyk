using UnityEngine;

public class PalletPickup : MonoBehaviour
{
    public int pointsPerPallet = 10;
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pallet"))
        {
            Debug.Log("Pallet gepikt! +" + pointsPerPallet + " punten!");
            
            // Maak pallet kind van Lift
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
            }
            
            Transform liftPoint = transform.Find("Lift");
            if (liftPoint != null)
            {
                other.transform.SetParent(liftPoint);
                other.transform.localPosition = Vector3.zero;
                other.transform.localRotation = Quaternion.identity;
            }
        }
    }
}
