using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class enemyHPmove : MonoBehaviour
{
    public Transform enemy;  // ������ �� ������ �����
    public Vector3 offset;   // �������� ��� ������
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;  // �������� ������
    }

    void Update()
    {
        if (enemy != null)
        {
            // ��������� ������� �������
            transform.position = enemy.position + offset;
            // ������������� � � ������
            transform.rotation = mainCamera.transform.rotation;
        }
    }
}
