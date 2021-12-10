using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rb;
    public BoxCollider2D wallDetector;
    public BoxCollider2D groudDetector;
    public SpriteRenderer sr;
    public MoveManager mm;

    private Game inputs;

    //double maxFallSpeed = -1;
    float jumpSpeed = 15;
    float walkSpeed = 7;
    bool dodging = false;
    int dodgeTime = 0;
    int totalDodgeLength = 8;
    float dodgeSpeed = 12;

    // flip the character horizontally
    public void flip(Direction direction)
    {
        int offset = -1;
        switch (direction)
        {
            case Direction.RIGHT : sr.flipX = true; offset = 1; break;
            case Direction.LEFT : sr.flipX = false; offset = -1; break;
            case Direction.NONE : return;
        }
        wallDetector.offset = new Vector2(offset * Mathf.Abs(wallDetector.offset.x), wallDetector.offset.y);
    }

    // Awake is called before start, when script is being loaded
    void Awake()
    {
        inputs = new Game();
    }

    void OnEnable()
    {
        inputs.Enable();
    }

    void OnDisable()
    {
        inputs.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }


    // Update is called once per frame
    void Update()
    {
        // read the user inputs

        if (inputs.Player.Jump.triggered)
        {
            mm.jump();
        }

        if (inputs.Player.StopJump.triggered)
        {
            mm.stopJump();
        }

        if (inputs.Player.Dodge.triggered)
        {
            float d = inputs.Player.Dodge.ReadValue<float>();
            Direction dir = Direction.NONE;
            switch (d) {
                case -1 : dir = Direction.LEFT; break;
                case 1 : dir = Direction.RIGHT; break;
            }
            mm.dodge(dir);
        }

        if (inputs.Player.Move.phase != InputActionPhase.Waiting || inputs.Player.Move.triggered)
        {
            int d = Mathf.RoundToInt(inputs.Player.Move.ReadValue<float>());
            Direction dir = Direction.NONE;
            switch (d)
            {
                case -1: dir = Direction.LEFT; break;
                case 1: dir = Direction.RIGHT; break;
            }
            mm.walk(dir);
        }



        // update the game state
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // TODO: based on the tag, test which trigger was hit. For example, if the tag is "wall", test wallDetector
        Debug.Log("Hit " + collision.tag);
    }

    // called at a specific rate. Use this to jump instead of gravity?
    void FixedUpdate()
    {
        if (dodging)
        {
            dodgeTime += 1;
        }

        if (dodgeTime >= totalDodgeLength)
        {
            dodging = false;
            dodgeTime = 0;
        }
    }
}
