using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using RTSCoreFramework;
using UnityStandardAssets.CrossPlatformInput;

namespace RTSPrototype
{
    [TaskCategory("RPGPrototype/AllyMember")]
    [TaskDescription("Returns Success if Owner is Current Player and Movement Input Has Been Received.")]
    public class PlayerWantsFreeMovement : Conditional
	{
		#region Shared
		public SharedVector3 MyMoveDirection;
		public SharedBool bIsFreeMoving;
		#endregion

		#region Fields
		private float myHorizontalMovement, myForwardMovement = 0.0f;
		Vector3 myDirection = Vector3.zero;
		#endregion

		#region Properties
		AllyMember allyMember
		{
			get
			{
				if(_allymember == null)
				{
					_allymember = GetComponent<AllyMember>();
				}
				return _allymember;
			}
		}
		AllyMember _allymember = null;

		AllyEventHandler myEventHandler
		{
			get
			{
				if(_myEventhandler == null)
				{
					_myEventhandler = GetComponent<AllyEventHandler>();
				}
				return _myEventhandler;
			}
		}
		AllyEventHandler _myEventhandler = null;

		bool bIsAlive => allyMember != null && allyMember.IsAlive;

        Camera myCamera
        {
            get
            {
                if (_myCamera == null)
                    _myCamera = Camera.main;

                return _myCamera;
            }
        }
        Camera _myCamera = null;

		Vector3 CamForward
        {
            get
            {
                return Vector3.Scale(myCamera.transform.forward, new Vector3(1, 0, 1)).normalized;
            }
        }
		#endregion

		#region Overrides
		public override TaskStatus OnUpdate()
		{
			if (bIsAlive == false ||
                allyMember == null ||
                allyMember.bIsCurrentPlayer == false)
			{
				ResetFreeMoveDirection();
				bIsFreeMoving.Value = false;
				return TaskStatus.Failure;
			}

			myHorizontalMovement = CrossPlatformInputManager.GetAxis("Horizontal");
            myForwardMovement = CrossPlatformInputManager.GetAxis("Vertical");
			myDirection = Vector3.zero;
            myDirection.x = myHorizontalMovement;
            myDirection.z = myForwardMovement;
            myDirection.y = 0;

			if (myDirection.sqrMagnitude > 0.05f)
            {
                //if (myEventHandler.bIsNavMoving)
                //{
                //    myEventHandler.CallEventFinishedMoving();
                //}
                //if (myEventHandler.bIsFreeMoving == false)
                //{
                //    myEventHandler.CallEventTogglebIsFreeMoving(true);
                //}
				//Also Calculate Move Direction Used For Movement Task
				CalculateFreeMoveDirection();
				bIsFreeMoving.Value = true;
                return TaskStatus.Success;
            }
            else
            {
                //if (myEventHandler.bIsFreeMoving)
                //{
                //    myEventHandler.CallEventTogglebIsFreeMoving(false);
                //}
				ResetFreeMoveDirection();
				bIsFreeMoving.Value = false;
                return TaskStatus.Failure;
            }

		}

		public override void OnConditionalAbort()
		{
			Debug.Log($"Conditional Abort on {transform.name}");
			//myEventHandler.CallEventTogglebIsFreeMoving(false);
			//myEventHandler.CallEventFinishedMoving();
		}
		#endregion

		#region Helpers
		void ResetFreeMoveDirection()
		{
			MyMoveDirection.Value = Vector3.zero;
		}

		void CalculateFreeMoveDirection()
		{
			// X = Horizontal Z = Forward
            // calculate move direction to pass to character
            if (myCamera != null)
            {
                // calculate camera relative direction to move:
                MyMoveDirection.Value = myDirection.z * CamForward + myDirection.x * myCamera.transform.right;
            }
            else
            {
                // we use world-relative directions in the case of no main camera
                MyMoveDirection.Value = myDirection.z * Vector3.forward + myDirection.x * Vector3.right;
            }
		}
		#endregion
	}
}