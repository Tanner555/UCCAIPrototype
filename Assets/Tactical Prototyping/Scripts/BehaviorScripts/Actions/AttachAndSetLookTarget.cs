using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using RTSCoreFramework;
using Opsive.UltimateCharacterController.Character;
using uccEventHelper = UtilitiesAndHelpersForUCC.UCCEventsControllerUtility;

namespace RTSPrototype
{
	[TaskCategory("RPGPrototype/AllyMember")]
	[TaskDescription("If Current Player and Not Targetting an Enemy, Attaches Look Source To Camera. Otherwise if Targetting Enemy, Attaches Look Source To Character.")]
	public class AttachAndSetLookTarget : Action
	{
		#region Shared
		public SharedBool bIsCurrentPlayer;
		public SharedBool bTargetEnemy;
		public SharedTransform CurrentTargettedEnemy;
		#endregion

		#region Properties
		LocalLookSource myLocalLookSource
		{
			get
			{
				if (_myLocalLookSource == null)
				{
					_myLocalLookSource = GetComponent<LocalLookSource>();
				}
				return _myLocalLookSource;
			}
		}
		LocalLookSource _myLocalLookSource = null;

		//RTSCameraController myCameraController
		//{
		//	get
		//	{
		//		if (_myCameraController == null)
		//		{
		//			_myCameraController = Camera.main.GetComponent<RTSCameraController>();
		//		}
		//		return _myCameraController;
		//	}
		//}
		//RTSCameraController _myCameraController = null;
		#endregion

		#region Overrides
		public override TaskStatus OnUpdate()
		{
			uccEventHelper.CallOnCharacterAttachLookSource(this.gameObject, myLocalLookSource);
			if (bIsCurrentPlayer.Value && bTargetEnemy.Value == false)
			{
				//Current Player and Not Targetting an Enemy
				//uccEventHelper.CallOnCharacterAttachLookSource(this.gameObject, myCameraController);			
				myLocalLookSource.Target = null;
			}
			else
			{
				//Not Current Player or Current Player is Targetting an Enemy
				//uccEventHelper.CallOnCharacterAttachLookSource(this.gameObject, myLocalLookSource);
				if (bTargetEnemy.Value)
				{
					myLocalLookSource.Target = CurrentTargettedEnemy.Value;
				}
				else
				{
					myLocalLookSource.Target = null;
				}
			}
			return TaskStatus.Success;
		}
		#endregion
	}
}