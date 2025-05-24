using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemySpider_warrior: Enemy
{
    protected override void Start()
    {
        base.Start();
    }
    void Update()
    {
        base.Update();
    }

    protected override void Attack()
    {
        base.Attack(); // ���� ����� ��������� ������� ���������

        if (player != null && player.TryGetComponent(out PlayerHealth playerScript))
        {
           // playerScript.TakeDamage(damage);
            Debug.Log($"����-���� �������� ������ �� {damage} �����!");
        }
    }
    protected override void Die()
    {
        // ��������� ����������� ��������� ��� ������ �����
        Debug.Log("����-���� ����!");
        base.Die();
    }
}
