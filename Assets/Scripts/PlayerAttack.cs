using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public int damage = 20;
    public bool canHitMultipleTimes = false;
    public float lifeTime = 1;
    private float timer = 0;

    private void Update()
    {
        if (timer > lifeTime && lifeTime > 0)
        {
            Destroy(this); return;
        }
        else if (lifeTime > 0)
        {
            timer += Time.deltaTime;
        }
    }
}
