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
        Projectile projectileScript = projectile.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            projectileScript.Initialize(direction, projectileSpeed, damage);
        }

        // �������� ����� (���� ����)
        // animator?.SetTrigger("Attack");
    }


}