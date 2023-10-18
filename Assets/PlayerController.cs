using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{   
    [Header("Horizontal Movement Settings:")]
    [SerializeField] private float walkSpeed = 1; //sets the players movement speed on the ground
    [Space(5)]
    [HideInInspector] public Rigidbody2D rb;

    private float xAxis, yAxis;

    // Start is called before the first frame update
    void Start()
    {     
          rb = GetComponent<Rigidbody2D>();

    }

    // Update is called once per frame
    void Update()
    {
         GetInputs();
         Move();
    }

    void GetInputs()
    {
       xAxis = Input.GetAxisRaw("Horizontal");
      yAxis = Input.GetAxisRaw("Vertical");
    }

    private void Move()
    {
        rb.velocity = new Vector2(walkSpeed * xAxis, rb.velocity.y);
        //anim.SetBool("Walking", rb.velocity.x != 0 && Grounded());
    }
}
