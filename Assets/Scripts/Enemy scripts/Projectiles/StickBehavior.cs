using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class StickBehavior : ProjectileBehavior
{
    public float timer = .5f;
    public float StickDamage = 10f;
    private void Update()
    {
        if (isInstantiated)
        {
            transform.forward = aiTransform.forward;
            isInstantiated = false;
        }

        timer = timer - Time.deltaTime;
        if (timer <= 0)
        {
            Destroy(gameObject);
        }

    }
    private void Start()
    {
        GameObject.Find("AudioManager").GetComponent<AudioManager>().OnStickShootSounds(transform.position);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.layer == 3)
        {
            collision.gameObject.GetComponent<PlayerController>().TakeDamage(StickDamage);

        }
    }
}
