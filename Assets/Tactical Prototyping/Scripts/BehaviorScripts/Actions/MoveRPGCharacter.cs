using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using RTSCoreFramework;
using Opsive.UltimateCharacterController.Game;
using Opsive.UltimateCharacterController.Character;

namespace RTSPrototype
{
	[TaskCategory("RPGPrototype/AllyMember")]
    [TaskDescription("Moves RPG Character From A Given Direction. This only moves the input vector, so make sure to rotate the character as well.")]
    public class MoveRPGCharacter : Action
	{
		#region Shared
		public SharedVector3 MyMoveDirection;
		public SharedBool bIsFreeMoving;

		//public SharedFloat stationaryTurnSpeed;
		//public SharedFloat movingTurnSpeed;
		//public SharedFloat moveThreshold;

		//public SharedFloat animatorForwardCap;
		//public SharedFloat animationSpeedMultiplier;

		#endregion

		#region Fields
		private float turnAmount, forwardAmount, turnSpeed;
		private Vector3 localMove;
		#endregion

		#region Properties
		UltimateCharacterLocomotion m_Controller
		{
			get
			{
				if (_m_Controller == null)
				{
					_m_Controller = GetComponent<UltimateCharacterLocomotion>();
				}
				return _m_Controller;
			}
		}
		private UltimateCharacterLocomotion _m_Controller = null;

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

		Animator myAnimator
		{
			get
			{
				if(_myAnimator == null)
				{
					_myAnimator = GetComponent<Animator>();
				}
				return _myAnimator;
			}
		}
		Animator _myAnimator = null;
		float speedMultiplier
        {
            get
            {
                return bIsFreeMoving.Value ?
                    1.2f * _baseSpeedMultiplier :
                    1.0f * _baseSpeedMultiplier;
            }
        }
		float _baseSpeedMultiplier = 1.0f;
		#endregion

		#region Overrides
		public override void OnStart()
		{
		
		}

		public override TaskStatus OnUpdate()
		{
			//SetForwardAndTurn(MyMoveDirection.Value);
			//ApplyExtraTurnRotation();
			//UpdateAnimator();
			KinematicObjectManager.SetCharacterMovementInput(m_Controller.KinematicObjectIndex, MyMoveDirection.Value.x, MyMoveDirection.Value.z);
			return TaskStatus.Success;
		}

		public override void OnPause(bool paused)
		{
			KinematicObjectManager.SetCharacterMovementInput(m_Controller.KinematicObjectIndex, 0, 0);
		}
		#endregion

		#region Helpers
		float GetDeltaYawRotation(float horizontal, float forward, Quaternion rotation)
		{
			var lookVector = RTSPlayerInput.thisInstance.GetLookVector(true);
			return m_Controller.ActiveMovementType.GetDeltaYawRotation(horizontal, forward, lookVector.x, lookVector.y);
		}

		//void SetForwardAndTurn(Vector3 movement)
  //      {
  //          // convert the world relative moveInput vector into a local-relative
  //          // turn amount and forward amount required to head in the desired direction
  //          if (movement.magnitude > moveThreshold.Value)
  //          {
  //              movement.Normalize();
  //          }
  //          localMove = transform.InverseTransformDirection(movement);
  //          //CheckGroundStatus();
  //          //localMove = Vector3.ProjectOnPlane(localMove, m_GroundNormal);
  //          turnAmount = Mathf.Atan2(localMove.x, localMove.z);
  //          forwardAmount = localMove.z;
  //      }

		//void ApplyExtraTurnRotation()
  //      {
  //          // help the character turn faster (this is in addition to root rotation in the animation)
  //          turnSpeed = Mathf.Lerp(stationaryTurnSpeed.Value, movingTurnSpeed.Value, forwardAmount);
  //          transform.Rotate(0, turnAmount * turnSpeed * Time.deltaTime, 0);
  //      }

		//void UpdateAnimator()
  //      {
  //          myAnimator.SetFloat("Forward", forwardAmount * animatorForwardCap.Value, 0.1f, Time.deltaTime);
  //          myAnimator.SetFloat("Turn", turnAmount, 0.1f, Time.deltaTime);
  //          myAnimator.speed = animationSpeedMultiplier.Value * speedMultiplier;
  //      }
		#endregion
	}
}