using UnityEngine;

public class GunController : MonoBehaviour
{
    private void Update()
    {
        transform.rotation = Quaternion.LookRotation(new Vector3(0, -0.1f, 0)-transform.position.normalized);
    }

    public void SetFiring(bool isFiring)
    {
        foreach (var gun in GetComponentsInChildren<IGun>())
        {
            gun.Firing = isFiring;
        }
    }

    public void AimAt(Vector3 targetLocation)
    {
        foreach (var gun in GetComponentsInChildren<IGun>())
        {
            gun.AimAt(targetLocation);
        }
    }
}
