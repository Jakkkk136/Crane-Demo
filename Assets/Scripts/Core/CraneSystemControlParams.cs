using System;
using Enums;
using UnityEngine;

namespace Core
{
	/// <summary>
	/// Contains fields and methods related to changing given float param of some crane system
	/// </summary>
	[Serializable]
	public class CraneSystemControlParams
	{
		[SerializeField] private float changeSpeed;
		[SerializeField] private float smoothingTime;
		[Space] 
		[SerializeField] private bool haveLimit;
		[SerializeField] private Vector2 changeLimit;

		private float currentSpeed;
		private float refCurrentSpeed;
		private eMoveDirection lastChangeDirection;
		
		/// <summary>
		/// Function for calculating crane param change. Applies smoothing on move start.
		/// </summary>
		/// <param name="currentDirection"> Direction of changing crane system param</param>
		/// <param name="currentSystemParamValue"> Current crane system param value, used for limiting returned value</param>
		/// <returns>Smoothed and limited change in crane system param value</returns>
		public float GetCraneSystemParamChange(eMoveDirection currentDirection, float currentSystemParamValue)
		{
			if (lastChangeDirection != currentDirection)	//If direction of move is changed, reset current speed for applying smoothing
			{
				lastChangeDirection = currentDirection;
				currentSpeed = 0f;
				refCurrentSpeed = 0f;
			}

			//Smoothing
			currentSpeed = Mathf.SmoothDamp(
				currentSpeed, changeSpeed, ref refCurrentSpeed, smoothingTime);

			float changeAmount = (int)currentDirection * currentSpeed * Time.fixedDeltaTime;

			float limitedValue;

			//Applying limitation to returned param value
			if (haveLimit)
			{
				limitedValue = currentDirection == eMoveDirection.positive
					? changeLimit.y - currentSystemParamValue
					: changeLimit.x - currentSystemParamValue;
			}
			else
			{
				limitedValue = changeAmount;
			}
			
			return currentDirection == eMoveDirection.positive ? 
				Mathf.Min(changeAmount, limitedValue) : 
				Mathf.Max(changeAmount, limitedValue);
		}
	}
}