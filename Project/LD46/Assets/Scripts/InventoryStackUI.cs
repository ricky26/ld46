using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryStackUI: MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IAcceptInventory
{
    public Image background;
    public Image image;
    public Text quantityText;
    public Sprite defaultSprite;

    public Func<InventoryStack, object, bool> canAccept;
    public AcceptStack accept;
    public Action<InventoryStack?> setStack;

    public delegate bool AcceptStack(ref InventoryStack stack, object source);

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

    public bool CanAccept(Vector2 screenPos, InventoryStack stack, object source)
    {
        return (canAccept != null) && canAccept(stack, source);
    }

    public bool Accept(Vector2 screenPos, ref InventoryStack stack, object source)
    {
        return (accept != null) && accept(ref stack, source);
    }
    
    public void ShowDragPreview(Vector2 screenPos, InventoryStack stack, object source)
    {
        background.color = CanAccept(screenPos, stack, source) ? Color.green : Color.red;
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

        var newDragOver = eventData.pointerCurrentRaycast.gameObject?.GetComponentInParent<IAcceptInventory>();
        dragPreview.SetPosition(eventData.position);

        if ((lastDragOver != null) && (newDragOver != lastDragOver))
        {
            lastDragOver.ClearDragPreview();
        }

        lastDragOver = newDragOver;
        lastDragOver?.ShowDragPreview(eventData.position, stack.Value, this);
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
            if (lastDragOver.CanAccept(eventData.position, stack, this))
            {
                setStack(null);
                if (!lastDragOver.Accept(eventData.position, ref stack, this))
                {
                    setStack(stack);
                }
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
