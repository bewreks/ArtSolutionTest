using UnityEngine;

namespace Components
{
	public struct RotateToComponent
	{
		public Quaternion FromRotation;
		public Quaternion FinalRotation;
		public float      TotalTime;
		public float      Time;
	}
}