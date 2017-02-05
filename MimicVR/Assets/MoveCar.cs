using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCar : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		//Vector3 v = new Vector3(0, 0, 1);
		transform.Translate(Vector3.forward * Time.deltaTime, Space.Self);
	}
}
