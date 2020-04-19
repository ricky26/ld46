using UnityEngine;

public class TransferLoot: MonoBehaviour
{
    public InventoryUI targetUI;
    public GameObject toggleUI;
    public Objective objective;

    public void Transfer()
    {
        var inventory = objective?.Squad?.inventory;
        objective?.Squad?.NewInventory();
        targetUI.SetInventory(inventory);
        toggleUI.SetActive(inventory);
    }
}
