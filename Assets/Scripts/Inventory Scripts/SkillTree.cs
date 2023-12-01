using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SkillTree : MonoBehaviour
{
    public static SkillTree skillTree;
    private void Awake() => skillTree = this;

    public int[] SkillLevels;
    public int[] SkillCaps;
    public string[] SkillNames;
    public string[] SkillDescriptions;

    public List<Skill> SkillList;
    public GameObject SkillHolder;

    public List<GameObject> ConnectorList;
    public GameObject ConnectorHolder;
    public TextMeshProUGUI scaleCounter;

    public int SkillPoint;
    public UIManager uIManager;

    private void Start()
    {
        SkillPoint = 0;

        // To be decided on how many skills, will need to change amounts of SkillLevels, SkillCaps, and SkillDescription.
        SkillLevels = new int[5];
        SkillCaps = new[] { 1, 1, 1, 1, 1 };

        SkillNames = new[] { "Blink", "Unimplimented", "Heavy Attack", "Unimplimented", "Unimplimented Ult" };
        SkillDescriptions = new[]
        {
            "Teleport forward quickly",
            ":'(",
            "Hold for a heavy attack",
            ":'(",
            ":'("
        };

        foreach (var skill in SkillHolder.GetComponentsInChildren<Skill>()) SkillList.Add(skill);
        foreach (var connector in ConnectorHolder.GetComponentsInChildren<RectTransform>()) ConnectorList.Add(connector.gameObject);

        for (var i = 0; i < SkillList.Count; i++) SkillList[i].id = i;

        SkillList[0].ConnectedSkills = new[] { 1 };
        SkillList[2].ConnectedSkills = new[] { 3 };
        SkillList[1].ConnectedSkills = new[] { 4 };
        SkillList[3].ConnectedSkills = new[] { 4 };


        UpdateAllSkillsUI();
    }

    public void UpdateAllSkillsUI()
    {
        if(SkillPoint == 20)
        {
            // end the game we won
            uIManager.winScreen.onPause();
        }
        foreach (var skill in SkillList) skill.UpdateUI();
        scaleCounter.text = "Gurt Scales:<br>" + SkillPoint + "/20";
    }
}
