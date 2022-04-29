using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
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
    public TrailRenderer tr;
    public LineRenderer lr;
    public BezierController bezier;
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
            if (mm.attack())
            {
                attack();
            }
        }


    }

    void FixedUpdate()
    {
        if (attacking)
        {
            attackRemaining -= 1;
            transform.position = steadyLoc;
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

        if (lr.enabled)
        {
            maintainTrailAfterJump();
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



        //transform.localPosition = attackLoc;
        jumpTo(attackLoc, Vector2.zero);
        steadyLoc = rb.transform.position; // set steadyLoc to attackLoc's global position
        attacking = true;
        attackRemaining = attackLength;

        splashDamage.activate();
    }

    // move the ball to the new position and give it a pretty trail
    // both given locations should be in local coordinates (relative to player)
    private void jumpTo(Vector2 newPosition, Vector2 approachDirection)
    {
        tr.AddPositions(bezier.trail(transform.position,
            Utils.sum(transform.position,
                Utils.scale(Utils.subtract(transform.position,tr.GetPosition(tr.positionCount-2)), 8)),
            Utils.sum(transform.parent.position, approachDirection),
            Utils.sum(transform.parent.position, newPosition)));
        Vector3[] positions = new Vector3[tr.positionCount];
        tr.GetPositions(positions);
        lr.enabled = true;
        Vector3[] lrPositions = new Vector3[lr.positionCount];
        lr.GetPositions(lrPositions);
        lrPositions = lrPositions.Concat(positions).ToArray();
        lr.positionCount = lrPositions.Length;
        lr.SetPositions(lrPositions);

        transform.localPosition = newPosition;
        tr.Clear();
        tr.forceRenderingOff = true;
    }

    private void maintainTrailAfterJump()
    {
        //return;
        int approxTrPositions = Mathf.FloorToInt(tr.time / Time.fixedDeltaTime);
        if (lr.positionCount <= approxTrPositions) //|| lr.positionCount < tr.positionCount)
        {
            if (tr.positionCount < lr.positionCount)
            {
                Debug.Log("lr longer than tr " + tr.positionCount + " " + lr.positionCount);
            }
            else // lr shorter or equal to tr
            {
                Vector3[] lrPositionsB = new Vector3[lr.positionCount];
                lr.GetPositions(lrPositionsB);
                Vector3[] trPositionsB = new Vector3[tr.positionCount];
                Array.ConstrainedCopy(lrPositionsB, 0, trPositionsB,
                    trPositionsB.Length - lrPositionsB.Length, lrPositionsB.Length);
                for (int i = 0; i <= trPositionsB.Length - lrPositionsB.Length + 1; i++)
                {
                    trPositionsB[i] =
                        Vector3.Lerp(lrPositionsB[0], lrPositionsB[Mathf.Min(1, lrPositionsB.Length)],
                        (float)i / (trPositionsB.Length - lrPositionsB.Length + 1));
                }
                tr.SetPositions(trPositionsB);
                tr.forceRenderingOff = false;

                lr.positionCount = 0;
                lr.enabled = false;
                //Debug.Log("disabled");
                return;
            }
        }
        
        // add the latest point
        lr.positionCount = lr.positionCount + 1;
        lr.SetPosition(lr.positionCount - 1, new Vector3(transform.position.x, transform.position.y, 0));

        // remove the last couple points
        int trailLength = lr.positionCount;
        int deleteNum = Mathf.RoundToInt(Mathf.Max(4, trailLength / 10));
        //int deleteNum = 1;
        Vector3[] lrPositions = new Vector3[lr.positionCount];
        lr.GetPositions(lrPositions);
        //Array.Reverse(lrPositions);
        Vector3[] newPositions = new Vector3[lr.positionCount - deleteNum];
        //lrPositions.CopyTo(newPositions, deleteNum);
        Array.ConstrainedCopy(lrPositions, deleteNum, newPositions, 0, newPositions.Length);
        lr.positionCount = trailLength - deleteNum;
        //Array.Reverse(newPositions);
        lr.SetPositions(newPositions);
    }
}
