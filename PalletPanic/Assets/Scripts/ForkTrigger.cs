using UnityEngine;

public class ForkTrigger : MonoBehaviour
{
    private ForkliftController forkliftMovement;
    private Rigidbody currentPallet;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        forkliftMovement = GetComponentInParent<ForkliftController>();

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
            forkliftMovement.DetachPallet();
            currentPallet = null;
        }
    }
}
