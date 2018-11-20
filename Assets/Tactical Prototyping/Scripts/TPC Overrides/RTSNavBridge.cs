using Opsive.ThirdPersonController;
using RTSCoreFramework;
using UnityEngine;
using Chronos;

namespace RTSPrototype
{
    public class RTSNavBridge : NavMeshAgentBridge
    {
        #region FieldsandProps
        AllyMember allyMember
        {
            get
            {
                if (_allyMember == null)
                    _allyMember = GetComponent<AllyMember>();

                return _allyMember;
            }
        }
        AllyMember _allyMember = null;

        AllyEventHandlerWrapper myEventHandler
        {
            get
            {
                if (_myEventHandler == null)
                    _myEventHandler = GetComponent<AllyEventHandlerWrapper>();

                return _myEventHandler;
            }
        }
        AllyEventHandlerWrapper _myEventHandler;

        RTSGameMaster gamemaster { get { return RTSGameMaster.thisInstance; } }
        //Targeting
        public Transform targetTransform { get; protected set; }
        public bool isTargeting { get; protected set; }
        protected Quaternion lookTargetRotation
        {
            get
            {
                return targetTransform != null ? Quaternion.LookRotation(targetTransform.position - transform.position) : new Quaternion();
            }
        }
        protected Quaternion lookDestinationRotation
        {
            get
            {
                return (isMoving && !ReachedDestination()) ?
                    Quaternion.LookRotation(m_NavMeshAgent.destination - transform.position) :
                    Quaternion.LookRotation(m_Transform.forward);
            }
        }
        protected Quaternion mainCameraRotation
        {
            get
            {
                return Camera.main ? Camera.main.transform.rotation : new Quaternion(0, 0, 0, 0);
            }
        }

        protected bool canUpdateMovement
        {
            get { return isMoving; }
        }

        //NavMeshMovement
        bool isMoving { get { return myEventHandler.bIsNavMoving; } }

        //Camera is Moving
        private bool moveCamera = false;
        
        //See if need to rotate or continue moving
        bool bIsShooting = false;
        bool bIsMeleeing = false;
        //LookRotation Local Variable
        Quaternion lookRotation;
        //Sprinting
        bool bIsSprinting = false;
        float speedMultiplier = 1f;
        float walkSpeed = 1f;
        float sprintSpeed = 1.5f;
        //Free Movement Fields
        float myHorizontalMovement = 0.0f;
        float myForwardMovement = 0.0f;
        Vector3 myDirection = Vector3.zero;
        //Pause Functionality
        private bool bPauseCommandMoveCached = false;
        private Vector3 pausedCommandMoveLocation;
        private bool bIsTimelinePaused
        {
            get { return myEventHandler.bAllyIsPaused; }
        }

        #endregion

        #region UnityMessages
        protected override void FixedUpdate()
        {
            if (bIsTimelinePaused) return;
            CheckForFreeMovement();
            UpdateMovementOrRotate();
        }

        private void Start()
        {
            OnToggleSprinting();
            SubToEvents();
        }

        private void OnDisable()
        {
            UnsubFromEvents();
        }
        #endregion

        #region Targetting
        public void LookAtTarget(Transform _target)
        {
            if (_target != null)
            {
                targetTransform = _target;
                isTargeting = true;
            }
            else
            {
                StopTargeting();
            }
        }

        public void StopTargeting()
        {
            targetTransform = null;
            isTargeting = false;
        }
        #endregion

        #region Handlers
        void OnTogglePauseCommandMode(bool _isPaused)
        {
            //UnPausing and Command Moving During Pause
            if(_isPaused == false && bPauseCommandMoveCached)
            {
                Invoke("TriggerCommandMoveFromPause", 0.1f);
            }
            bPauseCommandMoveCached = false;
        }

        void TriggerCommandMoveFromPause()
        {
            MoveToDestination(pausedCommandMoveLocation);
        }

        void MoveToDestination(Vector3 _destination)
        {
            if(bIsTimelinePaused)
            {
                bPauseCommandMoveCached = true;
                pausedCommandMoveLocation = _destination;
            }
            if (allyMember.isSurfaceWalkable(_destination))
            {
                m_NavMeshAgent.ResetPath();
                m_NavMeshAgent.SetDestination(_destination);
                float _distance = Vector3.Distance(m_NavMeshAgent.path.corners[0], m_Transform.position);
                if (_distance >= 3.0f)
                {
                    Debug.Log("Path is not accurate, updating position...");
                    m_NavMeshAgent.updatePosition = true;
                }
            }
            Invoke("CheckWalkable", 0.2f);
        }

        void OnCommandAttack(AllyMember _ally)
        {
            if (_ally != null)
                LookAtTarget(_ally.transform);
        }

        protected virtual void UpdateTargettedEnemy(AllyMember _ally)
        {
            if (_ally == null) return;
            Transform _allyTransform = _ally.transform;
            if(_allyTransform != targetTransform)
            {
                targetTransform = _allyTransform;
            }
        }

        void OnToggleSprinting()
        {
            bIsSprinting = myEventHandler.bIsSprinting;
            speedMultiplier = bIsSprinting ?
                sprintSpeed : walkSpeed;

        }

        void OnCommandStopTargetting()
        {
            StopTargeting();
        }

        void ToggleMoveCamera(bool _enable)
        {
            moveCamera = _enable;
        }

        void TogglebIsShooting(bool _enable)
        {
            bIsShooting = _enable;
        }

        void TogglebIsMeleeing(bool _enable)
        {
            bIsMeleeing = _enable;
        }

        void HandleAllyDeath()
        {
            Destroy(this);
        }
        #endregion

        #region NavMeshMovement
        void FinishMovingNavMesh()
        {
            myEventHandler.CallEventFinishedMoving();
        }

        bool ReachedDestination()
        {
            return m_NavMeshAgent != null && m_NavMeshAgent.enabled && m_NavMeshAgent.remainingDistance != Mathf.Infinity &&
                m_NavMeshAgent.remainingDistance <= 0.2f && !m_NavMeshAgent.pathPending && isMoving && m_NavMeshAgent.hasPath;
        }
        #endregion

        #region MainMovementMethod
        void MoveCharacterMain()
        {
            var velocity = Vector3.zero;
            lookRotation = Quaternion.LookRotation(m_Transform.forward);
            
            // Only move if a path exists.
            // Update only when needed by targeting or move command
            if (canUpdateMovement && m_NavMeshAgent.desiredVelocity.sqrMagnitude > 0.01f)
            {
                if (m_NavMeshAgent.updateRotation)
                {
                    lookRotation = Quaternion.LookRotation(m_NavMeshAgent.desiredVelocity);
                }
                else
                {
                    lookRotation = Quaternion.LookRotation(m_Transform.forward);
                }
                // The normalized velocity should be relative to the look direction.
                velocity = Quaternion.Inverse(lookRotation) * m_NavMeshAgent.desiredVelocity;
                // Only normalize if the magnitude is greater than 1. This will allow the character to walk.
                if (velocity.sqrMagnitude > 1)
                {
                    velocity.Normalize();
                }
                // Smoothly come to a stop at the destination.
                if (m_NavMeshAgent.remainingDistance < 1f)
                {
                    velocity *= m_ArriveRampDownCurve.Evaluate(1 - m_NavMeshAgent.remainingDistance);
                }
                else
                {
                    //Change Velocity to Speed Multiplier
                    velocity *= speedMultiplier;
                }
            }

            // Don't let the NavMeshAgent move the character - the controller can move it.
            m_NavMeshAgent.updatePosition = false;
            m_NavMeshAgent.velocity = Vector3.zero;
            m_Controller.Move(velocity.x, velocity.z, lookRotation);
            m_NavMeshAgent.nextPosition = m_Transform.position;
            //Check for end of destination if moving
            if (isMoving && ReachedDestination()) FinishMovingNavMesh();
        }
        #endregion

        #region MoveOrRotateMethod
        void UpdateMovementOrRotate()
        {
            bool _isFreeMoving = myEventHandler.bIsFreeMoving;
            m_NavMeshAgent.updateRotation = !_isFreeMoving && isMoving;
            if (!_isFreeMoving)
            {
                //Change localRotation if targetting is active
                if ((bIsShooting || bIsMeleeing) && isTargeting && targetTransform != null)
                {
                    //Stand Still and Rotate towards Target
                    m_NavMeshAgent.updatePosition = false;
                    m_NavMeshAgent.velocity = Vector3.zero;
                    Vector3 velocity = Vector3.zero;
                    lookRotation = lookTargetRotation;
                    m_Controller.Move(velocity.x, velocity.z, lookRotation);
                }
                else
                {
                    //Still targetting enemy but enemy transform is null
                    if (targetTransform == null && isTargeting)
                        myEventHandler.CallEventStopTargettingEnemy();

                    MoveCharacterMain();
                }
            }
            else if (allyMember.bIsCurrentPlayer)
            {
                MoveFreely();
            }
        }
        #endregion

        #region FreeMovement
        void MoveFreely()
        {
            myDirection *= speedMultiplier;
            m_NavMeshAgent.updatePosition = false;
            m_NavMeshAgent.velocity = Vector3.zero;
            m_Controller.Move(myDirection.x, myDirection.z, mainCameraRotation);
        }

        void CheckForFreeMovement()
        {
            if (allyMember.bIsCurrentPlayer == false) return;

            myHorizontalMovement = RTSPlayerInput.thisInstance.GetAxisRaw(Constants.HorizontalInputName);
            myForwardMovement = RTSPlayerInput.thisInstance.GetAxisRaw(Constants.ForwardInputName);
            myDirection = Vector3.zero;
            myDirection.x = myHorizontalMovement;
            myDirection.z = myForwardMovement;
            myDirection.y = 0;
            if(myDirection.sqrMagnitude > 0.01f)
            {
                if (isMoving == true)
                {
                    FinishMovingNavMesh();
                }
                myEventHandler.CallEventTogglebIsFreeMoving(true);
            }
            else
            {
                if (myEventHandler.bIsFreeMoving)
                    myEventHandler.CallEventTogglebIsFreeMoving(false);
            }
        }
        #endregion

        #region NavMeshChecking/Reset
        void CheckWalkable()
        {
            if (!allyMember.isSurfaceWalkable(m_NavMeshAgent.destination))
            {
                myEventHandler.CallEventFinishedMoving();
                Invoke("ResetNavmeshAgent", 0.1f);
            }
        }

        void ResetNavmeshAgent()
        {
            Debug.Log("Destination isn't walkable, resetting agent");
            ToggleNavMeshAgent();
            Invoke("ToggleNavMeshAgent", 0.1f);
        }

        void ToggleNavMeshAgent()
        {
            m_NavMeshAgent.enabled = !m_NavMeshAgent.enabled;
        }
        #endregion

        #region Initialization
        void SubToEvents()
        {
            myEventHandler.EventCommandAttackEnemy += OnCommandAttack;
            myEventHandler.OnUpdateTargettedEnemy += UpdateTargettedEnemy;
            myEventHandler.EventStopTargettingEnemy += OnCommandStopTargetting;
            myEventHandler.EventToggleIsShooting += TogglebIsShooting;
            myEventHandler.EventToggleIsMeleeing += TogglebIsMeleeing;
            myEventHandler.EventToggleIsSprinting += OnToggleSprinting;
            myEventHandler.EventCommandMove += MoveToDestination;
            myEventHandler.EventAllyDied += HandleAllyDeath;
            gamemaster.EventHoldingRightMouseDown += ToggleMoveCamera;
            gamemaster.OnTogglebIsInPauseControlMode += OnTogglePauseCommandMode;
        }

        void UnsubFromEvents()
        {
            myEventHandler.EventCommandAttackEnemy -= OnCommandAttack;
            myEventHandler.OnUpdateTargettedEnemy -= UpdateTargettedEnemy;
            myEventHandler.EventStopTargettingEnemy -= OnCommandStopTargetting;
            myEventHandler.EventToggleIsShooting -= TogglebIsShooting;
            myEventHandler.EventToggleIsMeleeing -= TogglebIsMeleeing;
            myEventHandler.EventToggleIsSprinting -= OnToggleSprinting;
            myEventHandler.EventCommandMove -= MoveToDestination;
            myEventHandler.EventAllyDied -= HandleAllyDeath;
            gamemaster.EventHoldingRightMouseDown -= ToggleMoveCamera;
            gamemaster.OnTogglebIsInPauseControlMode -= OnTogglePauseCommandMode;
        }
        #endregion
    }
}