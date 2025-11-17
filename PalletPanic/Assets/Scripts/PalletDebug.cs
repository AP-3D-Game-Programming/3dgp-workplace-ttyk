using UnityEngine;

public class PalletDebug : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("TRIGGER WERKT! Geraakt door: " + other.gameObject.name);
    }
}
