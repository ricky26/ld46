using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Project/Item Type", order = 1)]
public class ItemType: ScriptableObject
{
    public string id;
    public Vector2Int size;
    public int maxStack;
    public bool fungible;
    public string displayName;
    public string description;
    public int value;
    public Sprite sprite;

    public InventoryStack CreateStack(int quantity = 1)
    {
        return new InventoryStack(this, quantity);
    }
}
