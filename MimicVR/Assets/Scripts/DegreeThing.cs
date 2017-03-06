using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DegreeThing : MonoBehaviour {

    // This script shows how mnay milliseconds it takes to turn *rotation* degrees.

    [SerializeField]
    SocketMoveCmd socket;

    [SerializeField]
    LocalMoveCmd local;

    RobotCommandInput moveCmd;
    float startingDegree = 0;
    float currentDegree = 0;
    float startingTime = 0;
    float endingTime = 0;
    float howLong = 0;
    float rotation = 180;

    [SerializeField]
    Transform target;

    public void getDegree()
    {
        if (transform.rotation.eulerAngles.y > -0.1)
            currentDegree = transform.rotation.eulerAngles.y - startingDegree;
        else
            currentDegree = Mathf.Abs(transform.rotation.eulerAngles.y) + 180 - startingDegree;
    }

	// Use this for initialization
	void Start ()
    {
		startingTime = Environment.TickCount;
        if (socket != null)
        {
            moveCmd = socket;
        }
        else
        {
            moveCmd = local;
        }

        moveCmd.Left();
    }

    // Update is called once per frame
    void Update()
    {

        // PID controller might work best here, but you can start with this.
        // Also what is our destination?

        if (currentDegree%rotation >= rotation-3 || currentDegree%rotation <= 3)
        { 
            endingTime = Environment.TickCount;
            startingDegree = currentDegree;
        }

        getDegree();

        if (endingTime > 1)
        {
            howLong = endingTime - startingTime;
            if (howLong > 100)
            {
                startingTime = Environment.TickCount;
                endingTime = 0;
                print("Rotation speed is " + rotation + " degrees per " + howLong + " milliseconds.");
            }
        }

        if (Mathf.Sign(rotation) == -1.0f)
        {
            StartCoroutine(moveAction(moveCmd.Left, howLong));
        }
        else
        {
            StartCoroutine(moveAction(moveCmd.Right, howLong));
        }


    }

    IEnumerator moveAction(Action run, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        run();
    }

}
