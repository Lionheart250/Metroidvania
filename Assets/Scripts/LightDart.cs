using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightDart : MonoBehaviour
{
    [SerializeField] float damage;
    [SerializeField] float hitForce;
    [SerializeField] int speed;
    [SerializeField] float lifetime = 1;

    private bool shouldMove = true;
    public bool canDetach = false;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, lifetime);
    }
    void Update()
    {
        if (canDetach)
        {
            DetachFromEnemy();
        }
    }

    private void FixedUpdate()
    {
        if (shouldMove)
        {
            transform.position += speed * transform.right;
        }
        if (attachedEnemy != null)
        {
            // Update the position of the LightDart to match the attached enemy
            transform.position = attachedEnemy.transform.position + offsetFromEnemy;
        }
    }

    // Detect hit
    private Enemy attachedEnemy; // Reference to the enemy the LightDart is attached to
    private Vector3 offsetFromEnemy;
    public bool canDamage = true;
    public bool canStick = true;

    private void OnTriggerEnter2D(Collider2D _other)
    {
         if (_other.CompareTag("Enemy"))
        {
            if (canDamage)
            {
                ApplyDamage(_other);
            }
            if (canStick)
            {
                StickToEnemy(_other);
            }
        }
        else if (_other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            shouldMove = false;

            // Disable movement (e.g., Rigidbody2D or any other movement component)
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = Vector2.zero;
                rb.gravityScale = 0;
            }
        }
    }

    private void ApplyDamage(Collider2D _other)
    {
        Enemy enemy = _other.GetComponent<Enemy>();

            enemy.EnemyGetsHit(damage, (_other.transform.position - transform.position).normalized, hitForce);
        
    }

    private void StickToEnemy(Collider2D _other)
    {
        shouldMove = false;
        attachedEnemy = _other.GetComponent<Enemy>();

        // Calculate the offset between the LightDart and the enemy's position
        offsetFromEnemy = transform.position - _other.transform.position;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = Vector2.zero;
                rb.gravityScale = 0;
            }

        
    }

    public void DetachFromEnemy()
    {
        attachedEnemy = null;
    }
}
