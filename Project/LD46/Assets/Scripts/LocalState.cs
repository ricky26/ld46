using System;
using System.IO;
using System.Linq;
using UnityEngine;

public class LocalState: MonoBehaviour
{
    public ItemTable itemTable;
    public InventoryUI pocketsUI;
    public InventoryUI stashUI;
    public Inventory pockets;
    public Inventory stash;
    public GlobalSettings global;

    public string GlobalPath => Path.Combine(Application.persistentDataPath, "global.json");
    public string PocketsPath => Path.Combine(Application.persistentDataPath, "pockets.json");
    public string StashPath => Path.Combine(Application.persistentDataPath, "stash.json");

    private void Start()
    {
        global = LoadJson(GlobalPath, () => GlobalSettings.Create());

        pockets = LoadInventory(PocketsPath) ?? Inventory.Create(new Vector2Int(7, 4));
        pockets.name = "Pockets";
        pockets.OnStacksChanged.AddListener(SaveAll);

        stash = LoadInventory(StashPath) ?? Inventory.Create(new Vector2Int(14, 50));
        stash.name = "Stash";
        stash.OnStacksChanged.AddListener(SaveAll);

        pocketsUI?.SetInventory(pockets);
        stashUI?.SetInventory(stash);
    }

    public void SaveAll()
    {
        SaveJson(GlobalPath, global);
        SaveInventory(PocketsPath, pockets);
        SaveInventory(StashPath, stash);
    }

    private T LoadJson<T>(string path, Func<T> factory)
    {
        if (!File.Exists(path))
        {
            return factory();
        }

        var contents = File.ReadAllText(path);
        return JsonUtility.FromJson<T>(contents);
    }

    private void SaveJson(string path, object value)
    {
        var contents = JsonUtility.ToJson(value);
        File.WriteAllText(path, contents);
    }

    private Inventory LoadInventory(string path)
    {
        if (!File.Exists(path))
        {
            return null;
        }

        var contents = File.ReadAllText(path);
        return Deserialize(JsonUtility.FromJson<SerializedInventory>(contents));
    }

    private void SaveInventory(string path, Inventory inventory)
    {
        var s = Serialize(inventory);
        var contents = JsonUtility.ToJson(s);
        File.WriteAllText(path, contents);
    }

    private InventoryStack Deserialize(SerializedInventoryStack src)
    {
        return new InventoryStack
        {
            itemType = itemTable.TypeFromID(src.itemType),
            quantity = src.quantity,
        };
    }

    private static SerializedInventoryStack Serialize(InventoryStack src)
    {
        return new SerializedInventoryStack
        {
            itemType = src.itemType.id,
            quantity = src.quantity,
        };
    }
    private static InventoryStack? Deserialize(SerializedInventoryStack? src)
    {
        return src.HasValue ? Deserialize(src.Value) : (InventoryStack?)null;
    }

    private static SerializedInventoryStack? Serialize(InventoryStack? src)
    {
        return src.HasValue ? Serialize(src.Value) : (SerializedInventoryStack?)null;
    }

    private Inventory Deserialize(SerializedInventory src)
    {
        var inventory = Inventory.Create(src.size);
        inventory.stacks = src.stacks.Select(Deserialize).ToArray();
        return inventory;
    }

    private static SerializedInventory Serialize(Inventory src)
    {
        return new SerializedInventory
        {
            size = src.size,
            stacks = src.stacks.Select(Serialize).ToArray(),
        };
    }

    [Serializable]
    private struct SerializedInventoryStack
    {
        public string itemType;
        public int quantity;
    }

    [Serializable]
    private struct SerializedInventory
    {
        public Vector2Int size;
        public SerializedInventoryStack?[] stacks;
    }

}

[Serializable]
public struct GlobalSettings
{
    public long nextDude;

    private static readonly DateTime unixEpoch = new DateTime(1970, 1, 1);

    public static long ToUnixTimestamp(DateTime dt)
    {
        return (long)DateTime.UtcNow.Subtract(unixEpoch).TotalSeconds;
    }

    public static DateTime FromUnixTimestamp(long ts)
    {
        return unixEpoch.AddSeconds((double)ts);
    }

    public static GlobalSettings Create()
    {
        return new GlobalSettings
        {
            nextDude = ToUnixTimestamp(DateTime.UtcNow.AddMinutes(5)),
        };
    }
}
