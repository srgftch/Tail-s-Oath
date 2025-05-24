using UnityEngine;

public class EnemySpiderShooter : Enemy
{
    [Header("Shooting Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform shootingPoint;
    [SerializeField] private float projectileSpeed = 7f;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Attack()
    {
        if (projectilePrefab == null || shootingPoint == null) return;

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
        Projectile projectileScript = projectile.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            projectileScript.Initialize(direction, projectileSpeed, damage);
        }

        // Анимация атаки (если есть)
        // animator?.SetTrigger("Attack");
    }


}