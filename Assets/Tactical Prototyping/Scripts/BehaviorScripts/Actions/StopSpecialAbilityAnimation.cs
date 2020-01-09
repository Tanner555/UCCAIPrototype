using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using RTSCoreFramework;

namespace RTSPrototype
{
	[TaskCategory("RPGPrototype/AllyMember")]
	[TaskDescription("Stop The Ability Animation From Ability To Use. Will Return Failure If Ability To Use is NULL.")]
	public class StopSpecialAbilityAnimation : Action
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
			if(AbilityToUse.Value != null)
			{
				var _behavior = allymember.GetAbilityBehavior(AbilityToUse.Value.GetType());
				if (_behavior != null)
				{
					_behavior.StopAbilityAnimation();
					return TaskStatus.Success;
				}
			}
			//Ability is Null, Return Failure
			return TaskStatus.Failure;
		}
		#endregion

	}
}