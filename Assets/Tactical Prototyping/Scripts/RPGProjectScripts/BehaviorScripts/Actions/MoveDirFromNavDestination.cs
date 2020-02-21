using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using RTSCoreFramework;
using UnityEngine.AI;

namespace RPGPrototype {
	[TaskCategory("RPGPrototype/AllyMember")]
    [TaskDescription("Provides Move Direction From Navigation Destination.")]
	public class MoveDirFromNavDestination : Action
	{
		#region Shared
		public SharedVector3 MyMoveDirection;
		public SharedVector3 MyNavDestination;
		public SharedBool bFinishedMoving;
		#endregion

		#region Properties
		NavMeshAgent navMeshAgent
		{
			get
			{
				if(_navMeshAgent == null)
				{
					_navMeshAgent = GetComponent<NavMeshAgent>();
				}
				return _navMeshAgent;
			}
		}
		NavMeshAgent _navMeshAgent = null;

		AllyMember allyMember
		{
			get
			{
				if (_allymember == null)
				{
					_allymember = GetComponent<AllyMember>();
				}
				return _allymember;
			}
		}
		AllyMember _allymember = null;

		AIControllerRPG aiController
		{
			get
			{
				if (_aiController == null)
				{
					_aiController = (AIControllerRPG)allyMember.aiController;
				}
				return _aiController;
			}
		}
		AIControllerRPG _aiController = null;

		AllyEventHandler myEventHandler
		{
			get
			{
				if (_myEventhandler == null)
				{
					_myEventhandler = GetComponent<AllyEventHandler>();
				}
				return _myEventhandler;
			}
		}
		AllyEventHandler _myEventhandler = null;

		BehaviorTree AllyBehaviorTree
		{
			get
			{
				if (_AllyBehaviorTree == null)
				{
					_AllyBehaviorTree = GetComponent<BehaviorTree>();
				}
				return _AllyBehaviorTree;
			}
		}
		BehaviorTree _AllyBehaviorTree = null;
		#endregion

		#region Fields

		#endregion

		#region Overrides
		public override void OnStart()
		{
			//navMeshAgent.SetDestination(MyNavDestination.Value);
		}

		public override TaskStatus OnUpdate()
		{
			//By Default, Will Keep Moving Until in a Finished State
			bFinishedMoving.Value = false;

			if(navMeshAgent == null) Debug.LogError(gameObject.name + "navmesh is null");
			if(!navMeshAgent.isOnNavMesh) Debug.LogError(gameObject.name + " uh oh this guy is not on the navmesh");
			if (navMeshAgent == null || !navMeshAgent.isOnNavMesh)
			{
				//Stop Moving and Finish Task
				bFinishedMoving.Value = true;
				return TaskStatus.Failure;
			}

			if(navMeshAgent.destination != MyNavDestination.Value) navMeshAgent.SetDestination(MyNavDestination.Value);

			if (navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
			{
				navMeshAgent.updateRotation = true;
				MyMoveDirection.Value = navMeshAgent.desiredVelocity;
				//Haven't Finished Moving Yet, Returning Success Temporarily
				return TaskStatus.Success;
			}
			else if(Vector3.Distance(transform.position, navMeshAgent.destination) > navMeshAgent.stoppingDistance + 0.1f)
			{
				//Fix Stopping Distance Issue, Which Causes Character to Stop Before Reaching Destination
                //string _msg = "Temporarily Ignoring Stopping Distance Issue." +
                //    $"Remaining Distance: {navMeshAgent.remainingDistance}" +
                //    $"Stopping Distance: {navMeshAgent.stoppingDistance}" +
                //    $"Distance To Destination: {Vector3.Distance(transform.position, navMeshAgent.destination)}";
                //Debug.Log(_msg);
				navMeshAgent.updateRotation = true;
				MyMoveDirection.Value = Vector3.zero;
				//Haven't Finished Moving Yet, Returning Success Temporarily
				return TaskStatus.Success;
			}
			else
			{
				//Finished Moving, Stop Running This Task
				bFinishedMoving.Value = true;
				return TaskStatus.Success;
			}
		}
		#endregion

	} 
}