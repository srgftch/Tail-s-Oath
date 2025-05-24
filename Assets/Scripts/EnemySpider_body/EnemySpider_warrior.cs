using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemySpider_warrior: Enemy
{
    protected override void Start()
    {
        base.Start();
    }
    void Update()
    {
        base.Update();
    }

    protected override void Attack()
    {
        base.Attack(); // Если нужно сохранить базовое поведение

        if (player != null && player.TryGetComponent(out PlayerHealth playerScript))
        {
           // playerScript.TakeDamage(damage);
            Debug.Log($"Паук-воин атаковал игрока на {damage} урона!");
        }
    }
    protected override void Die()
    {
        // Добавляем специфичное поведение при смерти паука
        Debug.Log("Паук-воин умер!");
        base.Die();
    }
}
