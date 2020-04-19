using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BarracksUI : MonoBehaviour
{
    public RectTransform dudeContainer;
    public LocalState localState;
    public MatchController matchController;
    public Text dudeCountdown;
    public DudeConfig dudeConfig;
    public ItemTable itemTable;
    public GameObject dudeUIPrefab;
    public Button startMatchButton;
    public float dudeInterval = 300f;
    public int maxSquadSize = 5;

    private List<GameObject> dudeUIs;

    private void Start()
    {
        dudeUIs = new List<GameObject>();
    }

    private void Update()
    {
        var now = DateTime.UtcNow;
        var dt = GlobalSettings.FromUnixTimestamp(localState.global.nextDude);
        var span = now - dt;
        
        if (now > dt)
        {
            var numToSpawn = 1 + Mathf.FloorToInt((float)span.TotalSeconds / dudeInterval);
            dt = dt.AddSeconds(dudeInterval * numToSpawn);
            localState.global.nextDude = GlobalSettings.ToUnixTimestamp(dt);

            localState.barracks = localState.barracks.Concat(new[]
            {
                DudeLoadout.GenerateNew(dudeConfig, itemTable),
            }).ToArray();

            localState.SaveAll();
        }

        dudeCountdown.text = $"{Mathf.CeilToInt((float)-span.TotalSeconds)}s until new dude";

        var numChecks = 0;

        for (var index = 0; index < localState.barracks.Length; ++index)
        {
            if (index >= dudeUIs.Count)
            {
                var go = Instantiate(dudeUIPrefab, dudeContainer);
                dudeUIs.Add(go);
            }

            var ui = dudeUIs[index].GetComponent<BarracksDudeUI>();
            ui.Configure(localState.barracks[index]);

            if (ui.IsSelected)
            {
                if (numChecks >= maxSquadSize)
                {
                    ui.IsSelected = false;
                }
                else
                {
                    ++numChecks;
                }
            }
        }

        for (var index = localState.barracks.Length; index < dudeUIs.Count;)
        {
            var go = dudeUIs[index];
            Destroy(go);
            dudeUIs.RemoveAt(index);
        }

        if ((numChecks == 0) && (dudeUIs.Count > 0))
        {
            dudeUIs[0].GetComponent<BarracksDudeUI>().IsSelected = true;
            numChecks++;
        }

        startMatchButton.interactable = numChecks > 0;
    }

    public void StartMatch()
    {
        var selectedIndices = new List<int>();
        var rax = localState.barracks;

        for (int index = 0; index < rax.Length; ++index)
        {
            var dude = rax[index];
            if (dudeUIs[index].GetComponent<BarracksDudeUI>().IsSelected)
            {
                selectedIndices.Add(index);
            }
        }

        var squad = selectedIndices.Select(idx => localState.barracks[idx]).ToArray();
        localState.barracks = rax.Where((_, idx) => !selectedIndices.Contains(idx)).ToArray();
        localState.SaveSquad(squad);
        localState.SaveAll();

        matchController.StartMatch();
    }

    public void AddDude()
    {
        localState.global.nextDude = GlobalSettings.ToUnixTimestamp(DateTime.UtcNow);
    }
}
