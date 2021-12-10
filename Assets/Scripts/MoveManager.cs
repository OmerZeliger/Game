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
    Jump jumper;
    Walk walker;
    Dodge dodger;

    // other useful constants
    private int groundLayer; // TODO: add a public list of useful constants like this

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

    void Awake()
    {
        groundLayer = LayerMask.GetMask("Ground");
        jumper = new DoubleJump();
        walker = new BasicWalk();
        dodger = new GroundDodge();
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

        if (pc.groudDetector.IsTouchingLayers(groundLayer))
        {
            grounded = true;
            doubleJumped = false;
            airDodged = false;
        }
        if (pc.wallDetector.IsTouchingLayers(groundLayer))
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
        public abstract void jump(MoveManager mm);

        protected virtual void regularJump(MoveManager mm)
        {
            {
                jmp(mm.rb);
                mm.jumping = true;
                mm.grounded = false;
            }
        }

        protected virtual void wallJump(MoveManager mm)
        {
            // TODO: implement
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
        public override void jump(MoveManager mm)
        {
            if (mm.dodging || !mm.grounded) { return; } // TODO: queueing
            if (mm.grounded)
            {
                regularJump(mm);
            }
            else if (mm.holdingWall)
            {
                // TODO: implement special jump for holding onto the wall
            }
        }
    }

    class DoubleJump : Jump
    {
        Jump groundedJump;
        // TODO: return a bool for queueing purposes

        public DoubleJump()
        {
            groundedJump = new BasicJump();
            // TODO: why can't I call the other constructor without groundedJump being null?
        }

        public DoubleJump(Jump gj)
        {
            groundedJump = gj;
        }

        public override void jump(MoveManager mm)
        {
            if (!mm.grounded && !mm.doubleJumped)
            {
                regularJump(mm);
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
        protected virtual float walkSpeed
        {
            get { return 7; }
        }

        protected void alwaysWalk(MoveManager mm, Direction dir)
        {
            int d = 0;
            switch (dir)
            {
                case Direction.RIGHT: d = 1; break;
                case Direction.LEFT: d = -1; break;
                case Direction.NONE: d = 0; break;
            }
            this.w(mm, d);
            if (!mm.horzDir.Equals(dir) && dir != Direction.NONE)
            {
                mm.pc.flip(dir);
                mm.horzDir = dir;
            }
        }

        // set velocity to walk in the correct direction, set the character state to facing that direction
        public abstract void walk(MoveManager mm, Direction dir);

        protected void w(MoveManager mm, int dir)
        {
            mm.rb.velocity = new Vector2(dir * walkSpeed, mm.rb.velocity.y);
        }
    }

    class BasicWalk : Walk
    {
        public override void walk(MoveManager mm, Direction dir)
        {
            if (!mm.holdingWall && !mm.dodging)
            {
                alwaysWalk(mm, dir);
            }
        }
    }

    abstract class Dodge
    {
        protected virtual int forwardDodgeSpeed
        {
            get { return 12; }
        }
        protected virtual int backwardsDodgeSpeed
        {
            get { return 10; }
        }
        protected virtual int forwardsDodgeLength
        {
            get { return 8; }
        }
        protected virtual int backwardsDodgeLength
        {
            get { return 5; }
        }

        // dodge in the given direction if appropriate
        public abstract void dodge(MoveManager mm, Direction dir);

        // dodge in the given direction
        protected void alwaysDodge(MoveManager mm, Direction dir)
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

    class GroundDodge : Dodge
    {
        public override void dodge(MoveManager mm, Direction dir)
        {
            if (mm.grounded && !mm.dodging && dir != Direction.NONE)
            {
                alwaysDodge(mm, dir);
            }
        }
    }
}