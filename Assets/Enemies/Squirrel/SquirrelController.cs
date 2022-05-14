using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquirrelController : MonoBehaviour, EnemyController
{
    public GameObject acorn;
    private AcornController myAcorn;

    private GameObject player;
    private float acornTimer;
    private float acornResetTime = 3;
    private bool active = false;

    public float knockbackSpeed()
    {
        return 0;
    }

    public Vector2 position()
    {
        return transform.position;
    }

    void Awake()
    {
        myAcorn = Instantiate(acorn, transform.position, transform.rotation).GetComponent<AcornController>();
        myAcorn.gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            if (acornTimer <= 0)
            {
                acornTimer = acornResetTime;
                myAcorn.transform.position = transform.position;
                myAcorn.gameObject.SetActive(true);
                myAcorn.throwAt(player.transform.position);
                Debug.Log("threw acorn");
            }
            else { acornTimer -= Time.deltaTime; }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player = collision.gameObject;
            active = true;
            acornTimer = 0;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) {
            active = false;
        }
    }
}
