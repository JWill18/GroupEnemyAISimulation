using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GroupEnemyAISimulation.Assets.Scripts.Math
{
	/// <summary>
	/// Used for calculating angles around an object. Based on a -180.0 to 180.0 scale
	/// </summary>
	public class MathAngle
	{
		/// <summary>
		/// Determines if the rotation is greater than the opposite designated angle. Ex. The opposite of Base Angle '120.0' is '-60.0' 
		/// </summary>
		/// <param name="currentRotation">The current rotation angle</param>
		/// <param name="baseAngle">The base angle that will be flipped to determine the opposite angle.</param>
		/// <returns>Stating if the current rotation is greater than the opposite angle of the base angle.</returns>
		public static bool IsGreaterThanFlippedAngle(float currentRotation, float baseAngle)
		{
			bool greaterThanAngle = false;

			if (baseAngle > 0)
				greaterThanAngle = currentRotation > -180.0f + baseAngle;
			else
				greaterThanAngle = currentRotation > 180.0f + baseAngle;

			return greaterThanAngle;
		}

		/// <summary>
		/// Determiens if the rotation is less than the opposite designated angle. Ex. The opposite of Base Angle '120.0' is '-60.0'
		/// </summary>
		/// <param name="currentRotation">The current rotation angle</param>
		/// <param name="baseAngle">The base angle that will be flipped to determine the opposite angle.</param>
		/// <returns>Stating if the current rotation is less than the opposite angle of the base angle</returns>
		public static bool IsLessThanFlippedAngle(float currentRotation, float baseAngle)
		{
			bool lessThanAngle = false;

			if (baseAngle > 0)
				lessThanAngle = currentRotation <= -180.0f + baseAngle;
			else
				lessThanAngle = currentRotation <= 180.0f + baseAngle;

			return lessThanAngle;
		}

		/// <summary>
		/// Determines if the current rotation needs to be rotated in the clockwise direction to be within the appropriate angle limit
		/// </summary>
		/// <param name="currentRotation">The current rotation angle</param>
		/// <param name="baseAngle">The base angle that will stand as the central angle that all calculations are based on</param>
		/// <param name="angleLimit">The limit of angles that are allowed. Number should be greater than base angle.</param>
		/// <returns>Returns if the current angle will need to be adjusted to fit within the bounds of the base angle and angle limit</returns>
		public static bool NeedToAdjustClockwise(float currentRotation, float baseAngle, float angleLimit)
		{
			if (baseAngle > 0)
				//  Current rotation is less than the limit AND Current rotation is greater than the opposite angle
				return currentRotation <= angleLimit && IsGreaterThanFlippedAngle(currentRotation, baseAngle);
			else
				//  (Limit is greater than -180 AND Current rotation is less than limit) OR (Limit is less than or equal to -180 AND current rotation is less than converted angle limit AND Current rotation is greater than the opposite angle of base)
				return (angleLimit >= -180.0f && currentRotation <= angleLimit) || (angleLimit <= -180.0f && currentRotation <= (180.0f - (-angleLimit - 180.0f)) && IsGreaterThanFlippedAngle(currentRotation, baseAngle));
		}

		/// <summary>
		/// Determiens if the current rotation needs to be rotated in the counter-clockwise direction to be within in the appropriate angle limit
		/// </summary>
		/// <param name="currentRotation">The current rotation angle</param>
		/// <param name="baseAngle">The base angle that will stand as the central angle that all calculations are based on</param>
		/// <param name="angleLimit">The limit of angles that are allowed. Number should be less than the base angle.</param>
		/// <returns>Returns if the current angle will need to be adjusted to fit within the bounds of the base angle and the angle limit</returns>
		public static bool NeedToAdjustCounterClockwise(float currentRotation, float baseAngle, float angleLimit)
		{
			if (baseAngle > 0)
				//  Current rotation greater than limit OR  Current rotation is less than the opposite angle of base
				return currentRotation >= angleLimit || IsLessThanFlippedAngle(currentRotation, baseAngle);
			else
				//  Current rotation greater than limit AND Current rotation is less than the opposite angle of base
				return currentRotation >= angleLimit && IsLessThanFlippedAngle(currentRotation, baseAngle);
		}
	}
}
