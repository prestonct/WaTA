using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;


public class EnemyMelee : EnemyBehavior
{

    //attacking
    public GameObject Stick;
    public float weaponLength;
    public float weaponDamage;
    public bool canDealDamage;
    public bool hasDealtDamage;

    protected override void Start()
    {
        base.Start();
        canDealDamage = false;
        hasDealtDamage = false;
    }

    public override void AttackPlayer()
    {
        base.AttackPlayer();

        if (!alreadyAttacked)
        {
            GameObject stick = Instantiate(Stick, transform.position + transform.forward * 2, Quaternion.identity);
            stick.GetComponent<ProjectileBehavior>().InstantiateProjectile(transform);
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }


    public void StartDealDamage()
    {
        canDealDamage = true;
        hasDealtDamage = false;
    }
    public void EndDealDamage()
    {
        canDealDamage = false;
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, transform.position - transform.up * weaponLength);
    }



}
