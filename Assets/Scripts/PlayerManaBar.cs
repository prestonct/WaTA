using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerManaBar : MonoBehaviour
{
    public Slider slider;


    // Manages mana bar
    public void SetMaxMana(float mana)
    {
        slider.maxValue = mana;
        slider.value = mana;
    }

    public void UpdateManaBar(float mana)
    {
        slider.value = mana;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
