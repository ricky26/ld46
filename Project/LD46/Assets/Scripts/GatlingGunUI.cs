
using UnityEngine;
using UnityEngine.UI;

public class GatlingGunUI: MonoBehaviour
{
    public GatlingGun gatlingGun;
    public Slider heatUI;

    private void Update()
    {
        heatUI.value = gatlingGun.Heat;
    }
}
