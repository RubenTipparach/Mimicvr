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
    Vector3 startPosition;
    Quaternion startRotation;
    float degreesFromStart;
    float distanceToStart;
    float deltaPosition;
    float velocity1;
    float velocity2;
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

        StartCoroutine(
            startDelay(5));
        //startDelay(0);
    }

    IEnumerator startDelay(float waitTime)
    {

        float addedSeconds = 1;
        int i = 0;

        startPosition = transform.position;
        startRotation = transform.rotation;

        while (i < 3)
        {
            lastPosition = transform.position;

            StartCoroutine(moveAction(moveCmd.Forward, 1 * addedSeconds));
            StartCoroutine(moveAction(moveCmd.Stop, 3 * addedSeconds));

            yield return new WaitForSeconds(waitTime * addedSeconds);

            currentPosition = transform.position;
            deltaPosition = Vector3.Distance(transform.position, lastPosition);
            velocity1 = deltaPosition / 2;

            StartCoroutine(moveAction(moveCmd.Backward, 1 * addedSeconds));
            StartCoroutine(moveAction(moveCmd.Stop, 3 * addedSeconds));

            yield return new WaitForSeconds(waitTime * addedSeconds);

            lastPosition = currentPosition;
            currentPosition = transform.position;
            deltaPosition = Vector3.Distance(transform.position, lastPosition);
            velocity2 = deltaPosition / 2;

            averageVelocity = (averageVelocity + (velocity1 + velocity2) / 2) / 2;

            i++;
        }

        distanceToStart = Vector3.Distance(transform.position, startPosition);
        degreesFromStart = Quaternion.Angle(transform.rotation, startRotation);

        while (distanceToStart > 0.5)
        {
            yield return new WaitForSeconds(waitTime * addedSeconds);
            distanceToStart = Vector3.Distance(transform.position, startPosition);
            yield return new WaitForSeconds(waitTime * addedSeconds);
            StartCoroutine(moveAction(moveCmd.Forward, 0));
            StartCoroutine(moveAction(moveCmd.Stop, (distanceToStart / averageVelocity) * 2));
            Debug.Log("Distance to Start: " + distanceToStart);
        }
        
        Debug.Log("Velocity: " + averageVelocity);
        
        Debug.Log("Angle from Start: " + degreesFromStart);
        Debug.Log("Distance to point: " + Vector3.Distance(Vector3.one, transform.position));

        


    }

    IEnumerator moveAction(Action run, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        run();
    }
	
    void Update () {
		
    }
}