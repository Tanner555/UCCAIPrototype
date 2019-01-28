using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTSCoreFramework
{
    public class AllyStatController : MonoBehaviour
    {
        #region Fields
        [Header("Will be used to identify the character")]
        public ECharacterType characterType;

        private CharacterStats myCharacterStats;
        private Dictionary<EWeaponType, WeaponStats> allWeaponStats = new Dictionary<EWeaponType, WeaponStats>();
        #endregion

        #region SetupProperties
        protected AllyMember allyMember
        {
            get
            {
                if (__allyMember == null)
                    __allyMember = GetComponent<AllyMember>();

                return __allyMember;
            }
        }
        AllyMember __allyMember = null;

        protected AllyEventHandler eventHandler
        {
            get
            {
                if (__eventHandler == null)
                    __eventHandler = GetComponent<AllyEventHandler>();

                return __eventHandler;
            }
        }
        AllyEventHandler __eventHandler = null;

        protected RTSStatHandler statHandler
        {
            get
            {
                return RTSStatHandler.thisInstance;
            }
        }

        protected RTSGameMaster gamemaster
        {
            get { return RTSGameMaster.thisInstance; }
        }
        #endregion

        #region AccessProperties
        //Health
        public virtual int Stat_Health
        {
            get { return myCharacterStats.Health; }
            set
            {
                myCharacterStats.Health = value;
                CallOnHealthChanged();
            }
        }
        public virtual int Stat_MaxHealth
        {
            get { return myCharacterStats.MaxHealth; }
        }
        //Stamina
        public virtual int Stat_Stamina
        {
            get { return myCharacterStats.Stamina; }
            set
            {
                myCharacterStats.Stamina = value;
                CallOnStaminaChanged();
            }
        }
        public virtual int Stat_MaxStamina
        {
            get { return myCharacterStats.MaxStamina; }
        }
        //Weapons
        public virtual EEquipType Stat_EquipType
        {
            get { return myCharacterStats.EquippedWeapon; }
        }
        public virtual EWeaponType Stat_PrimaryWeapon
        {
            get { return myCharacterStats.PrimaryWeapon; }
        }
        public virtual EWeaponType Stat_SecondaryWeapon
        {
            get { return myCharacterStats.SecondaryWeapon; }
        }
        //Other Character Stats
        public virtual ECharacterType Stat_CharacterType
        {
            get { return myCharacterStats.CharacterType; }
        }
        public virtual string Stat_CharacterName
        {
            get { return myCharacterStats.CharacterType.ToString(); }
        }
        public virtual Sprite Stat_CharacterPortrait
        {
            get { return myCharacterStats.CharacterPortrait; }
        }
        #endregion

        #region UnityMessages
        // Use this for initialization
        protected virtual void OnEnable()
        {
            SubToEvents();
        }

        protected virtual void Start()
        {
            Invoke("OnDelayStart", 0.5f);
        }

        protected virtual void OnDelayStart()
        {
            //Equip whatever the ally is holding
            eventHandler.CallOnEquipTypeChanged(myCharacterStats.EquippedWeapon);
            CallOnHealthChanged();
            CallOnStaminaChanged();
        }

        protected virtual void OnDisable()
        {
            UnsubFromEvents();
        }
        #endregion

        #region Getters
        public virtual int CalculateDamageRate()
        {
            var _weapon = GetWeaponStats();
            return _weapon.DamageRate;
        }

        public virtual EWeaponUsage GetWeaponUsage()
        {
            return GetWeaponStats().WeaponUsage;
        }

        public virtual float GetWeaponAttackRate()
        {
            return GetWeaponStats().AttackRate;
        }

        public virtual float GetMeleeMaxAttackDistance()
        {
            return GetWeaponStats().MeleeAttackDistance;
        }

        protected virtual WeaponStats GetWeaponStats()
        {
            switch (myCharacterStats.EquippedWeapon)
            {
                case EEquipType.Primary:
                    return allWeaponStats[myCharacterStats.PrimaryWeapon];
                case EEquipType.Secondary:
                    return allWeaponStats[myCharacterStats.SecondaryWeapon];
                default:
                    return new WeaponStats();
            }
        }

        protected virtual WeaponStats GetWeaponStatsFromWeaponType(EWeaponType _weaponType)
        {
            return allWeaponStats[_weaponType];
        }
        #endregion

        #region Handlers
        protected virtual void InitializeAllyStatController(RTSAllyComponentSpecificFields _specific, RTSAllyComponentsAllCharacterFields _allFields)
        {
            characterType = _specific.CharacterType;
            InitializeCharacterStats();
            RetrieveAllWeaponStats();
            UpdateUnequippedWeaponType();
        }

        /// <summary>
        /// It's not really a Handler right now. When the game starts,
        /// this method is called to update the unequipped weapon type
        /// on the allyEventHandler.
        /// </summary>
        void UpdateUnequippedWeaponType()
        {
            //Explicitly Set Weapon Because Unequipped weapon type
            //on allyEventHandler doesn't become set till OnDelayStart
            EWeaponType _weapon = myCharacterStats.EquippedWeapon == EEquipType.Primary ?
                myCharacterStats.SecondaryWeapon : myCharacterStats.PrimaryWeapon;
            eventHandler.UpdateUnequippedWeaponStats(
                GetWeaponStatsFromWeaponType(
                    _weapon));
        }

        void HandleEquipTypeChanged(EEquipType _eType)
        {
            myCharacterStats.EquippedWeapon = _eType;
            var _weapon = myCharacterStats.EquippedWeapon == EEquipType.Primary ?
                myCharacterStats.PrimaryWeapon : myCharacterStats.SecondaryWeapon;
            var _wUsage = GetWeaponStatsFromWeaponType(_weapon).WeaponUsage;
            eventHandler.CallOnWeaponChanged(myCharacterStats.EquippedWeapon, _weapon, _wUsage, true);

        }
        void HandleWeaponChanged(EEquipType _eType, EWeaponType _weaponType, EWeaponUsage _wUsage, bool _equipped)
        {
            switch (_eType)
            {
                case EEquipType.Primary:
                    if (_weaponType != myCharacterStats.PrimaryWeapon)
                        myCharacterStats.PrimaryWeapon = _weaponType;
                    break;
                case EEquipType.Secondary:
                    if (_weaponType != myCharacterStats.SecondaryWeapon)
                        myCharacterStats.SecondaryWeapon = _weaponType;
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Helpers
        protected virtual void CallOnHealthChanged()
        {
            eventHandler.CallOnHealthChanged(myCharacterStats.Health, myCharacterStats.MaxHealth);
        }

        protected virtual void CallOnStaminaChanged()
        {
            eventHandler.CallOnStaminaChanged(myCharacterStats.Stamina, myCharacterStats.MaxStamina);
        }
        #endregion

        #region Initialization
        protected virtual void SubToEvents()
        {
            eventHandler.OnEquipTypeChanged += HandleEquipTypeChanged;
            eventHandler.OnWeaponChanged += HandleWeaponChanged;
            eventHandler.InitializeAllyComponents += InitializeAllyStatController;
            gamemaster.EventUpdateCharacterStats += InitializeCharacterStats;
            
        }
        protected virtual void UnsubFromEvents()
        {
            eventHandler.OnEquipTypeChanged -= HandleEquipTypeChanged;
            eventHandler.OnWeaponChanged -= HandleWeaponChanged;
            eventHandler.InitializeAllyComponents -= InitializeAllyStatController;
            gamemaster.EventUpdateCharacterStats -= InitializeCharacterStats;
        }
        protected virtual void InitializeCharacterStats()
        {
            myCharacterStats = statHandler.RetrieveCharacterStats(allyMember, characterType);
        }
        protected virtual void RetrieveAllWeaponStats()
        {
            allWeaponStats = statHandler.WeaponStatDictionary;
        }
        #endregion

    }
}