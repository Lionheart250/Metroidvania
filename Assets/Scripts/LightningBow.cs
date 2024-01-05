using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningBow : MonoBehaviour
{
    public float moveSpeed = 25f;  // Adjust this speed as needed
    public float maxRadius = 5f;  // Maximum distance from the player
    public string horizontalAxis = "Horizontal";  // Gamepad horizontal axis
    public string verticalAxis = "Vertical";      // Gamepad vertical axis

    // Update is called once per frame
    void Update()
    {
        // Get the current position and rotation of the LightningBow
        Vector2 lightningBowPosition = transform.position;
        Quaternion lightningBowRotation = transform.rotation;

        // Get input from the gamepad joystick
        float horizontalInput = Input.GetAxis(horizontalAxis);
        float verticalInput = Input.GetAxis(verticalAxis);

        // Calculate the move direction
        Vector2 moveDirection = new Vector2(horizontalInput, verticalInput).normalized;

        // Debugging: Output the move direction vector
        Debug.DrawRay(lightningBowPosition, moveDirection * 10f, Color.green);

        // Calculate the target position within the circular range
        Vector2 targetPosition = lightningBowPosition + moveDirection * moveSpeed * Time.deltaTime;

        // Clamp the target position to stay within the circular range
        targetPosition = Vector2.ClampMagnitude(targetPosition - (Vector2)transform.parent.position, maxRadius) + (Vector2)transform.parent.position;

        // Calculate the rotation angle in degrees
        float angle = Mathf.Atan2(targetPosition.y - lightningBowPosition.y, targetPosition.x - lightningBowPosition.x) * Mathf.Rad2Deg;

        // Set the rotation based on the move direction
        lightningBowRotation = Quaternion.Euler(0f, 0f, angle);

        // Set the position based on the move direction and speed
        lightningBowPosition = targetPosition;

        // Update the position and rotation of the LightningBow
        transform.position = lightningBowPosition;
        transform.rotation = lightningBowRotation;

        // Additional Debugging: Output the current rotation
        Debug.Log("Rotation: " + angle);
    }
}
