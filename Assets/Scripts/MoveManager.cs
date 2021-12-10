using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveManager : MonoBehaviour
{
    public Rigidbody2D rb;
    public PlayerController pc;

    // player movement state
    bool jumping; // is the player y-velocity positive after a jump
    bool frontTouchingWall;
    bool holdingWall;
    bool doubleJumped;
    bool falling;
    bool grounded;
    Direction horzDir = Direction.LEFT;
    bool dodging;
    int dodgeRemaining;
    bool airDodged;
    Vector2 steadyVel; // any velocity that the game needs to maintain for a period of time
    //float gravity = -0.5;

    // the movement strategies
    Jump jumper = new BasicJump();
    Walk walker = new BasicWalk();
    Dodge dodger = new GroundDodge();

    // other useful constants
    private int groundLayer = 3; // TODO: add a public list of useful constants like this

    // TODO: set the different types of movements after instantiation

    // public methods for movement
    public void jump()
    {
        jumper.jump(this);
    }

    public void stopJump()
    {
        jumper.stopJump(this);
    }

    public void walk(Direction dir)
    {
        walker.walk(this, dir);
    }

    public void dodge(Direction dir)
    {
        dodger.dodge(this, dir);
    }

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

        if (pc.groudDetector.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            grounded = true;
        }
        if (pc.wallDetector.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            frontTouchingWall = true;
        }

        falling = rb.velocity.y < 0;
        jumping = jumping && !falling;

        if (dodging)
        {
            rb.velocity = steadyVel;
            dodgeRemaining -= 1;
            if (dodgeRemaining <= 0)
            {
                dodging = false;
                rb.velocity = new Vector2(0, 0);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Wall"))
        {
            
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Wall"))
        {
            if (!pc.groudDetector.IsTouchingLayers(groundLayer))
            {
                grounded = false;
            }
            if (!pc.wallDetector.IsTouchingLayers(groundLayer))
            {
                frontTouchingWall = false;
            }
        }
    }

    abstract class Jump
    {
        protected virtual float jumpSpeed
        {
            get { return 15; }
        }

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
        public virtual void walk(MoveManager mm, Direction dir)
        {
            if (!mm.holdingWall && !mm.dodging)
            {
                int d = 0;
                switch (dir)
                {
                    case Direction.RIGHT : d = 1; break;
                    case Direction.LEFT : d = -1; break;
                    case Direction.NONE : d = 0; break;
                }
                this.w(mm, d);
                if (!mm.horzDir.Equals(dir) && dir != Direction.NONE)
                {
                    mm.pc.flip(dir);
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
        public virtual void dodge(MoveManager mm, Direction dir)
        {
            if (mm.grounded && !mm.dodging && dir != Direction.NONE)
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
                case Direction.RIGHT: d = 1; break;
                case Direction.LEFT: d = -1; break;

            }
            mm.rb.velocity = new Vector2(d * forwardDodgeSpeed, 0);
        }

        // dodge in the opposite direction the character is facing
        protected void backwardsDodge(MoveManager mm)
        {
            int d = 0;
            switch (mm.horzDir)
            {
                case Direction.RIGHT: d = -1; break;
                case Direction.LEFT: d = 1; break;

            }
            mm.rb.velocity = new Vector2(d * backwardsDodgeSpeed, 0);
        }
    }

    class GroundDodge : Dodge { }
}