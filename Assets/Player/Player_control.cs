using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] private float startSpeed = 5f;
    private float currentSpeed;
    private float currentDebuf = 0f;
    private int hitCount = 0;
    private bool isStunned = false;
    private float debufTimer = 0f;
    private float stunTimer = 0f;
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
        currentSpeed = startSpeed;
    }

    void Update()
    {
        if (canMove)
        {
            if (isRoll==false)
            {
                rb.velocity = move * currentSpeed;
            }
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
        if (debufTimer > 0)
        {
            debufTimer -= Time.deltaTime;
            if (debufTimer <= 0)
            {
                ResetDebuf();
            }
        }

        if (isStunned && stunTimer > 0)
        {
            stunTimer -= Time.deltaTime;
            if (stunTimer <= 0)
            {
                EndStun();
            }
        }
    }
    public void ApplySpiderHit() //попадание снарядом паука
    {
        // Сбрасываем таймер при каждом новом попадании
        debufTimer = 13f;

        if (isStunned)
        {
            // Если уже в стане - просто обновляем таймер
            stunTimer = 3f;
            return;
        }

        hitCount++;

        if (hitCount >= 5)
        {
            StartStun();
            return;
        }

        float debufAmount = hitCount * 0.2f;
        ChangeSpeed(debufAmount);
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

    public void ChangeSpeed(float debuf)
    {
        currentDebuf = Mathf.Clamp(debuf, 0f, 0.8f);
        currentSpeed = startSpeed * (1 - currentDebuf);
        Debug.Log($"Текущая скорость: {currentSpeed} (дебаф: {currentDebuf * 100}%)");
    }
    private void StartStun()
    {
        isStunned = true;
        stunTimer = 3f;
        currentSpeed = 0;
        Debug.Log("Игрок оглушен на 3 секунды!");

        // Запускаем анимацию стана
        // animator.SetBool("IsStunned", true);
    }

    private void EndStun()
    {
        isStunned = false;
        hitCount = 2; // Возвращаем на 2 уровень дебафа
        ChangeSpeed(0.4f);
        Debug.Log("Стан закончился, восстановлена скорость до 60%");

        // animator.SetBool("IsStunned", false);
    }

    private void ResetDebuf()
    {
        hitCount = 0;
        currentDebuf = 0f;
        currentSpeed = startSpeed;
        Debug.Log("Дебафф скорости полностью сброшен");
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
