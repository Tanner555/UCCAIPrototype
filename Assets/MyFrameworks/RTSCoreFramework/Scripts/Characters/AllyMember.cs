using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTSCoreFramework
{
    public class AllyMember : MonoBehaviour
    {
        #region Fields
        //Inspector Set Variables
        [Header("Faction and General Settings")]
        public RTSGameMode.EFactions AllyFaction;
        public RTSGameMode.ECommanders GeneralCommander;
        [Header("Debug Menu")]
        public bool Debug_InfiniteHealth = false;
        public bool Debug_DoNotShoot = false;

        [Header("Camera Follow Transforms")]
        [SerializeField]
        protected Transform chestTransform;
        [SerializeField]
        protected Transform headTransform;
        [SerializeField]
        [Tooltip("Used as a start point for raycasting")]
        protected Transform LOSTransform;

        //Gun Properties, Can Delete in the Future
        protected float lowAmmoThreshold = 14.0f;
        protected float firerate = 0.3f;

        //Other private variables
        protected bool ExecutingShootingBehavior;
        protected bool wantsFreedomToMove;
        protected float freeMoveThreshold;
        protected float DefaultShootDelay;

        //Active Time Bar
        protected bool bActiveTimeBarFullBeenCalled = false;
        protected int ActiveTimeBarRefillRate = 5;
        #endregion

        #region Properties
        public RTSGameMode gamemode { get { return RTSGameMode.thisInstance; } }
        public RTSGameMaster gamemaster { get { return RTSGameMaster.thisInstance; } }
        public AllyMember DamageInstigator { get; protected set; }
        //Faction Properties
        public PartyManager partyManager
        {
            get
            {
                if (_partyManager == null)
                    _partyManager = FindPartyManager();

                return _partyManager;
            }
        }
        private PartyManager _partyManager = null;
        public int FactionPlayerCount { get { return gamemode.GetAllyFactionPlayerCount((AllyMember)this); } }
        public int GeneralPlayerCount { get { return gamemode.GetAllyGeneralPlayerCount((AllyMember)this); } }
        //Camera Follow Transforms
        public Transform ChestTransform
        {
            get
            {
                if (chestTransform != null) return chestTransform;
                //Try To Find Spine1 Object On UMA Character
                Transform _chest = myAnimator.GetBoneTransform(HumanBodyBones.Chest);
                if (_chest != null)
                {
                    chestTransform = _chest;
                    return chestTransform;
                }
                //Return Empty If No Spine is Found
                return transform;
            }
        }
        public Transform HeadTransform
        {
            get
            {
                if (headTransform != null) return headTransform;
                //Try To Find Head Object On UMA Character
                Transform _head = myAnimator.GetBoneTransform(HumanBodyBones.Head);
                if (_head != null)
                {
                    headTransform = _head;
                    return headTransform;
                }
                //Return Empty If No Spine is Found
                return transform;
            }
        }
        public Transform MyLOSTransform
        {
            get
            {
                if (LOSTransform != null) return LOSTransform;
                return this.transform;
            }
        }

        public virtual AllyMember enemyTarget
        {
            get { return aiController.currentTargettedEnemy; }
        }

        public int PartyKills
        {
            get { return partyManager.PartyKills; }
            set { partyManager.PartyKills = value; }
        }
        public int PartyPoints
        {
            get { return partyManager.PartyPoints; }
            set { partyManager.PartyPoints = value; }
        }
        public int PartyDeaths
        {
            get { return partyManager.PartyDeaths; }
            set { partyManager.PartyDeaths = value; }
        }

        //Health Properties
        public virtual int AllyHealth
        {
            get { return _allyHealth; }
            protected set { _allyHealth = value; }
        }
        private int _allyHealth = 100;
        public virtual int AllyMaxHealth
        {
            get { return _allyMaxHealth; }
        }
        private int _allyMaxHealth = 100;

        public virtual int AllyMinHealth
        {
            get { return _allyMinHealth; }
        }
        private int _allyMinHealth = 0;

        public virtual float HealthAsPercentage
        {
            get { return ((float)AllyHealth / (float)AllyMaxHealth); }
        }

        public virtual int AllyStamina
        {
            get { return _AllyStamina; }
            protected set { _AllyStamina = value; }
        }
        private int _AllyStamina = 0;

        public virtual int AllyMaxStamina
        {
            get { return _AllyMaxStamina; }
        }
        private int _AllyMaxStamina = 0;

        public virtual int AllyMinStamina
        {
            get { return _AllyMinStamina; }
        }
        private int _AllyMinStamina = 0;

        public virtual int AllyActiveTimeBar
        {
            get { return _AllyActiveTimeBar; }
            set
            {
                _AllyActiveTimeBar = value;
                allyEventHandler.CallOnActiveTimeChanged(_AllyActiveTimeBar, AllyMaxActiveTimeBar);
            }
        }
        private int _AllyActiveTimeBar = 0;

        public virtual int AllyMaxActiveTimeBar
        {
            get { return 100; }
        }

        public virtual int AllyMinActiveTimeBar
        {
            get { return 0; }
        }

        public virtual bool IsAlive
        {
            get { return AllyHealth > AllyMinHealth; }
        }

        //Ammo Properties
        public virtual int CurrentEquipedAmmo
        {
            get { return 0; }
        }

        //Other Weapon Properties
        public virtual bool bIsCarryingMeleeWeapon
        {
            get
            {
                return false;
            }
        }

        public virtual float WeaponAttackRate
        {
            get { return 0f; }
        }

        public virtual float MaxMeleeAttackDistance
        {
            get { return 0f; }
        }

        //Character Stat Properties
        public virtual string CharacterName
        {
            get { return "MyName"; }
        }

        public virtual ECharacterType CharacterType
        {
            get { return ECharacterType.NoCharacterType; }
        }

        public virtual Sprite CharacterPortrait
        {
            get { return null; }
        }

        protected Dictionary<AbilityConfig, AbilityBehaviour> AbilityDictionary = new Dictionary<AbilityConfig, AbilityBehaviour>();

        //AI Props
        public float FollowDistance { get { return aiController.followDistance; } }
        #endregion

        #region PlayerComponents
        protected Rigidbody myRigidbody
        {
            get
            {
                if (_myRigidbody == null)
                    _myRigidbody = GetComponent<Rigidbody>();

                return _myRigidbody;
            }
        }
        Rigidbody _myRigidbody = null;
        public AllyEventHandler allyEventHandler
        {
            get
            {
                if (_allyEventHandler == null)
                    _allyEventHandler = GetComponent<AllyEventHandler>();

                return _allyEventHandler;
            }
        }
        AllyEventHandler _allyEventHandler = null;
        public AllyAIController aiController
        {
            get
            {
                if (_aiController == null)
                    _aiController = GetComponent<AllyAIController>();

                return _aiController;
            }
        }
        AllyAIController _aiController = null;

        protected AllyStatController allyStatController
        {
            get
            {
                if (_allyStatController == null)
                    _allyStatController = GetComponent<AllyStatController>();

                return _allyStatController;
            }
        }
        AllyStatController _allyStatController = null;

        protected Animator myAnimator
        {
            get
            {
                if (_myAnimator == null)
                    _myAnimator = GetComponent<Animator>();

                return _myAnimator;
            }
        }
        Animator _myAnimator = null;
        #endregion

        #region BooleanProperties
        protected virtual bool AllComponentsAreValid
        {
            get
            {
                return allyEventHandler && aiController &&
                  allyStatController;
            }
        }

        public bool bIsCurrentPlayer { get; protected set; } = false;
        public bool bIsGeneralInCommand { get { return partyManager ? partyManager.AllyIsGeneralInCommand(this) : false; } }
        public bool bIsInGeneralCommanderParty { get { return partyManager.bIsCurrentPlayerCommander; } }
        //Ui Target Info
        public bool bAllyIsUiTarget { get { return allyEventHandler.bAllyIsUiTarget; } }
        //Other Easy Getter Properties
        public bool bIsAttacking { get { return allyEventHandler.bIsAttacking; } }
        public bool bIsAIAttacking { get { return allyEventHandler.bIsAiAttacking; } }
        public bool bIsCommandAttacking { get { return allyEventHandler.bIsCommandAttacking; } }
        public bool bIsNavMoving { get { return allyEventHandler.bIsNavMoving; } }
        public bool bIsFreeMoving { get { return allyEventHandler.bIsFreeMoving; } }
        public bool bIsCommandMoving { get { return allyEventHandler.bIsCommandMoving; } }
        public bool bIsAIMoving { get { return allyEventHandler.bIsAIMoving; } }
        public bool bIsUsingAbility { get { return allyEventHandler.bIsUsingAbility; } }
        public bool bActiveTimeBarIsRegenerating => allyEventHandler.bActiveTimeBarIsRegenerating;
        #endregion

        #region UnityMessages
        protected virtual void OnEnable()
        {
            //SetInitialReferences();
            InitializeAllyValues();
            SubToEvents();
        }

        protected virtual void OnDisable()
        {
            UnSubFromEvents();
            StopServices();
        }

        // Use this for initialization
        protected virtual void Start()
        {

        }

        protected virtual void OnDelayStart()
        {
            AllyActiveTimeBar = 0;
        }
        #endregion

        #region Handlers
        protected virtual void InitializeAlly(RTSAllyComponentSpecificFields _specific, RTSAllyComponentsAllCharacterFields _allFields)
        {
            AllyFaction = _specific.AllyFaction;
            GeneralCommander = _specific.GeneralCommander;
            if (_allFields.bBuildLOSChildObject)
            {
                LOSTransform = _specific.LOSChildObjectTransform;
            }

            if (gamemode == null)
            {
                Debug.LogError("No gamemode on ai player!");
            }

            //Add Ally to PartyMembers Rather than Finding them
            //To Make Spawning Easier
            if (partyManager != null) partyManager.AddPartyMember(this);
            //Create Overrideable Late Start to Accommodate 
            //Assets Having Long StartUp 
            Invoke("OnDelayStart", 0.5f);
            StartServices();
        }

        protected virtual void OnActiveTimeBarDepletion()
        {
            //Reset Active Time Bar
            AllyActiveTimeBar = AllyMinActiveTimeBar;
            bActiveTimeBarFullBeenCalled = false;
        }

        protected virtual void OnToggleActiveTimeRegeneration(bool _enable)
        {
            if (_enable)
            {
                if (IsInvoking("SE_UpdateActiveTimeBar") == false)
                {
                    InvokeRepeating("SE_UpdateActiveTimeBar", 0.5f, 0.2f);
                }
            }
            else
            {
                if (IsInvoking("SE_UpdateActiveTimeBar"))
                {
                    CancelInvoke("SE_UpdateActiveTimeBar");
                }
                allyEventHandler.CallOnActiveTimeBarDepletion();
            }
        }

        /// <summary>
        /// Called Before AllyInCommand has been set
        /// </summary>
        /// <param name="_party"></param>
        /// <param name="_toSet"></param>
        /// <param name="_current"></param>
        protected virtual void HandleOnAllySwitch(PartyManager _party, AllyMember _toSet, AllyMember _current)
        {
            if (_party != partyManager) return;

            bIsCurrentPlayer = _toSet == this && _toSet.bIsInGeneralCommanderParty;
        }

        protected virtual void OnTryHitscanFire(Vector3 _force)
        {
            RaycastHit _hit;
            var _target = enemyTarget;
            bool _validShot = ChestTransform != null &&
                _target != null;
            if (_validShot && Physics.Linecast(MyLOSTransform.position, _target.ChestTransform.position, out _hit))
            {
                var _root = _hit.transform.root;
                //If Hit Self
                if (_root == transform)
                {
                    Debug.Log(CharacterName.ToString()
                        + " is shooting himself.");
                }
                bool _isEnemy = _root.tag == gamemode.AllyTag;
                if (_isEnemy)
                {
                    _target.allyEventHandler.CallOnAllyTakeDamage(
                        GetDamageRate(), _hit.point, _force, this, _hit.transform.gameObject, _hit.collider);
                }
            }
        }

        protected virtual void OnTryMeleeAttack()
        {
            float _delay = Mathf.Max(0.1f, WeaponAttackRate - 0.5f);
            Invoke("OnTryMeleeAttackDelay", _delay);
        }

        protected virtual void OnTryMeleeAttackDelay()
        {
            RaycastHit _hit;
            var _target = enemyTarget;
            bool _validShot = ChestTransform != null &&
                _target != null;
            if (_validShot && Physics.Linecast(MyLOSTransform.position, _target.ChestTransform.position, out _hit))
            {
                var _root = _hit.transform.root;
                //If Hit Self
                if (_root == transform)
                {
                    Debug.Log(CharacterName.ToString()
                        + " is shooting himself.");
                }
                bool _isEnemy = _root.tag == gamemode.AllyTag;
                if (_isEnemy)
                {
                    _target.allyEventHandler.CallOnAllyTakeDamage(
                        GetDamageRate(), _hit.point, Vector3.zero, this, _hit.transform.gameObject, _hit.collider);
                }
            }
        }

        public virtual void AllyTakeDamage(int amount, AllyMember _instigator)
        {
            SetDamageInstigator(_instigator);
            if (IsAlive == false) return;
            if (AllyHealth > AllyMinHealth)
            {
                AllyHealth = Mathf.Max(AllyMinHealth, AllyHealth - amount);
            }
            if (IsAlive == false)
            {
                allyEventHandler.CallEventAllyDied();
            }
        }

        public virtual void AllyTakeDamage(int amount, Vector3 position, Vector3 force, AllyMember _instigator, GameObject hitGameObject, Collider hitCollider)
        {
            SetDamageInstigator(_instigator);
            if (IsAlive == false) return;
            if (AllyHealth > AllyMinHealth)
            {
                AllyHealth = Mathf.Max(AllyMinHealth, AllyHealth - amount);
            }
            // Apply a force to the hit rigidbody if the force is greater than 0.
            if (myRigidbody != null && !myRigidbody.isKinematic && force.sqrMagnitude > 0)
            {
                myRigidbody.AddForceAtPosition(force, position);
            }

            if (IsAlive == false)
            {
                allyEventHandler.CallEventAllyDied();
            }
        }

        public virtual void AllyHeal(int amount)
        {
            if (IsAlive == false) return;
            if (AllyHealth < AllyMaxHealth)
            {
                AllyHealth = Mathf.Min(AllyMaxHealth, AllyHealth + amount);
            }
        }

        public virtual void AllyHeal(int amount, AllyMember _healer)
        {
            AllyHeal(amount);
        }

        public virtual void AllyRegainStamina(int amount)
        {
            if (IsAlive == false) return;
            if (AllyStamina < AllyMaxStamina)
            {
                AllyStamina = Mathf.Min(AllyMaxStamina, AllyStamina + amount);
            }
        }

        public virtual void AllyDrainStamina(int amount)
        {
            if (IsAlive == false) return;
            if (AllyStamina > AllyMinStamina)
            {
                AllyStamina = Mathf.Max(AllyMinStamina, AllyStamina - amount);
            }
        }

        protected virtual void SetDamageInstigator(AllyMember _instigator)
        {
            if (_instigator != null && _instigator != DamageInstigator)
            {
                DamageInstigator = _instigator;
            }
        }
        /// <summary>
        /// Called After AllyInCommand has been set
        /// </summary>
        protected virtual void OnPartySwitch()
        {
            //Switch Tags Depending on whether isCurrentPlayer
            if (bIsCurrentPlayer)
            {
                gameObject.layer = gamemode.SingleCurrentPlayerLayer;
            }
            else
            {
                gameObject.layer = gamemode.SingleAllyLayer;
            }
        }

        public virtual void AllyOnDeath()
        {
            //if gamemode, find allies and exclude this ally
            if (gamemode != null && partyManager != null)
            {
                AllyMember _firstAlly = partyManager.FindPartyMembers(true, this);
                if (_firstAlly != null)
                {
                    //Fixes Enemy PartyManager not setting AllyInCommand
                    if (bIsInGeneralCommanderParty)
                    {
                        //Only Switch Player If Ally Killed is the Current Player
                        if (bIsCurrentPlayer)
                            partyManager.SetAllyInCommand(_firstAlly);
                    }
                    else
                    {
                        partyManager.SetAllyInCommand(_firstAlly);
                    }
                }
                else
                {
                    partyManager.CallEventNoPartyMembers(partyManager, this, true);
                }
                //Add to death count
                PartyDeaths += 1;

                gamemode.ProcessAllyDeath(this);
                Invoke("DestroyAlly", 0.1f);
            }
            else
            {
                Debug.LogError(@"Could not kill allymember because 
                there is no partymember or gamemode");

            }
        }

        private void OnEquippedWeaponAmmoChanged(int _loaded, int _unloaded)
        {

        }

        private void DestroyAlly() { Destroy(this); }
        #endregion

        #region Getters - Finders
        public bool ActiveTimeBarIsFull()
        {
            return AllyActiveTimeBar >= AllyMaxActiveTimeBar;
        }

        public bool CanUseAbility(System.Type _type)
        {
            if (AllyStamina <= AllyMinStamina) return false;
            AbilityConfig _config = GetAbilityConfig(_type);
            return _config != null &&
                AllyStamina >= _config.GetEnergyCost() &&
                AbilityDictionary[_config].CanUseAbility();
        }

        public AbilityConfig GetAbilityConfig(System.Type _type)
        {
            foreach (var _config in AbilityDictionary.Keys)
            {
                if (_config.GetType().Equals(_type))
                {
                    return _config;
                }
            }
            return null;
        }

        public PartyManager FindPartyManager()
        {
            PartyManager _foundPMan = null;
            foreach (var pManager in GameObject.FindObjectsOfType<PartyManager>())
            {
                if (pManager.GeneralCommander == GeneralCommander)
                    _foundPMan = pManager;
            }
            return _foundPMan;
        }

        public bool IsEnemyFor(AllyMember player)
        {
            return player.AllyFaction != AllyFaction;
        }

        public virtual int GetDamageRate()
        {
            return 1;
        }

        public virtual bool isSurfaceWalkable(RaycastHit hit)
        {
            return aiController.isSurfaceWalkable(hit);
        }

        public virtual bool isSurfaceWalkable(Vector3 _point)
        {
            return aiController.isSurfaceWalkable(_point);
        }
        #endregion

        #region Setters - Updaters
        public void UpdateAbilityDictionary(Dictionary<AbilityConfig, AbilityBehaviour> _abilityDic)
        {
            AbilityDictionary.Clear();
            AbilityDictionary = _abilityDic;
        }
        #endregion

        #region Services
        protected virtual void SE_UpdateActiveTimeBar()
        {
            if (AllyActiveTimeBar < AllyMaxActiveTimeBar)
            {
                AllyActiveTimeBar = Mathf.Min(AllyActiveTimeBar + ActiveTimeBarRefillRate, AllyMaxActiveTimeBar);
            }
            else if(bActiveTimeBarFullBeenCalled == false)
            {
                bActiveTimeBarFullBeenCalled = true;
                //Reached Max and Haven't Called Event
                allyEventHandler.CallOnActiveTimeBarIsFull();
            }
        }
        #endregion

        #region Initialization
        protected virtual void SetInitialReferences()
        {
            //allyEventHandler = GetComponent<AllyEventHandler>();
            //aiController = GetComponent<AllyAIController>();
            //allyStatController = GetComponent<AllyStatController>();

            if (partyManager == null)
                Debug.LogError("No partymanager on allymember!");
            if (allyEventHandler == null)
                Debug.LogError("No eventHandler on allymember!");
            if (aiController == null)
                Debug.LogError("No aiController on allymember!");
            if (allyStatController == null)
                Debug.LogError("No allyStatController on allymember!");

            if (AllyFaction == RTSGameMode.EFactions.Faction_Default)
            {
                AllyFaction = RTSGameMode.EFactions.Faction_Allies;
            }

        }

        protected virtual void InitializeAllyValues()
        {
            AllyActiveTimeBar = 0;
        }

        protected virtual void SubToEvents()
        {
            allyEventHandler.EventAllyDied += AllyOnDeath;
            //Called After AllyInCommand has been set
            allyEventHandler.EventPartySwitching += OnPartySwitch;
            allyEventHandler.OnAmmoChanged += OnEquippedWeaponAmmoChanged;
            allyEventHandler.OnTryHitscanFire += OnTryHitscanFire;
            allyEventHandler.OnTryMeleeAttack += OnTryMeleeAttack;
            allyEventHandler.OnAllyTakeDamage += AllyTakeDamage;
            allyEventHandler.OnActiveTimeBarDepletion += OnActiveTimeBarDepletion;
            allyEventHandler.OnToggleActiveTimeRegeneration += OnToggleActiveTimeRegeneration;
            allyEventHandler.InitializeAllyComponents += InitializeAlly;
            //Called Before AllyInCommand has been set
            gamemaster.OnAllySwitch += HandleOnAllySwitch;
        }

        protected virtual void UnSubFromEvents()
        {
            allyEventHandler.EventAllyDied -= AllyOnDeath;
            //Called After AllyInCommand has been set
            allyEventHandler.EventPartySwitching -= OnPartySwitch;
            allyEventHandler.OnAmmoChanged -= OnEquippedWeaponAmmoChanged;
            allyEventHandler.OnTryHitscanFire -= OnTryHitscanFire;
            allyEventHandler.OnTryMeleeAttack -= OnTryMeleeAttack;
            allyEventHandler.OnAllyTakeDamage -= AllyTakeDamage;
            allyEventHandler.OnActiveTimeBarDepletion -= OnActiveTimeBarDepletion;
            allyEventHandler.OnToggleActiveTimeRegeneration -= OnToggleActiveTimeRegeneration;
            allyEventHandler.InitializeAllyComponents -= InitializeAlly;
            //Called Before AllyInCommand has been set
            gamemaster.OnAllySwitch -= HandleOnAllySwitch;
        }

        protected virtual void StartServices()
        {
            
        }

        protected virtual void StopServices()
        {
            CancelInvoke();
        }
        #endregion

        #region Commented Code
        //Health
        //public float AllyHealth { get { return npcHealth.npcHealth; } }
        //public float AllyMaxHealth { get { return npcHealth.npcMaxHealth; } }
        //public float healthAsPercentage { get { return AllyHealth / AllyMaxHealth; } }
        //shortcut properties for partymanager gamemode properties
        #endregion
    }
}