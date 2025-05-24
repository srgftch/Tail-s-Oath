using UnityEngine;
using System.Collections;

public class PoisonZone : MonoBehaviour
{
    [Header("���������� ����������")]
    [SerializeField] private SpriteRenderer warningCircle;
    [SerializeField] private SpriteRenderer poisonEffect;
    [SerializeField] private Animator animator;

    private int damage;
    private float interval;
    private float duration;
    private float warningDuration;
    private bool isActive;

    public void Initialize(int dmg, float intvl, float dur, float warning)
    {
        damage = dmg;
        interval = intvl;
        duration = dur;
        warningDuration = warning;

        StartCoroutine(ZoneLifecycle());
    }

    private IEnumerator ZoneLifecycle()
    {
        // ���� ��������������
        warningCircle.enabled = true;
        poisonEffect.enabled = false;
        yield return new WaitForSeconds(warningDuration);

        // ��������� ����
        warningCircle.enabled = false;
        poisonEffect.enabled = true;
        //animator?.SetTrigger("Activate");
        isActive = true;

        // ���� ��������� �����
        float activeTime = 0;
        while (activeTime < duration)
        {
            activeTime += interval;
            yield return new WaitForSeconds(interval);
        }

        // �����������
        isActive = false;
        //animator?.SetTrigger("Deactivate");
        yield return new WaitForSeconds(0.5f); // �������� ������������

        Destroy(gameObject);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!isActive || !other.CompareTag("player")) return;

        PlayerHealth player = other.GetComponent<PlayerHealth>();
        if (player != null)
        {
            player.TakeDamage(damage);
        }
    }
}