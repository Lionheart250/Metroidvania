using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform posA, posB;
    public float speed;
    public float decelerationDistance = 1.0f;  // Adjust this value based on your requirements
    private Vector3 targetPos;
    private bool isMoving = false;
    private bool canTrigger = true;
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
        targetPos = transform.position;
    }

    private void Update()
    {
        if (isMoving)
        {
            float distanceToTarget = Vector2.Distance(transform.position, targetPos);

            if (distanceToTarget < decelerationDistance)
            {
                // Calculate deceleration factor based on the remaining distance
                float decelerationFactor = distanceToTarget / decelerationDistance;

                // Slow down the platform as it nears the target
                rb.velocity = moveDirection * speed * decelerationFactor;
            }
            else
            {
                // Move the platform at normal speed
                rb.velocity = moveDirection * speed;
            }

            // Check if the platform has reached the target position
            if (distanceToTarget < 0.05f)
            {
                isMoving = false;
                canTrigger = true;
                rb.velocity = Vector2.zero;
                transform.position = targetPos;
            }
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
            // The actual movement is handled in Update
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
            canTrigger = false;
            targetPos = (targetPos == posA.position) ? posB.position : posA.position;
            DirectionCalculate();

            playerController.isOnPlatform = true;
            playerController.platformRb = rb;
            playerRb.gravityScale = playerRb.gravityScale * 8;
        }
        else
        {
            playerController.isOnPlatform = true;
            playerRb.gravityScale = playerRb.gravityScale * 8;

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {   
        
        if (collision.CompareTag("Player") && !isMoving)
        {
            // Trigger stopping when player exits
            isMoving = false;

            playerController.isOnPlatform = false;
            playerRb.gravityScale = playerRb.gravityScale / 8;
        }
        else
        {
            playerController.isOnPlatform = false;
            playerRb.gravityScale = playerRb.gravityScale / 8;
        }
    }
}
