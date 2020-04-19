
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
        hpSlider.value = healthy ? healthy.HPFraction : 0;
        name.text = target ? target.gameObject?.name ?? "" : "";
    }
}
