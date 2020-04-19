using UnityEngine;
using UnityEngine.UI;

public class SquadSpawner: MonoBehaviour
{
    public Team team;
    public GameObject dudePrefab;
    public GameObject squadPrefab;
    public int minSquadMembers = 1;
    public int maxSquadMembers = 1;

    public float squadSpawnMin = 1.0f;
    public float squadSpawnMax = 10.0f;
    public float dudeSpawnMin = 0.8f;
    public float dudeSpawnMax = 1.0f;

    private Squad squad;
    private int leftToSpawnInSquad;
    private float nextSpawnTime;
    private int squadIndex;


    private void Start()
    {
        nextSpawnTime = Random.Range(squadSpawnMin, squadSpawnMax);
    }

    private void Update()
    {
        var now = Time.time;
        if ((nextSpawnTime > now) || (team.EnemyDudes.Length == 0))
        {
            return;
        }

        if (leftToSpawnInSquad > 0)
        {
            // Spawn squad member.
            --leftToSpawnInSquad;
        }
        else if (team.layDownArms)
        {
            nextSpawnTime = now + Random.Range(squadSpawnMin, squadSpawnMax);
            return;
        }
        else
        {
            // Start a new squad.
            ++squadIndex;
            var squadGo = Instantiate(squadPrefab);
            squadGo.name = $"Squad {squadIndex} - {name}";
            squad = squadGo.AddComponent<Squad>();
            squad.team = team;
            leftToSpawnInSquad = Random.Range(minSquadMembers, maxSquadMembers) - 1;
        }

        nextSpawnTime = now + ((leftToSpawnInSquad > 0)
            ? Random.Range(dudeSpawnMin, dudeSpawnMax)
            : Random.Range(squadSpawnMin, squadSpawnMax));

        var dudeGo = Instantiate(dudePrefab, transform.position, transform.rotation);
        var dude = dudeGo.GetComponent<Dude>();
        dude.squad = squad;
    }
}
