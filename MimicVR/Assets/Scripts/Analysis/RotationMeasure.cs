using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationMeasure : MonoBehaviour {

	[SerializeField]
	SocketMoveCmd socket;

	[SerializeField]
	LocalMoveCmd local;

	RobotCommandInput moveCmd;

	float currentDelta;

	float currentAngle;

	float lastAngle;

	// Use this for initialization
	void Start()
	{
		if (socket != null)
		{
			moveCmd = socket;
		}
		else
		{
			moveCmd = local;
		}

		moveCmd.Left();

		lastAngle = transform.rotation.eulerAngles.y;
		StartCoroutine(AnalyzeDegreeSeperation(1));
	}

	private IEnumerator AnalyzeDegreeSeperation(float waitTime)
	{
		while(true)
		{
			yield return new WaitForSeconds(waitTime);

			lastAngle = currentAngle;
			currentAngle = transform.rotation.eulerAngles.y;

			currentDelta = lastAngle - currentAngle;

			Debug.Log("Delta Angle: " + currentDelta);
		}
	}

	// Update is called once per frame
	void Update () {
		
	}
}
