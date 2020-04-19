using UnityEngine;

[RequireComponent(typeof(RectTransform))]
[ExecuteInEditMode]
public class DriveOneDimension: MonoBehaviour
{
    public bool driveHorizontal;
    public float ratio = 1.0f;

    private RectTransform rectTransform;

    private void OnValidate()
    {
        Start();
    }

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        var sizeDelta = rectTransform.sizeDelta;
        if (driveHorizontal)
        {
            sizeDelta.x = ratio * sizeDelta.y;
        }
        else
        {
            sizeDelta.y = ratio * sizeDelta.x;
        }
        rectTransform.sizeDelta = sizeDelta;
    }
}
