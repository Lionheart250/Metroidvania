using System.Collections;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.InputSystem;




public class PlayerController : MonoBehaviour
{

    [Header("Horizontal Movement Settings:")]
    [SerializeField] private float walkSpeed = 1; //sets the players movement speed on the ground
    private float originalWalkSpeed;
    [Space(5)]



    [Header("Vertical Movement Settings")]
    [SerializeField] private float jumpForce = 45f; //sets how hight the player can jump
    [SerializeField] private float lightJumpForce = 60f; 
    private int jumpBufferCounter = 0; //stores the jump button input
    [SerializeField] private int jumpBufferFrames; //sets the max amount of frames the jump buffer input is stored
    [SerializeField] private int lightJumpBufferFrames;

    private float coyoteTimeCounter = 0; //stores the Grounded() bool
    [SerializeField] private float coyoteTime; ////sets the max amount of frames the Grounded() bool is stored

    [SerializeField] private int airJumpCounter = 0; //keeps track of how many times the player has jumped in the air
    private int lightJumpCounter = 0;
    [SerializeField] private float lightJumpChargeSpeed;
    [SerializeField] public float lightJumpChargeTime;
    [SerializeField] private int maxAirJumps; //the max no. of air jumps
    [SerializeField] private int maxLightJumps;
    [SerializeField] private int maxFallingSpeed; //the max no. of air jumps
    public float fallGravityMultiplier; // You can adjust this value
    private float originalFallGravityMultiplier;
    private float gravity; //stores the gravity scale at start
    [Space(5)]

    [Header("Wall Jump Settings")]
    [SerializeField] private float wallSlidingSpeed = 2f;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float wallJumpingDuration;
    [SerializeField] private Vector2 wallJumpingPower;
    float wallJumpingDirection;
    bool isWallSliding;
    bool isWallJumping;

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
    [SerializeField] GameObject puddleFormCollider;
    private bool canDash = true, dashed;
    private bool canDodge = true, dodged;
    [Space(5)]



    [Header("Attack Settings:")]
    [SerializeField] private Transform SideAttackTransform; //the middle of the side attack area
    [SerializeField] private Vector2 SideAttackArea; //how large the area of side attack is
    [SerializeField] private Transform UpAttackTransform; //the middle of the up attack area
    [SerializeField] private Vector2 UpAttackArea; //how large the area of side attack is

    [SerializeField] private Transform DownAttackTransform; //the middle of the down attack area
    [SerializeField] private Vector2 DownAttackArea; //how large the area of down attack is
    [SerializeField] private Transform ChargeAttackTransform; //the middle of the side attack area
    [SerializeField] private Vector2 ChargeAttackArea; //how large the area of side attack is
    [SerializeField] private Transform ShadowSideAttackTransform; //the middle of the side attack area
    [SerializeField] private Vector2 ShadowSideAttackArea; //how large the area of side attack is
    [SerializeField] private Transform ShadowUpAttackTransform; //the middle of the side attack area
    [SerializeField] private Vector2 ShadowUpAttackArea; //how large the area of side attack is
    [SerializeField] private Transform ShadowDownAttackTransform; //the middle of the side attack area
    [SerializeField] private Vector2 ShadowDownAttackArea; //how large the area of side attack is

    [SerializeField] private LayerMask attackableLayer; //the layer the player can attack and recoil off of

    private float timeBetweenAttack = 0.4f, timeSinceAttck;

    [SerializeField] private float damage; //the damage the player does to an enemy

    [SerializeField] private GameObject slashEffect; //the effect of the slashs
    [SerializeField] private GameObject chargeSlashEffect; 
    [SerializeField] private float chargeSpeed;
    [SerializeField] public float chargeTime;  // Adjust this value as needed
    bool restoreTime;
    float restoreTimeSpeed;
    [SerializeField] GameObject chargeParticles;
    [Space(5)]



    [Header("Recoil Settings:")]
    [SerializeField] private int recoilXSteps = 1; //how many FixedUpdates() the player recoils horizontally for
    [SerializeField] private int recoilYSteps = 1; //how many FixedUpdates() the player recoils vertically for

    [SerializeField] private float recoilXSpeed = 100; //the speed of horizontal recoil
    [SerializeField] private float recoilYSpeed = 100; //the speed of vertical recoil

    private int stepsXRecoiled, stepsYRecoiled; //the no. of steps recoiled horizontally and verticall
    [Space(5)]


    [Header("Health Settings")]
    public int health;
    public int maxHealth;
    public int maxTotalHealth = 10;
    public int heartShards;
    [SerializeField] GameObject bloodSpurt;
    [SerializeField] float hitFlashSpeed;
    public delegate void OnHealthChangedDelegate();
    [HideInInspector] public OnHealthChangedDelegate onHealthChangedCallback;

    float healTimer;
    [SerializeField] float timeToHeal;
    [Space(5)]


    [Header("Mana Settings")]
    [SerializeField] UnityEngine.UI.Image manaStorage;

    [SerializeField] float mana;
    [SerializeField] float manaDrainSpeed;
    [SerializeField] float manaGain;
    public bool halfMana;

    public ManaOrbsHandler manaOrbsHandler;
    public int orbShard;
    public int manaOrbs;
    [Space(5)]


    [Header("Spell Settings")]
    //spell stats
    [SerializeField] float manaSpellCost = 0.3f;
    [SerializeField] float timeBetweenCast = 2f;
    [SerializeField] float spellDamage; //upspellexplosion and downspellfireball
    [SerializeField] float downSpellForce; // desolate dive only
    [SerializeField] private Transform eyeAttackTransform;
    //spell cast objects
    [SerializeField] GameObject sideSpellFireball;
    [SerializeField] GameObject sideSpellShadowFireball;
    [SerializeField] GameObject upSpellExplosion;
    [SerializeField] GameObject downSpellFireball;
    [SerializeField] GameObject downSpellShadowFireball;
    [SerializeField] GameObject shadowBloodSpray;
    [SerializeField] GameObject lightBall;
    [SerializeField] GameObject lightningBow;
    [SerializeField] GameObject lightningArrow;
    [SerializeField] GameObject lightningDart;
    [SerializeField] GameObject lightningStrike;
    [SerializeField] float lightJumpDamage;
    [SerializeField] private Transform LightJumpTransform; //the middle of the up attack area
    [SerializeField] private Vector2 LightJumpArea; //how large the area of side attack is
    float timeSinceCast;
    float castOrHealTimer;
    [Space(5)]


    [Header("Camera Stuff")]
    [SerializeField] private float playerFallSpeedThreshold = -10;
    [Space(5)]

    [Header("Audio")]
    [SerializeField] AudioClip landingSound;
    [SerializeField] AudioClip jumpSound;
    [SerializeField] AudioClip dashAndAttackSound;
    [SerializeField] AudioClip spellCastSound;
    [SerializeField] AudioClip ExplosionSpellCastSound;
    [SerializeField] AudioClip hurtSound;
    [Space(5)]


    [Header("BlackShield Settings")]
    public UnityEngine.Rendering.Universal.Light2D playerLight;
    public bool Shielded = false;
    [SerializeField] GameObject Shield;
    [Space(5)]
    

    

    [HideInInspector] public PlayerStateList pState;
    [HideInInspector] public Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sr;
    private Color originalColor;
    private CapsuleCollider2D mainCollider;
    private AudioSource audioSource;

    //Input Variables
    private float xAxis, yAxis;
    private bool attack = false;
    private bool blackShield = false;
    bool openMap;
    bool openInventory;

    private bool canFlash = true;

    private bool landingSoundPlayed;


    public static PlayerController Instance;

    //unlocking 
    public bool unlockedWallJump;
    public bool unlockedDash;
    public bool unlockedDodge;
    public bool unlockedVarJump;

    public bool unlockedSideCast;
    public bool unlockedUpCast;
    public bool unlockedDownCast;
    public bool unlockedBlackShield;
    [Space(5)]

    [Header("Health Shards")]
    public bool unlockedHeartShard1;
    public bool unlockedHeartShard2, unlockedHeartShard3, unlockedHeartShard4, unlockedHeartShard5, unlockedHeartShard6,
    unlockedHeartShard7, unlockedHeartShard8, unlockedHeartShard9, unlockedHeartShard10, unlockedHeartShard11, unlockedHeartShard12, unlockedHeartShard13,
    unlockedHeartShard14, unlockedHeartShard15, unlockedHeartShard16, unlockedHeartShard17, unlockedHeartShard18, unlockedHeartShard19, unlockedHeartShard20;
    [Space(5)]

    [Header("Orb Shards")]
    public bool unlockedOrbShard1;
    public bool unlockedOrbShard2, unlockedOrbShard3, unlockedOrbShard4, unlockedOrbShard5,
    unlockedOrbShard6, unlockedOrbShard7, unlockedOrbShard8, unlockedOrbShard9;     
    

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
    {
        pState = GetComponent<PlayerStateList>();
        

        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        gravity = rb.gravityScale;
        anim = GetComponent<Animator>();
        manaOrbsHandler = FindObjectOfType<ManaOrbsHandler>();
        originalWalkSpeed = walkSpeed;
        originalFallGravityMultiplier = fallGravityMultiplier;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        mainCollider = GetComponent<CapsuleCollider2D>();
        originalColor = sr.color;
    

        

        SaveData.Instance.LoadPlayerData();
        if(manaOrbs > 3)
        {
            manaOrbs = 3;
        }
        if (halfMana)
        {
            UIManager.Instance.SwitchMana(UIManager.ManaState.HalfMana);
        }

        Mana = mana;
        manaStorage.fillAmount = Mana;

        //Health = maxHealth;
        Debug.Log(transform.position);
    }

    // Update is called once per frame
    private void OnDrawGizmos()
{
    DrawGizmo(SideAttackTransform, SideAttackArea, Color.red);
    DrawGizmo(ChargeAttackTransform, ChargeAttackArea, Color.red);
    DrawGizmo(UpAttackTransform, UpAttackArea, Color.red);
    DrawGizmo(DownAttackTransform, DownAttackArea, Color.red);
    
    // Draw shadow gizmos in a different color (e.g., blue)
    DrawGizmo(ShadowSideAttackTransform, ShadowSideAttackArea, Color.blue);
    DrawGizmo(ShadowUpAttackTransform, ShadowUpAttackArea, Color.blue);
    DrawGizmo(ShadowDownAttackTransform, ShadowDownAttackArea, Color.blue);
    
    Gizmos.color = Color.red; // Reset color for the remaining gizmos
    Gizmos.DrawWireSphere(LightJumpTransform.position, Mathf.Max(LightJumpArea.x, LightJumpArea.y) * 0.5f);
}

private void DrawGizmo(Transform transform, Vector3 area, Color color)
{
    Gizmos.color = color;
    Gizmos.DrawWireCube(transform.position, area);
}

    void Update()
    {
        if (GameManager.Instance.gameIsPaused) return;

        if (pState.cutscene) return;
        if(pState.alive)
        {
            GetInputs();
            ToggleMap();
            ToggleInventory();
            
        }
        
        UpdateJumpVariables();
        RestoreTimeScale();
        UpdateCameraYDampForPlayerFall();

        if (pState.alive)
        {
            Heal();
        }

        if (pState.dashing || pState.healing || pState.dodging) return;

        if(pState.alive)
        {
            if(!isWallJumping)
            {   
                //Flip();
                Move();
                Jump();
                LightJump();
                SwapForm();
            }
            if(unlockedWallJump)
            {
                WallSlide();
                WallJump();
            }
            if(unlockedDash)
            {
                StartDash();
            }   
            if(unlockedDodge)
            {
                StartDodge();
            }         
            Attack();
            ShadowAttack();
            CastSpell();
            BlackShield();
            LightningBow();
            LightDart();
            LightningStrike();
            StartShadowDash();
            EndShadowDash();
        }        
        FlashWhileInvincible();     
        
        if(Input.GetKeyDown(KeyCode.L))
        {
            Health = 0;
            StartCoroutine(Death());
        }
        if (manaOrbs > 3)
        {
            manaOrbs = 3;
        }    
    }
   public bool isOnPlatform;
   public Rigidbody2D platformRb;

    private void FixedUpdate()
    {
        if (pState.cutscene) return;

        if (pState.dashing || pState.healing || pState.dodging) return;
        Recoil();
        if(pState.alive)
        {
            if(!isWallJumping)
            {
                Flip();
                //Move();
                //Jump();
            }
        

    if (isOnPlatform && platformRb != null)
    {
        // If on platform and not providing any input, match platform velocity
        rb.velocity = new Vector2(walkSpeed * xAxis + platformRb.velocity.x, rb.velocity.y);
    }
    }
    }
    private Vector2 lastInputDirection;

    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (_other.CompareTag("StayShadowPuddle"))
        {
            pState.puddleForm = true;
            // You may add additional logic or start a coroutine here if needed
        }
    
        if (_other.GetComponent<Enemy>() != null && pState.casting && !pState.lightJumping) 
        {
            _other.GetComponent<Enemy>().EnemyGetsHit(spellDamage, (_other.transform.position - transform.position).normalized, recoilYSpeed);
        }
        if (_other.GetComponent<Enemy>() != null && pState.lightJumping)
        {
            int _recoilLeftOrRight = pState.lookingRight ? 1 : -1;
            Vector2 hitDirection = new Vector2(_recoilLeftOrRight, -1);

        
            Hit(LightJumpTransform, LightJumpArea, ref pState.recoilingY, hitDirection, recoilYSpeed);

    
            Vector2 oppositeDirection = -hitDirection;

    
            rb.velocity = oppositeDirection * (lightJumpForce * 0.5f);
        }
}

private void OnTriggerExit2D(Collider2D _other)
{
    if (_other.CompareTag("StayShadowPuddle"))
    {
        pState.puddleForm = false;

        // Coroutine should only run when puddleForm toggles from true to false
        StartCoroutine(ShadowDashSequenceExitLongPuddle());
    }
}

    void ToggleMainColliderOn()
    {
    mainCollider.enabled = true;

    Debug.Log("Main collider is now enabled.");
    }

    void ToggleMainColliderOff()
    {
    mainCollider.enabled = false;

    Debug.Log("Main collider is now disabled.");
    }



    void GetInputs()
    {
       xAxis = Input.GetAxisRaw("Horizontal");
       yAxis = Input.GetAxisRaw("Vertical");
       attack = Input.GetButtonDown("Attack");
       blackShield = Input.GetButtonDown("Shield");
       openMap = Input.GetButton("Map");
       openInventory = Input.GetButton("Inventory");
        if (Input.GetButton("Cast/Heal"))
        {
            castOrHealTimer += Time.deltaTime;
        }
        

        // New Input System
        if (Gamepad.current != null && Gamepad.current.circleButton.isPressed)
        {
            castOrHealTimer += Time.deltaTime;
        }
        
    }
    void FreezeRigidbodyPosition()
    {
        savedPositionConstraints = rb.constraints;
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
    }

    void UnfreezeRigidbodyPosition()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;;
    }    

    void ToggleMap()
    {
        if(openMap)
        {
            UIManager.Instance.mapHandler.SetActive(true);
        }
        else
        {
            UIManager.Instance.mapHandler.SetActive(false);
        }
    }

    void ToggleInventory()
    {
        if (openInventory)
        {
            UIManager.Instance.inventory.SetActive(true);
        }
        else
        {
            UIManager.Instance.inventory.SetActive(false);
        }
    }
    

    void Flip()
{
    if (xAxis < 0)
    {
        if (pState.lookingRight || !pState.lookingRight)
        {
            transform.eulerAngles = new Vector2(0, 180);
            //transform.localScale = new Vector3(1, 1, 1);
            pState.lookingRight = false;
        }
    }
    else if (xAxis > 0)
    {
        if (!pState.lookingRight)
        {
            transform.eulerAngles = new Vector2(0, 0);
           // transform.localScale = new Vector3(-1, 1, 1);
            pState.lookingRight = true;
        }
    }
}

   private float walkTimer = 0f;
    private float maxWalkTimer = 1f;

    private void Move()
    {
        if (!pState.lightJumping)
        {
            rb.velocity = new Vector2(walkSpeed * xAxis, rb.velocity.y);
            if (rb.velocity.x != 0 && Grounded())
            {
                walkTimer += Time.deltaTime;

                // Clamp walkTimer to ensure it never goes above 2 or below 0
                walkTimer = Mathf.Clamp(walkTimer, 0f, maxWalkTimer);

                anim.SetBool("Running", true);
                anim.SetBool("Walking", false);

                // Check if walkTimer is greater than or equal to 0.5
                if (walkTimer >= 0.24f && rb.velocity.x != 0 && Grounded())
                {
                    anim.SetBool("Walking", true);
                    anim.SetBool("Running", false);
                }


            }
            else
            {
                // If not moving, reset walkTimer and set both Walking and Running to false
                walkTimer = 0;
                walkTimer = Mathf.Clamp(walkTimer, 0f, maxWalkTimer);
                anim.SetBool("Running", false);
                anim.SetBool("Walking", false);
                Debug.Log("walkTimer: " + walkTimer);
            }
        }
    }


    void UpdateCameraYDampForPlayerFall()
    {
        //if falling past a certain speed threshold
        if (rb.velocity.y < playerFallSpeedThreshold && !CameraManager.Instance.isLerpingYDamping && !CameraManager.Instance.hasLerpedYDamping)
        {
            StartCoroutine(CameraManager.Instance.LerpYDamping(true));
        }
        //if standing stil or moving up
        if(rb.velocity.y >= 0 && !CameraManager.Instance.isLerpingYDamping && CameraManager.Instance.hasLerpedYDamping)
        {
            //reset camera function
            CameraManager.Instance.hasLerpedYDamping = false;
            StartCoroutine(CameraManager.Instance.LerpYDamping(false));
        }

    }

    void StartDash()
    {
        if ((Input.GetButtonDown("Dash")|| (Gamepad.current?.rightTrigger.wasPressedThisFrame == true)) && canDash && !dashed && pState.lightForm)
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
        audioSource.PlayOneShot(dashAndAttackSound);
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

    void StartDodge()
    {
        if ((Input.GetButtonDown("Dash")|| (Gamepad.current?.leftTrigger.wasPressedThisFrame == true)) && canDodge && !dodged && pState.lightForm)
        {
            StartCoroutine(Dodge());
            dodged = true;
        }
        if (Grounded())
        {
            dodged = false;
        }
    }

    IEnumerator Dodge()
    {
        canDodge = false;
        pState.invincible = true;
        pState.dodging = true;
        anim.SetTrigger("Dashing");
        audioSource.PlayOneShot(dashAndAttackSound);
        rb.gravityScale = 0;
        int _dir = pState.lookingRight ? 1 : -1;
        rb.velocity = new Vector2(-_dir * (dashSpeed * 0.10f), 0);
        //if (Grounded()) Instantiate(dashEffect, transform);
        FreezeRigidbodyPosition();
        yield return new WaitForSeconds(dashTime);
        rb.gravityScale = gravity;
        UnfreezeRigidbodyPosition();
        pState.dodging = false;
        pState.invincible = false;  
        yield return new WaitForSeconds(dashCooldown);    
        canDodge = true;
    }
    
    private bool isShadowDashing = false;
    private bool canStartShadowDash = true;
    void StartShadowDash()
    {
        if (canStartShadowDash && (Input.GetButtonDown("Dash") || (Gamepad.current?.rightTrigger.wasPressedThisFrame == true)) && pState.shadowForm)
        {
            StartCoroutine(ShadowDashSequence());
        }
    }
    
     IEnumerator ShadowDashSequence()
    {
        isShadowDashing = true;
        canStartShadowDash = false;

        sr.enabled = false;
        Color spriteColor = sr.color;
        spriteColor.a = 0f;
        sr.color = spriteColor;
    
        puddleFormCollider.SetActive(true);
        walkSpeed = 40f;
        ToggleMainColliderOff();
    
    // Wait for a set amount of time (adjust the time according to your needs)
        yield return new WaitForSeconds(1f);
        if (!pState.puddleForm)
        {
    // Call EndShadowDash after the delay
        isShadowDashing = false;
    
        ToggleMainColliderOn();
        walkSpeed = 50f;
        puddleFormCollider.SetActive(false);
        sr.enabled = true;
        spriteColor.a = originalColor.a;  // Use the original alpha value
        sr.color = spriteColor;

        Debug.Log("EndShadowDash called.");

        yield return new WaitForSeconds(1f);
        canStartShadowDash = true;
        }
    }  
 
    void EndShadowDash()
    {   
        if ((Input.GetButtonUp("Dash") || (Gamepad.current?.rightTrigger.wasReleasedThisFrame == true)) && pState.shadowForm && isShadowDashing && !pState.puddleForm)
        {
        isShadowDashing = false;
        ToggleMainColliderOn();
        walkSpeed = 50f;
        puddleFormCollider.SetActive(false);
        sr.enabled = true;
        Color spriteColor = sr.color;
        spriteColor.a = originalColor.a;  // Use the original alpha value

        StartCoroutine(ShadowDashSequenceExitLongPuddle());
        

        }
    }
    
    IEnumerator ShadowDashSequenceExitLongPuddle()
    {   
        yield return new WaitForSeconds(0.2f);
        isShadowDashing = false;
        sr.enabled = false;
        Color spriteColor = sr.color;
        spriteColor.a = 0f;
        sr.color = spriteColor;
        ToggleMainColliderOn();
        walkSpeed = 50f;
        puddleFormCollider.SetActive(false);
        sr.enabled = true;
        spriteColor.a = originalColor.a;  // Use the original alpha value
        sr.color = spriteColor;

        Debug.Log("EndShadowDash called.");

        yield return new WaitForSeconds(1f);
        canStartShadowDash = true;
    }
     
    public IEnumerator WalkIntoNewScene(Vector2 _exitDir, float _delay)
    {
        //If exit direction is upwards
        if(_exitDir.y != 0)
        {
            rb.velocity = jumpForce * _exitDir;
        }

        //If exit direction requires horizontal movement
        if(_exitDir.x != 0)
        {
            xAxis = _exitDir.x > 0 ? 1 : -1;

            Move();
        }

        Flip();
        yield return new WaitForSeconds(_delay);
        pState.cutscene = false;
    }

    void SwapForm()
{
    if ((Gamepad.current?.triangleButton.wasPressedThisFrame == true || Input.GetKeyDown(KeyCode.R)) && Grounded())
    {
        // Toggle between shadow form and light form
        pState.shadowForm = !pState.shadowForm;
        pState.lightForm = !pState.shadowForm;

        // Ensure sr is not null
        if (sr != null)
        {
            // Set the color based on the current form
            //sr.color = pState.shadowForm ? Color.black : Color.white;

            // Modify walkSpeed and gravity based on the current form
            if (pState.lightForm)
            {   
                anim.SetBool("ShadowForm", false);
                // Adjust parameters for light form
                walkSpeed = 60f; // Increase walkSpeed by 35%
                fallGravityMultiplier = originalFallGravityMultiplier * 1.5f;
                maxFallingSpeed = 150;
                anim.SetBool("LightForm", true);
            }
            else if (pState.shadowForm)
            {   
                anim.SetBool("LightForm", false);
                // Adjust parameters for shadow form
                // Reset to original walkSpeed if not in light form
                walkSpeed = 50f;
                fallGravityMultiplier = originalFallGravityMultiplier * 1.5f;
                maxFallingSpeed = 100;
                anim.SetBool("ShadowForm", true);
            }
        }
    }
}





    void Attack()
{
    timeSinceAttck += Time.deltaTime;

    if ((attack || (Gamepad.current?.squareButton.wasPressedThisFrame == true)) && timeSinceAttck >= timeBetweenAttack && !pState.shadowForm)
    {
        timeSinceAttck = 0;
        anim.SetTrigger("Attacking");
        audioSource.PlayOneShot(dashAndAttackSound);

        // Handle different attack types based on input and conditions
        if (yAxis == 0 || (yAxis < 0 && Grounded()) && !pState.shadowForm)
        {
            int _recoilLeftOrRight = pState.lookingRight ? 1 : -1;

            // Handle regular attack
            Hit(SideAttackTransform, SideAttackArea, ref pState.recoilingX, Vector2.right * _recoilLeftOrRight, recoilXSpeed);
            Instantiate(slashEffect, SideAttackTransform);
        }
        else if (yAxis > 0 && !pState.shadowForm)
        {
            // Handle up attack
            Hit(UpAttackTransform, UpAttackArea, ref pState.recoilingY, Vector2.up, recoilYSpeed);
            SlashEffectAtAngle(slashEffect, 80, UpAttackTransform);
        }
        else if (yAxis < 0 && !Grounded() && !pState.shadowForm)
        {
            
            // Handle down attack
            Hit(DownAttackTransform, DownAttackArea, ref pState.recoilingY, Vector2.down, recoilYSpeed);
            SlashEffectAtAngle(slashEffect, -90, DownAttackTransform);
        }
    }

    if ((Input.GetButton("Attack") || (Gamepad.current?.squareButton.isPressed == true)) && chargeTime <= 2 && !pState.shadowForm)
    {
        chargeTime += Time.deltaTime * chargeSpeed;
        chargeTime = Mathf.Clamp(chargeTime, 0f, 2f);

        GameObject _chargeParticles = Instantiate(chargeParticles, transform.position, Quaternion.identity);

        if (chargeTime >= 2f)
        {
            // Double the size
            Vector3 newScale = _chargeParticles.transform.localScale * 2f;
            _chargeParticles.transform.localScale = newScale;
        }

        Destroy(_chargeParticles, 0.05f);
    }
    else if ((Input.GetButtonDown("Attack") || (Gamepad.current?.squareButton.wasPressedThisFrame == true)) && !pState.shadowForm)
    {
        // If the attack button is pressed, but not held, reset chargeTime
        chargeTime = 0;
    }

    if ((Input.GetButtonUp("Attack") || (Gamepad.current?.squareButton.wasReleasedThisFrame == true)) && chargeTime < 2 && !pState.shadowForm)
    {
        chargeTime = 0;
    }
    else if ((Input.GetButtonUp("Attack") || (Gamepad.current?.squareButton.wasReleasedThisFrame == true)) && chargeTime >= 2 && !pState.shadowForm)
    {
        audioSource.PlayOneShot(dashAndAttackSound);
        // Release charge if the button is released and charging duration is sufficient
        ReleaseCharge();
    }
}


void ShadowAttack()
{
    if ((attack || (Gamepad.current?.squareButton.wasPressedThisFrame == true)) && timeSinceAttck >= timeBetweenAttack && pState.shadowForm)
    {
        timeSinceAttck = 0;
        anim.SetTrigger("Attacking");
        audioSource.PlayOneShot(dashAndAttackSound);

        // Handle different attack types based on input and conditions
        if (yAxis == 0 || (yAxis < 0 && Grounded()) && !pState.lightForm)
        {
            int _recoilLeftOrRight = pState.lookingRight ? 1 : -1;

            ShadowHit(ShadowSideAttackTransform, ShadowSideAttackArea, ref pState.recoilingX, Vector2.right * _recoilLeftOrRight, recoilXSpeed);
            Instantiate(chargeSlashEffect, ShadowSideAttackTransform);
        }
        else if (yAxis > 0 && pState.shadowForm)
        {
            // Handle up attack
            ShadowHit(ShadowUpAttackTransform, ShadowUpAttackArea, ref pState.recoilingY, Vector2.up, recoilYSpeed);
            SlashEffectAtAngle(chargeSlashEffect, 80, ShadowUpAttackTransform);
        }
        else if (yAxis < 0 && !Grounded() && pState.shadowForm)
        {
            // Handle down attack
            ShadowHit(ShadowDownAttackTransform, ShadowDownAttackArea, ref pState.recoilingY, Vector2.down, recoilYSpeed);
            SlashEffectAtAngle(chargeSlashEffect, -90, ShadowDownAttackTransform);
        }
    }
}

    void ReleaseCharge()
{
    int _recoilLeftOrRight = pState.lookingRight ? 1 : -1;

    // Manually set the damage value
    int customDamage = 5; // Replace 20 with your desired damage value

    Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(ChargeAttackTransform.position, ChargeAttackArea, 0, attackableLayer);

    if (objectsToHit.Length > 0)
    {
        pState.recoilingX = true;
        dashed = false;
    }

    for (int i = 0; i < objectsToHit.Length; i++)
    {
        if (objectsToHit[i].GetComponent<Enemy>() != null)
        {
            // Pass the custom damage to the EnemyGetsHit method
            objectsToHit[i].GetComponent<Enemy>().EnemyGetsHit(customDamage, Vector2.right * _recoilLeftOrRight, recoilXSpeed);

            if (objectsToHit[i].CompareTag("Enemy"))
                {
                    if(Mana < 1)
                    {
                        Mana += manaGain;
                    }

                    if(Mana >= 1 || (halfMana && Mana >= 0.5))
                    {
                        manaOrbsHandler.UpdateMana(manaGain * 3);
                    }
                                        
                }
        }
    }
    
    Instantiate(chargeSlashEffect, ChargeAttackTransform);
    chargeTime = 0;
    // Add code for the actual attack logic here
    Debug.Log("Charge Released");
}


    void Hit(Transform _attackTransform, Vector2 _attackArea, ref bool _recoilBool, Vector2 _recoilDir, float _recoilStrength)
    {
        Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(_attackTransform.position, _attackArea, 0, attackableLayer);

        if (objectsToHit.Length > 0)
        {
            _recoilBool = true;
            dashed = false;
        }
        for (int i = 0; i < objectsToHit.Length; i++)
        {
            if (objectsToHit[i].GetComponent<Enemy>() != null)
            {
                objectsToHit[i].GetComponent<Enemy>().EnemyGetsHit(damage, _recoilDir, _recoilStrength);

                if (objectsToHit[i].CompareTag("Enemy"))
                {
                    if(Mana < 1)
                    {
                        Mana += manaGain;
                    }

                    if(Mana >= 1 || (halfMana && Mana >= 0.5))
                    {
                        manaOrbsHandler.UpdateMana(manaGain * 3);
                    }
                                        
                }
            }
        }

        transform.position += new Vector3(_recoilDir.x * _recoilStrength * Time.deltaTime, _recoilDir.y * _recoilStrength * Time.deltaTime, 0f);

    }

    void ShadowHit(Transform _attackTransform, Vector2 _attackArea, ref bool _recoilBool, Vector2 _recoilDir, float _recoilStrength)
    {
        Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(_attackTransform.position, _attackArea, 0, attackableLayer);

        if (objectsToHit.Length > 0)
        {
            _recoilBool = true;
            dashed = false;
        }
        for (int i = 0; i < objectsToHit.Length; i++)
        {
            if (objectsToHit[i].GetComponent<Enemy>() != null)
            {
                objectsToHit[i].GetComponent<Enemy>().EnemyGetsHit(damage, _recoilDir, _recoilStrength);
            }
        }

        transform.position += new Vector3(_recoilDir.x * _recoilStrength * Time.deltaTime, _recoilDir.y * _recoilStrength * Time.deltaTime, 0f);

    }

    void SlashEffectAtAngle(GameObject _slashEffect, int _effectAngle, Transform _attackTransform)
{
    _slashEffect = Instantiate(_slashEffect, _attackTransform);

    // Flip the instantiated slash effect based on player direction
    _slashEffect.transform.localScale = new Vector3(1, pState.lookingRight ? 1 : -1, 1);

    // Set the angle of the slash effect
    _slashEffect.transform.eulerAngles = new Vector3(0, 0, _effectAngle);
}

    void ShadowSlashEffectAtAngle(GameObject _slashEffect, int _effectAngle, Transform _attackTransform)
    {
        _slashEffect = Instantiate(_slashEffect, _attackTransform);
        _slashEffect.transform.eulerAngles = new Vector3(0, 0, _effectAngle);
        _slashEffect.transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y);
    }

    void Recoil()
    {
        if (pState.recoilingX)
        {
            if (pState.lookingRight)
            {
                rb.velocity = new Vector2(-recoilXSpeed, 0);
            }
            else
            {
                rb.velocity = new Vector2(recoilXSpeed, 0);
            }
        }

        if (pState.recoilingY)
        {
            //rb.gravityScale = 0;
            if (yAxis < 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, recoilYSpeed);
            }
            else
            {
                rb.velocity = new Vector2(rb.velocity.x, -recoilYSpeed);
            }
            airJumpCounter = 0;
            lightJumpCounter = 0;
        }
        else if (pState.lightJumping == false)
        {
            rb.gravityScale = gravity;
        }

        //stop recoil
        if (pState.recoilingX && stepsXRecoiled < recoilXSteps)
        {
            stepsXRecoiled++;
        }
        else
        {
            StopRecoilX();
        }
        if (pState.recoilingY && stepsYRecoiled < recoilYSteps)
        {
            stepsYRecoiled++;
        }
        else
        {
            StopRecoilY();
        }

        if (Grounded())
        {
            StopRecoilY();
        }
    }
    void StopRecoilX()
    {
        stepsXRecoiled = 0;
        pState.recoilingX = false;
    }
    void StopRecoilY()
    {
        stepsYRecoiled = 0;
        pState.recoilingY = false;
    }

    public void TakeDamage(float _damage) 
{   
    if(pState.alive && !pState.dodging && !pState.lightJumping)
    {
        audioSource.PlayOneShot(hurtSound);

        Health -= Mathf.RoundToInt(_damage);
        Debug.Log("Player Health: " + Health); // Add this debug log
        if (Health <= 0)
        {
            Health = 0;
            StartCoroutine(Death());
        }
        else if(!pState.dodging)
        {
            StartCoroutine(StopTakingDamage());
        }
    }
}

IEnumerator StopTakingDamage()
{
    pState.invincible = true;
    Debug.Log("Player is invincible"); // Add this debug log
    GameObject _bloodSpurtParticles = Instantiate(bloodSpurt, transform.position, Quaternion.identity);
    Destroy(_bloodSpurtParticles, 1.5f);
    anim.SetTrigger("TakeDamage");
    Debug.Log("Triggered TakeDamage animation"); // Add this debug log
    yield return new WaitForSeconds(1f);
    pState.invincible = false;
    Debug.Log("Player is no longer invincible"); // Add this debug log
}


    IEnumerator Flash()
    {
        sr.enabled = !sr.enabled;
        canFlash = false;
        yield return new WaitForSeconds(0.1f);
        canFlash = true;
    }

    void FlashWhileInvincible()
    {
        if (pState.invincible)
        {
            if(Time.timeScale > 0.2 && canFlash)
            {
                StartCoroutine(Flash());
            }
        }
        else
        {
            sr.enabled = true;
        }
    }
    void RestoreTimeScale()
    {
        if (restoreTime)
        {
            if (Time.timeScale < 1)
            {
                Time.timeScale += Time.deltaTime * restoreTimeSpeed;
            }
            else
            {
                Time.timeScale = 1;
                restoreTime = false;
            }
        }
    }
    public void HitStopTime(float _newTimeScale, int _restoreSpeed, float _delay)
    {
        restoreTimeSpeed = _restoreSpeed;
        if (_delay > 0)
        {
            StopCoroutine(StartTimeAgain(_delay));
            StartCoroutine(StartTimeAgain(_delay));
        }
        else
        {
            restoreTime = true;
        }
        Time.timeScale = _newTimeScale;
    }
    IEnumerator StartTimeAgain(float _delay)
    {
        restoreTime = true;
        yield return new WaitForSeconds(_delay);
    }

    IEnumerator Death()
    {
        pState.alive = false;
        Time.timeScale = 1f;
        GameObject _bloodSpurtParticles = Instantiate(bloodSpurt, transform.position, Quaternion.identity);
        Destroy(_bloodSpurtParticles, 1.5f);
        anim.SetTrigger("Death");

        yield return new WaitForSeconds(0.9f);
        StartCoroutine(UIManager.Instance.ActivateDeathScreen());

        yield return new WaitForSeconds(0.1f);
        Instantiate(GameManager.Instance.shade, transform.position, Quaternion.identity);

        SaveData.Instance.SavePlayerData();

    }

    public int Health
    {
        get { return health; }
        set
        {
            if (health != value)
            {
                health = Mathf.Clamp(value, 0, maxHealth);

                if (onHealthChangedCallback != null)
                {
                    onHealthChangedCallback.Invoke();
                }
            }
        }
    }

    public void Respawned()
    {
        if(!pState.alive)
        {
            pState.alive = true;
            halfMana = true;
            UIManager.Instance.SwitchMana(UIManager.ManaState.HalfMana);
            Mana = 0;
            Health = maxHealth;
            anim.Play("Player_Idle");
            StartCoroutine(UIManager.Instance.DeactivateDeathScreen());
        }
    }

    public void RestoreMana()
    {
        halfMana = false;
        UIManager.Instance.SwitchMana(UIManager.ManaState.FullMana);
    }

   void Heal()
    {
     if (Input.GetButton("Cast/Heal") || (Gamepad.current?.circleButton.isPressed == true))
    {
        //Debug.Log("castOrHealTimer: " + castOrHealTimer);
        castOrHealTimer = Mathf.Clamp(castOrHealTimer, 0f, 1.0f);
       
    }
    else
    {
        //Debug.Log("castOrHealTimer: " + castOrHealTimer);
        // Decrease the timer when the button is not held down
        castOrHealTimer -= Time.deltaTime;
        castOrHealTimer = Mathf.Clamp(castOrHealTimer, 0f, 1.0f);
    }
    if ((Input.GetButton("Cast/Heal") || (Gamepad.current?.circleButton.isPressed == true)) && castOrHealTimer > 0.5f && Health < maxHealth && Mana > 0 && !pState.jumping && !pState.dashing)
    {
            pState.healing = true;
            anim.SetBool("Healing", true);

            //healing
            healTimer += Time.deltaTime;
            if (healTimer >= timeToHeal)
            {
                Health++;
                healTimer = 0;
            }

            //drain mana
            manaOrbsHandler.usedMana = true;
            manaOrbsHandler.countDown = 3f;
            Mana -= Time.deltaTime * manaDrainSpeed;
        }
        else
        {
            pState.healing = false;
            anim.SetBool("Healing", false);
            healTimer = 0;
        }
    }
    public float Mana
    {
        get { return mana; }
        set
        {
            //if mana stats change
            if (mana != value)
            {
                if(!halfMana)
                {
                    mana = Mathf.Clamp(value, 0, 1);
                }
                else
                {
                    mana = Mathf.Clamp(value, 0, 0.5f);
                }
                
                manaStorage.fillAmount = Mana;
            }
        }
    }

    void LightningBow()
    {
        if ((Input.GetButtonDown("Cast/Heal") || (Gamepad.current?.circleButton.wasPressedThisFrame == true)) && castOrHealTimer <= 0.5f && timeSinceCast >= timeBetweenCast && Mana >= manaSpellCost && pState.lightForm && yAxis > 0 && unlockedUpCast && Grounded())
        {
            pState.aiming = true;
            lightningBow.SetActive(true);
            FreezeRigidbodyPosition(); 
            timeSinceCast = 0;
            anim.SetBool("Casting", true);
        }
        else
        {
            timeSinceCast += Time.deltaTime;
        }

        if ((Input.GetButtonUp("Cast/Heal") || (Gamepad.current?.circleButton.wasReleasedThisFrame == true)) && Mana >= manaSpellCost && pState.lightForm && pState.aiming)
        {
            
            UnfreezeRigidbodyPosition();
            StartCoroutine(ShootLightningArrow());
            
            Mana -= manaSpellCost;
            manaOrbsHandler.usedMana = true;
            manaOrbsHandler.countDown = 3f;
        }
    }

    IEnumerator ShootLightningArrow()
    {
        Vector3 aimingDirection = new Vector3(xAxis, yAxis, 0f).normalized;

        GameObject _lightningArrow = Instantiate(lightningArrow, lightningBow.transform.position, Quaternion.identity);

        _lightningArrow.transform.position = lightningBow.transform.position;

        float angle = Mathf.Atan2(aimingDirection.y, aimingDirection.x) * Mathf.Rad2Deg;
        _lightningArrow.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        yield return new WaitForSeconds(0.15f);
        pState.aiming = false;
        lightningBow.SetActive(false);
        anim.SetBool("Casting", false);
    }

    void LightDart()
    {  
        if ((Input.GetButtonUp("Cast/Heal") || (Gamepad.current?.circleButton.wasReleasedThisFrame == true)) && !pState.aiming && castOrHealTimer < 0.5f && timeSinceCast >= timeBetweenCast 
        && Mana >= manaSpellCost / 3.0f && pState.lightForm)
        {
            pState.casting = true;
            timeSinceCast = 0;
            StartCoroutine(LightDartCoroutine());
        }
        else
        {
              timeSinceCast += Time.deltaTime;
        }
    }
    IEnumerator LightDartCoroutine()
    {   
    if (yAxis == 0 && unlockedSideCast)
    {
        anim.SetBool("Casting", true);
        GameObject _lightDart = Instantiate(lightningDart, SideAttackTransform.position, Quaternion.identity);
        timeSinceCast = 1;
        audioSource.PlayOneShot(spellCastSound);

    
        // Flip fireball based on the player's facing direction
        if (pState.lookingRight)
        {
            _lightDart.transform.eulerAngles = Vector3.zero;
        }
        else
        {
            _lightDart.transform.eulerAngles = new Vector2(_lightDart.transform.eulerAngles.x, 180); 
        }
        
            Mana -= manaSpellCost / 3f;
            manaOrbsHandler.usedMana = true;
            manaOrbsHandler.countDown = 3f;
            
        }
        else
        {
            timeSinceCast += Time.deltaTime;
        }
        yield return new WaitForSeconds(0.15f);
        anim.SetBool("Casting", false);
        castOrHealTimer = 0;
    }


    
    void LightningStrike()
    {
        if ((Input.GetButtonUp("Cast/Heal") || (Gamepad.current?.circleButton.wasReleasedThisFrame == true)) && !pState.aiming &&  castOrHealTimer <= 0.5f && timeSinceCast >= timeBetweenCast 
        && Mana >= manaSpellCost && pState.lightForm)
        {
            if ((yAxis < 0 && Grounded()) && unlockedDownCast && pState.lightForm)
            {
                StartCoroutine(LightningStrikeHit());
            }
            if ((yAxis < 0 && !Grounded()) && unlockedDownCast && pState.lightForm)
            {
                StartCoroutine(AirLightningStrikeHit());
            }
        }
    }

    IEnumerator LightningStrikeHit()
    {
        anim.SetBool("Casting", true);
        pState.casting = true;
        FreezeRigidbodyPosition();

        yield return new WaitForSeconds(0.15f);

        lightningStrike.SetActive(true);
        audioSource.PlayOneShot(spellCastSound);

        Mana -= manaSpellCost;
        manaOrbsHandler.usedMana = true;
        manaOrbsHandler.countDown = 3f;
        yield return new WaitForSeconds(0.5f);
               
        anim.SetBool("Casting", false);
        pState.casting = false;
        lightningStrike.SetActive(false);
        UnfreezeRigidbodyPosition();

    }

    IEnumerator AirLightningStrikeHit()
    {
        anim.SetBool("Casting", true);
        pState.casting = true;
            
        yield return new WaitForSeconds(0.15f);

        downSpellFireball.SetActive(true);
        audioSource.PlayOneShot(spellCastSound);

        Mana -= manaSpellCost;
        manaOrbsHandler.usedMana = true;
        manaOrbsHandler.countDown = 3f;
        yield return new WaitForSeconds(0.35f);
        
    }
    
    void CastSpell()
    {
        if ((Input.GetButtonUp("Cast/Heal") || (Gamepad.current?.circleButton.wasReleasedThisFrame == true)) && castOrHealTimer <= 0.5f && timeSinceCast >= timeBetweenCast && Mana >= manaSpellCost && pState.shadowForm)
        {
            pState.casting = true;
            timeSinceCast = 0;
            StartCoroutine(CastCoroutine());
        }
        else
        {
            timeSinceCast += Time.deltaTime;
        }

        if (!Input.GetButton("Cast/Heal") && (Gamepad.current?.circleButton.isPressed == false))
        {
            //castOrHealTimer = 0;
            
        }

        if (Grounded())
        {
            //disable downspell if on the ground
            downSpellFireball.SetActive(false);
            lightBall.SetActive(false);
        }
        //if down spell is active, force player down until grounded
        if(downSpellFireball.activeInHierarchy)
        {
            rb.velocity += downSpellForce * Vector2.down;
        }
        if(downSpellShadowFireball.activeInHierarchy)
        {
            rb.velocity += (downSpellForce * 0.05f) * Vector2.up;
        }
    }
    IEnumerator CastCoroutine()
    {   if (yAxis == 0 && unlockedSideCast)
        {
            anim.SetBool("Casting", true);
            pState.casting = true;
            GameObject _shadowBloodSpray = Instantiate(shadowBloodSpray, eyeAttackTransform.position, Quaternion.identity);

            if (pState.lookingRight)
            {
                _shadowBloodSpray.transform.eulerAngles = Vector3.zero;
               
            }
            else
            {
                _shadowBloodSpray.transform.eulerAngles =  new Vector2(_shadowBloodSpray.transform.eulerAngles.x, 180);
            }

            Mana -= manaSpellCost;
            manaOrbsHandler.usedMana = true;
            manaOrbsHandler.countDown = 3f;
            pState.casting = false;
        }     
        if( yAxis > 0 && unlockedUpCast)
        {
            anim.SetBool("Casting", true);
            yield return new WaitForSeconds(0.15f);

            Instantiate(upSpellExplosion, transform);
            rb.velocity = Vector2.zero;

            Mana -= manaSpellCost;
            manaOrbsHandler.usedMana = true;
            manaOrbsHandler.countDown = 3f;
            audioSource.PlayOneShot(ExplosionSpellCastSound);
            yield return new WaitForSeconds(0.35f);
        }
        else if((yAxis < 0 && !Grounded()) && unlockedDownCast)
        {
            anim.SetBool("Casting", true);
            
            

            downSpellShadowFireball.SetActive(true);

            audioSource.PlayOneShot(ExplosionSpellCastSound);

            Mana -= manaSpellCost;
            manaOrbsHandler.usedMana = true;
            manaOrbsHandler.countDown = 3f;
            pState.recoilingY = true;
            yield return new WaitForSeconds(0.35f);
            downSpellShadowFireball.SetActive(false);
            
        }
        else if((yAxis < 0 && Grounded()) && unlockedDownCast)
        {
            anim.SetBool("Casting", true);
            
            yield return new WaitForSeconds(0.15f);

            sideSpellShadowFireball.SetActive(true);
            audioSource.PlayOneShot(ExplosionSpellCastSound);

            Mana -= manaSpellCost;
            manaOrbsHandler.usedMana = true;
            manaOrbsHandler.countDown = 3f;
            pState.recoilingX = true;
            yield return new WaitForSeconds(0.35f);
            sideSpellShadowFireball.SetActive(false);
            
        }          
        anim.SetBool("Casting", false);
        pState.casting = false;
        //castOrHealTimer = 0;
    }


    

    void BlackShield()
    {   
        if ((Input.GetButtonDown("Shield") || (Gamepad.current?.leftTrigger.wasPressedThisFrame == true)) && !Shielded && unlockedBlackShield && pState.shadowForm && Grounded())
        {              
            //audioSource.PlayOneShot(jumpSound);
            anim.SetBool("Casting", true);
            Shield.SetActive(true);
            Shielded = true; 
            FreezeRigidbodyPosition();          
        }
        else if ((Input.GetButtonUp("Shield") || (Gamepad.current?.leftTrigger.wasReleasedThisFrame == true)) && Shielded && unlockedBlackShield && pState.shadowForm) 
        {   
            anim.SetBool("Casting", false);
            Shield.SetActive(false);
            Shielded = false;  
            UnfreezeRigidbodyPosition();  
        }
    }
    
    public bool Grounded()
{
    bool grounded = Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckY, whatIsGround)
        || Physics2D.Raycast(groundCheckPoint.position + new Vector3(groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround)
        || Physics2D.Raycast(groundCheckPoint.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround);

    if (grounded)
    {
        Debug.DrawRay(groundCheckPoint.position, Vector2.down * groundCheckY, Color.green); // Visualize the first raycast
        Debug.DrawRay(groundCheckPoint.position + new Vector3(groundCheckX, 0, 0), Vector2.down * groundCheckY, Color.green); // Visualize the second raycast
        Debug.DrawRay(groundCheckPoint.position + new Vector3(-groundCheckX, 0, 0), Vector2.down * groundCheckY, Color.green); // Visualize the third raycast
    }
    else
    {
        Debug.DrawRay(groundCheckPoint.position, Vector2.down * groundCheckY, Color.yellow); // Visualize the first raycast
        Debug.DrawRay(groundCheckPoint.position + new Vector3(groundCheckX, 0, 0), Vector2.down * groundCheckY, Color.red); // Visualize the second raycast
        Debug.DrawRay(groundCheckPoint.position + new Vector3(-groundCheckX, 0, 0), Vector2.down * groundCheckY, Color.red); // Visualize the third raycast
    }

    return grounded;
}

    

    private RigidbodyConstraints2D savedPositionConstraints;
    void Jump()
    {    

    if (jumpBufferCounter > 0 && Grounded() && coyoteTimeCounter > 0 && !pState.jumping)
    {
        pState.jumping = true;
        
        //audioSource.PlayOneShot(jumpSound);

        rb.velocity = new Vector3(rb.velocity.x, jumpForce);        
    }

    if (!Grounded() && airJumpCounter < maxAirJumps && ((Input.GetButtonDown("Jump") || (Gamepad.current?.crossButton.wasPressedThisFrame == true)) && unlockedVarJump && !pState.lightForm))
    {
        audioSource.PlayOneShot(jumpSound);

        pState.jumping = true;

        airJumpCounter++;

        rb.velocity = new Vector3(rb.velocity.x, jumpForce);
    }

    if ((Input.GetButtonUp("Jump") || (Gamepad.current?.crossButton.wasReleasedThisFrame == true)) && rb.velocity.y > 3 && !pState.lightJumping)
    {
        pState.jumping = false;

        rb.velocity = new Vector2(rb.velocity.x, 0);
    }

    // Increase gravity while falling
    if (rb.velocity.y < 0 && !pState.lightJumping)
    {
        rb.velocity += Vector2.up * Physics2D.gravity.y * (fallGravityMultiplier - 1) * Time.deltaTime;
    }

    // Clamp the vertical velocity to control falling speed
    rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -maxFallingSpeed, rb.velocity.y));

    anim.SetBool("Jumping", !Grounded());
}

    // Add a variable to track the last light jump direction
private Vector2 lastLightJumpDirection = Vector2.zero;

void LightJump()
{
    if (!Grounded() && lightJumpCounter < maxLightJumps && (Input.GetButtonDown("Jump") || (Gamepad.current?.crossButton.wasPressedThisFrame == true)) && unlockedVarJump && pState.lightForm && !pState.lightJumping && !isWallJumping && !Walled())
    {   
        
        StartCoroutine(LightJumpCoroutine());
        lightJumpCounter++;
    }

     if (Grounded())
    {
        lastLightJumpDirection = Vector2.zero;
    }
}

IEnumerator LightJumpCoroutine()
{   
    pState.lightJumping = true;
    UnfreezeRigidbodyPosition();
    Vector2 launchDirection;
    if (Mathf.Approximately(xAxis, 0) && Mathf.Approximately(yAxis, 0))
    {
    // No input, use the current facing direction or another default direction.
    launchDirection = lastLightJumpDirection;
    }
    else
    {
    float angle = Mathf.Atan2(yAxis, xAxis) * Mathf.Rad2Deg;
    angle = Mathf.Round(angle / 45) * 45;
    float roundedX = Mathf.Cos(angle * Mathf.Deg2Rad);
    float roundedY = Mathf.Sin(angle * Mathf.Deg2Rad);
    
    launchDirection = new Vector2(roundedX, roundedY).normalized;
    }
    if (launchDirection != lastLightJumpDirection)
    {
    rb.velocity = new Vector2(rb.velocity.x, 0);
    FreezeRigidbodyPosition();
    yield return new WaitForSeconds(0.15f);
    UnfreezeRigidbodyPosition();
    
    
    lightBall.SetActive(true);
    anim.SetTrigger("Dashing");
    audioSource.PlayOneShot(dashAndAttackSound);
    
    rb.gravityScale = 0;

    rb.velocity = launchDirection * lightJumpForce;
    lastLightJumpDirection = launchDirection;
    
    yield return new WaitForSeconds(dashTime);
    pState.lightJumping = false;
    FreezeRigidbodyPosition();
    yield return new WaitForSeconds(0.1f);
    UnfreezeRigidbodyPosition();
    rb.velocity = Vector2.zero;
    lightBall.SetActive(false);        
    rb.gravityScale = gravity;
    

    }
}

    void UpdateJumpVariables()
    {
        if (Grounded())
        { 
            if (!landingSoundPlayed || (!landingSoundPlayed && !isOnPlatform))
            {
                audioSource.PlayOneShot(landingSound);
                landingSoundPlayed = true;
            }
            pState.jumping = false;
            pState.lightJumping = false;
            coyoteTimeCounter = coyoteTime;
            airJumpCounter = 0;
            lightJumpCounter = 0;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
            landingSoundPlayed = false;
        }

if ((Input.GetButtonDown("Jump") || (Gamepad.current?.crossButton.wasPressedThisFrame == true)))
        {
            jumpBufferCounter = jumpBufferFrames;
        }
        if ((Input.GetButtonDown("Jump") || (Gamepad.current?.crossButton.wasPressedThisFrame == true)) && pState.lightJumping)
        {
            jumpBufferCounter = lightJumpBufferFrames;

        }
        else
        {
            jumpBufferCounter--;
        }
    }

    
    private bool Walled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
    }

    void WallSlide()
    {
        if(Walled() && !Grounded() && xAxis != 0)
        {
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
            pState.lightJumping = false;
            lightBall.SetActive(false);
            UnfreezeRigidbodyPosition();
        }
        else
        {
            isWallSliding = false;
        }
    }
    void WallJump()
{
    if (isWallSliding)
    {
        isWallJumping = false;
        wallJumpingDirection = !pState.lookingRight ? 1 : -1;

        CancelInvoke(nameof(StopWallJumping));
    }

    if ((Input.GetButtonDown("Jump") || (Gamepad.current?.crossButton.wasPressedThisFrame == true)) && isWallSliding)
    {
        isWallJumping = true;
        rb.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);

        dashed = false;
        airJumpCounter = 0;
        lightJumpCounter = 0;

        if ((pState.lookingRight && transform.eulerAngles.y == 0) || (!pState.lookingRight && transform.eulerAngles.y != 0))
        {
            pState.lookingRight = !pState.lookingRight;
            int _yRotation = pState.lookingRight ? 0 : 180;

            transform.eulerAngles = new Vector2(transform.eulerAngles.x, _yRotation);
        }

        // Debug statement for wall jump
        Debug.Log("Wall jump!");
        
        Invoke(nameof(StopWallJumping), wallJumpingDuration);
    }
}

    void StopWallJumping()
    {
        isWallJumping = false;
    }

    
}

