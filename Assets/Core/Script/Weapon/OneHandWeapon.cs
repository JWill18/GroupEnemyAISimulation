using GroupEnemyAISimulation.Assets.Scripts.AI;
using GroupEnemyAISimulation.Assets.Scripts.Player;
using UnityEngine;

public class OneHandWeapon : MonoBehaviour
{
	public float Damage = 0.0f;

	private PlayerControls _parentPlayer;

	private AIUnit _parentUnit;

	// Start is called before the first frame update
	void Start()
	{
		_parentPlayer = GetComponentInParent<PlayerControls>();
		_parentUnit = GetComponentInParent<AIUnit>();
	}

	// Update is called once per frame
	void Update()
	{
		
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.GetComponent<PlayerControls>() != null)
		{
			var player = other.gameObject.GetComponent<PlayerControls>();

			if (player != _parentPlayer && _parentUnit != null && _parentUnit.IsAttacking && !_parentUnit.HasDealtDamage)
			{
				_parentUnit.HasDealtDamage = true;
				player.TakeDamage(Damage + _parentUnit.BaseDamage);
			}
		}
		else if (other.gameObject.GetComponent<AIUnit>() != null)
		{
			var unit = other.gameObject.GetComponent<AIUnit>();

			if (unit != _parentUnit && _parentPlayer != null && _parentPlayer.IsAttacking && !_parentPlayer.HasDealthDamage)
			{
				_parentPlayer.HasDealthDamage = true;
				unit.TakeDamage(Damage + _parentPlayer.BaseDamage);
			}
		}
	}
}
