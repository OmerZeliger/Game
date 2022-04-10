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
    public GameObject respawn;

    private Game inputs;
    private Interactor interactibleInRange;

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

        if (inputs.Player.Interact.triggered)
        {
            if (interactibleInRange != null)
            {
                interactibleInRange.interact();
            }
        }

        if (true) //(inputs.Player.Move.phase != InputActionPhase.Waiting || inputs.Player.Move.triggered)
            // TODO: for some reason the player's rounded box collider will slide backwards slowly after contacting a wall
            // leave it this way until I figure out why and how to stop it lol
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

        if (collision.CompareTag("Interactable"))
        {
            interactibleInRange = collision.gameObject.GetComponent<Interactor>();
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        // TODO: based on the tag, test which trigger was hit. For example, if the tag is "wall", test wallDetector
        if (collision.CompareTag("Interactable"))
        {
            interactibleInRange = null;
        }
    }

    // called at a specific rate. Use this to jump instead of gravity?
    void FixedUpdate()
    {
        if (transform.position.y < -10)
        {
            transform.position = respawn.transform.position;
        }
    }
}
