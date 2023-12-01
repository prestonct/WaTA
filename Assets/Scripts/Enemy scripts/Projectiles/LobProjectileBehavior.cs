using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class LobProjectileBehavior : ProjectileBehavior
{
    public GameObject Explosion;
    public float upLobForce = 8f;
    Rigidbody rb;
    public float forwardForceMult;

    private void Update()
    {

        if (isInstantiated)
        {

            float distanceToPlayer = Vector3.Distance(transform.position, Player.position);
            float forwardLobForce = (forwardForceMult * distanceToPlayer);

            rb = GetComponent<Rigidbody>();
            rb.AddForce(aiTransform.forward * forwardLobForce, ForceMode.Impulse);
            rb.AddForce(aiTransform.up * upLobForce, ForceMode.Impulse);
            isInstantiated = false;
        }


    }

    private void OnCollisionStay(Collision collision)
    {
        Instantiate(Explosion, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}

