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

    private Renderer palletRenderer;
    private GameObject currentCargoInstance;
    private CargoType previousCargoType = (CargoType)(-1); // Initialiseer met ongeldige waarde
    private bool isApplyingCargo = false;

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

        // Zorg ervoor dat we bij start de juiste cargo hebben
        previousCargoType = cargoType;
        ApplyCargo();
    }

    private void OnValidate() // Wijzigingen via inspector
    {
        // Voorkom dat OnValidate meerdere keren parallel cargo toevoegt
        if (isApplyingCargo)
            return;

        InitializeRenderer();
        ApplyMaterial();

        // Check of cargo type is gewijzigd
        if (previousCargoType != cargoType)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                // In Edit Mode: stel de cargo wijziging uit tot na OnValidate
                CargoType targetType = cargoType;
                EditorApplication.delayCall += () =>
                {
                    if (this != null)
                    {
                        cargoType = targetType;
                        ApplyCargo();
                        previousCargoType = cargoType;
                    }
                };
            }
            else
#endif
            {
                // In Play Mode: direct toepassen
                ApplyCargo();
                previousCargoType = cargoType;
            }
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
        isApplyingCargo = true;

        // Verwijder huidige cargo
        RemoveCurrentCargo();

        // Instantieer nieuwe cargo
        InstantiateNewCargo();

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            EditorUtility.SetDirty(gameObject);
        }
#endif

        isApplyingCargo = false;
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
        // Verzamel alle children die verwijderd moeten worden
        List<GameObject> childrenToRemove = new List<GameObject>();

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (child != null)
            {
                childrenToRemove.Add(child.gameObject);
            }
        }

        // Verwijder alle children
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
}