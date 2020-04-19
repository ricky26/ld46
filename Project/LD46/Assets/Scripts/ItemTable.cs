
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Project/Item Table", order = 1)]
public class ItemTable: ScriptableObject
{
    public ItemType[] itemTypes;
    public WeaponType[] weaponTypes;

    public ItemType TypeFromID(string id)
    {
        return itemTypes.FirstOrDefault(x => x.id == id);
    }

    public WeaponType WeaponFromID(string id)
    {
        return weaponTypes.FirstOrDefault(x => x.id == id);
    }
}
