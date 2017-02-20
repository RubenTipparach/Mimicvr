using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViveRemoteController : MonoBehaviour {

	SteamVR_TrackedObject controller;

	[SerializeField]
	Transform car;

	[SerializeField]
	GameObject lineRenderer;

	Vector3 startingPoint;

	Vector3 destination;

	bool pointMarked = false;

	// Use this for initialization
	void Start () {
		controller = GetComponent<SteamVR_TrackedObject>();
    }
	
	// Update is called once per frame
	void Update () {

		if (SteamVR_Controller.Input((int)controller.index).GetTouch(SteamVR_Controller.ButtonMask.Trigger))
		{
			Debug.Log("stuff");
			pointMarked = true;
			startingPoint = car.position;
			destination = this.transform.position;
		}

		if(pointMarked)
		{
			car.transform.Translate((destination - car.transform.position).normalized * Time.deltaTime);
			Debug.DrawLine(startingPoint, destination, Color.green);
		}
	}

	void OnTriggerEnter(Collider c)
	{

	}
}
