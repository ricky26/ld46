
using UnityEngine;

public class Laser: MonoBehaviour
{
    private void Start()
    {
        Destroy(gameObject, 0.3f);
    }
}
