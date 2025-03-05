using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float speed = 5f;
    private bool canMove = true; // Флаг возможности движения

    private Rigidbody2D rb;
    private Vector2 move;
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private GameObject inventoryUI; // Панель инвентаря
    [SerializeField]
    private GameObject ActionUI; // Надпись возможности действия
    private bool isInventoryOpen = false;

    [Header("Параметры здоровья")]
    private int maxHP = 100; // Максимальное ХП
    private int currentHP;  // Текущее ХП

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHP = maxHP;
    }

    void Update()
    {
        if (canMove)
        {
            rb.velocity = move * speed;
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    [ContextMenu("Move")]
    public void Move(InputAction.CallbackContext context)
    {
        if (!canMove) return; // Блокируем движение при открытом инвентаре

        animator.SetBool("isMoving", true);
        if (context.canceled)
        {
            animator.SetBool("isMoving", false);
            animator.SetFloat("lastX", move.x);
            animator.SetFloat("lastY", move.y);
        }
        move = context.ReadValue<Vector2>();
        animator.SetFloat("inputX", move.x);
        animator.SetFloat("inputY", move.y);
    }

    [ContextMenu("PlayerInventory")]
    public void Inventory(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!isInventoryOpen) // Открываем инвентарь
            {
                StartCoroutine(ShowInventoryAfterAnimation());
            }
            else // Закрываем инвентарь
            {
                StartCoroutine(HideInventoryBeforeAnimation());
            }

            canMove = false; // Запрещаем движение во время анимации
        }
    }

    // Ожидаем анимацию перед включением инвентаря
    private IEnumerator ShowInventoryAfterAnimation()
    {
        animator.SetBool("isInventoryOpen", true); // Запускаем анимацию открытия

        // Ждем один кадр, чтобы Animator обновил состояние
        yield return null;

        // Ждем, пока анимация проиграется
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        inventoryUI.SetActive(true); // Включаем UI после завершения анимации
        isInventoryOpen = true;
    }

    // Закрываем UI перед проигрыванием анимации
    private IEnumerator HideInventoryBeforeAnimation()
    {
        inventoryUI.SetActive(false); // Сначала выключаем UI
        animator.SetBool("isInventoryOpen", false); // Запускаем анимацию закрытия
        // Ждем один кадр, чтобы Animator обновил состояние
        yield return null;

        // Ждем, пока анимация проиграется
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        isInventoryOpen = false;
        canMove = true; // После закрытия разрешаем движение
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag.Equals("interactive"))
        {
            collision.gameObject.GetComponent<InteractiveObject>().action();
        }
    }

    // Получение урона
    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP < 0) currentHP = 0;

        Debug.Log($"Игрок получил {damage} урона. HP: {currentHP}");

        if (currentHP == 0)
        {
            Die();
        }
    }
    public void Heal(int healAmount)
    {
        currentHP += healAmount;
        if (currentHP > maxHP) currentHP = maxHP;

        Debug.Log($"Игрок вылечился на {healAmount}. HP: {currentHP}");
    }
    
    private void Die()
    {
        Debug.Log("Игрок погиб!");
        currentHP = maxHP;
    }
}
