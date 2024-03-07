using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mirror : MonoBehaviour
{
    public float shootForce = 10f; // Force to shoot the player

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Calculate the direction from the mirror to the player
            Vector2 direction = (other.transform.position - transform.position).normalized;

            // Shoot the player in the opposite direction
            Rigidbody2D playerRigidbody = other.gameObject.GetComponent<Rigidbody2D>();
            playerRigidbody.velocity = direction * shootForce;
        }
    }

}
