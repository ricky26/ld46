
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public partial class Objective: MonoBehaviour
{
    public string description = "Doing a thing";
    public ObjectiveKind kind = ObjectiveKind.Default;
    public float duration = 1.0f;
    public float activateRadius = 5.0f;
    public bool hideDudes = false;
    public bool disableAttacking = false;

    public UnityEvent<Squad> OnStart;
    public UnityEvent<Squad> OnEnd;

    private float? startedAt;
    private float lastScan;
    private Squad squad;
    private const float scanInterval = 0.3f;

    public Squad Squad => squad;

    public float? Deadline => startedAt + duration;

    public bool IsBusy => startedAt.HasValue;

    public bool GetIsCompleteAt(float time)
    {
        return GetProgressFractionAt(time) >= 1.0f;
    }

    public float GetTimeRemainingAt(float time)
    {
        if (startedAt == null)
        {
            return 0.0f;
        }

        return (startedAt.Value + duration) - time;
    }

    public float GetProgressFractionAt(float time)
    {
        if (startedAt == null)
        {
            return 0.0f;
        }

        return Mathf.Clamp01((time - startedAt.Value) / duration);
    }

    public void Activate(Squad squad)
    {
        startedAt = Time.time;
        this.squad = squad;
        OnStart.Invoke(squad);

        var dudes = FindObjectsOfType<Dude>(true)
            .Where(dude => dude.squad == squad)
            .ToArray();

        foreach (var dude in dudes)
        {
            dude.canAttack = !disableAttacking;
            //dude.gameObject.SetActive(!hideDudes);
        }
    }

    public void Complete(Squad squad)
    {
        var now = Time.time;
        var deadline = Deadline;

        if ((this.squad == squad) && deadline.HasValue && (deadline.Value <= now))
        {
            var dudes = FindObjectsOfType<Dude>(true)
                .Where(dude => dude.squad == squad)
                .ToArray();

            foreach (var dude in dudes)
            {
                if (!dude.gameObject.activeSelf)
                {
                    dude.transform.position = transform.position;
                }

                dude.canAttack = true;
                dude.gameObject.SetActive(true);
            }

            OnEnd.Invoke(squad);
            startedAt = null;
            squad = null;
        }
    }

    private void Update()
    {
        var now = Time.time;
        var scanDelta = now - lastScan;

        if (scanDelta > scanInterval)
        {
            lastScan = now;

            foreach (var collider in Physics.OverlapSphere(transform.position, activateRadius))
            {
                var dude = collider.GetComponentInParent<Dude>();
                if (dude && dude.squad)
                {
                    dude.squad.TriggerEnterObjective(this, dude);
                }
            }
        }
    }
}
