using UnityEngine;
using UnityEngine.UI;

public class EnemySpiderHP : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    [SerializeField] private Slider healthBar; // Ссылка на UI-хп бар

    private Transform player;

    private void Start()
    {
        currentHealth = maxHealth;
        healthBar.maxValue = maxHealth;
        healthBar.value = maxHealth;
        player = GameObject.FindGameObjectWithTag("player")?.transform;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.value = currentHealth;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    { 
        if (player.TryGetComponent(out PExp playerScript))
        {
            playerScript.AddExperience(10);
        }
        Destroy(gameObject); // Удаляем врага
    }
}
