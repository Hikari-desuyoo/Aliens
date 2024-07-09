
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Enemy : UdonSharpBehaviour
{
    public float maxHp = 10f;
    public Renderer renderer;
    public ParticleSystem particles;
    public GameObject ship;
    private float _hp = 10f;

    public void GetShot(float damage)
    {
        var targetHp = _hp - damage;
        if (targetHp <= 0)
        {
            targetHp = 0;
            ship.gameObject.SetActive(false);
            particles.gameObject.SetActive(true);
            SendCustomEventDelayedSeconds("Die", 5, VRC.Udon.Common.Enums.EventTiming.Update);
        }
        _hp = targetHp;
    }

    void Die()
    {
        particles.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
}
