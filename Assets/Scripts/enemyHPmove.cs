using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class enemyHPmove : MonoBehaviour
{
    public Transform enemy;  // Ссылка на объект врага
    public Vector3 offset;   // Смещение над врагом
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;  // Получаем камеру
    }

    void Update()
    {
        if (enemy != null)
        {
            // Обновляем позицию полоски
            transform.position = enemy.position + offset;
            // Разворачиваем её к камере
            transform.rotation = mainCamera.transform.rotation;
        }
    }
}
