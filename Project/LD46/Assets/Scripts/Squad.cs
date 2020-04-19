using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Squad: MonoBehaviour
{
    public Team team;
    public Transform goalTransform;
    public UnityEvent<Objective> OnEnterObjective;
    public Inventory inventory;
    public Vector2Int inventorySize;

    public void TriggerEnterObjective(Objective objective)
    {
        OnEnterObjective?.Invoke(objective);
    }

    public void NewInventory()
    {
        inventory = Inventory.Create(inventorySize);
        inventory.name = $"{name} Inventory";
    }

    public Dude[] GetDudes()
    {
        return FindObjectsOfType<Dude>(true)
            .Where(x => x.squad == this)
            .ToArray();
    }

    private void Start()
    {
        if (inventory == null)
        {
            NewInventory();
        }
    }
}
