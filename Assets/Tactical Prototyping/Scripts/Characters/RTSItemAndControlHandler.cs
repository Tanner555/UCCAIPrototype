using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Opsive.ThirdPersonController;
using Opsive.ThirdPersonController.Wrappers.Abilities;
using RTSCoreFramework;

namespace RTSPrototype
{
    public class RTSItemAndControlHandler : MonoBehaviour
    {
        #region PropsAndFields
        RTSGameMaster gamemaster
        {
            get { return RTSGameMaster.thisInstance; }
        }

        RTSGameModeWrapper gamemode
        {
            get { return (RTSGameModeWrapper)RTSGameModeWrapper.thisInstance; }
        }

        Opsive.ThirdPersonController.Abilities.HeightChange HeightChangeTPCAbility
        {
            get
            {
                if (_HeightChangeTPCAbility == null)
                    _HeightChangeTPCAbility = GetComponent<Opsive.ThirdPersonController.Abilities.HeightChange>();

                return _HeightChangeTPCAbility;
            }
        }
        Opsive.ThirdPersonController.Abilities.HeightChange _HeightChangeTPCAbility = null;

        bool isAiming = false;
        [Header("Gun Types")]
        public ItemType AssualtRifleType;
        public ItemType PistolType;
        public ItemType ShotgunType;
        public ItemType SniperRifleType;
        public ItemType FistType;
        public ItemType KnifeType;
        public ItemType AxeType;
        public ItemType CrossbowType;
        public ItemType KatanaType;

        public const string AssaultRifleName = "Assault Rifle";
        public string AssaultRifName { get { return AssaultRifleName; } }
        const string PistolName = "Pistol";
        const string ShotgunName = "Shotgun";
        const string SniperRifleName = "Sniper Rifle";

        protected bool bIsReloading = false;
        #endregion

        #region Components
        AllyEventHandlerWrapper myEventHandler
        {
            get
            {
                if (_myEventHandler == null)
                    _myEventHandler = GetComponent<AllyEventHandlerWrapper>();

                return _myEventHandler;
            }    
        }
        private AllyEventHandlerWrapper _myEventHandler = null;

        ItemHandler itemHandler
        {
            get
            {
                if (_itemHandler == null)
                    _itemHandler = GetComponent<ItemHandler>();

                return _itemHandler;
            }
        }
        ItemHandler _itemHandler = null;

        Inventory myInventory
        {
            get
            {
                if (_myInventory == null)
                    _myInventory = GetComponent<Inventory>();

                return _myInventory;
            }
        }
        Inventory _myInventory = null;

        RigidbodyCharacterController myController
        {
            get
            {
                if (_myController == null)
                    _myController = GetComponent<RigidbodyCharacterController>();

                return _myController;
            }
        }
        RigidbodyCharacterController _myController = null;

        RTSNavBridge myNavBidge
        {
            get
            {
                if (_myNavBidge == null)
                    _myNavBidge = GetComponent<RTSNavBridge>();

                return _myNavBidge;
            }
        }
        RTSNavBridge _myNavBidge = null;

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

        bool AllCompsAreValid
        {
            get { return myEventHandler && itemHandler && 
                    myInventory && myController && myNavBidge 
                    && allyMember; }
        }
        #endregion

        #region UnityMessages
        private void Awake()
        {
            //InitialSetup();
            
        }

        private void Start()
        {
            Invoke("OnDelayStart", 0.5f);
        }

        private void OnDelayStart()
        {
            OnUnequippedAmmoChanged();
        }

        private void OnEnable()
        {
            SubToEvents();
        }

        private void OnDisable()
        {
            UnsubFromEvents();
        }
        #endregion

        #region Handlers
        void InitializeAllyWeaponItems(RTSAllyComponentSpecificFields _specific, RTSAllyComponentsAllCharacterFields _allFields)
        {
            var _tpcAllyCompsToInit = (RTSAllyComponentsAllCharacterFieldsWrapper)_allFields;
            AssualtRifleType = _tpcAllyCompsToInit.AssualtRifleType;
            PistolType = _tpcAllyCompsToInit.PistolType;
            ShotgunType = _tpcAllyCompsToInit.ShotgunType;
            SniperRifleType = _tpcAllyCompsToInit.SniperRifleType;
            FistType = _tpcAllyCompsToInit.FistType;
            KnifeType = _tpcAllyCompsToInit.KnifeType;
            AxeType = _tpcAllyCompsToInit.AxeType;
            CrossbowType = _tpcAllyCompsToInit.CrossbowType;
            KatanaType = _tpcAllyCompsToInit.KatanaType;
        }

        /// <summary>
        /// No Event Yet, But considering using this as a handler when
        /// picking up ammo. Used to Update Unequipped Ammo When Weapon Switch
        /// Hasn't Yet Occurred.
        /// </summary>
        void OnUnequippedAmmoChanged()
        {
            int _loaded = 0;
            int _unloaded = 0;
            EEquipType _unequippedEType = myEventHandler.MyEquippedType == EEquipType.Primary ?
                EEquipType.Secondary : EEquipType.Primary;
            GetAmmoCountForEquipType(out _loaded, out _unloaded);
            myEventHandler.UpdateWeaponAmmoCount(EEquipType.Secondary,
                _loaded, _unloaded);
        }

        void OnWeaponTypeChanged(EEquipType _eType, EWeaponType _weaponType, EWeaponUsage _wUsage, bool _equipped)
        {
            if (_equipped)
            {
                SetEquippedItem(GetTPSItemFromWeaponType(_weaponType));
            }
        }

        void OnSetAimHandler(bool _isAiming)
        {
            isAiming = _isAiming;
            myController.Aim = _isAiming;
        }

        void OnSwitchPrevItem()
        {
            myInventory.SwitchItem(true, true);
        }

        void OnSwitchNextItem()
        {
            myInventory.SwitchItem(true, false);
        }

        void OnTryUseWeapon()
        {
            if (!AllCompsAreValid) return;
            if(allyMember.bIsCarryingMeleeWeapon == false &&
                myInventory.GetCurrentItemCount(typeof(PrimaryItemType), true) <= 0)
            {
                if (bIsReloading == false)
                {
                    myEventHandler.CallOnTryReload();
                }
            }
            else if (!itemHandler.TryUseItem(typeof(PrimaryItemType)))
            {
                Debug.Log("Couldn't fire primary weapon");
            }
        }

        void OnStopTargetingEnemy()
        {
            if (!AllCompsAreValid) return;
            itemHandler.TryStopUse(true);
        }

        void OnTryReload()
        {
            Debug.Log("Try Reloading");
            if (!AllCompsAreValid) return;
            bIsReloading = true;
            Invoke("ResetIsReloading", 5f);
            if (!itemHandler.TryReload())
            {
                Debug.Log("Couldn't reload primary weapon");
            }
        }

        void ResetIsReloading()
        {
            bIsReloading = false;
        }

        void OnTryCrouch()
        {
            var _ability = HeightChangeTPCAbility;
            if (_ability != null)
            {
                if (!_ability.IsActive)
                {
                    if (!myController.TryStartAbility(_ability))
                    {
                        Debug.Log("Ability HeightChange Failed");
                    }
                }
                else
                {
                    myController.TryStopAbility(_ability);
                }
            }
        }

        void OnToggleSpecialAbility(bool _isActive)
        {
            var _ability = HeightChangeTPCAbility;
            if (_ability != null && _ability.IsActive)
            {
                myController.TryStopAbility(_ability);
            }
        }
        #endregion

        #region Finders/Getters
        void GetAmmoCountForEquipType(out int _loaded, out int _unloaded)
        {
            ItemType _item = 
                GetTPSItemFromWeaponType(myEventHandler.MyUnequippedWeaponType);
            GetAmmoCountForItemType(_item, out _loaded, out _unloaded);
        }

        void GetAmmoCountForItemType(ItemType _item, out int _loaded, out int _unloaded)
        {
            _loaded = myInventory.GetItemCount(_item, true);
            _unloaded = myInventory.GetItemCount(_item, false);
        }

        ItemType GetTPSItemFromWeaponType(EWeaponType _weaponType)
        {
            switch (_weaponType)
            {
                case EWeaponType.Fist:
                    return FistType;
                case EWeaponType.Knife:
                    return KnifeType;
                case EWeaponType.Pistol:
                    return PistolType;
                case EWeaponType.AssaultRifle:
                    return AssualtRifleType;
                case EWeaponType.Shotgun:
                    return ShotgunType;
                case EWeaponType.SniperRifle:
                    return SniperRifleType;
                case EWeaponType.Axe:
                    return AxeType;
                case EWeaponType.Crossbow:
                    return CrossbowType;
                case EWeaponType.Katana:
                    return KatanaType;
                default:
                    return null;
            }
        }

        //Make sure to use wrapper ability namespace, 
        //otherwise the method won't find the ability
        Opsive.ThirdPersonController.Abilities.Ability FindAbility(System.Type _type)
        {
            if (AllCompsAreValid)
            {
                foreach (var _ability in myController.Abilities)
                {
                    if (_type.Equals(_ability.GetType()))
                    {
                        return _ability;
                    }
                }
            }
            return null;
        }
        #endregion

        #region RPGInventoryToTPC
        void SetEquippedItem(ItemType _type)
        {
            var _gun = myInventory.GetCurrentItem(typeof(PrimaryItemType));
            if (_gun != null && _gun.ItemType != _type)
            {
                myInventory.EquipItem((PrimaryItemType)_type);
            }
            else if (_gun == null)
            {
                Debug.Log("Not Setting Equipped Weapon " + _type.ToString());
            }
        }
        #endregion
        
        #region Initialization
        void InitialSetup()
        {
            //myEventHandler = GetComponent<AllyEventHandlerWrapper>();
            //itemHandler = GetComponent<ItemHandler>();
            //myInventory = GetComponent<Inventory>();
            //myController = GetComponent<RigidbodyCharacterController>();
            //myNavBidge = GetComponent<RTSNavBridge>();
            //allyMember = GetComponent<AllyMember>();

            //if (!AllCompsAreValid)
            //{
            //    Debug.LogError("Not all Components can be found");
            //}
        }

        void SubToEvents()
        {
            if (!AllCompsAreValid) return;
            myEventHandler.OnTryAim += OnSetAimHandler;
            myEventHandler.OnSwitchToPrevItem += OnSwitchPrevItem;
            myEventHandler.OnSwitchToNextItem += OnSwitchNextItem;
            myEventHandler.OnTryUseWeapon += OnTryUseWeapon;
            myEventHandler.OnTryReload += OnTryReload;
            myEventHandler.OnTryCrouch += OnTryCrouch;
            myEventHandler.OnWeaponChanged += OnWeaponTypeChanged;
            myEventHandler.EventStopTargettingEnemy += OnStopTargetingEnemy;
            myEventHandler.EventToggleIsUsingAbility += OnToggleSpecialAbility;
            myEventHandler.InitializeAllyComponents += InitializeAllyWeaponItems;
        }

        void UnsubFromEvents()
        {
            myEventHandler.OnTryAim -= OnSetAimHandler;
            myEventHandler.OnSwitchToPrevItem -= OnSwitchPrevItem;
            myEventHandler.OnSwitchToNextItem -= OnSwitchNextItem;
            myEventHandler.OnTryUseWeapon -= OnTryUseWeapon;
            myEventHandler.OnTryReload -= OnTryReload;
            myEventHandler.OnTryCrouch -= OnTryCrouch;
            myEventHandler.OnWeaponChanged -= OnWeaponTypeChanged;
            myEventHandler.EventStopTargettingEnemy -= OnStopTargetingEnemy;
            myEventHandler.EventToggleIsUsingAbility -= OnToggleSpecialAbility;
            myEventHandler.InitializeAllyComponents -= InitializeAllyWeaponItems;
        }
        #endregion
    }
}