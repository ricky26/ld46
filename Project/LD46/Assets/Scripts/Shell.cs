
using UnityEngine;

public class Shell: MonoBehaviour
{
    public float gravityFactor = 0.0f;
    public float flightDuration = 0.0f;
    public GameObject damagePrefab;

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float startedTime;

    private Vector3 CalculatePositionAtTime(Vector3 startPosition, Vector3 targetPosition, float time)
    {
        var deltaPos = (targetPosition - startPosition);
        var deltaNorm = deltaPos.normalized;
        var flatFactor = deltaPos * time;

        var curveTime = Mathf.Sin(Mathf.PI * time);
        var right = Vector3.Cross(deltaNorm, Vector3.up);
        var up = -Vector3.Cross(deltaNorm, right);
        var curveFactor = up * curveTime * gravityFactor;

        return startPosition + flatFactor + curveFactor;
    }

    private Vector3 CalculateDirectionAtTime(Vector3 startPosition, Vector3 targetPosition, float time)
    {
        var deltaPos = (targetPosition - startPosition);
        var deltaNorm = deltaPos.normalized;

        var right = Vector3.Cross(deltaNorm, Vector3.up);
        var up = -Vector3.Cross(deltaNorm, right);

        var curveTime = Mathf.Cos(Mathf.PI * time);
        var maxCurve = (0.5f * deltaPos + 2 * up * gravityFactor).normalized;

        return Vector3.LerpUnclamped(deltaNorm, maxCurve, curveTime);
    }

    public Vector3 CalculateFiringVector(Vector3 startPosition, Vector3 targetPosition)
    {
        return CalculateDirectionAtTime(startPosition, targetPosition, 0);
    }

    public void Configure(Vector3 startPosition, Vector3 targetPosition)
    {
        startedTime = Time.time;
        this.startPosition = startPosition;
        this.targetPosition = targetPosition;
        Update();
    }

    private void Update()
    {
        var now = Time.time;
        var t = Mathf.Clamp01((now - startedTime) / flightDuration);

        transform.position = CalculatePositionAtTime(startPosition, targetPosition, t);
        transform.rotation = Quaternion.LookRotation(CalculateDirectionAtTime(startPosition, targetPosition, t));

        if (t >= 1)
        {
            if (damagePrefab)
            {
                Instantiate(damagePrefab, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }
    }
}
