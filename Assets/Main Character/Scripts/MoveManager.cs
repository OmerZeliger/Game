using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveManager : MonoBehaviour
{
    public Rigidbody2D rb;
    public PlayerController pc;

    // constants
    int queueLength = 10; // TODO: find a more elegant solution than having a bunch of repeated code?
    // TODO: change the queue length based on the framerate. Or just use delta time lol
    int coyoteTime = 3;

    // player movement state
    bool jumping; // is the player y-velocity positive after a jump
    bool frontTouchingWall;
    bool holdingWall;
    bool doubleJumped;
    bool falling;
    bool grounded;
    int extraGroundedTime;
    Direction horzDir = Direction.LEFT;
    bool dodging;
    int dodgeRemaining;
    bool airDodged;
    Vector2 steadyVel; // any velocity that the game needs to maintain for a period of time
    bool invincible { get { return invincibilityRemaining > 0; } }
    int defaultInvincibility = 20;
    int invincibilityRemaining;
    bool stunned { get { return stunRemaining > 0; } }
    int stunRemaining;
    int defaultStun = 8;
    //float gravity = -0.5;

    // queue
    int jumpQueue = 0;
    int dodgeQueue = 0;
    Direction dodgeQueueDirection;

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
        if (!jumper.jump(this))
        {
            jumpQueue = queueLength;
        }
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
        if (!dodger.dodge(this, dir))
        {
            dodgeQueue = queueLength;
            dodgeQueueDirection = dir;
        }
    }

    public bool getHit(EnemyController enemy)
    {
        if (!invincible)
        {
            // reset all queues
            jumpQueue = 0;
            dodgeQueue = 0;
            // stop all motion
            jumping = false;
            holdingWall = false;
            dodging = false;
            dodgeRemaining = 0; //TODO: start dodge recovery timer
            airDodged = false;
            doubleJumped = false; //TODO: keep this? it'll let you recover double jump after getting hit mid-air

            // stun the player
            stunRemaining = defaultStun;

            // knock the player away from the enemy
            Vector2 knockback = Utils.scale(Utils.subtract(rb.transform.position, enemy.position()).normalized,
                enemy.knockbackSpeed());
            if (grounded)
            {
                //TODO: not very elegant. what effect do I want?
                knockback = new Vector2(knockback.x, Mathf.Max(knockback.y, 3));
            }
            rb.velocity = knockback;
            Debug.Log(knockback);
            invincibilityRemaining = defaultInvincibility; // give the player some invincibility frames
            return true; // return true if the player was successfully hit
        } else { return false; }
    }

    public bool attack()
    {
        return !stunned;
    }

    void Awake()
    {
        groundLayer = LayerMask.GetMask("Ground");
        jumper = new BasicJump();
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
        // take care of queued actions
        if (jumpQueue > 0)
        {
            jumpQueue -= 1;
            if (jumper.jump(this))
            {
                jumpQueue = 0;
            }
        }
        if (dodgeQueue > 0)
        {
            dodgeQueue -= 1;
            if (dodger.dodge(this, dodgeQueueDirection))
            {
                dodgeQueue = 0;
            }
        }
    }

    void FixedUpdate()
    {
        // TODO: update the game state based on what's going on
        // check if grounded
        // check if falling
        // check if jumping
        // check if move queued
        // check if dodging - if dodging, maintain current velocity and decrease dodgeRemaining

        // check if grounded
        if (pc.groudDetector.IsTouchingLayers(groundLayer))
        {
            grounded = true;
            doubleJumped = false;
            airDodged = false;
        }
        // check if the front of the player is touching a wall
        if (pc.wallDetector.IsTouchingLayers(groundLayer))
        {
            frontTouchingWall = true;
        }

        // check if player is falling/jumping
        falling = rb.velocity.y < 0;
        jumping = jumping && !falling;

        // maintain a dodge
        if (dodging)
        {
            rb.velocity = steadyVel;
            dodgeRemaining -= 1;
            if (dodgeRemaining <= 0)
            {
                dodging = false;
                //rb.velocity = new Vector2(0, 0);
                // don't need this as long as walk gets called at every update
                //TODO: start dodge recovery timer
            }
        }

        // decrease coyote time
        extraGroundedTime = Mathf.Max(extraGroundedTime - 1, 0);

        // decrease invinciblity time
        if (invincible)
        {
            invincibilityRemaining -= 1;
            if (!invincible)
            {
                //pc.recollide(); // treat any collisions coming out of invincibility as new (intended for enemies)
                // moved this to OnTriggerStay instead
            }
        }

        // decrease stun time
        if (stunned)
        {
            stunRemaining -= 1;
            if (!stunned)
            {
                // invincibility should last longer than stun anyway so this hopefully shouldn't matter
                //pc.recollide(); // treat any collisions coming out of stun as new (intended for enemies)
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
                extraGroundedTime = coyoteTime;
            }
            if (!pc.wallDetector.IsTouchingLayers(groundLayer))
            {
                frontTouchingWall = false;
            }
        }
    }

    public Direction horizontalDirection
    {
        get { return horzDir; }
    }

    void clearQueue()
    {
        dodgeQueue = 0;
        jumpQueue = 0;
    }

    // returns true if the player is attached to the ground - holding a wall, standing, in coyote time
    bool groundedCheck()
    {
        return grounded || extraGroundedTime > 0 || frontTouchingWall;
    }

    abstract class Jump
    {
        protected virtual float jumpSpeed
        {
            get { return 15; }
        }

        // begin a jump if appropriate
        public abstract bool jump(MoveManager mm);

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
        public override bool jump(MoveManager mm)
        {
            if (mm.dodging || !mm.groundedCheck()) { return false; }
            if (mm.holdingWall)
            {
                // TODO: implement special jump for holding onto the wall
            }
            else if (mm.grounded || mm.extraGroundedTime > 0)
            {
                regularJump(mm);
            }
            return true;
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

        public override bool jump(MoveManager mm)
        {
            if (!mm.groundedCheck() && !mm.doubleJumped)
            {
                regularJump(mm);
                mm.doubleJumped = true;
                return true;
            }
            else
            {
                return groundedJump.jump(mm);
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
            if (mm.stunned) { return; }
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
        public abstract bool dodge(MoveManager mm, Direction dir);

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
        public override bool dodge(MoveManager mm, Direction dir)
        {
            if (mm.groundedCheck() && !mm.dodging && dir != Direction.NONE)
            {
                alwaysDodge(mm, dir);
                return true;
            }
            else { return false; }
        }
    }
}