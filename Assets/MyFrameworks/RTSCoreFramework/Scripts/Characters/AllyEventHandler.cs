using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RTSCoreFramework
{
    #region RTSAllyComponentSpecificFields
    [System.Serializable]
    public class RTSAllyComponentSpecificFields
    {
        [Header("Base Ally Fields")]
        public ECharacterType CharacterType;
        public RTSGameMode.EFactions AllyFaction;
        public RTSGameMode.ECommanders GeneralCommander;
        
        [Header("Child Prefab Info")]
        //HealthBar
        public bool bBuildEnemyHealthBar = false;

        [HideInInspector]
        public Transform LOSChildObjectTransform = null;
        //I'll Try Setting These Images When I instantiate the prefab
        [HideInInspector]
        public Image EnemyHealthBarImage = null;
        [HideInInspector]
        public Image EnemyActiveBarImage = null;

        /// <summary>
        /// Use For Instance Functionality - Turning Light on and off.
        /// </summary>
        [HideInInspector]
        public GameObject AllyIndicatorSpotlightInstance = null;
    }
    #endregion

    #region RTSAllyComponentsAllCharacterFields
    [System.Serializable]
    public class RTSAllyComponentsAllCharacterFields
    {
        [Header("AI Fields")]
        public float sightRange = 0f;
        public float followDistance = 0f;

        [Header("Special Abilities")]
        public AbilityConfig[] specialAbilitiesArray;
        public AudioClip outOfEnergySoundClip;

        [Header("Tactics")]
        public int tacticsExecutionsPerSecond = 5;

        [Header("Child Prefab Info")]
        //LOSObject
        public bool bBuildLOSChildObject = true;
        public Vector3 LOSChildObjectPosition = Vector3.zero;
        public Vector3 LOSChildObjectRotation = Vector3.zero;
        //HealthBar
        public GameObject EnemyHealthBarPrefab = null;
        public Vector3 EnemyHealthBarPosition = Vector3.zero;
        public Vector3 EnemyHealthBarRotation = Vector3.zero;
        public Vector2 EnemyHealthSizeDelta = new Vector2(100, 100);
        public Vector3 EnemyHealthLocalScale = new Vector3(0.07f, 0.07f, 0.07f);
        //Spotlight
        public bool bBuildAllyIndicatorSpotlight = true;
        /// <summary>
        /// Use For Instantiating
        /// </summary>
        public GameObject AllyIndicatorSpotlightPrefab = null;

        public Vector3 AllyIndicatorSpotlightPosition = Vector3.zero;
        public Vector3 AllyIndicatorSpotlightRotation = Vector3.zero;
        public Color AllyHighlightColor = Color.green;
        public Color EnemyHighlightColor = Color.red;
        //Waypoint Renderer
        public Material WaypointRendererMaterial = null;
        //Blood
        public GameObject BloodParticles = null;

    }
    #endregion

    public class AllyEventHandler : MonoBehaviour
    {
        #region InstanceProperties
        RTSStatHandler globalStatHandler
        {
            get
            {
                if (_globalStatHandler == null)
                    _globalStatHandler = RTSStatHandler.thisInstance;

                return _globalStatHandler;
            }
        }
        RTSStatHandler _globalStatHandler = null;
        #endregion

        #region FieldsAndProps
        public bool bIsSprinting { get; protected set; }
        public bool bActiveTimeBarIsRegenerating { get; protected set; }
        public bool bIsTacticsEnabled { get; protected set; }
        //Is moving through nav mesh agent, regardless of
        //whether it's ai or a command
        public bool bIsNavMoving { get { return bIsCommandMoving || bIsAIMoving; } }
        public bool bIsCommandMoving { get; protected set; }
        public bool bIsAIMoving { get; protected set; }
        public bool bIsFreeMoving { get; protected set; }
        public bool bIsAttacking
        {
            get { return bIsCommandAttacking || bIsAiAttacking; }
        }
        public bool bIsCommandAttacking { get; protected set; }
        public bool bIsAiAttacking { get; protected set; }
        public bool bIsAimingToShoot { get; protected set; }
        public bool bIsMeleeingEnemy { get; protected set; }
        public bool bIsUsingAbility { get; protected set; }
        public bool bCanEnableAITactics
        {
            get
            {
                return (bIsCommandMoving ||
                  bIsFreeMoving) == false &&
                  bIsCommandAttacking == false;
            }
        }

        //Ui Target Info
        public bool bAllyIsUiTarget { get; protected set; }
        //Character Weapon Stats
        public EEquipType MyEquippedType { get; protected set; }
        public EEquipType MyUnequippedType
        {
            get
            {
                return MyEquippedType == EEquipType.Primary ?
                  EEquipType.Secondary : EEquipType.Primary;
            }
        }
        public EWeaponType MyEquippedWeaponType
        {
            get { return _MyEquippedWeaponType; }
            set { _MyEquippedWeaponType = value; }
        }
        private EWeaponType _MyEquippedWeaponType = EWeaponType.NoWeaponType;
        public EWeaponType MyUnequippedWeaponType
        {
            get { return _MyUnequippedWeaponType; }
            set { _MyUnequippedWeaponType = value; }
        }
        private EWeaponType _MyUnequippedWeaponType = EWeaponType.NoWeaponType;
        public int PrimaryLoadedAmmoAmount { get; protected set; }
        public int PrimaryUnloadedAmmoAmount { get; protected set; }
        public int SecondaryLoadedAmmoAmount { get; protected set; }
        public int SecondaryUnloadedAmmoAmount { get; protected set; }
        public Sprite MyEquippedWeaponIcon { get; protected set; }
        public Sprite MyUnequippedWeaponIcon { get; protected set; }

        //Pause Functionality
        public virtual bool bAllyIsPaused
        {
            get { return false; }
        }

        protected bool bHasStartedFromDelay = false;

        #endregion

        #region DelegatesAndEvents
        public delegate void GeneralEventHandler();
        public delegate void GeneralOneBoolHandler(bool _enable);
        public event GeneralEventHandler EventAllyDied;
        public event GeneralEventHandler EventSwitchingFromCom;
        public event GeneralEventHandler EventPartySwitching;
        public event GeneralEventHandler EventSetAsCommander;
        public event GeneralEventHandler EventKilledEnemy;
        public event GeneralEventHandler EventStopTargettingEnemy;
        public event GeneralEventHandler EventFinishedMoving;
        public event GeneralEventHandler EventToggleIsSprinting;
        public event GeneralOneBoolHandler EventToggleAllyTactics;
        public event GeneralOneBoolHandler EventTogglebIsFreeMoving;
        public event GeneralOneBoolHandler EventToggleIsShooting;
        public event GeneralOneBoolHandler EventToggleIsMeleeing;
        public event GeneralOneBoolHandler EventToggleIsUsingAbility;
        //Opsive TPC Events
        public event GeneralEventHandler OnSwitchToPrevItem;
        public event GeneralEventHandler OnSwitchToNextItem;
        //Called by Opsive Shooter Script, then another class
        //handles the hitscan firing.
        public event GeneralVector3Handler OnTryHitscanFire;
        //Called by Opsive Shooter Script, then another class
        //handles the attacking.
        public event GeneralEventHandler OnTryMeleeAttack;
        //Tries Using Primary Item, Used By RTSItemHandler
        public event GeneralEventHandler OnTryUseWeapon;
        public event GeneralEventHandler OnTryReload;
        public event GeneralEventHandler OnTryCrouch;
        public event GeneralOneBoolHandler OnTryAim;

        //For Special Abilities
        public delegate void OneSystemTypeArgHandler(System.Type _type);
        public event OneSystemTypeArgHandler OnTrySpecialAbility;

        //For Active Time Bar Functionality
        public delegate void OneRTSActionItemArgHandler(RTSActionItem _actionItem);
        public event GeneralEventHandler OnActiveTimeBarIsFull;
        public event GeneralEventHandler OnActiveTimeBarDepletion;
        public event GeneralEventHandler OnRemoveCommandActionFromQueue;
        public event GeneralEventHandler OnRemoveAIActionFromQueue;
        public event OneRTSActionItemArgHandler OnAddActionItemToQueue;
        public event GeneralOneBoolHandler OnToggleActiveTimeRegeneration;

        //May use delegate in the future
        //public delegate void RtsHitTypeAndRayCastHitHandler(rtsHitType hitType, RaycastHit hit);
        public event GeneralEventHandler OnHoverOver;
        public event GeneralEventHandler OnHoverLeave;

        public delegate void GeneralVector3Handler(Vector3 _point);
        public event GeneralVector3Handler EventCommandMove;

        public delegate void AllyHandler(AllyMember ally);
        public event AllyHandler EventCommandAttackEnemy;
        public event AllyHandler OnUpdateTargettedEnemy;

        public delegate void EEquipTypeHandler(EEquipType _eType);
        public delegate void EWeaponTypeHandler(EEquipType _eType, EWeaponType _weaponType, EWeaponUsage _wUsage, bool _equipped);
        public event EEquipTypeHandler OnEquipTypeChanged;
        public event EWeaponTypeHandler OnWeaponChanged;

        public delegate void TwoIntArgsHandler(int _firstNum, int _secondNum);
        public event TwoIntArgsHandler OnAmmoChanged;
        public event TwoIntArgsHandler OnHealthChanged;
        public event TwoIntArgsHandler OnStaminaChanged;
        public event TwoIntArgsHandler OnActiveTimeChanged;

        public delegate void RTSTakeDamageHandler(int amount, Vector3 position, Vector3 force, AllyMember _instigator, GameObject hitGameObject, Collider hitCollider);
        public event RTSTakeDamageHandler OnAllyTakeDamage;

        public delegate void RTSAllyComponentInitializationHandler(RTSAllyComponentSpecificFields _specificComps, RTSAllyComponentsAllCharacterFields _allAllyComps);
        public event RTSAllyComponentInitializationHandler InitializeAllyComponents;
        #endregion

        #region UnityMessages
        protected virtual void Awake()
        {
            bIsSprinting = true;
            bIsTacticsEnabled = false;
            bIsAimingToShoot = false;
            bIsFreeMoving = false;
            bIsCommandAttacking = false;
            bIsAiAttacking = false;
            bIsMeleeingEnemy = false;
            bIsUsingAbility = false;
            bActiveTimeBarIsRegenerating = false;
        }

        protected virtual void Start()
        {
            Invoke("OnDelayStart", 0.5f);
            SubToEvents();
        }

        protected virtual void OnDelayStart()
        {
            bHasStartedFromDelay = true;
            SubToEventsLater();
        }

        protected virtual void OnDisable()
        {
            UnsubFromEvents();
        }
        #endregion

        #region Initialization
        protected virtual void SubToEvents()
        {

        }

        protected virtual void SubToEventsLater()
        {

        }

        protected virtual void UnsubFromEvents()
        {

        }
        #endregion

        #region EventCalls
        public virtual void CallEventAllyDied()
        {
            if (EventAllyDied != null)
            {
                EventAllyDied();
                this.enabled = false;
            }
        }

        public virtual void CallEventToggleIsShooting(bool _enable)
        {
            bIsAimingToShoot = _enable;
            if (EventToggleIsShooting != null)
            {
                EventToggleIsShooting(_enable);
            }
            if (bIsMeleeingEnemy)
            {
                CallEventToggleIsMeleeing(false);
            }
        }

        public virtual void CallEventToggleIsMeleeing(bool _enable)
        {
            bIsMeleeingEnemy = _enable;
            if(EventToggleIsMeleeing != null)
            {
                EventToggleIsMeleeing(_enable);
            }
            if (bIsAimingToShoot)
            {
                CallEventToggleIsShooting(false);
            }
        }

        public virtual void CallEventToggleIsUsingAbility(bool _enable)
        {
            bIsUsingAbility = _enable;
            if (EventToggleIsUsingAbility != null) EventToggleIsUsingAbility(_enable);
        }

        public virtual void CallEventToggleIsSprinting()
        {
            bIsSprinting = !bIsSprinting;
            if (EventToggleIsSprinting != null) EventToggleIsSprinting();
        }

        public virtual void CallEventFinishedMoving()
        {
            bIsCommandMoving = false;
            bIsAIMoving = false;
            if (EventFinishedMoving != null) EventFinishedMoving();
        }

        public virtual void CallOnSwitchToPrevItem()
        {
            if (OnSwitchToPrevItem != null) OnSwitchToPrevItem();
        }

        public virtual void CallOnSwitchToNextItem()
        {
            if (OnSwitchToNextItem != null) OnSwitchToNextItem();
        }
        /// <summary>
        /// Need to changes string ref in Shootable Weapon script
        /// if I change the name of this method.
        /// </summary>
        public virtual void CallOnTryHitscanFire(Vector3 _force)
        {
            CallOnActiveTimeBarDepletion();
            if (OnTryHitscanFire != null) OnTryHitscanFire(_force);
        }
        /// <summary>
        /// Need to changes string ref in Melee Weapon script
        /// if I change the name of this method.
        /// </summary>
        public virtual void CallOnTryMeleeAttack()
        {
            CallOnActiveTimeBarDepletion();
            if (OnTryMeleeAttack != null) OnTryMeleeAttack();
        }

        /// <summary>
        /// Called To Fire The TPC Weapon, Doesn't Do Damage Automatically
        /// </summary>
        public virtual void CallOnTryUseWeapon()
        {
            if (OnTryUseWeapon != null) OnTryUseWeapon();
        }

        public virtual void CallOnTryReload()
        {
            if (OnTryReload != null) OnTryReload();
        }

        public virtual void CallOnTryCrouch()
        {
            if (OnTryCrouch != null) OnTryCrouch();
        }

        public virtual void CallOnTryAim(bool _enable)
        {
            if (OnTryAim != null) OnTryAim(_enable);
        }

        public virtual void CallOnTrySpecialAbility(System.Type _type)
        {
            if (OnTrySpecialAbility != null) OnTrySpecialAbility(_type);
        }

        public virtual void CallOnActiveTimeBarIsFull()
        {
            if (OnActiveTimeBarIsFull != null) OnActiveTimeBarIsFull();
        }

        public virtual void CallOnActiveTimeBarDepletion()
        {
            if (OnActiveTimeBarDepletion != null) OnActiveTimeBarDepletion();
        }

        public virtual void CallOnRemoveCommandActionFromQueue()
        {
            if (OnRemoveCommandActionFromQueue != null) OnRemoveCommandActionFromQueue();
        }

        public virtual void CallOnRemoveAIActionFromQueue()
        {
            if (OnRemoveAIActionFromQueue != null) OnRemoveAIActionFromQueue();
        }

        public virtual void CallOnAddActionItemToQueue(RTSActionItem _actionItem)
        {
            if (OnAddActionItemToQueue != null) OnAddActionItemToQueue(_actionItem);
        }

        public virtual void CallOnToggleActiveTimeRegeneration(bool _enable)
        {
            if(_enable != bActiveTimeBarIsRegenerating)
            {
                bActiveTimeBarIsRegenerating = _enable;
                if (OnToggleActiveTimeRegeneration != null) OnToggleActiveTimeRegeneration(_enable);
            }
        }

        public virtual void CallEventSwitchingFromCom()
        {
            if (bIsFreeMoving) CallEventTogglebIsFreeMoving(false);
            if (EventSwitchingFromCom != null) EventSwitchingFromCom();
        }

        /// <summary>
        /// Called After AllyInCommand Has Been Set By the PartyManager
        /// </summary>
        public virtual void CallEventPartySwitching()
        {
            if (EventPartySwitching != null) EventPartySwitching();
        }

        public virtual void CallEventSetAsCommander()
        {
            CallEventFinishedMoving();
            if (EventSetAsCommander != null) EventSetAsCommander();
        }

        public virtual void CallEventKilledEnemy()
        {
            if (EventKilledEnemy != null) EventKilledEnemy();
        }

        public virtual void CallEventOnHoverOver(rtsHitType hitType, RaycastHit hit)
        {
            CallEventOnHoverOver();
        }

        public virtual void CallEventOnHoverLeave(rtsHitType hitType, RaycastHit hit)
        {
            CallEventOnHoverLeave();
        }

        public virtual void CallEventOnHoverOver()
        {
            if (OnHoverOver != null)
            {
                OnHoverOver();
            }
        }

        public virtual void CallEventOnHoverLeave()
        {
            if (OnHoverLeave != null)
            {
                OnHoverLeave();
            }
        }

        public virtual void CallEventCommandMove(rtsHitType hitType, RaycastHit hit)
        {
            bIsAimingToShoot = bIsCommandAttacking = bIsAiAttacking = false;
            bIsCommandMoving = true;
            bIsAIMoving = false;
            CallEventCommandMove(hit.point);
        }

        public virtual void CallEventAIMove(Vector3 _point)
        {
            bIsAimingToShoot = false;
            bIsAIMoving = true;
            bIsCommandMoving = false;
            CallEventCommandMove(_point);
        }

        protected virtual void CallEventCommandMove(Vector3 _point)
        {
            if (EventCommandMove != null) EventCommandMove(_point);
        }

        public virtual void CallEventPlayerCommandAttackEnemy(AllyMember ally)
        {
            bIsAIMoving = bIsCommandMoving = false;
            bIsCommandAttacking = true;
            bIsAiAttacking = false;
            CallEventCommandAttackEnemy(ally);
        }

        public virtual void CallEventAICommandAttackEnemy(AllyMember ally)
        {
            bIsAIMoving = bIsCommandMoving = false;
            bIsAiAttacking = true;
            bIsCommandAttacking = false;
            CallEventCommandAttackEnemy(ally);
        }

        protected virtual void CallEventCommandAttackEnemy(AllyMember ally)
        {
            if (EventCommandAttackEnemy != null)
            {
                EventCommandAttackEnemy(ally);
            }
        }

        public virtual void CallOnUpdateTargettedEnemy(AllyMember _ally)
        {
            if (OnUpdateTargettedEnemy != null) OnUpdateTargettedEnemy(_ally);
        }

        public virtual void CallEventStopTargettingEnemy()
        {
            bIsCommandAttacking = bIsAiAttacking = bIsAimingToShoot = false;
            if (EventStopTargettingEnemy != null) EventStopTargettingEnemy();
            CallEventToggleIsShooting(false);
            CallEventToggleIsMeleeing(false);
            CallOnTryAim(false);
        }

        public virtual void CallEventTogglebIsFreeMoving(bool _enable)
        {
            bIsFreeMoving = _enable;
            if (EventTogglebIsFreeMoving != null) EventTogglebIsFreeMoving(_enable);
        }

        /// <summary>
        /// Event handler controls bIsTacticsEnabled, makes code more centralized.
        /// Now is called inside TacticsController, Rather than being Handled by Controller
        /// </summary>
        /// <param name="_enable"></param>
        public virtual void CallEventToggleAllyTactics(bool _enable)
        {
            bIsTacticsEnabled = _enable;
            if (EventToggleAllyTactics != null) EventToggleAllyTactics(_enable);
        }

        public virtual void CallOnEquipTypeChanged(EEquipType _eType)
        {
            MyEquippedType = _eType;
            if (OnEquipTypeChanged != null) OnEquipTypeChanged(_eType);
        }

        public virtual void CallToggleEquippedWeapon()
        {
            var _toggleType = MyEquippedType == EEquipType.Primary ?
                EEquipType.Secondary : EEquipType.Primary;
            CallOnEquipTypeChanged(_toggleType);
        }

        public virtual void CallOnWeaponChanged(EEquipType _eType, EWeaponType _weaponType, EWeaponUsage _wUsage, bool _equipped)
        {
            //If MyEquippedWeaponType Hasn't Been Set, Do Not Update MyUnequippedWeaponType
            if (MyEquippedWeaponType != EWeaponType.NoWeaponType)
            {
                MyUnequippedWeaponType = MyEquippedWeaponType;
            }
            MyEquippedWeaponType = _weaponType;
            if (globalStatHandler &&
                globalStatHandler.WeaponStatDictionary.ContainsKey(MyEquippedWeaponType) &&
                globalStatHandler.WeaponStatDictionary.ContainsKey(MyUnequippedWeaponType))
            {
                MyEquippedWeaponIcon =
                globalStatHandler.WeaponStatDictionary
                [MyEquippedWeaponType].WeaponIcon;
                if (MyUnequippedWeaponType != EWeaponType.NoWeaponType)
                {
                    MyUnequippedWeaponIcon =
                    globalStatHandler.WeaponStatDictionary
                    [MyUnequippedWeaponType].WeaponIcon;
                }
            }

            if (OnWeaponChanged != null)
            {
                OnWeaponChanged(_eType, _weaponType, _wUsage, _equipped);
            }
        }

        protected virtual void CallOnAmmoChanged(int _loaded, int _unloaded)
        {
            if (MyEquippedType == EEquipType.Primary)
            {
                PrimaryLoadedAmmoAmount = _loaded;
                PrimaryUnloadedAmmoAmount = _unloaded;
            }
            else
            {
                SecondaryLoadedAmmoAmount = _loaded;
                SecondaryUnloadedAmmoAmount = _unloaded;
            }
            if (OnAmmoChanged != null) OnAmmoChanged(_loaded, _unloaded);
        }

        public virtual void CallOnHealthChanged(int _current, int _max)
        {
            if (OnHealthChanged != null) OnHealthChanged(_current, _max);
        }

        public virtual void CallOnStaminaChanged(int _current, int _max)
        {
            if (OnStaminaChanged != null) OnStaminaChanged(_current, _max);
        }

        public virtual void CallOnActiveTimeChanged(int _current, int _max)
        {
            if (OnActiveTimeChanged != null) OnActiveTimeChanged(_current, _max);
        }

        public virtual void CallOnAllyTakeDamage(int amount, Vector3 position, Vector3 force, AllyMember _instigator, GameObject hitGameObject, Collider hitCollider)
        {
            if (OnAllyTakeDamage != null)
                OnAllyTakeDamage(amount, position, force, _instigator, hitGameObject, hitCollider);
        }

        public virtual void CallInitializeAllyComponents(RTSAllyComponentSpecificFields _specificComps, RTSAllyComponentsAllCharacterFields _allAllyComps)
        {
            if (InitializeAllyComponents != null) InitializeAllyComponents(_specificComps, _allAllyComps);
        }
        #endregion

        #region PublicMethods
        /// <summary>
        /// Used Primarily to update unequipped ammo count, called
        /// from allyStatsController.
        /// </summary>
        /// <param name="_eType"></param>
        /// <param name="_loaded"></param>
        /// <param name="_unloaded"></param>
        public void UpdateWeaponAmmoCount(EEquipType _eType, int _loaded, int _unloaded)
        {
            if (_eType == EEquipType.Primary)
            {
                PrimaryLoadedAmmoAmount = _loaded;
                PrimaryUnloadedAmmoAmount = _unloaded;
            }
            else
            {
                SecondaryLoadedAmmoAmount = _loaded;
                SecondaryUnloadedAmmoAmount = _unloaded;
            }
            CallOnAmmoChanged(_loaded, _unloaded);
        }
        /// <summary>
        /// Used to update a few unequipped weapon stats
        /// from allystatcontroller, on start
        /// </summary>
        /// <param name="_weaponStats"></param>
        public void UpdateUnequippedWeaponStats(WeaponStats _weaponStats)
        {
            MyUnequippedWeaponType = _weaponStats.WeaponType;
            MyUnequippedWeaponIcon = _weaponStats.WeaponIcon;
        }

        public void SetAllyIsUiTarget(bool _isTarget)
        {
            bAllyIsUiTarget = _isTarget;
        }
        #endregion

    }
}