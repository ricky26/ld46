using System.Linq;
using UnityEngine;

public class Team: MonoBehaviour
{
    public bool layDownArms;

    private const float scanInterval = 0.1f;
    private float lastScan;
    private Dude[] enemyDudes;

    public Dude[] EnemyDudes
    {
        get => enemyDudes ?? new Dude[0];
        set => enemyDudes = value;
    }

    private void Update()
    {
        var now = Time.time;
        var scanDelta = now - lastScan;

        if (scanDelta >= scanInterval)
        {
            lastScan = now;
            var allDudes = FindObjectsOfType<Dude>();
            EnemyDudes = allDudes
                .Where(dude => (dude.squad?.team != this) && dude.IsValidTarget)
                .ToArray();
        }
    }
}
