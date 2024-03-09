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

    public LineRenderer aimingLine;
    private bool shadowHooking = false;

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
    public Rigidbody2D ballRigidbody;

    // Update is called once per frame
    void Start()
    {
        grappleRope.enabled = false;
        m_springJoint2D.enabled = false;
        pState = GetComponentInParent<PlayerStateList>();
        playerController = GetComponentInParent<PlayerController>();

        // Initialize the LineRenderer component
        aimingLine.positionCount = 2;
        aimingLine.enabled = false;
        
    }
    public float lerpSpeed = 5f;
    private float currentDistance;
    private bool isLerpingGunHolder = false;
    void Update()
    {
        
        
        
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

        if (Input.GetButtonDown("Shield") || (Gamepad.current?.rightTrigger.wasPressedThisFrame == true))
        {
            SetGrapplePoint();
            shadowHooking = true;
            aimingLine.enabled = false;

        }
        else if (Input.GetButton("Shield") || (Gamepad.current?.rightTrigger.isPressed == true) && shadowHooking)
        {
            if (grappleRope.enabled)
            {
                RotateGun(grapplePoint, false);
                aimingLine.enabled = true;
            }
            else
            {
                RotateGun(aimingDirection, false);
                aimingLine.enabled = true;
            }

            if (launchToPoint && grappleRope.isGrappling)
            {
                if (Launch_Type == LaunchType.Transform_Launch)
                {
                    StartCoroutine(LerpGunHolder(grapplePoint, launchSpeed));
                }
            }

        }
        else
        {
            //RotateGun(aimingDirection, true);
            grappleRope.enabled = false;
            m_springJoint2D.enabled = false;
            ballRigidbody.gravityScale = 20;
            //grapplePoint = Vector2.zero;
            shadowHooking = false;

                aimingLine.enabled = true;
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
        float sphereRadius = 100f;

        RaycastHit2D[] allHits = Physics2D.CircleCastAll(firePoint.position, sphereRadius, Vector2.zero, 0, layerMask);

        // Sort hits by distance from the player
        Array.Sort(allHits, (a, b) => Vector2.Distance(a.point, firePoint.position).CompareTo(Vector2.Distance(b.point, firePoint.position)));

        foreach (var hit in allHits)
        {
            if (hit.collider != null && hit.collider.gameObject.layer == grappableLayer && ((Vector2.Distance(hit.point, firePoint.position) <= maxDistance) || !hasMaxDistance))
            {
                // Check if there is anything blocking between player and grapple point
                RaycastHit2D obstruction = Physics2D.Linecast(firePoint.position, hit.collider.gameObject.transform.position, obstacleLayerMask);
                Debug.DrawLine(firePoint.position, hit.collider.gameObject.transform.position, Color.red, 1f); // Draw a red line from firePoint to hit.point
                if (obstruction.collider == null)
                {
                    // Check if the hit object is different from the last grappled object
                    if (lastGrappledObject == null || hit.collider.gameObject != lastGrappledObject)
                    {
                        grappledObject = hit.collider.gameObject;
                        grapplePoint = hit.collider.gameObject.transform.position;
                        DistanceVector = grapplePoint - (Vector2)gunPivot.position;
                        grappleRope.enabled = true;

                        // Print the layer of the hit object
                        Debug.Log("Hit object on layer: " + LayerMask.LayerToName(hit.collider.gameObject.layer));

                        // Update the last grappled object
                        lastGrappledObject = grappledObject;

                        // Exit the loop after finding a valid grapple point
                        break;
                    }
                }
            }
        }

        if (grapplePoint == Vector2.zero)
        {
            Debug.Log("No collider hit on Grappable layer or distance too far or obstruction present. Setting grapplePoint to Vector2.zero.");
        }
    }




    private IEnumerator LerpGunHolder(Vector3 targetPosition, float speed)
    {
        isLerpingGunHolder = true;

        float startTime = Time.time;
        Vector3 startPosition = gunHolder.position;
        float journeyLength = Vector3.Distance(startPosition, targetPosition);

        while (Vector3.Distance(gunHolder.position, targetPosition) > 0.01f)
        {
            float distCovered = (Time.time - startTime) * speed;
            float fractionOfJourney = distCovered / journeyLength;
            gunHolder.position = Vector3.Lerp(startPosition, targetPosition, fractionOfJourney);

            // Calculate the direction towards the target position
            Vector3 direction = (targetPosition - gunHolder.position).normalized;

            yield return null;
        }

        // Lerp finished, do something here
        isLerpingGunHolder = false;
        Debug.Log("Lerp Finished");

        // Perform the action here
        grappleRope.enabled = false;
        m_springJoint2D.enabled = false;
        ballRigidbody.gravityScale = 20;
        //grapplePoint = Vector2.zero;
        aimingLine.enabled = false;
        shadowHooking = false;
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
                ballRigidbody.gravityScale = 0;
                ballRigidbody.velocity = Vector2.zero;
            }
            if (Launch_Type == LaunchType.Physics_Launch)
            {
                m_springJoint2D.connectedAnchor = grapplePoint;
                m_springJoint2D.distance = 0;
                m_springJoint2D.frequency = launchSpeed;
                m_springJoint2D.enabled = true;
                ballRigidbody.gravityScale = 0;
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
}
