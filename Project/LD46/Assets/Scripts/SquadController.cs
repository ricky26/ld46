
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SquadController: MonoBehaviour
{
    public Squad squad;
    public GameObject dudeUIPrefab;
    public Button[] objectiveButtons;
    public ObjectiveUI objectiveUI;
    public Text inventoryText;
    public GameObject gameOverScreen;
    public LocalState localState;
    public GameObject dudePrefab;
    public Transform spawnLocation;

    private ObjectiveKind? nextObjectiveKind;
    private Objective currentObjective;
    private const float updateInterval = 0.3f;
    private float nextUpdate;
    private Dictionary<Dude, GameObject> dudeUIs = new Dictionary<Dude, GameObject>();
    private Objective[] lastObjectives;
    private int lastObjectiveWrite;

    public void SetNextObjectiveType(ObjectiveKind nextKind)
    {
        nextObjectiveKind = nextKind;
        foreach (var btn in objectiveButtons)
        {
            btn.interactable = false;
        }
    }

    public void SetNextObjectiveTypeStr(string kindStr)
    {
        var kind = (ObjectiveKind)Enum.Parse(typeof(ObjectiveKind), kindStr);
        SetNextObjectiveType(kind);
    }

    private void Start()
    {
        lastObjectives = new Objective[5];
        lastObjectiveWrite = 0;
        nextUpdate = Time.time;

        squad.OnEnterObjective.AddListener(OnEnterObjective);

        foreach (var loadout in localState.LoadSquad())
        {
            var go = Instantiate(dudePrefab, spawnLocation.position, Quaternion.identity);
            var dude = go.GetComponent<Dude>();
            dude.loadout = loadout;
            dude.name = loadout.name;
            dude.squad = squad;
        }
    }

    private void OnEnterObjective(Objective obj)
    {
        if ((currentObjective == obj) && !obj.IsBusy)
        {
            obj.Activate(squad);
        }
    }

    private void UpdateDudes()
    {
        var myDudes = squad.GetDudes();
        var missingDudes = dudeUIs
            .Where(kvp => !myDudes.Contains(kvp.Key))
            .ToArray();

        foreach (var kvp in missingDudes)
        {
            dudeUIs.Remove(kvp.Key);
            Destroy(kvp.Value);
        }

        foreach (var dude in myDudes)
        {
            if (dudeUIs.ContainsKey(dude))
            {
                continue;
            }

            var uiGo = Instantiate(dudeUIPrefab, transform, false);
            var dudeUI = uiGo.GetComponent<DudeUI>();
            dudeUI.target = dude;
            dudeUIs.Add(dude, uiGo);
        }

        if (myDudes.Length == 0)
        {
            gameOverScreen.SetActive(true);
        }
    }

    private void UpdateObjective()
    {
        var now = Time.time;
        var targetKind = nextObjectiveKind ?? ObjectiveKind.Default;

        if (currentObjective && (currentObjective.IsBusy || (nextObjectiveKind == null) || (currentObjective.kind == targetKind)))
        {
            var isDone = currentObjective.GetIsCompleteAt(now);
            if (isDone)
            {
                objectiveUI.SetObjective(null);
                currentObjective.Complete(squad);
                currentObjective = null;
            }
            else
            {
                return;
            }
        }

        var potentialObjectives = FindObjectsOfType<Objective>()
            .Where(obj => (obj.kind == targetKind) && !lastObjectives.Contains(obj))
            .ToArray();
        if (potentialObjectives.Length == 0)
        {
            return;
        }

        var nextObjectiveIdx = UnityEngine.Random.Range(0, potentialObjectives.Length);
        currentObjective = potentialObjectives[nextObjectiveIdx];
        nextObjectiveKind = null;

        if (currentObjective.kind == ObjectiveKind.Default)
        {
            lastObjectives[lastObjectiveWrite] = currentObjective;
            lastObjectiveWrite = (lastObjectiveWrite + 1) % lastObjectives.Length;
        }

        objectiveUI.SetObjective(currentObjective);
        squad.goalTransform = currentObjective.transform;

        foreach (var btn in objectiveButtons)
        {
            btn.interactable = true;
        }
    }

    private void Update()
    {
        var now = Time.time;
        if (now < nextUpdate)
        {
            return;
        }
        nextUpdate = now + updateInterval;

        var numSlots = squad.inventory.NumSlots;
        var filledSlots = squad.inventory.FilledSlots;
        inventoryText.text = $"Inventory: {filledSlots}/{numSlots}";

        UpdateDudes();
        UpdateObjective();
    }
}
