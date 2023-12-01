using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class BulletProjectileBehavior : ProjectileBehavior
{
    public float forwardBulletForce = 32f;
    Rigidbody rb;
    public float timer = 5f;
    public float BulletProjectileDamage = 5f;
    private void Update()
    {
        if (isInstantiated)
        {
            rb = GetComponent<Rigidbody>();
            rb.AddForce(aiTransform.forward * forwardBulletForce, ForceMode.Impulse);
            isInstantiated = false;
        }

        timer = timer - Time.deltaTime;
        if (timer <= 0)
        {
            Destroy(gameObject);
        }

    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 3)
        {
            collision.gameObject.GetComponent<PlayerController>().TakeDamage(BulletProjectileDamage);

        }
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 3)
        {
            other.gameObject.GetComponent<PlayerController>().TakeDamage(BulletProjectileDamage);

        }
        Destroy(gameObject);
    }
}
