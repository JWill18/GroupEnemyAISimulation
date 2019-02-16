using GroupEnemyAISimulation.Assets.Scripts.Player;
using UnityEngine;

namespace GroupEnemyAISimulation.Assets.Scripts.Camera
{
	public class CameraController : MonoBehaviour
	{
		public PlayerControls FocusObject;

		public float CameraDistanceLagX;

		public float CameraDistanceLagZ;

		public float MoveSpeed;

		// Use this for initialization
		void Start()
		{

		}

		// Update is called once per frame
		void Update()
		{
			if(!IsPlayerInView())
			{
				var distanceFromFocusObject = FocusObject.transform.position - transform.position;

				var moveX = 0.0f;
				var moveZ = 0.0f;

				if(!IsPlayerInViewNegativeX(distanceFromFocusObject.x))
				{
					moveX = -MoveSpeed;
				}
				else if (!IsPlayerInViewPositiveX(distanceFromFocusObject.x))
				{
					moveX = MoveSpeed;
				}

				if(!IsPlayerInViewNegativeZ(distanceFromFocusObject.z))
				{
					moveZ = -MoveSpeed;
				}
				else if (!IsPlayerInViewPositiveZ(distanceFromFocusObject.z))
				{
					moveZ = MoveSpeed;
				}

				transform.Translate(moveX * Time.deltaTime, 0, moveZ * Time.deltaTime);
			}
		}

		private bool IsPlayerInView()
		{
			var distanceFromFocusObject = transform.position - FocusObject.transform.position;

			var withinXView = IsPlayerInViewNegativeX(distanceFromFocusObject.x) && IsPlayerInViewPositiveX(distanceFromFocusObject.x);

			var withinZView = IsPlayerInViewNegativeZ(distanceFromFocusObject.z) && IsPlayerInViewPositiveZ(distanceFromFocusObject.z);

			return withinXView && withinZView;
		}

		private bool IsPlayerInViewPositiveX(float xDifference)
		{
			return xDifference < CameraDistanceLagX;
		}

		private bool IsPlayerInViewNegativeX(float xDifference)
		{
			return xDifference > -CameraDistanceLagX;
		}

		private bool IsPlayerInViewPositiveZ(float ZDifference)
		{
			return ZDifference < CameraDistanceLagZ;
		}

		private bool IsPlayerInViewNegativeZ(float ZDifference)
		{
			return ZDifference > 5.0f;
		}
	}
}
