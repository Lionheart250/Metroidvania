using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class BatCaster : Enemy
{
    [SerializeField] private float chaseDistance;
    [SerializeField] private float stunDuration;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float avoidanceForce = 5f; // Adjust this value for obstacle avoidance strength
    [SerializeField] private float maxSteeringForce = 1f; // Adjust this value for steering force
    [SerializeField] private float maxSpeed = 5f; 
    [SerializeField] private float obstacleAvoidanceRadius = 2f;
    [SerializeField] private float nextWaypointDistance = 1f; 
    [SerializeField] private Transform groundCheckTransform;
    [SerializeField] private Transform roofCheckTransform;
    [SerializeField] private Transform wallCheckTransform;
    [SerializeField] private Transform target;
    [SerializeField] private bool isChasing = false;
    Path path;
    int currentWaypoint = 0;
    bool reachedEndOfPath = false;
    Seeker seeker;
    [SerializeField] private float projectileCooldown = 2f;
    private float projectileTimer = 0f;
    public GameObject projectilePrefab;

    float timer;
    float obstacleAvoidanceRayLength = 1f;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        ChangeState(EnemyStates.Bat_Idle);
        seeker = GetComponent<Seeker>();

        InvokeRepeating("UpdatePath", 0f, .5f);

        PlayerController playerController = PlayerController.Instance;
        if (playerController != null)
    {
        target = playerController.transform;
    }
    else
    {
        Debug.LogError("PlayerController.Instance is null or the player controller script is missing.");
    }
    }

     void UpdatePath()
    {
        if (seeker.IsDone())
            seeker.StartPath(rb.position, target.position, OnPathComplete);
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }   
    }

    protected override void Update()
    {
        base.Update();
        if (!PlayerController.Instance.pState.alive)
        {
            ChangeState(EnemyStates.Bat_Idle);
        }
    }

    void FixedUpdate()
    {
        if(!isChasing)
        return;
        
        if (path == null)
        return;

        if (currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return;
        }
        else
        {
            reachedEndOfPath = false;
        }



        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }
    }

    

    protected override void UpdateEnemyStates()
    {
        float _dist = Vector2.Distance(transform.position, PlayerController.Instance.transform.position);

        switch (GetCurrentEnemyState)
        {
            case EnemyStates.Bat_Idle:
                if (_dist < chaseDistance)
                {
                    ChangeState(EnemyStates.Bat_Chase);
                }
                break;

            case EnemyStates.Bat_Chase:
                HandleChaseState();
                projectileTimer += Time.deltaTime;
                if (projectileTimer >= projectileCooldown)
                {
                    ShootProjectile();
                    projectileTimer = 0f;
                }
                break;

            case EnemyStates.Bat_Stunned:
                timer += Time.deltaTime;

                if (timer > stunDuration)
                {
                    ChangeState(EnemyStates.Bat_Idle);
                    timer = 0;
                }
                break;

            case EnemyStates.Bat_Death:
                Death(Random.Range(2, 3));
                break;

            default:
                isChasing = false;
                break;
        }
    }
    
    private void ShootProjectile()
{
    GameObject projectile = Instantiate(projectilePrefab, wallCheckTransform.position, Quaternion.identity);
    Vector2 directionToPlayer = (PlayerController.Instance.transform.position - transform.position).normalized;
    BatFireball batFireball = projectile.GetComponent<BatFireball>();
    
    if (batFireball != null)
    {
        batFireball.SetDirection(directionToPlayer);
    }
    else
    {
        Debug.LogError("BatFireball component not found on the projectile prefab!");
    }
}
   private void HandleChaseState()
{
    isChasing = true;
    float distanceOffset = 30.0f; // Adjust this value based on the desired distance
    float stoppingDistance = 1.0f; // Adjust this value based on how close you want the enemy to get

    // Toggle between distanceOffset and -distanceOffset based on the enemy's position relative to the player
    float offsetSign = (transform.position.x < PlayerController.Instance.transform.position.x) ? 1.0f : -1.0f;

    // Calculate the desired position above the player
    Vector2 desiredPosition = new Vector2(PlayerController.Instance.transform.position.x + offsetSign * distanceOffset, PlayerController.Instance.transform.position.y + distanceOffset);

    // Calculate the direction towards the desired position
    Vector2 chaseDirection = (desiredPosition - (Vector2)transform.position).normalized;
    Vector2 desiredVelocity = chaseDirection * maxSpeed;
    RaycastHit2D hit = Physics2D.CircleCast(transform.position, obstacleAvoidanceRadius, chaseDirection, obstacleAvoidanceRayLength, obstacleLayer);

    float distanceToPlayer = Vector2.Distance(transform.position, desiredPosition);

    Debug.DrawLine(transform.position, desiredPosition, Color.blue);

    if (distanceToPlayer > stoppingDistance)
    {
        // Continue moving towards the player
        // Calculate the desired velocity (direction towards player)

        // Calculate the steering force to reach the desired velocity
        Vector2 steeringForce = Vector2.ClampMagnitude(desiredVelocity - rb.velocity, maxSteeringForce);

        // Apply the steering force to the rigidbody
        rb.AddForce(steeringForce);

        // Limit the maximum speed
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxSpeed);

        FlipBat(); // You may need to adjust this method based on your game's specific requirements
    }
     if (hit.collider != null)
    {
        // If an obstacle is detected, adjust the desired velocity to avoid it
        Vector2 avoidanceDirection = Vector2.Perpendicular(hit.normal).normalized;

        // Calculate the perpendicular steering force to move away from the obstacle
        Vector2 perpendicularSteeringForce = avoidanceDirection * avoidanceForce;

        // Add the perpendicular steering force to the overall steering force
        desiredVelocity += perpendicularSteeringForce;

        // Check for ground, roof, and wall to fine-tune the steering
        bool nearGround = Physics2D.Linecast(transform.position, groundCheckTransform.position, obstacleLayer);
        bool nearRoof = Physics2D.Linecast(transform.position, roofCheckTransform.position, obstacleLayer);
        bool nearWall = Physics2D.Linecast(transform.position, wallCheckTransform.position, obstacleLayer);

    if (nearGround)
    {
    // Optionally, adjust the steering force when near the ground
    float groundAvoidanceAngle = Vector2.SignedAngle(Vector2.up, hit.normal);

    // Calculate the perpendicular avoidance direction
    Vector2 perpendicularAvoidanceDirection = Quaternion.Euler(0, 0, groundAvoidanceAngle + 90f) * Vector2.right;

    // Calculate the combined avoidance force (away from the ground and perpendicular)
    Vector2 groundAvoidanceForce = (chaseDirection + perpendicularAvoidanceDirection).normalized * avoidanceForce;

    // Calculate the additional force to move left or right based on player's x-position
    float horizontalOffset = Mathf.Clamp(PlayerController.Instance.transform.position.x - transform.position.x, -1f, 1f);
    float horizontalScalingFactor = 100f; // Adjust this value to increase or decrease the impact of horizontal movement
    Vector2 horizontalAvoidanceForce = Vector2.right * horizontalOffset * horizontalScalingFactor * avoidanceForce;

    desiredVelocity += groundAvoidanceForce + horizontalAvoidanceForce;

    }

    if (nearRoof)
    {
    // Optionally, adjust the steering force when near the roof
    float roofAvoidanceAngle = Vector2.SignedAngle(Vector2.down, hit.normal);

    // Calculate the perpendicular avoidance direction
    Vector2 perpendicularAvoidanceDirection = Quaternion.Euler(0, 0, roofAvoidanceAngle + 90f) * Vector2.right;

    // Calculate the combined avoidance force (away from the roof and perpendicular)
    Vector2 roofAvoidanceForce = (chaseDirection + perpendicularAvoidanceDirection).normalized * avoidanceForce;

    // Calculate the additional force to move left or right based on player's x-position
    float horizontalOffset = Mathf.Clamp(PlayerController.Instance.transform.position.x - transform.position.x, -1f, 1f);
    float horizontalScalingFactor = 100f; // Adjust this value to increase or decrease the impact of horizontal movement
    Vector2 horizontalAvoidanceForce = Vector2.right * horizontalOffset * horizontalScalingFactor * avoidanceForce;

    desiredVelocity += roofAvoidanceForce + horizontalAvoidanceForce;

    }

    if (nearWall)
    {
    // Optionally, adjust the steering force when near a wall
    float wallAvoidanceAngle = Vector2.SignedAngle(chaseDirection, hit.normal);

    // Calculate the perpendicular avoidance direction
    Vector2 perpendicularAvoidanceDirection = Quaternion.Euler(0, 0, wallAvoidanceAngle + 90f) * Vector2.right;

    // Calculate the combined avoidance force (away from the wall and perpendicular)
    Vector2 wallAvoidanceForce = (chaseDirection + perpendicularAvoidanceDirection).normalized * avoidanceForce;

    // Calculate the additional force to move up or down based on player's y-position
    float verticalOffset = Mathf.Clamp(PlayerController.Instance.transform.position.y - transform.position.y, -1f, 1f);
    float verticalScalingFactor = 5f; // Adjust this value to increase or decrease the impact of vertical movement
    Vector2 verticalAvoidanceForce = Vector2.up * verticalOffset * verticalScalingFactor * avoidanceForce;

    desiredVelocity += wallAvoidanceForce + verticalAvoidanceForce;

    }
}
}


private void OnDrawGizmosSelected()
    {
        // Draw the CircleCast in the Scene view
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, obstacleAvoidanceRadius);
    }



    public override void EnemyGetsHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        base.EnemyGetsHit(_damageDone, _hitDirection, _hitForce);

        if (health > 0)
        {
            ChangeState(EnemyStates.Bat_Stunned);
        }
        else
        {
            ChangeState(EnemyStates.Bat_Death);
        }
    }

    protected override void Death(float _destroyTime)
    {
        rb.gravityScale = 12;
        base.Death(_destroyTime);
    }

    protected override void ChangeCurrentAnimation()
    {
        anim.SetBool("Idle", GetCurrentEnemyState == EnemyStates.Bat_Idle);

        anim.SetBool("Chase", GetCurrentEnemyState == EnemyStates.Bat_Chase);

        anim.SetBool("Stunned", GetCurrentEnemyState == EnemyStates.Bat_Stunned);

        if (GetCurrentEnemyState == EnemyStates.Bat_Death)
        {
            anim.SetTrigger("Death");
        }
    }

  void FlipBat()
{
    sr.flipX = PlayerController.Instance.transform.position.x < transform.position.x;

    // Flip the wallCheckTransform along with the bat
    Vector3 wallCheckScale = wallCheckTransform.localScale;
    wallCheckScale.x = Mathf.Abs(wallCheckScale.x) * (sr.flipX ? -1 : 1);
    wallCheckTransform.localScale = wallCheckScale;

    // Move the wallCheckTransform along the x-axis when flipping
    Vector3 wallCheckPosition = wallCheckTransform.localPosition;
    wallCheckPosition.x = (sr.flipX ? -1 : 1) * 10; // Adjust the position accordingly
    wallCheckTransform.localPosition = wallCheckPosition;
}
}
