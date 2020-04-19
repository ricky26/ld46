using UnityEngine;
using UnityEngine.UI;

public class BarracksDudeUI: MonoBehaviour
{
    public DudeLoadout dude;
    public Text nameText;

    private void OnEnable()
    {
        if (!dude)
        {
            return;
        }

        nameText.text = dude.name;
    }
}
