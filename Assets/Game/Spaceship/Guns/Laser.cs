﻿
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Laser : Gun
{
    public LineRenderer lineRenderer;
    public Transform laserPointer;
    public float speed;
    public float fadeSpeed;
    public float reach;

    public override void OnShootingFixed()
    {
        // extends the laser (by changing the position of the
        // second point of the laser line renderer) unless
        // reach value has been... well... reached.

        var targetPosition = lineRenderer.GetPosition(1);
        targetPosition.z += speed;

        if(targetPosition.z > reach) return;

        lineRenderer.SetPosition(1, targetPosition);
    }

    public override void OnNotShootingFixed()
    {
        // diminishes the laser (by changing the position of the
        // second point of the laser line renderer)

        var targetPosition = lineRenderer.GetPosition(1);
        targetPosition.z -= fadeSpeed;

        // if its going to give the laser a negative z
        // it would make it go the opposite direction
        // so we fix it to be 0
        if(targetPosition.z < 0) targetPosition.z = 0;

        lineRenderer.SetPosition(1, targetPosition);
    }

    protected override void FixedUpdate()
    {
        // one might think why this is here and
        // not in OnShootingFixed, that's because
        // even when player has lifted the trigger,
        // the laser will still be fading away no its
        // not gone yet

        base.FixedUpdate();

        // does a raycast untill where the laser
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
            lineRenderer.transform.TransformPoint(
                lineRenderer.GetPosition(1)
            )
        );
    }
}
