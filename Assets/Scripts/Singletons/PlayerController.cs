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
    [SerializeField] private float shadowJumpForce = 60f; 
    [SerializeField] private float shadowJumpChargeSpeed;
    [SerializeField] public float shadowJumpChargeTime; 

    private int jumpBufferCounter = 0; //stores the jump button input
    [SerializeField] private int jumpBufferFrames; //sets the max amount of frames the jump buffer input is stored

    private float coyoteTimeCounter = 0; //stores the Grounded() bool
    [SerializeField] private float coyoteTime; ////sets the max amount of frames the Grounded() bool is stored

    private int airJumpCounter = 0; //keeps track of how many times the player has jumped in the air
    [SerializeField] private int maxAirJumps; //the max no. of air jumps
    [SerializeField] private int maxFallingSpeed; //the max no. of air jumps
    public float fallGravityMultiplier = 2.0f; // You can adjust this value
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
    private bool canDash = true, dashed;
    private bool canDodge = true, dodged;
    [Space(5)]



    [Header("Attack Settings:")]
    [SerializeField] private Transform SideAttackTransform; //the middle of the side attack area
    [SerializeField] private Vector2 SideAttackArea; //how large the area of side attack is
    [SerializeField] private Transform ChargeAttackTransform; //the middle of the side attack area
    [SerializeField] private Vector2 ChargeAttackArea; //how large the area of side attack is

    [SerializeField] private Transform UpAttackTransform; //the middle of the up attack area
    [SerializeField] private Vector2 UpAttackArea; //how large the area of side attack is

    [SerializeField] private Transform DownAttackTransform; //the middle of the down attack area
    [SerializeField] private Vector2 DownAttackArea; //how large the area of down attack is

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
    [SerializeField] private int recoilXSteps = 5; //how many FixedUpdates() the player recoils horizontally for
    [SerializeField] private int recoilYSteps = 5; //how many FixedUpdates() the player recoils vertically for

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
    //spell cast objects
    [SerializeField] GameObject sideSpellFireball;
    [SerializeField] GameObject upSpellExplosion;
    [SerializeField] GameObject downSpellFireball;
    [SerializeField] GameObject shadowBall;
    [SerializeField] float shadowJumpDamage;
    [SerializeField] private Transform ShadowJumpTransform; //the middle of the up attack area
    [SerializeField] private Vector2 ShadowJumpArea; //how large the area of side attack is
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
    [SerializeField] AudioClip hurtSound;
    [Space(5)]

    [Header("Reflect Settings")]
    public UnityEngine.Rendering.Universal.Light2D playerLight;
    private float timeBetweenReflect = 0.4f, timeSinceReflect;
    public bool Shielded = false;
    [SerializeField] GameObject Shield;
    [Space(5)]
    

    

    [HideInInspector] public PlayerStateList pState;
    [HideInInspector] public Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sr;
    private AudioSource audioSource;

    //Input Variables
    private float xAxis, yAxis;
    private bool attack = false;
    private bool reflect = false;
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
    public bool unlockedReflect;
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
        

        SaveData.Instance.LoadPlayerData();
        if(manaOrbs > 3)
        {
            manaOrbs = 3;
        }
        if (halfMana)
        {
            UIManager.Instance.SwitchMana(UIManager.ManaState.HalfMana);
        }

        gravity = rb.gravityScale;

        Mana = mana;
        manaStorage.fillAmount = Mana;

        //Health = maxHealth;
        Debug.Log(transform.position);
    }

    // Update is called once per frame
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(SideAttackTransform.position, SideAttackArea);
        Gizmos.DrawWireCube(ChargeAttackTransform.position, ChargeAttackArea);
        Gizmos.DrawWireCube(UpAttackTransform.position, UpAttackArea);
        Gizmos.DrawWireCube(DownAttackTransform.position, DownAttackArea);
        Gizmos.DrawWireSphere(ShadowJumpTransform.position, Mathf.Max(ShadowJumpArea.x, ShadowJumpArea.y) * 0.5f);

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

        if (pState.dashing || pState.healing) return;

        if(pState.alive)
        {
            if(!isWallJumping)
            {   
                //Flip();
                Move();
                Jump();
                ShadowJump();
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
            CastSpell();
            Reflect();
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
    private Vector2 relativeTransform;
   public bool isOnPlatform;
   public Rigidbody2D platformRb;

    private void FixedUpdate()
    {
        if (pState.cutscene) return;

        if (pState.dashing || pState.healing) return;
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
    if (platformRb != null)
    {
        // If on platform and not providing any input, match platform velocity
        rb.velocity = new Vector2(walkSpeed * xAxis + platformRb.velocity.x, rb.velocity.y);
    }
}

    }
        
    }

    private Vector2 lastInputDirection;

    private void OnTriggerEnter2D(Collider2D _other) //for up and down cast spell
    {
        if(_other.GetComponent<Enemy>() != null && pState.casting)
        {
            _other.GetComponent<Enemy>().EnemyGetsHit(spellDamage, (_other.transform.position - transform.position).normalized, -recoilYSpeed);
        }
        if(_other.GetComponent<Enemy>() != null && pState.shadowJumping)
        {
            int _recoilLeftOrRight = pState.lookingRight ? 1 : -1;
            int _recoilUpOrDown = rb.velocity.y > 0 ? 1 : -1;
            float recoilStrength = 1.0f;

            _other.GetComponent<Enemy>().EnemyGetsHit(shadowJumpDamage, (_other.transform.position - transform.position).normalized, recoilYSpeed);
            Hit(ShadowJumpTransform, ShadowJumpArea, ref pState.recoilingY, new Vector2(_recoilLeftOrRight, _recoilUpOrDown), recoilStrength);

            if (xAxis != 0 || yAxis != 0)
            {
            // If there is input, update the last input direction
            lastInputDirection = new Vector2(xAxis, yAxis).normalized;
            }

            // ...

            // When you need to use the launch direction:
            rb.velocity = -lastInputDirection * shadowJumpForce;
            
        }

    }

    void GetInputs()
    {
       xAxis = Input.GetAxisRaw("Horizontal");
       yAxis = Input.GetAxisRaw("Vertical");
       attack = Input.GetButtonDown("Attack");
       reflect = Input.GetButtonDown("Reflect");
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


    private void Move()
    {
        if(pState.shadowJumping == false)
        {
        rb.velocity = new Vector2(walkSpeed * xAxis, rb.velocity.y);
        anim.SetBool("Walking", rb.velocity.x != 0 && Grounded());
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
        if ((Input.GetButtonDown("Dash")|| (Gamepad.current?.rightTrigger.wasPressedThisFrame == true)) && canDash && !dashed && xAxis != 0)
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
         if ((Input.GetButtonDown("Dash")|| (Gamepad.current?.rightTrigger.wasPressedThisFrame == true)) && canDodge && !dodged && xAxis == 0 && Grounded())
        {
            StartCoroutine(Dodge());
            dodged = false;
        }
    }

    IEnumerator Dodge()
    {
        canDodge = false;
        canDash = false;
        pState.dashing = true;
        pState.invincible = true;
        pState.dodging = true;
        anim.SetTrigger("Dashing");
        audioSource.PlayOneShot(dashAndAttackSound);
        rb.gravityScale = 0;
        int _dir = pState.lookingRight ? 1 : -1;
        rb.velocity = new Vector2(-_dir * (dashSpeed / 3), 0);
        //if (Grounded()) Instantiate(dashEffect, transform);
        yield return new WaitForSeconds(dashTime);
        rb.gravityScale = gravity;
        pState.dodging = false;
         pState.dashing = false;
        yield return new WaitForSeconds(dashCooldown * 4);
        pState.invincible = false;
       
        canDodge = true;
        canDash = true;
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
        if (Gamepad.current?.triangleButton.wasPressedThisFrame == true && !Grounded())
        {
            pState.shadow = !pState.shadow;

            // Ensure sr is not null
            if (sr != null)
            {
                // Set the color based on the shadow state
                sr.color = pState.shadow ? Color.black : Color.white;

                // Modify walkSpeed and gravity if in shadow form
                if (pState.shadow)
                {
                    walkSpeed *= 1.35f; // Increase walkSpeed by 25%
                    rb.gravityScale *= 0.50f;
                    fallGravityMultiplier *= 0f;
                }
                else
                {
                    // Reset to original walkSpeed if not in shadow form
                    walkSpeed = originalWalkSpeed;
                    rb.gravityScale = gravity;
                    fallGravityMultiplier = originalFallGravityMultiplier;
                }
            }
        }
    }




    void Attack()
    {
    timeSinceAttck += Time.deltaTime;

    if ((attack || (Gamepad.current?.squareButton.wasPressedThisFrame == true)) && timeSinceAttck >= timeBetweenAttack)
    {
        timeSinceAttck = 0;
        anim.SetTrigger("Attacking");
        audioSource.PlayOneShot(dashAndAttackSound);

        // Handle different attack types based on input and conditions
        if (yAxis == 0 || (yAxis < 0 && Grounded()))
        {
            int _recoilLeftOrRight = pState.lookingRight ? 1 : -1;

            // Handle regular attack
            Hit(SideAttackTransform, SideAttackArea, ref pState.recoilingX, Vector2.right * _recoilLeftOrRight, recoilXSpeed);
            Instantiate(slashEffect, SideAttackTransform);
        }
        else if (yAxis > 0)
        {
            // Handle up attack
            Hit(UpAttackTransform, UpAttackArea, ref pState.recoilingY, Vector2.up, recoilYSpeed);
            SlashEffectAtAngle(slashEffect, 80, UpAttackTransform);
        }
        else if (yAxis < 0 && !Grounded())
        {
            // Handle down attack
            Hit(DownAttackTransform, DownAttackArea, ref pState.recoilingY, Vector2.down, recoilYSpeed);
            SlashEffectAtAngle(slashEffect, -90, DownAttackTransform);
        }
    }

        if ((Input.GetButton("Attack") || (Gamepad.current?.squareButton.isPressed == true)) && chargeTime <= 2)
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

        else if ((Input.GetButtonDown("Attack") || (Gamepad.current?.squareButton.wasPressedThisFrame == true)))
        {
            // If the attack button is pressed, but not held, reset chargeTime
            chargeTime = 0;
        }
        if ((Input.GetButtonUp("Attack") || (Gamepad.current?.squareButton.wasReleasedThisFrame == true)) && chargeTime < 2)
        {
            chargeTime = 0;
            
            
        }
        else if ((Input.GetButtonUp("Attack") || (Gamepad.current?.squareButton.wasReleasedThisFrame == true)) && chargeTime >= 2)
        {   
            audioSource.PlayOneShot(dashAndAttackSound);
            // Release charge if the button is released and charging duration is sufficient
            ReleaseCharge();
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

    transform.position += new Vector3(_recoilDir.x * _recoilStrength * Time.deltaTime, 0f, 0f);

    // Apply vertical recoil to the player
    // Modify the player's position based on the vertical recoil direction and strength
    transform.position += new Vector3(0f, _recoilDir.y * _recoilStrength * Time.deltaTime, 0f);
    }

    void SlashEffectAtAngle(GameObject _slashEffect, int _effectAngle, Transform _attackTransform)
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
            rb.gravityScale = 0;
            if (yAxis < 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, recoilYSpeed);
            }
            else
            {
                rb.velocity = new Vector2(rb.velocity.x, -recoilYSpeed);
            }
            airJumpCounter = 0;
        }
        else
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
    if(pState.alive && !pState.dodging && !pState.shadowJumping)
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
    bool isHealButtonPressed = Input.GetButton("Cast/Heal") || (Gamepad.current?.circleButton.isPressed == true);

    if (isHealButtonPressed)
    {
        castOrHealTimer += Time.deltaTime;
        Debug.Log("castOrHealTimer: " + castOrHealTimer);
    }

    if (castOrHealTimer > 0.5f && Health < maxHealth && Mana > 0 && !pState.jumping && !pState.dashing)
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

    void CastSpell()
    {
        if ((Input.GetButtonUp("Cast/Heal") || (Gamepad.current?.circleButton.wasReleasedThisFrame == true)) && castOrHealTimer <= 0.5f && timeSinceCast >= timeBetweenCast && Mana >= manaSpellCost)
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
            castOrHealTimer = 0;
        }

        if (Grounded())
        {
            //disable downspell if on the ground
            downSpellFireball.SetActive(false);
            shadowBall.SetActive(false);
        }
        //if down spell is active, force player down until grounded
        if(downSpellFireball.activeInHierarchy)
        {
            rb.velocity += downSpellForce * Vector2.down;
        }
    }
    IEnumerator CastCoroutine()
    {
        audioSource.PlayOneShot(spellCastSound);

        //side cast
        if ((yAxis == 0 || (yAxis < 0 && Grounded())) && unlockedSideCast)
        {
            anim.SetBool("Casting", true);
            yield return new WaitForSeconds(0.15f);
            GameObject _fireBall = Instantiate(sideSpellFireball, SideAttackTransform.position, Quaternion.identity);

            //flip fireball
            if(pState.lookingRight)
            {
                _fireBall.transform.eulerAngles = Vector3.zero; // if facing right, fireball continues as per normal
                //_fireBall.transform.eulerAngles = new Vector3(0, 0, 45);
               // _fireBall.transform.eulerAngles = new Vector3(0, 0, -45);
            }
            else
            {
                _fireBall.transform.eulerAngles = new Vector2(_fireBall.transform.eulerAngles.x, 180); 
                //if not facing right, rotate the fireball 180 deg
            }
            pState.recoilingX = true;

            Mana -= manaSpellCost;
            manaOrbsHandler.usedMana = true;
            manaOrbsHandler.countDown = 3f;
            yield return new WaitForSeconds(0.35f);
        }

        //up cast
        else if( yAxis > 0 && unlockedUpCast)
        {
            anim.SetBool("Casting", true);
            yield return new WaitForSeconds(0.15f);

            Instantiate(upSpellExplosion, transform);
            rb.velocity = Vector2.zero;

            Mana -= manaSpellCost;
            manaOrbsHandler.usedMana = true;
            manaOrbsHandler.countDown = 3f;
            yield return new WaitForSeconds(0.35f);
        }

        //down cast
        else if((yAxis < 0 && !Grounded()) && unlockedDownCast)
        {
            anim.SetBool("Casting", true);
            
            yield return new WaitForSeconds(0.15f);

            downSpellFireball.SetActive(true);

            Mana -= manaSpellCost;
            manaOrbsHandler.usedMana = true;
            manaOrbsHandler.countDown = 3f;
            yield return new WaitForSeconds(0.35f);
        }

        
        anim.SetBool("Casting", false);
        pState.casting = false;
    }

    void Reflect()
    {   
        if ((Input.GetButtonDown("Reflect") || (Gamepad.current?.leftTrigger.wasPressedThisFrame == true)) && !Shielded && timeSinceReflect >= timeBetweenReflect && unlockedReflect)
        {              
        timeSinceReflect = 0;
        audioSource.PlayOneShot(jumpSound);
        anim.SetBool("Casting", true);
        StartCoroutine(ReflectCoroutine());
        }
        else
        {
        timeSinceReflect += Time.deltaTime;
        }
    }
    IEnumerator ReflectCoroutine()
    {
        RigidbodyConstraints2D originalConstraints = rb.constraints;
        Vector3 originalPosition = rb.position;

        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
        //float originalIntensity = playerLight.intensity;
        //float originalFalloffIntensity = playerLight.falloffIntensity;
        //Color originalColor = playerLight.color;
        //Color angelicColor = new Color(1f, 0.9f, 0.7f); 
        Shield.SetActive(true);
        Shielded = true;
        
                     
        anim.SetBool("Casting", true);
            
        GameObject _lightshield = Instantiate(Shield, rb.position, Quaternion.identity);            
        if(pState.lookingRight)
        {
            _lightshield.transform.eulerAngles = Vector3.zero; 
        }
        else
        {
            _lightshield.transform.eulerAngles = new Vector2(_lightshield.transform.eulerAngles.x, 180); 
        }

        //playerLight.intensity = 50f;
        //playerLight.falloffIntensity = 0f; 
        //playerLight.color = angelicColor;

        yield return new WaitForSeconds(0.2f);

        //playerLight.intensity = originalIntensity;
        //playerLight.falloffIntensity = originalFalloffIntensity;
        //playerLight.color = originalColor;
        Shield.SetActive(false);
        Shielded = false;       
        Destroy(_lightshield);
        rb.constraints = originalConstraints;   
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
    

    private RigidbodyConstraints2D savedPositionConstraints;
     void Jump()
    {
    
    if (jumpBufferCounter > 0 && coyoteTimeCounter > 0 && !pState.jumping)
    {
        audioSource.PlayOneShot(jumpSound);

        rb.velocity = new Vector3(rb.velocity.x, jumpForce);

        pState.jumping = true;
    }

    if (!Grounded() && airJumpCounter < maxAirJumps && (Input.GetButtonDown("Jump") || (Gamepad.current?.crossButton.wasPressedThisFrame == true)) && unlockedVarJump && pState.shadow == false)
    {
        audioSource.PlayOneShot(jumpSound);

        pState.jumping = true;

        airJumpCounter++;

        rb.velocity = new Vector3(rb.velocity.x, jumpForce);
    }

    if ((Input.GetButtonUp("Jump") || (Gamepad.current?.crossButton.wasReleasedThisFrame == true)) && rb.velocity.y > 3)
    {
        pState.jumping = false;

        rb.velocity = new Vector2(rb.velocity.x, 0);
    }

    // Increase gravity while falling
    if (rb.velocity.y < 0)
    {
        rb.velocity += Vector2.up * Physics2D.gravity.y * (fallGravityMultiplier - 1) * Time.deltaTime;
    }

    // Clamp the vertical velocity to control falling speed
    rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -maxFallingSpeed, rb.velocity.y));

    anim.SetBool("Jumping", !Grounded());
}


    void ShadowJump()
    {
            Debug.Log($"Charge Time: {shadowJumpChargeTime}");

        if (!Grounded() && airJumpCounter < maxAirJumps && (Input.GetButtonDown("Jump") || (Gamepad.current?.crossButton.wasPressedThisFrame == true)) && unlockedVarJump && pState.shadow == true)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            savedPositionConstraints = rb.constraints & RigidbodyConstraints2D.FreezeRotation;

            rb.constraints = RigidbodyConstraints2D.FreezePosition;
            airJumpCounter++;
            pState.hovering = true;
        }
        if (!Grounded() && (Input.GetButton("Jump") || (Gamepad.current?.crossButton.isPressed == true)) && shadowJumpChargeTime <= 2 && pState.hovering == true)
        {
            shadowJumpChargeTime += Time.deltaTime;

            shadowJumpChargeTime = Mathf.Clamp(shadowJumpChargeTime, 0f, 2f);

            GameObject _chargeParticles = Instantiate(chargeParticles, transform.position, Quaternion.identity);

            if (shadowJumpChargeTime >= 2f)
            {
                // Double the size
                Vector3 newScale = _chargeParticles.transform.localScale * 2f;
                _chargeParticles.transform.localScale = newScale;
            }
            
            Destroy(_chargeParticles, 0.05f);
        }
        if(!Grounded() && (Input.GetButton("Jump") || (Gamepad.current?.crossButton.isPressed == true)) && pState.shadowJumping)
        {
            pState.shadowJumping = false;
            shadowBall.SetActive(false);
        }
        if ((Input.GetButtonUp("Jump") || (Gamepad.current?.crossButton.wasReleasedThisFrame == true)) && pState.hovering)
        {
            pState.shadowJumping = true;

            // Calculate the launch direction based on input.
            Vector2 launchDirection = new Vector2(xAxis, yAxis).normalized;
            shadowJumpChargeTime = 0;


            // Apply the hover launch force.
            rb.velocity = launchDirection * shadowJumpForce;
            shadowBall.SetActive(true);


            //Debug.Log($"Horizontal Input: {horizontalInput}, Vertical Input: {verticalInput}, Launch Direction: {launchDirection}");

            // Reset hovering state.
            pState.hovering = false;
            rb.constraints = savedPositionConstraints;

        }
    }
    void UpdateJumpVariables()
    {
        if (Grounded())
        { 
            if (!landingSoundPlayed && !isOnPlatform)
            {
                audioSource.PlayOneShot(landingSound);
                landingSoundPlayed = true;
            }
            pState.jumping = false;
            pState.shadowJumping = false;
            coyoteTimeCounter = coyoteTime;
            airJumpCounter = 0;
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

