
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Enemy : UdonSharpBehaviour
{
    public int maxHp = 10;
    public Renderer renderer;
    private int _hp = 10;

    public void GetShot(int damage)
    {
        var targetHp = _hp - damage;
        if (targetHp <= 0)
        {
            targetHp = 0;
            Destroy(gameObject);
        }
        _hp = targetHp;
    }
}
