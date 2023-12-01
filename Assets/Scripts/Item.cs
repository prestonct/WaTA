using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Item : MonoBehaviour
{

    ParticleSystem _particleSystem;
    bool shouldDestroy = false;
    float timeToDestroy = 0.1f;
    float timer = 0;
    private void Start()
    {
        Rigidbody rb = GetComponentInChildren<Rigidbody>();
        rb.AddTorque(new Vector3(0, 10, 0));
        _particleSystem = GetComponentInChildren<ParticleSystem>();
    }

    private void Update()
    {
        if (shouldDestroy)
        {
            if (timer > timeToDestroy)
            {
                Destroy(this.gameObject);
            }
            timer += Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        ItemCollection itemCollection = other.GetComponent<ItemCollection>();

        if (itemCollection != null)
        {
            _particleSystem.Play();
            itemCollection.ItemCollected();
            shouldDestroy = true;
        }
    }


}
