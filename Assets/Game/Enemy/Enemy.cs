
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Enemy : UdonSharpBehaviour
{
    public int maxHp = 10;
    public Renderer renderer;
    public ParticleSystem particles;
    public GameObject ship;
    private int _hp = 10;

    public void GetShot(int damage)
    {
        var targetHp = _hp - damage;
        if (targetHp <= 0)
        {
            targetHp = 0;
            ship.gameObject.SetActive(false);
            particles.gameObject.SetActive(true);
        }
        _hp = targetHp;
    }

    void Update()
    {
        if (particles.gameObject.activeInHierarchy && !particles.IsAlive()) {
            // Resets enemy for it to go back to the pool
            particles.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }
    }
}
