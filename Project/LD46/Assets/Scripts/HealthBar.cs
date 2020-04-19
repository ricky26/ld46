
using UnityEngine;
using UnityEngine.UI;

public class HealthBar: MonoBehaviour
{
    public Healthy target;
    public Slider hpSlider;

    private Canvas canvas;
    private RectTransform myTransform;
    private RectTransform parentTransform;

    private void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        myTransform = GetComponent<RectTransform>();
        parentTransform = myTransform.parent.GetComponent<RectTransform>();
    }

    private void Update()
    {
        var isVisible = target && (target.HPFraction < 1.0f);
        hpSlider.gameObject.SetActive(isVisible);

        if (!isVisible)
        {
            return;
        }

        var camera = canvas.worldCamera ?? Camera.main;
        var screenPos = camera.WorldToScreenPoint(target.transform.position + Vector3.up * 1.5f);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentTransform, screenPos, null, out var canvasPos);
        myTransform.localPosition = canvasPos;
        hpSlider.value = target.HP / target.maxHp;
    }
}
