
using UnityEngine;

public class GunshipMovement : MonoBehaviour
{
    public float range = 150.0f;
    public float orbitTime = 120.0f;

    private void Update()
    {
        var t = (Time.time / orbitTime) % 1;
        var angle = Mathf.PI * 2 * t;

        var x = Mathf.Sin(angle) * range;
        var z = Mathf.Cos(angle) * range;
        var pos = transform.position;
        pos.x = x;
        pos.z = z;
        transform.position = pos;
        transform.rotation = Quaternion.Euler(0, Mathf.Rad2Deg * angle, 0);
    }
}
