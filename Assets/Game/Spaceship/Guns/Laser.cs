
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Laser : Gun
{
    public LineRenderer lineRenderer;
    public Transform laserPointer;
    public ParticleSystem smoke;
    public float speed;
    public float fadeSpeed;
    public float reach;
    public int smokeAmount;
    private float _smokeTimer = 0f;

    void FixedUpdate()
    {
        if(shooting)
        {
            // extends the laser (by changing the position of the
            // second point of the laser line renderer) unless
            // reach value has been... well... reached.

            var targetPosition = lineRenderer.GetPosition(1);
            targetPosition.z += speed * Time.fixedDeltaTime;

            if(targetPosition.z > reach) return;

            lineRenderer.SetPosition(1, targetPosition);
        } else {
            // diminishes the laser (by changing the position of the
            // second point of the laser line renderer)

            var targetPosition = lineRenderer.GetPosition(1);
            targetPosition.z -= fadeSpeed * Time.fixedDeltaTime;

            // if its going to give the laser a negative z
            // it would make it go the opposite direction
            // so we fix it to be 0
            if(targetPosition.z < 0) targetPosition.z = 0;

            lineRenderer.SetPosition(1, targetPosition);
        }

        // does a raycast until where the laser
        // can reach to see if it hits anything

        var position = laserPointer.position;
        Ray ray = new Ray(position, laserPointer.forward);

        if (Physics.Raycast(ray, out var hit, GetLaserDistance()))
        {
            // if hit, laser length will be adjusted so it
            // doesn't get through the object
            var laserEndPosition = lineRenderer.GetPosition(1);
            laserEndPosition.z = hit.distance;
            lineRenderer.SetPosition(1, laserEndPosition);

            // actually transfer damage if its an enemy
            // also make it brighter! the amount of
            // brightness is defined in a way that
            // is relative to the enemy health
            // the number 20 comes from 20 being
            // the max emission on poiyomi

            if (hit.collider != null)
            {
                var enemy = hit.collider.gameObject.GetComponent<Enemy>();
                if(enemy != null)
                {
                    var targetDamage = damage * Time.fixedDeltaTime;
                    enemy.GetShot(targetDamage);
                    var emissionStrength = enemy.renderer.material.GetFloat("_EmissionStrength");
                    enemy.renderer.material.SetFloat(
                        "_EmissionStrength",
                        emissionStrength + 20f * targetDamage / enemy.maxHp
                    );
                }
            }

            // release smoke particles and position
            // on world position of laser ending point
            smoke.transform.position = GetLaserEndPoint();
            _smokeTimer += Time.fixedDeltaTime;
            var smokeAmountCurrent = (int) (_smokeTimer * smokeAmount);
            smoke.Emit(smokeAmountCurrent);
            _smokeTimer -= smokeAmountCurrent / (float) smokeAmount;
        }
    }

    float GetLaserDistance()
    {
        // gets the distance by calculating the distance
        // between the global position of the two points
        // of the lasers line renderer
        return Vector3.Distance(
            lineRenderer.transform.TransformPoint(
                lineRenderer.GetPosition(0)
            ),
            GetLaserEndPoint()
        );
    }

    Vector3 GetLaserEndPoint()
    {
        return lineRenderer.transform.TransformPoint(
            lineRenderer.GetPosition(1)
        );
    }
}
