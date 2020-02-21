using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using RTSCoreFramework;

namespace RPGPrototype
{
	[TaskCategory("RPGPrototype/AllyMember")]
	[TaskDescription("Empties The Active Time Bar. OnlyDepleteIfAboveMinimum Checkbox Will Only Deplete Time Bar if Amount is Greater Than 0.")]
	public class DepleteActiveTimeBar : Action
	{
		#region Shared
		public SharedBool OnlyDepleteIfAboveMinimum = false;
		#endregion

		#region Properties
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
			if (OnlyDepleteIfAboveMinimum.Value)
			{
				if (allyMember.AllyActiveTimeBar > allyMember.AllyMinActiveTimeBar)
				{
					allyMember.DepleteActiveTimeBar();
				}
			}
			else
			{
				allyMember.DepleteActiveTimeBar();
			}
			return TaskStatus.Success;
		}
		#endregion
	}
}