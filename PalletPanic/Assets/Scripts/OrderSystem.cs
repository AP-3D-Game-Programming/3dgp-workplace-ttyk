using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PalletStackOrder
{
    public List<PalletColor> colors; // Van onder naar boven
    public bool isDelivered = false;

    public PalletStackOrder(List<PalletColor> stackColors)
    {
        colors = new List<PalletColor>(stackColors);
    }

    public int GetHeight()
    {
        return colors.Count;
    }

    public string GetDescription()
    {
        string desc = $"Stapel van {colors.Count} paletten:\n";
        for (int i = colors.Count - 1; i >= 0; i--) // Van boven naar onder tonen
        {
            desc += $"  • {colors[i]}\n";
        }
        return desc;
    }
}

[System.Serializable]
public class Order
{
    public List<PalletStackOrder> stacks;

    public Order()
    {
        stacks = new List<PalletStackOrder>();
    }

    public bool IsCompleted()
    {
        foreach (var stack in stacks)
        {
            if (!stack.isDelivered)
                return false;
        }
        return true;
    }

    public string GetFullDescription()
    {
        string desc = "Opdracht - Lever de volgende stapels:\n\n";
        for (int i = 0; i < stacks.Count; i++)
        {
            desc += $"Stapel {i + 1}:\n{stacks[i].GetDescription()}\n";
        }
        return desc;
    }
}

public class OrderSystem : MonoBehaviour
{
    public static OrderSystem Instance { get; private set; }

    // Difficulty settings
    private int minPalletsPerStack = 1;
    private int maxPalletsPerStack = 2;
    private int numberOfStacks = 1;

    public Order currentOrder;

    public System.Action<Order> OnOrderGenerated;
    public System.Action OnOrderCompleted;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        GenerateNewOrder();
    }

    public void GenerateNewOrder()
    {
        currentOrder = new Order();

        for (int i = 0; i < numberOfStacks; i++)
        {
            int stackHeight = Random.Range(minPalletsPerStack, maxPalletsPerStack + 1);
            List<PalletColor> stackColors = new List<PalletColor>();

            // Genereer random kleuren voor deze stapel
            for (int j = 0; j < stackHeight; j++)
            {
                PalletColor randomColor = (PalletColor)Random.Range(0, System.Enum.GetValues(typeof(PalletColor)).Length);
                stackColors.Add(randomColor);
            }

            currentOrder.stacks.Add(new PalletStackOrder(stackColors));
        }

        Debug.Log("Nieuwe opdracht gegenereerd:");
        Debug.Log(currentOrder.GetFullDescription());

        OnOrderGenerated?.Invoke(currentOrder);
    }
}
