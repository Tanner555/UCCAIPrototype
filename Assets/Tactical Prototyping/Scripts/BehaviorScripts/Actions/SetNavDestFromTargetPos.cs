using UnityEngine;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using RTSCoreFramework;
using UnityEngine.AI;

namespace RTSPrototype
{
	[TaskCategory("RPGPrototype/AllyMember")]
    [TaskDescription("Resets The Provided Character Navigation Movement BlackBoard Variables and Nav. FleeFromTargetInstead Checkbox will attempt to set the destination to an area away from the target.")]
	public class SetNavDestFromTargetPos : Action
	{
		#region Shared
		public SharedBool FleeFromTargetInstead;
		public SharedVector3 MyNavDestination;
		public SharedBool bHasSetDestination;
		public SharedTransform CurrentTargettedEnemy;
		#endregion

		#region Fields
		List<float> myLookAheadDistances = new List<float>
		{
			1, 2, 3, 4, 5, 6, 7
		};
		int targetRetries = 5;
		float wanderRate = 2;
		Vector3 cachedEnemyPosition;
		#endregion

		#region Properties
		AllyMemberWrapper allyMember
		{
			get
			{
				if(_allyMember == null)
				{
					_allyMember = GetComponent<AllyMemberWrapper>();
				}
				return _allyMember;
			}
		}
		AllyMemberWrapper _allyMember = null;

		AllyAIControllerWrapper aiController
		{
			get
			{
				if (_aiController == null)
				{
					_aiController = GetComponent<AllyAIControllerWrapper>();
				}
				return _aiController;
			}
		}
		AllyAIControllerWrapper _aiController = null;

		NavMeshAgent myNavAgent
		{
			get
			{
				if(_myNavAgent == null)
				{
					_myNavAgent = GetComponent<NavMeshAgent>();
				}
				return _myNavAgent;
			}
		}
		NavMeshAgent _myNavAgent = null;
		#endregion

		#region Overrides
		public override TaskStatus OnUpdate()
		{
			if (FleeFromTargetInstead.Value == false)
			{
				Vector3 _currTargetPos = CurrentTargettedEnemy.Value.position;
				if (_currTargetPos != cachedEnemyPosition)
				{
					cachedEnemyPosition = _currTargetPos;
					Vector3 _enemyDestHitPos;
					if(TryGetNearTargetDestination(out _enemyDestHitPos))
					{
						//Set Nav Dest To Target Pos
						SetDestination(_enemyDestHitPos);
						return TaskStatus.Success;
					}
					else
					{
						Debug.Log("Couldn't Set Near Enemy Destination.");
						ResetDestination();
						return TaskStatus.Failure;
					}
				}
				else
				{
					if (Vector3.Distance(transform.position, _currTargetPos) > allyMember.MaxMeleeAttackDistance)
					{
						//If Enemy Is Standing Still and Isn't Within Melee Attack Distance, Go Towards Him.
						SetDestination(_currTargetPos);
					}
				}
				return TaskStatus.Success;
			}
			else
			{
				//Flee From Target
				Vector3 _fleeDestination;
				if (TryGetFleeDestination(myLookAheadDistances, out _fleeDestination))
				{
					SetDestination(_fleeDestination);
					return TaskStatus.Success;
				}
				else
				{
					ResetDestination();
					return TaskStatus.Failure;
				}
			}
		}

		public override void OnReset()
		{
			cachedEnemyPosition = Vector3.zero;
		}
		#endregion

		#region Helpers
		private void SetDestination(Vector3 _destination)
		{
			MyNavDestination.Value = _destination;
			bHasSetDestination.Value = true;
		}

		void ResetDestination()
		{
			bHasSetDestination.Value = false;
			MyNavDestination.Value = transform.position;
		}

		private bool TryGetNearTargetDestination(out Vector3 hitPosition)
		{
			hitPosition = Vector3.zero;
			Transform _enemyTransform = CurrentTargettedEnemy.Value;
			Vector3 _direction = _enemyTransform.forward;
			bool _validDestination = false;
			int _attempts = targetRetries;
			Vector3 destination = _enemyTransform.position;
			float _wanderDistance = allyMember.MaxMeleeAttackDistance;
			while (!_validDestination && _attempts > 0)
			{
				_direction = _direction + Random.insideUnitSphere * wanderRate;
				destination = _enemyTransform.position + _direction.normalized * _wanderDistance;
				_validDestination = SamplePosition(destination, out hitPosition);
				_attempts--;
			}
			return _validDestination;
		}

		private bool TryGetFleeDestination(List<float> lookAheadDistances, out Vector3 myDestination)
		{
			foreach (float _lookAheadDistance in lookAheadDistances)
			{
				myDestination = transform.position + (transform.position - CurrentTargettedEnemy.Value.transform.position).normalized * _lookAheadDistance;
				if (SamplePosition(myDestination, out myDestination))
				{
					return true;
				}
			}
			myDestination = Vector3.zero;
			return false;
		}

		/// <summary>
		/// Returns true if the position is a valid pathfinding position.
		/// </summary>
		/// <param name="position">The position to sample.</param>
		/// <returns>True if the position is a valid pathfinding position.</returns>
		protected bool SamplePosition(Vector3 position, out Vector3 hitPosition)
		{
			NavMeshHit hit;			
			if(NavMesh.SamplePosition(position, out hit, aiController.myNavAgentHeight * 2, NavMesh.AllAreas) &&
				aiController.isSurfaceWalkable(hit.position))
			{
				hitPosition = hit.position;
				return true;
			}
			hitPosition = Vector3.zero;
			return false;
		}
		#endregion
	}
}