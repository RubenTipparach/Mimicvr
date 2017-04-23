using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToController : MonoBehaviour {

    [SerializeField]
    Transform destinationMarker;

    SteamVR_TrackedObject controller;

    [SerializeField]
    List<PathFinder> getPathTriggers;

    // Use this for initialization
    void Start () {
		controller = GetComponent<SteamVR_TrackedObject>();
    }

    // Update is called once per frame
    void Update () {
        if (SteamVR_Controller.Input((int)controller.index).GetTouch(SteamVR_Controller.ButtonMask.Trigger))
        {
            destinationMarker.position = transform.position;
           // pointMarked = true;
           //  startingPoint = car.position;
           //destination = this.transform.position;
        }

        if (SteamVR_Controller.Input((int)controller.index).GetTouch(SteamVR_Controller.ButtonMask.Grip))
        {
            if(getPathTriggers != null)
            {
                foreach(var path in getPathTriggers)
                {
                    // its a bool so that I can be lazy on the UI side.
                    path.getNewPath = true;
                }
            }
        }
    }
}
