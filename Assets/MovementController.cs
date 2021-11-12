using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    public Rigidbody2D rb;

    //double maxFallSpeed = -1;
    float jumpSpeed = 15;

    // Awake is called before start, when script is being loaded
    void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // read the user inputs
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
        }

        // update the game state
    }

    // called at a specific rate. Use this to jump instead of gravity?
    void FixedUpdate()
    {
        
    }
}
