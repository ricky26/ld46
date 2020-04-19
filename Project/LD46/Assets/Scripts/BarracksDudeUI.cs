using UnityEngine;
using UnityEngine.UI;

public class BarracksDudeUI: MonoBehaviour
{
    public DudeLoadout dude;
    public Text nameText;

    private void OnEnable()
    {
        nameText.text = dude.name;
    }
}
