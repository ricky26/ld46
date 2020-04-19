using UnityEngine;

public interface IAcceptInventory
{
    GameObject gameObject { get; }

    void ShowDragPreview(Vector2 screenPos, InventoryStack stack, object source);

    void ClearDragPreview();

    bool CanAccept(Vector2 screenPos, InventoryStack stack, object source);

    bool Accept(Vector2 screenPos, ref InventoryStack stack, object source);
}