using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public class PIDV
{
	public readonly float pFactor, iFactor, dFactor;

	Vector3 integral;
	Vector3 lastError;

	public PIDV(float pFactor, float iFactor, float dFactor)
	{
		this.pFactor = pFactor;
		this.iFactor = iFactor;
		this.dFactor = dFactor;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="setpoint"></param>
	/// <param name="actual"></param>
	/// <param name="timeFrame"></param>
	/// <returns></returns>
	public Vector3 Update(Vector3 setpoint, Vector3 actual, float timeFrame)
	{
		Vector3 present = setpoint - actual;

		integral += present * timeFrame;

		Vector3 deriv = (present - lastError) / timeFrame;

		lastError = present;

		return present * pFactor + integral * iFactor + deriv * dFactor;
	}
}