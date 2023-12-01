using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHUD : MonoBehaviour
{
    public float maxHealth = 100.0f;
    public float currentHealth;
    public float maxMana = 100.0f;
    public float currentMana;

    public PlayerHealthBar healthBar;
    public PlayerManaBar manaBar;

    public float healOverTimeAmount = 7f;
    public float healOverTimeDelay = 4f;
    public float damageCooldown = 5f;

    private bool canHeal = true;
    public UIManager manager;

    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        currentMana = maxMana;
        manaBar.SetMaxMana(maxMana);

        StartCoroutine(HealOverTime());
    }

    void Update()
    {

    }

    IEnumerator HealOverTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(healOverTimeDelay);

            if (canHeal && currentHealth < maxHealth)
            {
                currentHealth += healOverTimeAmount;
                healthBar.UpdateHealthBar(currentHealth);
            }
        }
    }

    public void TakeDamage(float damage)
    {
        Debug.Log("Hit!");
        currentHealth -= damage;
        healthBar.UpdateHealthBar(currentHealth);
        if(currentHealth < 0)
        {
            manager.gameOver.onPause();
        }
        StartCoroutine(DamageCooldown());
    }

    IEnumerator DamageCooldown()
    {
        canHeal = false;
        yield return new WaitForSeconds(damageCooldown);
        canHeal = true;
    }
    public void Heal(float healing)
    {
        currentHealth += healing;
        healthBar.UpdateHealthBar(currentHealth);
    }

    public void UseMana(float usage)
    {
        currentMana -= usage;
        manaBar.UpdateManaBar(currentMana);
    }

    public void RechargeMana(float recharge)
    {
        currentMana += recharge;
        manaBar.UpdateManaBar(currentMana);
    }
}
