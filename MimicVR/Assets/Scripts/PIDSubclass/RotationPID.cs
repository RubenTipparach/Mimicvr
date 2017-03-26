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
        StartCoroutine(moveAction((result) =>
        {
            if(result > 0)
            {
                moveCmd.Right();
            }
            else {
                moveCmd.Left();
            }
        },
        Time.deltaTime));
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("updating as normal");
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


