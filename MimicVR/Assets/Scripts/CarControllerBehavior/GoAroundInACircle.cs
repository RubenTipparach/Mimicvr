using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoAroundInACircle : MonoBehaviour {

	[SerializeField]
	SocketMoveCmd socket;

	[SerializeField]
	LocalMoveCmd local;

	RobotCommandInput moveCmd;

	// Use this for initialization
	void Start () {
		if(socket != null)
		{
			moveCmd = socket;
		}
		else
		{
			moveCmd = local;
		}

		//StartCoroutine(startDelay(0));
    }

	IEnumerator startDelay(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);

		float addedSeconds = 1;

		StartCoroutine(moveAction(moveCmd.Forward, 1 * addedSeconds));
		StartCoroutine(moveAction(moveCmd.Left, 2 * addedSeconds));
		StartCoroutine(moveAction(moveCmd.Forward, 3 * addedSeconds));
		StartCoroutine(moveAction(moveCmd.Left, 4 * addedSeconds));
		StartCoroutine(moveAction(moveCmd.Forward, 5 * addedSeconds));
		StartCoroutine(moveAction(moveCmd.Stop, 6 * addedSeconds));

	}

	IEnumerator moveAction(Action run, float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		run();
	}

	// Update is called once per frame
	void Update () {
		
	}

}
