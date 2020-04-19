using UnityEngine;

interface IGun
{
    bool Firing { set; }

    void AimAt(Vector3 targetLocation);

    GameObject CreateUI();
}
