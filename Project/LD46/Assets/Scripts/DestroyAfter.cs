using UnityEngine;

public class DestroyAfter : MonoBehaviour
{
    public float delay = 1.0f;

    private void Start()
    {
        Destroy(gameObject, delay);
    }
}
