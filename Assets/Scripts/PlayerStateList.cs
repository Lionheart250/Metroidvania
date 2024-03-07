using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateList : MonoBehaviour
{
    public static PlayerStateList Instance { get; private set; }

    public bool lightForm = false;
    public bool shadowForm = false;
    public bool puddleForm = false;
    public bool jumping = false;
    public bool lightJumping = false;
    public bool hovering = false;
    public bool dashing = false;
    public bool dodging = false;
    public bool recoilingX, recoilingY;
    public bool lookingRight = true;
    public bool invincible;
    public bool healing;
    public bool casting;
    public bool aiming = false;
    public bool cutscene = false;
    public bool alive = true; // Default to alive

    private void Awake()
    {
        // Ensure there's only one instance of the PlayerStateList
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Makes the object persist between scenes

            lightForm = true; // Light form is true by default
            shadowForm = false; // Shadow form is false by default
        }
        else
        {
            Destroy(gameObject); // If another instance is found, destroy it
        }
    }
}
