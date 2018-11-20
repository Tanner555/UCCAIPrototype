using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RTSCoreFramework
{
    public class AllyAIController : MonoBehaviour
    {
        #region Components
        protected NavMeshAgent myNavAgent
        {
            get
            {
                if (_myNavAgent == null)
                    _myNavAgent = GetComponent<NavMeshAgent>();

                return _myNavAgent;
            }
        }
        private NavMeshAgent _myNavAgent = null;
        protected AllyEventHandler myEventHandler
        {
            get
            {
                if (_myEventHandler == null)
                    _myEventHandler = GetComponent<AllyEventHandler>();

                return _myEventHandler;
            }
        }
        private AllyEventHandler _myEventHandler = null;
        protected AllyMember allyMember
        {
            get
            {
                if (_allyMember == null)
                    _allyMember = GetComponent<AllyMember>();

                return _allyMember;
            }
        }
        private AllyMember _allyMember = null;
        #endregion

        #region Fields
        protected bool bIsShooting = false;
        protected bool bIsMeleeing
        {
            get { return myEventHandler.bIsMeleeingEnemy; }
        }
        protected float defaultFireRepeatRate = 0.25f;
        //Used for finding closest ally
        [Header("AI Finder Properties")]
        public float sightRange = 40f;
        public float followDistance = 5f;
        //Private Layers using gamemode values
        //Set to -1 to compare an unset layer
        private LayerMask __allyLayers = -1;
        private LayerMask __sightLayers = -1;

        protected Collider[] colliders;
        protected List<Transform> uniqueTransforms = new List<Transform>();
        protected List<AllyMember> scanEnemyList = new List<AllyMember>();
        #endregion

        #region Properties
        protected RTSGameMaster gamemaster
        {
            get { return RTSGameMaster.thisInstance; }
        }

        protected RTSGameMode gamemode
        {
            get { return RTSGameMode.thisInstance; }
        }

        public AllyMember currentTargettedEnemy
        {
            get { return __currentTargettedEnemy; }
            set
            {
                __currentTargettedEnemy = value;
                myEventHandler.CallOnUpdateTargettedEnemy(__currentTargettedEnemy);
            }
        }
        private AllyMember __currentTargettedEnemy = null;
        public AllyMember previousTargettedEnemy { get; protected set; }
        public AllyMember allyInCommand { get { return allyMember.partyManager.AllyInCommand; } }

        //AllyMember Transforms
        Transform headTransform { get { return allyMember.HeadTransform; } }
        Transform chestTransform { get { return allyMember.ChestTransform; } }
        Transform losTransform { get { return allyMember.MyLOSTransform; } }

        //Layer Props
        public LayerMask allyLayers
        {
            get
            {
                if (__allyLayers == -1)
                    __allyLayers = gamemode.AllyLayers;

                return __allyLayers;
            }
        }

        public LayerMask sightLayers
        {
            get
            {
                if (__sightLayers == -1)
                    __sightLayers = gamemode.SightLayers;

                return __sightLayers;
            }
        }

        protected bool bIsMoving
        {
            get { return myEventHandler.bIsNavMoving; }
        }

        protected bool bCarryingRangeAndNoAmmoLeft =>
            allyMember.bIsCarryingMeleeWeapon ?
                false : allyMember.CurrentEquipedAmmo <= 0;

        protected bool bStopUpdatingBattleBehavior
        {
            get
            {
                return currentTargettedEnemy == null ||
                currentTargettedEnemy.IsAlive == false ||
                myEventHandler.bIsFreeMoving ||
                //Carrying Ranged Weapon and
                //Doesn't Have Any Ammo Left 
                bCarryingRangeAndNoAmmoLeft;
            }
        }

        protected virtual bool AllCompsAreValid
        {
            get
            {
                return myNavAgent && myEventHandler
                    && allyMember;
            }
        }
        #endregion

        #region UnityMessages
        protected virtual void OnEnable()
        {
            SubToEvents();
        }

        // Use this for initialization
        protected virtual void Start()
        {
            if (!AllCompsAreValid)
            {
                Debug.LogError("Not all comps are valid!");
            }
            StartServices();
        }

        protected virtual void Update()
        {

        }

        protected virtual void LateUpdate()
        {

        }

        protected virtual void OnDisable()
        {
            UnSubFromEvents();
            CancelServices();
        }

        protected virtual void OnDrawGizmos()
        {

        }
        #endregion

        #region Getters
        public virtual bool isEnemyFor(Transform _transform, out AllyMember _ally)
        {
            _ally = null;
            if (_transform.root.GetComponent<AllyMember>())
                _ally = _transform.root.GetComponent<AllyMember>();

            return _ally != null && allyMember.IsEnemyFor(_ally);
        }

        public virtual bool isSurfaceWalkable(RaycastHit hit)
        {
            return myNavAgent.enabled && myNavAgent.CalculatePath(hit.point, myNavAgent.path) &&
            myNavAgent.path.status == NavMeshPathStatus.PathComplete;
        }

        public virtual bool isSurfaceWalkable(Vector3 _point)
        {
            return myNavAgent.enabled && myNavAgent.CalculatePath(_point, myNavAgent.path) &&
            myNavAgent.path.status == NavMeshPathStatus.PathComplete;
        }

        public virtual float GetAttackRate()
        {
            return allyMember.WeaponAttackRate;
        }

        protected virtual bool IsTargetInMeleeRange(GameObject _target)
        {
            bool _isCarryingMelee = allyMember.bIsCarryingMeleeWeapon;
            if (_isCarryingMelee == false) return false;
            float _distanceToTarget = (_target.transform.position - transform.position).magnitude;
            return _distanceToTarget <= allyMember.MaxMeleeAttackDistance;
        }
        #endregion

        #region Handlers
        protected virtual void OnAllyInitComps(RTSAllyComponentSpecificFields _specific, RTSAllyComponentsAllCharacterFields _allFields)
        {
            sightRange = _allFields.sightRange;
            followDistance = _allFields.followDistance;
        }

        protected virtual void OnWeaponChanged(EEquipType _eType, EWeaponType _weaponType, EWeaponUsage _wUsage, bool _equipped)
        {
            if (IsInvoking("UpdateBattleBehavior"))
            {
                bool _commandAttackRestart = myEventHandler.bIsCommandAttacking ? true : false;
                AllyMember _currentTargetRestart = currentTargettedEnemy;
                myEventHandler.CallEventFinishedMoving();
                myEventHandler.CallEventStopTargettingEnemy();
                StartCoroutine(OnWeaponChangedDelay(_commandAttackRestart, _currentTargetRestart));
            }
        }

        /// <summary>
        /// Used To Delay The Restarting of Attacking Enemy Target
        /// </summary>
        protected virtual IEnumerator OnWeaponChangedDelay(bool _isCommand, AllyMember _ally)
        {
            yield return new WaitForSeconds(0.2f);
            if (_isCommand)
            {
                myEventHandler.CallEventPlayerCommandAttackEnemy(_ally);
            }
            else
            {
                myEventHandler.CallEventAICommandAttackEnemy(_ally);
            }
        }

        protected virtual void HandleCommandAttackEnemy(AllyMember enemy)
        {
            CommandAttackEnemy(enemy);
        }

        protected virtual void HandleStopTargetting()
        {
            currentTargettedEnemy = null;
            StopBattleBehavior();
            CancelInvoke();
        }

        protected virtual void HandleOnMoveAlly(Vector3 _point)
        {
            if (myEventHandler.bIsCommandMoving)
            {
                if (IsInvoking("UpdateBattleBehavior"))
                    StopBattleBehavior();
            }
        }

        protected virtual void HandleOnAIStopMoving()
        {

        }

        protected virtual void OnEnableCameraMovement(bool _enable)
        {

        }

        protected virtual void TogglebIsShooting(bool _enable)
        {
            bIsShooting = _enable;
        }
        #endregion

        #region AITacticsCommands
        public virtual bool IsWithinFollowingDistance()
        {
            //Temp fix for PartyManager Delaying AllyInCommand Init Methods
            var _allyInCommand = allyInCommand;
            if (_allyInCommand == null)
            {
                Debug.Log("IsWithinFollowingDistance: Ally In Command is Null");
                return false;
            }

            return Vector3.Distance(transform.position,
                _allyInCommand.transform.position) <= followDistance;
        }

        public virtual bool Tactics_IsEnemyWithinSightRange()
        {
            AllyMember _closestEnemy = FindClosestEnemy();
            bool _valid = _closestEnemy != null && _closestEnemy.IsAlive;
            if (_valid)
            {
                currentTargettedEnemy = _closestEnemy;
            }
            else
            {
                if (currentTargettedEnemy == null ||
                    !currentTargettedEnemy.IsAlive)
                {
                    currentTargettedEnemy = null;
                }
            }
            return (_valid);
        }

        public virtual void Tactics_MoveToLeader()
        {
            if (allyMember.bIsGeneralInCommand) return;

            //Temporarily Fixes Bug with Ally Attacking 
            //An Enemy After They Are Set To Command 
            //After Tactics Have Been Followed when Switching
            //From Command
            if (IsInvoking("UpdateBattleBehavior"))
            {
                StopBattleBehavior();
                myEventHandler.CallEventStopTargettingEnemy();
            }

            if (IsWithinFollowingDistance() == false)
            {
                myEventHandler.CallEventAIMove(allyInCommand.transform.position);
            }
            else
            {
                if (myEventHandler.bIsAIMoving == true)
                {
                    myEventHandler.CallEventFinishedMoving();
                }
            }
        }

        public virtual void AttackTargettedEnemy()
        {
            if (currentTargettedEnemy != null &&
                currentTargettedEnemy.IsAlive)
            {
                if (myEventHandler.bIsAiAttacking == false)
                    myEventHandler.CallEventAICommandAttackEnemy(currentTargettedEnemy);
            }
            else if (myEventHandler.bIsAiAttacking)
            {
                myEventHandler.CallEventStopTargettingEnemy();
            }
        }

        //public void Tactics_AttackClosestEnemy()
        //{
        //    if(currentTargettedEnemy == null || currentTargettedEnemy.IsAlive == false)
        //    {
        //        AllyMember _closestEnemy = FindClosestEnemy();
        //        if (_closestEnemy != null)
        //        {
        //            currentTargettedEnemy = _closestEnemy;
        //            if (myEventHandler.bIsAiAttacking == false && currentTargettedEnemy != null)
        //            {
        //                myEventHandler.CallEventAICommandAttackEnemy(currentTargettedEnemy);
        //            }
        //        }
        //    }
        //}
        #endregion

        #region AITacticsHelpers
        protected virtual AllyMember FindClosestEnemy()
        {
            AllyMember _closestEnemy = null;
            if (headTransform == null)
            {
                Debug.LogError("No head assigned on AIController, cannot run look service");
                return _closestEnemy;
            }
            colliders = Physics.OverlapSphere(transform.position, sightRange, allyLayers);
            AllyMember _enemy = null;
            scanEnemyList.Clear();
            uniqueTransforms.Clear();
            foreach (Collider col in colliders)
            {
                if (uniqueTransforms.Contains(col.transform.root)) continue;
                uniqueTransforms.Add(col.transform.root);
                if (isEnemyFor(col.transform, out _enemy))
                {
                    RaycastHit hit;
                    if (hasLOSWithinRange(_enemy, out hit))
                    {
                        if (hit.transform.root == _enemy.transform.root)
                            scanEnemyList.Add(_enemy);
                    }
                }
            }

            if (scanEnemyList.Count > 0)
                _closestEnemy = DetermineClosestAllyFromList(scanEnemyList);

            return _closestEnemy;
        }

        protected virtual bool hasLOSWithinRange(AllyMember _enemy, out RaycastHit _hit)
        {
            RaycastHit _myHit;
            bool _bHit = Physics.Linecast(losTransform.position,
                        _enemy.ChestTransform.position, out _myHit);
            _hit = _myHit;
            bool _valid = _bHit && _myHit.transform != null &&
                _myHit.transform.root.tag == gamemode.AllyTag;
            if (_valid)
            {
                AllyMember _hitAlly = _myHit.transform.root.GetComponent<AllyMember>();
                if (_hitAlly == allyMember)
                {
                    Debug.Log(allyMember.CharacterName +
                        " Has LOS With Himself.");
                }
                //TODO: RTSPrototype Fix hasLosWithinRange() hitting self instead of enemy
                return _hitAlly != null &&
                    (_hitAlly == allyMember ||
                    _hitAlly.IsEnemyFor(allyMember));
            }
            return false;
        }

        protected virtual AllyMember DetermineClosestAllyFromList(List<AllyMember> _allies)
        {
            AllyMember _closestAlly = null;
            float _closestDistance = Mathf.Infinity;
            foreach (var _ally in _allies)
            {
                float _newDistance = Vector3.Distance(_ally.transform.position,
                    transform.position);
                if (_newDistance < _closestDistance)
                {
                    _closestDistance = _newDistance;
                    _closestAlly = _ally;
                }
            }
            return _closestAlly;
        }
        #endregion

        #region ShootingAndBattleBehavior
        protected virtual void CommandAttackEnemy(AllyMember enemy)
        {
            previousTargettedEnemy = currentTargettedEnemy;
            currentTargettedEnemy = enemy;
            if (IsInvoking("UpdateBattleBehavior") == false)
            {
                StartBattleBehavior();
            }
            else if (IsInvoking("UpdateBattleBehavior") && previousTargettedEnemy != currentTargettedEnemy)
            {
                StopBattleBehavior();
                Invoke("StartBattleBehavior", 0.05f);
            }
        }

        protected virtual void UpdateBattleBehavior()
        {
            // Pause Ally Tactics If Ally Is Paused
            // Due to the Game Pausing Or Control Pause Mode
            // Is Active
            if (myEventHandler.bAllyIsPaused) return;

            if (bStopUpdatingBattleBehavior)
            {
                myEventHandler.CallEventStopTargettingEnemy();
                myEventHandler.CallEventFinishedMoving();
                return;
            }

            if (allyMember.bIsCarryingMeleeWeapon)
            {
                //Melee Behavior
                if (IsTargetInMeleeRange(currentTargettedEnemy.gameObject))
                {
                    if(bIsMeleeing == false)
                    {
                        StartMeleeAttackBehavior();
                    }
                }
                else
                {
                    if (bIsMeleeing == true)
                    {
                        StopMeleeAttackBehavior();
                    }
                    
                    myEventHandler.CallEventAIMove(currentTargettedEnemy.transform.position);
                }
            }
            else
            {
                //Shooting Behavior
                RaycastHit _hit;
                if (hasLOSWithinRange(currentTargettedEnemy, out _hit))
                {
                    if (bIsShooting == false)
                    {
                        StartShootingBehavior();
                    }
                }
                else
                {
                    if (bIsShooting == true)
                        StopShootingBehavior();

                    if (bIsMoving == false)
                    {
                        myEventHandler.CallEventAIMove(currentTargettedEnemy.transform.position);
                    }
                }
            }
        }

        protected virtual void StartBattleBehavior()
        {
            InvokeRepeating("UpdateBattleBehavior", 0f, 0.2f);
        }

        protected virtual void StopBattleBehavior()
        {
            CancelInvoke("UpdateBattleBehavior");
            StopShootingBehavior();
        }

        protected virtual void StartShootingBehavior()
        {
            myEventHandler.CallEventToggleIsShooting(true);
            InvokeRepeating("MakeFireRequest", 0.0f, GetAttackRate());
        }

        protected virtual void StopShootingBehavior()
        {
            myEventHandler.CallEventToggleIsShooting(false);
            CancelInvoke("MakeFireRequest");
        }

        protected virtual void StartMeleeAttackBehavior()
        {
            myEventHandler.CallEventToggleIsMeleeing(true);
            InvokeRepeating("MakeMeleeAttackRequest", 0.0f, GetAttackRate());
        }

        protected virtual void StopMeleeAttackBehavior()
        {
            myEventHandler.CallEventToggleIsMeleeing(false);
            CancelInvoke("MakeMeleeAttackRequest");
        }

        protected virtual void MakeFireRequest()
        {
            if (allyMember.ActiveTimeBarIsFull())
            {
                // Pause Ally Tactics If Ally Is Paused
                // Due to the Game Pausing Or Control Pause Mode
                // Is Active
                if (myEventHandler.bAllyIsPaused) return;

                myEventHandler.CallOnTryUseWeapon();
            }
        }

        protected virtual void MakeMeleeAttackRequest()
        {
            if (allyMember.ActiveTimeBarIsFull())
            {
                // Pause Ally Tactics If Ally Is Paused
                // Due to the Game Pausing Or Control Pause Mode
                // Is Active
                if (myEventHandler.bAllyIsPaused) return;

                myEventHandler.CallOnTryUseWeapon();
            }
        }
        #endregion

        #region Initialization
        protected virtual void SubToEvents()
        {
            myEventHandler.EventCommandAttackEnemy += HandleCommandAttackEnemy;
            myEventHandler.EventStopTargettingEnemy += HandleStopTargetting;
            myEventHandler.EventToggleIsShooting += TogglebIsShooting;
            myEventHandler.EventCommandMove += HandleOnMoveAlly;
            myEventHandler.EventFinishedMoving += HandleOnAIStopMoving;
            myEventHandler.OnWeaponChanged += OnWeaponChanged;
            myEventHandler.InitializeAllyComponents += OnAllyInitComps;
            gamemaster.EventHoldingRightMouseDown += OnEnableCameraMovement;
        }

        protected virtual void UnSubFromEvents()
        {
            myEventHandler.EventCommandAttackEnemy -= HandleCommandAttackEnemy;
            myEventHandler.EventStopTargettingEnemy -= HandleStopTargetting;
            myEventHandler.EventToggleIsShooting -= TogglebIsShooting;
            myEventHandler.EventCommandMove -= HandleOnMoveAlly;
            myEventHandler.EventFinishedMoving -= HandleOnAIStopMoving;
            myEventHandler.OnWeaponChanged -= OnWeaponChanged;
            myEventHandler.InitializeAllyComponents -= OnAllyInitComps;
            gamemaster.EventHoldingRightMouseDown -= OnEnableCameraMovement;
        }

        protected virtual void StartServices()
        {

        }

        protected virtual void CancelServices()
        {
            CancelInvoke();
        }
        #endregion

    }
}