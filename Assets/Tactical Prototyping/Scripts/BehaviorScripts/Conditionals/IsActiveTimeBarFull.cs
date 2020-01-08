using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using RTSCoreFramework;

namespace RTSPrototype
{
	[TaskCategory("RPGPrototype/AllyMember")]
	[TaskDescription("Returns True if The Active Time Bar Is Full. Will Return False if AllyMember doesn't exist.")]
	public class IsActiveTimeBarFull : Conditional
	{
		#region Properties
		AllyMember allyMember
		{
			get
			{
				if(_allyMember == null)
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
			if (allyMember != null && allyMember.ActiveTimeBarIsFull())
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