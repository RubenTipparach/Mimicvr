using UnityEngine;
using System.Collections;

public class ShipControllerAngularPID : MonoBehaviour {

	Vector3 rotationTarget = Vector3.zero;
	Vector3 target = Vector3.zero;
	public float targetAngle = 0;
	
    //shouldnt touch these in general
	[SerializeField]
	float currentAngle;
	[SerializeField]
	float acceleration;
	[SerializeField]
	float angleSpeed = 0;

	public float maxAcceleration = 180;// degrees per second^2
	public float maxAngularSpeed = 90; //degrees per second

	public float porportionalGain = 20;
	public float differentialGain = 10;

	float lastError;
	bool orientToTarget = false;

	// Use this for initialization
	void Start () {
		// For the initialization, we are going to use the y axis. Rotation about the y axis gives us a good pivot for calculating XZ cordinates.
		// Consider using relative angles in order to find the nearest possible rotation.
		targetAngle = transform.eulerAngles.y;
		currentAngle = targetAngle;
		
		// Initialize pitiching for the YZ plane.
		// I remember modifying this code, but not understanding it because I took a trial/error approach.
		// It was highly unstable and didn't work well for rotating the ship. It was glorious however.
	}
	
	// Rule of thumb, all physics stuff should go here.
	void FixedUpdate()
	{
		// The goal (I think) is to reduce this error by adjusting the left/right thrusters.
		float error = targetAngle - currentAngle;// Mathf.DeltaAngle(curPos, targetPos);
		float differentialError = (error - lastError) / Time.deltaTime;
		lastError = error;

		acceleration = error * porportionalGain + differentialError * differentialGain;
		acceleration = Mathf.Clamp(acceleration, -maxAcceleration, maxAcceleration);

		angleSpeed += acceleration * Time.deltaTime;
		angleSpeed = Mathf.Clamp(angleSpeed, -maxAngularSpeed, maxAngularSpeed);

		currentAngle += angleSpeed * Time.deltaTime;

		gameObject.GetComponent<Rigidbody>().rotation = Quaternion.Euler(0, currentAngle % 360, 0);

		//switcheroo the target angle.
		if(currentAngle > 360)
		{
			currentAngle = currentAngle - 360;
			targetAngle = targetAngle - 360;
		}

		if (currentAngle < -360)
		{
			currentAngle = currentAngle + 360;
			targetAngle = targetAngle + 360;
		}
	}

	void GetNewHeading()
	{
		//if (Input.GetMouseButtonDown(0))
		if (Input.GetMouseButton(0))
		{
			Plane p = new Plane(Vector3.up, 0);
			Camera camera = GameObject.Find("Main Camera").GetComponent<Camera>();
			Ray ray = camera.ScreenPointToRay(Input.mousePosition);
			float distance;
			if (p.Raycast(ray, out distance))
			{
				//Gets normalized vector
				target = ray.GetPoint(distance) ;
				orientToTarget = true;
				//to save float space ;)
				//float compressSign = Mathf.Sign(targetAngle);
				//targetAngle = (Mathf.Abs(targetAngle) %360) * compressSign;

				//Debug.Log("sign = " + Vector3.Cross(transform.forward, rotationTarget).y);
				//Debug.Log("angle = " + Vector3.Angle(transform.forward, rotationTarget));
			}
		}

		if (Vector3.Distance(target, transform.position) < 20)
		{
			orientToTarget = false;
		}

		//continously update this.
		//gets the sign, and the shortest angle. Adds on to actual current angle, always considering the negative and positives of the target angle.
		if (orientToTarget)
		{
			rotationTarget = (target - transform.position).normalized;
			int sign = Vector3.Cross(transform.forward, rotationTarget).y < 0 ? -1 : 1;
			targetAngle = (Vector3.Angle(transform.forward, rotationTarget) * sign + currentAngle);
		}
	}

	// Update is called once per frame
	void Update () {
		GetNewHeading();
	}
}
