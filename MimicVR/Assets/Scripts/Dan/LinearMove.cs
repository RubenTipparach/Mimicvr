using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearMove : MonoBehaviour {

    [SerializeField]
    SocketMoveCmd socket;

    [SerializeField]
    LocalMoveCmd local;

    RobotCommandInput moveCmd;

    Vector3 currentPosition;
    Vector3 lastPosition;
    float deltaPosition;
    float velocity;
    float averageVelocity = 0;

    void Start () {
        if (socket != null)
        {
            moveCmd = socket;
        }
        else
        {
            moveCmd = local;
        }

        StartCoroutine(startDelay(0));
    }

    IEnumerator startDelay(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        float addedSeconds = 1;
        int i = 0;

        while (i < 5) {

            lastPosition = transform.position;

            moveCmd.Forward();

            currentPosition = transform.position;
            deltaPosition = currentPosition.magnitude - lastPosition.magnitude;
            velocity = deltaPosition / 1;

            StartCoroutine(moveAction(moveCmd.Backward, 2 * addedSeconds));

            lastPosition = currentPosition;
            currentPosition = transform.position;
            deltaPosition = currentPosition.magnitude - lastPosition.magnitude;
            velocity = deltaPosition / 1;

            averageVelocity = (averageVelocity + velocity) / 2;
            i++;
        }

        StartCoroutine(moveAction(moveCmd.Stop, 5 * addedSeconds));

        Debug.Log("Velocity: " + averageVelocity);
    }

    IEnumerator moveAction(Action run, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        run();
    }
	
    void Update () {
		
    }
}