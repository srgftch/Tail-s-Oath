using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PAttack : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Animator animator;

    [SerializeField] private int attackDamage = 20; // Урон от атаки
    [SerializeField] private float attackRange = 1.5f; // Радиус атаки
    [SerializeField] private float attackCooldown = 1f; // Время между атаками
    private float nextAttackTime = 0f;

    [SerializeField] private Transform attackPointLeft; // Точки, из которых идёт удар
    [SerializeField] private Transform attackPointRight;
    [SerializeField] private Transform attackPointUp;
    [SerializeField] private Transform attackPointDown;

    [SerializeField] private LayerMask enemyLayer; // Слой врагов

    private void Start()
    {
    }

    public void AttackInput(InputAction.CallbackContext context)
    {
        if (context.performed && Time.time >= nextAttackTime) // Проверка, чтобы атака происходила только при нажатии и с учетом кулдауна
        {
            Attack(); // Вызываем метод атаки
            nextAttackTime = Time.time + attackCooldown; // Обновляем время следующей атаки
        }
    }

    private void Attack()
    {
        if (!player.canMove) return;
        animator.SetTrigger("Attack");

        // Получаем направление атаки
        (float, float) direction = (
            Mathf.RoundToInt(animator.GetFloat("lastX")),
            Mathf.RoundToInt(animator.GetFloat("lastY"))
        );

        // Переменная для хранения списка врагов
        Collider2D[] hitEnemies = direction switch
        {
            (1, 0) => Physics2D.OverlapCircleAll(attackPointRight.position, attackRange, enemyLayer),
            (-1, 0) => Physics2D.OverlapCircleAll(attackPointLeft.position, attackRange, enemyLayer),
            (0, 1) => Physics2D.OverlapCircleAll(attackPointUp.position, attackRange, enemyLayer),
            (0, -1) => Physics2D.OverlapCircleAll(attackPointDown.position, attackRange, enemyLayer),
            _ => new Collider2D[0] // Если направление неизвестно, не атакуем
        };



        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.TryGetComponent(out Enemy enemyHealth))
            {
                enemyHealth.TakeDamage(attackDamage);
                Debug.Log($"Игрок ударил врага на {attackDamage} урона!");
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPointRight == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPointRight.position, attackRange);
        Gizmos.DrawWireSphere(attackPointLeft.position, attackRange);
        Gizmos.DrawWireSphere(attackPointUp.position, attackRange);
        Gizmos.DrawWireSphere(attackPointDown.position, attackRange);
    }
}
