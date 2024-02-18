using System.Collections;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController3D : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f; // Placeholder for jump force
    private int xAxis; // Placeholder for x axis movement
    private Rigidbody rb; // Placeholder for Rigidbody reference


    public static PlayerController3D Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
    }
    }


    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Assign the Rigidbody component

    }

    void Update()
    {
        // Rotate the GameObject at a 45-degree angle on the X axis
        transform.rotation = Quaternion.Euler(45f, 0f, 0f);

        Move();
    }

    private void Move()
    {
        // 3D Movement
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(horizontal, 0f, vertical).normalized;

        // Check for obstacles
        RaycastHit hit;
        bool obstacleAhead = Physics.Raycast(transform.position, moveDirection, out hit, 5f) && hit.collider.CompareTag("Obstacle");

        if (!obstacleAhead)
        {
            // Move the player
            transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);

            // Keep the player upright
            Vector3 currentRotation = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(45f, currentRotation.y, 0f);

            // Flip sprite when moving left
            if (horizontal < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else if (horizontal > 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }

            // Set "Walking" parameter in animator
            Animator anim = GetComponent<Animator>();
            anim.SetBool("Walking", moveDirection.magnitude > 0);
        }
    }

    public IEnumerator WalkIntoNewScene(Vector2 _exitDir, float _delay)
    {
        //If exit direction is upwards
        if (_exitDir.y != 0)
        {
            rb.velocity = jumpForce * _exitDir;
        }

        //If exit direction requires horizontal movement
        if (_exitDir.x != 0)
        {
            xAxis = _exitDir.x > 0 ? 1 : -1;

            Move();
        }

        yield return new WaitForSeconds(_delay);
    }
}
