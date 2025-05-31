using UnityEngine;

public class EnemySpiderAlchemist: Enemy
{
    [Header("Shooting Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform shootingPoint;
    [SerializeField] private float projectileSpeed = 7f;

    [Header("�������� ����")]
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

        // ������� ���� �� ������� ������� ������
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

        // ������� ������
        GameObject projectile = Instantiate(projectilePrefab, shootingPoint.position, Quaternion.identity);
        // ������������ �������� ������� � ������
        Physics2D.IgnoreCollision(
            projectile.GetComponent<Collider2D>(),
            GetComponent<Collider2D>()
        );  

        // ����������� � ������
        Vector2 direction = (player.position - shootingPoint.position).normalized;

        // ������ �������� �������
        ProjectilePoison projectileScript = projectile.GetComponent<ProjectilePoison>();
        if (projectileScript != null)
        {
            projectileScript.Initialize(direction, projectileSpeed, damage);
        }


    }

    protected override void Die()
    {
        if (player.TryGetComponent(out PExp playerScript))
        {
            playerScript.AddExperience(20);
        }
        base.Die();
    }


}