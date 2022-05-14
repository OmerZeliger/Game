using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcornController : MonoBehaviour, EnemyController
{
    public float knockbackSpeed()
    {
        return GetComponent<Rigidbody2D>().velocity.magnitude;
    }

    public Vector2 position()
    {
        return Utils.subtract(transform.position, GetComponent<Rigidbody2D>().velocity);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void throwAt(Vector2 target)
    {
        Vector2 angleToPlayer = Utils.subtract(target, transform.position);
        angleToPlayer = new Vector2(angleToPlayer.x, angleToPlayer.y + (angleToPlayer.magnitude / 3));
        GetComponent<Rigidbody2D>().velocity = Utils.scale(angleToPlayer.normalized, 7f);
    }
}
