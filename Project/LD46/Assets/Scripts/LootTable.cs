using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Project/Loot Table", order = 1)]
public class LootTable: ScriptableObject
{
    public Entry[] entries;

    [Serializable]
    public struct Entry
    {
        public ItemType itemType;
        public int minQuantity;
        public int maxQuantity;
        public float weight;
    }

    public InventoryStack? Loot()
    {
        var totalWeight = entries.Sum(e => e.weight);
        if (totalWeight == 0)
        {
            return null;
        }

        var v = UnityEngine.Random.value * totalWeight;
        var idx = 0;

        while ((idx < entries.Length - 1) && (entries[idx].weight > v))
        {
            v -= entries[idx].weight;
            idx++;
        }

        var entry = entries[idx];
        var qty = UnityEngine.Random.Range(entry.minQuantity, entry.maxQuantity + 1);
        return new InventoryStack(entry.itemType, qty);
    }
}
