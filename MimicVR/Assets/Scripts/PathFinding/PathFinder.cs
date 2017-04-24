using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PathFinder : MonoBehaviour {

    NavMeshAgent navAgent;

    [SerializeField]
    public bool getNewPath = false;

    [SerializeField]
    public Transform target;

    [SerializeField]
    public Transform masterTarget;

    [SerializeField]
    LayerMask layerMask;

    //[SerializeField]
    Vector3[] _waypoints;

    [SerializeField]
    LineRenderer lineRender;

    public int movingToPosition;

    [SerializeField]
    Transform hullTransform;


    [SerializeField]
    float nextWaypointThreshold = .1f;


    [SerializeField]
    float floorOffset = 0.5f;

    // Use this for initialization
    void Start () {
        floorOffset = transform.position.y;
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.enabled = false;
        //lineRender = GetComponent<LineRenderer>();
    }
	
    /// <summary>
    /// When get new path is triggered, the program gets the path from Unity's native
    /// NavMesh agent path finding algorithm.
    /// getNewPath variable is used this way since it allows users to manually trigger
    /// path calculation when neeeded.
    /// </summary>
	void Update () {
        if (getNewPath)
        {
            NavMeshPath path = new NavMeshPath();
            navAgent.enabled = true;

            navAgent.CalculatePath(masterTarget.position, path);

            if(path != null)
            {

                //Debug.Log(path.status);

                if (path.status == NavMeshPathStatus.PathPartial || path.status == NavMeshPathStatus.PathComplete)
                {
                     Debug.Log("path calculated.");
                
                    _waypoints = path.corners;
                    lineRender.numPositions = _waypoints.Length;
                    lineRender.SetPositions(_waypoints);

                    // start from 0.
                    movingToPosition = 0;

                    target.position = _waypoints[0];
                }
            }

            getNewPath = false;
            navAgent.enabled = false;
        }

        if(movingToPosition != lineRender.numPositions - 1 && _waypoints != null)
        {
            //keep testing the car and move it forward continuously
            var colliders = Physics.OverlapSphere(_waypoints[movingToPosition], nextWaypointThreshold, layerMask);

            Debug.Log("colliders detected " );

            foreach (var w in _waypoints)
            {
                Debug.Log(w);
            }

            foreach (var c in colliders)
            {
                if (c.transform == hullTransform)
                {
                    Debug.Log("next position");

                    movingToPosition++;
                    target.position = _waypoints[movingToPosition];
                }
            }
        }
    }

    /// <summary>
    /// Waypoints from user gets substituted here.
    /// </summary>
    /// <param name="waypoints"></param>
    /// <param name="offset"></param>
    public void uploadWaypoints(List<Vector3> waypoints, Vector3 offset)
    {
        List<Vector3> copiedList = new List<Vector3>(waypoints.Count);

        foreach(var w in waypoints)
        {
            copiedList.Add(new Vector3(w.x, floorOffset, w.z));
        }

        Debug.Log("path calculated.");

        this._waypoints = copiedList.ToArray();
        lineRender.numPositions = this._waypoints.Length;
        lineRender.SetPositions(this._waypoints);

        // start from 0.
        movingToPosition = 0;

        target.position = this._waypoints[0];
    }
}
