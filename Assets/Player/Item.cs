using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Атрибут CreateAssetMenu позволяет создавать предметы через меню Unity
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string itemName = "New Item"; // Название предмета
    public Sprite icon = null;         // Иконка предмета
    public bool isStackable = false;   // Можно ли стакать предмет
    public int maxStack = 1;           // Максимальное количество в стаке
    public virtual void Use()
    {

    }
}
