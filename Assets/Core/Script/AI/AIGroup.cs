using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using GroupEnemyAISimulation.Assets.Scripts.Enum;
using GroupEnemyAISimulation.Assets.Scripts.Player;
using GroupEnemyAISimulation.Assets.Scripts.Math;

namespace GroupEnemyAISimulation.Assets.Scripts.AI
{
	/// <summary>
	/// Represents a group of units that are acting together to achieve a single objective
	/// </summary>
	public class AIGroup : MonoBehaviour
	{
		#region Properties
		/// <summary>
		/// List of all units that are still active in the group
		/// </summary>
		public List<AIUnit> AliveUnits { get { return GetComponentsInChildren<AIUnit>().Where(unit => !unit.IsDead()).ToList(); } }

		/// <summary>
		/// List of all units that have been defeated in the group
		/// </summary>
		public List<AIUnit> DeadUnits { get { return GetComponentsInChildren<AIUnit>().Where(unit => unit.IsDead()).ToList(); } }

		/// <summary>
		/// List of all units that are both alive and dead in the group
		/// </summary>
		public List<AIUnit> AllUnits { get { return GetComponentsInChildren<AIUnit>().ToList(); } }

		/// <summary>
		/// The current situation of the group. Used to determine overall options for action.
		/// </summary>
		public AIGroupState GroupState;

		/// <summary>
		/// The primary behavior that will be used to determine actions. Options available are based on GroupState.
		/// </summary>
		public AIGroupBehavior PrimaryBehaviour;

		/// <summary>
		/// The secondary behavior that will be used to further determine actions. Options available are based on GroupState and PrimaryBehavior.
		/// </summary>
		public AIGroupBehavior SecondaryBehavior;

		/// <summary>
		/// Shows the current amount of health of the group by adding up the health of all of the units in the group.
		/// </summary>
		public float GroupHealth { get { return CalcGroupHealth(); } }

		/// <summary>
		/// Shows the total amount of health of the group by adding up the max health of all of the units in the group.
		/// </summary>
		public float GroupTotalHealth { get { return CalcGroupTotalHealth(); } }

		/// <summary>
		/// Shows the weakest unit in the group.
		/// </summary>
		public AIUnit WeakestUnit { get { return CalcWeakestUnit(); } }

		/// <summary>
		/// Shows the strongest unit in the group.
		/// </summary>
		public AIUnit StrongestUnit { get { return CalcStrongestUnit(); } }

		/// <summary>
		/// Determines if anyone in the group has seen the player before.
		/// </summary>
		public bool HasSeenPlayer;

		/// <summary>
		/// Determines if the player is currently in sight of the group.
		/// </summary>
		public bool FoundPlayer { get { return DetectPlayer(); } }

		/// <summary>
		/// Holds the target player that the group is focused on.
		/// </summary>
		public GameObject TargetPlayer { get { return GetTargetPlayer(); } }

		/// <summary>
		/// Determines if the unit is ready to attack
		/// </summary>
		private bool _readyToAttack { get { return _lastAttackTimer >= MinAttackIntervalTimer; } }

		/// <summary>
		/// Determines if a unit is currently attacking.
		/// </summary>
		private bool _isAttacking { get { return AliveUnits.Where(unit => unit.BattleState == AIUnitBattleState.Attacking).Count() > 0; } }

		/// <summary>
		/// Represent the soonest that the group can attack the player
		/// </summary>
		public float MinAttackIntervalTimer;

		/// <summary>
		/// Represents the maximum amount of time that the group should wait to attack the player
		/// </summary>
		public float MaxAttackIntervalTimer;

		/// <summary>
		/// Represent the time since the last attack
		/// </summary>
		private float _lastAttackTimer = 0.0f;
		#endregion

		#region MonoBehaviour
		// Use this for initialization
		void Start()
		{
			CalcUnitIndex();
		}

		// Update is called once per frame
		void Update()
		{
			// Determine the current state and behavior of the group.
			DetermineGroupState();
			DeterminePrimaryBehavior();
			DetermineSecondaryBehavior();

			// React based on the determined behaviors
			ReactWithCurrentStateAndBehaviors();
		}
		#endregion

		#region Determine Behavior
		/// <summary>
		/// Determines the current state of the group.
		/// </summary>
		private void DetermineGroupState()
		{
			/*
			 * Used to Determine Current State:
			 * 
			 * Offensive : If the player is in sight, has taken damage or has less health than (GroupHealth x 0.75) then actively charge the player.
			 * Defensive : If the player is in sight, has not taken damage and has more health than (GroupHealth x 0.75) then react to player actions.
			 * Protective : If the player is in sight, then swap between Offensive and Defensive to protect the objective.
			 * Patrol : If the player has been sighted but is no longer within range, then seek player out.
			 * 
			 */

			if(HasSeenPlayer)
			{
				if (FoundPlayer)
				{
					if (TargetPlayer.GetComponent<PlayerControls>() != null)
					{
						var targetPlayerControls = TargetPlayer.GetComponent<PlayerControls>();

						if (targetPlayerControls.Health < targetPlayerControls.MaxHealth || targetPlayerControls.Health < (GroupHealth * 0.75))
						{
							GroupState = AIGroupState.Offensive;
						}
						else
						{
							GroupState = AIGroupState.Defensive;
						}
					}
				}
				else
				{
					GroupState = AIGroupState.Patrol;
				}
			}
			else
			{
				GroupState = AIGroupState.Normal;
			}

		}

		/// <summary>
		/// Determines the current primary behavior of the group based on the GroupState
		/// </summary>
		private void DeterminePrimaryBehavior()
		{
			/*
			 * State Meanings:
			 * 
			 * Normal : The group has not noticed the player and is acting normally.
			 * Offensive : The group has noticed the player and is actively trying to finish the battle.
			 * Defensive : The group has noticed the player and reacts to the player's actions.
			 * Protective : The group has noticed the player and is trying to prevent the player from achieving a goal (e.g. Protect the treasure!).
			 * Patrol : The group has or has not noticed the player and is searching the perimeter for anything suspicious.
			 * 
			 */
			if(GroupState == AIGroupState.Normal)
			{
				PrimaryBehaviour = AIGroupBehavior.Passive;
			}
			if (GroupState == AIGroupState.Offensive)
			{
				PrimaryBehaviour = AIGroupBehavior.Aggresive;
			}
			else if (GroupState == AIGroupState.Defensive)
			{
				PrimaryBehaviour = AIGroupBehavior.Passive;
			}
			else if (GroupState == AIGroupState.Protective)
			{
				PrimaryBehaviour = AIGroupBehavior.Aggresive;
			}
			else if (GroupState == AIGroupState.Patrol)
			{
				PrimaryBehaviour = AIGroupBehavior.Passive;
			}
		}

		/// <summary>
		/// Determines the current secondary behavior of the group based on the GroupState and the PrimaryBehavior
		/// </summary>
		private void DetermineSecondaryBehavior()
		{

		}

		/// <summary>
		/// Tells the group how to react based on the current state and behaviors of the group
		/// </summary>
		private void ReactWithCurrentStateAndBehaviors()
		{
			/*
			 * State Meanings:
			 * 
			 * Normal : The group has not noticed the player and is acting normally.
			 * Offensive : The group has noticed the player and is actively trying to finish the battle.
			 * Defensive : The group has noticed the player and reacts to the player's actions.
			 * Protective : The group has noticed the player and is trying to prevent the player from achieving a goal (e.g. Protect the treasure!).
			 * Patrol : The group has or has not noticed the player and is searching the perimeter for anything suspicious.
			 * 
			 */
			if (GroupState != AIGroupState.Normal)
			{
				var playerControls = TargetPlayer.GetComponent<PlayerControls>();

				if (playerControls != null && playerControls.CurrentMoveSpeed < playerControls.MovementSpeed / 2)
				{
					RotationMovement();

					if (!_isAttacking && _readyToAttack)
					{
						// Random chance to attack during this time
						bool attackChance = Random.value > 0.5f;
						if (attackChance)
						{
							var randomNum = Random.Range(0, AliveUnits.Count() - 1);
							AliveUnits[randomNum].SetBattleState(AIUnitBattleState.Attacking);
						}
					}

					_lastAttackTimer += Time.deltaTime;
				}
			}
		}

		/// <summary>
		/// Is called by units to notify that the group is done attacking.
		/// </summary>
		/// <param name="done">Tells whether a unit is done attacking</param>
		internal void DoneAttacking(AIUnit unit)
		{
			_lastAttackTimer = 0.0f;
			unit.SetBattleState(AIUnitBattleState.Normal);
		}
		#endregion

		#region Group Movements
		private void RotationMovement()
		{
			// Finds out how many degrees each unit will cover. Based on 180.0 to -180.0 degrees.
			var angleDivision = 360.0f / (AliveUnits.Count);

			// Used to determine the margin of error angles.
			var angleMOE = 10.0f;

			// Sorts the Units based on the index
			var unitsByIndex = AliveUnits.OrderByDescending(u => u.Index).ToList();

			// Used to distribute the units between -180 and 180
			var positiveObjects = 0;
			var negativeOjects = 1;

			// For each unit, distribute their position between the positive and negative angles
			for (var i = 0; i < unitsByIndex.Count; i++)
			{
				// Determines if the unit will be placed in the positive or negative angles
				var positiveRotation = i % 2 == 0 ? true : false;

				// Grab the unit.
				var unit = unitsByIndex[i];

				if (unit.BattleState == AIUnitBattleState.Attacking)
				{
					if (unit.UnitAnimator != null)
					{
						unit.UnitAnimator.SetInteger("StrafingDirection", 0);
					}

					continue;
				}

				// Find the position and rotation relative to the Target player
				var unitToTargetLocalPosition = TargetPlayer.transform.InverseTransformPoint(unit.transform.position);
				var unitToTargetLocalRotation = Mathf.Atan2(unitToTargetLocalPosition.x, unitToTargetLocalPosition.z) * Mathf.Rad2Deg;

				// If the unit is being distributed in the positive angle
				if (positiveRotation)
				{
					/*
					 * Create some base variables to hold angles
					 */

					// Create base angle for the unit using the positive index
					var positiveBaseAngle = (positiveObjects * angleDivision);

					// Exempt the strongest unit from the angles
					if (i == 0)
						positiveBaseAngle = 0;

					// Create the min and max angles that the unit can be at
					var negativeAngle = positiveBaseAngle - (angleDivision/2);
					var positiveAngle = positiveBaseAngle + (angleDivision/2);

					// Create the min and max angles for the base angle that the unit can be at
					var positiveBaseMOE = positiveBaseAngle + angleMOE;
					var negativeBaseMOE = positiveBaseAngle - angleMOE;

					if (unit.BattleState != AIUnitBattleState.Attacking)
					{
						// Based on what angle the unit is at then rotate the unit.
						if (MathAngle.NeedToAdjustClockwise(unitToTargetLocalRotation, positiveBaseAngle, negativeAngle))
						{
							// Rotate clockwise at normal speed
							unit.transform.RotateAround(TargetPlayer.transform.position, Vector3.up, unit.MovementSpeed * Time.deltaTime * 10);

							if (unit.UnitAnimator != null)
							{
								unit.UnitAnimator.SetInteger("StrafingDirection", 1);
								unit.UnitAnimator.SetFloat("MoveSpeed", unit.MovementSpeed);
							}
						}
						else if (MathAngle.NeedToAdjustCounterClockwise(unitToTargetLocalRotation, positiveBaseAngle, positiveAngle))
						{
							// Rotate counter clockwise at normal speed
							unit.transform.RotateAround(TargetPlayer.transform.position, Vector3.up, unit.MovementSpeed * Time.deltaTime * -10);

							if (unit.UnitAnimator != null)
							{
								unit.UnitAnimator.SetInteger("StrafingDirection", 2);
								unit.UnitAnimator.SetFloat("MoveSpeed", unit.MovementSpeed);
							}
						}
						else if (MathAngle.NeedToAdjustClockwise(unitToTargetLocalRotation, negativeBaseMOE, negativeBaseMOE))
						{
							// Rotate clockwise at half speed
							unit.transform.RotateAround(TargetPlayer.transform.position, Vector3.up, (unit.MovementSpeed / 2) * Time.deltaTime * 5);

							if (unit.UnitAnimator != null)
							{
								unit.UnitAnimator.SetInteger("StrafingDirection", 1);
								unit.UnitAnimator.SetFloat("MoveSpeed", unit.MovementSpeed/4);
							}
						}
						else if (MathAngle.NeedToAdjustCounterClockwise(unitToTargetLocalRotation, positiveBaseMOE, positiveBaseMOE))
						{
							// Rotate counter clockwise at half speed
							unit.transform.RotateAround(TargetPlayer.transform.position, Vector3.up, (unit.MovementSpeed / 2) * Time.deltaTime * -5);

							if (unit.UnitAnimator != null)
							{
								unit.UnitAnimator.SetInteger("StrafingDirection", 2);
								unit.UnitAnimator.SetFloat("MoveSpeed", unit.MovementSpeed/4);
							}
						}
						else
						{
							if (unit.UnitAnimator != null)
							{
								unit.UnitAnimator.SetInteger("StrafingDirection", 0);
							}
						}
					}
					// Increase positive index
					positiveObjects++;
				}
				else
				{
					/*
					 * Create some base variables to hold angles
					 */

					// Create base angle for the unit using the negative index
					var negativeBaseAngle = (-negativeOjects * angleDivision);

					// Create the min and max angles that the unit can be at
					var negativeAngle = negativeBaseAngle - (angleDivision / 2);
					var positiveAngle = negativeBaseAngle + (angleDivision / 2);

					// Create the min and max angles for the base angle that the unit can be at
					var positiveBaseMOE = negativeBaseAngle + angleMOE;
					var negativeBaseMOE = negativeBaseAngle - angleMOE;

					if (unit.BattleState != AIUnitBattleState.Attacking)
					{
						// Based on what angle the unit is at then rotate the unit.
						if (MathAngle.NeedToAdjustClockwise(unitToTargetLocalRotation, negativeBaseAngle, negativeAngle))
						{
							// Rotate clockwise at normal speed
							unit.transform.RotateAround(TargetPlayer.transform.position, Vector3.up, unit.MovementSpeed * Time.deltaTime * 10);

							if (unit.UnitAnimator != null)
							{
								unit.UnitAnimator.SetInteger("StrafingDirection", 1);
								unit.UnitAnimator.SetFloat("MoveSpeed", unit.MovementSpeed);
							}
						}
						else if (MathAngle.NeedToAdjustCounterClockwise(unitToTargetLocalRotation, negativeBaseAngle, positiveAngle))
						{
							// Rotate counter clockwise at normal speed
							unit.transform.RotateAround(TargetPlayer.transform.position, Vector3.up, unit.MovementSpeed * Time.deltaTime * -10);

							if (unit.UnitAnimator != null)
							{
								unit.UnitAnimator.SetInteger("StrafingDirection", 2);
								unit.UnitAnimator.SetFloat("MoveSpeed", unit.MovementSpeed);
							}
						}
						else if (MathAngle.NeedToAdjustClockwise(unitToTargetLocalRotation, negativeBaseMOE, negativeBaseMOE))
						{
							// Rotate clockwise at half speed
							unit.transform.RotateAround(TargetPlayer.transform.position, Vector3.up, (unit.MovementSpeed / 2) * Time.deltaTime * 5);

							if (unit.UnitAnimator != null)
							{
								unit.UnitAnimator.SetInteger("StrafingDirection", 1);
								unit.UnitAnimator.SetFloat("MoveSpeed", unit.MovementSpeed / 4);
							}
						}
						else if (MathAngle.NeedToAdjustCounterClockwise(unitToTargetLocalRotation, positiveBaseMOE, positiveBaseMOE))
						{
							// Rotate counter clockwise at half speed
							unit.transform.RotateAround(TargetPlayer.transform.position, Vector3.up, (unit.MovementSpeed / 2) * Time.deltaTime * -5);

							if (unit.UnitAnimator != null)
							{
								unit.UnitAnimator.SetInteger("StrafingDirection", 2);
								unit.UnitAnimator.SetFloat("MoveSpeed", unit.MovementSpeed / 4);
							}
						}
					}
					// Increase negative index
					negativeOjects++;
				}
			}
		}
		#endregion

		#region Sensory
		/// <summary>
		/// Determines if the player has been found by a member of the group.
		/// </summary>
		/// <returns>If the player has been seen or not.</returns>
		private bool DetectPlayer()
		{
			var foundPlayer = false;

			foreach (var unit in AliveUnits)
			{
				if(unit.PlayerInSight)
				{
					foundPlayer = true;
					HasSeenPlayer = true;
				}
			}

			return foundPlayer;
		}

		/// <summary>
		/// Grabs the target player that the group is targeting.
		/// </summary>
		/// <returns>Returns null or the player object</returns>
		private GameObject GetTargetPlayer()
		{
			GameObject playerObject = null;

			if(FoundPlayer)
			{
				foreach(var unit in AliveUnits)
				{
					if(unit.TargetPlayer != null)
					{
						playerObject = unit.TargetPlayer;
					}
				}
			}

			return playerObject;
		}
		#endregion

		#region Units Calculations
		/// <summary>
		/// Calculates the current health of the group by adding all of the units' health values in the group
		/// </summary>
		/// <returns>All of the health values added together</returns>
		internal float CalcGroupHealth()
		{
			float groupHealth = 0.0f;

			// If there are any units in the group, add their current health values to the total
			if(AliveUnits.Count != 0)
			{
				foreach(var unit in AliveUnits)
				{
					groupHealth += unit.Health;
				}
			}

			return groupHealth;
		}

		/// <summary>
		/// Calculates the total health of the group by adding all of the units' max health values in the group
		/// </summary>
		/// <returns>All of the max health values added together</returns>
		internal float CalcGroupTotalHealth()
		{
			float totalGroupHealth = 0.0f;

			// If there are any units in the group, add their max health values to the total
			if (AliveUnits.Count != 0)
			{
				foreach(var unit in AliveUnits)
				{
					totalGroupHealth += unit.MaxHealth;
				}
			}

			return totalGroupHealth;
		}

		/// <summary>
		/// Calculate the weakest unit in the group
		/// </summary>
		/// <returns>The unit with the lowest health</returns>
		internal AIUnit CalcWeakestUnit()
		{
			AIUnit weakestUnit = null;

			// If there are any units in the group, determine which one has the lowest health value.
			if(AliveUnits.Count != 0)
			{
				weakestUnit = AliveUnits[0];

				foreach(var unit in AliveUnits)
				{
					// If the unit has less health than the weakest unit, then assign unit to being the weakestUnit variable.
					if(unit.Health < weakestUnit.Health)
					{
						weakestUnit = unit;
					}
				}
			}

			return weakestUnit;
		}

		/// <summary>
		/// Calculates the strongest unit in the group
		/// </summary>
		/// <returns>The unit with the highest health</returns>
		internal AIUnit CalcStrongestUnit()
		{
			AIUnit strongestUnit = null;

			// If there are any units in the group, determine which one has the highest health value.
			if (AliveUnits.Count != 0)
			{
				strongestUnit = AliveUnits[0];

				foreach (var unit in AliveUnits)
				{
					// If the unit has more health than the strongest unit, then assign unit to being the strongestUnit variable.
					if (unit.Health > strongestUnit.Health)
					{
						strongestUnit = unit;
					}
				}
			}

			return strongestUnit;
		}

		/// <summary>
		/// Calculates the index of each unit in the group which determines their priority in the list
		/// </summary>
		internal void CalcUnitIndex()
		{
			// for every unit in the group, determine unit priority
			for(var i = 0; i < AliveUnits.Count; i++)
			{
				// Grab the unit and create a default priority
				var unit = AliveUnits[i];
				var indexPriority = 0;

				// Create a temp array of the unit group and remove the current unit from the temp array
				var tempUnits = AliveUnits;
				tempUnits.RemoveAt(i);

				// For every unit in the temp array, determine the priority of the current unit
				foreach (var otherUnits in tempUnits)
				{
					// Lower health means lower priority
					if (unit.Health < otherUnits.Health)
					{
						indexPriority++;
					}
				}

				// Check for any units with the same priority and increase current unit priority
				var matchingUnitIndices = tempUnits.Where(tempUnit => tempUnit.Index == indexPriority);
				if (matchingUnitIndices.Count() > 0)
				{
					indexPriority += matchingUnitIndices.Count();
				}

				// Set the unit index
				unit.Index = indexPriority;
			}
		}
		#endregion
	}
}