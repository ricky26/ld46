
using UnityEngine;

public class SquadLoader: MonoBehaviour
{
    public LocalState localState;
    public Squad squad;
    public GameObject dudePrefab;

    private void Start()
    {
        foreach (var loadout in localState.LoadSquad())
        {
            var go = Instantiate(dudePrefab, transform.position, Quaternion.identity);
            var dude = go.GetComponent<Dude>();
            dude.loadout = loadout;
            dude.name = loadout.name;
        }
    }
}
