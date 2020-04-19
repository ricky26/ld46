using UnityEngine;

public class GatlingGun : BaseGun
{
    public float maxBarrelSpeed = 1.0f;
    public float speedModifier = 0.9f;
    public float minBarrelSpeed = 0.9f;
    public float shotsPerRotation = 9.0f;
    public float errorDistance = 2.0f;
    public float maxHeat = 4.0f;
    public float overheatPenalty = 0.7f;
    public GameObject shellPrefab;
    public Transform barrel;
    public Transform firePoint;
    public Vector3 aimPoint;

    private Shell shellTemplate;
    private float barrelSpeed = 0.0f;
    private float lastFired = 0.0f;
    private float numRotations = 0.0f;
    private float heat = 0.0f;
    private bool isOverheating = false;

    public float Heat { get => heat; }

    public override void AimAt(Vector3 targetLocation)
    {
        aimPoint = targetLocation;
    }

    protected override void Start()
    {
        base.Start();

        shellTemplate = shellPrefab.GetComponentInChildren<Shell>();
    }

    protected override void Update()
    {
        base.Update();

        var dt = Time.deltaTime;
        var now = Time.time;

        var speedMod = Mathf.Pow(speedModifier, dt);
        var actuallyFiring = Firing && !isOverheating;

        if (actuallyFiring)
        {
            barrelSpeed += (maxBarrelSpeed - barrelSpeed) * (1 - speedMod);

            if (barrelSpeed > minBarrelSpeed)
            {
                heat = Mathf.Clamp01(heat + dt / maxHeat);
                if (heat >= 1)
                {
                    isOverheating = true;
                }
            }
        }
        else
        {
            barrelSpeed = barrelSpeed * speedMod;

            var penalty = isOverheating ? overheatPenalty : 1;
            heat = Mathf.Clamp01(heat - (dt * penalty) / maxHeat);

            if (heat <= 0)
            {
                isOverheating = false;
            }
        }

        numRotations += barrelSpeed * dt;

        var firePos = firePoint ? firePoint.position : transform.position;

        if (shellTemplate)
        {
            var direction = shellTemplate.CalculateFiringVector(firePos, aimPoint);
            transform.rotation = Quaternion.LookRotation(direction);
        }

        barrel.localRotation = Quaternion.Euler(0, 0, -numRotations * 360.0f);

        if (actuallyFiring && (barrelSpeed > minBarrelSpeed))
        {
            var deltaRotations = numRotations - lastFired;
            var numToFire = Mathf.FloorToInt(deltaRotations * shotsPerRotation);

            if (numToFire > 0)
            {
                lastFired = numRotations;

                for (var shellIdx = 0; shellIdx < numToFire; ++shellIdx)
                {
                    var errorAngle = Random.value * 2 * Mathf.PI;
                    var errorDistance = Random.value;
                    errorDistance = errorDistance * errorDistance * this.errorDistance;

                    var errorX = Mathf.Sin(errorAngle) * errorDistance;
                    var errorZ = Mathf.Cos(errorAngle) * errorDistance;
                    var error = new Vector3(errorX, 0, errorZ);

                    var go = Instantiate(shellPrefab, firePos, Quaternion.identity);
                    var shell = go.GetComponentInChildren<Shell>();
                    if (shell)
                    {
                        shell.Configure(firePos, aimPoint + error);
                    }
                }
            }
        }
        else
        {
            // If we're under firing speed, just reset the last fired time
            // so that we don't spew bullets when we eventually make it back up
            // to speed.
            numRotations = numRotations % 1;
            lastFired = numRotations;
        }
    }

    public override GameObject CreateUI()
    {
        return null;
    }
}
