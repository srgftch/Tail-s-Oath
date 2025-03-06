using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemySpider : MonoBehaviour
{
    [SerializeField]
    private float speed = 5f;
    private bool canMove = true; // ���� ����������� ��������

    private Rigidbody2D rb;
    private Vector2 move;
    [SerializeField]
    private Animator animator;

    private bool canActive = false; // ����������� ������ ��������
    private bool isInventoryOpen = false;

    [Header("��������� ��������")]
    private int maxHP = 100; // ������������ ��
    private int currentHP;  // ������� ��

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
        if (!canMove) return; // ��������� �������� ��� �������� ���������

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

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("interactive"))
        {
            canActive = true;
        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("interactive"))
        {
            canActive = false;
        }
    }

    public void Action(InputAction.CallbackContext context, Collider2D collision)
    {
        if (canActive == true && context.performed)
        {
            collision.gameObject.GetComponent<InteractiveObject>().action();
        }
    }

    // ��������� �����
    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP < 0) currentHP = 0;

        Debug.Log($"����� ������� {damage} �����. HP: {currentHP}");

        if (currentHP == 0)
        {
            Die();
        }
    }
    public void Heal(int healAmount)
    {
        currentHP += healAmount;
        if (currentHP > maxHP) currentHP = maxHP;

        Debug.Log($"����� ��������� �� {healAmount}. HP: {currentHP}");
    }

    private void Die()
    {
        Debug.Log("����� �����!");
        currentHP = maxHP;
    }
}
