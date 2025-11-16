using UnityEngine;

public class ForkTrigger : MonoBehaviour
{
    private Movement forkliftMovement;
    private Rigidbody currentPallet;

    void Start()
    {
        forkliftMovement = GetComponentInParent<Movement>();

        if (forkliftMovement == null)
        {
            Debug.LogError("Movement script not found on parent!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pallet") && currentPallet == null)
        {
            Rigidbody palletRb = other.GetComponent<Rigidbody>();
            if (palletRb != null)
            {
                currentPallet = palletRb;
                forkliftMovement.AttachPallet(palletRb);
                
                // NIEUW: Roep OnPickedUp aan op Pallet script
                Pallet palletScript = palletRb.GetComponent<Pallet>();
                if (palletScript != null)
                {
                    palletScript.OnPickedUp();
                }
            }
            else
            {
                Debug.LogWarning("Pallet has no Rigidbody component");
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Pallet") && other.GetComponent<Rigidbody>() == currentPallet)
        {
            // NIEUW: Roep OnReleased aan op Pallet script
            Pallet palletScript = other.GetComponent<Pallet>();
            if (palletScript != null)
            {
                palletScript.OnReleased();
            }
            
            forkliftMovement.DetachPallet();
            currentPallet = null;
        }
    }
}
