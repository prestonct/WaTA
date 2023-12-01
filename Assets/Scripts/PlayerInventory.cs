using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements;


public class PlayerInventory : MonoBehaviour
{
    public static bool inInventory = false;
    public GameObject inventoryUI;
    public GameObject HUD;


    public void OpenInventory()
    {
        //if (inInventory)
        //{
        //    Close();
        //}
        //else
        //{
        //    Open();
        //}
    }

    public void Close()
    {
        HUD.SetActive(true);
        inventoryUI.SetActive(false);
        inInventory = false;
        Time.timeScale = 1f;
    }

    public void Open()
    {
        HUD.SetActive(false);
        inventoryUI.SetActive(true);
        inInventory = true;
        Time.timeScale = 0f;
    }
}
