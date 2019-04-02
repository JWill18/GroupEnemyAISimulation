using UnityEngine;

namespace GroupEnemyAISimulation.Assets.Scripts.Player
{
	public class PlayerControls : MonoBehaviour
	{
		#region Movement Properties
		/// <summary>
		/// The speed that the player moves.
		/// </summary>
		public float MovementSpeed;

		/// <summary>
		/// Returns the current speed of the player.
		/// </summary>
		public float CurrentMoveSpeed
		{ get
			{
				// Compares the speed in both directions and takes the higher speed
				var speed = Mathf.Abs(_verticalSpeed) > Mathf.Abs(_horizontalSpeed) ? _verticalSpeed : _horizontalSpeed;
				return Mathf.Abs(speed) * MovementSpeed;
			}
		}

		/// <summary>
		/// Returns the speed of the vertical axis
		/// </summary>
		private float _verticalSpeed { get { return Input.GetAxis("Vertical"); } }

		/// <summary>
		/// Returns the speed of the horizontal axis
		/// </summary>
		private float _horizontalSpeed { get { return Input.GetAxis("Horizontal"); } }

		private bool _canMove;
		#endregion

		#region Health Properties
		/// <summary>
		/// The current health of the player.
		/// </summary>
		public float Health;

		/// <summary>
		/// The maximum amount the health can be.
		/// </summary>
		public float MaxHealth;
		#endregion

		#region Animations
		/// <summary>
		/// The animator that is in charge of animation transitions
		/// </summary>
		private Animator PlayerAnimator { get { return GetComponentInChildren<Animator>(); } }

		/// <summary>
		/// Reference to the Child Object that contains the player mesh
		/// </summary>
		public GameObject PlayerCharacter;
		#endregion

		#region Combat
		public float BaseDamage = 15.0f;

		internal bool IsAttacking;

		internal bool HasDealthDamage;

		private bool _canAttack;
		#endregion

		#region MonoBehaviour
		// Use this for initialization
		void Start()
		{
			_canMove = true;
			_canAttack = true;
		}

		// Update is called once per frame
		void Update()
		{
			if (_canMove)
			{
				if (_verticalSpeed != 0)
				{
					PlayerAnimator.SetFloat("MoveSpeed", MovementSpeed);
					transform.Translate(Vector3.forward * Input.GetAxis("Vertical") * MovementSpeed * Time.deltaTime, Space.World);
				}

				if (_horizontalSpeed != 0)
				{
					PlayerAnimator.SetFloat("MoveSpeed", MovementSpeed);
					transform.Translate(Vector3.right * Input.GetAxis("Horizontal") * MovementSpeed * Time.deltaTime, Space.World);
				}

				if (_verticalSpeed == 0 && _horizontalSpeed == 0)
				{
					PlayerAnimator.SetFloat("MoveSpeed", 0);
				}
				else
				{
					var lookDirection = new Vector3(_horizontalSpeed, 0.0f, _verticalSpeed);
					PlayerCharacter.transform.rotation = Quaternion.LookRotation(lookDirection);
				}

				if(_canAttack && Input.GetAxis("Fire1") != 0)
				{
					IsAttacking = true;
					_canMove = false;
					_canAttack = false;
				}
			}

			PlayerAnimator.SetBool("IsAttacking", IsAttacking);
		}

		// Matches movement to animation
		private void OnAnimatorMove()
		{
			transform.position = PlayerAnimator.rootPosition;
		}
		#endregion

		#region Health
		/// <summary>
		/// Tells the player to take damage
		/// </summary>
		/// <param name="amount">The amount of damage to take</param>
		public void TakeDamage(float amount)
		{
			Health -= amount;

			_canAttack = false;
			_canMove = false;
			IsAttacking = false;

			PlayerAnimator.SetBool("IsTakingDamage", true);
		}

		/// <summary>
		/// Kills off the player
		/// </summary>
		public void Death()
		{
			PlayerAnimator.SetBool("IsDying", true);
		}
		#endregion

		#region Combat
		public void FinishAttacking()
		{
			PlayerAnimator.SetBool("IsAttacking", false);

			IsAttacking = false;
			_canAttack = true;
			_canMove = true;
			HasDealthDamage = false;
		}

		public void FinishTakingDamage()
		{
			PlayerAnimator.SetBool("IsTakingDamage", false);

			if(Health <= 0)
			{
				Death();
			}
			else
			{
				_canAttack = true;
				_canMove = true;
			}
		}
		#endregion
	}
}