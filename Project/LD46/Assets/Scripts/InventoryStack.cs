using UnityEngine;

public struct InventoryStack
{
    public ItemType itemType;
    public int quantity;

    public InventoryStack(ItemType itemType, int quantity)
    {
        this.itemType = itemType;
        this.quantity = quantity;
    }

    public bool Intersects(Vector2Int localPos, Vector2Int position)
    {
        return Intersects(localPos, position, new Vector2Int(1, 1));
    }

    public bool Intersects(Vector2Int localPos, Vector2Int position, Vector2Int size)
    {
        var ln = localPos;
        var lx = ln + itemType.size;
        var tn = position;
        var tx = tn + size;

        return (((tn.x >= ln.x) && (tn.x < lx.x))
            || ((tx.x > ln.x) && (tx.x <= lx.x)))
            && (((tn.y >= ln.y) && (tn.y < lx.y))
            || ((tx.y > ln.y) && (tx.y <= lx.y)));
    }

    public bool CanMerge(InventoryStack other)
    {
        if (!itemType.fungible || (itemType != other.itemType))
        {
            return false;
        }

        return quantity < itemType.maxStack;
    }

    public bool Merge(ref InventoryStack other)
    {
        if (!CanMerge(other))
        {
            return false;
        }

        var toMove = Mathf.Min(itemType.maxStack - quantity, other.quantity);
        quantity += toMove;
        other.quantity -= toMove;
        return toMove == other.quantity;
    }
}
