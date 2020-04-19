using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(GunController))]
public class LocalPlayer : MonoBehaviour
{
    public bool forceFiring = false;

    private Plane aimPlane;
    private new Camera camera;
    private GunController gunController;

    private void Start()
    {
        aimPlane = new Plane(Vector3.up, Vector3.zero);
        camera = GetComponentInChildren<Camera>();
        gunController = GetComponent<GunController>();
    }

    private void Update()
    {
        var ray = camera.ScreenPointToRay(Input.mousePosition);
        float enter;

        if (aimPlane.Raycast(ray, out enter))
        {
            var hitPoint = ray.GetPoint(enter);
            gunController.AimAt(hitPoint);
        }

        gunController.SetFiring(forceFiring || (!EventSystem.current.IsPointerOverGameObject() && Input.GetButton("Fire1")));
    }
}
