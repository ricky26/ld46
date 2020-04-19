using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryStackUI: MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IAcceptInventory
{
    public Image background;
    public Image image;
    public Text quantityText;
    public Sprite defaultSprite;

    public Func<InventoryStack, bool> canAccept;
    public AcceptStack accept;
    public Action<InventoryStack?> setStack;

    public delegate bool AcceptStack(ref InventoryStack stack);

    private InventoryStack? stack;
    private IAcceptInventory lastDragOver;
    private InventoryDragPreview dragPreview;

    public InventoryStack? Stack
    {
        get
        {
            return stack;
        }

        set
        {
            stack = value;
            Sync();
        }
    }

    public bool CanAccept(Vector2 screenPos, InventoryStack stack, Vector2Int? draggedFrom)
    {
        return (canAccept != null) && canAccept(stack);
    }

    public bool Accept(Vector2 screenPos, ref InventoryStack stack)
    {
        return (accept != null) && accept(ref stack);
    }
    
    public void ShowDragPreview(Vector2 screenPos, InventoryStack stack, Vector2Int? draggedFrom)
    {
        background.color = CanAccept(screenPos, stack, draggedFrom) ? Color.green : Color.red;
    }

    public void ClearDragPreview()
    {
        background.color = Color.white;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!stack.HasValue)
        {
            return;
        }

        lastDragOver = null;
        background.raycastTarget = false;
        dragPreview = FindObjectOfType<InventoryDragPreview>(true);
        dragPreview.Configure(stack.Value, defaultSprite);
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!stack.HasValue)
        {
            return;
        }

        dragPreview.SetPosition(eventData.position);

        var parentInventory = GetComponentInParent<InventoryUI>();
        var newDragOver = eventData.pointerCurrentRaycast.gameObject?.GetComponentInParent<IAcceptInventory>();

        if ((lastDragOver != null) && (newDragOver != lastDragOver))
        {
            lastDragOver.ClearDragPreview();
        }
        lastDragOver = newDragOver;

        if (newDragOver != null)
        {
            //var cellPos = lastDragOver.WorldToCell(dragPreview.TopLeftFromMid(eventData.position));
            //var draggedFrom = (parentInventory && ReferenceEquals(lastDragOver, parentInventory)) ? stack.Value.position : (Vector2Int?)null;
            lastDragOver.ShowDragPreview(eventData.position, stack.Value, null);// draggedFrom);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!stack.HasValue)
        {
            return;
        }

        background.raycastTarget = true;
        dragPreview.Disable();

        if (lastDragOver != null)
        {
            lastDragOver.ClearDragPreview();

            var stack = this.stack.Value;
            var parentInventory = GetComponentInParent<InventoryUI>();
            //var oldPos = stack.position;
            //var cellPos = dragOverInventory.WorldToCell(dragPreview.TopLeftFromMid(eventData.position));
            //var draggedFrom = (parentInventory && ReferenceEquals(lastDragOver, parentInventory)) ? stack.position : (Vector2Int?)null;
            if (lastDragOver.CanAccept(eventData.position, stack, null)) //draggedFrom))
            {
                /*if (parentInventory)
                {
                    parentInventory.Inventory.RemoveAt(stack.position);
                }*/

                if (lastDragOver.Accept(eventData.position, ref stack))
                {
                    setStack(null);
                }
                else
                {
                    setStack(stack);
                }

                //stack.position = cellPos;
                /*if (!dragOverInventory.Inventory.Insert(ref stack))
                {
                    stack.position = oldPos;
                    parentInventory.Inventory.Insert(ref stack);
                }*/
            }
        }
    }

    private void Sync()
    {
        image.gameObject.SetActive(stack.HasValue);
        if (!stack.HasValue)
        {
            return;
        }

        image.sprite = stack.Value.itemType.sprite ? stack.Value.itemType.sprite : defaultSprite;
        quantityText.text = stack.Value.quantity.ToString();
    }
}
