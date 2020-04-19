using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Project/Weapon Type", order = 1)]
public class WeaponType : ScriptableObject
{
    public string id;
    public float fireInterval = 1.0f;
    public float damage = 1.0f;
}
