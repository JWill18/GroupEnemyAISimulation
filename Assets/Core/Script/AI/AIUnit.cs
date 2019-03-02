using GroupEnemyAISimulation.Assets.Scripts.Enum;
using GroupEnemyAISimulation.Assets.Scripts.Player;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GroupEnemyAISimulation.Assets.Scripts.AI
{
	public class AIUnit : MonoBehaviour
	{
		#region Properties

		/// <summary>
		/// The Current health of the unit
		/// </summary>
		public float Health;

		/// <summary>
		/// The maximum amount of health the unit can have.
		/// </summary>
		public float MaxHealth;

		/// <summary>
		/// Speed that the unit can move
		/// </summary>
		public float MovementSpeed;

		/// <summary>
		/// The base amount of damage the unit deals to the player
		/// </summary>
		public float BaseDamage;

		/// <summary>
		/// Determines if the unit is within attack range
		/// </summary>
		internal bool InAttackRange { get { return Vector3.Distance(transform.position, TargetPlayer.transform.position) < AttackRange; } }

		/// <summary>
		/// The Unit must be within attacking range to be able to hit the player
		/// </summary>
		public float AttackRange;

		/// <summary>
		/// Determines if the player is currently attacking or not
		/// </summary>
		public bool IsAttacking;

		/// <summary>
		/// The state of the unit in battle
		/// </summary>
		internal AIUnitBattleState BattleState;

		/// <summary>
		/// The player that the unit is targeting
		/// </summary>
		public GameObject TargetPlayer;

		/// <summary>
		/// The radius distance to be checking if the unit sees the player
		/// </summary>
		public float SightRadius;

		/// <summary>
		/// The sight angle needed to spot the player
		/// </summary>
		public float SightAngles;

		/// <summary>
		/// Determines if the unit sees the player
		/// </summary>
		internal bool PlayerInSight { get { return DetectPlayerInSightRange(); } }

		/// <summary>
		/// Determines if the unit is colliding with the player
		/// </summary>
		internal bool PlayerInContact;

		/// <summary>
		/// The Unit Group that the unit belongs to
		/// </summary>
		public AIGroup UnitGroup { get { return GetComponentInParent<AIGroup>() ?? null; } }

		/// <summary>
		/// The minimum amount of distance the unit will try to keep from the player
		/// </summary>
		public float MinDistanceFromPlayer;

		/// <summary>
		/// The maximum amount of distance the unit will try to keep from the player
		/// </summary>
		public float MaxDistanceFromPlayer;

		/// <summary>
		/// The Unit Group index of priorities
		/// </summary>
		internal int Index;

		/// <summary>
		/// The animator that is in charge of animation transitions
		/// </summary>
		internal Animator UnitAnimator { get { return GetComponentInChildren<Animator>(); } }
		#endregion

		#region MonoBehaviour
		// Use this for initialization
		void Start()
		{
		}

		// Update is called once per frame
		void Update()
		{
			if(PlayerInSight || PlayerInContact)
			{
				if (BattleState != AIUnitBattleState.Attacking)
				{
					// Always look at Player
					LookAtTargetPlayer();

					MaintainDistance();
				}
				else
				{
					if (InAttackRange && !IsAttacking)
					{
						// Attack Player
						StartCoroutine(AttackTargetPlayer());
					}
					else if (!InAttackRange)
					{
						//Close in on Player
						MoveTowardsTargetPlayer();
					}
				}
			}
			else if (UnitGroup.FoundPlayer)
			{
				TargetPlayer = UnitGroup.TargetPlayer;

				LookAtTargetPlayer();

				MaintainDistance();
			}
		}

		/// <summary>
		/// Checks to see if the player has collided with the object.
		/// </summary>
		/// <param name="collision">The object that collides with the unit</param>
		private void OnCollisionEnter(Collision collision)
		{
			if (collision.gameObject.CompareTag("Player"))
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
			if (collision.gameObject.CompareTag("Player"))
			{
				PlayerInContact = false;
			}
		}
		#endregion

		#region Detection
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
		#endregion

		#region Movement
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
			var playerControls = TargetPlayer.GetComponent<PlayerControls>();

			var distance = Vector3.Distance(transform.position, TargetPlayer.transform.position);

			var move = MovementSpeed;

			// Slows down movement if player is moving to await player actions.
			if (playerControls != null && playerControls.CurrentMoveSpeed > playerControls.MovementSpeed / 2)
				move = MovementSpeed / 4;

			if (distance < MinDistanceFromPlayer)
			{
				transform.position = Vector3.MoveTowards(transform.position, TargetPlayer.transform.position, -move * Time.deltaTime);
			}
			else if (distance > MaxDistanceFromPlayer)
			{
				transform.position = Vector3.MoveTowards(transform.position, TargetPlayer.transform.position, move * Time.deltaTime);
			}
			else
			{
				move = 0;
			}

			if(UnitAnimator != null && BattleState != AIUnitBattleState.Attacking)
				UnitAnimator.SetFloat("MoveSpeed", move);
		}

		/// <summary>
		/// Moves the unit within striking range of the player
		/// </summary>
		private void MoveTowardsTargetPlayer()
		{
			var distance = Vector3.Distance(transform.position, TargetPlayer.transform.position);
			var move = MovementSpeed * Time.deltaTime;

			if(distance > AttackRange)
			{
				transform.position = Vector3.MoveTowards(transform.position, TargetPlayer.transform.position, move);
			}
			else if (distance < AttackRange / 2)
			{
				transform.position = Vector3.MoveTowards(transform.position, TargetPlayer.transform.position, -move);
			}

			if (UnitAnimator != null && BattleState == AIUnitBattleState.Attacking)
				UnitAnimator.SetFloat("MoveSpeed", MovementSpeed);

		}
		#endregion

		#region Combat
		private IEnumerator AttackTargetPlayer()
		{
			if (UnitAnimator != null)
				UnitAnimator.SetBool("IsAttacking", true);
			IsAttacking = true;

			var playerControl = TargetPlayer.GetComponent<PlayerControls>();
			playerControl.TakeDamage(BaseDamage);

			yield return new WaitForSeconds(1.30f);
			if (UnitAnimator != null)
				UnitAnimator.SetBool("IsAttacking", false);

			IsAttacking = false;
			UnitGroup.DoneAttacking(this);
		}
		#endregion

		#region Unit Status
		/// <summary>
		/// Determines if the unit is dead
		/// </summary>
		/// <returns>If the unit is dead</returns>
		public bool IsDead()
		{
			bool isDead;

			if(Health <= 0)
			{
				isDead = true;
			}
			else
			{
				isDead = false;
			}

			return isDead;
		}

		/// <summary>
		/// Sets the battle state of the unit
		/// </summary>
		/// <param name="unitState">The new state that the unit will be in.</param>
		public void SetBattleState(AIUnitBattleState unitState)
		{
			BattleState = unitState;
		}
		#endregion
	}
}
