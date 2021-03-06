﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTSCoreFramework;
using Opsive.UltimateCharacterController;
using Chronos;
using Opsive.UltimateCharacterController.Character;
using Opsive.UltimateCharacterController.Traits;
using Opsive.UltimateCharacterController.Inventory;
using Opsive.UltimateCharacterController.Items;
using Opsive.UltimateCharacterController.Items.Actions;
using uccEventHelper = UtilitiesAndHelpersForUCC.UCCEventsControllerUtility;

namespace RTSPrototype
{
    public class AllyMemberWrapper : AllyMember
    {
        #region Components        
        //Wrappers
        public AllyAIControllerWrapper aiControllerWrapper
        {
            get
            {
                if (_aiControllerWrapper == null)
                    _aiControllerWrapper = GetComponent<AllyAIControllerWrapper>();

                return _aiControllerWrapper;
            }
        }
        private AllyAIControllerWrapper _aiControllerWrapper = null;
        //Third Person Controller
        public CharacterHealth myCharacterHealth
        {
            get
            {
                if (_myCharacterHealth == null)
                    _myCharacterHealth = GetComponent<CharacterHealth>();

                return _myCharacterHealth;
            }
        }
        public CharacterHealth _myCharacterHealth = null;

        public ItemHandler itemHandler
        {
            get
            {
                if (_itemHandler == null)
                    _itemHandler = GetComponent<ItemHandler>();

                return _itemHandler;
            }
        }
        private ItemHandler _itemHandler = null;

        public Inventory myInventory
        {
            get
            {
                if (_myInventory == null)
                    _myInventory = GetComponent<Inventory>();

                return _myInventory;
            }
        }
        private Inventory _myInventory = null;

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

        protected string AllyClockKey
        {
            get { return gamemaster.allyClocksName; }
        }
        #endregion

        #region InstanceProperties
        protected new RTSGameMasterWrapper gamemaster
        {
            get { return RTSGameMasterWrapper.thisInstance; }
        }
        #endregion

        #region Properties
        public bool bIsUCCCharacter => ((AllyEventHandlerWrapper)allyEventHandler).bIsUCCCharacter;

        public override int AllyHealth
        {
            get { return allyStatController.Stat_Health; }
            protected set { allyStatController.Stat_Health = value; }
        }

        public override int AllyMaxHealth
        {
            get { return allyStatController.Stat_MaxHealth; }
        }

        public override int AllyStamina
        {
            get { return allyStatController.Stat_Stamina; }
            protected set { allyStatController.Stat_Stamina = value; }
        }

        public override int AllyMaxStamina
        {
            get { return allyStatController.Stat_MaxStamina; }
        }

        public override int CurrentEquipedAmmo
        {
            get
            {
                bool _isMelee = bIsCarryingMeleeWeapon;
                if (_isMelee) return 1;

                int _loaded, _unloaded;
                GetAmmoCount(out _loaded, out _unloaded, _isMelee);
                return _loaded + _unloaded;
            }
        }

        public override bool bIsCarryingMeleeWeapon
        {
            get
            {
                return allyStatController.GetWeaponUsage() == EWeaponUsage.Melee;
            }
        }

        public override float WeaponAttackRate
        {
            get { return allyStatController.GetWeaponAttackRate(); }
        }

        public override float MaxMeleeAttackDistance
        {
            get
            {
                return allyStatController.GetMeleeMaxAttackDistance();
            }
        }

        public override string CharacterName
        {
            get
            {
                return allyStatController.Stat_CharacterName;
            }
        }

        public override ECharacterType CharacterType
        {
            get
            {
                return allyStatController.Stat_CharacterType;
            }
        }

        public override Sprite CharacterPortrait
        {
            get
            {
                return allyStatController.Stat_CharacterPortrait;
            }
        }

        //public AllyMemberWrapper enemyTargetWrapper
        //{
        //    get { return aiControllerWrapper.currentTargettedEnemyWrapper; }
        //}

        #endregion

        #region UnityMessages
        protected override void OnEnable()
        {
            base.OnEnable();
            
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        protected override void Start()
        {
            base.Start();

        }

        protected override void OnDelayStart()
        {
            base.OnDelayStart();
        }

        #endregion

        #region Handlers
        public override void AllyTakeDamage(int _amount, Vector3 _position, Vector3 _force, AllyMember _instigator, GameObject _hitGameObject, Collider _hitCollider)
        {
            base.AllyTakeDamage(_amount, _position, _force, _instigator, _hitGameObject, _hitCollider);
            allyEventHandler.CallOnAllyAfterTakeDamage(_amount, _position, _force, _instigator, _hitCollider);
            if (bIsUCCCharacter)
            {
                if (bIsCurrentPlayer)
                {
                    uccEventHelper.CallOnHealthDamage(gameObject, _amount, _position, _force, _instigator.gameObject, _hitCollider);
                }

                if (IsAlive == false)
                {
                    uccEventHelper.CallOnDeath(gameObject, _position, _force, _instigator.gameObject);
                }
            }
        }

        public override void AllyTakeDamage(int amount, AllyMember _instigator)
        {
            base.AllyTakeDamage(amount, _instigator);
            Vector3 _position = ChestTransform.position;
            Vector3 _force = Vector3.zero;
            Collider _hitCollider = gameObject.GetComponentInChildren<Collider>();
            allyEventHandler.CallOnAllyAfterTakeDamage(amount, _position, _force, _instigator, _hitCollider);
            if (bIsUCCCharacter)
            {
                if (bIsCurrentPlayer)
                {
                    uccEventHelper.CallOnHealthDamage(gameObject, amount, _position, _force, _instigator.gameObject, _hitCollider);
                }

                if (IsAlive == false)
                {
                    uccEventHelper.CallOnDeath(gameObject, _position, _force, _instigator.gameObject);
                }
            }
        }

        public override void AllyOnDeath(Vector3 position, Vector3 force, GameObject attacker)
        {
            base.AllyOnDeath(position, force, attacker);

        }
        #endregion

        #region Getters
        void GetAmmoCount(out int _loaded, out int _unloaded, bool isMelee)
        {
            _loaded = 1;
            _unloaded = 1;
            ////Don't Calculate Ammo If Using A Melee Weapon
            //if (isMelee) return;

            //if (bIsUCCCharacter)
            //{
            //    Item _item = myInventory.GetItem(0);
            //    ItemAction _cItemAction; ShootableWeapon _shootableWeapon;
            //    if ((_cItemAction = _item.GetItemAction(0)) != null &&
            //        _cItemAction is ShootableWeapon &&
            //        (_shootableWeapon = (ShootableWeapon)_cItemAction) != null)
            //    {
            //        _loaded = (int)_shootableWeapon.ClipRemaining;
            //        _unloaded = (int)myInventory.GetItemTypeCount(_shootableWeapon.ConsumableItemType);
            //    }
            //}
        }

        public override int GetDamageRate()
        {
            return allyStatController.CalculateDamageRate();
        }
        #endregion

        #region Initialization
        protected override void SubToEvents()
        {
            base.SubToEvents();
        }

        protected override void UnSubFromEvents()
        {
            base.UnSubFromEvents();
        }
        #endregion

        #region Testing
        void EquipTesting()
        {
            //Debug.Log("HP " + HPValue);
            //if (bIsCurrentPlayer == false) return;
            //Debug.Log("ATK " + ATKValue);
            //Debug.Log("DEF " + DEFValue);
            //Debug.Log("Right Hand " + RightHandName);
            //Debug.Log("Left Hand " + LeftHandName);

            //var _inventory = ORKFramework.ORK.Game.ActiveGroup.Inventory;

            //var _w = GetWeaponFromName("Shotgun");
            //if (_w != null)
            //{
            //    Debug.Log("Equipping " + _w.GetName());
            //    var _shortcut = new ORKFramework.EquipShortcut(ORKFramework.EquipSet.Weapon, _w.ID, 1, 1);
            //    var _b = RPGCombatant.Equipment.Equip(RightHandID, _shortcut, RPGCombatant.Inventory, false, false);
            //    Debug.Log("Is Equipped " + RPGCombatant.Equipment.IsEquipped(ORKFramework.EquipSet.Weapon, _w.ID, 1));
            //    Debug.Log("Right Hand " + RightHandName);
            //    Debug.Log("Left Hand " + LeftHandName);
            //    RPGCombatant.Equipment.FireChanged();
            //}

            //var _potion = GetItemFromName("Potion");
            //if(_potion != null)
            //{
            //    Debug.Log("Adding Potion");
            //    var _pgain = new ORKFramework.ItemGain()
            //    {
            //        chance = 100,
            //        quantity = 1,
            //        level = 1,
            //        type = ORKFramework.ItemDropType.Item,
            //        id = _potion.ID
            //    };
            //    _inventory.Add(new ORKFramework.ItemGain[1] { _pgain }, true, true);
            //}

            //var _weapon = GetWeaponFromName("Pistol");
            //if(_weapon != null)
            //{
            //    var _gain = new ORKFramework.ItemGain()
            //    {
            //        chance = 100,
            //        quantity = 1,
            //        level = 1,
            //        type = ORKFramework.ItemDropType.Weapon,
            //        id = _weapon.ID
            //    };
            //    RPGCombatant.Inventory.Add(new ORKFramework.ItemGain[1] { _gain}, false, false);
            //}

            //Debug.Log("W "+ORKFramework.ORK.Weapons.data.Length);
            //Debug.Log("I "+ORKFramework.ORK.Items.data.Length);
            //Debug.Log("Right Equipped " + RightHandName);
            //Debug.Log("Left Equipped " + LeftHandName);
            //Debug.Log("Right IsEquipped " + RightHandEquipSlot.Equipped.ToString());
            //Debug.Log("Left IsEquipped " + LeftHandEquipSlot.Equipped.ToString());
            //var _rpgInventory = ORKFramework.ORK.Game.ActiveGroup.Inventory;
            //RPGCombatant.Inventory.Add(RightHandEquipSlot.Equipment, false, false, false);
            //Debug.Log(RPGCombatant.Inventory.Weapons.GetCount(0));
            //RPGCombatant.Inventory.Add(new ORKFramework.ItemGain[2], false, false);

            //foreach (var _item in _rpgInventory.GetContent(false,false,true,false,0,false))
            //{
            //    Debug.Log(_item);
            //}
            //if (RightHandEquipSlot.Equipped)
            //{
            //    var _myHandler = GetComponent<RTSItemAndControlHandler>();
            //    if (_previousName != RightHandName) Debug.Log(RightHandName);
            //    _previousName = RightHandName;
            //    if (_myHandler != null && _myHandler.CheckForInventoryMatch(RightHandName))
            //    {
            //        _myHandler.SetEquippedItemFromString(RightHandName);
            //    }
            //}
        }

        //void WaitForGunn()
        //{
        //    if (bIsCurrentPlayer == false) return;
        //    for (int i = 0; i < ORKFramework.ORK.Weapons.Count; i++)
        //    {
        //        var _slot = RPGCombatant.Equipment.GetFakeEquip(1, new ORKFramework.EquipShortcut(ORKFramework.EquipSet.Weapon, i, 1, 1));
        //        foreach (var _s in _slot)
        //        {
        //            if (_s != null && _s.Equipment != null && _s.Equipped == true)
        //            {
        //                var _myHandler = GetComponent<RTSItemAndControlHandler>();
        //                if (_myHandler != null && _myHandler.CheckForInventoryMatch(_s.Equipment.GetName()))
        //                {
        //                    _myHandler.SetEquippedItemFromString(_s.Equipment.GetName());
        //                }
        //            }
        //        }
        //    }

        //}
        #endregion

    }
}