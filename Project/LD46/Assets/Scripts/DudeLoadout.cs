using UnityEngine;

[CreateAssetMenu(fileName = "Dude", menuName = "Project/Dude Loadout", order = 1)]
public class DudeLoadout : ScriptableObject
{
    public InventoryStack weapon;

    public static DudeLoadout Create()
    {
        var loadout = CreateInstance<DudeLoadout>();
        return loadout;
    }

    public static DudeLoadout GenerateNew(DudeConfig config, ItemTable itemTable)
    {
        var loadout = Create();
        var nameIdx = Random.Range(0, config.names.Length);
        loadout.name = config.names[nameIdx];
        loadout.weapon = itemTable.TypeFromID("pistol").CreateStack();
        return loadout;
    }
}
