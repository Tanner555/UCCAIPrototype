using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Opsive.ThirdPersonController;
using RTSCoreFramework;
using Chronos;

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
        private SharedProperty<int> m_PrimaryLoadedCount = null;
        private SharedProperty<int> m_DualWieldLoadedCount = null;
        private SharedProperty<int> m_PrimaryUnloadedCount = null;
        private SharedProperty<int> m_DualWieldUnloadedCount = null;
        private SharedProperty<int> m_SecondaryItemCount = null;
        private SharedProperty<int> m_FirstExtensionItemCount = null;
        private SharedProperty<Item> m_CurrentPrimaryItem = null;
        private SharedProperty<Item> m_CurrentDualWieldItem = null;
        #endregion

        #region Properties
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
            SharedManager.InitializeSharedFields(this.gameObject, this);
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

        protected override void SubToEvents()
        {
            base.SubToEvents();
            //OnInventorySecondaryItemCountChange event doesn't take a parameter, unlock the other two events
            EventHandler.RegisterEvent(this.gameObject, "OnInventoryPrimaryItemCountChange", OnPrimaryItemCountChange);
            EventHandler.RegisterEvent(this.gameObject, "OnInventorySecondaryItemCountChange", OnPrimaryItemCountChange);
        }

        protected override void SubToEventsLater()
        {
            base.SubToEventsLater();
            EventHandler.RegisterEvent<Item>(this.gameObject, "OnInventoryPrimaryItemChange", OnPrimaryItemChange);
            EventHandler.RegisterEvent<Item>(this.gameObject, "OnInventoryDualWieldItemChange", OnPrimaryItemChange);
            //OnInventoryConsumableItemCountChange Takes Three Params
            EventHandler.RegisterEvent<Item, bool, bool>(this.gameObject, "OnInventoryConsumableItemCountChange", OnConsumableItemCountChange);
        }

        protected override void UnsubFromEvents()
        {
            base.UnsubFromEvents();
            EventHandler.UnregisterEvent<Item>(this.gameObject, "OnInventoryPrimaryItemChange", OnPrimaryItemChange);
            EventHandler.UnregisterEvent<Item>(this.gameObject, "OnInventoryDualWieldItemChange", OnPrimaryItemChange);
            //OnInventorySecondaryItemCountChange event doesn't take a parameter, unlock the other two events
            EventHandler.UnregisterEvent(this.gameObject, "OnInventoryPrimaryItemCountChange", OnPrimaryItemCountChange);
            EventHandler.UnregisterEvent(this.gameObject, "OnInventorySecondaryItemCountChange", OnPrimaryItemCountChange);
            //OnInventoryConsumableItemCountChange Takes Three Params
            EventHandler.UnregisterEvent<Item, bool, bool>(this.gameObject, "OnInventoryConsumableItemCountChange", OnConsumableItemCountChange);
        }
        #endregion

        #region Handlers
        void OnPrimaryItemChange(Item item)
        {
            RequestCallAmmoChangedEvent();
        }

        void OnPrimaryItemCountChange()
        {
            RequestCallAmmoChangedEvent();
        }

        void OnConsumableItemCountChange(Item item, bool added, bool immediateChange)
        {
            //Only Call OnAmmoChanged If the Item is The Current Primary Item
            if (item == m_CurrentPrimaryItem.Get())
            {
                RequestCallAmmoChangedEvent();
            }
        }
        #endregion

        #region HelperMethods
        void RequestCallAmmoChangedEvent()
        {
            int _loadedAmmo = m_PrimaryLoadedCount.Get();
            int _unloadedAmmo = m_PrimaryUnloadedCount.Get();
            if (_loadedAmmo < int.MaxValue && _loadedAmmo >= 0 &&
                _unloadedAmmo < int.MaxValue && _unloadedAmmo >= 0)
            {
                CallOnAmmoChanged(_loadedAmmo, _unloadedAmmo);
            }
        }
        #endregion
    }
}