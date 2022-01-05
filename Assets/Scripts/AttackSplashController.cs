using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSplashController : MonoBehaviour
{
    public Collider2D splashDamage;

    // Start is called before the first frame update
    void Start()
    {
        splashDamage.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        reorient();
    }

    // turn the attack on
    public void activate()
    {
        reorient();
        splashDamage.enabled = true;
    }

    // turn the attack off
    public void deactivate()
    {
        splashDamage.enabled = false;
    }

    private void reorient()
    {
        float newRotation = Vector2.SignedAngle(Vector2.right, transform.parent.localPosition);
        transform.rotation = Quaternion.Euler(0, 0, newRotation);
    }
}
