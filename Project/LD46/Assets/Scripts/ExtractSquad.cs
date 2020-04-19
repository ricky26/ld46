using System;
using System.Linq;
using UnityEngine;

public class ExtractSquad : MonoBehaviour
{
    public Squad squad;
    public LocalState localState;
    public GameObject endGameScreen;
    public GameObject animPrefab;

    public void TriggerAnim()
    {
        Instantiate(animPrefab, transform.position, Quaternion.identity);
    }

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

        var dudes = squad.GetDudes().Select(dude => dude.loadout).Where(x => x).ToArray();
        var rax = new DudeLoadout[localState.barracks.Length + dudes.Length];
        Array.Copy(dudes, 0, rax, 0, dudes.Length);
        Array.Copy(localState.barracks, 0, rax, dudes.Length, localState.barracks.Length);
        localState.barracks = rax;
        localState.SaveAll();

        endGameScreen.SetActive(true);
    }
}
