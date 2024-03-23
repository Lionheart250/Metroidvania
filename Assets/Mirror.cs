using System.Collections;
using UnityEngine;

public class Mirror : MonoBehaviour
{
    public enum ShootDirection { Up, Down, Left, Right, UpLeft, UpRight, DownLeft, DownRight }

    public float shootForce = 10f; // Force to shoot the player
    public ShootDirection direction = ShootDirection.Up; // Default shoot direction is up
    public float delay = 0.5f; // Delay before shooting the player
    public float launchTime = 0.5f;
    private float launchTimer = 0f;


    // // Flag to indicate if player is inside a mirror trigger
    private Coroutine shootingCoroutine; // Reference to the shooting coroutine

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !PlayerController.Instance.pState.shadowForm)
        {
            PlayerController.Instance.insideMirrorTrigger = true;
            Debug.Log("Player entered the mirror trigger.");
            StartShootingCoroutine(PlayerController.Instance.rb);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !PlayerController.Instance.pState.shadowForm)
        {
            PlayerController.Instance.insideMirrorTrigger = false;
            Debug.Log("Player exited the mirror trigger.");
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && shootingCoroutine == null && PlayerController.Instance.insideMirrorTrigger && !PlayerController.Instance.pState.shadowForm)
        {
            Debug.Log("Player is inside the mirror trigger and shooting coroutine is null.");
            StartShootingCoroutine(PlayerController.Instance.rb);
        }
    }

    private void StartShootingCoroutine(Rigidbody2D playerRigidbody)
    {
        PlayerController.Instance.IncrementActiveMirrorCoroutines();
        shootingCoroutine = StartCoroutine(ShootAfterDelay(playerRigidbody));
    }

    private IEnumerator ShootAfterDelay(Rigidbody2D playerRigidbody)
    {
        
        launchTimer = 0f;
        PlayerController.Instance.pState.lightningBody = true;
        playerRigidbody.velocity = Vector2.zero; // Stop the player's movement
        playerRigidbody.gravityScale = 0;
        Vector2 startPosition = playerRigidbody.transform.position;
        Vector2 endPosition = transform.position;
        float lerpTime = 0.1f; // Time taken to reach the end position
        float elapsedTime = 0f;

        while (elapsedTime < lerpTime)
        {
            float t = elapsedTime / lerpTime;
            playerRigidbody.transform.position = Vector2.Lerp(startPosition, endPosition, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        playerRigidbody.transform.position = endPosition;

        yield return new WaitForSeconds(delay - lerpTime);

        //yield return new WaitForSeconds(delay);

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
                shootVector = (Vector2.up + Vector2.left).normalized;
                break;
            case ShootDirection.UpRight:
                shootVector = (Vector2.up + Vector2.right).normalized;
                break;
            case ShootDirection.DownLeft:
                shootVector = (Vector2.down + Vector2.left).normalized;
                break;
            case ShootDirection.DownRight:
                shootVector = (Vector2.down + Vector2.right).normalized;
                break;
        }

        // Shoot the player in the specified direction
        playerRigidbody.velocity += shootVector * shootForce;

        

        // Allow the player to steer left and right until launch time ends or grounded
        while (launchTimer < launchTime && !PlayerController.Instance.Grounded())
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            playerRigidbody.velocity += Vector2.right * horizontalInput * shootForce * Time.deltaTime;
            playerRigidbody.velocity = new Vector2(
                Mathf.Clamp(playerRigidbody.velocity.x, -shootForce, shootForce),
                playerRigidbody.velocity.y);

            launchTimer += Time.deltaTime;

            yield return null;
        }

        launchTimer = 0f;
        // Reset player velocity and properties
        ///playerRigidbody.velocity = Vector2.zero;
        PlayerController.Instance.DecrementActiveMirrorCoroutines();
        shootingCoroutine = null;
        
        if (!PlayerController.Instance.insideMirrorTrigger && shootingCoroutine == null && PlayerController.Instance.GetActiveMirrorCoroutinesCount() == 0)
        {
        playerRigidbody.gravityScale = PlayerController.Instance.gravity;
        PlayerController.Instance.pState.lightningBody = false;
        }
    }
}
