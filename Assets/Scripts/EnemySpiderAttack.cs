using UnityEngine;
using System.Collections;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private int damage = 10;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 2f;
    private float nextAttackTime = 0f;

    private Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("player")?.transform;
    }

    private void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= attackRange && Time.time >= nextAttackTime)
        {
            Attack();
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    private void Attack()
    {
        if (player.TryGetComponent(out Player playerScript))
        {
            playerScript.TakeDamage(damage);
            Debug.Log($"Враг атаковал игрока на {damage} урона!");
        }
    }
}
