﻿using BaseFramework;
using RTSCoreFramework;
using UnityEngine;
using Chronos;
using Opsive.UltimateCharacterController;
using Opsive.UltimateCharacterController.Inventory;
using Opsive.UltimateCharacterController.Character;
using Opsive.UltimateCharacterController.Input;
using UnityEngine.AI;
using Opsive.UltimateCharacterController.Character.Abilities;
using Opsive.UltimateCharacterController.Character.Abilities.AI;
using Opsive.UltimateCharacterController.Game;
using uccEventHelper = UtilitiesAndHelpersForUCC.UCCEventsControllerUtility;

namespace RTSPrototype
{
    public class RTSNavBridge : MonoBehaviour
    {
        #region FieldsandProps
        protected string m_HorizontalInputName = "Horizontal";
        protected string m_ForwardInputName = "Vertical";
        //From NavMeshAgentBridge Script
        [Tooltip("Specifies how quickly the agent should slow down when arriving at the destination")]
        [SerializeField] protected AnimationCurve m_ArriveRampDownCurve = new AnimationCurve(new Keyframe[2] { new Keyframe(0, 1), new Keyframe(1, 0.25f) });

        NavMeshAgent m_NavMeshAgent
        {
            get
            {
                if(_m_NavMeshAgent == null)
                {
                    _m_NavMeshAgent = GetComponent<NavMeshAgent>();
                }
                return _m_NavMeshAgent;
            }
        }
        NavMeshAgent _m_NavMeshAgent = null;

        Transform m_Transform
        {
            get { return transform; }
        }

        Ability NavAbility
        {
            get
            {
                if(_navAbility == null)
                {
                    _navAbility = m_Controller.GetAbility<RTSNavMeshAgentMovement>();
                }
                return _navAbility;
            }
        }
        Ability _navAbility = null;


        UltimateCharacterLocomotion m_Controller
        {
            get
            {
                if(_m_Controller == null)
                {
                    _m_Controller = GetComponent<UltimateCharacterLocomotion>();
                }
                return _m_Controller;
            }
        }
        private UltimateCharacterLocomotion _m_Controller = null;

        //UltimateCharacterLocomotionHandler m_Handler
        //{
        //    get
        //    {
        //        if (_m_Handler == null)
        //        {
        //            _m_Handler = GetComponent<UltimateCharacterLocomotionHandler>();
        //        }
        //        return _m_Handler;
        //    }
        //}
        //private UltimateCharacterLocomotionHandler _m_Handler = null;

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

        LocalLookSource myLocalLookSource
        {
            get
            {
                if(_myLocalLookSource == null)
                {
                    _myLocalLookSource = GetComponent<LocalLookSource>();
                }
                return _myLocalLookSource;
            }
        }
        LocalLookSource _myLocalLookSource = null;

        RTSCameraController myCameraController
        {
            get
            {
                if(_myCameraController == null)
                {
                    _myCameraController = Camera.main.GetComponent<RTSCameraController>();
                }
                return _myCameraController;
            }
        }
        RTSCameraController _myCameraController = null;

        RTSGameMaster gamemaster { get { return RTSGameMaster.thisInstance; } }
        UnityMsgManager myUnityMsgManager { get { return UnityMsgManager.thisInstance;} }
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
        //protected Quaternion lookDestinationRotation
        //{
        //    get
        //    {
        //        return (isMoving && !ReachedDestination()) ?
        //            Quaternion.LookRotation(m_NavMeshAgent.destination - transform.position) :
        //            Quaternion.LookRotation(m_Transform.forward);
        //    }
        //}
        protected Quaternion mainCameraRotation
        {
            get
            {
                return Camera.main ? Camera.main.transform.rotation : new Quaternion(0, 0, 0, 0);
            }
        }

        //protected bool canUpdateMovement
        //{
        //    get { return isMoving; }
        //}

        //NavMeshMovement
        /// <summary>
        /// Temp Fix, Do Not Use This Prop
        /// </summary>
        //bool isMoving { get { return myEventHandler.bIsNavMoving; } }

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
        //New Look Functionality
        bool bIsLookingAtTarget = false;

        #endregion

        #region UnityMessages
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
        void TogglebIsLookingAtTarget(bool _enable)
        {
            bIsLookingAtTarget = _enable;
            if (bIsLookingAtTarget)
            {
                uccEventHelper.CallOnCharacterAttachLookSource(this.gameObject, myLocalLookSource);
                myLocalLookSource.Target = targetTransform;
            }
            else
            {
                uccEventHelper.CallOnCharacterAttachLookSource(this.gameObject, myCameraController);
            }
        }

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
        private void OnFixedUpdateHandler()
        {
            if (bIsTimelinePaused) return;
            //CheckForFreeMovement();
            //UpdateMovementOrRotate();
        }

        private void OnFinishMovingHandler()
        {
            //m_NavMeshAgent.SetDestination(transform.position);
            //m_NavMeshAgent.isStopped = true;
            m_NavMeshAgent.ResetPath();
            //m_NavMeshAgent.isStopped = false;
        }

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
            if (bIsTimelinePaused)
            {
                bPauseCommandMoveCached = true;
                pausedCommandMoveLocation = _destination;
            }
            if (allyMember.isSurfaceWalkable(_destination))
            {
                //If Command Moving and Looking at Target
                //Stop looking at target, and use Camera LookSource
                //if (myEventHandler.bIsCommandMoving && bIsLookingAtTarget)
                //{
                //    TogglebIsLookingAtTarget(false);
                //}
                m_NavMeshAgent.isStopped = false;
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
            //bIsSprinting = myEventHandler.bIsSprinting;
            //speedMultiplier = bIsSprinting ?
            //    sprintSpeed : walkSpeed;

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

        void HandleAllyDeath(Vector3 position, Vector3 force, GameObject attacker)
        {
            Destroy(this);
        }
        #endregion

        #region NavMeshMovement
        //void FinishMovingNavMesh()
        //{
        //    myEventHandler.CallEventFinishedMoving();
        //}

        //bool ReachedDestination()
        //{
        //    return m_NavMeshAgent != null && m_NavMeshAgent.enabled && m_NavMeshAgent.remainingDistance != Mathf.Infinity &&
        //        m_NavMeshAgent.remainingDistance <= 0.2f && !m_NavMeshAgent.pathPending && isMoving && m_NavMeshAgent.hasPath;
        //}
        #endregion

        #region MainMovementMethod
        //void MoveCharacterMain()
        //{
        //    var velocity = Vector3.zero;
        //    lookRotation = Quaternion.LookRotation(m_Transform.forward);

        //    // Only move if a path exists.
        //    // Update only when needed by targeting or move command
        //    if (canUpdateMovement && m_NavMeshAgent.desiredVelocity.sqrMagnitude > 0.01f)
        //    {
        //        if (m_NavMeshAgent.updateRotation)
        //        {
        //            lookRotation = Quaternion.LookRotation(m_NavMeshAgent.desiredVelocity);
        //        }
        //        else
        //        {
        //            lookRotation = Quaternion.LookRotation(m_Transform.forward);
        //        }
        //        // The normalized velocity should be relative to the look direction.
        //        velocity = Quaternion.Inverse(lookRotation) * m_NavMeshAgent.desiredVelocity;
        //        // Only normalize if the magnitude is greater than 1. This will allow the character to walk.
        //        if (velocity.sqrMagnitude > 1)
        //        {
        //            velocity.Normalize();
        //        }
        //        // Smoothly come to a stop at the destination.
        //        if (m_NavMeshAgent.remainingDistance < 1f)
        //        {
        //            velocity *= m_ArriveRampDownCurve.Evaluate(1 - m_NavMeshAgent.remainingDistance);
        //        }
        //        else
        //        {
        //            //Change Velocity to Speed Multiplier
        //            velocity *= speedMultiplier;
        //        }
        //    }

        //    // Don't let the NavMeshAgent move the character - the controller can move it.
        //    m_NavMeshAgent.updatePosition = false;
        //    m_NavMeshAgent.velocity = Vector3.zero;
        //    //m_Controller.Move(velocity.x, velocity.z, GetDeltaYawRotation(velocity.x, velocity.z, lookRotation));
        //    m_NavMeshAgent.nextPosition = m_Transform.position;
        //    //Check for end of destination if moving
        //    if (isMoving && ReachedDestination()) FinishMovingNavMesh();
        //}
        #endregion

        #region MoveOrRotateMethod
        //void UpdateMovementOrRotate()
        //{
        //    bool _isFreeMoving = myEventHandler.bIsFreeMoving;
        //    m_NavMeshAgent.updateRotation = !_isFreeMoving && isMoving;
        //    if (!_isFreeMoving)
        //    {
        //        //Change localRotation if targetting is active
        //        if ((bIsShooting || bIsMeleeing) && isTargeting && targetTransform != null)
        //        {
        //            //Stand Still and Rotate towards Target
        //            m_NavMeshAgent.updatePosition = false;
        //            m_NavMeshAgent.velocity = Vector3.zero;
        //            //Needs to Toggle Look Source If Still Using Camera Controller
        //            if(bIsLookingAtTarget == false)
        //            {
        //                TogglebIsLookingAtTarget(true);
        //            }
        //            //Vector3 velocity = Vector3.zero;
        //            //lookRotation = lookTargetRotation;
        //            //m_Controller.Move(velocity.x, velocity.z, GetDeltaYawRotation(velocity.x, velocity.z, lookRotation));
        //        }
        //        else
        //        {
        //            //Needs to Toggle Look Source If Still Using Local Look Source
        //            if (bIsLookingAtTarget)
        //            {
        //                TogglebIsLookingAtTarget(false);
        //            }
        //            //Still targetting enemy but enemy transform is null
        //            if (targetTransform == null && isTargeting)
        //                myEventHandler.CallEventStopTargettingEnemy();

        //            MoveCharacterMain();
        //        }
        //    }
        //    else if (allyMember.bIsCurrentPlayer)
        //    {
        //        MoveFreely();
        //    }
        //}
        #endregion

        #region FreeMovement
        void MoveFreely()
        {
            //myDirection *= speedMultiplier;
            m_NavMeshAgent.updatePosition = false;
            m_NavMeshAgent.velocity = Vector3.zero;
            //m_Controller.Move(myDirection.x, myDirection.z, GetDeltaYawRotation(myDirection.x, myDirection.z, mainCameraRotation));
            KinematicObjectManager.SetCharacterMovementInput(m_Controller.KinematicObjectIndex, myDirection.x, myDirection.z);
        }

        //void CheckForFreeMovement()
        //{
        //    if (allyMember.bIsCurrentPlayer == false) return;
            
        //    myHorizontalMovement = RTSPlayerInput.thisInstance.GetAxisRaw(m_HorizontalInputName);
        //    myForwardMovement = RTSPlayerInput.thisInstance.GetAxisRaw(m_ForwardInputName);
        //    myDirection = Vector3.zero;
        //    myDirection.x = myHorizontalMovement;
        //    myDirection.z = myForwardMovement;
        //    myDirection.y = 0;
        //    if(myDirection.sqrMagnitude > 0.01f)
        //    {
        //        if (isMoving == true)
        //        {
        //            FinishMovingNavMesh();
        //            //Stop Looking At Target, and Use Camera LookSource
        //            if (bIsLookingAtTarget)
        //            {
        //                TogglebIsLookingAtTarget(false);
        //            }
        //        }
                
        //        if (myEventHandler.bIsFreeMoving == false)
        //        {
        //            //if (NavAbility.CanStopAbility())
        //            //{
        //            //    NavAbility.StopAbility();
        //            //}
        //            NavAbility.Enabled = false;
        //            //foreach (var _a in m_Controller.ActiveAbilities)
        //            //{
        //            //    Debug.Log("Positional: " + _a.AllowPositionalInput.ToString());
        //            //    Debug.Log("Rotational: " + _a.AllowRotationalInput.ToString());
        //            //    //Debug.Log("Ability: " + _a);
        //            //}
        //            myEventHandler.CallEventTogglebIsFreeMoving(true);
        //        }
        //    }
        //    else
        //    {
        //        if (myEventHandler.bIsFreeMoving)
        //        {
        //            //if (NavAbility.CanStartAbility())
        //            //{
        //            //    NavAbility.StartAbility();
        //            //}
        //            NavAbility.Enabled = true;
        //            myEventHandler.CallEventTogglebIsFreeMoving(false);
        //        }
        //    }
        //}
        #endregion

        #region NavMeshChecking/Reset
        //void CheckWalkable()
        //{
        //    if (!allyMember.isSurfaceWalkable(m_NavMeshAgent.destination))
        //    {
        //        myEventHandler.CallEventFinishedMoving();
        //        Invoke("ResetNavmeshAgent", 0.1f);
        //    }
        //}

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

        #region Getters
        float GetDeltaYawRotation(float horizontal, float forward, Quaternion rotation)
        {
            var lookVector = RTSPlayerInput.thisInstance.GetLookVector(true);
            return m_Controller.ActiveMovementType.GetDeltaYawRotation(horizontal, forward, lookVector.x, lookVector.y);
        }
        #endregion

        #region Initialization
        void SubToEvents()
        {
            myEventHandler.EventCommandAttackEnemy += OnCommandAttack;
            myEventHandler.OnUpdateTargettedEnemy += UpdateTargettedEnemy;
            //myEventHandler.EventStopTargettingEnemy += OnCommandStopTargetting;
            //myEventHandler.EventToggleIsShooting += TogglebIsShooting;
            //myEventHandler.EventToggleIsMeleeing += TogglebIsMeleeing;
            //myEventHandler.EventToggleIsSprinting += OnToggleSprinting;
            //myEventHandler.EventCommandMove += MoveToDestination;
            myEventHandler.EventAllyDied += HandleAllyDeath;
            //myEventHandler.EventFinishedMoving += OnFinishMovingHandler;
            gamemaster.EventHoldingRightMouseDown += ToggleMoveCamera;
            gamemaster.OnTogglebIsInPauseControlMode += OnTogglePauseCommandMode;
            myUnityMsgManager.RegisterOnFixedUpdate(OnFixedUpdateHandler);
        }

        void UnsubFromEvents()
        {
            myEventHandler.EventCommandAttackEnemy -= OnCommandAttack;
            myEventHandler.OnUpdateTargettedEnemy -= UpdateTargettedEnemy;
            //myEventHandler.EventStopTargettingEnemy -= OnCommandStopTargetting;
            //myEventHandler.EventToggleIsShooting -= TogglebIsShooting;
            //myEventHandler.EventToggleIsMeleeing -= TogglebIsMeleeing;
            //myEventHandler.EventToggleIsSprinting -= OnToggleSprinting;
            //myEventHandler.EventCommandMove -= MoveToDestination;
            myEventHandler.EventAllyDied -= HandleAllyDeath;
            //myEventHandler.EventFinishedMoving -= OnFinishMovingHandler;
            gamemaster.EventHoldingRightMouseDown -= ToggleMoveCamera;
            gamemaster.OnTogglebIsInPauseControlMode -= OnTogglePauseCommandMode;
            myUnityMsgManager.DeregisterOnFixedUpdate(OnFixedUpdateHandler);
        }
        #endregion
    }

}