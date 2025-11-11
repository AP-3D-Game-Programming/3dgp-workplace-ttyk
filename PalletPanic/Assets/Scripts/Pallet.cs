using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public enum PalletColor
{
    Red,
    Blue,
    Green,
    Yellow
}

public enum CargoType
{
    None,
    Barrels,
    Boxes,
    Drums,
    AmpRacks
}

public class Pallet : MonoBehaviour
{
    [Header("Pallet Settings")]
    public PalletColor palletColor;

    [Header("Cargo Settings")]
    public CargoType cargoType = CargoType.None;

    [Header("Materials")]
    public Material redMaterial;
    public Material blueMaterial;
    public Material greenMaterial;
    public Material yellowMaterial;

    [Header("Cargo Prefabs")]
    public GameObject cargoBarrelsPrefab;
    public GameObject cargoBoxesPrefab;
    public GameObject cargoCratesPrefab;
    public GameObject cargoAmpRacksPrefab;

    [Header("Highlight Settings")]
    public bool isHighlighted = false;
    [Range(0.5f, 3f)]
    public float pulseSpeed = 1.5f;
    [Range(0.5f, 5f)]
    public float glowIntensity = 2f;
    public Color highlightColor = Color.yellow;

    private Renderer palletRenderer;
    private GameObject currentCargoInstance;
    private CargoType previousCargoType = (CargoType)(-1);
    private List<Material> originalMaterials = new List<Material>();
    private List<Material> highlightMaterials = new List<Material>();
    private List<Renderer> allRenderers = new List<Renderer>();
    private bool wasHighlighted = false;

    private void InitializeRenderer()
    {
        if (palletRenderer == null)
        {
            palletRenderer = GetComponent<Renderer>();
            if (palletRenderer == null)
            {
                palletRenderer = GetComponentInChildren<Renderer>();
            }
        }
    }

    private void Awake()
    {
        InitializeRenderer();
        ApplyMaterial();
        previousCargoType = cargoType;
        ApplyCargo();
        SetupHighlight();
    }

    private void Update()
    {
        if (isHighlighted != wasHighlighted)
        {
            SetupHighlight();
            wasHighlighted = isHighlighted;
        }

        if (isHighlighted && highlightMaterials.Count > 0)
        {
            float emission = Mathf.PingPong(Time.time * pulseSpeed, 1f) * glowIntensity;
            
            foreach (Material mat in highlightMaterials)
            {
                if (mat != null)
                {
                    mat.SetColor("_EmissionColor", highlightColor * emission);
                }
            }
        }
    }

    private void OnValidate()
    {
        InitializeRenderer();
        ApplyMaterial();

        // Check of cargo type is gewijzigd
        if (previousCargoType != cargoType)
        {
#if UNITY_EDITOR
            // Gebruik altijd EditorApplication.delayCall om Instantiate uit te stellen
            CargoType targetType = cargoType;
            EditorApplication.delayCall += () =>
            {
                if (this != null && gameObject != null)
                {
                    cargoType = targetType;
                    ApplyCargo();
                    previousCargoType = cargoType;
                }
            };
#else
            ApplyCargo();
            previousCargoType = cargoType;
#endif
        }
    }

    public void SetColor(PalletColor newColor)
    {
        palletColor = newColor;
        ApplyMaterial();
    }

    public void SetCargoType(CargoType newCargoType)
    {
        if (cargoType != newCargoType)
        {
            cargoType = newCargoType;
            ApplyCargo();
        }
    }

    private void ApplyMaterial()
    {
        Material materialToApply = GetMaterialForColor(palletColor);

        if (materialToApply != null && palletRenderer != null)
        {
            palletRenderer.material = materialToApply;
        }
    }

    private void ApplyCargo()
    {
        RemoveCurrentCargo();
        InstantiateNewCargo();
        SetupHighlight(); // Update highlight na cargo wijziging

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            EditorUtility.SetDirty(gameObject);
        }
#endif
    }

    private void InstantiateNewCargo()
    {
        GameObject cargoPrefab = GetPrefabForCargoType(cargoType);

        if (cargoPrefab != null)
        {
            currentCargoInstance = Instantiate(cargoPrefab, transform);
            currentCargoInstance.transform.localPosition = Vector3.zero;
            currentCargoInstance.transform.localRotation = Quaternion.identity;
        }
    }

    private void RemoveCurrentCargo()
    {
        List<GameObject> childrenToRemove = new List<GameObject>();

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (child != null)
            {
                childrenToRemove.Add(child.gameObject);
            }
        }

        foreach (GameObject child in childrenToRemove)
        {
            if (Application.isPlaying)
            {
                Destroy(child);
            }
            else
            {
                DestroyImmediate(child);
            }
        }

        currentCargoInstance = null;
    }

    private GameObject GetPrefabForCargoType(CargoType type)
    {
        switch (type)
        {
            case CargoType.None:
                return null;
            case CargoType.Barrels:
                return cargoBarrelsPrefab;
            case CargoType.Boxes:
                return cargoBoxesPrefab;
            case CargoType.Drums:
                return cargoCratesPrefab;
            case CargoType.AmpRacks:
                return cargoAmpRacksPrefab;
            default:
                return null;
        }
    }

    private Material GetMaterialForColor(PalletColor color)
    {
        switch (color)
        {
            case PalletColor.Red:
                return redMaterial;
            case PalletColor.Blue:
                return blueMaterial;
            case PalletColor.Green:
                return greenMaterial;
            case PalletColor.Yellow:
                return yellowMaterial;
            default:
                return null;
        }
    }

    private void SetupHighlight()
    {
        // Verzamel alle renderers (pallet + cargo + alle nested children)
        allRenderers.Clear();
        allRenderers.AddRange(GetComponentsInChildren<Renderer>(true)); // true = include inactive

        // Clear oude materials
        originalMaterials.Clear();
        highlightMaterials.Clear();

        if (isHighlighted)
        {
            // Maak highlight materials voor alle renderers
            foreach (Renderer rend in allRenderers)
            {
                if (rend != null && rend.sharedMaterials != null)
                {
                    // Maak kopieën voor highlight
                    Material[] newMaterials = new Material[rend.sharedMaterials.Length];
                    for (int i = 0; i < rend.sharedMaterials.Length; i++)
                    {
                        if (rend.sharedMaterials[i] != null)
                        {
                            newMaterials[i] = new Material(rend.sharedMaterials[i]);
                            newMaterials[i].EnableKeyword("_EMISSION");
                            newMaterials[i].globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
                            highlightMaterials.Add(newMaterials[i]);
                        }
                    }
                    rend.materials = newMaterials;
                }
            }
        }
        else
        {
            // Reset naar originele state door alle renderers te resetten
            foreach (Renderer rend in allRenderers)
            {
                if (rend != null && rend.sharedMaterials != null)
                {
                    Material[] resetMaterials = new Material[rend.sharedMaterials.Length];
                    for (int i = 0; i < rend.sharedMaterials.Length; i++)
                    {
                        resetMaterials[i] = rend.sharedMaterials[i];
                    }
                    rend.materials = resetMaterials;
                }
            }
            
            // Forceer material refresh van het pallet zelf
            ApplyMaterial();
        }

        wasHighlighted = isHighlighted;
    }

    public void SetHighlight(bool highlight)
    {
        isHighlighted = highlight;
        SetupHighlight();
    }
}