using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using RTSCoreFramework;

namespace RPGPrototype
{
	[TaskCategory("RPGPrototype/AllyMember")]
	[TaskDescription("Updates Active Time Bar if bUpdateActiveTimeBar is Set. ActiveTimeBarRefillRate Determines How Much the Bar is Filled On Each Update.")]
	public class UpdateActiveTimeBar : Action
	{
		#region Shared
		public SharedBool bUpdateActiveTimeBar;
		public SharedInt ActiveTimeBarRefillRate;
		#endregion

		#region Properties
		int AllyActiveTimeBar
		{
			get
			{
				return allyMember.AllyActiveTimeBar;
			}
			set
			{
				allyMember.AllyActiveTimeBar = value;
			}
		}

		int AllyMaxActiveTimeBar
		{
			get
			{
				return allyMember.AllyMaxActiveTimeBar;
			}
		}

		AllyMember allyMember
		{
			get
			{
				if (_allyMember == null)
				{
					_allyMember = GetComponent<AllyMember>();
				}
				return _allyMember;
			}
		}
		AllyMember _allyMember = null;
		#endregion

		#region Overrides
		public override TaskStatus OnUpdate()
		{
			if (bUpdateActiveTimeBar.Value && AllyActiveTimeBar < AllyMaxActiveTimeBar)
			{
				AllyActiveTimeBar = Mathf.Min(AllyActiveTimeBar + ActiveTimeBarRefillRate.Value, AllyMaxActiveTimeBar);
			}
			return TaskStatus.Success;
		}
		#endregion
	}
}