using SocketIO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationDataCollector : MonoBehaviour
{

    [SerializeField]
    SocketIOComponent socketDataCollector;

    [SerializeField]
    Transform trackingAgent;

    // Use this for initialization
    void Start () {
        // some bug requires this test message to be sent first.
        socketDataCollector.Emit("test");
    }
	
	// Update is called once per frame
	void Update () {
       

    }

    public void commandFwd(string input)
    {
        var jso = JSONExt.ToJSO(
       new RobotData()
       {
           position = trackingAgent.position,
           direction = trackingAgent.rotation.eulerAngles,
           command = input
       });

       Debug.Log(jso);

       socketDataCollector.Emit("robot_sim_collect_data", jso);
    }
}
