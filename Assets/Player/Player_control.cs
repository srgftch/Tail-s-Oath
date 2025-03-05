using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float speed = 5f;
    private bool canMove = true; // ���� ����������� ��������

    private Rigidbody2D rb;
    private Vector2 move;
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private GameObject inventoryUI; // ������ ���������
    [SerializeField]
    private GameObject ActionUI; // ������� ����������� ��������
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

    [ContextMenu("PlayerInventory")]
    public void Inventory(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!isInventoryOpen) // ��������� ���������
            {
                StartCoroutine(ShowInventoryAfterAnimation());
            }
            else // ��������� ���������
            {
                StartCoroutine(HideInventoryBeforeAnimation());
            }

            canMove = false; // ��������� �������� �� ����� ��������
        }
    }

    // ������� �������� ����� ���������� ���������
    private IEnumerator ShowInventoryAfterAnimation()
    {
        animator.SetBool("isInventoryOpen", true); // ��������� �������� ��������

        // ���� ���� ����, ����� Animator ������� ���������
        yield return null;

        // ����, ���� �������� �����������
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        inventoryUI.SetActive(true); // �������� UI ����� ���������� ��������
        isInventoryOpen = true;
    }

    // ��������� UI ����� ������������� ��������
    private IEnumerator HideInventoryBeforeAnimation()
    {
        inventoryUI.SetActive(false); // ������� ��������� UI
        animator.SetBool("isInventoryOpen", false); // ��������� �������� ��������
        // ���� ���� ����, ����� Animator ������� ���������
        yield return null;

        // ����, ���� �������� �����������
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        isInventoryOpen = false;
        canMove = true; // ����� �������� ��������� ��������
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag.Equals("interactive"))
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
