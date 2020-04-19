using UnityEngine;

public class GunController : MonoBehaviour
{
    public GunshipMovement gunshipMovement;

    private void Update()
    {
        transform.rotation = Quaternion.LookRotation(gunshipMovement.smoothFocalPoint - transform.position);
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
