using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PAttack : MonoBehaviour
{
    [SerializeField] private Player_control player;
    [SerializeField] private Animator animator;

    [SerializeField] private int attackDamage = 20; // ���� �� �����
    [SerializeField] private float attackRange = 1.5f; // ������ �����
    [SerializeField] private float attackCooldown = 1f; // ����� ����� �������
    private float nextAttackTime = 0f;

    [SerializeField] private Transform attackPointLeft; // �����, �� ������� ��� ����
    [SerializeField] private Transform attackPointRight;
    [SerializeField] private Transform attackPointUp;
    [SerializeField] private Transform attackPointDown;

    [SerializeField] private LayerMask enemyLayer; // ���� ������

    private void Start()
    {
    }

    public void AttackInput(InputAction.CallbackContext context)
    {
        if (context.performed && Time.time >= nextAttackTime) // ��������, ����� ����� ����������� ������ ��� ������� � � ������ ��������
        {
            Attack(); // �������� ����� �����
            nextAttackTime = Time.time + attackCooldown; // ��������� ����� ��������� �����
        }
    }

    private void Attack()
    {
        if (!player.canMove) return;
        animator.SetTrigger("Attack");

        // �������� ����������� �����
        (float, float) direction = (
            Mathf.RoundToInt(animator.GetFloat("lastX")),
            Mathf.RoundToInt(animator.GetFloat("lastY"))
        );

        // ���������� ��� �������� ������ ������
        Collider2D[] hitEnemies = direction switch
        {
            (1, 0) => Physics2D.OverlapCircleAll(attackPointRight.position, attackRange, enemyLayer),
            (-1, 0) => Physics2D.OverlapCircleAll(attackPointLeft.position, attackRange, enemyLayer),
            (0, 1) => Physics2D.OverlapCircleAll(attackPointUp.position, attackRange, enemyLayer),
            (0, -1) => Physics2D.OverlapCircleAll(attackPointDown.position, attackRange, enemyLayer),
            _ => new Collider2D[0] // ���� ����������� ����������, �� �������
        };



        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.TryGetComponent(out Enemy enemyHealth))
            {
                enemyHealth.TakeDamage(attackDamage);
                Debug.Log($"player attacked on {attackDamage} damage!");
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

    public void AttackUp(int up)
    {
        attackDamage += up;
    }

    public void AttackRangeUp(float up)
    {
        attackRange += up;
    }
}
