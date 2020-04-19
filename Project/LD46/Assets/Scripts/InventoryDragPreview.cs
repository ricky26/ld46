using UnityEngine;
using UnityEngine.UI;

public class InventoryDragPreview: MonoBehaviour
{
    public Image sprite;
    public Text quantityText;
    public new RectTransform transform;
    public GameObject cellPrefab;

    public void Configure(InventoryStack stack, Sprite defaultSprite)
    {
        var cellTransform = cellPrefab.GetComponent<RectTransform>();
        var cellRect = cellTransform.rect;
        var size = stack.itemType?.size ?? new Vector2Int(1, 1);

        gameObject.SetActive(true);
        sprite.sprite = stack.itemType?.sprite ? stack.itemType.sprite : defaultSprite;
        quantityText.text = stack.quantity.ToString();
        transform.sizeDelta = new Vector2(size.x * cellRect.width, size.y * cellRect.height);
    }

    public void SetPosition(Vector2 position)
    {
        transform.position = position;
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }

    public Vector2 TopLeftFromMid(Vector2 mid)
    {
        var rect = transform.rect;
        var offset = new Vector2(rect.width * 0.5f, -rect.height * 0.5f);
        return mid + offset;
    }
}
