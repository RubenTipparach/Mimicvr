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

    [SerializeField]
    public Transform masterTarget;

    [SerializeField]
    LayerMask layerMask;

    Vector3[] waypoints;

    [SerializeField]
    LineRenderer lineRender;

    public int movingToPosition;

    [SerializeField]
    Transform hullTransform;


    [SerializeField]
    float nextWaypointThreshold = .1f;

    // Use this for initialization
    void Start () {

        navAgent = GetComponent<NavMeshAgent>();
        //lineRender = GetComponent<LineRenderer>();
    }
	
	// Update is called once per frame
	void Update () {
        if (getNewPath)
        {
            NavMeshPath path = new NavMeshPath();
            navAgent.enabled = true;

            navAgent.CalculatePath(masterTarget.position, path);

            //Debug.Log(path.status);

            if (path.status == NavMeshPathStatus.PathPartial || path.status == NavMeshPathStatus.PathComplete)
            {
                // Debug.Log("path calculated.");
                
                waypoints = path.corners;
                lineRender.numPositions = waypoints.Length;
                lineRender.SetPositions(waypoints);

                // start from 0.
                movingToPosition = 0;

                target.position = waypoints[0];
            }

            getNewPath = false;
            navAgent.enabled = false;
        }

        if(movingToPosition != lineRender.numPositions-1 && waypoints != null)
        {
            //keep testing the car and move it forward continuously
            var colliders = Physics.OverlapSphere(waypoints[movingToPosition], nextWaypointThreshold, layerMask);

            Debug.Log("colliders detected " + colliders.Length);

            foreach (var c in colliders)
            {
                Debug.Log(c.name);
                if (c.transform == hullTransform)
                {
                    movingToPosition++;
                    target.position = waypoints[movingToPosition];
                }
            }
        }
    }
}
