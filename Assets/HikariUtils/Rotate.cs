
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Rotate : UdonSharpBehaviour
{
    public Vector3 rotation;

    void FixedUpdate()
    {
        transform.Rotate(rotation.x, rotation.y, rotation.z);
    }
}
