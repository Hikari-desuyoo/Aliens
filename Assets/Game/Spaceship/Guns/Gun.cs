using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

// base class for any guns that might be attached to spaceship
public class Gun : UdonSharpBehaviour
{
    public bool shooting = false;
    public int damage = 2;

    // These are called every frame, depending if gun is being shot or not
    public virtual void OnShooting()
    {
    }

    public virtual void OnNotShooting()
    {
    }

    // These are called every 0.02 seconds, depending if gun is being shot or not
    public virtual void OnShootingFixed()
    {
    }

    public virtual void OnNotShootingFixed()
    {
    }

    protected virtual void Update()
    {
        // I hope I don't need to explain this one haha
        if (shooting) OnShooting();
        else OnNotShooting();
    }

    protected virtual void FixedUpdate()
    {
        if (shooting) OnShootingFixed();
        else OnNotShootingFixed();
    }
}
