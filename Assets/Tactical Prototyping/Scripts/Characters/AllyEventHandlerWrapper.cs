using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Opsive.UltimateCharacterController;
using Opsive.UltimateCharacterController.Inventory;
using RTSCoreFramework;
using Chronos;
using Opsive.UltimateCharacterController.Items;
using Opsive.UltimateCharacterController.Items.Actions;
using uccEventHelper = UtilitiesAndHelpersForUCC.UCCEventsControllerUtility;
using BehaviorDesigner.Runtime;
using Opsive.UltimateCharacterController.Audio;

namespace RTSPrototype
{
    #region RTSAllyComponentSpecificFields
    [System.Serializable]
    public class RTSAllyComponentSpecificFieldsWrapper : RTSAllyComponentSpecificFields
    {
        [Header("SpecificFieldsWrapper")]
        [Tooltip("Is the prefab used a UCC Character? Can cause errors if not setup properly.")]
        public bool bUseUCCCharacter = true;
        [Tooltip("Different From Typical UCC Boolean. True by default. Allows any ragdoll model to work.")]
        public bool bBuildNonUCCCharacterCompletely = true;
        [Tooltip("False by default. Only Enable this option if you set the correct avatar for the specific model in RPG Attributes Object.")]
        public bool bChangeNonUCCCharacterAnimAvatar = false;
        [Header("RPG Character Attributes")]
        [SerializeField]
        public RPGAllySpecificCharacterAttributesObject RPGCharacterAttributesObject;
    }
    #endregion

    #region RTSAllyComponentsAllCharacterFields
    [System.Serializable]
    public class RTSAllyComponentsAllCharacterFieldsWrapper : RTSAllyComponentsAllCharacterFields
    {
        [Header("Behaviour Designer Settings")]
        public bool bUseBehaviourTrees = true;
        public ExternalBehaviorTree allAlliesDefaultBehaviourTree;
        [Header("GunTypes")]
        public ItemType AssualtRifleType;
        public ItemType PistolType;
        public ItemType ShotgunType;
        public ItemType SniperRifleType;
        public ItemType FistType;
        public ItemType KnifeType;
        public ItemType AxeType;
        public ItemType CrossbowType;
        public ItemType KatanaType;
        [Header("Sounds")]
        [SerializeField]
        public AudioClipSet damageSounds = new AudioClipSet();
        [SerializeField]
        public AudioClipSet deathSounds = new AudioClipSet();
    }
    #endregion

    public class AllyEventHandlerWrapper : AllyEventHandler
    {
        #region Properties
        //UCC Properties
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

        //Pause Functionality
        private Timeline myTimeLine
        {
            get
            {
                if (_myTimeLine == null)
                {
                    _myTimeLine = GetComponent<Timeline>();
                }
                return _myTimeLine;
            }
        }
        private Timeline _myTimeLine = null;

        public override bool bAllyIsPaused
        {
            get { return myTimeLine.timeScale == 0; }
        }

        public bool bIsUCCCharacter { get; protected set; }
        #endregion

        #region UnityMessages
        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();
            //SharedManager.InitializeSharedFields(this.gameObject, this);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }
        #endregion

        #region Overrides
        public override void CallInitializeAllyComponents(RTSAllyComponentSpecificFields _specificComps, RTSAllyComponentsAllCharacterFields _allAllyComps)
        {
            var _allySpecFieldsWrapper = (RTSAllyComponentSpecificFieldsWrapper)_specificComps;
            bIsUCCCharacter = _allySpecFieldsWrapper.bUseUCCCharacter;
            base.CallInitializeAllyComponents(_specificComps, _allAllyComps);
        }

        public override void CallEventSetAsCommander()
        {
            base.CallEventSetAsCommander();
            if (bHasStartedFromDelay)
            {
                RequestCallAmmoChangedEvent();
            }
        }

        public override void CallEventAllyDied(Vector3 position, Vector3 force, GameObject attacker)
        {
            base.CallEventAllyDied(position, force, attacker);
            if (bIsUCCCharacter)
            {
                uccEventHelper.CallOnDeath(this.gameObject, position, force, attacker);
            }
        }

        public override void CallOnTryAim(bool _enable)
        {
            base.CallOnTryAim(_enable);
            if (bIsUCCCharacter)
            {
                uccEventHelper.CallOnAimAbilityAim(this.gameObject, _enable);
            }
        }

        protected override void SubToEvents()
        {
            base.SubToEvents();
            //Old TPC Events
            //OnInventorySecondaryItemCountChange event doesn't take a parameter, unlock the other two events
            //EventHandler.RegisterEvent(this.gameObject, "OnInventoryPrimaryItemCountChange", OnPrimaryItemCountChange);
            //EventHandler.RegisterEvent(this.gameObject, "OnInventorySecondaryItemCountChange", OnPrimaryItemCountChange);
        }

        protected override void SubToEventsLater()
        {
            base.SubToEventsLater();
            //Old TPC Events
            //EventHandler.RegisterEvent<Item>(this.gameObject, "OnInventoryPrimaryItemChange", OnPrimaryItemChange);
            //EventHandler.RegisterEvent<Item>(this.gameObject, "OnInventoryDualWieldItemChange", OnPrimaryItemChange);
            ////OnInventoryConsumableItemCountChange Takes Three Params
            //EventHandler.RegisterEvent<Item, bool, bool>(this.gameObject, "OnInventoryConsumableItemCountChange", OnConsumableItemCountChange);
            //New UCC Events
            if (bIsUCCCharacter)
            {
                uccEventHelper.RegisterOnItemUseConsumableItemType(this.gameObject, OnItemUseConsumableItemType);
                uccEventHelper.RegisterOnInventoryEquipItem(this.gameObject, OnInventoryEquipItem);
                uccEventHelper.RegisterOnInventoryAddItem(this.gameObject, OnInventoryAddItem);
            }
        }

        protected override void UnsubFromEvents()
        {
            base.UnsubFromEvents();
            //Old TPC Events
            //EventHandler.UnregisterEvent<Item>(this.gameObject, "OnInventoryPrimaryItemChange", OnPrimaryItemChange);
            //EventHandler.UnregisterEvent<Item>(this.gameObject, "OnInventoryDualWieldItemChange", OnPrimaryItemChange);
            ////OnInventorySecondaryItemCountChange event doesn't take a parameter, unlock the other two events
            //EventHandler.UnregisterEvent(this.gameObject, "OnInventoryPrimaryItemCountChange", OnPrimaryItemCountChange);
            //EventHandler.UnregisterEvent(this.gameObject, "OnInventorySecondaryItemCountChange", OnPrimaryItemCountChange);
            ////OnInventoryConsumableItemCountChange Takes Three Params
            //EventHandler.UnregisterEvent<Item, bool, bool>(this.gameObject, "OnInventoryConsumableItemCountChange", OnConsumableItemCountChange);
            //New UCC Events
            if (bIsUCCCharacter)
            {
                uccEventHelper.UnregisterOnItemUseConsumableItemType(this.gameObject, OnItemUseConsumableItemType);
                uccEventHelper.UnregisterOnInventoryEquipItem(this.gameObject, OnInventoryEquipItem);
                uccEventHelper.UnregisterOnInventoryAddItem(this.gameObject, OnInventoryAddItem);
            }
        }
        #endregion

        #region Handlers
        //New UCC Handlers
        void OnInventoryAddItem(Item item)
        {
            RequestCallAmmoChangedEvent();
        }

        void OnInventoryEquipItem(Item item, int slotID)
        {
            RequestCallAmmoChangedEvent();
        }

        void OnItemUseConsumableItemType(Item m_Item, ItemType m_ConsumableItemType, float m_ClipRemaining)
        {
            int _loadedAmmo = (int)m_ClipRemaining;
            int _unloadedAmmo = (int)myInventory.GetItemTypeCount(m_ConsumableItemType);
            //Debug.Log("OnItemUseConsumableItemType: load = " + _loadedAmmo + " unload = " + _unloadedAmmo);
            RequestCallAmmoChangedEvent(_loadedAmmo, _unloadedAmmo);
        }

        //Old TPC Handlers
        //void OnPrimaryItemChange(Item item)
        //{
        //    RequestCallAmmoChangedEvent();
        //}

        //void OnPrimaryItemCountChange()
        //{
        //    RequestCallAmmoChangedEvent();
        //}

        //void OnConsumableItemCountChange(Item item, bool added, bool immediateChange)
        //{
        //    //Only Call OnAmmoChanged If the Item is The Current Primary Item
        //    //if (item == m_CurrentPrimaryItem.Get())
        //    //{
        //    //    RequestCallAmmoChangedEvent();
        //    //}
        //}
        #endregion

        #region HelperMethods
        void RequestCallAmmoChangedEvent()
        {
            if (bIsUCCCharacter)
            {
                Item _cItem = myInventory.GetItem(0);
                if (_cItem == null) return;
                ItemAction _cItemAction = _cItem.GetItemAction(0);
                if (_cItemAction == null) return;

                int _loadedAmmo = 1;
                int _unloadedAmmo = 1;

                if (_cItemAction is ShootableWeapon)
                {
                    ShootableWeapon _cShootable = (ShootableWeapon)_cItemAction;
                    _loadedAmmo = (int)_cShootable.ClipRemaining;
                    _unloadedAmmo = (int)myInventory.GetItemTypeCount(_cShootable.ConsumableItemType);
                }

                if (_loadedAmmo < int.MaxValue && _loadedAmmo >= 0 &&
                    _unloadedAmmo < int.MaxValue && _unloadedAmmo >= 0)
                {
                    CallOnAmmoChanged(_loadedAmmo, _unloadedAmmo);
                }
            }
        }

        void RequestCallAmmoChangedEvent(int loadedAmmo, int unloadedAmmo)
        {
            if (loadedAmmo < int.MaxValue && loadedAmmo >= 0 &&
                unloadedAmmo < int.MaxValue && unloadedAmmo >= 0)
            {
                CallOnAmmoChanged(loadedAmmo, unloadedAmmo);
            }
        }
        #endregion
    }
}