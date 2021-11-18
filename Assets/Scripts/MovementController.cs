using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    public Rigidbody2D rb;

    //double maxFallSpeed = -1;
    float jumpSpeed = 15;
    float walkSpeed = 7;
    bool dodging = false;
    int dodgeTime = 0;
    int totalDodgeLength = 5;
    float dodgeSpeed = 15;

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
        Vector2 newVel = new Vector2(0,rb.velocity.y);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            newVel.y = jumpSpeed;
        }

        if (dodgeTime >= totalDodgeLength)
        {
            dodging = false;
            dodgeTime = 0;
        }

        if (dodging)
        {
            newVel.x = dodgeSpeed;
            dodgeTime += 1;
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            newVel.x = dodgeSpeed;
            dodging = true;
            dodgeTime += 1;
        }

        if (!dodging)
        {
            if (Input.GetKey(KeyCode.RightArrow))
            {
                newVel.x = newVel.x + walkSpeed;
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                newVel.x = newVel.x - walkSpeed;
            }
        }

        rb.velocity = newVel;

        // update the game state
    }

    // called at a specific rate. Use this to jump instead of gravity?
    void FixedUpdate()
    {
        
    }
}
