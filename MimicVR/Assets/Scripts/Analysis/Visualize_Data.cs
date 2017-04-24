using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Visualize_Data : MonoBehaviour {
    [SerializeField]
    TextAsset simText;

    [SerializeField]
    TextAsset vrText;

    [SerializeField]
    SimData simData;

    [SerializeField]
    VrData vrData;

    [SerializeField]
    bool reload = false;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if(reload)
        {
            simData = JsonUtility.FromJson<SimData>(simText.text);
            vrData = JsonUtility.FromJson<VrData>(vrText.text);
            reload = false;
        }

        // draw sim text
        foreach(var s in simData.data)
        {
            Vector3 dir =  Quaternion.Euler(s.robot_sim.direction) * Vector3.forward;
            Debug.DrawLine(s.robot_sim.position, s.robot_sim.position + dir * .1f, Color.green);
        }

        // draw vr text
        foreach (var v in vrData.data)
        {
            Vector3 dir = Quaternion.Euler(v.robot_vr.direction) * Vector3.forward;
            Debug.DrawLine(v.robot_vr.position, v.robot_vr.position + dir * .1f, Color.red);
        }
    }

    [Serializable]
    public class SimData
    {
        public RoboWrapperSim[] data;
    }

    [Serializable]
    public class RoboWrapperSim
    {
        public RobotData robot_sim;
    }

    [Serializable]
    public class VrData
    {
        public RoboWrapperVr[] data;
    }

    [Serializable]
    public class RoboWrapperVr
    {
        public RobotData robot_vr;
    }
}
