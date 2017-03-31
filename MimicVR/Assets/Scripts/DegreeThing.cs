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
    float rotation = 90;
    float average = 0;
    int count = 0;
    int maxCount = 20;
    float totalSec = 0;
    Boolean isOn = false;
    float firstDegree = 0;
    float secondDegree = 0;
    float howFar = 0;

    [SerializeField]
    Transform target;

    public void getDegree()
    {
        if (transform.rotation.eulerAngles.y > -0.1)
            currentDegree = transform.rotation.eulerAngles.y;
        else
            currentDegree = Mathf.Abs(transform.rotation.eulerAngles.y) + 180;
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

        moveCmd.Right();
    }

    // Update is called once per frame
    void Update()
    {
        // PID controller might work best here, but you can start with this.
        // Also what is our destination?
        if (count < maxCount)
        {
            if (currentDegree % rotation >= rotation - 3 || currentDegree % rotation <= 3)
            {
                endingTime = Environment.TickCount;
            }

            getDegree();

            if (endingTime > 1)
            {
                howLong = endingTime - startingTime;
                if (howLong > 75)
                {
                    startingTime = Environment.TickCount;
                    endingTime = 0;
                    count++;
                    totalSec += howLong;
                    average = totalSec / count;
                    print("Avarage time to rotate " + rotation + "degree is " + average + " milliseconds.");
                }
            }
            moveCmd.Right();
        }
        else if (count == maxCount)
        {
            moveCmd.Stop();
            count++;
        }

        if (isOn)
        {
            moveCmd.Right();
            getDegree();
            secondDegree = currentDegree;
            if (firstDegree >= 270)
            {
                howFar = Math.Abs(firstDegree - 360);
                if (secondDegree > howFar && secondDegree < 90)
                {
                    isOn = false;
                    moveCmd.Stop();
                }
            }
            else
            {
                howFar = firstDegree + rotation;
                if (secondDegree > howFar)
                {
                    isOn = false;
                    moveCmd.Stop();
                }
            }

        }

         if (count > maxCount)
         {
            if (Input.GetKeyUp("f"))
            {
                getDegree();
                firstDegree = currentDegree;
                isOn = true;
                print("done.");
            }

         }

    }

    IEnumerator moveAction(Action run, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        run();
    }

}
