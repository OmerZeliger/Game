using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaterpillarSegmentEventHandler : MonoBehaviour
{
    private int segmentID;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void assign(int ID)
    {
        segmentID = ID;
    }

    public void startCrawl()
    {
        gameObject.GetComponentInParent<CaterpillarAnimationController>().animateNextLeg(segmentID);
    }
}
