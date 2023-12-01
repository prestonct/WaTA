using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static SkillTree;

public class ItemCollection : MonoBehaviour
{
    public static ItemCollection itemCollection;
    void Start()
    {
        NumberOfItems = 0;
    }
    public int NumberOfItems { get; private set; }

    public UnityEvent<ItemCollection> OnItemCollected;

    // Coin collection system
    public void ItemCollected()
    {
        skillTree.SkillPoint += 1;
        NumberOfItems++;
        skillTree.UpdateAllSkillsUI();
        OnItemCollected.Invoke(this);
    }
}

