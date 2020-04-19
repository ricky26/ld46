
using System.Linq;
using UnityEngine;

public class Waypoint: MonoBehaviour
{
    private new SphereCollider collider;

    private void Start()
    {
        collider = GetComponent<SphereCollider>();
    }

    private void Update()
    {
        foreach (var other in Physics.OverlapSphere(transform.position, collider.radius))
        {
            var dude = other.GetComponentInParent<Dude>();
            if (dude?.squad?.goalTransform == transform)
            {
                var otherWaypoints = FindObjectsOfType<Waypoint>()
                    .Where(x => x != this)
                    .ToArray();
                var idx = Random.Range(0, otherWaypoints.Length);
                dude.squad.goalTransform = otherWaypoints[idx].transform;
            }
        }
    }
}
