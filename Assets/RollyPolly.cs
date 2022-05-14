using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollyPolly : ACritter
{
    private float countdown = 2;
    private float yVel;

    public override void forget(ACritter other)
    {
        throw new System.NotImplementedException();
    }

    public override void notice(ACritter other)
    {
        throw new System.NotImplementedException();
    }

    public override void takeHit(float damage, Vector2 knockbackDirection, float stun)
    {
        throw new System.NotImplementedException();
    }

    public override void touchOther(ACritter other)
    {
        throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        yVel = GetComponent<Rigidbody2D>().velocity.y;
    }

    // Update is called once per frame
    void Update()
    {
        countdown -= Time.deltaTime;
        if (countdown <= 0)
        {
            GetComponent<Animator>().SetTrigger("Jump");
            countdown = 2;
        }
    }

    private void jump()
    {
        GetComponent<Rigidbody2D>().velocity = new Vector2(0, 5);
    }
}
