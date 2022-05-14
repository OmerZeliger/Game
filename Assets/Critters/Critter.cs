using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Critter : MonoBehaviour
{
    private ACritter controller;

    private void Awake()
    {
        controller = GetComponent<ACritter>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Critter"))
        {
            controller.touchOther(collision.gameObject.GetComponent<ACritter>());
        }
    }
}
