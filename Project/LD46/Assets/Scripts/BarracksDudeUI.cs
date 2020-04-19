using UnityEngine;
using UnityEngine.UI;

public class BarracksDudeUI: MonoBehaviour
{
    public DudeLoadout dude;
    public Text nameText;
    public Toggle takeToggle;

    public bool IsSelected
    {
        get => takeToggle.isOn;
        set => takeToggle.isOn = value;
    }

    private void OnEnable()
    {
        Configure(dude);
    }

    public void Configure(DudeLoadout dude)
    {
        if (!dude)
        {
            return;
        }

        nameText.text = dude.name;
    }
}
