using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    public float parallaxMultiplier = 0.5f;
    public float maxOffsetX = 5f; // Maximum horizontal offset
    private Transform mainCameraTransform;
    private Vector3 lastCameraPosition;
    private Vector3 startPos;

    void Start()
    {
        mainCameraTransform = Camera.main.transform;
        lastCameraPosition = mainCameraTransform.position;
        startPos = transform.position;
    }

    void Update()
    {
        Vector3 deltaMovement = mainCameraTransform.position - lastCameraPosition;
        Vector3 parallaxDelta = new Vector3(deltaMovement.x * parallaxMultiplier, 0, 0);
        Vector3 newPos = transform.position + parallaxDelta;

        // Apply horizontal limits
        newPos.x = Mathf.Clamp(newPos.x, startPos.x - maxOffsetX, startPos.x + maxOffsetX);

        // Smoothly move the transform
        transform.position = Vector3.Lerp(transform.position, newPos, 0.3f);

        lastCameraPosition = mainCameraTransform.position;
    }
}
