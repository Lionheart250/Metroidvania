using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class ShadowHook : MonoBehaviour
{
    public float moveSpeed = 25f;  // Adjust this speed as needed
    public float maxRadius = 5f;  // Maximum distance from the player
    public string horizontalAxis = "Horizontal";  // Gamepad horizontal axis
    public string verticalAxis = "Vertical";      // Gamepad vertical axis
    [Header("Scripts:")]
    public GrappleRope grappleRope;
    [Header("Layer Settings:")]
    [SerializeField] private LayerMask grappableLayerMask;

    [Header("Transform Refrences:")]
    public Transform gunHolder;
    public Transform gunPivot;
    public Transform firePoint;

    [Header("Rotation:")]
    [SerializeField] private bool rotateOverTime = true;
    [Range(0, 80)] [SerializeField] private float rotationSpeed = 4;

    [Header("Distance:")]
    [SerializeField] private bool hasMaxDistance = true;
    [SerializeField] private float maxDistance = 4;

    [Header("Launching")]
    [SerializeField] private bool launchToPoint = true;
    [SerializeField] private LaunchType Launch_Type = LaunchType.Transform_Launch;
    [Range(0, 500)] [SerializeField] private float launchSpeed = 500;

    [Header("No Launch To Point")]
    [SerializeField] private bool autoCongifureDistance = false;
    [SerializeField] private float targetDistance = 3;
    [SerializeField] private float targetFrequency = 3;

    [HideInInspector] public PlayerStateList pState;
    [HideInInspector] public PlayerController playerController;
    private bool obstacleInBetween;

    private enum LaunchType
    {
        Transform_Launch,
        Physics_Launch,
    }

    [Header("Component Refrences:")]
    [HideInInspector] public Vector2 grapplePoint;
    [HideInInspector] public Vector2 DistanceVector;
    Vector3 aimingDirection;
    public SpringJoint2D m_springJoint2D;
    public Rigidbody2D playerRigidbody;

    // Update is called once per frame
    void Start()
    {
        grappleRope.enabled = false;
        m_springJoint2D.enabled = false;
        pState = GetComponentInParent<PlayerStateList>();
        playerController = GetComponentInParent<PlayerController>();
        
    }
    public float lerpSpeed = 5f;
    private float currentDistance;
    private bool coroutining = false;
    void Update()
    {
        
        SetGrappleColor();
        
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Calculate aiming direction
        Vector3 targetAimingDirection = new Vector3(horizontalInput, verticalInput, 0f).normalized;

        aimingDirection = Vector3.Lerp(aimingDirection, targetAimingDirection, moveSpeed * Time.deltaTime);

        // Calculate new distance based on input and speed
        currentDistance += moveSpeed * Time.deltaTime;

        // Clamp currentDistance to ensure it doesn't exceed maxDistance
        currentDistance = Mathf.Clamp(currentDistance, 0f, maxDistance);

        // Calculate aim position
        Vector3 targetPosition = firePoint.position + aimingDirection * maxDistance;
        Vector3 targetLinePosition = firePoint.position + targetAimingDirection * maxDistance * moveSpeed * Time.deltaTime;

        if (Input.GetButtonDown("Shield") || (Gamepad.current?.leftShoulder.wasPressedThisFrame == true) && !coroutining)
        {
            SetGrapplePoint();
            
            
            if (grapplePoint != Vector2.zero)
            {
            grappleRope.enabled = true;
            pState.shadowHooking = true;
            playerController.anim.SetBool("Casting", true);
            StartCoroutine(LaunchToTarget(grapplePoint, launchSpeed));
            }
        }
        else
        {
            //grapplePoint = Vector2.zero;
            //RotateGun(aimingDirection, true);
            //grappleRope.enabled = false;
            m_springJoint2D.enabled = false;
            //playerRigidbody.gravityScale = playerController.gravity;
            playerController.anim.SetBool("Casting", false);
            if(playerController.Grounded())
            {
                pState.shadowHooking = false;
            }
        }
    }


    void RotateGun(Vector3 lookPoint, bool allowRotationOverTime)
    {
        Vector3 distanceVector = lookPoint - gunPivot.position;

        float angle = Mathf.Atan2(distanceVector.y, distanceVector.x) * Mathf.Rad2Deg;
        if (rotateOverTime && allowRotationOverTime)
        {
            Quaternion startRotation = gunPivot.rotation;
            gunPivot.rotation = Quaternion.Lerp(startRotation, Quaternion.AngleAxis(angle, Vector3.forward), Time.deltaTime * rotationSpeed);
        }
        else
            gunPivot.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

    }

    private GameObject grappledObject;
    private GameObject lastGrappledObject;
    private GameObject currentGrapplePoint;

    void SetGrapplePoint()
{
    // Define layers to ignore
    int defaultLayer = LayerMask.NameToLayer("Default");
    int backgroundLayer = LayerMask.NameToLayer("Background");
    int midgroundLayer = LayerMask.NameToLayer("Midground");
    int foregroundLayer = LayerMask.NameToLayer("Foreground");
    int grappableLayer = LayerMask.NameToLayer("Grappable");

    // Create a layer mask that excludes the specified layers
    int layerMask = ~(1 << defaultLayer | 1 << backgroundLayer | 1 << foregroundLayer);
    int obstacleLayerMask = ~(1 << defaultLayer | 1 << backgroundLayer | 1 << foregroundLayer | 1 << grappableLayer | 1 << midgroundLayer);

    // Adjust the size of the sphere based on your needs
    float sphereRadius = maxDistance;

    RaycastHit2D[] allHits = Physics2D.CircleCastAll(firePoint.position, sphereRadius, Vector2.zero, 0, layerMask);

    // Sort hits by distance from the player
    Array.Sort(allHits, (a, b) => Vector2.Distance(a.point, firePoint.position).CompareTo(Vector2.Distance(b.point, firePoint.position)));

    bool foundGrapplePoint = false;
    int grappleableCount = 0;
    RaycastHit2D closestGrappleableHit = new RaycastHit2D();

    foreach (var hit in allHits)
    {
        if (hit.collider != null && hit.collider.gameObject.layer == grappableLayer && ((Vector2.Distance(hit.point, firePoint.position) <= maxDistance) || !hasMaxDistance))
        {
            // Check if there is anything blocking between player and grapple point
            RaycastHit2D obstruction = Physics2D.Linecast(firePoint.position, hit.collider.gameObject.transform.position, obstacleLayerMask);
            Debug.DrawLine(firePoint.position, hit.collider.gameObject.transform.position, Color.red, 1f); // Draw a red line from firePoint to hit.point
            if (obstruction.collider == null)
            {
                // Check if the grapple point is in the direction the player is facing
                Vector2 directionToGrapplePoint = (hit.collider.gameObject.transform.position - transform.position).normalized;
                if ((directionToGrapplePoint.x > 0 && pState.lookingRight) || (directionToGrapplePoint.x < 0 && !pState.lookingRight))
                {
                    grappleableCount++;
                    closestGrappleableHit = hit;

                    grappledObject = hit.collider.gameObject;
                    grapplePoint = hit.collider.gameObject.transform.position;
                    DistanceVector = grapplePoint - (Vector2)gunPivot.position;
                    grappleRope.enabled = true;

                    // Print the layer of the hit object
                    Debug.Log("Hit object on layer: " + LayerMask.LayerToName(hit.collider.gameObject.layer));

                    // Update the last grappled object
                    lastGrappledObject = grappledObject;

                    // Change the color of the current grapple point
                    if (currentGrapplePoint != null)
                    {
                        //StartCoroutine(LerpColor(currentGrapplePoint.GetComponent<SpriteRenderer>(), Color.white, colorChangeSpeed));
                    }
                    currentGrapplePoint = grappledObject;
                    //StartCoroutine(LerpColor(currentGrapplePoint.GetComponent<SpriteRenderer>(), Color.green, colorChangeSpeed));

                    foundGrapplePoint = true;
                    break;
                }
            }
        }
    }

    if (grappleableCount == 1 && !foundGrapplePoint)
    {
        // Only one grappleable object found and it wasn't selected, so select it now
        grappledObject = closestGrappleableHit.collider.gameObject;
        grapplePoint = closestGrappleableHit.collider.gameObject.transform.position;
        DistanceVector = grapplePoint - (Vector2)gunPivot.position;
        grappleRope.enabled = true;

        // Print the layer of the hit object
        Debug.Log("Hit object on layer: " + LayerMask.LayerToName(closestGrappleableHit.collider.gameObject.layer));

        // Update the last grappled object to null
        lastGrappledObject = null;

        // Change the color of the current grapple point
        if (currentGrapplePoint != null)
        {
            //StartCoroutine(LerpColor(currentGrapplePoint.GetComponent<SpriteRenderer>(), Color.white, colorChangeSpeed));
        }
        currentGrapplePoint = grappledObject;
        //StartCoroutine(LerpColor(currentGrapplePoint.GetComponent<SpriteRenderer>(), Color.green, colorChangeSpeed));
    }

    if (grapplePoint == Vector2.zero)
    {
        Debug.Log("No collider hit on Grappable layer or distance too far or obstruction present. Setting grapplePoint to Vector2.zero.");
        
    }
}












    private IEnumerator LaunchToTarget(Vector3 targetPosition, float launchSpeed)
    {
        coroutining = true;
        playerRigidbody.velocity = Vector2.zero;
        yield return new WaitForSeconds(0.2f);
        Vector2 launchDirection = (targetPosition - gunHolder.position).normalized;
        float startTime = Time.time;
        float maxDuration = Vector2.Distance(gunHolder.position, targetPosition) / launchSpeed;

        while (Time.time - startTime < maxDuration)
        {
            playerRigidbody.velocity = launchDirection * launchSpeed;
            yield return null;
        }

        // Ensure the player reaches the exact target position
        gunHolder.position = targetPosition;
        if(pState.shadowHooking)
        {
            playerRigidbody.velocity += new Vector2(launchDirection.x * launchSpeed * 0.1f, launchDirection.y * launchSpeed * 0.1f);
            
        }

        playerRigidbody.gravityScale = playerController.gravity;
        grappleRope.enabled = false;
        yield return new WaitForSeconds(0.5f);
        pState.shadowHooking = false;
        grappleRope.enabled = false;
        coroutining = false;
        m_springJoint2D.enabled = false;
        grapplePoint = Vector2.zero;
        playerRigidbody.gravityScale = playerController.gravity;
        //yield return new WaitForSeconds(1.5f);
    }




    public void Grapple()
    {

        if (!launchToPoint && !autoCongifureDistance)
        {
            m_springJoint2D.distance = targetDistance;
            m_springJoint2D.frequency = targetFrequency;
        }

        if (!launchToPoint)
        {
            if (autoCongifureDistance)
            {
                m_springJoint2D.autoConfigureDistance = true;
                m_springJoint2D.frequency = 0;
            }
            m_springJoint2D.connectedAnchor = grapplePoint;
            m_springJoint2D.enabled = true;
        }

        else
        {
            if (Launch_Type == LaunchType.Transform_Launch)
            {
                playerRigidbody.gravityScale = 0;
                playerRigidbody.velocity = Vector2.zero;
            }
            if (Launch_Type == LaunchType.Physics_Launch)
            {
                m_springJoint2D.connectedAnchor = grapplePoint;
                m_springJoint2D.distance = 0;
                m_springJoint2D.frequency = launchSpeed;
                m_springJoint2D.enabled = true;
                playerRigidbody.gravityScale = 0;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (hasMaxDistance)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(firePoint.position, maxDistance);
        }
    }
    float colorChangeSpeed = 8f; // Speed of the color change
    void SetGrappleColor()
{
    // Define layers to ignore
    int defaultLayer = LayerMask.NameToLayer("Default");
    int backgroundLayer = LayerMask.NameToLayer("Background");
    int midgroundLayer = LayerMask.NameToLayer("Midground");
    int foregroundLayer = LayerMask.NameToLayer("Foreground");
    int grappableLayer = LayerMask.NameToLayer("Grappable");

    // Create a layer mask that excludes the specified layers
    int layerMask = ~(1 << defaultLayer | 1 << backgroundLayer | 1 << foregroundLayer);
    int obstacleLayerMask = ~(1 << defaultLayer | 1 << backgroundLayer | 1 << foregroundLayer | 1 << grappableLayer | 1 << midgroundLayer);

    // Adjust the size of the sphere based on your needs
    float colorSphereRadius = maxDistance;

    RaycastHit2D[] allHits = Physics2D.CircleCastAll(firePoint.position, colorSphereRadius, Vector2.zero, 0, layerMask);

    // Sort hits by distance from the player
    Array.Sort(allHits, (a, b) => Vector2.Distance(a.point, firePoint.position).CompareTo(Vector2.Distance(b.point, firePoint.position)));

    // Check if there is only one grappleable object in range and it wasn't already selected
    int grappableCount = 0;
    GameObject grappleableObj = null;
    foreach (var hit in allHits)
    {
        if (hit.collider != null && hit.collider.gameObject.layer == grappableLayer && ((Vector2.Distance(hit.point, firePoint.position) <= maxDistance) || !hasMaxDistance))
        {
            // Check if there is anything blocking between player and grapple point
            RaycastHit2D obstruction = Physics2D.Linecast(firePoint.position, hit.collider.gameObject.transform.position, obstacleLayerMask);
            if (obstruction.collider == null)
            {
                // Check if the grapple point is in the direction the player is facing
                Vector2 directionToGrapplePoint = (hit.collider.gameObject.transform.position - transform.position).normalized;
                if ((directionToGrapplePoint.x > 0 && pState.lookingRight) || (directionToGrapplePoint.x < 0 && !pState.lookingRight))
                {
                    grappleableObj = hit.collider.gameObject;
                    grappableCount++;
                    break; // Exit the loop after finding the first valid grapple point
                }
                else
                {
                    // Reset the color of the grapple point since it's not in front of the player
                    StartCoroutine(LerpColorWhite(hit.collider.gameObject.GetComponent<SpriteRenderer>(), Color.white, colorChangeSpeed));
                }
            }
        }
    }

    if (grappableCount == 1)
    {
        // Reset color of the previous grapple point if it's not the same as the new grapple point
        if (currentGrapplePoint != null && currentGrapplePoint != grappleableObj)
        {
            StartCoroutine(LerpColorWhite(currentGrapplePoint.GetComponent<SpriteRenderer>(), Color.white, colorChangeSpeed));
        }

        // Change the color of the new grapple point
        currentGrapplePoint = grappleableObj;
        StartCoroutine(LerpColorGreen(currentGrapplePoint.GetComponent<SpriteRenderer>(), Color.green, colorChangeSpeed));

        // Reset the last grappled object if it's not within range
        if (lastGrappledObject != null && Vector2.Distance(lastGrappledObject.transform.position, firePoint.position) > maxDistance)
        {
            lastGrappledObject = null;
        }
    }
    else
    {
        // Continue with the existing logic for multiple grapple points
        foreach (var hit in allHits)
        {
            if (hit.collider != null && hit.collider.gameObject.layer == grappableLayer && ((Vector2.Distance(hit.point, firePoint.position) <= maxDistance) || !hasMaxDistance))
            {
                // Check if there is anything blocking between player and grapple point
                RaycastHit2D obstruction = Physics2D.Linecast(firePoint.position, hit.collider.gameObject.transform.position, obstacleLayerMask);
                if (obstruction.collider == null)
                {
                    // Check if the hit object is different from the last grappled object
                    if (lastGrappledObject == null || hit.collider.gameObject != lastGrappledObject)
                    {
                        // Reset color of the previous grapple point if it's not the same as the new grapple point
                        if (currentGrapplePoint != null && currentGrapplePoint != hit.collider.gameObject)
                        {
                            StartCoroutine(LerpColorWhite(currentGrapplePoint.GetComponent<SpriteRenderer>(), Color.white, colorChangeSpeed));
                        }

                        // Change the color of the new grapple point
                        currentGrapplePoint = hit.collider.gameObject;
                        StartCoroutine(LerpColorGreen(currentGrapplePoint.GetComponent<SpriteRenderer>(), Color.green, colorChangeSpeed));

                        // Check if the new object is closer than the current grappled object
                        if (lastGrappledObject != null && Vector2.Distance(hit.collider.gameObject.transform.position, firePoint.position) <
                            Vector2.Distance(lastGrappledObject.transform.position, firePoint.position))
                        {
                            // Set the new object as the last grappled object
                            lastGrappledObject = hit.collider.gameObject;
                        }

                        break;
                    }
                }
            }
        }
    }

    // Check if the player is not facing any grappleable object
    if (grappleableObj == null)
    {
        // Reset color of the current grapple point if there is one
        if (currentGrapplePoint != null)
        {
            StartCoroutine(LerpColorWhite(currentGrapplePoint.GetComponent<SpriteRenderer>(), Color.white, colorChangeSpeed));
            currentGrapplePoint = null;
        }
    }
}







    IEnumerator LerpColorGreen(SpriteRenderer spriteRenderer, Color targetColor, float speed)
    {
        float t = 0f;
        Color startColor = spriteRenderer.color;

        while (t < 1f)
        {
            t += Time.deltaTime * speed;
            spriteRenderer.color = Color.Lerp(startColor, targetColor, Mathf.Clamp01(t));
            yield return null;
        }

        spriteRenderer.color = targetColor;
    }

    IEnumerator LerpColorWhite(SpriteRenderer spriteRenderer, Color targetColor, float speed)
    {
        float t = 0f;
        Color startColor = spriteRenderer.color;

        while (t < 1f)
        {
            t += Time.deltaTime * speed;
            spriteRenderer.color = Color.Lerp(startColor, targetColor, Mathf.Clamp01(t));
            yield return null;
        }

        spriteRenderer.color = targetColor;
    }
    



}
