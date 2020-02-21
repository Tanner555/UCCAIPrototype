using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using RTSCoreFramework;

namespace RPGPrototype
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

		AIControllerRPG aiController
		{
			get
			{
				if (_aiController == null)
				{
					_aiController = (AIControllerRPG)allyMember.aiController;
				}
				return _aiController;
			}
		}
		AIControllerRPG _aiController = null;
		#endregion

		#region Overrides
		public override TaskStatus OnUpdate()
		{
			int _damage = aiController.GetDamageRate();
			AllyMemberRPG _ally = CurrentTargettedEnemy.Value.GetComponent<AllyMemberRPG>();
			_ally.AllyTakeDamage(_damage, allyMember);
			return TaskStatus.Success;
		}
		#endregion
	}
}