using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RotationPID : AbstractMoveCarBehavior
{

    [SerializeField]
    float heading = 0;

    [SerializeField]
    PID pidController;

    [SerializeField]
    float showingPidResult;

    [SerializeField]
    DirectionalDisplay directionDisp;




    // old pid stuff.
    [SerializeField]
    float currentAngle;
    public float targetAngle = 0;
    Vector3 rotationTarget = Vector3.zero;

    // rotation, acceleration stuff
    [SerializeField]
    float angleSpeed = 0;
    public float maxAcceleration = 180;// degrees per second^2
    public float maxAngularSpeed = 90; //degrees per second

    void getNewAngleoffset()
    {
        Vector3 target = directionDisp.target.position;
        rotationTarget = (target - transform.position).normalized;
        int sign = Vector3.Cross(transform.forward, rotationTarget).y < 0 ? -1 : 1;
        targetAngle = (Vector3.Angle(transform.forward, target) * sign + currentAngle);
    }

    public override void cStart()
    {
        Debug.Log("object initialized.");

        targetAngle = transform.eulerAngles.y;
        currentAngle = targetAngle;


        //StartCoroutine(moveAction((result) =>
        //{
        //    if(result > 0)
        //    {
        //        moveCmd.Right();
        //    }
        //    else {
        //        moveCmd.Left();
        //    }
        //},
        //Time.deltaTime));
    }

    bool finishedRun = true;
    // Update is called once per frame
    void Update()
    {
        getNewAngleoffset();

        //Debug.Log("updating as normal");
        float result = pidController.Update(targetAngle, currentAngle, Time.deltaTime);

        result = Mathf.Clamp(result, -maxAcceleration, maxAcceleration);

        angleSpeed += result * Time.deltaTime;
        angleSpeed = Mathf.Clamp(angleSpeed, -maxAngularSpeed, maxAngularSpeed);

        currentAngle += angleSpeed * Time.deltaTime;

        gameObject.GetComponent<Rigidbody>().rotation = Quaternion.Euler(0, currentAngle % 360, 0);


        // float result = heading - transform.rotation.eulerAngles.y;
        //showingPidResult = result;

        // transform.rotation = Quaternion.Euler(0, heading, 0);
        if (finishedRun)
        {
            //if (result > 0.1f)
            //{
            //    StartCoroutine(moveAction2(
            //        () =>
            //        {
            //            moveCmd.Left();
            //            Debug.Log("moving Left");
            //        }
            //        , moveCmd.Stop,
            //        1, 1));
            //}
            //else if (result < -0.1f)
            //{
            //    StartCoroutine(moveAction2(
            //        () =>
            //        {
            //            moveCmd.Right();
            //            Debug.Log("moving Right");
            //        }
            //        , moveCmd.Stop,
            //        1, 1));
            //}

            //Debug.Log(string.Format("current offset {0}", result));
        }

        //switcheroo the target angle.
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

    IEnumerator moveAction2(
        Action run, Action stop,
        float waitTime, float secondWaitTime)
    {
        Debug.Log(string.Format("moving for {0} seconds", waitTime));
        finishedRun = false;

        run();
        yield return new WaitForSeconds(waitTime);

        stop();
        yield return new WaitForSeconds(secondWaitTime);

        finishedRun = true;
    }

    IEnumerator moveAction(Action<float> run, float waitTime)
    {
        while (true)
        {
            yield return new WaitForSeconds(waitTime);

            float result = pidController.Update(heading, transform.rotation.eulerAngles.y, waitTime);

            Debug.Log(string.Format("current offset {0}", result));

            run(result);
        }
    }
}


