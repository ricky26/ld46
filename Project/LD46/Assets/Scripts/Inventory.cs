
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Item", menuName = "Project/Inventory", order = 1)]
public class Inventory: ScriptableObject
{
    public Vector2Int size;
    public InventoryStack?[] stacks;
    public UnityEvent OnStacksChanged;

    public int NumSlots => size.x * size.y;

    public int FilledSlots => stacks.Sum(stack => stack.HasValue ? stack.Value.itemType.size.x * stack.Value.itemType.size.y : 0);

    public static Inventory Create(Vector2Int size)
    {
        var inventory = CreateInstance<Inventory>();
        inventory.name = "Inventory";
        inventory.size = size;
        inventory.stacks = new InventoryStack?[size.x * size.y];
        inventory.OnStacksChanged = new UnityEvent();
        return inventory;
    }

    public Vector2Int OffsetToPos(int offset)
    {
        var y = offset / size.x;
        var x = offset % size.x;
        return new Vector2Int(x, y);
    }

    public int PosToOffset(Vector2Int pos)
    {
        return pos.x + (pos.y * size.x);
    }

    public bool RemoveAt(Vector2Int position)
    {
        for (var index = 0; index < stacks.Length; ++index)
        {
            var stack = stacks[index];
            var stackPos = OffsetToPos(index);
            if (!stack.HasValue || !stack.Value.Intersects(stackPos, position))
            {
                continue;
            }

            stacks[index] = null;
            OnStacksChanged.Invoke();
            return true;
        }

        return false;
    }

    public bool CanInsertAt(Vector2Int position, InventoryStack stack, Vector2Int? draggedFrom)
    {
        if (stack.quantity <= 0)
        {
            return false;
        }

        var size = stack.itemType.size;
        if (!ClampRect(this.size, ref position, ref size))
        {
            return false;
        }

        for (var index = 0; index < stacks.Length; ++index)
        {
            var existingStack = stacks[index];
            var existingPos = OffsetToPos(index);
            if (existingPos == draggedFrom)
            {
                continue;
            }

            if (existingStack.HasValue && existingStack.Value.Intersects(existingPos, position, size) && !existingStack.Value.CanMerge(stack))
            {
                return false;
            }
        }

        return true;
    }

    public bool Insert(Vector2Int insertPos, ref InventoryStack stack, Vector2Int? draggedFrom)
    {
        if (stack.quantity <= 0)
        {
            return false;
        }

        for (var index = 0; index < stacks.Length; ++index)
        {
            var maybeStack = stacks[index];
            var existingPos = OffsetToPos(index);
            if (existingPos == draggedFrom)
            {
                continue;
            }

            if (maybeStack.HasValue && maybeStack.Value.Intersects(existingPos, insertPos, stack.itemType.size))
            {
                var existingStack = maybeStack.Value;
                var ret = existingStack.Merge(ref stack);
                stacks[index] = existingStack;
                OnStacksChanged?.Invoke();
                return ret;
            }
        }

        if (draggedFrom.HasValue)
        {
            var offset = PosToOffset(draggedFrom.Value);
            stacks[offset] = null;
        }

        stacks[PosToOffset(insertPos)] = stack;
        OnStacksChanged?.Invoke();
        return true;
    }

    public void Replace(Vector2Int position, InventoryStack stack)
    {
        var index = PosToOffset(position);
        stacks[index] = stack;
        OnStacksChanged?.Invoke();
    }

    public bool InsertAnywhere(ref InventoryStack stack)
    {
        if (stack.quantity <= 0)
        {
            return false;
        }

        for (var index = 0; index < stacks.Length; ++index)
        {
            var maybeStack = stacks[index];
            var existingPos = OffsetToPos(index);

            if (maybeStack.HasValue && maybeStack.Value.CanMerge(stack))
            {
                var existingStack = maybeStack.Value;
                var ret = existingStack.Merge(ref stack);
                stacks[index] = existingStack;
                OnStacksChanged?.Invoke();
                
                if (ret)
                {
                    return ret;
                }
            }

            if (CanInsertAt(existingPos, stack, null))
            {
                stacks[index] = stack;
                OnStacksChanged?.Invoke();
                return true;
            }
        }

        return false;
    }

    public static bool ClampRect(Vector2Int srcSize, ref Vector2Int targetPos, ref Vector2Int targetSize)
    {
        var isValid = true;

        if (targetPos.x < 0)
        {
            targetPos.x = 0;
            isValid = false;
        }

        if (targetPos.x > srcSize.x)
        {
            targetPos.x = srcSize.x;
            isValid = false;
        }

        if (targetPos.x + targetSize.x > srcSize.x)
        {
            targetSize.x = srcSize.x - targetPos.x;
            isValid = false;
        }

        if (targetPos.y < 0)
        {
            targetPos.y = 0;
            isValid = false;
        }

        if (targetPos.y > srcSize.y)
        {
            targetPos.y = srcSize.y;
            isValid = false;
        }

        if (targetPos.y + targetSize.y > srcSize.y)
        {
            targetSize.y = srcSize.y - targetPos.y;
            isValid = false;
        }

        return isValid;
    }
}
