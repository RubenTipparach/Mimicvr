using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// TODO: Needs Z axis.
/// </summary>
public class AngularPIDDriver : MonoBehaviour
{
	private Rigidbody _rigidbody;

    [SerializeField]
	private float angularForce = 1.0f; // this is how strong your rotation thrusters are

	//[SerializeField]
	private float _rotationalDistanceThreshold = 1;
    private float _acceleration;
    private float _maxAngularVelocity;

    //the following needs variable validation
    private Vector3 _currentTarget;
    private bool _targetSet = false;

	private Vector3 _inputTarget;

	[SerializeField]
	Vector3 RollChange = Vector3.up;

	//some how this got stuck to false, not sure how...
	public bool _isActive;

	public bool IsActive
	{
		get
		{
			return _isActive;
		}
		set
		{
			_isActive = value;
		}
	}

	void Awake()
	{
		IsActive = true;
	}

	// Use this for initialization
	void Start()
	{
		_rigidbody = GetComponent<Rigidbody>();
		_acceleration = angularForce / _rigidbody.mass;

		// compute max angular velocity so it can be decelerated in 1/2 rotation (PI angle)
		_maxAngularVelocity = Mathf.Sqrt(_acceleration * Mathf.PI);
		//_rotationalDistanceThreshold = GetComponent<SpaceObject>().ObjectSize;// should just do the whole sphere, my little shippies are twirling round too much.
    }

	// Update is called once per frame
	void Update()
	{
	}

	//call in FixedUpdate	
	/// <summary>
	/// Rotation algorithm. Please becareful as this is a lot of calculations.
	/// </summary>
	void FixedUpdate()
	{
		if(!IsActive)
		{
			return;
		}

		OldAngularUpdate();
    }

	void OnGUI()
	{
		// DebugLib.DrawLabel(transform, "Distance Rot: " + (Vector3.Distance(transform.position, _inputTarget)), Vector2.up * 15);
		// DebugLib.DrawLabel(transform, "Rot Thresh: " + _rotationalDistanceThreshold, Vector2.up * 30);
	}

	void OldAngularUpdate()
	{
		if ( //_targetSet &&  Why targetset = false? idk
			(Vector3.Distance(transform.position, _inputTarget) > _rotationalDistanceThreshold))
					//|| !GetComponent<ShipObject>().TargetLocked) // if distance is close or not weapon locked. This is for reasons :))
		{
			//Vector3 targetDirection = currentTarget - transform.position;

			CalculateHeadingMode();

			//Debug.DrawLine(transform.position, _inputTarget, Color.red);

			Vector3 localDirection = transform.InverseTransformDirection(_currentTarget);
			Vector3 angularVelocity = transform.InverseTransformDirection(_rigidbody.angularVelocity);

			float upAxisSpeed = angularVelocity.y;
			float rightAxisSpeed = angularVelocity.x;
			float rollAxisSpeed = angularVelocity.z; // z might be kept to 0.

			// SLOWDOWN CODE HERE
			Vector2 upAxisVector = new Vector2(localDirection.x, localDirection.z).normalized; // vector in X-Z plane
			float upAxisAngle = Vector2.Angle(Vector2.up, upAxisVector) * Mathf.Deg2Rad;   // in degrees, convert to RADS!
			float upAxisStaticSign = Mathf.Sign(upAxisVector.x);

			// Same for Right axis = X
			//if z doesnt work use x
			Vector2 rightAxisVector = new Vector2(localDirection.y, localDirection.z).normalized;
			float rightAxisAngle = Vector2.Angle(Vector2.up, rightAxisVector) * Mathf.Deg2Rad;
			float rightAxisStaticSign = Mathf.Sign(-rightAxisVector.x);

			// roll to 0 z.... We are golden! Z can change depending on weapon facing. This seems
			// to be the most effective and efficient weapon-arc-facing strategy.
			Vector3 controllRollDirection = transform.InverseTransformDirection(-(Quaternion.Euler(RollChange)*Vector3.up));

			Vector2 rollAxisVector = new Vector2(controllRollDirection.x, controllRollDirection.y).normalized;

			float rollAxisAngle = Vector2.Angle(Vector2.right, rollAxisVector) * Mathf.Deg2Rad;

			float rollAxisStaticSign = Mathf.Sign(rollAxisVector.x);

			// get forces

			// yaw
			float forceUp = angularForce * GetAxisForce(upAxisAngle, upAxisSpeed, upAxisStaticSign);

			// pitch
			float forceRight = angularForce * GetAxisForce(rightAxisAngle, rightAxisSpeed, rightAxisStaticSign);

			//roll
			float forceRoll = angularForce * GetAxisForce(rollAxisAngle, rollAxisSpeed, rollAxisStaticSign);

			// apply forces
			_rigidbody.AddRelativeTorque(forceUp * Vector3.up);
			_rigidbody.AddRelativeTorque(forceRight * Vector3.right);
			_rigidbody.AddRelativeTorque(forceRoll * Vector3.forward);

			//Debug.Log(string.Format("forceUp: {0}, forceRight: {1}, forceRoll: {2}", forceUp, forceRight, forceRoll));
		}
	}

	// this return 0, -1 or 1, meaning none or max force in either direction
	float GetAxisForce(float axisAngle, float axisSpeed, float directionSign, bool debug = false)
	{
		// this is direction of current rotation, helps to choose direction to reach target
		float dynamicCorrectionSign = Mathf.Sign(axisSpeed * directionSign);

		// this is how long will reach to stop from current velocity OR to accelerate from idle to current velocity
		float timeToStop = Mathf.Abs(axisSpeed / _acceleration);

		// how long would take to reach target in shorter direction if angular velocity is 0
		float staticToReach = Mathf.Sqrt(4 * axisAngle / _acceleration);
		float staticToReach2 = Mathf.Sqrt(4 * (2 * Mathf.PI - axisAngle) / _acceleration); // same for longer direction

        // adjust these durations by current speed (this is basically integral part of velocity)
		float shorterTimeToReach = staticToReach - dynamicCorrectionSign * timeToStop;
		float longerTimeToReach = staticToReach2 + dynamicCorrectionSign * timeToStop;

		// choose direction, considering current velocity
		float force;
		float timeToReach;
		if (shorterTimeToReach <= longerTimeToReach)
		{
			force = directionSign;
			timeToReach = shorterTimeToReach;
		}
		else
		{
			force = -directionSign;
			timeToReach = longerTimeToReach;
		}

        if (debug)
        {
            Debug.Log(string.Format("roll_timeToStop {0}", timeToStop));
        }

		// distance is small enough, consider target rotation reached (this pat could use more love)
		if (Mathf.Abs(timeToReach) < 0.1f)
		{
			// idle
			force = 0;
		}
		else if (timeToStop >= timeToReach)
		{
			// moving too fast, slow down
			force = -force;
		}

		return force;
	}
	
	private void CalculateHeadingMode()
	{
		//// get direction to target in local coordinates
		//if (!GetComponent<ShipObject>().TargetLocked) // update the position as long as its not forced by the weapon manager/ ship object
		//{
		//	_currentTarget = (_inputTarget - transform.position).normalized;
		//}
	}

    /// <summary>
    /// The input target will be a normalized direction, used to rotate the ship
    /// using euler angles.
    /// </summary>
    /// <param name="inputTarget"></param>
	public void GetNewHeading(Vector3 inputTarget)
	{
		_inputTarget = inputTarget;
        _currentTarget = (inputTarget - transform.position).normalized;
		_targetSet = true;
	}

    /// <summary>
    /// This, I'm not sure. We can just do math in the other one.
    /// </summary>
    /// <param name="inputTarget"></param>
    /// <param name="leaderPos"></param>
	/// <remarks>Currently not used!</remarks>
	public void GetNewHeading(Vector3 inputTarget, Vector3 leaderPos)
	{
		_inputTarget = inputTarget;
        _currentTarget = (inputTarget - leaderPos).normalized;
		_targetSet = true;
	}
}

