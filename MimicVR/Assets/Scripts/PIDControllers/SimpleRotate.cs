using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleRotate : AbstractMoveCarBehavior
{

    [SerializeField]
    float currentAngle;

    [SerializeField]
    float angleBias = .01f;

    public float targetAngle = 0;
    Vector3 rotationTarget = Vector3.zero;

    [SerializeField]
    DirectionalDisplay directionDisp;

    bool turning = false;

    [SerializeField]
    bool runWithUpdate = false;

    [SerializeField]
    float runInterval = .2f;

    public bool Turning
    {
        get
        {
            return turning;
        }
    }

    // Use this for initialization
    public override void cStart()
    {
        if (!runWithUpdate)
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

    void getNewAngleoffset()
    {
        Vector3 target = directionDisp.target.position;
        rotationTarget = (target - transform.position).normalized;
        int sign = Vector3.Cross(transform.forward, rotationTarget).y < 0 ? -1 : 1;
        targetAngle = (Vector3.Angle(transform.forward, target) * sign + currentAngle);

        currentAngle = transform.rotation.eulerAngles.y;
    }

    // Update is called once per frame
    void Update () {
        if (runWithUpdate) autoUpdate();
    }

    // avoid spamming the system!
    void autoUpdate()
    {
        // TODO: throttle this code.

        getNewAngleoffset();


        // todo bias
        float angleDiff = Vector3.Dot(transform.forward, rotationTarget);

        // code goes here. 
        if (1.0f - angleDiff > angleBias)
        {
            if (targetAngle < currentAngle)
            {
                moveCmd.Left();
                turning = true;
            }
            else
            if (targetAngle > currentAngle)
            {
                moveCmd.Right();
                turning = true;
            }
        }
        else
        if (turning)
        {
            moveCmd.Stop();
            turning = false;
        }

        //Debug.Log(string.Format("currentAngleDiff: {0} {1}", 1-angleDiff, turning));

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
