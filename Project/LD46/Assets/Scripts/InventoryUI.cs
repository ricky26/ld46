
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour, IAcceptInventory
{
    public GameObject cellPrefab;
    public GameObject stackPrefab;
    public RectTransform inventoryContainer;

    [SerializeField]
    private Inventory inventory;
    private float cellWidth;
    private float cellHeight;
    private Image[] cells;
    private InventoryStackUI[] inventoryStacks;
    private Vector2Int previewOffset;
    private Vector2Int previewSize;

    public Inventory Inventory => inventory;

    public void SetInventory(Inventory inventory)
    {
        if (this.inventory && (this.inventory != inventory))
        {
            this.inventory.OnStacksChanged.RemoveListener(SyncStacks);
        }
        this.inventory = inventory;

        // First, purge the nonbeleivers.
        var t = inventoryContainer.transform;
        var numChildren = t.childCount;
        for (int i = 0; i < numChildren; ++i)
        {
            Destroy(t.GetChild(i).gameObject);
        }
        inventoryStacks = new InventoryStackUI[inventory.size.x * inventory.size.y];

        if (!inventory)
        {
            return;
        }

        // Create the inventory tiles.
        var cellRectTransform = cellPrefab.GetComponent<RectTransform>();
        var cellRect = cellRectTransform.rect;
        cellWidth = cellRect.width;
        cellHeight = cellRect.height;
        cells = new Image[inventory.size.x * inventory.size.y];

        for (int y = 0; y < inventory.size.y; ++y)
        {
            for (int x = 0; x < inventory.size.x; ++x)
            {
                var go = Instantiate(cellPrefab, inventoryContainer);
                var rectTransform = go.GetComponent<RectTransform>();
                var pos = new Vector2(cellWidth * x, -cellHeight * y);
                rectTransform.localPosition = pos;

                var cellOffset = y * inventory.size.x + x;
                cells[cellOffset] = go.GetComponent<Image>();
            }
        }

        inventoryContainer.sizeDelta = new Vector2(cellWidth * inventory.size.x, cellHeight * inventory.size.y);
        inventory.OnStacksChanged.AddListener(SyncStacks);
        SyncStacks();
    }

    private void Start()
    {
        SetInventory(inventory);
    }

    private void SyncStacks()
    {
        for(int index = 0; index < inventory.NumSlots; ++index)
        {
            var a = inventory.stacks[index];
            var b = inventoryStacks[index];
            var position = inventory.OffsetToPos(index);

            var hasA = a.HasValue;
            var hasB = b != null;

            if (hasA && !hasB)
            {
                // Exists in src but not here.
                var stack = a.Value;
                var go = Instantiate(stackPrefab, inventoryContainer);
                var rectTransform = go.GetComponent<RectTransform>();
                var stackUI = go.GetComponent<InventoryStackUI>();

                rectTransform.sizeDelta = new Vector2(cellWidth * stack.itemType.size.x, cellHeight * stack.itemType.size.y);
                var pos = new Vector2(cellWidth * position.x, -cellHeight * position.y);
                rectTransform.localPosition = pos;

                stackUI.Stack = stack;
                stackUI.canAccept = stack => inventory.CanInsertAt(position, stack, null);
                stackUI.accept = (ref InventoryStack stack) => inventory.Insert(position, ref stack);
                stackUI.setStack = stack =>
                {
                    if (stack.HasValue)
                    {
                        inventory.Replace(position, stack.Value);
                    }
                    else
                    {
                        inventory.RemoveAt(position);
                    }
                };

                inventoryStacks[index] = stackUI;
            }
            else if (hasB && !hasA)
            {
                var stackUI = inventoryStacks[index];
                Destroy(stackUI.gameObject);
                inventoryStacks[index] = null;
            }
            else if (hasA)
            {
                b.Stack = a.Value;
            }
        }
    }

    public Vector2Int WorldToCell(Vector2 worldPos)
    {
        var localPos = inventoryContainer.InverseTransformPoint(worldPos);
        var x = Mathf.RoundToInt((localPos.x / cellWidth) - 1);
        var y = Mathf.RoundToInt((localPos.y / -cellHeight) - 1);
        return new Vector2Int(x, y);
    }

    public void ShowDragPreview(Vector2 screenPos, InventoryStack stack, Vector2Int? draggedFrom)
    {
        var cellPos = WorldToCell(screenPos);
        ShowDragPreview(cellPos, stack, draggedFrom);
    }

    public bool ShowDragPreview(Vector2Int position, InventoryStack stack, Vector2Int? draggedFrom)
    {
        ClearDragPreview();

        var size = stack.itemType.size;
        var isValid = Inventory.ClampRect(inventory.size, ref position, ref size)
            && inventory.CanInsertAt(position, stack, draggedFrom);

        previewOffset = position;
        previewSize = size;

        for (int y = 0; y < size.y; ++y)
        {
            for (int x = 0; x < size.x; ++x)
            {
                var cellOffset = (position.y + y) * inventory.size.x + position.x + x;
                cells[cellOffset].color = isValid ? Color.green : Color.red;
            }
        }

        return true;
    }

    public void ClearDragPreview()
    {
        for (int y = 0; y < previewSize.y; ++y)
        {
            for (int x = 0; x < previewSize.x; ++x)
            {
                var cellOffset = (previewOffset.y + y) * inventory.size.x + previewOffset.x + x;
                cells[cellOffset].color = Color.white;
            }
        }
    }

    public bool CanAccept(Vector2 screenPos, InventoryStack stack, Vector2Int? draggedFrom)
    {
        var cellPos = WorldToCell(screenPos);
        return inventory.CanInsertAt(cellPos, stack, draggedFrom);
    }

    public bool Accept(Vector2 screenPos, ref InventoryStack stack)
    {
        var cellPos = WorldToCell(screenPos);
        return inventory.Insert(cellPos, ref stack);
    }
}
