
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour, IAcceptInventory
{
    public GameObject cellPrefab;
    public GameObject stackPrefab;
    public RectTransform inventoryContainer;
    public InventoryUI pairedInventoryUI;
    public Inventory pairedInventory;

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
        inventoryStacks = new InventoryStackUI[inventory.size.x * inventory.size.y];

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
                stackUI.canAccept = (newStack, source) => inventory.CanInsertAt(position, newStack, PosFromSource(source));
                stackUI.accept = (ref InventoryStack newStack, object source) => inventory.Insert(position, ref newStack, PosFromSource(source));
                stackUI.setStack = newStack =>
                {
                    if (newStack.HasValue)
                    {
                        inventory.Replace(position, newStack.Value);
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
        var x = Mathf.FloorToInt(localPos.x / cellWidth);
        var y = Mathf.FloorToInt(localPos.y / -cellHeight);
        return new Vector2Int(x, y);
    }

    private Vector2Int? PosFromSource(object source)
    {
        if (source == null)
        {
            return null;
        }

        for (int index = 0; index < inventoryStacks.Length; ++index)
        {
            var stack = inventoryStacks[index];
            if (ReferenceEquals(stack, source))
            {
                return inventory.OffsetToPos(index);
            }
        }

        return null;
    }

    public void ShowDragPreview(Vector2 screenPos, InventoryStack stack, object source)
    {
        ClearDragPreview();

        var position = WorldToCell(screenPos);
        var draggedFrom = PosFromSource(source);
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

    public bool CanAccept(Vector2 screenPos, InventoryStack stack, object source)
    {
        var cellPos = WorldToCell(screenPos);
        var draggedFrom = PosFromSource(source);
        return inventory.CanInsertAt(cellPos, stack, draggedFrom);
    }

    public bool Accept(Vector2 screenPos, ref InventoryStack stack, object source)
    {
        var cellPos = WorldToCell(screenPos);
        var draggedFrom = PosFromSource(source);
        return inventory.Insert(cellPos, ref stack, draggedFrom);
    }

    public void TakeAll()
    {
        var pairedInventory = pairedInventoryUI ? pairedInventoryUI.inventory : this.pairedInventory;
        if (!pairedInventory)
        {
            return;
        }

        for (int index = 0; index < inventory.stacks.Length; ++index)
        {
            var maybeStack = inventory.stacks[index];
            if (!maybeStack.HasValue)
            {
                continue;
            }

            var stack = maybeStack.Value;
            if (!pairedInventory.InsertAnywhere(ref stack))
            {
                inventory.stacks[index] = stack;
                return;
            }

            inventory.stacks[index] = null;
        }

        inventory.OnStacksChanged?.Invoke();
    }
}
