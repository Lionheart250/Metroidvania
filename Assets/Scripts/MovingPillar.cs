using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPillar : MonoBehaviour
{
    public Transform posA, posB;
    public float speed;
    public float decelerationDistance = 1.0f;  
    private Vector3 targetPos;
    private bool isMoving = true; // Start moving by default
    Rigidbody2D rb;

    Vector3 moveDirection;

    private void Start()
    {
        targetPos = posB.position; // Start moving towards posB
        rb = GetComponent<Rigidbody2D>();
        DirectionCalculate(); // Calculate direction once
    }

    private void Update()
    {
        if (isMoving)
        {
            float distanceToTarget = Vector2.Distance(transform.position, targetPos);

            if (distanceToTarget < decelerationDistance)
            {
                float decelerationFactor = distanceToTarget / decelerationDistance;
                rb.velocity = moveDirection * speed * decelerationFactor;
            }
            else
            {
                rb.velocity = moveDirection * speed;
            }

            if (distanceToTarget < 0.05f)
            {
                // Switch target position when reaching one of the positions
                targetPos = (targetPos == posA.position) ? posB.position : posA.position;
                DirectionCalculate(); // Recalculate direction for the new target
            }
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
}
