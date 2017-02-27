using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// Oh god... look at all the variables!!!
/// </summary>
//[RequireComponent(typeof(InputManager))]
public class ImpulsePIDDriver : MonoBehaviour
{
	[SerializeField]
	private float _maxForce = 100;

    private float _limitedForce = 0;

	[SerializeField]
	private float proportionalGain = 20;

	[SerializeField]
	private float integralGain = 0.5f;

	[SerializeField]
	private float differentialGain = 0.5f;

	Vector3 targetPosition = Vector3.zero;
	Vector3 currentPosition = Vector3.zero;

	Vector3 integrator = Vector3.zero; // error accumulator. Acts as an area under the graph to how much error the movement has gained.
	Vector3 lastError = Vector3.zero;
	Vector3 diff = Vector3.zero;

	float velocity = 0;
	float acceleration = 0;
	float distance = 0;

	Vector3 force = Vector3.zero; //lateral thrusers included.

	[SerializeField]
	bool AllowForwardForceOnly = false;


    [SerializeField]
	private float maxVelocity = 10;

	[SerializeField]
	private float lateralThrustToImpulseRatio = .25f;
	//bool targetReached = true;

	float lastDistance = 0;
	public float deltaApproachThreshold = .01f;

	float timeToMaxAccel = 0;

	float distanceToSlow = 0;

	public bool _isActive;

	public bool _severCollisionDanger = false; //check it out!

	[SerializeField]
	bool debug = false;

	/// <summary>
	/// The maximum speed.
	/// </summary>
	public float MaxSpeed
    {
        get
        {
            return _maxForce;
        }
    }

    /// <summary>
    /// The actual speed. Limited by control factors etc.
    /// </summary>
    public float LimitSpeed
    {
        get
        {
            return _limitedForce;
        }
        set
        {
            _limitedForce = value;
        }
    }

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

	public Vector3 GetTargetPosition
	{
		get
		{
			return targetPosition;
		}
	}

	void Awake()
	{
		IsActive = true;
        _limitedForce = _maxForce;

    }

	// Use this for initialization
	void Start()
	{
		targetPosition = transform.position;
	}

	// Sync with physics
	void FixedUpdate()
	{
		if (!IsActive)
		{
			return;
		}

		OldManualPID();

    }

	// Update is called once per frame
	void Update()
	{
	}

	/// <summary>
	/// The old method. I guess it'll be here for a while.
	/// Now I need logic to avoid collision.
	/// </summary>
	private void OldManualPID()
    {
        var rb = GetComponent<Rigidbody>();
        distance = (currentPosition - targetPosition).magnitude;
        currentPosition = transform.position;

		if (debug) { Debug.DrawLine(transform.position, targetPosition); }

        if (/*!targetReached &&*/
            maxVelocity > rb.velocity.magnitude
            && (lastDistance - distance) >= -deltaApproachThreshold
			&& !_severCollisionDanger) // needs threshold 
        {
            Vector3 error = targetPosition - currentPosition; //the vector from destination to where we are.
            integrator += error * Time.deltaTime; // increments how much error has occured over time.

            Vector3 difference = (error - lastError) / Time.deltaTime;
            lastError = error;

            diff = difference;

            force = error * proportionalGain + integrator * integralGain + difference * differentialGain;
            force = Vector3.ClampMagnitude(force, _limitedForce);

            //timeToMaxAccel = rb.velocity.magnitude * rb.mass / force.magnitude;
            distanceToSlow = rb.velocity.magnitude * rb.mass / force.magnitude;

			Vector3 forwardForce = force.magnitude * transform.forward;
			float angleBetween = Vector3.Angle(transform.forward, (targetPosition - transform.position).normalized);
			//Debug.Log("angle between" + angleBetween);
			float forceOffset = 0;
            if (angleBetween < 90)
			{
				forceOffset = (90 - angleBetween) / 90;// less 90 degrees will accelerate the ship forward uses basic offset here.
				forwardForce = forceOffset * forwardForce;
            }
			else
			{
				forwardForce = Vector3.zero;
            }

			if (AllowForwardForceOnly)
			{
				// changing it to f/4.
				rb.AddForce(forwardForce + force * lateralThrustToImpulseRatio); // + force/10 this last bit here might be useful for big ships that manuever sort distance
				 // more testing to be done here.
			}
			else
			{
				rb.AddForce(forwardForce * force.magnitude);
            }


            velocity = rb.velocity.magnitude;
            acceleration = force.magnitude;
            //Debug.Log("Soft Thrust");
            // acceleration = force/Mass, velocity = acceleration * time, thus, t = v/a = v/(f/m) = vm/f
        }
        else
        //if ((lastDistance - distance) < 0 )//|| targetReached)
        {
            //stabilize ship.
			// what if we eliminated this?
        //    force = -rb.velocity * rb.velocity.magnitude * rb.mass;
        //    velocity = rb.velocity.magnitude;
        //    acceleration = force.magnitude;
        //    distanceToSlow = rb.velocity.magnitude * rb.mass / force.magnitude;
        //    //consider drifting distance, and apply even more force!

        //    rb.AddForce(force);
        //    //Debug.Log("Hard stop");
        }

        //TODO: just clamp the lateral thrusters and we're good!
        lastDistance = distance;

		// collision detection!
		CastCollisionDetection();
    }

    /// <summary>
    /// TODO: A cleaner more reliable PID implementation.
    /// </summary>
    private void PIDController()
    {
        throw new NotImplementedException();
    }

	/// <summary>
	/// Casts the collision detection. Determines if a ship needs to manuever
	/// out of the way. Avoids fancy shmancy ORCA algorithm.
	/// </summary>
	/// <remarks>
	/// This should be disabled if engines are destroyed,
	///  or is under tractor beam influence.
	/// </remarks>
	private void CastCollisionDetection()
	{
		var rigidbody = GetComponent<Rigidbody>();
		Vector3 velocity = rigidbody.velocity;
		float radius2 = 0;
		float seekDistance = 0;

		//first radius is length, second is thickness!
		//float radius2 = GetComponent<SpaceObject>().ObjectSize * 2; //at least 2 spheres! comeon!

		//at least 2 spheres! comeon! Let the speed determine sphere size! muahahaha
		//float seekDistance = GetComponent<SpaceObject>().ObjectSize * (GetComponent<Rigidbody>().velocity.magnitude * 2); 


		RaycastHit[] forwardHit;

		forwardHit = Physics.SphereCastAll(transform.position, radius2, velocity.normalized, seekDistance); // Or maybe just do one? Maybe not because of self :(

		bool detectedCollision = false;

		foreach(var hit in forwardHit)
		{
			//if(hit.transform != transform && hit.transform.GetComponent<SpaceObject>() != null) //bullets don't count, too much of a cluster fuck.
			{

				// get velocity, and compare to object. 
				// Compare these two vectors, and apply force in the difference vector
				// X---------+
				//    \\    /
				//     \\  /   // Actual algorithm uses a plane projection system which gets the avoidance vector
				//      \\/    // This vector will be added into the velocity, we'll see if its any good, but it will falloff when velocity deviates from collision by > 90 degrees.
				//       *     (Exagerated)
				// but assume the [STAR] is a possible [COLLISION], the best avoidance route is the opposite of the two dashes '\\', meaning
				// moving [SLIGHTLY UPWARD] will avoid intersecting the radius of the [STAR]

				
				var velocityNorm = velocity.normalized;
				float speed = rigidbody.velocity.magnitude; //in case I want more gradient.
															// using hit point instead of hit.transform.position

				Vector3 hitData = hit.transform.position; //hit.distance * velocityNorm + transform.position;// 
                Vector3 collisionVector = (hitData - transform.position).normalized;

                detectedCollision = true;
				// I'm calling it a night, we need more parameters to detect the diff between centroid and collision point.
				// if collision is iminent, shut down engines, and drift out of centroid.
				//severity not detected, we should be doing basic raytracing for finer granularity.

				//RaycastHit hitRay;

				//if (Physics.Raycast(transform.position, velocity.normalized * seekDistance, out hitRay))
				//{
				//	float distance = (hitRay.point - transform.position).magnitude;
				//	if (distance < radius2 * 2)
				//	{
				//		_severCollisionDanger = true;
				//	}
				//	else
				//	{
				//		_severCollisionDanger = false;
				//	}
				//}

				if (debug)
				{
					//Debug.DrawLine(transform.position, velocityNorm * seekDistance + transform.position, Color.yellow); // how far ahead am I looking?
					Debug.DrawLine(transform.position, hitData, Color.red); // What dangers lie ahead? ;use speed for more precision & hit.point?
				}

				float angleBetween = Vector3.Angle(velocityNorm, collisionVector);
				//Debug.Log("angle between" + angleBetween);
				
				// If a ship is behind you or chasing you, ignore it.
				if (angleBetween < 135)
				{
					// what is the angle of difference? 
					// So we got up, and right, it all depends on what is the best right?
					// One more step for accuracy maybe to check which quadrant this thing falls in.
					//Vector3 diffVector = Vector3.Cross(velocity,transform.right);
					Vector3 diffVector = Vector3.Cross(collisionVector - velocityNorm, transform.right); //umm, one direction makes them keep trying to dive :/

					//project onto plane of velocity. // Funny how if this vector is positive, it ATTRACTS, if it is negated, it REPELS!
					Vector3 projectedVtr = -Vector3.ProjectOnPlane(collisionVector, velocityNorm).normalized;

					if (debug)
					{
						//Debug.DrawLine(transform.position, projectedVtr * radius2 + transform.position, Color.cyan);
					}

					// use 1/4 limited force(allocated via engineering).
					rigidbody.AddForce(projectedVtr * (_limitedForce));
					
				}
            }
		}

		if(!detectedCollision)
		{
			_severCollisionDanger = false;
        }
    }

	// used for debuggin ship speeds.
	void OnGUI()
	{
		/*
		Vector3 v = Camera.main.WorldToScreenPoint(transform.position);

        GUI.Label(new Rect(new Vector2(v.x, Screen.height - v.y - 20)
			, new Vector2(200, 50)), "Speed: " + GetComponent<Rigidbody>().velocity.magnitude);
		*/
	}

	/* 
	*  Illustration: the double thick line represents the projection direction, hence the dot product.
	*  The result is the brackets, which is the vector that we wont. Damn that's smart.
	*     *      x } star is the object queried, X is the heading of the ship.
	*   || \     ^
	*   ||  \    |
	*   ||   \   |
	*   ||    \  | } forward vector (velocity)
	*   ||     \ |
	* --[]<------S--
	*/
	private Vector3 ProjectOnPlane(Vector3 positionOffsetVector, Vector3 planeNormal)
	{
		var distance = -Vector3.Dot(planeNormal, (positionOffsetVector));
		return positionOffsetVector + planeNormal * distance;
    }

    public void GetNewHeading(Vector3 inputTarget)
	{
		//Debug.Log("New heading set for " + gameObject.name + " " + inputTarget);
		targetPosition = inputTarget;
	}

    /// <summary>
    /// Saving this for the advanced fleet management stuff.
    /// </summary>
    /// <param name="netFormationForce"></param>
    public void UpdateHeadingVelocity(Vector3 netFormationForce)
    {
        // oh shit this is gonna be fun!
    }
}

