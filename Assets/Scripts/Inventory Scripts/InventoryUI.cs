using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
public class InventoryUI : MonoBehaviour
{
    private TextMeshProUGUI itemText;

    void Start()
    {
        itemText = GetComponent<TextMeshProUGUI>();
    }

    public void UpdateItemText(SkillTree skillTree)
    {
        itemText.text = skillTree.SkillPoint.ToString();
    }
}