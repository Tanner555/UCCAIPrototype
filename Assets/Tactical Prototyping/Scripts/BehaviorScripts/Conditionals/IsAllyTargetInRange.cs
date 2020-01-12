using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using RTSCoreFramework;

namespace RTSPrototype
{
	[TaskCategory("RPGPrototype/AllyMember")]
    [TaskDescription("Checks If Target is In Range Using RPG Weapon Attack Range.")]
	public class IsAllyTargetInRange : Conditional
	{
		#region Shared
		public SharedBool bIsTargetTooClose;
		public SharedTransform CurrentTargettedEnemy;
		#endregion

		#region Properties
		AllyMember CurrentTargettedEnemyAlly
		{
			get
			{
				//Don't Retrieve AllyMember if TargetTransform Doesn't Exist
				if (CurrentTargettedEnemy.Value == null) return null;
				//If TargetAllyComp is NULL or TargetAllyComp is a Reference of another Ally (Switched Target)
				if (_CurrentTargettedEnemyAlly == null ||
					_CurrentTargettedEnemyAlly.transform != CurrentTargettedEnemy.Value)
				{
					_CurrentTargettedEnemyAlly = CurrentTargettedEnemy.Value.GetComponent<AllyMember>();
				}
				return _CurrentTargettedEnemyAlly;
			}
		}
		AllyMember _CurrentTargettedEnemyAlly = null;

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
				if(_aiController == null)
				{
					_aiController = GetComponent<AllyAIControllerWrapper>();
				}
				return _aiController;
			}
		}
		AllyAIControllerWrapper _aiController = null;
		#endregion

		#region Overrides
		public override TaskStatus OnUpdate()
		{
			//If Out of Range, You Won't Be Too Close To Target. Unless Ally has LOS with himself (bug).
			bIsTargetTooClose.Value = false;

			if (allyMember.bIsCarryingMeleeWeapon)
			{
				if (aiController.IsTargetInMeleeRange(CurrentTargettedEnemy.Value.gameObject))
				{
					CheckIfTargetIsTooClose();
					return TaskStatus.Success;
				}
			}
			else
			{
				RaycastHit _hit;
				if (aiController.hasLOSWithinRange(CurrentTargettedEnemyAlly, out _hit))
				{
					CheckIfTargetIsTooClose();
					return TaskStatus.Success;
				}
			}
			return TaskStatus.Failure;
		}
		#endregion

		#region Helpers
		void CheckIfTargetIsTooClose()
		{
			if(allyMember.MinimumTargetRange >= allyMember.MaxMeleeAttackDistance)
			{
				Debug.LogError($"Minimum Target Range {allyMember.MinimumTargetRange} is Greater or Equal To MaxMeleeAttackDistance {allyMember.MaxMeleeAttackDistance}. Cannot Check If Target Is Too Close");
				bIsTargetTooClose.Value = false;
			}
			else
			{
				float _distanceToTarget = (CurrentTargettedEnemy.Value.position - transform.position).magnitude;
				if(_distanceToTarget <= allyMember.MinimumTargetRange)
				{
					bIsTargetTooClose.Value = true;
				}
			}
		}
		#endregion
	}
}