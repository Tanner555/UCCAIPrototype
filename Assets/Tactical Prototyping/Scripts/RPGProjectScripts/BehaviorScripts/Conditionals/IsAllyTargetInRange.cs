using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using RTSCoreFramework;

namespace RPGPrototype {
	[TaskCategory("RPGPrototype/AllyMember")]
    [TaskDescription("Checks If Target is In Range Using RPG Weapon Attack Range.")]
	public class IsAllyTargetInRange : Conditional
	{
		#region Shared
		public SharedTransform CurrentTargettedEnemy;
		#endregion

		#region Properties
		AIControllerRPG aiController
		{
			get
			{
				if(_aiController == null)
				{
					_aiController = GetComponent<AIControllerRPG>();
				}
				return _aiController;
			}
		}
		AIControllerRPG _aiController = null;
		#endregion

		#region Overrides
		public override TaskStatus OnUpdate()
		{
			if (aiController.myRPGWeapon == null)
			{
				Debug.LogWarning("myRPGWeapon is NULL, couldn't update target in range task");
				return TaskStatus.Failure;
			}
            float distanceToTarget = (CurrentTargettedEnemy.Value.transform.position - transform.position).magnitude;
			if(distanceToTarget <= aiController.myRPGWeapon.GetMaxAttackRange())
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