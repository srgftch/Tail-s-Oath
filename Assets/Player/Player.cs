using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private float speed = 3f;
    private Vector2 move;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        rb.velocity = move * speed;
    }
    [ContextMenu("Move")]
    public void Move(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }
}