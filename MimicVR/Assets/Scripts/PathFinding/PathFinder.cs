using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PathFinder : MonoBehaviour {

    NavMeshAgent navAgent;

    [SerializeField]
    bool getNewPath = false;

    [SerializeField]
    public Transform target;

    Vector3[] waypoints;

    LineRenderer lineRender;

	// Use this for initialization
	void Start () {

        navAgent = GetComponent<NavMeshAgent>();
        lineRender = GetComponent<LineRenderer>();
    }
	
	// Update is called once per frame
	void Update () {
        if (getNewPath)
        {
            NavMeshPath path = new NavMeshPath();
            navAgent.enabled = true;

            navAgent.CalculatePath(target.position, path);

            //Debug.Log(path.status);

            if (path.status == NavMeshPathStatus.PathPartial || path.status == NavMeshPathStatus.PathComplete)
            {
               // Debug.Log("path calculated.");
                waypoints = path.corners;
                lineRender.SetPositions(waypoints);
            }

            getNewPath = false;
            navAgent.enabled = false;
        }
    }
}
