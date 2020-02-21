using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using RTSCoreFramework;

namespace RPGPrototype {
	[TaskCategory("RPGPrototype/AllyMember")]
    [TaskDescription("Simple Valid Target Check using bTargetEnemy and CurrentTargettedEnemy. CheckAllyAndResetTargetIfFail Checkbox Will Check Ally (Not NULL and isAlive) And Reset Target if Fails")]
	public class IsAllyTargetValid : Conditional
	{
		#region Shared
		public SharedBool bTargetEnemy;
		public SharedTransform CurrentTargettedEnemy;
		[BehaviorDesigner.Runtime.Tasks.Tooltip("Check Ally And Will Reset Target if Fails")]
		public SharedBool CheckAllyAndResetTargetIfFail = false;
		#endregion

		#region Properties
		AllyMember CurrentTargettedEnemyAlly
		{
			get
			{
				//Don't Retrieve AllyMember if TargetTransform Doesn't Exist
				if(CurrentTargettedEnemy.Value == null) return null;
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
		#endregion

		#region Overrides
		public override TaskStatus OnUpdate()
		{
			if (CheckAllyAndResetTargetIfFail.Value)
			{
				//Ally Check and Reset If Failed
				if(bTargetEnemy.Value && CurrentTargettedEnemy.Value != null &&
					CurrentTargettedEnemyAlly != null && CurrentTargettedEnemyAlly.IsAlive)
				{
					return TaskStatus.Success;
				}
				else
				{
					bTargetEnemy.Value = false;
					CurrentTargettedEnemy.Value = null;
					_CurrentTargettedEnemyAlly = null;
					return TaskStatus.Failure;
				}	
			}
			else
			{
				//Simple Validity Check, No Reset
				if(bTargetEnemy.Value && CurrentTargettedEnemy.Value != null)
				{
					return TaskStatus.Success;
				}
				else
				{
					return TaskStatus.Failure;
				}	
			}		
		}

		public override void OnReset()
		{
			_CurrentTargettedEnemyAlly = null;
		}
		#endregion
	}
}