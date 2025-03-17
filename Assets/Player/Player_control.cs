using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    public bool canMove = true; // проверка делает ли персонаж другие действия

    [SerializeField] private bool reloadPos = false;

    private Rigidbody2D rb;
    private Vector2 move;

    [SerializeField] private Animator animator;

    [SerializeField] private GameObject inventoryUI; 
    [SerializeField] private GameObject ActionUI; 

    private bool canActive = false; // проверка на активный объект рядом
    private bool isInventoryOpen = false; // проверка на открытый инвентарь

    [SerializeField] private float rollCooldown = 1f;
    private float nextRollTime = 0f;
    [SerializeField] private float rollForce = 7f; 

    private bool isRoll = false; // проверка на нахождение в рывке

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        int isPosAval = PlayerPrefs.GetInt(PlayerConsts.RESTART_POS_AVALAIB);
        if (reloadPos && isPosAval == 1)
        {
            gameObject.transform.position = LoadRestartPos();
        }
    }

    void Update()
    {
        if (canMove)
        {
            if (isRoll==false)
            {
                rb.velocity = move * speed;
            }
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }
    private Vector2 LoadRestartPos()
    {
        return new Vector2(
            PlayerPrefs.GetFloat(PlayerConsts.X_RESTART_POS),
            PlayerPrefs.GetFloat(PlayerConsts.Y_RESTART_POS)
            );
    }

    [ContextMenu("Move")]
    public void Move(InputAction.CallbackContext context)
    {
        if (!canMove) return; 

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
        if (move.sqrMagnitude > 0) // Если есть движение
        {
            animator.SetFloat("lastX", move.x);
            animator.SetFloat("lastY", move.y);
        }
    }

    public void RollInput(InputAction.CallbackContext context) // инициализация рывка
    {
        if (context.performed && Time.time >= nextRollTime)
        {
            Roll();
            nextRollTime = Time.time + rollCooldown;
        }
    }

    private void Roll() // рывок
    {
        if (!canMove) return;

        isRoll = true;

        animator.SetTrigger("Roll");

        Vector2 rollDirection = new Vector2(
            Mathf.RoundToInt(animator.GetFloat("lastX")),
            Mathf.RoundToInt(animator.GetFloat("lastY"))
        ).normalized;

        rb.AddForce(rollDirection * rollForce, ForceMode2D.Impulse);

        StartCoroutine(EndRoll());
    }

    private IEnumerator EndRoll()
    {
        yield return new WaitForSeconds(0.3f); // Длительность рывка
        isRoll = false;
    }


    [ContextMenu("PlayerInventory")]
    public void Inventory(InputAction.CallbackContext context) // открытие инвентаря
    {
        if (context.performed)
        {
            if (!isInventoryOpen) 
            {
                StartCoroutine(ShowInventoryAfterAnimation());
            }
            else
            {
                StartCoroutine(HideInventoryBeforeAnimation());
            }

            canMove = false; 
        }
    }


    private IEnumerator ShowInventoryAfterAnimation()
    {
        move.x = 0;
        move.y = 0;
        animator.SetBool("isMoving", false); 
        animator.SetBool("isInventoryOpen", true); 


        yield return null;


        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        inventoryUI.SetActive(true); 
        isInventoryOpen = true;
    }


    private IEnumerator HideInventoryBeforeAnimation()
    {
        inventoryUI.SetActive(false); 
        animator.SetBool("isInventoryOpen", false); 

        yield return null;


        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        isInventoryOpen = false;
        canMove = true; 
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
}
