using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowPlatform : MonoBehaviour
{
    private BoxCollider2D platformCollider;

    void Start()
    {
        platformCollider = GetComponent<BoxCollider2D>();
        EnableCollider(); // Ensure collider is initially enabled
    }

    private void EnableCollider()
    {
        if (platformCollider != null && !platformCollider.enabled)
        {
            platformCollider.enabled = true;
        }
    }

    private void DisableCollider()
    {
        if (platformCollider != null && platformCollider.enabled)
        {
            platformCollider.enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && platformCollider.enabled)
        {
            CheckPlayerState();
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CheckPlayerState();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            EnableCollider();
        }
    }

    private void CheckPlayerState()
    {
        if (PlayerController.Instance != null && !PlayerController.Instance.pState.shadowForm)
        {
            DisableCollider();
        }
    }
}




