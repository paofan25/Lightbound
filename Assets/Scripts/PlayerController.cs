using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3.0f;

    float horizontalInput;
    
    float verticalInput;

    Rigidbody2D rigid;

    Animator animator;

    Vector2 faceDirect = new Vector2(0, -1);  //д╛хооРоб
    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        if (null == rigid) Debug.LogError("Get rigidbody2d component failed!");

        animator = GetComponentInChildren<Animator>();
        if(null == animator) Debug.LogError("Get animator component failed!");
    }

    private void Update()
    {
        GetPlayerInput();
        AnimatorController();
    }

    private void FixedUpdate()
    {
        PlayerMovement();
    }

    void GetPlayerInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (verticalInput != 0)
        {
            horizontalInput = 0;
            faceDirect = new Vector2(0, verticalInput);
        }
        else
        {
            verticalInput = 0;
            faceDirect = new Vector2(horizontalInput, 0);
        }
    }

    void PlayerMovement()
    {
        rigid.velocity = new Vector2(horizontalInput * moveSpeed, verticalInput * moveSpeed);
    }

    void AnimatorController()
    {
        animator.SetInteger("horizontal", (int)faceDirect.x);
        animator.SetInteger("vertical", (int)faceDirect.y);
    }
}
