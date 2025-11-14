/* 
 * // Usage
 * Pallet pallet = somePallet.GetComponent<Pallet>();
 * 
 * // CargoType
 * pallet.SetCargoType(CargoType.Barrels);
 * 
 * // PalletColor
 * pallet.SetColor(PalletColor.Red);
 * pallet.SetColor(PalletColor.Blue);
 * 
 * // Highlight
 * pallet.SetHighlight(true/false);     // Enable/Disable
 * pallet.ToggleHighlight();            // Toggle
 * pallet.pulseSpeed = 2.5f;
 * pallet.glowIntensity = 3.0f;
 * pallet.highlightColor = Color.green;
 */

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
    Crates,
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
    public GameObject Barrels;
    public GameObject Boxes;
    public GameObject Crates;
    public GameObject Drums;
    public GameObject AmpRacks;

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

    // Pallet highlight
    private List<Material> palletOriginalMaterials = new List<Material>();
    private List<Material> palletHighlightMaterials = new List<Material>();

    // Cargo highlight
    private Renderer[] cargoRenderers;
    private Material[][] cargoOriginalMaterials;
    private Material[][] cargoHighlightMaterials;

    private bool wasHighlighted = false;
    private float pulseTime = 0f;

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

        if (isHighlighted)
        {
            pulseTime += Time.deltaTime * pulseSpeed;
            float emission = Mathf.PingPong(pulseTime, 1f) * glowIntensity;
            Color emissionColor = highlightColor * emission;

            // Update pallet emission
            foreach (Material mat in palletHighlightMaterials)
            {
                if (mat != null)
                {
                    mat.SetColor("_EmissionColor", emissionColor);
                }
            }

            // Update cargo emission
            if (cargoHighlightMaterials != null)
            {
                foreach (Material[] mats in cargoHighlightMaterials)
                {
                    if (mats != null)
                    {
                        foreach (Material mat in mats)
                        {
                            if (mat != null)
                            {
                                mat.SetColor("_EmissionColor", emissionColor);
                            }
                        }
                    }
                }
            }
        }
    }

    private void OnValidate()
    {
        InitializeRenderer();
        ApplyMaterial();

        if (previousCargoType != cargoType)
        {
#if UNITY_EDITOR
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
        SetupHighlight();

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

            // Verzamel cargo renderers
            cargoRenderers = currentCargoInstance.GetComponentsInChildren<Renderer>();
            StoreCargoOriginalMaterials();
        }
        else
        {
            cargoRenderers = null;
            cargoOriginalMaterials = null;
        }
    }

    private void StoreCargoOriginalMaterials()
    {
        if (cargoRenderers == null || cargoRenderers.Length == 0)
        {
            cargoOriginalMaterials = null;
            return;
        }

        cargoOriginalMaterials = new Material[cargoRenderers.Length][];

        for (int i = 0; i < cargoRenderers.Length; i++)
        {
            if (cargoRenderers[i] != null)
            {
                cargoOriginalMaterials[i] = cargoRenderers[i].sharedMaterials;
            }
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
        cargoRenderers = null;
        cargoOriginalMaterials = null;
    }

    private GameObject GetPrefabForCargoType(CargoType type)
    {
        switch (type)
        {
            case CargoType.None:
                return null;
            case CargoType.Barrels:
                return Barrels;
            case CargoType.Boxes:
                return Boxes;
            case CargoType.Crates:
                return Crates;
            case CargoType.Drums:
                return Drums;
            case CargoType.AmpRacks:
                return AmpRacks;
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
        pulseTime = 0f;

        if (isHighlighted)
        {
            EnablePalletHighlight();
            EnableCargoHighlight();
        }
        else
        {
            DisablePalletHighlight();
            DisableCargoHighlight();
        }

        wasHighlighted = isHighlighted;
    }

    private void EnablePalletHighlight()
    {
        palletOriginalMaterials.Clear();
        palletHighlightMaterials.Clear();

        if (palletRenderer != null && palletRenderer.sharedMaterials != null)
        {
            Material[] newMaterials = new Material[palletRenderer.sharedMaterials.Length];

            for (int i = 0; i < palletRenderer.sharedMaterials.Length; i++)
            {
                if (palletRenderer.sharedMaterials[i] != null)
                {
                    palletOriginalMaterials.Add(palletRenderer.sharedMaterials[i]);

                    newMaterials[i] = new Material(palletRenderer.sharedMaterials[i]);
                    newMaterials[i].EnableKeyword("_EMISSION");
                    newMaterials[i].globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
                    palletHighlightMaterials.Add(newMaterials[i]);
                }
            }

            palletRenderer.materials = newMaterials;
        }
    }

    private void DisablePalletHighlight()
    {
        if (palletRenderer != null && palletOriginalMaterials.Count > 0)
        {
            palletRenderer.materials = palletOriginalMaterials.ToArray();
        }

        palletHighlightMaterials.Clear();
        ApplyMaterial();
    }

    private void EnableCargoHighlight()
    {
        if (cargoRenderers == null || cargoRenderers.Length == 0 || cargoOriginalMaterials == null)
            return;

        Shader litShader = Shader.Find("Universal Render Pipeline/Lit");
        if (litShader == null)
        {
            Debug.LogWarning("URP/Lit shader niet gevonden! Cargo highlight werkt niet.");
            return;
        }

        cargoHighlightMaterials = new Material[cargoRenderers.Length][];

        for (int i = 0; i < cargoRenderers.Length; i++)
        {
            if (cargoRenderers[i] != null && cargoOriginalMaterials[i] != null)
            {
                Material[] newMats = new Material[cargoOriginalMaterials[i].Length];

                for (int j = 0; j < cargoOriginalMaterials[i].Length; j++)
                {
                    if (cargoOriginalMaterials[i][j] != null)
                    {
                        Material newMat = new Material(litShader);

                        // Kopieer basis eigenschappen
                        if (cargoOriginalMaterials[i][j].HasProperty("_BaseColor"))
                        {
                            newMat.SetColor("_BaseColor", cargoOriginalMaterials[i][j].GetColor("_BaseColor"));
                        }
                        else if (cargoOriginalMaterials[i][j].HasProperty("_Color"))
                        {
                            newMat.SetColor("_BaseColor", cargoOriginalMaterials[i][j].GetColor("_Color"));
                        }

                        if (cargoOriginalMaterials[i][j].mainTexture != null)
                        {
                            newMat.mainTexture = cargoOriginalMaterials[i][j].mainTexture;
                        }

                        // Setup emission
                        newMat.EnableKeyword("_EMISSION");
                        newMat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
                        newMat.SetColor("_EmissionColor", highlightColor * glowIntensity);

                        newMats[j] = newMat;
                    }
                }

                cargoHighlightMaterials[i] = newMats;
                cargoRenderers[i].materials = newMats;
            }
        }
    }

    private void DisableCargoHighlight()
    {
        if (cargoRenderers == null || cargoOriginalMaterials == null)
            return;

        for (int i = 0; i < cargoRenderers.Length; i++)
        {
            if (cargoRenderers[i] != null && cargoOriginalMaterials[i] != null)
            {
                cargoRenderers[i].materials = cargoOriginalMaterials[i];
            }
        }

        // Clean up highlight materials
        if (cargoHighlightMaterials != null)
        {
            foreach (Material[] mats in cargoHighlightMaterials)
            {
                if (mats != null)
                {
                    foreach (Material mat in mats)
                    {
                        if (mat != null)
                        {
                            if (Application.isPlaying)
                                Destroy(mat);
                            else
                                DestroyImmediate(mat);
                        }
                    }
                }
            }
            cargoHighlightMaterials = null;
        }
    }

    private void OnDestroy()
    {
        DisableCargoHighlight();
    }

    public void SetHighlight(bool highlight)
    {
        isHighlighted = highlight;
        SetupHighlight();
    }

    public void ToggleHighlight()
    {
        isHighlighted = !isHighlighted;
        SetupHighlight();
    }
}