using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SynchTargets : MonoBehaviour {

    [SerializeField]
    public Vector3 offsetVector;

    [SerializeField]
    Transform masterTarget;

	// Use this for initialization
	void Start () {
        offsetVector = transform.position - masterTarget.position;
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = masterTarget.position + offsetVector;
	}
}
