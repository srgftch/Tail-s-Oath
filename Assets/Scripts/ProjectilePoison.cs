using UnityEngine;

public class ProjectilePoison : MonoBehaviour
{
    // Вложенный класс для обработки отложенного урона
    private class PoisonDamageHandler : MonoBehaviour
    {
        private PlayerHealth targetPlayer;
        private int damage;

        public void Initialize(PlayerHealth player, float projectileSpeed, int delayedDamage)
        {
            targetPlayer = player;
            damage = delayedDamage;
            Invoke("ApplyDelayedDamage", 1f); // Таймер на 1 секунду
        }

        private void ApplyDelayedDamage()
        {
            if (targetPlayer != null)
            {
                targetPlayer.TakeDamage(damage);
                Debug.Log($"Нанесён отложенный урон: {damage} единиц");
            }
            Destroy(gameObject); // Уничтожаем обработчик после выполнения
        }
    }

    [Header("Damage Settings")]
    private int damage;
    [SerializeField] private int delayedDamage = 5;
    [SerializeField] private float delayTime = 1f;

    private Vector2 direction;
    private float speed;

    public void Initialize(Vector2 shootDirection, float projectileSpeed, int projectileDamage)
    {
        direction = shootDirection;
        speed = projectileSpeed;
        damage = projectileDamage;

        // Поворот снаряда в направлении движения
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
        if (!collision.CompareTag("player")) return;

        PlayerHealth player = collision.GetComponent<PlayerHealth>();
        if (player == null) return;

        // 1. Наносим мгновенный урон
        player.TakeDamage(damage);
        Debug.Log($"Нанесён мгновенный урон: {damage} единиц");

        // 2. Создаем обработчик для отложенного урона
        CreateDamageHandler(player);

        // 3. Уничтожаем снаряд (но обработчик продолжит работу)
        DestroyProjectile();
    }

    private void CreateDamageHandler(PlayerHealth player)
    {
        GameObject handlerObject = new GameObject("PoisonDamageHandler");
        PoisonDamageHandler handler = handlerObject.AddComponent<PoisonDamageHandler>();
        handler.Initialize(player, speed, damage-5);
    }

    private void DestroyProjectile()
    {
        // Отключаем визуал и коллайдер
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;

        // Уничтожаем объект через небольшой запас времени
        Destroy(gameObject, 0.1f);
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}