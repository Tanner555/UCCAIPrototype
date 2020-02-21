using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using RTSCoreFramework;

namespace RPGPrototype
{
	[TaskCategory("RPGPrototype/AllyMember")]
	[TaskDescription("Resets Try Use Ability and Ability To Use Variables.")]
	public class ResetTryUseSpecialAbility : Action
	{
		#region Shared
		public SharedBool bTryUseAbility;
		public SharedBool bIsPerformingAbility;
		public SharedObject AbilityToUse;
		#endregion

		#region Overrides
		public override TaskStatus OnUpdate()
		{
			bTryUseAbility.Value = false;
			bIsPerformingAbility.Value = false;
			AbilityToUse.Value = null;
			return TaskStatus.Success;
		}
		#endregion

	}
}