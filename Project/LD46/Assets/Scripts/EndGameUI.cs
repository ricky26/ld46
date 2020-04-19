using UnityEngine;
using UnityEngine.UI;

public class EndGameUI: MonoBehaviour
{
    public Squad squad;
    public Text resultText;
    public GameObject[] objectsToDisable;

    private void OnEnable()
    {
        var isWin = squad.GetDudes().Length > 0;
        resultText.text = isWin ? "Survived!" : "MIA";

        foreach (var obj in objectsToDisable)
        {
            obj.SetActive(false);
        }
    }
}
