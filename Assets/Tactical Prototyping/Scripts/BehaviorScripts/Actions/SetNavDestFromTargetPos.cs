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
		#endregion

		#region Properties
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
				//Set Nav Dest To Target Pos
				MyNavDestination.Value = CurrentTargettedEnemy.Value.position;
				bHasSetDestination.Value = true;
				return TaskStatus.Success;
			}
			else
			{
				//Flee From Target
				Vector3 _fleeDestination;
				if (TryGetFleeDestination(myLookAheadDistances, out _fleeDestination))
				{
					MyNavDestination.Value = _fleeDestination;
					bHasSetDestination.Value = true;
					return TaskStatus.Success;
				}
				else
				{
					bHasSetDestination.Value = false;
					MyNavDestination.Value = transform.position;
					return TaskStatus.Failure;
				}
			}
		}
		#endregion

		#region Helpers
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