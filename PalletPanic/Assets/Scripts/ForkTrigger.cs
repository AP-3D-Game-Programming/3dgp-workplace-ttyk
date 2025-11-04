using UnityEngine;

public class ForkTrigger : MonoBehaviour
{
    private Movement forkliftMovement;
    private Rigidbody currentPallet;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        forkliftMovement = GetComponentInParent<Movement>();
        // Debug checks
        if (forkliftMovement == null)
        {
            Debug.LogError("Movement script niet gevonden op parent!");
        }
        Debug.Log("ForkTrigger Start() called - script is active");
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Trigger Enter detected with: {other.gameObject.name}, Tag: {other.tag}");

        if (other.CompareTag("Pallet") && currentPallet == null)
        {
            Rigidbody palletRb = other.GetComponent<Rigidbody>();
            if (palletRb != null)
            {
                currentPallet = palletRb;
                forkliftMovement.AttachPallet(palletRb);
                Debug.Log("Pallet opgepakt");
            }
            else
            {
                Debug.LogWarning("Pallet heeft geen Rigidbody!");
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        Debug.Log($"Trigger Exit detected with: {other.gameObject.name}");

        if (other.CompareTag("Pallet") && other.GetComponent<Rigidbody>() == currentPallet)
        {
            forkliftMovement.DetachPallet();
            currentPallet = null;
            Debug.Log("Pallet losgelaten");
        }
    }
}
