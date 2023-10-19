using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{   
    [Header("Horizontal Movement Settings:")]
    [SerializeField] private float walkSpeed = 1; //sets the players movement speed on the ground
    [Space(5)]

    [Header("Vertical Movement Settings")]
    [SerializeField] private float jumpForce = 45f; 
    private int jumpBufferCounter = 0; //stores the jump button input
    [SerializeField] private int jumpBufferFrames; //sets the max amount of frames the jump buffer input is stored
    private float coyoteTimeCounter = 0; //stores the Grounded() bool
    [SerializeField] private float coyoteTime; ////sets the max amount of frames the Grounded() bool is stored
     private int airJumpCounter = 0; //keeps track of how many times the player has jumped in the air
    [SerializeField] private int maxAirJumps; //the max no. of air jumps
    [SerializeField] private int maxFallingSpeed; //the max no. of air jumps
    private float gravity;
    [Space(5)]

    [Header("Ground Check Settings:")]
    [SerializeField] private Transform groundCheckPoint; //point at which ground check happens
    [SerializeField] private float groundCheckY = 0.2f; //how far down from ground chekc point is Grounded() checked
    [SerializeField] private float groundCheckX = 0.5f; //how far horizontally from ground chekc point to the edge of the player is
    [SerializeField] private LayerMask whatIsGround; //sets the ground layer
    [Space(5)]

    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed; //speed of the dash
    [SerializeField] private float dashTime; //amount of time spent dashing
    [SerializeField] private float dashCooldown; //amount of time between dashes
    [SerializeField] GameObject dashEffect;
    private bool canDash = true, dashed;
    [Space(5)]

    [HideInInspector] public PlayerStateList pState;
    [HideInInspector] public Rigidbody2D rb;

    private float xAxis, yAxis;
    Animator anim;

    public static PlayerController Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {     pState = GetComponent<PlayerStateList>();
    
          rb = GetComponent<Rigidbody2D>();
          anim = GetComponent<Animator>();
          gravity = rb.gravityScale;

    }

    // Update is called once per frame
    void Update()
    {
         GetInputs();
         UpdateJumpVariables();
         if (pState.dashing) return;
         Flip();
         Move();
         Jump();
         StartDash();
    }

    void GetInputs()
    {
       xAxis = Input.GetAxisRaw("Horizontal");
      yAxis = Input.GetAxisRaw("Vertical");
    }

    void Flip()
    {
        if (xAxis < 0)
        {
            transform.eulerAngles = new Vector2(0, 180);
            pState.lookingRight = false;
        }
        else if (xAxis > 0)
        {
            transform.eulerAngles = new Vector2(0, 0);
            pState.lookingRight = true;
        }
    }

    private void Move()
    {
        rb.velocity = new Vector2(walkSpeed * xAxis, rb.velocity.y);
        anim.SetBool("Walking", rb.velocity.x != 0 && Grounded());
    }

    void StartDash()
    {
        if (Input.GetButtonDown("Dash") && canDash && !dashed)
        {
            StartCoroutine(Dash());
            dashed = true;
        }

        if (Grounded())
        {
            dashed = false;
        }
    } 

    IEnumerator Dash()
    {
        canDash = false;
        pState.dashing = true;
        anim.SetTrigger("Dashing");
        // audioSource.PlayOneShot(dashAndAttackSound);
        rb.gravityScale = 0;
        int _dir = pState.lookingRight ? 1 : -1;
        rb.velocity = new Vector2(_dir * dashSpeed, 0);
        if (Grounded()) Instantiate(dashEffect, transform);
        yield return new WaitForSeconds(dashTime);
        rb.gravityScale = gravity;
        pState.dashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    public bool Grounded()
    {
        if (Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckY, whatIsGround) 
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround) 
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

     void Jump()
    {
        if (jumpBufferCounter > 0 && coyoteTimeCounter > 0 && !pState.jumping)
        {
            //audioSource.PlayOneShot(jumpSound);

            rb.velocity = new Vector3(rb.velocity.x, jumpForce);

            pState.jumping = true;
        }
        
        if (!Grounded() && airJumpCounter < maxAirJumps && Input.GetButtonDown("Jump")) //&& unlockedVarJump)
        {
            //audioSource.PlayOneShot(jumpSound);

            pState.jumping = true;

            airJumpCounter++;

            rb.velocity = new Vector3(rb.velocity.x, jumpForce);
        }

        if (Input.GetButtonUp("Jump") && rb.velocity.y > 3)
        {
            pState.jumping = false;

            rb.velocity = new Vector2(rb.velocity.x, 0);
        }

        rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -maxFallingSpeed, rb.velocity.y));

        anim.SetBool("Jumping", !Grounded());
    }
 
    void UpdateJumpVariables()
    {
        if (Grounded())
        { 
            //if (!landingSoundPlayed)
            {
                //audioSource.PlayOneShot(landingSound);
               // landingSoundPlayed = true;
            }
            pState.jumping = false;
            coyoteTimeCounter = coyoteTime;
            airJumpCounter = 0;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
           // landingSoundPlayed = false;
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferFrames;
        }
        else
        {
            jumpBufferCounter--;
        }
    }
    
}
