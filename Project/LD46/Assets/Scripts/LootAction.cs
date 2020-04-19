using UnityEngine;

public class LootAction : MonoBehaviour
{
    public Objective objective;
    public LootTable lootTable;
    public float minLootTime = 0.3f;
    public float maxLootTime = 1.2f;

    private Squad squad;
    private float nextUpdate;

    public void StartLooting()
    {
        var now = Time.time;
        squad = objective.Squad;
        nextUpdate = now + Random.Range(minLootTime, maxLootTime);
    }

    public void StopLooting()
    {
        var now = Time.time;
        squad = null;
        nextUpdate = now + 60;
    }

    private void Update()
    {
        var now = Time.time;
        if (now < nextUpdate)
        {
            return;
        }

        var inventory = squad?.inventory;
        if (!inventory)
        {
            nextUpdate = now + 60;
            return;
        }

        var maybeStack = lootTable.Loot();
        if (maybeStack.HasValue)
        {
            var stack = maybeStack.Value;
            inventory.InsertAnywhere(ref stack);
        }

        nextUpdate = now + Random.Range(minLootTime, maxLootTime);
    }
}
