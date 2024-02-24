using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IllusionaryWall : MonoBehaviour
{
    // Reference to the illusionary wall object
    public GameObject illusionaryWall;

    // Function called when a collision occurs with another collider
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the colliding object is the player's attack
        if (collision.gameObject.CompareTag("Player"))
        {
            // Trigger the illusion effect (e.g., disable the illusionary wall)
            illusionaryWall.SetActive(false);
        }
    }
}
