using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Opsive.ThirdPersonController;

namespace RTSPrototype
{
    #region AddableItemClass
    /// <summary>
    /// Specifies an item which can be added to the generated character.
    /// </summary>
    [System.Serializable]
    public class RTSAddableItem
    {
        [Tooltip("Not Needed For Data, For Easy Editor Access Only")]
        [SerializeField] public string AddableItemName;
        //Base Fields
        [Header("Base Addable Item Fields")]
        [Tooltip("The Third Person Controller ItemType reference")]
        [SerializeField] protected ItemType m_ItemType;
        [Tooltip("A reference to the base object to use")]
        [SerializeField] protected GameObject m_Base;
        [Tooltip("The name of the item")]
        [SerializeField] protected string m_ItemName;
        [Tooltip("The type of item to create")]
        [SerializeField] protected ItemBuilder.ItemTypes m_Type = ItemBuilder.ItemTypes.Shootable;
        [Tooltip("Specifies which hadn the item should be assigned to")]
        [SerializeField] protected ItemBuilder.HandAssignment m_HandAssignment = ItemBuilder.HandAssignment.Right;
        [Tooltip("The local position of the item")]
        [SerializeField] protected Vector3 m_LocalPosition;
        [Tooltip("The local rotation of the item")]
        [SerializeField] protected Vector3 m_LocalRotation;
        //Additional Fields
        [Header("Additional Shared Properties")]
        [SerializeField] [Range(0, 1)] public float WeaponVolume = 1;

        [SerializeField] public List<RTSTPCAnimatorProperties> m_DefaultStatesIdle = new List<RTSTPCAnimatorProperties>();
        [SerializeField] public List<RTSTPCAnimatorProperties> m_DefaultStatesMovement = new List<RTSTPCAnimatorProperties>();
        [SerializeField] public List<RTSTPCAnimatorProperties> m_AimStatesIdle = new List<RTSTPCAnimatorProperties>();
        [SerializeField] public List<RTSTPCAnimatorProperties> m_AimStatesMovement = new List<RTSTPCAnimatorProperties>();
        [SerializeField] public List<RTSTPCAnimatorProperties> m_UseStatesIdle = new List<RTSTPCAnimatorProperties>();
        [SerializeField] public List<RTSTPCAnimatorProperties> m_UseStatesMovement = new List<RTSTPCAnimatorProperties>();
        [SerializeField] public List<RTSTPCAnimatorProperties> m_EquipStatesIdle = new List<RTSTPCAnimatorProperties>();
        [SerializeField] public List<RTSTPCAnimatorProperties> m_EquipStatesMovement = new List<RTSTPCAnimatorProperties>();
        [SerializeField] public List<RTSTPCAnimatorProperties> m_UnequipStatesIdle = new List<RTSTPCAnimatorProperties>();
        [SerializeField] public List<RTSTPCAnimatorProperties> m_UnequipStatesMovement = new List<RTSTPCAnimatorProperties>();

        //Shooter Properties
        [Header("Additional Shooter Properties")]
        [SerializeField] public float m_FireRate;
        [SerializeField] public float m_Spread;
        [SerializeField] public int m_FireCount = 1;
        [SerializeField] public float m_RecoilAmount;
        [SerializeField] public LayerMask m_HitscanImpactLayers;
        [SerializeField] public float m_HitScanImpactForce;
        [SerializeField] public GameObject m_DefaultHitscanDecal;
        [SerializeField] public GameObject m_DefaultHitscanDust;
        [SerializeField] public GameObject m_DefaultHitscanSpark;
        [SerializeField] public int m_ClipSize;

        [SerializeField] public Vector3 m_FirePointPosition = Vector3.zero;
        [SerializeField] public Vector3 m_FirePointRotation = new Vector3(0, 180, 0);

        [SerializeField] public GameObject m_MuzzleFlash;
        //Set Transforms Inside Class Because They Can't Be Set In Inspector
        [HideInInspector] public Transform m_MuzzleFlashLocation;
        [SerializeField] public Vector3 m_MuzzleFlashPosition;
        [SerializeField] public Vector3 m_MuzzleFlashRotation;

        [SerializeField] public GameObject m_Smoke;
        //Set Transforms Inside Class Because They Can't Be Set In Inspector
        [HideInInspector] public Transform m_SmokeLocation;
        [SerializeField] public Vector3 m_SmokePosition;
        [SerializeField] public Vector3 m_SmokeRotation;

        [SerializeField] public AudioClip[] m_FireSound;
        [SerializeField] public AudioClip[] m_ReloadSound;
        [SerializeField] public AudioClip[] m_EmptyFireSound;

        //Melee Properties
        [Header("Additional Melee Properties")]
        [SerializeField] public List<RTSTPCAnimatorProperties> m_RecoilStatesIdle = new List<RTSTPCAnimatorProperties>();
        [SerializeField] public List<RTSTPCAnimatorProperties> m_RecoilStatesMovement = new List<RTSTPCAnimatorProperties>();

        [Tooltip("The number of melee attacks per second")]
        [SerializeField] public float m_AttackRate = 2;
        [Tooltip("The layers that the melee attack can hit")]
        [SerializeField] public LayerMask m_AttackLayer;
        [Tooltip("Any other hitboxes that should be used when determining if the melee weapon hit a target")]
        [SerializeField] public MeleeWeaponHitbox[] m_AttackHitboxes;
        [Tooltip("Can the attack be interrupted to move onto the next attack? The OnAnimatorItemAllowInterruption event must be added to the attack animation")]
        [SerializeField] public bool m_CanInterruptAttack;
        [Tooltip("When the weapon attacks should only one hit be registered per use?")]
        [SerializeField] public bool m_SingleHitAttack;
        [Tooltip("Should the weapon wait for the OnAnimatorItemEndUse to return to a non-use state?")]
        [SerializeField] public bool m_WaitForEndUseEvent;

        [Tooltip("Optionally specify a sound that should randomly play when the weapon is attacked")]
        [SerializeField] public AudioClip[] m_AttackSound;
        [Tooltip("If Attack Sound is specified, play the sound after the specified delay")]
        [SerializeField] public float m_AttackSoundDelay;

        [Tooltip("Optionally specify an event to send to the object hit on damage")]
        [SerializeField] public string m_DamageEvent;
        [Tooltip("The amount of damage done to the object hit")]
        [SerializeField] public float m_DamageAmount = 10;
        [Tooltip("How much force is applied to the object hit")]
        [SerializeField] public float m_ImpactForce = 5;
        [Tooltip("Optionally specify any default dust that should appear on at the location of the object hit. This is only used if no per-object dust is setup in the ObjectManager")]
        [SerializeField] public GameObject m_DefaultDust;
        [Tooltip("Optionally specify a default impact sound that should play at the point of the object hit. This is only used if no per-object sound is setup in the ObjectManager")]
        [SerializeField] public AudioClip m_DefaultImpactSound;

        // Exposed properties
        public ItemType ItemType { get { return m_ItemType; } }
        public GameObject Base { get { return m_Base; } }
        public string ItemName { get { return m_ItemName; } }
        public ItemBuilder.ItemTypes Type { get { return m_Type; } }
        public ItemBuilder.HandAssignment HandAssignment { get { return m_HandAssignment; } }
        public Vector3 LocalPosition { get { return m_LocalPosition; } }
        public Vector3 LocalRotation { get { return m_LocalRotation; } }

        #region OldRTSTPCCode
        //Don't Need These For Now And Are Harder To
        //Set Up Than Other Properties
        //[SerializeField] public GameObject m_FlashLight;
        //[SerializeField] public bool m_ActivateFlashlightOnAim;
        //[SerializeField] public GameObject m_LaserSight;
        //[SerializeField] public bool m_ActivateLaserSightOnAim;
        //[SerializeField] public bool m_DisableCrosshairsWhenLasterSightActive;

        //Easy Access Lists
        //[HideInInspector]
        //public List<List<RTSTPCAnimatorProperties>> AllIdleStates
        //{
        //    get
        //    {
        //        return new List<List<RTSTPCAnimatorProperties>> {
        //            m_DefaultStatesIdle, m_AimStatesIdle, m_UseStatesIdle, m_EquipStatesIdle, m_UnequipStatesIdle
        //        };
        //    }
        //}
        //[HideInInspector]
        //public List<List<RTSTPCAnimatorProperties>> AllMovementStates
        //{
        //    get
        //    {
        //        return new List<List<RTSTPCAnimatorProperties>> {
        //            m_DefaultStatesMovement, m_AimStatesMovement, m_UseStatesMovement, m_EquipStatesMovement, m_UnequipStatesMovement
        //        };
        //    }
        //}
        #endregion
    }
    #endregion

    [CreateAssetMenu(menuName = "RTSPrototype/RTSAddableItemData")]
    public class RTSAddableItemObject : ScriptableObject
    {
        public List<RTSAddableItem> AddableItems = new List<RTSAddableItem>();
    }
}