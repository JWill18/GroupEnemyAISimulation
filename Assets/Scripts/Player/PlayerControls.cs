using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GroupEnemyAISimulation.Assets.Scripts.Player
{
	public class PlayerControls : MonoBehaviour
	{

		/// <summary>
		/// The speed that the camera moves.
		/// </summary>
		public float MovementSpeed;

		/// <summary>
		/// The current health of the player.
		/// </summary>
		public float Health;

		/// <summary>
		/// The maximum amount the health can be.
		/// </summary>
		public float MaxHealth;

		// Use this for initialization
		void Start()
		{

		}

		// Update is called once per frame
		void Update()
		{
			if (Input.GetAxis("Vertical") != 0)
			{
				transform.Translate(Vector3.forward * Input.GetAxis("Vertical") * MovementSpeed * Time.deltaTime);
			}

			if (Input.GetAxis("Horizontal") != 0)
			{
				transform.Translate(Vector3.right * Input.GetAxis("Horizontal") * MovementSpeed * Time.deltaTime);
			}
		}
	}
}