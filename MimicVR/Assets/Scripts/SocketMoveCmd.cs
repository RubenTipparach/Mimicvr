using SocketIO;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SocketMoveCmd : MonoBehaviour {

	// Use JSON objects for serialization!
	[SerializeField]
	SocketIOComponent socket;

	// Use this for initialization
	void Start () {

		socket.On("reply", (SocketIOEvent e) => {
			Debug.Log(string.Format("[name: {0}, data: {1}]", e.name, e.data));
		});
	}

	// Update is called once per frame
	void Update () {
		//socket.Emit("test");

		if(Input.GetKeyDown(KeyCode.W))
		{
			RobotCommand("f");
		}

		if (Input.GetKeyDown(KeyCode.S))
		{
			RobotCommand("s");
		}

		if (Input.GetKeyDown(KeyCode.X))
		{
			RobotCommand("b");
		}

		if (Input.GetKeyDown(KeyCode.A))
		{
			RobotCommand("l");
		}

		if (Input.GetKeyDown(KeyCode.D))
		{
			RobotCommand("r");
        }
	}

	void RobotCommand(string input)
	{
		//input = string.Format("{{ \"command\" : \"{0}\" }}", input);
        Debug.Log("input: " + input);
		socket.Emit("test");

		//socket.Emit("robot-command", JSONObject.CreateStringObject(input));
		socket.Emit("robot-command", JSONExt.ToJSO(new RobotCommand() { command = input }));
	}
}

public static class JSONExt
{
	public static JSONObject ToJSO<T>(T input)
	{
		//Debug.Log("json: " + JsonUtility.ToJson(input));
		return JSONObject.Create(JsonUtility.ToJson(input));
    }
}

[Serializable]
public class RobotCommand
{
	public string command;
}


public class HelloEvent
{
	public string Hello { get; set; }

	public JSONObject data { get; set; }

	public override string ToString()
	{
		return string.Format("[SocketIOEvent: name={0}, data={1}]", Hello, data);
	}
}
