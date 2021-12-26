using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlappyEnemyController : MonoBehaviour
{
    public Rigidbody2D rb;
    public GameObject player;

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
        if (transform.position.y < player.transform.position.y
            && rb.velocity.y <= 0
            && Random.Range(0,99) < 30*(player.transform.position.y - transform.position.y))
        {
            flap();
        }

        rb.AddForce(new Vector2(3*(player.transform.position.x - transform.position.x) - (2*rb.velocity.x), 0));
    }

    private void flap()
    {
        rb.velocity = new Vector2(rb.velocity.x, 4);
    }
}
