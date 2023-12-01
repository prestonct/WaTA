using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehavior : MonoBehaviour
{
    public Transform aiTransform;
    public Transform Player;
    public bool isInstantiated = false;

    private void Awake()
    {
        Player = GameObject.Find("Player").transform;

    }

    public void InstantiateProjectile(Transform AItransform)
    {
        aiTransform = AItransform;
        isInstantiated = true;
    }

}
