using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleRotate : AbstractMoveCarBehavior
{

    [SerializeField]
    float currentAngle;

    public float targetAngle = 0;
    Vector3 rotationTarget = Vector3.zero;

    [SerializeField]
    DirectionalDisplay directionDisp;


    // Use this for initialization
    public override void cStart()
    {
    }


    void getNewAngleoffset()
    {
        Vector3 target = directionDisp.target.position;
        rotationTarget = (target - transform.position).normalized;
        int sign = Vector3.Cross(transform.forward, rotationTarget).y < 0 ? -1 : 1;
        targetAngle = (Vector3.Angle(transform.forward, target) * sign + currentAngle);
    }

    // Update is called once per frame
    void Update () {


        getNewAngleoffset();

        // code goes here.
        if(targetAngle < currentAngle)
        {
            moveCmd.Left();
        }
        else
        if (targetAngle > currentAngle)
        {
            moveCmd.Right();
        }


        if (currentAngle > 360)
        {
            currentAngle = currentAngle - 360;
            targetAngle = targetAngle - 360;
        }

        if (currentAngle < -360)
        {
            currentAngle = currentAngle + 360;
            targetAngle = targetAngle + 360;
        }
    }
}
