using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public GameObject inventoryPanel; // Ссылка на панель инвентаря

    void Update()
    {
        // Проверяем нажатие кнопки (например, "I" для открытия/закрытия инвентаря)
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
    }

    void ToggleInventory()
    {
        // Переключаем видимость инвентаря
        inventoryPanel.SetActive(!inventoryPanel.activeSelf);
    }
}