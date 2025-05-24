using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector2 direction;
    private float speed;
    private int damage;

    public void Initialize(Vector2 shootDirection, float projectileSpeed, int projectileDamage)
    {
        direction = shootDirection;
        speed = projectileSpeed;
        damage = projectileDamage;

        // Поворачиваем снаряд в направлении движения
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void Update()
    {
        // Движение снаряда
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }

            // Применяем эффект замедления
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                player.ApplySpiderHit();

            }
            Destroy(gameObject);
        }
    }

    private void OnBecameInvisible()
    {
        // Уничтожаем снаряд, если он вышел за границы камеры
        Destroy(gameObject);
    }
}
