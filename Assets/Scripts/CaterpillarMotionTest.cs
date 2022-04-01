using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaterpillarMotionTest : MonoBehaviour
{
    public Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = new Vector2(-0.55f, 0);
    }
}
