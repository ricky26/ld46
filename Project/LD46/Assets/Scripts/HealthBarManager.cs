
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HealthBarManager: MonoBehaviour
{
    public GameObject healthBarPrefab;

    private const float updateInterval = 0.3f;
    private float nextUpdate;
    private Dictionary<Healthy, GameObject> healthBars = new Dictionary<Healthy, GameObject>();

    private void Start()
    {
        nextUpdate = Time.time;
    }

    private void Update()
    {
        var now = Time.time;
        if (now < nextUpdate)
        {
            return;
        }

        nextUpdate = now + updateInterval;

        var healthyObjects = FindObjectsOfType<Healthy>()
            .Where(x => x.HPFraction < 1.0f)
            .ToArray();

        var keysToRemove = healthBars
            .Where(kvp => !healthyObjects.Contains(kvp.Key))
            .ToArray();
        foreach (var kvp in keysToRemove)
        {
            Destroy(kvp.Value);
            healthBars.Remove(kvp.Key);
        }

        foreach (var healthy in healthyObjects)
        {
            if (!healthBars.ContainsKey(healthy))
            {
                var go = Instantiate(healthBarPrefab, transform);
                var healthBar = go.GetComponent<HealthBar>();
                healthBar.target = healthy;
                healthBars.Add(healthy, go);
            }
        }
    }
}