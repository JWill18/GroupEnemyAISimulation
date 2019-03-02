using GroupEnemyAISimulation.Assets.Scripts.Player;
using UnityEngine;

namespace GroupEnemyAISimulation.Assets.Scripts.Camera
{
	public class CameraController : MonoBehaviour
	{
		/// <summary>
		/// The object that the camera will be focused on
		/// </summary>
		public PlayerControls FocusObject;

		/// <summary>
		/// The amount of room that the object has in the x axis to move before the camera will follow
		/// </summary>
		public float CameraDistanceLagX;

		/// <summary>
		/// The amount of room that the object has in the z axis to move before the camera will follow
		/// </summary>
		public float CameraDistanceLagZ;

		/// <summary>
		/// The movement speed of the camera
		/// </summary>
		public float MoveSpeed;

		// Use this for initialization
		void Start()
		{

		}

		// Update is called once per frame
		void Update()
		{
			// If the object is not within view. Adjust camera position
			if(!IsObjectInView())
			{
				// Find the difference in camera position and the focus object position
				var distanceFromFocusObject = FocusObject.transform.position - transform.position;

				// determines how to move and in what axis
				var moveX = 0.0f;
				var moveZ = 0.0f;

				// If the object is out of view in the x axis, determine which direction the camera will need to go
				if(!IsObjectInViewNegativeX(distanceFromFocusObject.x))
				{
					moveX = -MoveSpeed;
				}
				else if (!IsObjectInViewPositiveX(distanceFromFocusObject.x))
				{
					moveX = MoveSpeed;
				}

				// If the object is out of view in the z axis, determine which direction the camera will need to go
				if (!IsObjectInViewNegativeZ(distanceFromFocusObject.z))
				{
					moveZ = -MoveSpeed;
				}
				else if (!IsObjectInViewPositiveZ(distanceFromFocusObject.z))
				{
					moveZ = MoveSpeed;
				}

				// Move the camera towards the object
				transform.Translate(moveX * Time.deltaTime, 0, moveZ * Time.deltaTime);
			}
		}

		#region Camera Detection

		/// <summary>
		/// Checks to see if the Focus Object is within sight of the camera 
		/// </summary>
		/// <returns>The camera is within acceptable bounds of the camera's view</returns>
		private bool IsObjectInView()
		{
			// Finds the distance between the focus object and the camera object
			var distanceFromFocusObject = transform.position - FocusObject.transform.position;

			// Check to see if the object is within view in the x axis
			var withinXView = IsObjectInViewNegativeX(distanceFromFocusObject.x) && IsObjectInViewPositiveX(distanceFromFocusObject.x);

			// Check to see if the object is within view in the z axis
			var withinZView = IsObjectInViewNegativeZ(distanceFromFocusObject.z) && IsObjectInViewPositiveZ(distanceFromFocusObject.z);

			return withinXView && withinZView;
		}

		/// <summary>
		/// Checks to see if the Focus Object is within sight of the camera in the positive x axis
		/// </summary>
		/// <param name="xDifference">The difference value between the camera location and the focus object location in the x axis</param>
		/// <returns>The camera is with acceptable bounds of the camera's view in the positive x axis</returns>
		private bool IsObjectInViewPositiveX(float xDifference)
		{
			return xDifference < CameraDistanceLagX;
		}

		/// <summary>
		/// Checks to see if the Focus Object is within sight of the camera in the negative x axis
		/// </summary>
		/// <param name="xDifference">The difference value between the camera location and the focus object location in the x axis</param>
		/// <returns>The camera is with acceptable bounds of the camera's view in the negative x axis</returns>
		private bool IsObjectInViewNegativeX(float xDifference)
		{
			return xDifference > -CameraDistanceLagX;
		}

		/// <summary>
		/// Checks to see if the Focus Object is within sight of the camera in the positive z axis
		/// </summary>
		/// <param name="ZDifference">The difference value between the camera location and the focus object location in the z axis</param>
		/// <returns>The camera is with acceptable bounds of the camera's view in the positive z axis</returns>
		private bool IsObjectInViewPositiveZ(float ZDifference)
		{
			return ZDifference < CameraDistanceLagZ;
		}

		/// <summary>
		/// Checks to see if the Focus Object is within sight of the camera in the negative x axis
		/// </summary>
		/// <param name="ZDifference">The difference value between the camera location and the focus object location in the z axis</param>
		/// <returns>The camera is with acceptable bounds of the camera's view in the negative z axis</returns>
		private bool IsObjectInViewNegativeZ(float ZDifference)
		{
			return ZDifference > 5.0f;
		}
		#endregion
	}
}
