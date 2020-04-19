
using UnityEngine;
using UnityEngine.UI;

public class DudeUI: MonoBehaviour
{
    public Dude target;
    public Slider hpSlider;
    public Text name;

    private Healthy healthy;

    private void Start()
    {
        healthy = target.GetComponent<Healthy>();
    }

    private void Update()
    {
        hpSlider.value = healthy.HPFraction;
        name.text = target.gameObject.name;
    }
}
