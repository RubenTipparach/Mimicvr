using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMove : AbstractMoveCarBehavior {


    [SerializeField]
    DirectionalDisplay directionDisp;

    SimpleRotate rotationController;


    float distanceToTarget = 0;

    float distanceBias = .1f;

	// Update is called once per frame
	void Update () {
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

    // initialize, but does nothing right now.
    public override void cStart()
    {
        rotationController = GetComponent<SimpleRotate>();
    }
}
