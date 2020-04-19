using UnityEngine;

public class Healthy: MonoBehaviour
{
    public float damage;
    public float maxHp;

    public float HP => Mathf.Clamp(maxHp - damage, 0, maxHp);
    public float HPFraction => HP / maxHp;

    public void Damage(float amount, object source)
    {
        damage += amount;

        if (damage > maxHp)
        {
            Destroy(gameObject);
        }
    }
}
