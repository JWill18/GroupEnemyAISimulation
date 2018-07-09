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

		public GameObject TargetPlayer;

		public float SightRadius;
		public float SightAngles;
		public bool PlayerInSight { get { return DetectPlayerInSightRange(); } }
		public bool PlayerInContact;

		public AIGroup UnitGroup { get { return GetComponentInParent<AIGroup>() ?? null; } }

		public float MinDistanceFromPlayer;
		public float MaxDistanceFromPlayer;

		public int Index;

		// Use this for initialization
		void Start()
		{

		}

		// Update is called once per frame
		void Update()
		{
			if(PlayerInSight || PlayerInContact)
			{
				// Always look at Player
				LookAtTargetPlayer();

				// Keep Distance
				MaintainDistance();
			}
			else if (UnitGroup.FoundPlayer)
			{
				TargetPlayer = UnitGroup.TargetPlayer;

				LookAtTargetPlayer();

				MaintainDistance();
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

		/// <summary>
		/// Keeps the AI unit looking at the player as long as it is in range.
		/// </summary>
		public void LookAtTargetPlayer()
		{
			var relativePos = TargetPlayer.transform.position - transform.position;
			transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(relativePos), Time.deltaTime * 50);
		}

		/// <summary>
		/// Moves the unit away from the target and maintain a safe distance away.
		/// </summary>
		private void MaintainDistance()
		{
			var distance = Vector3.Distance(transform.position, TargetPlayer.transform.position);
			var move = MovementSpeed * Time.deltaTime;

			if (distance < MinDistanceFromPlayer)
				transform.position = Vector3.MoveTowards(transform.position, TargetPlayer.transform.position, -move);
			else if (distance > MaxDistanceFromPlayer)
				transform.position = Vector3.MoveTowards(transform.position, TargetPlayer.transform.position, move);
		}

		/// <summary>
		/// Checks to see if the player has collided with the object.
		/// </summary>
		/// <param name="collision">The object that collides with the unit</param>
		private void OnCollisionEnter(Collision collision)
		{
			if(collision.gameObject.CompareTag("Player"))
			{
				PlayerInContact = true;
			}
		}

		/// <summary>
		/// Checks to if the player has stopped colliding with the object.
		/// </summary>
		/// <param name="collision"></param>
		private void OnCollisionExit(Collision collision)
		{
			if(collision.gameObject.CompareTag("Player"))
			{
				PlayerInContact = false;
			}
		}
	}
}
