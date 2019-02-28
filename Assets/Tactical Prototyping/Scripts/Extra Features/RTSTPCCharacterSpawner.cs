﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BaseFramework;
using RTSCoreFramework;
using Abilities = Opsive.UltimateCharacterController.Character.Abilities;
using System;
using System.Linq;
using Chronos;
using UnityEngine.AI;
using UnityEngine.UI;
using Opsive.UltimateCharacterController.Items;
using Opsive.UltimateCharacterController.Utility.Builders;
using Opsive.UltimateCharacterController.Items.Actions;
using Opsive.UltimateCharacterController.Character;
using Opsive.UltimateCharacterController.Inventory;
using Opsive.UltimateCharacterController.Character.Abilities.AI;

namespace RTSPrototype
{
    #region RTSTPCAnimatorClass
    [System.Serializable]
    public class RTSTPCAnimatorProperties
    {
        public string stateName = "Movement";
        public float transitionDuration = 0.2f;
        public bool ItemNamePrefix = true;
        //[EnumFlag]
        //public AnimatorItemStateData.AnimatorLayer AnimLayers;
        public bool IgnoreLowerPriority = false;
    }
    #endregion
    /// <summary>
    /// TODO: RTSPrototype Fix RTSTPCCharacterSpawner Script
    /// </summary>
    public class RTSTPCCharacterSpawner : CharacterSpawner
    {
        #region CharacterBuilderFields
        //CharacterBuilder Fields
        [Header("CharacterBuilder Fields")]
        [Tooltip("The animator controller that the character should use")]
        [SerializeField] protected RuntimeAnimatorController m_AnimatorController;
        [Tooltip("Can the character hold items?")]
        [SerializeField] protected bool m_AddItems = true;
        [Tooltip("A reference to the ItemCollection used by the ItemSetManager and Inventory.")]
        [SerializeField] protected ItemCollection m_ItemCollection;

        string thirdPersonMovementType = "Opsive.UltimateCharacterController.ThirdPersonController.Character.MovementTypes.Combat";
        #endregion

        #region CharacterSetupFields
        //Character Setup Fields
        [Header("Character Setup Fields")]
        [Header("Items To Be Added To Inventory")]
        public List<RTSRangedWeaponInventoryItem> FirearmsToAdd = new List<RTSRangedWeaponInventoryItem>();
        public List<RTSMeleeInventoryItem> MeleeWeaponsToAdd = new List<RTSMeleeInventoryItem>();

        [Header("Ally Instance Setup Fields")]
        [SerializeField]
        protected RTSAllyComponentSpecificFields AllySpecificComponentsToSetUp;

        [Header("All Allies Setup Fields")]
        [SerializeField]
        protected RTSAllyComponentSetupObject AllAllyComponentFieldsObject;

        protected RTSAllyComponentsAllCharacterFieldsWrapper AllAllyComponentFields
        {
            get { return AllAllyComponentFieldsObject.AllyComponentSetupFields; }
        }
        #endregion

        #region ItemBuilderFields
        [Header("Item Builder Fields")]
        [Header("Weapon Positions and Rotations")]
        public RTSTPCWeaponPositionObject WeaponPositionsObject;

        private List<RTSTPCWeaponPositionClass> WeaponPositionsList
        {
            get { return WeaponPositionsObject != null ? WeaponPositionsObject.WeaponPositionsAndRotations : new List<RTSTPCWeaponPositionClass>(); }
        }

        [Header("Used For AddableItem Settings")]
        public RTSAddableItemObject SerializedAddableItem;

        protected List<RTSAddableItem> AddableItemsList
        {
            get { return SerializedAddableItem.AddableItems; }
        }
        #endregion

        #region InventoryClasses
        /// <summary>
        /// Used For Quickly Adding Ranged Weapons On Character Setup
        /// </summary>
        [System.Serializable]
        public class RTSRangedWeaponInventoryItem
        {
            [Tooltip("Only Used For Easy Access")]
            public string TheItemName;
            [Tooltip("A reference to the ItemType")]
            [SerializeField] public ItemType m_ItemType;
            [Tooltip("A reference to the bullets ItemType")]
            [SerializeField] public ItemType m_BulletItemType;
            [Tooltip("The number of bullets to pickup")]
            [SerializeField] public int m_BulletCount = 10;
        }

        /// <summary>
        /// Used For Quickly Adding Melee Weapons On Character Setup
        /// </summary>
        [System.Serializable]
        public class RTSMeleeInventoryItem
        {
            [Tooltip("Only Used For Easy Access")]
            public string TheItemName;
            [Tooltip("A reference to the ItemType")]
            [SerializeField] public ItemType m_ItemType;
        }
        #endregion

        #region Properties
        RTSGameModeWrapper gamemode
        {
            get { return RTSGameModeWrapper.thisInstance; }
        }

        RTSGameMasterWrapper gamemaster
        {
            get { return RTSGameMasterWrapper.thisInstance; }
        }
        #endregion

        #region CharacterBuilder_BuildCharacter
        protected override IEnumerator CharacterBuilder_BuildCharacter()
        {
            yield return new WaitForSeconds(0.05f);
            if (AllySpecificComponentsToSetUp.bBuildCharacterCompletely)
            {
                // Use the Character Builder to add the Ultimate Character Controller components.
                CharacterBuilder.BuildCharacter(spawnedGameObject, true, m_AnimatorController, string.Empty,
                                                thirdPersonMovementType, false,
                                                null, null, true);
                CharacterBuilder.BuildCharacterComponents(spawnedGameObject, true, m_AddItems, m_ItemCollection, false, false, true, true, false, true);
                // Ensure the smoothed bones have been added to the character.
                var characterLocomotion = spawnedGameObject.GetComponent<UltimateCharacterLocomotion>();
                characterLocomotion.AddDefaultSmoothedBones();

                // The Animator Monitor is one of the first components added and the item system hasn't been added to the character yet. Initialize the Item Parameters after the item system has been setup.
                if (m_AddItems)
                {
                    var animatorMonitor = spawnedGameObject.GetComponent<AnimatorMonitor>();
                    if (animatorMonitor != null)
                    {
                        animatorMonitor.InitializeItemParameters();
                    }
                }
            }
            else
            {
                var _characterLocomotionOLD2 = spawnedGameObject.GetComponent<UltimateCharacterLocomotion>();
                //If Character Already Has Needed Components, Don't Bother Trying to Set Them Up
                if (_characterLocomotionOLD2 != null &&
                    _characterLocomotionOLD2.GetAbility<RTSNavMeshAgentMovement>() == null &&
                    _characterLocomotionOLD2.GetAbility<RTSUpdateRotAbility>() == null &&
                    _characterLocomotionOLD2.GetAbility<RTSAreaEffectAbility>() == null &&
                    _characterLocomotionOLD2.GetAbility<RTSSelfHealAbility>() == null)
                {
                    var _oldNavMeshMovement = _characterLocomotionOLD2.GetAbility<NavMeshAgentMovement>();
                    if (_oldNavMeshMovement != null && (_oldNavMeshMovement is RTSNavMeshAgentMovement) == false)
                    {
                        AbilityBuilder.RemoveAbility<NavMeshAgentMovement>(_characterLocomotionOLD2);
                    }
                    var _itemEquipAbility = _characterLocomotionOLD2.GetAbility<Opsive.UltimateCharacterController.Character.Abilities.ItemEquipVerifier>();
                    var _ragdollAbility = _characterLocomotionOLD2.GetAbility<Opsive.UltimateCharacterController.Character.Abilities.Ragdoll>();
                    if (_itemEquipAbility != null)
                    {
                        AbilityBuilder.AddAbility(_characterLocomotionOLD2, typeof(RTSNavMeshAgentMovement), _itemEquipAbility.Index + 1);
                        AbilityBuilder.AddAbility(_characterLocomotionOLD2, typeof(RTSUpdateRotAbility), _itemEquipAbility.Index + 1);
                    }
                    else
                    {
                        AbilityBuilder.AddAbility(_characterLocomotionOLD2, typeof(RTSNavMeshAgentMovement));
                        AbilityBuilder.AddAbility(_characterLocomotionOLD2, typeof(RTSUpdateRotAbility));
                    }
                    //AbilityBuilder.SerializeAbilities(_characterLocomotion);
                    if (_ragdollAbility != null)
                    {
                        AbilityBuilder.AddAbility(_characterLocomotionOLD2, typeof(RTSAreaEffectAbility), _ragdollAbility.Index + 1);
                        AbilityBuilder.AddAbility(_characterLocomotionOLD2, typeof(RTSSelfHealAbility), _ragdollAbility.Index + 1);
                    }
                    else
                    {
                        AbilityBuilder.AddAbility(_characterLocomotionOLD2, typeof(Opsive.UltimateCharacterController.Character.Abilities.Ragdoll), 0);
                        AbilityBuilder.AddAbility(_characterLocomotionOLD2, typeof(RTSAreaEffectAbility), 1);
                        AbilityBuilder.AddAbility(_characterLocomotionOLD2, typeof(RTSSelfHealAbility), 2);
                    }
                    //AbilityBuilder.SerializeAbilities(_characterLocomotion);
                    _characterLocomotionOLD2.GetAbility<RTSNavMeshAgentMovement>().StartType = Abilities.Ability.AbilityStartType.Automatic;
                    _characterLocomotionOLD2.GetAbility<RTSUpdateRotAbility>().StartType = Abilities.Ability.AbilityStartType.Automatic;
                    var _rTSAreaEffectAbility = _characterLocomotionOLD2.GetAbility<RTSAreaEffectAbility>();
                    var _rTSSelfHealAbility = _characterLocomotionOLD2.GetAbility<RTSSelfHealAbility>();
                    _rTSAreaEffectAbility.StartType = Abilities.Ability.AbilityStartType.Manual;
                    _rTSAreaEffectAbility.StopType = Abilities.Ability.AbilityStopType.Manual;
                    _rTSAreaEffectAbility.AbilityIndexParameter = 201;
                    _rTSSelfHealAbility.StartType = Abilities.Ability.AbilityStartType.Manual;
                    _rTSSelfHealAbility.StopType = Abilities.Ability.AbilityStopType.Manual;
                    _rTSSelfHealAbility.AbilityIndexParameter = 202;
                }
            }
        }
        #endregion

        #region CharacterSetup_SetupCharacter
        protected override IEnumerator CharacterSetup_SetupCharacter()
        {
            yield return new WaitForSeconds(0f);
            //Immediately Add Event Handler For Easy Access
            spawnedGameObject.AddComponent<AllyEventHandlerWrapper>();
            //// Add the Speed Change ability.
            //var controller = spawnedGameObject.GetComponent<RigidbodyCharacterController>();
            //AddAllAbilities(controller);
            //// Add the weapons from array to the default loadout.
            //var inventory = spawnedGameObject.GetComponent<Inventory>();
            //var defaultLoadout = inventory.DefaultLoadout;
            //if (defaultLoadout == null)
            //{
            //    defaultLoadout = new Inventory.ItemAmount[0];
            //}

            //var newLoadout = new List<Inventory.ItemAmount>();
            //newLoadout.AddRange(defaultLoadout.ToList());

            ////Add Firearms To New Loadout
            //foreach (var _firearm in FirearmsToAdd)
            //{
            //    //Add The Firearm
            //    newLoadout.Add(new Inventory.ItemAmount(_firearm.m_ItemType, 1));
            //    //Add Ammunition
            //    newLoadout.Add(new Inventory.ItemAmount(_firearm.m_BulletItemType, _firearm.m_BulletCount));
            //}
            ////Add Melee Weapons To New Loadout
            //foreach (var _melee in MeleeWeaponsToAdd)
            //{
            //    //Add The Melee Weapon
            //    newLoadout.Add(new Inventory.ItemAmount(_melee.m_ItemType, 1));
            //}

            //inventory.DefaultLoadout = newLoadout.ToArray();
        }
        #endregion

        #region ItemBuilder_BuildItem
        protected override IEnumerator ItemBuilder_BuildItem()
        {
            yield return new WaitForSeconds(0f);
            //if (SerializedAddableItem == null)
            //{
            //    Debug.LogError("No SerializedAddableItem on character");
            //    yield return new WaitForSeconds(0f);
            //}
            ////Base Method Functionality
            //var characterAnimator = spawnedGameObject.GetComponent<Animator>();
            //for (int i = 0; i < AddableItemsList.Count; ++i)
            //{
            //    var item = GameObject.Instantiate(AddableItemsList[i].Base) as GameObject;
            //    item.name = AddableItemsList[i].Base.name;
            //    var handTransform = characterAnimator.GetBoneTransform(AddableItemsList[i].HandAssignment == ERTSItemBuilderHandAssignment.Left ? HumanBodyBones.LeftHand : HumanBodyBones.RightHand);
            //    item.transform.parent = handTransform.GetComponentInChildren<ItemPlacement>().transform;
            //    Vector3 _position = Vector3.zero;
            //    Vector3 _rotation = Vector3.zero;
            //    //Set Specific Position and Rotation Only If ItemType Has
            //    //Been Assigned in the Inspector
            //    if (AddableItemHasSpecificPosition(AddableItemsList[i].ItemType, out _position, out _rotation))
            //    {
            //        item.transform.localPosition = _position;
            //        item.transform.localEulerAngles = _rotation;
            //    }
            //    else
            //    {
            //        item.transform.localPosition = AddableItemsList[i].LocalPosition;
            //        item.transform.localEulerAngles = AddableItemsList[i].LocalRotation;
            //    }
            //    ItemBuilder.BuildItem(AddableItemsList[i].ItemName, AddableItemsList[i].ItemType, 122 /*Add Anim ID in item*/, spawnedGameObject, 123/*Add Slot ID in item*/,
            //        true, false, null, characterAnimator.runtimeAnimatorController, null, null, null,
            //        true, item, null/*ItemSlot*/, characterAnimator.runtimeAnimatorController, null /*Invisible ShadowCast Material*/,
            //        AddableItemsList[i].Type == ERTSItemBuilderItemType.Melee ? ItemBuilder.ActionType.MeleeWeapon : ItemBuilder.ActionType.ShootableWeapon, AddableItemsList[i].ItemType /*Action Item Type*/);

            //    //ItemBuilder.BuildItem(item, AddableItemsList[i].ItemType, AddableItemsList[i].ItemName, AddableItemsList[i].Type, AddableItemsList[i].HandAssignment);

            //    //Additional Method Functionality
            //    Transform itemTransform = item.transform;
            //    //Set Weapon Volume
            //    var _audioSource = item.GetComponent<AudioSource>();
            //    if (_audioSource != null)
            //    {
            //        _audioSource.volume = AddableItemsList[i].WeaponVolume;
            //    }
            //    //Set Up Shootable Weapon
            //    var _shootable = item.GetComponent<ShootableWeapon>();
            //    if (_shootable != null)
            //    {
            //        //Set up Muzzle Location
            //        if (AddableItemsList[i].m_MuzzleFlash != null)
            //        {
            //            AddableItemsList[i].m_MuzzleFlashLocation = CreateChildObject(itemTransform, "Muzzle Flash Location",
            //                AddableItemsList[i].m_MuzzleFlashPosition,
            //                AddableItemsList[i].m_MuzzleFlashRotation);
            //        }
            //        //Set up Smoke Location
            //        if (AddableItemsList[i].m_Smoke != null)
            //        {
            //            AddableItemsList[i].m_SmokeLocation = CreateChildObject(itemTransform, "Smoke Location",
            //                AddableItemsList[i].m_SmokePosition,
            //                AddableItemsList[i].m_SmokeRotation);
            //            _shootable.ModifyRTSShooterProperties(AddableItemsList[i]);
            //        }
            //    }
            //    else
            //    {
            //        //Set up Melee Weapon
            //        var _melee = item.GetComponent<MeleeWeapon>();
            //        if (_melee != null)
            //        {
            //            _melee.ModifyRTSMeleeProperties(AddableItemsList[i]);
            //        }
            //    }
            //}
        }
        #endregion

        #region CharacterBuilder_UpdateCharacter
        protected override IEnumerator CharacterBuilder_UpdateCharacter()
        {
            yield return new WaitForSeconds(0f);
            // Stop the character from moving so it will reinitialize correctly.
            //spawnedGameObject.GetComponent<UltimateCharacterLocomotion>().StopMovement();

            // After UMA updates it may not assign the correct animator controller - make sure it does.
            //var characterAnimator = spawnedGameObject.GetComponent<Animator>();
            //characterAnimator.runtimeAnimatorController = m_AnimatorController;
            //characterAnimator.updateMode = AnimatorUpdateMode.AnimatePhysics;

            //Inventory _inventory;
            //if ((_inventory = spawnedGameObject.GetComponent<Inventory>()) != null)
            //{
            //    //Fixes Issue With Inventory Not Loading Items Correctly
            //    //Due To Initialization Being Called Before Setting Items
            //    //Inventory Method Was Modified To Be Public
            //    _inventory.Initialize();
            //}

            //Destroy TPC Behaviours Not Needed By The Ally
            //DestroyUnNeededTPCBehaviours();

        }
        #endregion

        #region CharacterSetup_UpdateCharacterSetup
        protected override IEnumerator CharacterSetup_UpdateCharacterSetup()
        {
            spawnedGameObject.layer = gamemode.SingleAllyLayer;
            spawnedGameObject.tag = gamemode.AllyTag;

            //Add Ally Components
            if (spawnedGameObject.GetComponent<NavMeshAgent>() == false)
            {
                spawnedGameObject.AddComponent<NavMeshAgent>();
            }

            spawnedGameObject.AddComponent<AllyStatController>();
            spawnedGameObject.AddComponent<AllyActionQueueController>();
            var _timeline = spawnedGameObject.AddComponent<Timeline>();
            _timeline.mode = TimelineMode.Global;
            _timeline.globalClockKey = gamemaster.allyClocksName;
            _timeline.rewindable = false;

            //Spawn Child Objects
            if (AllAllyComponentFields.bBuildLOSChildObject)
            {
                var _losObject = new GameObject("LOSObject");
                _losObject.transform.parent = spawnedGameObject.transform;
                _losObject.transform.localPosition = AllAllyComponentFields.LOSChildObjectPosition;
                _losObject.transform.localEulerAngles = AllAllyComponentFields.LOSChildObjectRotation;
                AllySpecificComponentsToSetUp.LOSChildObjectTransform = _losObject.transform;
            }

            if (AllySpecificComponentsToSetUp.bBuildEnemyHealthBar &&
                AllAllyComponentFields.EnemyHealthBarPrefab != null)
            {
                var _enemyHealthBar = GameObject.Instantiate(AllAllyComponentFields.EnemyHealthBarPrefab,
                    spawnedGameObject.transform, false);
                var _rect = _enemyHealthBar.GetComponent<RectTransform>();
                _rect.localPosition = AllAllyComponentFields.EnemyHealthBarPosition;
                _rect.localEulerAngles = AllAllyComponentFields.EnemyHealthBarRotation;
                _rect.sizeDelta = AllAllyComponentFields.EnemyHealthSizeDelta;
                _rect.anchorMin = new Vector2(0.5f, 0.5f);
                _rect.anchorMax = new Vector2(0.5f, 0.5f);
                _rect.pivot = new Vector2(0.5f, 0.5f);
                _rect.localScale = AllAllyComponentFields.EnemyHealthLocalScale;

                //Attempt To Set Health and ActiveBar Images By Looking For Them in Code
                foreach (var _image in _enemyHealthBar.transform.GetComponentsInChildren<Image>(true))
                {
                    if (_image.name.ToLower().Contains("health"))
                    {
                        AllySpecificComponentsToSetUp.EnemyHealthBarImage = _image;
                    }
                    else if (_image.name.ToLower().Contains("active"))
                    {
                        AllySpecificComponentsToSetUp.EnemyActiveBarImage = _image;
                    }
                }
            }

            if (AllAllyComponentFields.bBuildAllyIndicatorSpotlight &&
                AllAllyComponentFields.AllyIndicatorSpotlightPrefab != null)
            {
                var _spotlight = GameObject.Instantiate(AllAllyComponentFields.AllyIndicatorSpotlightPrefab,
                    spawnedGameObject.transform, false);
                _spotlight.transform.localPosition = AllAllyComponentFields.AllyIndicatorSpotlightPosition;
                _spotlight.transform.localEulerAngles = AllAllyComponentFields.AllyIndicatorSpotlightRotation;
                AllySpecificComponentsToSetUp.AllyIndicatorSpotlightInstance = _spotlight;
                _spotlight.GetComponent<Light>().enabled = false;
            }

            // Wait For 0.05 Seconds
            yield return new WaitForSeconds(0.05f);

            //Delay Adding These Components
            spawnedGameObject.AddComponent<AllyMemberWrapper>();
            spawnedGameObject.AddComponent<AllyAIControllerWrapper>();
            spawnedGameObject.AddComponent<AllySpecialAbilitiesWrapper>();
            spawnedGameObject.AddComponent<RTSNavBridge>();
            spawnedGameObject.AddComponent<RTSItemAndControlHandler>();
            spawnedGameObject.AddComponent<AllyTacticsController>();
            spawnedGameObject.AddComponent<AllyVisuals>();

            //Call Ally Init Comps Event
            var _eventHandler = spawnedGameObject.GetComponent<AllyEventHandler>();
            _eventHandler.CallInitializeAllyComponents(AllySpecificComponentsToSetUp, AllAllyComponentFields);
        }
        #endregion

        #region CharacterBuilderHelpers
        void DestroyUnNeededTPCBehaviours()
        {
            //var _inventoryHandler = spawnedGameObject.GetComponent<InventoryHandler>();
            //if (_inventoryHandler != null)
            //    Destroy(_inventoryHandler);

            //var _characterHealth = spawnedGameObject.GetComponent<CharacterHealth>();
            //if (_characterHealth != null)
            //    Destroy(_characterHealth);

            //var _cSpawner = spawnedGameObject.GetComponent<CharacterRespawner>();
            //if (_cSpawner != null)
            //    Destroy(_cSpawner);

            //var _controllerHandler = spawnedGameObject.GetComponent<ControllerHandler>();
            //if (_controllerHandler != null)
            //    Destroy(_controllerHandler);

            //var _input = spawnedGameObject.GetComponent<Opsive.ThirdPersonController.Input.UnityInput>();
            //if (_input != null)
            //    Destroy(_input);

        }
        #endregion

        #region CharacterSetupHelpers
        /// <summary>
        /// Used To Add All Abilties Needed In Both Event Calls
        /// </summary>
        //private void AddAllAbilities(RigidbodyCharacterController controller)
        //{
        //    AddAbility<RTSDamageVisualization>(controller, typeof(RTSDamageVisualization), "", Ability.AbilityStartType.Manual, Ability.AbilityStopType.Manual);
        //    AddAbility<Abilities.Fall>(controller, typeof(Abilities.Fall), "", Abilities.Ability.AbilityStartType.Automatic, Abilities.Ability.AbilityStopType.Manual);
        //    AddAbility<Abilities.Jump>(controller, typeof(Abilities.Jump), "Jump", Abilities.Ability.AbilityStartType.ButtonDown, Abilities.Ability.AbilityStopType.Automatic);
        //    //AddAbility<SpeedChange>(controller, typeof(SpeedChange), "", Ability.AbilityStartType.Manual, Ability.AbilityStopType.Manual);
        //    AddAbility<RTSHeightChange>(controller, typeof(RTSHeightChange), "Crouch", Ability.AbilityStartType.Manual, Ability.AbilityStopType.Manual);
        //    AddAbility<Abilities.Die>(controller, typeof(Abilities.Die), "", Abilities.Ability.AbilityStartType.Manual, Abilities.Ability.AbilityStopType.Manual);
        //    AddAbility<RTSAreaEffectAbility>(controller, typeof(RTSAreaEffectAbility), "", Ability.AbilityStartType.Manual, Ability.AbilityStopType.Manual);
        //    AddAbility<RTSSelfHealAbility>(controller, typeof(RTSSelfHealAbility), "", Ability.AbilityStartType.Manual, Ability.AbilityStopType.Manual);
        //}

        //private void DestroyAllAbilities(RigidbodyCharacterController controller)
        //{
        //    DestroyImmediate(controller.GetComponent<RTSDamageVisualization>(), true);
        //    DestroyImmediate(controller.GetComponent<Abilities.Fall>(), true);
        //    DestroyImmediate(controller.GetComponent<Abilities.Jump>(), true);
        //    //DestroyImmediate(controller.GetComponent<SpeedChange>(), true);
        //    DestroyImmediate(controller.GetComponent<RTSHeightChange>(), true);
        //    DestroyImmediate(controller.GetComponent<Abilities.Die>(), true);
        //    DestroyImmediate(controller.GetComponent<RTSAreaEffectAbility>(), true);
        //    DestroyImmediate(controller.GetComponent<RTSSelfHealAbility>(), true);
        //}

        /// <summary>
        /// Adds the ability to the RigidbodyCharacterController.
        /// </summary>
        /// <param name="controller">A reference to the RigidbodyCharacterController.</param>
        /// <param name="type">The type of ability to add.</param>
        /// <param name="inputName">The ability input name. Can be empty.</param>
        /// <param name="startType">The ability StartType.</param>
        /// <param name="stopType">The ability StopType.</param>
        //private T AddAbility<T>(RigidbodyCharacterController controller, Type type, string inputName, Abilities.Ability.AbilityStartType startType, Abilities.Ability.AbilityStopType stopType) where T : Abilities.Ability
        //{
        //    var ability = controller.gameObject.AddComponent(type) as Abilities.Ability;
        //    // The RigidbodyCharacterController will show the ability inspector.
        //    ability.hideFlags = HideFlags.HideInInspector;

        //    // Set the base class values.
        //    ability.StartType = startType;
        //    ability.StopType = stopType;
        //    ability.InputName = inputName;

        //    // Add the ability to the RigidbodyCharacterController.
        //    var abilities = controller.Abilities;
        //    ability.Index = abilities.Length;
        //    Array.Resize(ref abilities, abilities.Length + 1);
        //    abilities[abilities.Length - 1] = ability;
        //    controller.Abilities = abilities;
        //    return (T)ability;
        //}

        //private bool ShouldDelayBehaviourEnabling(Behaviour _behaviour)
        //{
        //    //If Behaviour Equals Type, Delay Enable and Continue Loop
        //    foreach (var _type in TypesToDelay)
        //    {
        //        if (_behaviour.GetType().Equals(_type))
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}
        #endregion

        #region ItemBuilderHelpers
        private bool AddableItemHasSpecificPosition(ItemType _itemType, out Vector3 _position, out Vector3 _rotation)
        {
            _position = Vector3.zero;
            _rotation = Vector3.zero;

            if (WeaponPositionsObject == null) return false;

            foreach (var _wPos in WeaponPositionsList)
            {
                if (_wPos.m_ItemType == _itemType)
                {
                    _position = _wPos.m_LocalPosition;
                    _rotation = _wPos.m_LocalRotation;
                    return true;
                }
            }

            return false;
        }

        private Transform CreateChildObject(Transform parent, string name, Vector3 offset, Vector3 _eulerRotation)
        {
            var child = new GameObject(name).transform;
            child.parent = parent;
            child.localPosition = offset;
            child.transform.localEulerAngles = _eulerRotation;
            return child;
        }
        #endregion
    }
}