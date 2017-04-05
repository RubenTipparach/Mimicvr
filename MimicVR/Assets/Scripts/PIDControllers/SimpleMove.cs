using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMove : AbstractMoveCarBehavior {


    [SerializeField]
    DirectionalDisplay directionDisp;

    SimpleRotate rotationController;

    float distanceToTarget = 0;

    [SerializeField]
    float distanceBias = .1f;

    [SerializeField]
    bool runWithUpdate = false;

    [SerializeField]
    float runInterval = .2f;

    // initialize, but does nothing right now.
    public override void cStart()
    {
        rotationController = GetComponent<SimpleRotate>();

        if(!runWithUpdate)
        {
            StartCoroutine(run(runInterval));
        }
    }

    IEnumerator run(float waitTime)
    {
        yield return new WaitForSeconds(5);

        while (true)
        {
            yield return new WaitForSeconds(waitTime);
            autoUpdate();
        }
    }

    // Update is called once per frame
    void Update () {

        if(runWithUpdate) autoUpdate();
    }

    // avoid spamming the system!
    void autoUpdate()
    {
        CalculateDistFromDirection();

        //TODO: bias
        if (!rotationController.Turning)
        {
            if (distanceToTarget > distanceBias)
            {
                moveCmd.Forward();
            }
            else
            if (distanceToTarget < -distanceBias)
            {
                moveCmd.Backward();
            }
        }
    }

    void CalculateDistFromDirection()
    {
        Vector3 offset = directionDisp.target.position - transform.position;
        float distance = offset.magnitude;
        float projection = Vector3.Dot(offset.normalized, transform.forward);

        distanceToTarget = distance * Mathf.Sign(projection);

        //Debug.Log(string.Format("relativeDistance: {0}", distanceToTarget));
    }


}
