using UnityEngine;

public class ExtractSquad : MonoBehaviour
{
    public Squad squad;
    public LocalState localState;
    public GameObject endGameScreen;

    public void Extract()
    {
        if (squad.GetDudes().Length == 0)
        {
            return;
        }

        foreach (var team in FindObjectsOfType<Team>())
        {
            team.layDownArms = true;
        }

        endGameScreen.SetActive(true);
    }
}
