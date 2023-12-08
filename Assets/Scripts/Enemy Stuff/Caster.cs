using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Caster : Enemy
{    
    [SerializeField] private float flipWaitTime;
    [SerializeField] private float ledgeCheckX;
    [SerializeField] private float ledgeCheckY;
    [SerializeField] private LayerMask whatIsGround;
    

    [SerializeField] private GameObject enemyFireBallPrefab;

    [SerializeField] private Transform firePoint;
    [SerializeField] private float shootCooldown = 2f;
    [SerializeField] private float detectionRange = 60f;

    private float shootTimer = 0f;

    
    float timer;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        rb.gravityScale = 12f;
            ChangeState(EnemyStates.Caster_Idle);

    }
    protected override void Update()
{
    base.Update();

    if (!PlayerController.Instance.pState.alive)
    {
        ChangeState(EnemyStates.Caster_Idle);
    }

    // Check if it's time to shoot and if the player is in front
    if (shootTimer >= shootCooldown && PlayerInFront())
    {
        Debug.Log("Shooting Fireball!");
        ShootEnemyFireBall();
        // Reset the shoot timer
        shootTimer = 0f;
    }

    // Update the shoot timer
    shootTimer += Time.deltaTime;
}

    private void OnCollisionEnter2D(Collision2D _collision)
    {
        if(_collision.gameObject.CompareTag("Enemy"))
        {
            ChangeState(EnemyStates.Caster_Flip);
        }
    }

     private void ShootEnemyFireBall()
{
    // Instantiate the enemy fireball at the fire point
    GameObject enemyFireBall = Instantiate(enemyFireBallPrefab, firePoint.position, Quaternion.identity);

    // Set the speed of the enemy fireball (assuming the EnemyFireBall script has a 'speed' variable)
    EnemyFireBall enemyFireBallScript = enemyFireBall.GetComponent<EnemyFireBall>();
    if (enemyFireBallScript != null)
    {
        // Set the speed of the enemy fireball
        // enemyFireBallScript.speed = 0; // Adjust the speed as needed

        // Check the direction the enemy is facing and adjust fireball's rotation and scale accordingly
        if (transform.localScale.x < 0)
        {
            // Not facing right, rotate the fireball 180 degrees
            enemyFireBall.transform.eulerAngles = new Vector3(enemyFireBall.transform.eulerAngles.x, 180);

            // Optionally, flip the fireball's sprite
            Vector3 fireballScale = enemyFireBall.transform.localScale;
            fireballScale.x *= 1;
            enemyFireBall.transform.localScale = fireballScale;
        }
        else
        {
            // Facing right, set rotation to zero
            enemyFireBall.transform.eulerAngles = Vector3.zero;
        }
    }
}




    protected override void UpdateEnemyStates()
{
    if (health <= 0)
    {
        Death(0.05f);
    }

    Vector3 _ledgeCheckStart = transform.localScale.x > 0 ? new Vector3(ledgeCheckX, 0) : new Vector3(-ledgeCheckX, 0);
    Vector2 _wallCheckDir = transform.localScale.x > 0 ? transform.right : -transform.right;

    //Debug.DrawRay(transform.position + _ledgeCheckStart, Vector2.down * ledgeCheckY, Color.red);
    //Debug.DrawRay(transform.position, _wallCheckDir * ledgeCheckX, Color.blue);


    

    //Debug.DrawRay(transform.position + _detectionRangeStart, Vector2.down * ledgeCheckY, Color.blue);
    //Debug.DrawRay(transform.position, _detectionRangeDir * detectionRange, Color.blue);
    switch (GetCurrentEnemyState)
    {
        case EnemyStates.Caster_Idle:
            if (!Physics2D.Raycast(transform.position + _ledgeCheckStart, Vector2.down, ledgeCheckY, whatIsGround)
                || Physics2D.Raycast(transform.position, _wallCheckDir, ledgeCheckX, whatIsGround))
            {
                ChangeState(EnemyStates.Caster_Flip);
            }

            if (transform.localScale.x > 0)
            {
                rb.velocity = new Vector2(speed, rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2(-speed, rb.velocity.y);
            }
            break;

        case EnemyStates.Caster_Flip:
            timer += Time.deltaTime;

            if (timer > flipWaitTime)
            {
                timer = 0;
                transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
                ChangeState(EnemyStates.Caster_Idle);
            }
            break;
    }
}
void OnDrawGizmos()
{
    // Determine the position of the circle center based on the enemy's scale
    Vector3 circleCenter = transform.position + new Vector3(transform.localScale.x > 0 ? detectionRange : -detectionRange, 0f);

    // Draw a wire sphere using Gizmos (visualize the circle)
    Gizmos.color = Color.blue;
    Gizmos.DrawWireSphere(circleCenter, detectionRange);
}
private bool PlayerInFront()
{
    // Determine the position of the circle center based on the enemy's scale
    Vector3 circleCenter = transform.position + new Vector3(transform.localScale.x > 0 ? detectionRange : -detectionRange, 0f);

    // Perform a circle overlap check
    Collider2D[] colliders = Physics2D.OverlapCircleAll(circleCenter, detectionRange);

    // Debug logs for visualization (optional, for debugging purposes)
    Debug.DrawRay(transform.position, Vector2.right * detectionRange, Color.green); // Visualize the detection range
    Debug.DrawLine(transform.position, circleCenter, Color.red); // Visualize the circle center


    // Check if the "Player" tag is present in the overlapped colliders
    foreach (Collider2D collider in colliders)
    {
        if (collider.CompareTag("Player"))
        {
            Debug.Log("Player detected!");
            return true;
        }
    }

    return false;
}

}