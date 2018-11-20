using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Opsive.ThirdPersonController;
using Opsive.ThirdPersonController.Wrappers.Abilities;
using RTSCoreFramework;

namespace RTSPrototype
{
    public class AllyAIControllerWrapper : AllyAIController
    {
        #region Properties
        new AllyMemberWrapper allyMember
        {
            get
            {
                if (__allyMember == null)
                    __allyMember = GetComponent<AllyMemberWrapper>();

                return __allyMember;
            }
        }
        private AllyMemberWrapper __allyMember = null;

        new AllyEventHandlerWrapper myEventHandler
        {
            get
            {
                if (__myEventHandler == null)
                    __myEventHandler = GetComponent<AllyEventHandlerWrapper>();

                return __myEventHandler;
            }
        }
        private AllyEventHandlerWrapper __myEventHandler = null;

        RigidbodyCharacterController myRigidbodyTPC
        {
            get
            {
                if (_myRigidbodyTPC == null)
                    _myRigidbodyTPC = GetComponent<RigidbodyCharacterController>();

                return _myRigidbodyTPC;
            }
        }
        private RigidbodyCharacterController _myRigidbodyTPC = null;

        Inventory myInventory
        {
            get
            {
                if (_myInventory == null)
                    _myInventory = GetComponent<Inventory>();

                return _myInventory;
            }
        }
        private Inventory _myInventory = null;

        ItemHandler itemHandler
        {
            get
            {
                if (_itemHandler == null)
                    _itemHandler = GetComponent<ItemHandler>();

                return _itemHandler;
            }
        }
        private ItemHandler _itemHandler = null;

        RTSNavBridge myRTSNavBridge
        {
            get
            {
                if (_myRTSNavBridge == null)
                    _myRTSNavBridge = GetComponent<RTSNavBridge>();

                return _myRTSNavBridge;
            }
        }
        private RTSNavBridge _myRTSNavBridge = null;

        protected override bool AllCompsAreValid
        {
            get
            {
                return myRigidbodyTPC && myInventory && itemHandler
                    && myNavAgent && myRTSNavBridge && myEventHandler
                    && allyMember;
            }
        }

        public AllyMemberWrapper currentTargettedEnemyWrapper
        {
            get { return (AllyMemberWrapper)currentTargettedEnemy; }
        }
        #endregion

        #region UnityMessages
        protected override void Start()
        {
            base.Start();

        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }
        #endregion

        #region Handlers
        void OnDeath()
        {
            var _mHitbox = transform.GetComponentInChildren<MeleeWeaponHitbox>();
            if (_mHitbox != null)
            {
                SphereCollider _sphereCol;
                if ((_sphereCol = _mHitbox.GetComponent<SphereCollider>()) != null)
                {
                    _sphereCol.enabled = false;
                }
                _mHitbox.SetActive(false);
            }
            this.enabled = false;
        }      

        #endregion

        #region Initialization
        protected override void SubToEvents()
        {
            base.SubToEvents();
            myEventHandler.EventAllyDied += OnDeath;
        }

        protected override void UnSubFromEvents()
        {
            base.UnSubFromEvents();
            myEventHandler.EventAllyDied -= OnDeath;
        }
        #endregion

    }
}