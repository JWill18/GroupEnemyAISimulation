using GroupEnemyAISimulation.Assets.Scripts.Player;
using UnityEngine;

namespace GroupEnemyAISimulation.Assets.Scripts.Camera
{
	public class CameraController : MonoBehaviour
	{
		public PlayerControls FocusObject;

		public float CameraDistanceLag;

		// Use this for initialization
		void Start()
		{

		}

		// Update is called once per frame
		void Update()
		{
			if(FocusObject.CurrentSpeed > 0)
			{
				if(FocusObject.transform.position.x > transform.position.x + CameraDistanceLag || FocusObject.transform.position.x < transform.position.x - CameraDistanceLag 
					|| FocusObject.transform.position.z > transform.position.z + CameraDistanceLag || FocusObject.transform.position.z < transform.position.z - CameraDistanceLag)
				{
					var xDestination = FocusObject.transform.position.x;
					var zDestination = FocusObject.transform.position.z;

					var newDestination = new Vector3(xDestination, transform.position.y, zDestination);
					transform.position = Vector3.Lerp(transform.position, newDestination, Time.deltaTime);
				}
			}
		}
	}
}
