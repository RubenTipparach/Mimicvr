using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToController : MonoBehaviour {

    [SerializeField]
    Transform destinationMarker;

    SteamVR_TrackedObject controller;

    [SerializeField]
    List<PathFinder> getPathTriggers;

    [SerializeField]
    List<Vector3> masterWaypoints;

    [SerializeField]
    SynchTargets offset;

    // Use this for initialization
    void Start () {
		controller = GetComponent<SteamVR_TrackedObject>();
        masterWaypoints = new List<Vector3>();
    }

    // Update is called once per frame
    void Update () {
        //use trigger to drag around menu button.
        if (SteamVR_Controller.Input((int)controller.index).GetTouch(SteamVR_Controller.ButtonMask.Trigger))
        {
            destinationMarker.position = transform.position;
           // pointMarked = true;
           //  startingPoint = car.position;
           //destination = this.transform.position;
        }

        // use application menu to add waypoints
        if (SteamVR_Controller.Input((int)controller.index)
            .GetTouchDown(SteamVR_Controller.ButtonMask.ApplicationMenu))
        {
            masterWaypoints.Add(transform.position);
        }

        if (SteamVR_Controller.Input((int)controller.index).GetTouchDown(SteamVR_Controller.ButtonMask.Grip))
        {
            if(getPathTriggers != null)
            {
                foreach (var path in getPathTriggers)
                {
                    if (masterWaypoints.Count != 0)
                    {
                        if (path.GetComponent<SimulationDataCollector>().isSimBot)
                        {
                            path.uploadWaypoints(masterWaypoints, offset.offsetVector);
                        }
                        else
                        {
                            path.uploadWaypoints(masterWaypoints, Vector3.zero);
                        }

                    }
                    else
                    {
                        // its a bool so that I can be lazy on the UI side.
                        path.getNewPath = true;
                    }
                }

                masterWaypoints.Clear();
            }
        }
    }
}
