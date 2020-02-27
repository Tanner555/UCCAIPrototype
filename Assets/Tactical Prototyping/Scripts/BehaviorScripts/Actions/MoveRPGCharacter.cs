using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using RTSCoreFramework;
using Opsive.UltimateCharacterController.Game;
using Opsive.UltimateCharacterController.Character;

namespace RTSPrototype
{
	[TaskCategory("RPGPrototype/AllyMember")]
    [TaskDescription("Moves RPG Character From A Given Direction. This only moves the input vector, so make sure to rotate the character as well.")]
    public class MoveRPGCharacter : Action
	{
		#region Shared
		public SharedVector3 MyMoveDirection;
		public SharedBool bIsFreeMoving;
		public SharedBool bIsUCCCharacter;
		#endregion

		#region Properties
		IAllyMovable allyMovable
		{
			get
			{
				if(_allyMovable == null)
				{
					_allyMovable = GetComponent(typeof(IAllyMovable)) as IAllyMovable;
				}
				return _allyMovable;
			}
		}
		IAllyMovable _allyMovable = null;
		#endregion

		#region Overrides
		public override TaskStatus OnUpdate()
		{
			if (allyMovable != null)
			{
				allyMovable.MoveAlly(MyMoveDirection.Value, bIsFreeMoving.Value);
				return TaskStatus.Success;
			}
			return TaskStatus.Failure;
		}

		public override void OnPause(bool paused)
		{
			allyMovable.StopAllyMovement();
		}
		#endregion
	}
}