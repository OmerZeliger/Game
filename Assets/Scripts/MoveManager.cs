using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveManager : MonoBehaviour
{
    public Rigidbody2D rb;
    public MovementController mc;

    // player movement state
    bool jumping; // is the player y-velocity positive after a jump
    bool frontTouchingWall;
    bool holdingWall;
    bool doubleJumped;
    bool falling;
    bool grounded;
    HorizontalDirection horzDir = HorizontalDirection.NONE;
    bool dodging;
    int dodgeRemaining;
    bool airDodged;
    Vector2 steadyVel; // any velocity that the game needs to maintain for a period of time

    // other useful constants
    private int groundLayer; // TODO: add a public list of useful constants like this

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
        // check if dodging - if dodging, maintain current velocity and decrease dodgeRemaining

        falling = rb.velocity.y < 0;
        jumping = jumping && !falling;

        if (dodging)
        {
            rb.velocity = steadyVel;
            dodgeRemaining -= 1;
            if (dodgeRemaining <= 0)
            {
                dodging = false;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Wall"))
        {
            if (mc.groudDetector.IsTouchingLayers(groundLayer)) {
                grounded = true;
            }
            if (mc.wallDetector.IsTouchingLayers(groundLayer))
            {
                frontTouchingWall = true;
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Wall"))
        {
            if (!mc.groudDetector.IsTouchingLayers(groundLayer))
            {
                grounded = false;
            }
            if (!mc.wallDetector.IsTouchingLayers(groundLayer))
            {
                frontTouchingWall = false;
            }
        }
    }

    abstract class Jump
    {
        protected float jumpSpeed = 15;

        // begin a jump if appropriate
        public virtual void jump(MoveManager mm)
        {
            if (mm.dodging) { return; } // TODO: queueing
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

    class BasicJump : Jump { }

    class DoubleJump : Jump
    {
        Jump groundedJump;
        // TODO: add an air jump instead of manually doing it. That'll also take care of queueing.
        // TODO: return a bool for queueing purposes

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

    abstract class Walk
    {
        float walkSpeed = 7;

        // set velocity to walk in the correct direction, set the character state to facing that direction
        public virtual void walk(MoveManager mm, HorizontalDirection dir)
        {
            if (!mm.holdingWall && !mm.dodging)
            {
                int d = 0;
                switch (dir)
                {
                    case HorizontalDirection.RIGHT : d = 1; break;
                    case HorizontalDirection.LEFT : d = -1; break;
                    case HorizontalDirection.NONE : d = 0; break;
                        
                }
                this.w(mm, d);
                if (!mm.horzDir.Equals(dir))
                {
                    mm.mc.flip(dir);
                    mm.horzDir = dir;
                }
            }
        }

        protected void w(MoveManager mm, int dir)
        {
            mm.rb.velocity = new Vector2(dir * walkSpeed, mm.rb.velocity.y);
        }
    }

    class BasicWalk : Walk { }

    abstract class Dodge
    {
        float forwardDodgeSpeed = 12;
        float backwardsDodgeSpeed = 10;
        int forwardsDodgeLength = 8;
        int backwardsDodgeLength = 5;

        // dodge in the given direction if appropriate
        public virtual void dodge(MoveManager mm, HorizontalDirection dir)
        {
            if (mm.grounded && !mm.dodging)
            {
                if (dir.Equals(mm.horzDir))
                {
                    forwardDodge(mm);
                    mm.dodgeRemaining = forwardsDodgeLength;
                } else
                {
                    backwardsDodge(mm);
                    mm.dodgeRemaining = backwardsDodgeLength;
                }
                mm.dodging = true;
                mm.steadyVel = mm.rb.velocity;
            }
        }

        // dodge in the direction the character is facing
        protected void forwardDodge(MoveManager mm)
        {
            int d = 0;
            switch (mm.horzDir)
            {
                case HorizontalDirection.RIGHT: d = 1; break;
                case HorizontalDirection.LEFT: d = -1; break;
                case HorizontalDirection.NONE: d = 0; break;

            }
            mm.rb.velocity = new Vector2(d * forwardDodgeSpeed, 0);
        }

        // dodge in the opposite direction the character is facing
        protected void backwardsDodge(MoveManager mm)
        {
            int d = 0;
            switch (mm.horzDir)
            {
                case HorizontalDirection.RIGHT: d = 1; break;
                case HorizontalDirection.LEFT: d = -1; break;
                case HorizontalDirection.NONE: d = 0; break;

            }
            mm.rb.velocity = new Vector2(d * backwardsDodgeSpeed, 0);
        }
    }
}