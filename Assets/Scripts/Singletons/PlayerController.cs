using System.Collections;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.InputSystem;
using static LeanTween;




public class PlayerController : MonoBehaviour
{
    [SerializeField] private int comboCounter = 0;
    [Space(5)]
    [Header("Horizontal Movement Settings:")]
    [SerializeField] private float walkSpeed = 1; //sets the players movement speed on the ground
    [SerializeField] private float airWalkSpeed = 1; //sets the players movement speed in the air 
    [SerializeField] private float aimWalkSpeed = 1;
    private float originalWalkSpeed;
    [Space(5)]



    [Header("Vertical Movement Settings")]
    [SerializeField] private float jumpForce = 45f; //sets how hight the player can jump
    private int jumpBufferCounter = 0; //stores the jump button input
    private int lightJumpBufferCounter = 0;
    [SerializeField] private int jumpBufferFrames; //sets the max amount of frames the jump buffer input is stored
    [SerializeField] private int lightJumpBufferFrames;

    private float coyoteTimeCounter = 0; //stores the Grounded() bool
    [SerializeField] private float coyoteTime; ////sets the max amount of frames the Grounded() bool is stored

    [SerializeField] private int airJumpCounter = 0; //keeps track of how many times the player has jumped in the air
    private int lightJumpCounter = 0;
    [SerializeField] private int maxAirJumps; //the max no. of air jumps
    [SerializeField] private int maxLightJumps;
    [SerializeField] private float maxFallingSpeed; //the max fallspeed
    public float fallGravityMultiplier; // You can adjust this value
    [HideInInspector] public float gravity; //stores the gravity scale at start
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
    [SerializeField] private Transform ceilingCheckPoint;
    [SerializeField] private float ceilingCheckY = 0.2f; //how far up from ceiling check point is Ceilinged() checked
    [SerializeField] private float ceilingCheckX = 0.5f; //how far horizontally from ceiling check point is Ceilinged() checked
    [SerializeField] private LayerMask whatIsGround; //sets the ground layer
    [Space(5)]



    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed; //speed of the dash
    [SerializeField] private float dashTime; //amount of time spent dashing
    [SerializeField] private float dashCooldown; //amount of time between dashes
    [SerializeField] GameObject dashEffect;
    [SerializeField] GameObject puddleFormCollider;
    private bool canDash = true, dashed;
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

    private float timeBetweenRangedAttack  = 0.2f, timeSinceRangedAttck;
    private float timeBetweenAttack = 0.4f, timeSinceAttck;
    private float timeBetweenShadowAttack = 0.6f, timeSinceShadowAttck;

    [SerializeField] private float damage; //the damage the player does to an enemy

    [SerializeField] private GameObject slashEffect; //the effect of the slashs
    [SerializeField] private GameObject chargeSlashEffect; 
    [SerializeField] private GameObject shadowSlashEffect;
    [SerializeField] private float chargeSpeed;
    [SerializeField] public float chargeTime;  // Adjust this value as needed
    bool restoreTime;
    float restoreTimeSpeed;
    [SerializeField] GameObject chargeParticles;
    [Space(5)]



    [Header("Recoil Settings:")]
    [SerializeField] private int recoilXSteps = 2; //how many FixedUpdates() the player recoils horizontally for
    [SerializeField] private int recoilYSteps = 2; //how many FixedUpdates() the player recoils vertically for

    [SerializeField] private float recoilXSpeed = 25; //the speed of horizontal recoil
    [SerializeField] private float recoilYSpeed = 25; //the speed of vertical recoil

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
    [SerializeField] float timeBetweenCast = 1f;
    [SerializeField] float spellDamage; //upspellexplosion and downspellfireball
    [SerializeField] float downSpellForce; // desolate dive only
    [SerializeField] private Transform eyeAttackTransform;
    //spell cast objects
    [SerializeField] GameObject sideSpellFireball;
    [SerializeField] GameObject blackholeBlast;
    [SerializeField] GameObject upSpellExplosion;
    [SerializeField] GameObject downSpellFireball;
    [SerializeField] GameObject downSpellShadowFireball;
    [SerializeField] GameObject shadowBloodSpray;
    [SerializeField] GameObject shadowHook;
    [SerializeField] GameObject lightBall;
    [SerializeField] GameObject lightningBow;
    [SerializeField] GameObject lightningArrow;
    [SerializeField] GameObject lightningDart;
    [SerializeField] GameObject ringOfLight;
    [SerializeField] GameObject lightningStrike;
    [SerializeField] GameObject lightningStake;
    [SerializeField] float lightJumpDamage;
    [SerializeField] private Transform LightJumpTransform; //the middle of the up attack area
    [SerializeField] private Vector2 LightJumpArea; //how large the area of side attack is
    public float timeSinceCast;
    public float castOrHealTimer;
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
    [SerializeField] AudioClip blackholeSound;
    [Space(5)]


    [Header("BlackShield Settings")]
    [SerializeField] GameObject Shield;
    [Space(5)]
    

    

    [HideInInspector] public PlayerStateList pState;
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Animator anim;
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


    public static PlayerController Instance { get; private set; }

    //unlocking 
    public bool unlockedWallJump;
    public bool unlockedDash;
    public bool unlockedEmpowered;
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
        //Debug.Log(transform.position);
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
            if(unlockedEmpowered)
            {
                BecomeEmpowered();
            }         
            Attack();
            RangedAttack();
            ShadowAttack();
            CastSpell();
            //BlackShield();
            LightningBow();
            LightRing();
            LightningStrike();
            StartShadowDash();
            EndShadowDash();
            LightningBody();
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

    public void SetPlatformRigidbody(Rigidbody2D platformRb)
    {
        this.platformRb = platformRb;
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
        if (Gamepad.current != null && Gamepad.current?.dpad.down.isPressed == true)
        {
            castOrHealTimer += Time.deltaTime;
        }
        
    }

    private RigidbodyConstraints2D savedPositionConstraints;
    public void FreezeRigidbodyPosition()
    {
        savedPositionConstraints = rb.constraints;
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
    }

    public void UnfreezeRigidbodyPosition()
    {
        rb.constraints = savedPositionConstraints | RigidbodyConstraints2D.FreezeRotation;
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
    

    public delegate void PlayerFlippedEvent(Vector3 targetOffset);
    public static event PlayerFlippedEvent OnPlayerFlipped;
    public Vector3 TargetOffset { get; private set; }

    void Flip()
    {
        float targetOffset = CameraManager.Instance.xOffset;

        if (xAxis < 0 && pState.lookingRight)
        {
            transform.eulerAngles = new Vector2(0, 180);
            pState.lookingRight = false;

            // Check if the camera is not panning before starting the camera offset lerp
            if (!CameraManager.Instance.IsPanning)
            {
                StartCoroutine(CameraManager.Instance.LerpCameraOffset(new Vector3(targetOffset, 0, 0)));
                OnPlayerFlipped?.Invoke(new Vector3(targetOffset, 0, 0));
            }
        }
        else if (xAxis > 0 && !pState.lookingRight)
        {
            transform.eulerAngles = new Vector2(0, 0);
            pState.lookingRight = true;

            // Check if the camera is not panning before starting the camera offset lerp
            if (!CameraManager.Instance.IsPanning)
            {
                StartCoroutine(CameraManager.Instance.LerpCameraOffset(new Vector3(targetOffset, 0, 0)));
                OnPlayerFlipped?.Invoke(new Vector3(targetOffset, 0, 0));
            }
        }
    }





    private float walkTimer = 0f;
    private float maxWalkTimer = 1f;

    private void Move()
    {
        float currentWalkSpeed = pState.aiming ? aimWalkSpeed : walkSpeed;

        if (!pState.lightJumping && !pState.shadowHooking && !pState.lightningBody)
        {
            if (Grounded() )
            {
                // Grounded movement
                rb.velocity = new Vector2(currentWalkSpeed * xAxis, rb.velocity.y);

                if (rb.velocity.x != 0)
                {
                    walkTimer += Time.deltaTime;
                    walkTimer = Mathf.Clamp(walkTimer, 0f, maxWalkTimer);

                    if (walkTimer >= 0.24f)
                    {
                        anim.SetBool("Running", false);
                        anim.SetBool("Walking", true);
                    }
                    else
                    {
                        anim.SetBool("Running", true);
                        anim.SetBool("Walking", false);
                    }
                }
                else
                {
                    walkTimer = 0;
                    anim.SetBool("Running", false);
                    anim.SetBool("Walking", false);
                }
            }
            else
            {
                // Airborne movement
                    
                rb.velocity = new Vector2(airWalkSpeed * xAxis, rb.velocity.y);

                // Set animation states for airborne movement
                anim.SetBool("Running", false);
                anim.SetBool("Walking", false);
            }
        }
    }




    void UpdateCameraYDampForPlayerFall()
    {
        //if falling past a certain speed threshold
        if (rb.velocity.y < playerFallSpeedThreshold && !CameraManager.Instance.isLerpingYDamping && !CameraManager.Instance.hasLerpedYDamping)
        {
            StartCoroutine(CameraManager.Instance.LerpYDamping(true)); // Correct
            
        }
        //if standing stil or moving up
        if(rb.velocity.y >= 0 && !CameraManager.Instance.isLerpingYDamping && CameraManager.Instance.hasLerpedYDamping)
        {
            //reset camera function
            CameraManager.Instance.hasLerpedYDamping = false;
            StartCoroutine(CameraManager.Instance.LerpYDamping(false)); // Correct
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
        rb.velocity = Vector2.zero;
        pState.dashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
    public float returnSpeed = 0.2f; // Adjust the return speed as needed

    public void ReturnLightDarts(float delay)
    {
        GameObject[] lightDarts = GameObject.FindGameObjectsWithTag("LightDart");

        foreach (GameObject dart in lightDarts)
        {
            LightDart lightDart = dart.GetComponent<LightDart>();
            if (lightDart != null)
            {
                lightDart.canDetach = true;
                StartCoroutine(ReturnDart(dart, delay));
            }
        }
    }


    private IEnumerator ReturnDart(GameObject dart, float delay)
    {
        // Unparent the dart
        dart.transform.parent = null;

        float duration = 1f; // Default duration
        float returnSpeed = 250f; // Speed at which the dart returns (units per second)

        // Calculate the distance between the dart and the player
        float distanceToPlayer = Vector3.Distance(dart.transform.position, transform.position);

        // Calculate the duration based on the distance and return speed
        if (returnSpeed > 0f)
        {
            duration = distanceToPlayer / returnSpeed;
        }

        float elapsedTime = 0f;
        Vector3 startPosition = dart.transform.position;
        Vector3 targetPosition = transform.position;

        while (elapsedTime < duration)
        {
            // Calculate lerp progress
            float t = elapsedTime / duration;

            // Lerp the dart's position towards the player's current position
            dart.transform.position = Vector3.Lerp(startPosition, targetPosition, t);

            // Update the player's position each frame
            targetPosition = transform.position;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Set the dart's position to the player's position
        dart.transform.position = transform.position;

        // Destroy the dart
        Destroy(dart);
    }



    void BecomeEmpowered()
    {
        if ((Input.GetButtonDown("Dash")|| (Gamepad.current?.leftTrigger.wasPressedThisFrame == true)) && pState.lightForm && !pState.empoweredLight)
        {
            StartCoroutine(LightFormEngorgement());
        }
    }

    IEnumerator LightFormEngorgement()
    {
        pState.empoweredLight = true;
        yield return new WaitForSeconds(5f);
        pState.empoweredLight = false;
        ReturnLightDarts(returnSpeed); 
    }
    
    private bool isShadowDashing = false;
    private bool canStartShadowDash = true;
    void StartShadowDash()
    {
        if (canStartShadowDash && (Input.GetButtonDown("Dash") || (Gamepad.current?.rightTrigger.wasPressedThisFrame == true)) && pState.shadowForm && !pState.aiming)
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
        walkSpeed = 60f;
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

                // Modify walkSpeed and gravity based on the current form
                if (pState.lightForm)
                {   
                    anim.SetBool("ShadowForm", false);
                    walkSpeed = 60f; 
                    airWalkSpeed = 40f;
                    jumpForce = 120f;
                    maxFallingSpeed = 90f;
                    anim.SetBool("LightForm", true);
                    shadowHook.SetActive(false);
                }
                else if (pState.shadowForm)
                {   
                    anim.SetBool("LightForm", false);
                    walkSpeed = 50f;
                    airWalkSpeed = 40f;
                    jumpForce = 120f;
                    maxFallingSpeed = 90f;
                    anim.SetBool("ShadowForm", true);
                    shadowHook.SetActive(true);
                }
            }
        }
    }

    void RangedAttack()
    {
        timeSinceRangedAttck += Time.deltaTime;

        if ((attack || (Gamepad.current?.squareButton.wasPressedThisFrame == true)) && timeSinceRangedAttck >= timeBetweenRangedAttack && pState.empoweredLight && !pState.shadowForm)
        {
            // Attack logic
            timeSinceRangedAttck = 0;
            //anim.SetTrigger("RangedAttacking");
            audioSource.PlayOneShot(dashAndAttackSound);

            if (yAxis == 0 || (yAxis < 0 && Grounded()) && !pState.shadowForm)
            {
                // Handle regular attack
                int _recoilLeftOrRight = pState.lookingRight ? 1 : -1;
                GameObject _lightDart = Instantiate(lightningDart, transform.position, Quaternion.identity);

                if (pState.lookingRight)
                {
                    _lightDart.transform.eulerAngles = Vector3.zero;
                }
                else
                {
                    _lightDart.transform.eulerAngles = new Vector3(0, 180, 0); 
                }
            }
            else
            {
                // Shoot in 8 directions without moving
                Vector3 shootDirection = new Vector3(xAxis, yAxis, 0).normalized;
                float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
                angle = SnapAngle(angle);

                GameObject _lightDart = Instantiate(lightningDart, transform.position, Quaternion.Euler(new Vector3(0, 0, angle)));

                // Reset horizontal movement
                rb.velocity = new Vector2(0, rb.velocity.y);
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

    float SnapAngle(float angle)
    {
        // Snap angle to 0, 45, 90, 135, 180, 225, 270, 315 degrees
        float[] snapAngles = { 0, 45, 90, 135, 180, 225, 270, 315 };
        float minDifference = Mathf.Abs(angle - snapAngles[0]);
        float snapAngle = snapAngles[0];

        foreach (float snap in snapAngles)
        {
            float diff = Mathf.Abs(angle - snap);
            if (diff < minDifference)
            {
                minDifference = diff;
                snapAngle = snap;
            }
        }

        return snapAngle;
    }



    void Attack()
    {
        timeSinceAttck += Time.deltaTime;

        if ((attack || (Gamepad.current?.squareButton.wasPressedThisFrame == true)) && timeSinceAttck >= timeBetweenAttack && !pState.empoweredLight && !pState.shadowForm)
        {
            // Attack logic
            timeSinceAttck = 0;
            anim.SetTrigger("Attacking");
            audioSource.PlayOneShot(dashAndAttackSound);

            if (yAxis == 0 || (yAxis < 0 && Grounded()) && !pState.shadowForm)
            {
                // Handle regular attack
                int _recoilLeftOrRight = pState.lookingRight ? 1 : -1;
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

        if ((Input.GetButton("Attack") || (Gamepad.current?.squareButton.isPressed == true)) && chargeTime <= 3 && !pState.shadowForm)
        {
            chargeTime += Time.deltaTime * chargeSpeed;
            chargeTime = Mathf.Clamp(chargeTime, 0f, 3f);
            if (chargeTime >= 1.5f)
            {
                GameObject _chargeParticles = Instantiate(chargeParticles, transform.position, Quaternion.identity, transform);
                if (chargeTime >= 3f)
                {
                    // Double the size
                    Vector3 newScale = _chargeParticles.transform.localScale * 2f;
                    _chargeParticles.transform.localScale = newScale;
                }

                Destroy(_chargeParticles, 0.2f);
            }
        }
        else if ((Input.GetButtonDown("Attack") || (Gamepad.current?.squareButton.wasPressedThisFrame == true)) && !pState.shadowForm)
        {
            // If the attack button is pressed, but not held, reset chargeTime
            chargeTime = 0;
        }

        if ((Input.GetButtonUp("Attack") || (Gamepad.current?.squareButton.wasReleasedThisFrame == true)) && chargeTime < 3 && !pState.shadowForm)
        {
            chargeTime = 0;
        }
        else if ((Input.GetButtonUp("Attack") || (Gamepad.current?.squareButton.wasReleasedThisFrame == true)) && chargeTime >= 3 && !pState.shadowForm)
        {
            audioSource.PlayOneShot(dashAndAttackSound);
            // Release charge if the button is released and charging duration is sufficient
            ReleaseCharge();
        }
    }      

    private float quickAttackWindow = 1f; // Time window to consider attacks as quick

    void ShadowAttack()
    {
        timeSinceShadowAttck += Time.deltaTime;
        if (timeSinceShadowAttck >= quickAttackWindow)
        {
            comboCounter = 0;
        }
        if ((attack || (Gamepad.current?.squareButton.wasPressedThisFrame == true)) && timeSinceShadowAttck >= timeBetweenShadowAttack && pState.shadowForm)
        {
            if (timeSinceShadowAttck <= quickAttackWindow)
            {
                comboCounter++;
            }
            else
            {
                comboCounter++;
            }
            timeSinceShadowAttck = 0;
            audioSource.PlayOneShot(dashAndAttackSound);

            // Handle different attack types based on input and conditions
            if (yAxis == 0 || (yAxis < 0 && Grounded()) && !pState.lightForm)
            {
                int _recoilLeftOrRight = pState.lookingRight ? 1 : -1;

                ShadowHit(ShadowSideAttackTransform, ShadowSideAttackArea, ref pState.recoilingX, Vector2.right * _recoilLeftOrRight, recoilXSpeed);
                Instantiate(shadowSlashEffect, ShadowSideAttackTransform);

                // Set animation based on comboCounter
                if (comboCounter == 1)
                {
                    //anim.SetTrigger("Attack1");
                }
                else if (comboCounter == 2)
                {
                    //anim.SetTrigger("Attack2");
                }
                else if (comboCounter == 3)
                {
                    //anim.SetTrigger("Attack3");
                    comboCounter = 0; // Reset comboCounter after the 3rd attack
                }
            }

            // Handle up and down attacks
            if (yAxis > 0 && pState.shadowForm)
            {
                // Handle up attack
                ShadowHit(ShadowUpAttackTransform, ShadowUpAttackArea, ref pState.recoilingY, Vector2.up, recoilYSpeed);
                ShadowSlashEffectAtAngle(shadowSlashEffect, 80, ShadowUpAttackTransform);

                // Set animation based on comboCounter
                if (comboCounter == 1)
                {
                    //anim.SetTrigger("upAttack1");
                }
                else if (comboCounter == 2)
                {
                    //anim.SetTrigger("upAttack2");
                }
                else if (comboCounter == 3)
                {
                    //anim.SetTrigger("upAttack3");
                    comboCounter = 0; // Reset comboCounter after the 3rd attack
                }
            }
            else if (yAxis < 0 && !Grounded() && pState.shadowForm)
            {           
                // Handle down attack
                //anim.SetTrigger("downAttack1");
                ShadowHit(ShadowDownAttackTransform, ShadowDownAttackArea, ref pState.recoilingY, Vector2.down, recoilYSpeed);
                ShadowSlashEffectAtAngle(shadowSlashEffect, -90, ShadowDownAttackTransform);
                comboCounter = 0;
            }

            // Adjust time between attacks if in a quick combo
            if (comboCounter == 2)
            {
                timeBetweenShadowAttack = 0.3f;
            }
            else
            {
                timeBetweenShadowAttack = 0.6f;
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
            if (objectsToHit[i].CompareTag("Enemy"))
            {
                if (objectsToHit[i].GetComponent<Enemy>() != null)
                {
                    objectsToHit[i].GetComponent<Enemy>().EnemyGetsHit(damage, _recoilDir, _recoilStrength);
                }
                if (Mana < 1)
                {
                    Mana += manaGain;
                }

                if (Mana >= 1 || (halfMana && Mana >= 0.5))
                {
                    manaOrbsHandler.UpdateMana(manaGain * 3);
                }
            }
            if (objectsToHit[i].CompareTag("Illusion"))
            {
                // Destroy the illusionary wall when hit
                Destroy(objectsToHit[i].gameObject);
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

    public float castingRecoilMultiplier = 4f;

    void Recoil()
    {
        if (pState.recoilingX)
        {
            float currentRecoilXSpeed = recoilXSpeed;

            // Check if casting blackhole blast for increased recoil
            if (pState.casting && castingRecoilMultiplier > 0)
            {
                currentRecoilXSpeed *= castingRecoilMultiplier;
            }

            if (pState.lookingRight)
            {
                rb.velocity = new Vector2(-currentRecoilXSpeed, 0);
            }
            else
            {
                rb.velocity = new Vector2(currentRecoilXSpeed, 0);
            }
        }

        if (pState.recoilingY)
        {
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

    void LightningBody()
    {
        // Assuming your player's collider is called "mainCollider"
        Collider2D mainCollider = GetComponent<Collider2D>();

        // Get the layer index of the "Attackable" layer
        int attackableLayerIndex = LayerMask.NameToLayer("Attackable");

        if (pState.lightningBody || pState.lightJumping)
        {
            // Ignore collisions between the mainCollider and the "Attackable" layer
            Physics2D.IgnoreLayerCollision(mainCollider.gameObject.layer, attackableLayerIndex, true);
        }
        else
        {
            // Reset collision ignoring when lightningBody is false
            Physics2D.IgnoreLayerCollision(mainCollider.gameObject.layer, attackableLayerIndex, false);
        }
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
        if (Input.GetButton("Cast/Heal") || (Gamepad.current?.dpad.down.isPressed == true))
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
        if ((Input.GetButton("Cast/Heal") || (Gamepad.current?.dpad.down.isPressed == true)) && castOrHealTimer > 0.5f && Health < maxHealth && Mana > 0 && !pState.jumping && !pState.dashing)
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
        if ((Input.GetButtonDown("Cast/Heal") || (Gamepad.current?.circleButton.wasPressedThisFrame == true)) && timeSinceCast >= timeBetweenCast && Mana >= manaSpellCost && pState.lightForm && yAxis > 0 && unlockedUpCast && Grounded())
        {
            pState.aiming = true;
            lightningBow.SetActive(true);
            timeSinceCast = 0;
            anim.SetBool("Casting", true);
        }
        else
        {
            timeSinceCast += Time.deltaTime;
        }

        if ((Input.GetButtonUp("Cast/Heal") || (Gamepad.current?.circleButton.wasReleasedThisFrame == true)) && Mana >= manaSpellCost && pState.lightForm && pState.aiming)
        {
            
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

    void LightRing()
    {  
        if ((Input.GetButtonUp("Cast/Heal") || (Gamepad.current?.circleButton.wasReleasedThisFrame == true)) && !pState.aiming && timeSinceCast >= timeBetweenCast 
        && Mana >= manaSpellCost / 3.0f && pState.lightForm && yAxis == 0 && unlockedSideCast)
        {
            
            timeSinceCast = 0;
            StartCoroutine(LightRingCoroutine());
        }
        else
        {
              timeSinceCast += Time.deltaTime;
        }
    }

    public float throwSpeed = 100f; // Adjust as needed
    public float throwDistance = 20f; // Adjust as needed
    public float throwLerpTime = 1f; 
    IEnumerator LightRingCoroutine()
    {
        if (yAxis == 0 && unlockedSideCast)
        {
            pState.casting = true;
            anim.SetBool("Casting", true);
            Mana -= manaSpellCost / 3f;
            manaOrbsHandler.usedMana = true;
            manaOrbsHandler.countDown = 3f;

            // Store the initial position of the object and the player
            Vector3 initialPosition = transform.position;
            Vector3 playerPosition = transform.position;

            // Instantiate the object
            GameObject _ringOfLight = Instantiate(ringOfLight, initialPosition, Quaternion.identity);

            // Calculate the direction to move the object
            Vector3 direction = transform.right;

            // Shoot the object using LeanTween
            LeanTween.move(_ringOfLight, initialPosition + direction * throwDistance, throwLerpTime)
                .setEase(LeanTweenType.easeOutQuad)
                .setOnUpdate((Vector3 val) =>
                {
                    // Update the player's position continuously
                    playerPosition = transform.position;
                });

            // Wait for the object to finish moving
            yield return new WaitForSeconds(throwLerpTime);

            // Stop the object
            Rigidbody2D rb = _ringOfLight.GetComponent<Rigidbody2D>();
            rb.velocity = Vector2.zero;

            // Pause for a brief moment
            yield return new WaitForSeconds(0.5f);

            // Move the object back towards the player
            while (Vector3.Distance(playerPosition, _ringOfLight.transform.position) > 2f)
            {
                direction = (playerPosition - _ringOfLight.transform.position).normalized;
                rb.velocity = direction * throwSpeed;

                // Update the player's position continuously
                playerPosition = transform.position;

                yield return null;
            }

            // Destroy the object
            Destroy(_ringOfLight);
            
        }
        else
        {
            timeSinceCast += Time.deltaTime;
        }

        // Reset casting state
        anim.SetBool("Casting", false);
        castOrHealTimer = 0;
        pState.casting = false;
    }






   
    void LightningStrike()
    {
        if ((Input.GetButtonUp("Cast/Heal") || (Gamepad.current?.circleButton.wasReleasedThisFrame == true)) && !pState.aiming
        && Mana >= manaSpellCost / 3.0f && pState.lightForm && !pState.casting && !pState.lightningBody)
        {
            if ((yAxis < 0 && Grounded()) && unlockedDownCast)
            {
                StartCoroutine(LightningStrikeHit());
                timeSinceCast = 0;
            }
            if ((yAxis < 0 && !Grounded()) && unlockedDownCast)
            {
                StartCoroutine(AirLightningStrikeHit());
                timeSinceCast = 0;
            }
            else
            {
            timeSinceCast += Time.deltaTime;
            }
        }
    }

    IEnumerator LightningStrikeHit()
    {
        anim.SetBool("Casting", true);
        pState.casting = true;
        pState.lightningBody = true;
        
        // Add velocity upwards and to the right
        if (pState.lookingRight)
        {
        rb.velocity = new Vector2(0.4f, 0.6f) * jumpForce * 5;
        }
        // Add velocity upwards and to the left
        if (!pState.lookingRight)
        {
        rb.velocity = new Vector2(-0.4f, 0.6f) * jumpForce * 5;
        }


        yield return new WaitForSeconds(0.15f);

  
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0;

        yield return new WaitForSeconds(0.5f);

        audioSource.PlayOneShot(spellCastSound);
        GameObject Stake = Instantiate(lightningStake, transform.position, Quaternion.Euler(0, 0, -90));
        yield return new WaitForSeconds(0.1f);

        int numStakes = 3; // Number of stakes to instantiate
        float spacing = 5f; // Spacing between each stake
        float offsetMultiplier = 2f; // Offset multiplier for spacing
        float timeBetweenStakes = 0.1f; // Time between each stake spawning

        // Instantiate lightning stakes to the left and right of the player
        for (int i = 0; i < numStakes; i++)
        {
            // Instantiate left stakes
            Vector3 leftOffset = Vector3.left * spacing * (i + 1) * offsetMultiplier;
            GameObject leftStake = Instantiate(lightningStake, transform.position + leftOffset, Quaternion.Euler(0, 0, -90));

            // Instantiate right stakes
            Vector3 rightOffset = Vector3.right * spacing * (i + 1) * offsetMultiplier;
            GameObject rightStake = Instantiate(lightningStake, transform.position + rightOffset, Quaternion.Euler(0, 0, -90));

            // Wait for the specified time between each stake
            yield return new WaitForSeconds(timeBetweenStakes);
        }

        //yield return new WaitForSeconds(0.3f);
        rb.gravityScale = gravity;
        downSpellFireball.SetActive(true);

        Mana -= manaSpellCost;
        manaOrbsHandler.usedMana = true;
        manaOrbsHandler.countDown = 3f;

        // Add a loop to check if the player is grounded and end the coroutine if grounded
        while (!Grounded())
        {
            rb.velocity += walkSpeed * 5 * Vector2.down;
            pState.invincible = true;
            yield return null; // Wait for the next frame
        }

        lightningStrike.SetActive(true);

        //audioSource.PlayOneShot(spellCastSound);
        rb.velocity += walkSpeed * 5 * Vector2.down;
        pState.lightningBody = false;
        yield return new WaitForSeconds(0.5f);
        pState.invincible = false;

        anim.SetBool("Casting", false);

        lightningStrike.SetActive(false);

        yield return new WaitForSeconds(1.5f);
        pState.casting = false;
    }

    


    IEnumerator AirLightningStrikeHit()
    {
        anim.SetBool("Casting", true);
        pState.casting = true;
        pState.lightningBody = true;
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0;

        yield return new WaitForSeconds(0.5f);

        audioSource.PlayOneShot(spellCastSound);

        //yield return new WaitForSeconds(0.3f);
        rb.gravityScale = gravity;
        downSpellFireball.SetActive(true);

        Mana -= manaSpellCost;
        manaOrbsHandler.usedMana = true;
        manaOrbsHandler.countDown = 3f;

        // Add a loop to check if the player is grounded and end the coroutine if grounded
        while (!Grounded())
        {
            rb.velocity += walkSpeed * 5 * Vector2.down;
            pState.invincible = true;
            yield return null; // Wait for the next frame
        }

        GameObject Stake = Instantiate(lightningStake, transform.position, Quaternion.Euler(0, 0, -90));
        yield return new WaitForSeconds(0.1f);
        lightningStrike.SetActive(true);

        int numStakes = 3; // Number of stakes to instantiate
        float spacing = 5f; // Spacing between each stake
        float offsetMultiplier = 2f; // Offset multiplier for spacing
        float timeBetweenStakes = 0.1f; // Time between each stake spawning

        // Instantiate lightning stakes to the left and right of the player
        for (int i = 0; i < numStakes; i++)
        {
            // Instantiate left stakes
            Vector3 leftOffset = Vector3.left * spacing * (i + 1) * offsetMultiplier;
            GameObject leftStake = Instantiate(lightningStake, transform.position + leftOffset, Quaternion.Euler(0, 0, -90));

            // Instantiate right stakes
            Vector3 rightOffset = Vector3.right * spacing * (i + 1) * offsetMultiplier;
            GameObject rightStake = Instantiate(lightningStake, transform.position + rightOffset, Quaternion.Euler(0, 0, -90));

            // Wait for the specified time between each stake
            yield return new WaitForSeconds(timeBetweenStakes);
        }

        //audioSource.PlayOneShot(spellCastSound);
        rb.velocity += walkSpeed * 5 * Vector2.down;
        pState.lightningBody = false;
        yield return new WaitForSeconds(0.5f);
        pState.invincible = false;

        anim.SetBool("Casting", false);

        lightningStrike.SetActive(false);

        yield return new WaitForSeconds(1.5f);
        pState.casting = false;
    }



    
    void CastSpell()
    {
        if ((Input.GetButtonUp("Cast/Heal") || (Gamepad.current?.circleButton.wasReleasedThisFrame == true)) && timeSinceCast >= timeBetweenCast && Mana >= manaSpellCost && pState.shadowForm)
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
        }
        //if down spell is active, force player down until grounded
        if(downSpellFireball.activeInHierarchy)
        {
            //rb.velocity += downSpellForce * Vector2.down;
        }
        if(downSpellShadowFireball.activeInHierarchy)
        {
            rb.velocity += (downSpellForce * 0.05f) * Vector2.up;
        }
    }
    IEnumerator CastCoroutine()
    {   
        if (yAxis == 0 && unlockedSideCast)
        {
            anim.SetBool("Casting", true);
            pState.casting = true;
            yield return new WaitForSeconds(0.15f);
            audioSource.PlayOneShot(blackholeSound);
            GameObject _blackholeBlast = Instantiate(blackholeBlast, eyeAttackTransform.position, Quaternion.identity);
            rb.gravityScale = 0;
            pState.recoilingX = true;
            int _dir = pState.lookingRight ? 1 : -1;
            rb.velocity += new Vector2(-_dir * dashSpeed, 0);

            // Apply recoil logic here
            if (pState.lookingRight)
            {
                rb.velocity = new Vector2(-recoilXSpeed, 0);
            }
            else
            {
                rb.velocity = new Vector2(recoilXSpeed, 0);
            }

            if (pState.lookingRight)
            {
                _blackholeBlast.transform.eulerAngles = Vector3.zero;
               
            }
            else
            {
                _blackholeBlast.transform.eulerAngles =  new Vector2(_blackholeBlast.transform.eulerAngles.x, 180);
            }
            yield return new WaitForSeconds(0.15f);
            rb.gravityScale = gravity;
            Mana -= manaSpellCost;
            manaOrbsHandler.usedMana = true;
            manaOrbsHandler.countDown = 3f;
            pState.casting = false;
        }     
        if (yAxis > 0 && unlockedUpCast)
        {
            anim.SetBool("Casting", true);
            pState.casting = true;
            yield return new WaitForSeconds(0.15f);

            Instantiate(upSpellExplosion, transform);
            audioSource.PlayOneShot(ExplosionSpellCastSound);
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0;
            float startTime = Time.time;
            while (Time.time - startTime < 0.3f)
            {
                rb.velocity += (walkSpeed / 24) * Vector2.up;
                yield return null;
            }
            rb.gravityScale = gravity;
            Mana -= manaSpellCost;
            manaOrbsHandler.usedMana = true;
            manaOrbsHandler.countDown = 3f;

            yield return new WaitForSeconds(0.8f - (Time.time - startTime));
            pState.casting = false;
        }
        else if(yAxis < 0 && unlockedDownCast)
        {
            anim.SetBool("Casting", true);
            pState.casting = true;
            yield return new WaitForSeconds(0.15f);

            // Small upward and forward movement
            float upwardDuration = 0.1f;
            float upwardSpeed = 50f;
            float forwardSpeed = 50f; // Adjust the forward speed here
            float upwardElapsed = 0f;
            while (upwardElapsed < upwardDuration)
            {
                upwardElapsed += Time.deltaTime;
                rb.velocity = new Vector2(pState.lookingRight ? forwardSpeed : -forwardSpeed, upwardSpeed);
                yield return null;
            }

            // Shooting in the direction the player is facing
            float shootingSpeed = 250f; // Adjust the shooting speed here
            rb.velocity = new Vector2(pState.lookingRight ? shootingSpeed : -shootingSpeed, rb.velocity.y);

            audioSource.PlayOneShot(ExplosionSpellCastSound);
            Mana -= manaSpellCost;
            manaOrbsHandler.usedMana = true;
            manaOrbsHandler.countDown = 3f;

            // Continue moving forward until grounded, walled, or ceilinged
            while (!Grounded() && !Walled() && !Ceilinged())
            {
                rb.velocity = new Vector2(pState.lookingRight ? shootingSpeed : -shootingSpeed, rb.velocity.y);
                yield return null;
            }

            pState.casting = false;
        }
        anim.SetBool("Casting", false);
        
        //castOrHealTimer = 0;
    }


    

    //void BlackShield()
    //{   
        //if ((Input.GetButtonDown("Shield") || (Gamepad.current?.leftTrigger.wasPressedThisFrame == true)) && !Shielded && unlockedBlackShield && pState.shadowForm && Grounded())
        //{              
            //audioSource.PlayOneShot(jumpSound);
            //anim.SetBool("Casting", true);
            //Shield.SetActive(true);
            //Shielded = true; 
            //FreezeRigidbodyPosition();          
        //}
        //else if ((Input.GetButtonUp("Shield") || (Gamepad.current?.leftTrigger.wasReleasedThisFrame == true)) && Shielded && unlockedBlackShield && pState.shadowForm) 
        //{   
            //anim.SetBool("Casting", false);
            //Shield.SetActive(false);
            //Shielded = false;  
            //UnfreezeRigidbodyPosition();  
        //}
    //}
    
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

    public bool Ceilinged()
    {
        bool ceilinged = Physics2D.Raycast(ceilingCheckPoint.position, Vector2.up, ceilingCheckY, whatIsGround)
                    || Physics2D.Raycast(ceilingCheckPoint.position + new Vector3(ceilingCheckX, 0, 0), Vector2.up, ceilingCheckY, whatIsGround)
                    || Physics2D.Raycast(ceilingCheckPoint.position + new Vector3(-ceilingCheckX, 0, 0), Vector2.up, ceilingCheckY, whatIsGround);

        if (ceilinged)
        {
            Debug.DrawRay(ceilingCheckPoint.position, Vector2.up * ceilingCheckY, Color.green); // Visualize the first raycast
            Debug.DrawRay(ceilingCheckPoint.position + new Vector3(ceilingCheckX, 0, 0), Vector2.up * ceilingCheckY, Color.green); // Visualize the second raycast
            Debug.DrawRay(ceilingCheckPoint.position + new Vector3(-ceilingCheckX, 0, 0), Vector2.up * ceilingCheckY, Color.green); // Visualize the third raycast
        }
        else
        {
            Debug.DrawRay(ceilingCheckPoint.position, Vector2.up * ceilingCheckY, Color.yellow); // Visualize the first raycast
            Debug.DrawRay(ceilingCheckPoint.position + new Vector3(ceilingCheckX, 0, 0), Vector2.up * ceilingCheckY, Color.red); // Visualize the second raycast
            Debug.DrawRay(ceilingCheckPoint.position + new Vector3(-ceilingCheckX, 0, 0), Vector2.up * ceilingCheckY, Color.red); // Visualize the third raycast
        }

        return ceilinged;
    }


    
    void Jump()
    {    

        if (jumpBufferCounter > 0 && coyoteTimeCounter > 0 && !pState.jumping && !pState.lightningBody)
        {
            pState.jumping = true;
        
            //audioSource.PlayOneShot(jumpSound);

            rb.velocity = new Vector2(rb.velocity.x, jumpForce);  
            StartCoroutine(LerpAirWalkSpeed(25, 50, 0.4f));      
        }

        if (!Grounded() && airJumpCounter < maxAirJumps && ((Input.GetButtonDown("Jump") || (Gamepad.current?.crossButton.wasPressedThisFrame == true)) && unlockedVarJump && !pState.lightningBody))
        {
            //audioSource.PlayOneShot(jumpSound);

            pState.jumping = true;

            airJumpCounter++;

            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
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
    
    private IEnumerator LerpAirWalkSpeed(float startValue, float endValue, float duration)
{
    float timer = 0f;
    while (timer < duration)
    {
        float t = timer / duration;
        airWalkSpeed = Mathf.Lerp(startValue, endValue, t);
        timer += Time.deltaTime;
        yield return null;
    }
    airWalkSpeed = endValue;
}

    // Add a variable to track the last light jump direction
    private Vector2 lastLightJumpDirection = Vector2.zero;

    void LightJump()
    {
        if (!Grounded() && lightJumpCounter < maxLightJumps && lightJumpBufferCounter > 0 && unlockedVarJump && pState.lightForm && pState.empoweredLight && !pState.lightJumping && !isWallJumping && !Walled())
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
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0;
            yield return new WaitForSeconds(0.15f);
            audioSource.PlayOneShot(dashAndAttackSound);

            lightBall.SetActive(true);
            // Assuming the light ball's collider is on the "LightBallCollider" layer
            Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Attackable"), true);
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("LightBallCollider"), LayerMask.NameToLayer("Attackable"), false);

            anim.SetTrigger("Dashing");
            

            rb.gravityScale = 0;

            float distanceToTravel = 40f; // Adjust this value as needed
            float timeToTravel = 0.15f; // Adjust this value as needed
            Vector2 targetPosition = (Vector2)transform.position + (launchDirection * distanceToTravel);
            float startTime = Time.time;
            float elapsedTime = 0f;

            while (elapsedTime < timeToTravel)
            {
                elapsedTime = Time.time - startTime;
                float t = Mathf.Clamp01(elapsedTime / timeToTravel);
                rb.MovePosition(Vector2.Lerp(transform.position, targetPosition, t));
                yield return null;
            }

            rb.gravityScale = 0;
            rb.velocity = Vector2.zero;
            yield return new WaitForSeconds(0.15f);
            rb.velocity = Vector2.zero;
            lightBall.SetActive(false);
            Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Attackable"), false);
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("LightBallCollider"), LayerMask.NameToLayer("Attackable"), false);
            pState.lightJumping = false;
            rb.gravityScale = gravity;
            lastLightJumpDirection = launchDirection;
        }
    }





    void UpdateJumpVariables()
    {
        if (Grounded())
        { 
            if (!landingSoundPlayed)
            {
                audioSource.PlayOneShot(landingSound);
                landingSoundPlayed = true;
            }
            pState.jumping = false;
            pState.lightJumping = false;
            coyoteTimeCounter = coyoteTime;
            airJumpCounter = 0;
            lightJumpCounter = 0;
            lightJumpBufferCounter = 0;
            lightBall.SetActive(false);
            
        }
        if (!Grounded())
        {
            landingSoundPlayed = false;
        }
        if (Ceilinged())
        {
            lightBall.SetActive(false);
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
            //landingSoundPlayed = false;
        }

        if ((Input.GetButtonDown("Jump") || (Gamepad.current?.crossButton.wasPressedThisFrame == true)))
        {
            jumpBufferCounter = jumpBufferFrames;
        }
        if ((Input.GetButtonDown("Jump") || (Gamepad.current?.crossButton.wasPressedThisFrame == true)) && pState.lightForm && unlockedVarJump && !Grounded() && coyoteTimeCounter <= 0)
        {
            lightJumpBufferCounter = lightJumpBufferFrames;
        }
        else
        {
            jumpBufferCounter--;
            lightJumpBufferCounter--;
            
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
            lightJumpBufferCounter = 0;
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

