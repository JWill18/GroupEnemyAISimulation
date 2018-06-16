using GroupEnemyAISimulation.Assets.Scripts.Player;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GroupEnemyAISimulation.Assets.Scripts.AI
{
	public class AIUnit : MonoBehaviour
	{

		public float Health;
		public float MaxHealth;
		public float MovementSpeed;
		public bool SeePlayer;
		public GameObject TargetPlayer;
		public float SightRadius;
		public float SightAngles;

		private bool PlayerInSight { get { return DetectPlayerInSightRange(); } }
		private bool PlayerInContact;

		// Use this for initialization
		void Start()
		{

		}

		// Update is called once per frame
		void Update()
		{
			if(PlayerInSight || PlayerInContact)
			{
				var relativePos = TargetPlayer.transform.position - transform.position;
				transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(relativePos), Time.deltaTime * 50);
			}
		}

		/// <summary>
		/// Detects if the player is in Sight range.
		/// </summary>
		/// <returns>If the Unit sees the player</returns>
		public bool DetectPlayerInSightRange()
		{
			var foundPlayer = false;

			var colliders = Physics.OverlapSphere(transform.position, SightRadius).ToList();

			foreach (var objects in colliders)
			{
				if (objects.gameObject.CompareTag("Player"))
				{
					var targetDir = objects.gameObject.transform.position - transform.position;
					var angleOfSight = Vector3.Angle(targetDir, transform.forward);
					if(angleOfSight < SightAngles)
					{
						foundPlayer = true;
						TargetPlayer = objects.gameObject;
					}
				}
			}

			return foundPlayer;
		}

		private void OnCollisionEnter(Collision collision)
		{
			if(collision.gameObject.CompareTag("Player"))
			{
				PlayerInContact = true;
			}
		}

		private void OnCollisionExit(Collision collision)
		{
			if(collision.gameObject.CompareTag("Player"))
			{
				PlayerInContact = false;
			}
		}
	}
}
