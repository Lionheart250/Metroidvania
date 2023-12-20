using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateList : MonoBehaviour
{
    public static PlayerStateList Instance; // Singleton reference

    public bool jumping = false;
    public bool dashing = false;
    public bool dodging = false;
    public bool recoilingX, recoilingY;
    public bool lookingRight = false;
    public bool invincible;
    public bool healing;
    public bool casting;
    public bool cutscene = false;
    public bool alive = true; // Default to alive

    private void Awake()
    {
        // Ensure there's only one instance of the PlayerStateList
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Makes the object persist between scenes
        }
        else
        {
            Destroy(gameObject); // If another instance is found, destroy it
        }
    }
}
