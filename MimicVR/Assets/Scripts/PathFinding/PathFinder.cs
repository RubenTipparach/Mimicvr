using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PathFinder : MonoBehaviour {

    NavMeshAgent navAgent;

    bool getNetPath = false;

    [SerializeField]
    public Transform target;

    Vector3[] waypoints;

	// Use this for initialization
	void Start () {

        navAgent = GetComponent<NavMeshAgent>();

    }
	
	// Update is called once per frame
	void Update () {
        if (getNetPath)
        {

            NavMeshPath path = new NavMeshPath();
            navAgent.CalculatePath(target.position, path);
            if (path.status == NavMeshPathStatus.PathPartial)
            {
                waypoints = path.corners;
            }
        }
    }
}
