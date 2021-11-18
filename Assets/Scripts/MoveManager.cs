using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveManager : MonoBehaviour
{
    public Rigidbody2D rb;

    // player movement state
    bool jumping; // is the player y-velocity positive atfer a jump
    bool holdingWall;
    bool doubleJumped;
    bool falling;
    bool grounded;

    // TODO: set the different types of movements after instantiation

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        // TODO: update the game state based on what's going on
        // check if grounded
        // check if falling
        // check if jumping
        // check if move queued
    }

    abstract class Jump
    {
        protected float jumpSpeed = 15;

        // begin a jump if appropriate
        public virtual void jump(MoveManager mm)
        {
            if (mm.grounded)
            {
                jmp(mm.rb);
                mm.jumping = true;
                mm.grounded = false;
            }
            else if (mm.holdingWall)
            {
                // TODO: implement special jump for holding onto the wall
            }
        }

        // stop all upwards velocity after a jump
        public virtual void stopJump(MoveManager mm)
        {
            if (mm.jumping)
            {
                mm.rb.velocity = new Vector2(mm.rb.velocity.x, 0);
            }
            mm.jumping = false;
        }

        protected void jmp(Rigidbody2D rb)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
        }
    }

    class BasicJump : Jump
    {
        
    }

    class DoubleJump : Jump
    {
        Jump groundedJump;

        DoubleJump()
        {
            new DoubleJump(new BasicJump());
        }

        DoubleJump(Jump groundedJump)
        {
            this.groundedJump = groundedJump;
        }

        public override void jump(MoveManager mm)
        {
            if (!mm.grounded && !mm.doubleJumped)
            {
                jmp(mm.rb);
                mm.doubleJumped = true;
            }
            else
            {
                groundedJump.jump(mm);
            }
        }
    }
}
