using System.Collections;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform posA, posB;
    public float speed;
    public float decelerationDistance = 1.0f;
    private Vector3 targetPos;
    private bool isMoving = false;
    private bool canTrigger = true;
    private Rigidbody2D rb;

    private PlayerController playerController;
    private Rigidbody2D playerRb;
    private Vector3 moveDirection;

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
                float decelerationFactor = distanceToTarget / decelerationDistance;
                rb.velocity = moveDirection * speed * decelerationFactor;
            }
            else
            {
                rb.velocity = moveDirection * speed;
            }

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
        if (isMoving)
        {
            // The actual movement is handled in Update
        }
    }

    private void DirectionCalculate()
    {
        moveDirection = (targetPos - transform.position).normalized;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && canTrigger && !isMoving)
        {
            isMoving = true;
            canTrigger = false;
            targetPos = (targetPos == posA.position) ? posB.position : posA.position;
            DirectionCalculate();

            UpdatePlayerState(true);
        }
        else
        {
            UpdatePlayerState(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isMoving)
        {
            isMoving = false;
            UpdatePlayerState(false);
        }
        else
        {
            UpdatePlayerState(false);
        }
    }

    private void UpdatePlayerState(bool isOnPlatform)
    {
        if (playerRb != null)
        {

            playerController.isOnPlatform = isOnPlatform;
            playerRb.gravityScale = isOnPlatform ? playerRb.gravityScale * 8 : playerRb.gravityScale / 8;
        }
    }
}

