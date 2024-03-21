using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mirror : MonoBehaviour
{
    public enum ShootDirection { Up, Down, Left, Right, UpLeft, UpRight, DownLeft, DownRight }

    public float shootForce = 10f; // Force to shoot the player
    public ShootDirection direction = ShootDirection.Up; // Default shoot direction is up

    private Coroutine shootingCoroutine; // Reference to the shooting coroutine

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (shootingCoroutine != null)
            {
                // If a coroutine is already running, stop it
                StopCoroutine(shootingCoroutine);
            }

            // Start the new coroutine with a small delay to allow the current one to stop
            StartCoroutine(StartNewCoroutine(other.attachedRigidbody));
        }
    }

    private IEnumerator StartNewCoroutine(Rigidbody2D playerRigidbody)
    {
        yield return new WaitForSeconds(0.1f); // Small delay to allow current coroutine to stop

        // Start the shooting coroutine immediately
        shootingCoroutine = StartCoroutine(ShootAfterDelay(playerRigidbody));
    }

    private IEnumerator ShootAfterDelay(Rigidbody2D playerRigidbody)
    {
        PlayerController.Instance.pState.lightningBody = true;

        playerRigidbody.velocity = Vector2.zero; // Stop the player's movement
        playerRigidbody.gravityScale = 0;

        yield return new WaitForSeconds(0.5f); // Your delay before shooting the player

        Vector2 shootVector = Vector2.zero;

        // Set shoot vector based on direction setting
        switch (direction)
        {
            case ShootDirection.Up:
                shootVector = Vector2.up;
                break;
            case ShootDirection.Down:
                shootVector = Vector2.down;
                break;
            case ShootDirection.Left:
                shootVector = Vector2.left;
                break;
            case ShootDirection.Right:
                shootVector = Vector2.right;
                break;
            case ShootDirection.UpLeft:
                // Calculate shoot vector for UpLeft (45-degree angle between up and left)
                shootVector = (Vector2.up + Vector2.left).normalized;
                break;
            case ShootDirection.UpRight:
                // Calculate shoot vector for UpRight (45-degree angle between up and right)
                shootVector = (Vector2.up + Vector2.right).normalized;
                break;
            case ShootDirection.DownLeft:
                // Calculate shoot vector for DownLeft (45-degree angle between down and left)
                shootVector = (Vector2.down + Vector2.left).normalized;
                break;
            case ShootDirection.DownRight:
                // Calculate shoot vector for DownRight (45-degree angle between down and right)
                shootVector = (Vector2.down + Vector2.right).normalized;
                break;
        }

        // Shoot the player in the specified direction
        playerRigidbody.velocity += shootVector * shootForce;

        float endTime = Time.time + 0.5f; // Your launch time

        // Allow the player to steer left and right until launch time ends or grounded
        while (Time.time < endTime && !PlayerController.Instance.Grounded())
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            playerRigidbody.velocity += Vector2.right * horizontalInput * shootForce * Time.deltaTime;

            yield return null;
        }

        playerRigidbody.velocity = Vector2.zero;
        PlayerController.Instance.pState.lightningBody = false;
        playerRigidbody.gravityScale = PlayerController.Instance.gravity;
    }
}