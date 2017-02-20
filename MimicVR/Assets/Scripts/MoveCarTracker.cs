using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCarTracker : MonoBehaviour {

	[SerializeField]
	Transform trackingObj;

	SteamVR_TrackedObject trackedController;

	[SerializeField]
	Transform controllingObj;

	SteamVR_TrackedObject controller;

	[SerializeField]
	Vector3 initialRotation;
	//public Transform trackingObj1;

	Quaternion initialQuat;

	private IEnumerator coroutine;

	// Use this for initialization
	void Start () {
		trackedController = trackingObj.GetComponent<SteamVR_TrackedObject>();
		initialRotation = trackingObj.rotation.eulerAngles;
		//controller = controllingObj.GetComponent<SteamVR_TrackedObject>();

		coroutine = GetRotationOffset(2.0f);

		StartCoroutine(coroutine);

	}

	private IEnumerator GetRotationOffset(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		Debug.Log("Rotation set!");
		initialQuat = trackingObj.rotation;
	}

	// Update is called once per frame
	void Update()
	{
		transform.position = trackingObj.position;
		transform.rotation = trackingObj.rotation * Quaternion.Inverse(initialQuat);
		//var rotVel = SteamVR_Controller.Input((int)trackedController.index).angularVelocity;
		///var gravityVector = SteamVR_Controller.Input((int)trackedController.index).velocity;

		//Debug.Log("Acceleration: " + gravityVector);


	}
}
