using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using RTSCoreFramework;

namespace RTSPrototype
{
	[TaskCategory("RPGPrototype/AllyMember")]
	[TaskDescription("Checks If Ability Can Be Used.")]
	public class CanUseSpecialAbility : Conditional
	{
		#region Shared
		public SharedObject AbilityToUse;
		#endregion

		#region Properties
		protected AllyMember allymember
		{
			get
			{
				if (_allymember == null)
					_allymember = GetComponent<AllyMember>();

				return _allymember;
			}
		}
		AllyMember _allymember = null;
		#endregion

		#region Overrides
		public override TaskStatus OnUpdate()
		{
			if (allymember.CanUseAbility(AbilityToUse.Value.GetType()))
			{
				return TaskStatus.Success;
			}
			else
			{
				return TaskStatus.Failure;
			}
		}
		#endregion
	}
}