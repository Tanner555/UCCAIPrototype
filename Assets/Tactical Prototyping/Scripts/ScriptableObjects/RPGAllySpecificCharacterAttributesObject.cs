using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTSCoreFramework;
using Sirenix.OdinInspector;
using UnityEngine.UI;
//using RPG.Characters;

namespace RTSPrototype 
{
    [CreateAssetMenu(menuName = "RTSPrototype/RPGAllySpecificCharacterAttributes")]
    public class RPGAllySpecificCharacterAttributesObject : ScriptableObject
    {
        #region RPG Character Attributes
        //Used For Character Death
        [FoldoutGroup("RPG Character Attributes")]
        [Header("Character Death")]
        [SerializeField] public AudioClip[] damageSounds;
        [FoldoutGroup("RPG Character Attributes")]
        [SerializeField] public AudioClip[] deathSounds;
        [FoldoutGroup("RPG Character Attributes")]
        [SerializeField] public float deathVanishSeconds = 2.0f;

        [FoldoutGroup("RPG Character Attributes")]
        [Header("Animator")] [SerializeField] public RuntimeAnimatorController animatorController;
        [FoldoutGroup("RPG Character Attributes")]
        [SerializeField] public AnimatorOverrideController animatorOverrideController;
        [FoldoutGroup("RPG Character Attributes")]
        [SerializeField] public Avatar characterAvatar;
        [FoldoutGroup("RPG Character Attributes")]
        [SerializeField] [Range(.1f, 1f)] public float animatorForwardCap = 1f;

        [FoldoutGroup("RPG Character Attributes")]
        [Header("Audio")]
        [SerializeField] public float audioSourceSpatialBlend = 0.5f;

        [FoldoutGroup("RPG Character Attributes")]
        [Header("Capsule Collider")]
        [SerializeField] public Vector3 colliderCenter = new Vector3(0, 1.03f, 0);
        [FoldoutGroup("RPG Character Attributes")]
        [SerializeField] public float colliderRadius = 0.2f;
        [FoldoutGroup("RPG Character Attributes")]
        [SerializeField] public float colliderHeight = 2.03f;

        [FoldoutGroup("RPG Character Attributes")]
        [Header("Movement")]
        [SerializeField] public float moveSpeedMultiplier = .7f;
        [FoldoutGroup("RPG Character Attributes")]
        [SerializeField] public float animationSpeedMultiplier = 1.5f;
        [FoldoutGroup("RPG Character Attributes")]
        [SerializeField] public float movingTurnSpeed = 360;
        [FoldoutGroup("RPG Character Attributes")]
        [SerializeField] public float stationaryTurnSpeed = 180;
        [FoldoutGroup("RPG Character Attributes")]
        [SerializeField] public float moveThreshold = 1f;

        [FoldoutGroup("RPG Character Attributes")]
        [Header("Nav Mesh Agent")]
        [SerializeField] public float navMeshAgentSteeringSpeed = 1.0f;
        [FoldoutGroup("RPG Character Attributes")]
        [SerializeField] public float navMeshAgentStoppingDistance = 1.3f;
        #endregion

        #region RPG Special Abilites Attributes
        [FoldoutGroup("RPG Special Abilites Attributes")]
        [SerializeField] public RPGPrototype.OLDAbilities.AbilityConfigOLD[] abilities;
        [FoldoutGroup("RPG Special Abilites Attributes")]
        [SerializeField] public Image energyBar;
        [FoldoutGroup("RPG Special Abilites Attributes")]
        [SerializeField] public AudioClip outOfEnergy;
        #endregion

        #region WeaponSystemAttributes
        [Header("WeaponSystemAttributes")]
        [FoldoutGroup("WeaponSystemAttributes")]
        [SerializeField] public float baseDamage = 10f;
        [FoldoutGroup("WeaponSystemAttributes")]
        [SerializeField] public WeaponConfig currentWeaponConfig = null;
        #endregion
    }
}