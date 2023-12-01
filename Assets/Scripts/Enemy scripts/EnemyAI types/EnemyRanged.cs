using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;


public class EnemyRanged : EnemyBehavior
{

    //attacking
    public GameObject Projectile;

    public override void AttackPlayer()
    {
        base.AttackPlayer();

        if (!alreadyAttacked)
        {
            GameObject projectile = Instantiate(Projectile, transform.position, Quaternion.identity);
            projectile.GetComponent<ProjectileBehavior>().InstantiateProjectile(transform);
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);

        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }


}
