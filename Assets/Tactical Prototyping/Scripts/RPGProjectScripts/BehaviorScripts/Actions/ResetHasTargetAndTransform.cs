using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using RTSCoreFramework;

namespace RPGPrototype {
    [TaskCategory("RPGPrototype/AllyMember")]
    [TaskDescription("Resets bTargetEnemy and CurrentTargettedEnemy If Character Is Free Moving. AlsoResetsIfUsingAbility Checkbox Will Also Reset Target if Using Ability.")]
	public class ResetHasTargetAndTransform : Action
	{
		#region Shared
		public SharedBool bIsFreeMoving;
		public SharedBool bTargetEnemy;
		public SharedTransform CurrentTargettedEnemy;
		public SharedBool AlsoResetsIfUsingAbility;
		public SharedBool bTryUseAbility;
		#endregion

		#region Overrides
		public override void OnStart()
		{
		
		}

		public override TaskStatus OnUpdate()
		{
			if (AlsoResetsIfUsingAbility.Value)
			{
				//Also Reset If Using Ability
				if (bIsFreeMoving.Value || bTryUseAbility.Value)
				{
					bTargetEnemy.Value = false;
					CurrentTargettedEnemy.Value = null;
				}
			}
			else
			{
				//Normal FreeMoving Check
				if (bIsFreeMoving.Value)
				{
					bTargetEnemy.Value = false;
					CurrentTargettedEnemy.Value = null;
				}
			}
			return TaskStatus.Success;
		}
		#endregion

	} 
}