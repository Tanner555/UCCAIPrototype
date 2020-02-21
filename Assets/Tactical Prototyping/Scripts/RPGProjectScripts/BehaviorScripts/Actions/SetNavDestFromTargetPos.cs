using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using RTSCoreFramework;

namespace RPGPrototype {
	[TaskCategory("RPGPrototype/AllyMember")]
    [TaskDescription("Resets The Provided Character Navigation Movement BlackBoard Variables and Nav")]
	public class SetNavDestFromTargetPos : Action
	{
		#region Shared
		public SharedVector3 MyNavDestination;
		public SharedBool bHasSetDestination;
		public SharedTransform CurrentTargettedEnemy;
		#endregion

		#region Overrides
		public override TaskStatus OnUpdate()
		{
			MyNavDestination.Value = CurrentTargettedEnemy.Value.position;
			bHasSetDestination.Value = true;
			return TaskStatus.Success;
		}
		#endregion

	}
}