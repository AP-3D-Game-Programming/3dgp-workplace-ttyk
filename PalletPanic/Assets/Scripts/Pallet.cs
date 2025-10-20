using UnityEngine;

public enum PalletColor
{
    Red,
    Blue,
    Green,
    Yellow
}

public class Pallet : MonoBehaviour
{
    public PalletColor palletColor;

    public Material redMaterial;
    public Material blueMaterial;
    public Material greenMaterial;
    public Material yellowMaterial;

    private Renderer palletRenderer;

    private void InitializeRenderer()
    {
        if(palletRenderer == null)
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
    }

    private void OnValidate() //wijzigingen via inspector
    {
        InitializeRenderer();
        ApplyMaterial();
    }

    public void SetColor(PalletColor newColor)
    {
        palletColor = newColor;
        ApplyMaterial();
    }

    private void ApplyMaterial()
    {
        Material materialToApply = GetMaterialForColor(palletColor);

        if (materialToApply != null && palletRenderer != null)
        {
            palletRenderer.material = materialToApply;
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
