using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] protected float walkSpeed = 2f;
    [SerializeField] protected float chaseSpeed = 4f;
    [SerializeField] protected float wanderRadius = 5f;
    [SerializeField] protected float minWanderDelay = 1f;
    [SerializeField] protected float maxWanderDelay = 4f;
    [SerializeField] protected float attackFreezeDuration = 20f; // Новый параметр

    [Header("Combat")]
    [SerializeField] protected int maxHealth = 80;
    [SerializeField] protected int armor = 2;
    [SerializeField] protected int damage = 15;
    [SerializeField] protected float attackRange = 1f;
    [SerializeField] protected float attackCooldown = 2.5f;

    [Header("Vision")]
    [SerializeField] protected float visionRange = 8f;
    [SerializeField] protected float chaseRange = 12f;
    [SerializeField] protected LayerMask visionBlockingLayers;

    [Header("UI")]
    [SerializeField] protected Slider healthBar;

    protected Rigidbody2D rb;
    protected Animator animator;
    protected Transform player;
    protected Vector2 targetPosition;

    protected int currentHealth;
    protected float nextAttackTime;
    protected float nextWanderTime;
    protected bool isChasing;
    protected bool isAttacking; // Новый флаг
    protected float attackFreezeTimer; // Новый таймер

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("player")?.transform;
        currentHealth = maxHealth;
        SetNewWanderTarget();

        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = maxHealth;
        }
    }

    protected virtual void Update()
    {
        if (player == null) return;

        UpdateAttackFreezeTimer(); // Обновляем таймер заморозки

        if (!isAttacking) // Только если не в состоянии атаки
        {
            HandleVision();
            HandleMovement();
            HandleAttack();
        }
    }

    protected virtual void UpdateAttackFreezeTimer()
    {
        if (isAttacking)
        {
            attackFreezeTimer -= Time.deltaTime;
            if (attackFreezeTimer <= 0)
            {
                isAttacking = false;
            }
        }
    }

    protected virtual void HandleVision()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= chaseRange && CanSeePlayer())
        {
            isChasing = true;
        }
        else if (isChasing && distanceToPlayer > chaseRange * 1.5f)
        {
            isChasing = false;
            SetNewWanderTarget();
        }
    }

    protected virtual void HandleMovement()
    {
        if (isChasing)
        {
            ChasePlayer();
        }
        else
        {
            Wander();
        }
    }

    protected virtual void HandleAttack()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= attackRange && Time.time >= nextAttackTime)
        {
            Attack();
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    protected virtual void Attack()
    {
        isAttacking = true;
        attackFreezeTimer = attackFreezeDuration;
        rb.velocity = Vector2.zero; // Останавливаем движение

        animator.SetTrigger("Attack");
        if (player.TryGetComponent(out PlayerHealth health))
        {
            health.TakeDamage(damage);
        }
        // Базовая реализация атаки
    }

    // Остальные методы остаются без изменений
    protected virtual bool CanSeePlayer()
    {
        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, visionRange, visionBlockingLayers);
        return hit.collider == null || hit.collider.CompareTag("player");
    }

    protected virtual void ChasePlayer()
    {
        targetPosition = player.position;
        MoveToTarget(chaseSpeed);
    }

    protected virtual void Wander()
    {
        if (Time.time >= nextWanderTime)
        {
            SetNewWanderTarget();
        }
        MoveToTarget(walkSpeed);
        if (Vector2.Distance(transform.position, targetPosition) < 0.5f)
        {
            SetNewWanderTarget();
        }
    }

    protected virtual void SetNewWanderTarget()
    {
        Vector2 randomDirection = Random.insideUnitCircle * wanderRadius;
        targetPosition = (Vector2)transform.position + randomDirection;
        nextWanderTime = Time.time + Random.Range(minWanderDelay, maxWanderDelay);
    }

    protected virtual void MoveToTarget(float speed)
    {
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        rb.velocity = direction * speed;
    }

    public virtual void TakeDamage(int damage)
    {
        int modifiedDamage = CalculateModifiedDamage(damage);
        currentHealth -= modifiedDamage;
        Debug.Log($"Враг получил {modifiedDamage} урона (исходный: {damage}, броня: {armor})!");

        if (healthBar != null) healthBar.value = currentHealth;
        if (currentHealth <= 0) Die();
    }

    protected virtual int CalculateModifiedDamage(int baseDamage)
    {
        float damageAfterReduction = baseDamage * 0.9f;
        float finalDamage = damageAfterReduction - armor;
        return Mathf.RoundToInt(Mathf.Max(1, finalDamage));
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, wanderRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        if (Application.isPlaying)
        {
            Gizmos.color = isChasing ? Color.red : Color.green;
            Gizmos.DrawLine(transform.position, targetPosition);
        }
    }
}