
using UnityEngine;

public abstract class BaseGun: MonoBehaviour, IGun
{
    public virtual bool Firing { get; set; }

    public virtual void AimAt(Vector3 targetLocation)
    {
        transform.forward = (targetLocation - transform.position).normalized;
    }

    public abstract GameObject CreateUI();

    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {

    }
}
