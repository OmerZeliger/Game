using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    // constants
    float rotationDistance = 0.7f;
    float attackDistance = 1;
    float followSpeed = 3;
    int attackLength = 8;

    // stuff that changes
    bool attacking = false;
    int attackRemaining = 0;
    Vector2 steadyLoc;

    // references to other game stuff
    public MoveManager mm;
    public Rigidbody2D rb;
    public AttackSplashController splashDamage;
    private Game inputs;

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
        if (attacking)
        {
            attackRemaining -= 1;
            transform.localPosition = steadyLoc;
            if (attackRemaining <= 0) {
                attacking = false;
                splashDamage.deactivate();
            }
        }
        else
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
        
    }

    // attack lol
    void attack()
    {
        if (attacking)
        {
            // TODO: queue attack
            return;
        }
        Direction attackDir = Utils.intToVertDir(Mathf.RoundToInt(inputs.Player.Look.ReadValue<float>()));
        if (attackDir == Direction.NONE)
        {
            attackDir = mm.horizontalDirection;
        }

        Vector2 attackLoc = Utils.dirToVector(attackDir).normalized;
        attackLoc.Scale(new Vector2(attackDistance, attackDistance));

        transform.localPosition = attackLoc;
        steadyLoc = attackLoc;
        attacking = true;
        attackRemaining = attackLength;

        splashDamage.activate();
    }
}
