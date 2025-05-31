using UnityEngine;
using System.Collections;

public class PoisonZone : MonoBehaviour
{
    [Header("Timing Settings")]
    private float warningTime;    // ���� ��������������
    private float damageTime;     // ���� ��������� �����
    private float fadeOutTime;  // ���� ������������

    [Header("Damage Settings")]
    private int damage = 5;
    private float damageInterval = 1f;

    private bool isActive;
    private float damageTimer;
    private CircleCollider2D zoneCollider;
    protected Animator animator;

    public void Initialize(int dmg, float intvl, float dur, float warning, float disabling)
    {
        animator = GetComponent<Animator>();
        damage = dmg;
        damageInterval = intvl;
        damageTime = dur;
        warningTime = warning;
        fadeOutTime = disabling;

        StartCoroutine(ZoneLifecycle());
    }

    private void Awake()
    {
        zoneCollider = GetComponent<CircleCollider2D>();
        zoneCollider.enabled = false;

    }

    private IEnumerator ZoneLifecycle()
    {
        // === ���� 1: �������������� ===
        yield return new WaitForSeconds(warningTime);

        // === ���� 2: �������� ���� ===
        zoneCollider.enabled = true;
        isActive = true;
        animator.SetBool("Is_active", true);

        float activeTimer = 0;
        while (activeTimer < damageTime)
        {
            activeTimer += Time.deltaTime;
            yield return null;
        }

        // === ���� 3: ������������ ===
        isActive = false;
        animator.SetBool("Is_active", false);
        zoneCollider.enabled = false;

        yield return new WaitForSeconds(fadeOutTime);
        Destroy(gameObject);
    }

    private void Update()
    {
        if (!isActive) return;

        damageTimer += Time.deltaTime;
        if (damageTimer >= damageInterval)
        {
            damageTimer = 0;
            ApplyDamage();
        }
    }

    private void ApplyDamage()
    {
        animator.SetTrigger("Attack");
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position,
            zoneCollider.radius * transform.localScale.x);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("player"))
            {
                hit.GetComponent<PHealth>()?.TakeDamage(damage);
                Debug.Log($"������� ���� �� ����");
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position,
            GetComponent<CircleCollider2D>().radius * transform.localScale.x);
    }
}