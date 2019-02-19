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

		#region Camera
		public UnityEngine.Camera Camera;
		#endregion

		#region MonoBehaviour
		// Use this for initialization
		void Start()
		{

		}

		// Update is called once per frame
		void Update()
		{
			if (Input.GetAxis("Vertical") != 0)
			{
				transform.Translate(Vector3.forward * Input.GetAxis("Vertical") * MovementSpeed * Time.deltaTime, Space.World);
			}

			if (Input.GetAxis("Horizontal") != 0)
			{
				transform.Translate(Vector3.right * Input.GetAxis("Horizontal") * MovementSpeed * Time.deltaTime, Space.World);
			}

			LookAtCursor();
		}
		#endregion

		#region Mouse
		public void LookAtCursor()
		{
			var mouseX = Input.mousePosition.x;
			var mouseY = Input.mousePosition.y;
			var cameraDif = Camera.transform.position.y - transform.position.y;
			var worldpos = Camera.ScreenToWorldPoint(new Vector3(mouseX, mouseY, cameraDif));

			Vector3 cursorDirection = new Vector3(worldpos.x, transform.position.y, worldpos.z);

			transform.LookAt(cursorDirection);
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

			//if (Health <= 0)
			//    Death();
		}

		/// <summary>
		/// Kills off the player
		/// </summary>
		public void Death()
		{
			Destroy(gameObject);
		}
		#endregion
	}
}