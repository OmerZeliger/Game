using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CritterVision : MonoBehaviour
{
    private ACritter critter;

    private void Awake()
    {
        critter = GetComponentInParent<ACritter>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Critter"))
        {
            critter.notice(collision.GetComponent<ACritter>());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Critter"))
        {
            critter.forget(collision.GetComponent<ACritter>());
        }
    }
}
