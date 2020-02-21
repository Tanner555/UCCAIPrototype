using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using RTSCoreFramework;

namespace RPGPrototype
{
	[TaskCategory("RPGPrototype/AllyMember")]
	[TaskDescription("Compares Current To Previous Tactics Item To Determine If New Tactics Item Was Obtained. If Current Tactics Equal Past Tactics, Condition Will Fail. CheckPreviousNullInstead Boolean Will Check Previous Tactics Instead.")]
	public class HasNewTacticsItem : Conditional
	{
		#region Shared
		public SharedBool CheckPreviousNullInstead;
		public SharedTacticsItem CurrentExecutionItem;
		public SharedAllyMember CurrentExecutionTarget;
		public SharedTacticsItem PreviousExecutionItem;
		public SharedAllyMember PreviousExecutionTarget;
		#endregion

		public override TaskStatus OnUpdate()
		{
			if (CheckPreviousNullInstead.Value == false)
			{
				if (CurrentExecutionItem.Value == PreviousExecutionItem.Value &&
					CurrentExecutionTarget.Value == PreviousExecutionTarget.Value)
				{
					return TaskStatus.Failure;
				}
				else
				{
					return TaskStatus.Success;
				}
			}
			else
			{
				if(PreviousExecutionItem.Value != null && PreviousExecutionTarget.Value != null)
				{
					return TaskStatus.Success;
				}
				return TaskStatus.Failure;
			}
		}
	}
}