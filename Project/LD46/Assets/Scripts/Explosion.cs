﻿using UnityEngine;

public class Explosion: MonoBehaviour
{
    public float explosionRadius = 0.0f;
    public float damage = 0.0f;
    public float lifetime = 1.0f;
    public AudioClip[] soundEffects;

    private float spawnedAt = 0.0f;

    private void Start()
    {
        spawnedAt = Time.time;

        if (soundEffects?.Length > 0)
        {
            var clip = soundEffects[Random.Range(0, soundEffects.Length)];
            AudioSource.PlayClipAtPoint(clip, transform.position);
        }

        var position = transform.position;
        foreach (var collider in Physics.OverlapSphere(position, explosionRadius))
        {
            var healthy = collider.GetComponentInParent<Healthy>();
            if (healthy)
            {
                var fac = (collider.ClosestPoint(position) - position).magnitude / explosionRadius;
                var damage = fac * fac * this.damage;
                healthy.Damage(damage, this);
            }
        }
    }

    private void Update()
    {
        var now = Time.time;
        var t = Mathf.Clamp01((now - spawnedAt) / lifetime);

        // Apply scale?

        if (t >= 1)
        {
            Destroy(gameObject);
        }
    }
}
