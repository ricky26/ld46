using System;
using System.IO;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;

public class LocalState: MonoBehaviour
{
    public ItemTable itemTable;
    public InventoryUI pocketsUI;
    public InventoryUI stashUI;
    public Inventory pockets;
    public Inventory stash;
    public GlobalSettings global;
    public DudeLoadout[] barracks;

    public string GlobalPath => Path.Combine(Application.persistentDataPath, "global.json");
    public string PocketsPath => Path.Combine(Application.persistentDataPath, "pockets.json");
    public string StashPath => Path.Combine(Application.persistentDataPath, "stash.json");
    public string BarracksPath => Path.Combine(Application.persistentDataPath, "barracks.json");
    public string SquadPath => Path.Combine(Application.persistentDataPath, "squad.json");

    private void Start()
    {
        global = LoadJson(GlobalPath, () => GlobalSettings.Create());

        pockets = LoadInventory(PocketsPath) ?? Inventory.Create(new Vector2Int(7, 4));
        pockets.name = "Pockets";
        pockets.OnStacksChanged.AddListener(SaveAll);

        stash = LoadInventory(StashPath) ?? Inventory.Create(new Vector2Int(14, 50));
        stash.name = "Stash";
        stash.OnStacksChanged.AddListener(SaveAll);

        barracks = LoadJson(BarracksPath, () => Array.Empty<SerializedDude>()).Select(Deserialize).ToArray();

        pocketsUI?.SetInventory(pockets);
        stashUI?.SetInventory(stash);
    }

    public void SaveAll()
    {
        SaveJson(GlobalPath, global);
        SaveJson(BarracksPath, barracks.Select(Serialize).ToArray());
        SaveInventory(PocketsPath, pockets);
        SaveInventory(StashPath, stash);
    }

    public DudeLoadout[] LoadSquad()
    {
        var dudes = LoadJson(SquadPath, () => Array.Empty<SerializedDude>()).Select(Deserialize).ToArray();
        if (File.Exists(SquadPath))
        {
            File.Delete(SquadPath);
        }
        return dudes;
    }

    public void SaveSquad(DudeLoadout[] dudes)
    {
        SaveJson(SquadPath, dudes.Select(Serialize).ToArray());
    }

    public static T LoadJson<T>(string path, Func<T> factory)
    {
        if (!File.Exists(path))
        {
            return factory();
        }

        var contents = File.ReadAllText(path);
        return JsonConvert.DeserializeObject<T>(contents);
    }

    public static void SaveJson(string path, object value)
    {
        var contents = JsonConvert.SerializeObject(value);
        File.WriteAllText(path, contents);
    }

    private Inventory LoadInventory(string path)
    {
        if (!File.Exists(path))
        {
            return null;
        }

        var contents = File.ReadAllText(path);
        return Deserialize(JsonConvert.DeserializeObject<SerializedInventory>(contents));
    }

    private void SaveInventory(string path, Inventory inventory)
    {
        var s = Serialize(inventory);
        var contents = JsonConvert.SerializeObject(s);
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
    private InventoryStack? MaybeDeserialize(SerializedInventoryStack? src)
    {
        return src.HasValue ? Deserialize(src.Value) : (InventoryStack?)null;
    }

    private static SerializedInventoryStack? MaybeSerialize(InventoryStack? src)
    {
        return src.HasValue ? Serialize(src.Value) : (SerializedInventoryStack?)null;
    }

    private Inventory Deserialize(SerializedInventory src)
    {
        var inventory = Inventory.Create(src.size);
        inventory.stacks = new InventoryStack?[src.size.x * src.size.y];
        
        for (var index = 0; index < inventory.stacks.Length; ++index)
        {
            if ((src.stacks == null) || (src.stacks.Length <= index))
            {
                break;
            }

            inventory.stacks[index] = MaybeDeserialize(src.stacks[index]);
        }

        return inventory;
    }

    private static SerializedInventory Serialize(Inventory src)
    {
        return new SerializedInventory
        {
            size = src.size,
            stacks = src.stacks.Select(MaybeSerialize).ToArray(),
        };
    }

    private DudeLoadout Deserialize(SerializedDude src)
    {
        var dest = DudeLoadout.Create();
        dest.name = src.name;
        dest.weapon = Deserialize(src.weapon);
        return dest;
    }

    private SerializedDude Serialize(DudeLoadout dude)
    {
        return new SerializedDude
        {
            name = dude.name,
            weapon = Serialize(dude.weapon),
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

    [Serializable]
    private struct SerializedDude
    {
        public string name;
        public SerializedInventoryStack weapon;
    }
}

[Serializable]
public class GlobalSettings
{
    public long nextDude;

    private static readonly DateTime unixEpoch = new DateTime(1970, 1, 1);

    public static long ToUnixTimestamp(DateTime dt)
    {
        return (long)dt.Subtract(unixEpoch).TotalSeconds;
    }

    public static DateTime FromUnixTimestamp(long ts)
    {
        return unixEpoch.AddSeconds((double)ts);
    }

    public static GlobalSettings Create()
    {
        return new GlobalSettings
        {
            nextDude = ToUnixTimestamp(DateTime.UtcNow),
        };
    }
}
