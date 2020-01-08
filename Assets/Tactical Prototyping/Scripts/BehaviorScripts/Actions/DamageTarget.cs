using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using RTSCoreFramework;

namespace RTSPrototype
{
	[TaskCategory("RPGPrototype/AllyMember")]
	[TaskDescription("Damages The Ally Target. Doesn't Use Any Animations.")]
	public class DamageTarget : Action
	{
		#region Shared
		public SharedTransform CurrentTargettedEnemy;
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

		AllyAIControllerWrapper aiController
		{
			get
			{
				if (_aiController == null)
				{
					_aiController = (AllyAIControllerWrapper)allyMember.aiController;
				}
				return _aiController;
			}
		}
		AllyAIControllerWrapper _aiController = null;
		#endregion

		#region Overrides
		public override TaskStatus OnUpdate()
		{
			//int _damage = aiController.GetDamageRate();
			//AllyMemberWrapper _ally = CurrentTargettedEnemy.Value.GetComponent<AllyMemberWrapper>();
			//_ally.AllyTakeDamage(_damage, allyMember);
			return TaskStatus.Success;
		}
		#endregion
	}
}