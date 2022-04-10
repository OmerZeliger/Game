using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaterpillarAnimationController : MonoBehaviour
{
    public GameObject[] segments;
    bool crawling = false;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < segments.Length; i++)
        {
            segments[i].GetComponent<CaterpillarSegmentEventHandler>().assign(i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!crawling) {
            animateNextLeg(segments.Length);
            crawling = true;
        }
    }

    public void animateNextLeg(int i)
    {
        if (i > 0 && !segments[i - 1].GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Crawl"))
        {
            segments[i - 1].GetComponent<Animator>().SetTrigger("MoveTrigger");
        }
    }

    //void crawl()
    //{
    //    //TODO: why is this uneven?
    //    // I don't like the way this is uneven. Play around with triggers? Queued animations?
    //    // Look up how to have multiple animations playing at precise timepoints
    //    DoDelayAction(segments[7].GetComponent<Animator>(), 0.0f);
    //    DoDelayAction(segments[6].GetComponent<Animator>(), 0.2f);
    //    DoDelayAction(segments[5].GetComponent<Animator>(), 0.4f);
    //    DoDelayAction(segments[4].GetComponent<Animator>(), 0.6f);
    //    DoDelayAction(segments[3].GetComponent<Animator>(), 0.8f);
    //    DoDelayAction(segments[2].GetComponent<Animator>(), 1.0f);
    //    DoDelayAction(segments[1].GetComponent<Animator>(), 1.2f);
    //    DoDelayAction(segments[0].GetComponent<Animator>(), 1.4f);

    //    //TODO: why doesn't this work??
    //    //Debug.Log("Started");
    //    //for (int i = 0; i < segments.Length; i++)
    //    //{
    //    //    DoDelayAction(segments[segments.Length - i].GetComponent<Animator>(), 0.2f * i);

    //    //}
    //}


    void DoDelayAction(Animator segmentAnimator, float delayTime)
    {
        StartCoroutine(CrawlAnimator(segmentAnimator, delayTime));
    }



    // 
    IEnumerator CrawlAnimator(Animator segmentAnimator, float delayTime)
    {
        //Wait for the specified delay time before continuing.
        yield return new WaitForSeconds(delayTime);

        //Do the action after the delay time has finished.
        segmentAnimator.Play("Crawl",0,0);
    }
}
