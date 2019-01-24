using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Opsive.UltimateCharacterController;
using Opsive.UltimateCharacterController.Inventory;
using RTSCoreFramework;
using Chronos;
using Opsive.UltimateCharacterController.Events;
using Opsive.UltimateCharacterController.Items;
using Opsive.UltimateCharacterController.Items.Actions;

namespace RTSPrototype
{
    #region RTSAllyComponentsAllCharacterFields
    [System.Serializable]
    public class RTSAllyComponentsAllCharacterFieldsWrapper : RTSAllyComponentsAllCharacterFields
    {
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
    }
    #endregion

    public class AllyEventHandlerWrapper : AllyEventHandler
    {
        #region TPSSharedProperties
        //TODO: RTSPrototype Fix SharedProperty Refs Inside AllyEventHandlerWrapper
        //private SharedProperty<int> m_PrimaryLoadedCount = null;
        //private SharedProperty<int> m_DualWieldLoadedCount = null;
        //private SharedProperty<int> m_PrimaryUnloadedCount = null;
        //private SharedProperty<int> m_DualWieldUnloadedCount = null;
        //private SharedProperty<int> m_SecondaryItemCount = null;
        //private SharedProperty<int> m_FirstExtensionItemCount = null;
        //private SharedProperty<Item> m_CurrentPrimaryItem = null;
        //private SharedProperty<Item> m_CurrentDualWieldItem = null;
        #endregion

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
        public override void CallEventSetAsCommander()
        {
            base.CallEventSetAsCommander();
            if (bHasStartedFromDelay)
            {
                RequestCallAmmoChangedEvent();
            }
        }

        public override void CallEventAllyDied()
        {
            base.CallEventAllyDied();
            EventHandler.ExecuteEvent(this.gameObject, "OnDeath");
        }

        public override void CallOnTryAim(bool _enable)
        {
            base.CallOnTryAim(_enable);
            EventHandler.ExecuteEvent<bool>(this.gameObject, "OnAimAbilityAim", _enable);
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
            EventHandler.RegisterEvent<Item, ItemType, float>(this.gameObject, "OnItemUseConsumableItemType", OnItemUseConsumableItemType);
            EventHandler.RegisterEvent<Item, int>(this.gameObject, "OnInventoryEquipItem", OnInventoryEquipItem);
            EventHandler.RegisterEvent<Item>(this.gameObject, "OnInventoryAddItem", OnInventoryAddItem);
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
            EventHandler.UnregisterEvent<Item, ItemType, float>(this.gameObject, "OnItemUseConsumableItemType", OnItemUseConsumableItemType);
            EventHandler.UnregisterEvent<Item, int>(this.gameObject, "OnInventoryEquipItem", OnInventoryEquipItem);
            EventHandler.UnregisterEvent<Item>(this.gameObject, "OnInventoryAddItem", OnInventoryAddItem);
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
            Debug.Log("OnItemUseConsumableItemType: load = " + _loadedAmmo + " unload = " + _unloadedAmmo);
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