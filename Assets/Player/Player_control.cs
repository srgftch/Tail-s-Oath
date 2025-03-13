using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField]
    private bool reloadPos = false;

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
    private bool canActive = false; // ����������� ������ ��������
    private bool isInventoryOpen = false;

    [Header("��������� ��������")]
    private int maxHP = 100; // ������������ ��
    private int currentHP;  // ������� ��

    [SerializeField] private int attackDamage = 20; // Урон от атаки
    [SerializeField] private float attackRange = 1.5f; // Радиус атаки
    [SerializeField] private Transform attackPoint; // Точка, из которой идёт удар

    [SerializeField] private float attackCooldown = 1f; // Время между атаками
    private float nextAttackTime = 0f;

    [SerializeField] private LayerMask enemyLayer; // Слой врагов


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHP = maxHP;
        int isPosAval = PlayerPrefs.GetInt(PlayerConsts.RESTART_POS_AVALAIB);
        if (reloadPos && isPosAval == 1)
        {
            gameObject.transform.position = loadRestartPos();
        }
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



        if (Time.time >= nextAttackTime)
        {
            if (Input.GetKeyDown(KeyCode.Space)) // Атака по нажатию клавиши
            {
                Attack();
                nextAttackTime = Time.time + attackCooldown;
            }
        }

    }

    private void Attack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.TryGetComponent(out EnemySpiderHP enemyHealth))
            {
                enemyHealth.TakeDamage(attackDamage);
                Debug.Log($"Игрок ударил врага на {attackDamage} урона!");
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
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
            ActionUI.SetActive(true);
            canActive = true;
        }
        if (collision.gameObject.tag.Equals("Door"))
        {
            collision.gameObject.GetComponent<InteractiveObject>().action();
        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("interactive"))
        {
            canActive = false;
            ActionUI.SetActive(false);
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
    private Vector2 loadRestartPos()
    {
        return new Vector2(
            PlayerPrefs.GetFloat(PlayerConsts.X_RESTART_POS),
            PlayerPrefs.GetFloat(PlayerConsts.Y_RESTART_POS)
            );
    }
}
