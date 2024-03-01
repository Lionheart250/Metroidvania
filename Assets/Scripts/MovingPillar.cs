using UnityEngine;

public class MovingPillar : MonoBehaviour
{
    public Transform posA, posB;
    public float speed;
    public float decelerationDistance = 1.0f;
    public float startDelay = 1.0f; // Delay before starting movement
    public float tweenDuration = 1.0f; // Duration of the up and down tween
    private Vector3 targetPos;
    private bool isMoving = true; // Start moving by default
    private Rigidbody2D rb;

    private Vector3 moveDirection;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        LeanTween.delayedCall(startDelay, () =>
        {
            if (posB != null) // Check if posB is not null
            {
                targetPos = posB.position; // Start moving towards posB
                DirectionCalculate(); // Calculate direction once
                SmashUpDown();
            }
        });
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

    private void DirectionCalculate()
    {
        moveDirection = (targetPos - transform.position).normalized;
    }

    private void SmashUpDown()
    {
        LeanTween.moveY(gameObject, posA.position.y, tweenDuration)
            .setEase(LeanTweenType.easeInOutSine)
            .setLoopPingPong();
    }
}
