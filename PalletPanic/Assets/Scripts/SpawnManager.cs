using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; }

    [Header("Prefab")]
    public GameObject palletPrefab;

    [Header("Map References")]
    public Transform mapRoot;

    [Header("Spawn Settings")]
    public float spawnHeight = 0.5f;
    public Vector3 spawnArea = new Vector3(10, 0, 10);
    public int maxAttempts = 50;

    [Header("Layer Masks")]
    public LayerMask floorMask;
    public LayerMask obstacleMask;

    [Header("Detection Settings")]
    public float raycastHeight = 20f;
    public float raycastDistance = 50f;

    public delegate void PalletSpawnedHandler(GameObject pallet);
    public event PalletSpawnedHandler OnPalletSpawned;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SpawnPallet()
    {
        if (palletPrefab == null || mapRoot == null) return;

        for (int i = 0; i < maxAttempts; i++)
        {
            GameObject pallet = TrySpawnAtRandomPosition();
            if (pallet != null)
            {
                OnPalletSpawned?.Invoke(pallet);

                if (TutorialManager.Instance != null)
                    TutorialManager.Instance.pallet = pallet.transform;

                return;
            }
        }

        Debug.LogError("SpawnManager: Could not spawn pallet after max attempts.");
    }

    private GameObject TrySpawnAtRandomPosition()
    {
        Vector3 randomPos = new Vector3(
            mapRoot.position.x + Random.Range(-spawnArea.x / 2f, spawnArea.x / 2f),
            mapRoot.position.y + raycastHeight,
            mapRoot.position.z + Random.Range(-spawnArea.z / 2f, spawnArea.z / 2f)
        );

        if (Physics.Raycast(randomPos, Vector3.down, out RaycastHit hit, raycastDistance, floorMask))
        {
            Vector3 spawnPos = hit.point + Vector3.up * spawnHeight;
            Vector3 checkSize = GetPalletSize();
            Vector3 checkCenter = spawnPos + Vector3.up * (checkSize.y / 2f);

            Collider[] overlaps = Physics.OverlapBox(checkCenter, checkSize / 2f, Quaternion.identity, obstacleMask);
            if (overlaps.Length == 0)
            {
                GameObject spawnedPallet = Instantiate(palletPrefab, spawnPos, Quaternion.identity, mapRoot);
                return spawnedPallet;
            }
        }
        return null;
    }

    private Vector3 GetPalletSize()
    {
        Renderer renderer = palletPrefab.GetComponentInChildren<Renderer>();
        if (renderer != null) return renderer.bounds.size;
        return new Vector3(1f, 1f, 1f);
    }
}
