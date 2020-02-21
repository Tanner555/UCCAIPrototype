using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using RTSCoreFramework;

namespace RPGPrototype
{
	public class ResetTacticsItems : Action
	{
		#region Shared
		public SharedTacticsItem CurrentExecutionItem;
		public SharedAllyMember CurrentExecutionTarget;
		public SharedTacticsItem PreviousExecutionItem;
		public SharedAllyMember PreviousExecutionTarget;
		#endregion

		public override TaskStatus OnUpdate()
		{
			CurrentExecutionItem.Value = null;
			CurrentExecutionTarget.Value = null;
			PreviousExecutionItem.Value = null;
			PreviousExecutionTarget.Value = null;
			return TaskStatus.Success;
		}
	}
}