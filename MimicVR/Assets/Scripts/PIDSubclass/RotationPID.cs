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

    public override void cStart()
    {
        Debug.Log("object initialized.");
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
        //Debug.Log("updating as normal");
        float result = pidController.Update(heading, transform.rotation.eulerAngles.y, Time.deltaTime);

        if (finishedRun)
        {
            if (result > 0.1f)
            {
                StartCoroutine(moveAction2(
                    moveCmd.Left, moveCmd.Stop,
                    result / 180.0f));
            }
            else if (result < -0.1f)
            {
                StartCoroutine(moveAction2(
                    moveCmd.Right, moveCmd.Stop,
                    result / 180.0f));
            }

            //Debug.Log(string.Format("current offset {0}", result));
        }
    }

    IEnumerator moveAction2(
        Action run, Action stop, float waitTime)
    {
        Debug.Log(string.Format("moving for {0} seconds", waitTime));
        finishedRun = false;

        run();
        yield return new WaitForSeconds(waitTime);
        stop();

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


