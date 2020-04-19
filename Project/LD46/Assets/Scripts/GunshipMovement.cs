
using UnityEngine;

public class GunshipMovement : MonoBehaviour
{
    public float range = 150.0f;
    public float orbitTime = 120.0f;
    public Vector3 focalPoint;
    public Vector3 smoothFocalPoint;

    private Vector3 focalPointVel;

    private const float focalPointSpeed = 3;

    private void Update()
    {
        focalPointVel = Vector3.MoveTowards(focalPointVel, focalPoint - smoothFocalPoint, focalPointSpeed * Time.deltaTime);
        smoothFocalPoint += Time.deltaTime * focalPointVel;

        var t = (Time.time / orbitTime) % 1;
        var angle = Mathf.PI * 2 * t;

        var x = Mathf.Sin(angle) * range;
        var z = Mathf.Cos(angle) * range;
        var pos = transform.position;
        pos.x = x;
        pos.z = z;
        transform.position = pos + smoothFocalPoint;
        transform.rotation = Quaternion.Euler(0, Mathf.Rad2Deg * angle, 0);
    }
}
