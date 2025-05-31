using UnityEngine;

public class ProjectilePoison : MonoBehaviour
{
    // ��������� ����� ��� ��������� ����������� �����
    private class PoisonDamageHandler : MonoBehaviour
    {
        private PHealth targetPlayer;
        private int damage;

        public void Initialize(PHealth player, float projectileSpeed, int delayedDamage)
        {
            targetPlayer = player;
            damage = delayedDamage;
            Invoke("ApplyDelayedDamage", 1f); // ������ �� 1 �������
        }

        private void ApplyDelayedDamage()
        {
            if (targetPlayer != null)
            {
                targetPlayer.TakeDamage(damage);
                Debug.Log($"������ ���������� ����: {damage} ������");
            }
            Destroy(gameObject); // ���������� ���������� ����� ����������
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

        // ������� ������� � ����������� ��������
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void Update()
    {
        // �������� �������
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("player")) return;

        PHealth player = collision.GetComponent<PHealth>();
        if (player == null) return;

        // 1. ������� ���������� ����
        player.TakeDamage(damage);
        Debug.Log($"������ ���������� ����: {damage} ������");

        // 2. ������� ���������� ��� ����������� �����
        CreateDamageHandler(player);

        // 3. ���������� ������ (�� ���������� ��������� ������)
        DestroyProjectile();
    }

    private void CreateDamageHandler(PHealth player)
    {
        GameObject handlerObject = new GameObject("PoisonDamageHandler");
        PoisonDamageHandler handler = handlerObject.AddComponent<PoisonDamageHandler>();
        handler.Initialize(player, speed, damage-5);
    }

    private void DestroyProjectile()
    {
        // ��������� ������ � ���������
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;

        // ���������� ������ ����� ��������� ����� �������
        Destroy(gameObject, 0.1f);
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}