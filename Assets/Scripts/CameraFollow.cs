using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public float smoothSpeed = 0.125f;
    public float angle = 45f; // Angle in degrees

    private void LateUpdate()
    {
        if (target != null)
        {
            // Calculate the desired position with the fixed angle
            Vector3 angleOffset = Quaternion.Euler(angle, 0f, 0f) * offset;
            Vector3 desiredPosition = target.position + angleOffset;

            // Lerp between the current position and the desired position
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;

            // Look at the target
            transform.LookAt(target);
        }
    }
}
