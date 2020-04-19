using UnityEngine;

public interface IAcceptInventory
{
    GameObject gameObject { get; }

    void ShowDragPreview(Vector2 screenPos, InventoryStack stack, Vector2Int? draggedFrom);

    void ClearDragPreview();

    bool CanAccept(Vector2 screenPos, InventoryStack stack, Vector2Int? draggedFrom);

    bool Accept(Vector2 screenPos, ref InventoryStack stack);
}