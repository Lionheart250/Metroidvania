using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform posA, posB;
    public float speed;
    private Vector3 targetPos;
    private bool isMoving = false;  // Flag to track if the platform is currently moving
    private bool canTrigger = true;  // Flag to control trigger responsiveness
    Rigidbody2D rb;

    PlayerController playerController;
    Rigidbody2D playerRb;
    Vector3 moveDirection;

    private void Awake()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
        playerRb = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        targetPos = transform.position;  // Start at the current position
    }

    private void Update()
    {
        // Check if the platform has reached the target position
        if (isMoving && Vector2.Distance(transform.position, targetPos) < 0.05f)
        {
            isMoving = false;
            canTrigger = true;  // Allow triggering again
            rb.velocity = Vector2.zero;  // Stop the platform
            transform.position = targetPos; // this line is supposed to avoid floating away
        }
        if (!isMoving)
        {
            canTrigger = true;
        }
    }

    private void FixedUpdate()
    {
        // Move the platform only if it is currently set to move
        if (isMoving)
        {
            rb.velocity = moveDirection * speed;
        }
    }

    void DirectionCalculate()
    {
        moveDirection = (targetPos - transform.position).normalized;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && canTrigger && !isMoving)
        {
            // Trigger movement when player enters
            isMoving = true;
            canTrigger = false;  // Prevent further triggering until the platform stops
            targetPos = (targetPos == posA.position) ? posB.position : posA.position;  // Move to the opposite position
            DirectionCalculate();

            playerController.isOnPlatform = true;
            playerController.platformRb = rb;
            playerRb.gravityScale = playerRb.gravityScale * 4;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isMoving)
        {
            // Trigger stopping when player exits
            isMoving = false;

            playerController.isOnPlatform = false;
            playerRb.gravityScale = playerRb.gravityScale / 4;
        }
    }
}
