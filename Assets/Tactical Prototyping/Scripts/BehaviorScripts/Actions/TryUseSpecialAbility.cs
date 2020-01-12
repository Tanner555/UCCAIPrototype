using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using RTSCoreFramework;

namespace RTSPrototype
{
	[TaskCategory("RPGPrototype/AllyMember")]
	[TaskDescription("Try Using The Ability In The Ability To Use Slot. Retrieves Ability Animation Time From Ability Config.")]
	public class TryUseSpecialAbility : Action
	{
		#region Shared
		public SharedObject AbilityToUse;
		public SharedFloat AbilityAnimationTime;
		#endregion

		#region Properties
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
		#endregion

		#region Overrides
		public override TaskStatus OnUpdate()
		{
			AbilityAnimationTime.Value = ((AbilityConfig)AbilityToUse.Value).GetAbilityAnimationTime();
			myEventHandler.CallOnTryPerformSpecialAbility(AbilityToUse.Value.GetType());
			return TaskStatus.Success;
		}
		#endregion
	}
}