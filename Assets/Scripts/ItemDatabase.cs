using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public Item[] items; // Массив всех предметов

    // Метод для получения предмета по ID
    public Item GetItem(int id)
    {
        return items[id];
    }

    // Метод для получения предмета по имени
    public Item GetItem(string itemName)
    {
        foreach (Item item in items)
        {
            if (item.itemName == itemName)
                return item;
        }
        return null;
    }
}