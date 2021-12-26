using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    // constants
    float rotationDistance = 0.7f;
    float attackDistance = 1;
    float followSpeed = 3;

    public MoveManager mm;
    public Rigidbody2D rb;
    private Game inputs;

    // TODO: make the ball lag behind a little when the player walks by adding a rotating empty game object,
    // saving its location for a couple fixedUpdates, and moving the ball there after a delay

    // Start is called before the first frame update
    void Start()
    {
        
    }

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

    // Update is called once per frame
    void Update()
    {
        if (inputs.Player.Attack.triggered)
        {
            attack();
        }
    }

    void FixedUpdate()
    {
        transform.RotateAround(transform.parent.position, Vector3.forward, 1.8f);
        if (Mathf.Abs(transform.localPosition.magnitude - rotationDistance) > 0.01)
        {
            Vector2 scaled = transform.localPosition.normalized;
            scaled.Scale(new Vector2(rotationDistance, rotationDistance));
            Vector2 newVel = Utils.subtract(transform.localPosition, scaled);
            newVel.Scale(new Vector2(-followSpeed, -followSpeed));
            rb.velocity = newVel;
        }
    }

    // attack lol
    void attack()
    {
        Direction attackDir = Utils.intToVertDir(Mathf.RoundToInt(inputs.Player.Look.ReadValue<float>()));
        if (attackDir == Direction.NONE)
        {
            attackDir = mm.horizontalDirection;
        }

        Vector2 attackLoc = Utils.dirToVector(attackDir).normalized;
        attackLoc.Scale(new Vector2(attackDistance, attackDistance));

        transform.localPosition = attackLoc;
    }
}
