using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatFireball : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float detectionRadius = 5f; // Adjust this value for the detection radius

    private Transform player; // Reference to the player's transform
    private Vector2 direction;

    // Set the initial direction of the fireball
    public void SetDirection(Vector2 newDirection)
    {
        direction = newDirection.normalized;
        // Rotate the fireball to face the direction of movement
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    void Start()
    {
        // Find the player's transform using a tag (you can adjust this based on your setup)
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void FixedUpdate()
    {
        if (player != null)
        {
            // Perform a CircleCast to detect the player
            RaycastHit2D hit = Physics2D.CircleCast(transform.position, detectionRadius, direction, Mathf.Infinity, LayerMask.GetMask("Player"));

            // Visualize the CircleCast
            Debug.DrawRay(transform.position, direction * detectionRadius, Color.red);

            // If the player is detected, set the direction towards the player
            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                Vector2 directionToPlayer = (player.position - transform.position).normalized;
                SetDirection(directionToPlayer);
            }

            // Move the fireball in the specified direction
            transform.position += (Vector3)direction * speed * Time.fixedDeltaTime;
        }
        else
        {
            // Handle the case when the player is not found (optional)
            Destroy(gameObject);
        }
    }

    // You can add any additional logic for collisions or other interactions here

    void OnTriggerEnter2D(Collider2D other)
    {
        // Example: Destroy the fireball when it collides with an object
        if (other.CompareTag("Player"))
        {
            // You can add damage or any other logic here
            Destroy(gameObject);
        }
    }
}
