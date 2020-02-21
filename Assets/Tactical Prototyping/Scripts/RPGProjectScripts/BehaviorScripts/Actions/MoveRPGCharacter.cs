using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using RTSCoreFramework;

namespace RPGPrototype
{
	[TaskCategory("RPGPrototype/AllyMember")]
    [TaskDescription("Moves RPG Character From A Given Direction Using The Animator.")]
    public class MoveRPGCharacter : Action
	{
		#region Shared
		public SharedVector3 MyMoveDirection;
		public SharedBool bIsFreeMoving;

		public SharedFloat stationaryTurnSpeed;
		public SharedFloat movingTurnSpeed;
		public SharedFloat moveThreshold;

		public SharedFloat animatorForwardCap;
		public SharedFloat animationSpeedMultiplier;

		#endregion

		#region Fields
		private float turnAmount, forwardAmount, turnSpeed;
		private Vector3 localMove;
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
			SetForwardAndTurn(MyMoveDirection.Value);
            ApplyExtraTurnRotation();
            UpdateAnimator();
			return TaskStatus.Success;
		}
		#endregion

		#region Helpers
		void SetForwardAndTurn(Vector3 movement)
        {
            // convert the world relative moveInput vector into a local-relative
            // turn amount and forward amount required to head in the desired direction
            if (movement.magnitude > moveThreshold.Value)
            {
                movement.Normalize();
            }
            localMove = transform.InverseTransformDirection(movement);
            //CheckGroundStatus();
            //localMove = Vector3.ProjectOnPlane(localMove, m_GroundNormal);
            turnAmount = Mathf.Atan2(localMove.x, localMove.z);
            forwardAmount = localMove.z;
        }

		void ApplyExtraTurnRotation()
        {
            // help the character turn faster (this is in addition to root rotation in the animation)
            turnSpeed = Mathf.Lerp(stationaryTurnSpeed.Value, movingTurnSpeed.Value, forwardAmount);
            transform.Rotate(0, turnAmount * turnSpeed * Time.deltaTime, 0);
        }

		void UpdateAnimator()
        {
            myAnimator.SetFloat("Forward", forwardAmount * animatorForwardCap.Value, 0.1f, Time.deltaTime);
            myAnimator.SetFloat("Turn", turnAmount, 0.1f, Time.deltaTime);
            myAnimator.speed = animationSpeedMultiplier.Value * speedMultiplier;
        }
		#endregion
	}
}