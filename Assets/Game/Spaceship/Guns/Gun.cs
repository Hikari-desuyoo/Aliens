using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

// base class for any guns that might be attached to spaceship
public class Gun : UdonSharpBehaviour
{
    public bool shooting = false;
    public float damage = 2f;
}
