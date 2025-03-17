using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Player health")]
    [SerializeField] private Slider sliderHP;
    private int maxHP = 100;
    private int currentHP;

    private void Start()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        UpdateHealth();
        if (currentHP < 0) currentHP = 0;

        Debug.Log($"Get {damage} damage. HP: {currentHP}");

        if (currentHP == 0)
        {
            Die();
        }
    }
    private void UpdateHealth()
    {
        if (sliderHP.value != 0)
        {
            sliderHP.value = currentHP;
        }
    }
    public void Heal(int healAmount)
    {
        currentHP += healAmount;
        if (currentHP > maxHP) currentHP = maxHP;

        Debug.Log($"Healing {healAmount}. HP: {currentHP}");
    }

    private void Die()
    {
        Debug.Log("You die!");
    }
}
