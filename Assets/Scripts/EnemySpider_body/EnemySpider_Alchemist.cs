using UnityEngine;

public class EnemySpiderAlchemist: Enemy
{
    [Header("Shooting Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform shootingPoint;
    [SerializeField] private float projectileSpeed = 7f;

    [Header("Ядовитая жижа")]
    [SerializeField] private GameObject poisonZonePrefab;
    [SerializeField] private float damageTime = 4f;
    [SerializeField] private float damageInterval = 1f;
    [SerializeField] private int zoneDamage = 5;
    [SerializeField] private float warningTime = 1f;
    [SerializeField] private float fadeOutTime = 0.3f;
    [SerializeField] private float cooldown = 14f;

    private float lastAbilityTime;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        if (CanUsePoisonAbility())
        {
            UsePoisonAbility();
        }
    }

    private bool CanUsePoisonAbility()
    {
        return Time.time >= lastAbilityTime + cooldown &&
               Vector2.Distance(transform.position, player.position) <= attackRange;
    }

    private void UsePoisonAbility()
    {
        lastAbilityTime = Time.time;

        // Создаем зону на текущей позиции игрока
        GameObject zone = Instantiate(
            poisonZonePrefab,
            player.position,
            Quaternion.identity
        );

        PoisonZone zoneScript = zone.GetComponent<PoisonZone>();
        if (zoneScript != null)
        {
            zoneScript.Initialize(zoneDamage, damageInterval, damageTime, warningTime, fadeOutTime);
        }


    }

    protected override void Attack()
    {
        if (projectilePrefab == null || shootingPoint == null) return;

        isAttacking = true;
        attackFreezeTimer = attackFreezeDuration;
        rb.velocity = Vector2.zero;
        animator.SetTrigger("Attack");
        animator.SetBool("Moving_right", false);
        animator.SetBool("Moving_left", false);

        // Создаем снаряд
        GameObject projectile = Instantiate(projectilePrefab, shootingPoint.position, Quaternion.identity);
        // Игнорировать коллизию снаряда с пауком
        Physics2D.IgnoreCollision(
            projectile.GetComponent<Collider2D>(),
            GetComponent<Collider2D>()
        );  

        // Направление к игроку
        Vector2 direction = (player.position - shootingPoint.position).normalized;

        // Задаем скорость снаряду
        ProjectilePoison projectileScript = projectile.GetComponent<ProjectilePoison>();
        if (projectileScript != null)
        {
            projectileScript.Initialize(direction, projectileSpeed, damage);
        }


    }




}