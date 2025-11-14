using UnityEngine;

public class CargoBarrelHighlight : MonoBehaviour
{
    [Header("Highlight Settings")]
    [SerializeField] private bool isHighlighted = false;

    [Header("Visual Settings")]
    [SerializeField] private float pulseSpeed = 1.5f;
    [SerializeField] private float glowIntensity = 2.0f;
    [SerializeField] private Color highlightColor = Color.yellow;

    private Renderer[] childRenderers;
    private Material[][] originalMaterials;
    private Material[][] highlightMaterials;
    private float pulseTime = 0f;
    private bool wasHighlighted = false;

    private void Awake()
    {
        childRenderers = GetComponentsInChildren<Renderer>();
        StoreOriginalMaterials();
    }

    private void StoreOriginalMaterials()
    {
        originalMaterials = new Material[childRenderers.Length][];

        for (int i = 0; i < childRenderers.Length; i++)
        {
            if (childRenderers[i] != null)
            {
                originalMaterials[i] = childRenderers[i].sharedMaterials;
            }
        }
    }

    private void Update()
    {
        if (isHighlighted != wasHighlighted)
        {
            if (isHighlighted)
                EnableHighlight();
            else
                DisableHighlight();

            wasHighlighted = isHighlighted;
        }

        if (isHighlighted && highlightMaterials != null)
        {
            UpdateHighlight();
        }
    }

    private void UpdateHighlight()
    {
        pulseTime += Time.deltaTime * pulseSpeed;
        float pulseFactor = Mathf.PingPong(pulseTime, 1f);
        float currentIntensity = glowIntensity * pulseFactor;

        Color emissionColor = highlightColor * currentIntensity;

        foreach (Material[] mats in highlightMaterials)
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

    private void EnableHighlight()
    {
        pulseTime = 0f;

        Shader litShader = Shader.Find("Universal Render Pipeline/Lit");
        if (litShader == null)
        {
            Debug.LogError("URP/Lit shader niet gevonden!");
            return;
        }

        highlightMaterials = new Material[childRenderers.Length][];

        for (int i = 0; i < childRenderers.Length; i++)
        {
            if (childRenderers[i] != null && originalMaterials[i] != null)
            {
                Material[] newMats = new Material[originalMaterials[i].Length];

                for (int j = 0; j < originalMaterials[i].Length; j++)
                {
                    if (originalMaterials[i][j] != null)
                    {
                        // Maak nieuw material met URP/Lit shader
                        Material newMat = new Material(litShader);

                        // Kopieer basis eigenschappen van origineel material
                        if (originalMaterials[i][j].HasProperty("_BaseColor"))
                        {
                            newMat.SetColor("_BaseColor", originalMaterials[i][j].GetColor("_BaseColor"));
                        }
                        else if (originalMaterials[i][j].HasProperty("_Color"))
                        {
                            newMat.SetColor("_BaseColor", originalMaterials[i][j].GetColor("_Color"));
                        }

                        if (originalMaterials[i][j].mainTexture != null)
                        {
                            newMat.mainTexture = originalMaterials[i][j].mainTexture;
                        }

                        // Setup emission
                        newMat.EnableKeyword("_EMISSION");
                        newMat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
                        newMat.SetColor("_EmissionColor", highlightColor * glowIntensity);

                        newMats[j] = newMat;
                    }
                }

                highlightMaterials[i] = newMats;
                childRenderers[i].materials = newMats;
            }
        }
    }

    private void DisableHighlight()
    {
        for (int i = 0; i < childRenderers.Length; i++)
        {
            if (childRenderers[i] != null && originalMaterials[i] != null)
            {
                childRenderers[i].materials = originalMaterials[i];
            }
        }

        // Clean up highlight materials
        if (highlightMaterials != null)
        {
            foreach (Material[] mats in highlightMaterials)
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
            highlightMaterials = null;
        }
    }

    private void OnDestroy()
    {
        DisableHighlight();
    }

    public void SetHighlight(bool highlight)
    {
        isHighlighted = highlight;
    }

    public void ToggleHighlight()
    {
        isHighlighted = !isHighlighted;
    }

    public void SetPulseSpeed(float speed)
    {
        pulseSpeed = Mathf.Max(0f, speed);
    }

    public void SetGlowIntensity(float intensity)
    {
        glowIntensity = Mathf.Max(0f, intensity);
    }

    public void SetHighlightColor(Color color)
    {
        highlightColor = color;
    }
}