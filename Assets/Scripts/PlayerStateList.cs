using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateList : MonoBehaviour
{
    public bool jumping = false;
    public bool dashing = false;
    public bool recoilingX, recoilingY;
    public bool lookingRight;
    public bool invincible;
    public bool healing;
    public bool casting;
    public bool cutscene = false;
    public bool alive = true; // Default to alive

    // You can use the Awake method to initialize properties.
    private void Awake()
    {
        // Set any other initializations here if needed.
    }
}
