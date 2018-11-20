using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTSCoreFramework;

namespace RTSPrototype
{
    /// <summary>
    /// Used to document what code changes I make to certain classes
    /// and functions inside Opsive's third person controller
    /// </summary>
    /// 
    public class OpsiveTPCRewriteImplementation : MonoBehaviour
    {
        #region Common Properties
        //Transform rootAllyTransform
        //{
        //    get
        //    {
        //        if (_rootAllyTransform == null)
        //        {
        //            _rootAllyTransform = transform.root;
        //        }
        //        return _rootAllyTransform;
        //    }
        //}
        //Transform _rootAllyTransform = null;
        #endregion

        #region Used Code
        /// <summary>
        /// RTSPrototype-OpsiveTPC-ShootableWeapon: Inside HitscanFire() method, comment out code
        /// and insert this function. Replaces default hitscan fire with autotargeting ally target.
        /// </summary>
        //void OnRTSHitscanFire()
        //{
        //    var fireDirection = FireDirection();
        //    var _force = fireDirection * m_HitscanImpactForce;
        //    rootAllyTransform.SendMessage("CallOnTryHitscanFire", _force, SendMessageOptions.RequireReceiver);
        //}

        /// <summary>
        /// RTSPrototype-OpsiveTPC-ShootableWeapon: Insert This Method Anywhere Inside The Class
        /// </summary>
        /// <param name="_rtsItem"></param>
        public void ModifyRTSShooterProperties(RTSPrototype.RTSAddableItem _rtsItem)
        {
            #region ModifyFields
            //m_FireRate = _rtsItem.m_FireRate;
            //m_Spread = _rtsItem.m_Spread;
            //m_FireCount = _rtsItem.m_FireCount;
            //m_RecoilAmount = _rtsItem.m_RecoilAmount;
            //m_HitscanImpactLayers = _rtsItem.m_HitscanImpactLayers;
            //m_HitscanImpactForce = _rtsItem.m_HitScanImpactForce;
            //m_DefaultHitscanDecal = _rtsItem.m_DefaultHitscanDecal;
            //m_DefaultHitscanDust = _rtsItem.m_DefaultHitscanDust;
            //m_DefaultHitscanSpark = _rtsItem.m_DefaultHitscanSpark;

            //m_ClipSize = _rtsItem.m_ClipSize;
            //if (m_FirePoint != null)
            //{
            //    m_FirePoint.localPosition = _rtsItem.m_FirePointPosition;
            //    m_FirePoint.localEulerAngles = _rtsItem.m_FirePointRotation;
            //}
            //m_MuzzleFlash = _rtsItem.m_MuzzleFlash;
            //if (m_MuzzleFlash != null)
            //{
            //    m_MuzzleFlashLocation = _rtsItem.m_MuzzleFlashLocation;
            //}
            //m_Smoke = _rtsItem.m_Smoke;
            //if (m_Smoke != null)
            //{
            //    m_SmokeLocation = _rtsItem.m_SmokeLocation;
            //}

            //m_FireSound = _rtsItem.m_FireSound;
            //m_ReloadSound = _rtsItem.m_ReloadSound;
            //m_EmptyFireSound = _rtsItem.m_EmptyFireSound;
            #endregion

            #region AnimStateArraySetters
            //if (_rtsItem.m_DefaultStatesIdle.Count > 0)
            //    m_DefaultStates.Idle.Groups[0].States = new AnimatorItemStateData[_rtsItem.m_DefaultStatesIdle.Count];

            //if (_rtsItem.m_DefaultStatesMovement.Count > 0)
            //    m_DefaultStates.Movement.Groups[0].States = new AnimatorItemStateData[_rtsItem.m_DefaultStatesMovement.Count];

            //if (_rtsItem.m_AimStatesIdle.Count > 0)
            //    m_AimStates.Idle.Groups[0].States = new AnimatorItemStateData[_rtsItem.m_AimStatesIdle.Count];

            //if (_rtsItem.m_AimStatesMovement.Count > 0)
            //    m_AimStates.Movement.Groups[0].States = new AnimatorItemStateData[_rtsItem.m_AimStatesMovement.Count];

            //if (_rtsItem.m_UseStatesIdle.Count > 0)
            //    m_UseStates.Idle.Groups[0].States = new AnimatorItemStateData[_rtsItem.m_UseStatesIdle.Count];

            //if (_rtsItem.m_UseStatesMovement.Count > 0)
            //    m_UseStates.Movement.Groups[0].States = new AnimatorItemStateData[_rtsItem.m_UseStatesMovement.Count];

            //if (_rtsItem.m_EquipStatesIdle.Count > 0)
            //    m_EquipStates.Idle.Groups[0].States = new AnimatorItemStateData[_rtsItem.m_EquipStatesIdle.Count];

            //if (_rtsItem.m_EquipStatesMovement.Count > 0)
            //    m_EquipStates.Movement.Groups[0].States = new AnimatorItemStateData[_rtsItem.m_EquipStatesMovement.Count];

            //if (_rtsItem.m_UnequipStatesIdle.Count > 0)
            //    m_UnequipStates.Idle.Groups[0].States = new AnimatorItemStateData[_rtsItem.m_UnequipStatesIdle.Count];

            //if (_rtsItem.m_UnequipStatesMovement.Count > 0)
            //    m_UnequipStates.Movement.Groups[0].States = new AnimatorItemStateData[_rtsItem.m_UnequipStatesMovement.Count];
            #endregion

            #region DefaultStates
            //for (int i = 0; i < _rtsItem.m_DefaultStatesIdle.Count; i++)
            //{
            //    var _stateData = _rtsItem.m_DefaultStatesIdle[i];
            //    m_DefaultStates.Idle.Groups[0].States[i] = new AnimatorItemStateData(
            //        _stateData.stateName, _stateData.transitionDuration, _stateData.ItemNamePrefix)
            //    {
            //        Layer = _stateData.AnimLayers,
            //        IgnoreLowerPriority = _stateData.IgnoreLowerPriority
            //    };
            //}

            //for (int i = 0; i < _rtsItem.m_DefaultStatesMovement.Count; i++)
            //{
            //    var _stateData = _rtsItem.m_DefaultStatesMovement[i];
            //    m_DefaultStates.Movement.Groups[0].States[i] = new AnimatorItemStateData(
            //        _stateData.stateName, _stateData.transitionDuration, _stateData.ItemNamePrefix)
            //    {
            //        Layer = _stateData.AnimLayers,
            //        IgnoreLowerPriority = _stateData.IgnoreLowerPriority
            //    };
            //}
            #endregion

            #region AimStates
            //for (int i = 0; i < _rtsItem.m_AimStatesIdle.Count; i++)
            //{
            //    var _stateData = _rtsItem.m_AimStatesIdle[i];
            //    m_AimStates.Idle.Groups[0].States[i] = new AnimatorItemStateData(
            //        _stateData.stateName, _stateData.transitionDuration, _stateData.ItemNamePrefix)
            //    {
            //        Layer = _stateData.AnimLayers,
            //        IgnoreLowerPriority = _stateData.IgnoreLowerPriority
            //    };
            //}

            //for (int i = 0; i < _rtsItem.m_AimStatesMovement.Count; i++)
            //{
            //    var _stateData = _rtsItem.m_AimStatesMovement[i];
            //    m_AimStates.Movement.Groups[0].States[i] = new AnimatorItemStateData(
            //        _stateData.stateName, _stateData.transitionDuration, _stateData.ItemNamePrefix)
            //    {
            //        Layer = _stateData.AnimLayers,
            //        IgnoreLowerPriority = _stateData.IgnoreLowerPriority
            //    };
            //}
            #endregion

            #region UseStates
            //for (int i = 0; i < _rtsItem.m_UseStatesIdle.Count; i++)
            //{
            //    var _stateData = _rtsItem.m_UseStatesIdle[i];
            //    m_UseStates.Idle.Groups[0].States[i] = new AnimatorItemStateData(
            //        _stateData.stateName, _stateData.transitionDuration, _stateData.ItemNamePrefix)
            //    {
            //        Layer = _stateData.AnimLayers,
            //        IgnoreLowerPriority = _stateData.IgnoreLowerPriority
            //    };
            //}

            //for (int i = 0; i < _rtsItem.m_UseStatesMovement.Count; i++)
            //{
            //    var _stateData = _rtsItem.m_UseStatesMovement[i];
            //    m_UseStates.Movement.Groups[0].States[i] = new AnimatorItemStateData(
            //        _stateData.stateName, _stateData.transitionDuration, _stateData.ItemNamePrefix)
            //    {
            //        Layer = _stateData.AnimLayers,
            //        IgnoreLowerPriority = _stateData.IgnoreLowerPriority
            //    };
            //}
            #endregion

            #region EquipStates
            //for (int i = 0; i < _rtsItem.m_EquipStatesIdle.Count; i++)
            //{
            //    var _stateData = _rtsItem.m_EquipStatesIdle[i];
            //    m_EquipStates.Idle.Groups[0].States[i] = new AnimatorItemStateData(
            //        _stateData.stateName, _stateData.transitionDuration, _stateData.ItemNamePrefix)
            //    {
            //        Layer = _stateData.AnimLayers,
            //        IgnoreLowerPriority = _stateData.IgnoreLowerPriority
            //    };
            //}

            //for (int i = 0; i < _rtsItem.m_EquipStatesMovement.Count; i++)
            //{
            //    var _stateData = _rtsItem.m_EquipStatesMovement[i];
            //    m_EquipStates.Movement.Groups[0].States[i] = new AnimatorItemStateData(
            //        _stateData.stateName, _stateData.transitionDuration, _stateData.ItemNamePrefix)
            //    {
            //        Layer = _stateData.AnimLayers,
            //        IgnoreLowerPriority = _stateData.IgnoreLowerPriority
            //    };
            //}
            #endregion

            #region UnequipStates
            //for (int i = 0; i < _rtsItem.m_UnequipStatesIdle.Count; i++)
            //{
            //    var _stateData = _rtsItem.m_UnequipStatesIdle[i];
            //    m_UnequipStates.Idle.Groups[0].States[i] = new AnimatorItemStateData(
            //        _stateData.stateName, _stateData.transitionDuration, _stateData.ItemNamePrefix)
            //    {
            //        Layer = _stateData.AnimLayers,
            //        IgnoreLowerPriority = _stateData.IgnoreLowerPriority
            //    };
            //}

            //for (int i = 0; i < _rtsItem.m_UnequipStatesMovement.Count; i++)
            //{
            //    var _stateData = _rtsItem.m_UnequipStatesMovement[i];
            //    m_UnequipStates.Movement.Groups[0].States[i] = new AnimatorItemStateData(
            //        _stateData.stateName, _stateData.transitionDuration, _stateData.ItemNamePrefix)
            //    {
            //        Layer = _stateData.AnimLayers,
            //        IgnoreLowerPriority = _stateData.IgnoreLowerPriority
            //    };
            //}
            #endregion
        }

        /// <summary>
        /// RTSPrototype-OpsiveTPC-MeleeWeapon: Insert This Method Anywhere Inside The Class
        /// </summary>
        /// <param name="_rtsItem"></param>
        public void ModifyRTSMeleeProperties(RTSPrototype.RTSAddableItem _rtsItem)
        {
            #region ModifyProperties
            //m_AttackRate = _rtsItem.m_AttackRate;
            //m_AttackLayer = _rtsItem.m_AttackLayer;
            //m_AttackHitboxes = _rtsItem.m_AttackHitboxes;
            //m_CanInterruptAttack = _rtsItem.m_CanInterruptAttack;
            //m_SingleHitAttack = _rtsItem.m_SingleHitAttack;
            //m_WaitForEndUseEvent = _rtsItem.m_WaitForEndUseEvent;
            //m_AttackSound = _rtsItem.m_AttackSound;
            //m_AttackSoundDelay = _rtsItem.m_AttackSoundDelay;

            //m_DamageEvent = _rtsItem.m_DamageEvent;
            //m_DamageAmount = _rtsItem.m_DamageAmount;
            //m_ImpactForce = _rtsItem.m_ImpactForce;
            //m_DefaultDust = _rtsItem.m_DefaultDust;
            //m_DefaultImpactSound = _rtsItem.m_DefaultImpactSound;

            //m_DefaultStates = _rtsItem.m_DefaultStates;
            //m_AimStates = _rtsItem.m_AimStates;
            //m_UseStates = _rtsItem.m_UseStates;
            //m_EquipStates = _rtsItem.m_EquipStates;
            //m_UnequipStates = _rtsItem.m_UnequipStates;
            #endregion

            #region AnimStateArraySetters
            //if (_rtsItem.m_DefaultStatesIdle.Count > 0)
            //    m_DefaultStates.Idle.Groups[0].States = new AnimatorItemStateData[_rtsItem.m_DefaultStatesIdle.Count];

            //if (_rtsItem.m_DefaultStatesMovement.Count > 0)
            //    m_DefaultStates.Movement.Groups[0].States = new AnimatorItemStateData[_rtsItem.m_DefaultStatesMovement.Count];

            //if (_rtsItem.m_AimStatesIdle.Count > 0)
            //    m_AimStates.Idle.Groups[0].States = new AnimatorItemStateData[_rtsItem.m_AimStatesIdle.Count];

            //if (_rtsItem.m_AimStatesMovement.Count > 0)
            //    m_AimStates.Movement.Groups[0].States = new AnimatorItemStateData[_rtsItem.m_AimStatesMovement.Count];

            //if (_rtsItem.m_UseStatesIdle.Count > 0)
            //    m_UseStates.Idle.Groups[0].States = new AnimatorItemStateData[_rtsItem.m_UseStatesIdle.Count];

            //if (_rtsItem.m_UseStatesMovement.Count > 0)
            //    m_UseStates.Movement.Groups[0].States = new AnimatorItemStateData[_rtsItem.m_UseStatesMovement.Count];

            //if (_rtsItem.m_RecoilStatesIdle.Count > 0)
            //    m_RecoilStates.Idle.Groups[0].States = new AnimatorItemStateData[_rtsItem.m_RecoilStatesIdle.Count];

            //if (_rtsItem.m_RecoilStatesMovement.Count > 0)
            //    m_RecoilStates.Movement.Groups[0].States = new AnimatorItemStateData[_rtsItem.m_RecoilStatesMovement.Count];

            //if (_rtsItem.m_EquipStatesIdle.Count > 0)
            //    m_EquipStates.Idle.Groups[0].States = new AnimatorItemStateData[_rtsItem.m_EquipStatesIdle.Count];

            //if (_rtsItem.m_EquipStatesMovement.Count > 0)
            //    m_EquipStates.Movement.Groups[0].States = new AnimatorItemStateData[_rtsItem.m_EquipStatesMovement.Count];

            //if (_rtsItem.m_UnequipStatesIdle.Count > 0)
            //    m_UnequipStates.Idle.Groups[0].States = new AnimatorItemStateData[_rtsItem.m_UnequipStatesIdle.Count];

            //if (_rtsItem.m_UnequipStatesMovement.Count > 0)
            //    m_UnequipStates.Movement.Groups[0].States = new AnimatorItemStateData[_rtsItem.m_UnequipStatesMovement.Count];
            #endregion

            #region DefaultStates
            //for (int i = 0; i < _rtsItem.m_DefaultStatesIdle.Count; i++)
            //{
            //    var _stateData = _rtsItem.m_DefaultStatesIdle[i];
            //    m_DefaultStates.Idle.Groups[0].States[i] = new AnimatorItemStateData(
            //        _stateData.stateName, _stateData.transitionDuration, _stateData.ItemNamePrefix)
            //    {
            //        Layer = _stateData.AnimLayers,
            //        IgnoreLowerPriority = _stateData.IgnoreLowerPriority
            //    };
            //}

            //for (int i = 0; i < _rtsItem.m_DefaultStatesMovement.Count; i++)
            //{
            //    var _stateData = _rtsItem.m_DefaultStatesMovement[i];
            //    m_DefaultStates.Movement.Groups[0].States[i] = new AnimatorItemStateData(
            //        _stateData.stateName, _stateData.transitionDuration, _stateData.ItemNamePrefix)
            //    {
            //        Layer = _stateData.AnimLayers,
            //        IgnoreLowerPriority = _stateData.IgnoreLowerPriority
            //    };
            //}
            #endregion

            #region AimStates
            //for (int i = 0; i < _rtsItem.m_AimStatesIdle.Count; i++)
            //{
            //    var _stateData = _rtsItem.m_AimStatesIdle[i];
            //    m_AimStates.Idle.Groups[0].States[i] = new AnimatorItemStateData(
            //        _stateData.stateName, _stateData.transitionDuration, _stateData.ItemNamePrefix)
            //    {
            //        Layer = _stateData.AnimLayers,
            //        IgnoreLowerPriority = _stateData.IgnoreLowerPriority
            //    };
            //}

            //for (int i = 0; i < _rtsItem.m_AimStatesMovement.Count; i++)
            //{
            //    var _stateData = _rtsItem.m_AimStatesMovement[i];
            //    m_AimStates.Movement.Groups[0].States[i] = new AnimatorItemStateData(
            //        _stateData.stateName, _stateData.transitionDuration, _stateData.ItemNamePrefix)
            //    {
            //        Layer = _stateData.AnimLayers,
            //        IgnoreLowerPriority = _stateData.IgnoreLowerPriority
            //    };
            //}
            #endregion

            #region UseStates
            //for (int i = 0; i < _rtsItem.m_UseStatesIdle.Count; i++)
            //{
            //    var _stateData = _rtsItem.m_UseStatesIdle[i];
            //    m_UseStates.Idle.Groups[0].States[i] = new AnimatorItemStateData(
            //        _stateData.stateName, _stateData.transitionDuration, _stateData.ItemNamePrefix)
            //    {
            //        Layer = _stateData.AnimLayers,
            //        IgnoreLowerPriority = _stateData.IgnoreLowerPriority
            //    };
            //}

            //for (int i = 0; i < _rtsItem.m_UseStatesMovement.Count; i++)
            //{
            //    var _stateData = _rtsItem.m_UseStatesMovement[i];
            //    m_UseStates.Movement.Groups[0].States[i] = new AnimatorItemStateData(
            //        _stateData.stateName, _stateData.transitionDuration, _stateData.ItemNamePrefix)
            //    {
            //        Layer = _stateData.AnimLayers,
            //        IgnoreLowerPriority = _stateData.IgnoreLowerPriority
            //    };
            //}
            #endregion

            #region RecoilStates
            //for (int i = 0; i < _rtsItem.m_RecoilStatesIdle.Count; i++)
            //{
            //    var _stateData = _rtsItem.m_RecoilStatesIdle[i];
            //    m_RecoilStates.Idle.Groups[0].States[i] = new AnimatorItemStateData(
            //        _stateData.stateName, _stateData.transitionDuration, _stateData.ItemNamePrefix)
            //    {
            //        Layer = _stateData.AnimLayers,
            //        IgnoreLowerPriority = _stateData.IgnoreLowerPriority
            //    };
            //}

            //for (int i = 0; i < _rtsItem.m_RecoilStatesMovement.Count; i++)
            //{
            //    var _stateData = _rtsItem.m_RecoilStatesMovement[i];
            //    m_RecoilStates.Movement.Groups[0].States[i] = new AnimatorItemStateData(
            //        _stateData.stateName, _stateData.transitionDuration, _stateData.ItemNamePrefix)
            //    {
            //        Layer = _stateData.AnimLayers,
            //        IgnoreLowerPriority = _stateData.IgnoreLowerPriority
            //    };
            //}
            #endregion

            #region EquipStates
            //for (int i = 0; i < _rtsItem.m_EquipStatesIdle.Count; i++)
            //{
            //    var _stateData = _rtsItem.m_EquipStatesIdle[i];
            //    m_EquipStates.Idle.Groups[0].States[i] = new AnimatorItemStateData(
            //        _stateData.stateName, _stateData.transitionDuration, _stateData.ItemNamePrefix)
            //    {
            //        Layer = _stateData.AnimLayers,
            //        IgnoreLowerPriority = _stateData.IgnoreLowerPriority
            //    };
            //}

            //for (int i = 0; i < _rtsItem.m_EquipStatesMovement.Count; i++)
            //{
            //    var _stateData = _rtsItem.m_EquipStatesMovement[i];
            //    m_EquipStates.Movement.Groups[0].States[i] = new AnimatorItemStateData(
            //        _stateData.stateName, _stateData.transitionDuration, _stateData.ItemNamePrefix)
            //    {
            //        Layer = _stateData.AnimLayers,
            //        IgnoreLowerPriority = _stateData.IgnoreLowerPriority
            //    };
            //}
            #endregion

            #region UnequipStates
            //for (int i = 0; i < _rtsItem.m_UnequipStatesIdle.Count; i++)
            //{
            //    var _stateData = _rtsItem.m_UnequipStatesIdle[i];
            //    m_UnequipStates.Idle.Groups[0].States[i] = new AnimatorItemStateData(
            //        _stateData.stateName, _stateData.transitionDuration, _stateData.ItemNamePrefix)
            //    {
            //        Layer = _stateData.AnimLayers,
            //        IgnoreLowerPriority = _stateData.IgnoreLowerPriority
            //    };
            //}

            //for (int i = 0; i < _rtsItem.m_UnequipStatesMovement.Count; i++)
            //{
            //    var _stateData = _rtsItem.m_UnequipStatesMovement[i];
            //    m_UnequipStates.Movement.Groups[0].States[i] = new AnimatorItemStateData(
            //        _stateData.stateName, _stateData.transitionDuration, _stateData.ItemNamePrefix)
            //    {
            //        Layer = _stateData.AnimLayers,
            //        IgnoreLowerPriority = _stateData.IgnoreLowerPriority
            //    };
            //}
            #endregion
        }

        /// <summary>
        /// RTSPrototype-OpsiveTPC-MeleeWeapon: Inside Attack() method, comment out all of the code.
        /// This method references the Health script that my game doesn't use. 
        /// </summary>
        //protected virtual void Attack(Transform hitTransform, Vector3 hitPoint, Vector3 hitNormal)
        //{
        //    Comment out the method code
        //}

        /// <summary>
        /// RTSPrototype-OpsiveTPC-MeleeWeapon: Inside TryUse() method, inside the second if statement
        /// right before where the method returns true, add this message call to the rootally
        /// </summary>
        //public override bool TryUse()
        //{
        //    if (!m_InUse && m_LastAttackTime + m_AttackDelay < Time.time)
        //    {
        //        .....
        //        .....
        //        rootAllyTransform.SendMessage("CallOnTryMeleeAttack", SendMessageOptions.RequireReceiver);
        //        return true;
        //    }
        //}

        /// <summary>
        /// RTSPrototype-OpsiveTPC-CameraHandler: Inside Update() method, comment out ln of code
        /// at the end, where m_StepZoom is being set.
        /// This allows me to use a custom stepzoom solution in my RTSCameraController inherited class.
        /// </summary>
        //void Update()
        //{
        //    m_StepZoom = m_CameraController.ActiveState.StepZoomSensitivity > 0 ?
        //    m_PlayerInput.GetAxisRaw(Constants.StepZoomName) : 0;
        //}

        /// <summary>
        /// RTSPrototype-OpsiveTPC-Respawner: Inside OnDisable() method, comment out code
        /// at beginning that calls the OnStartRespawn method. Do not comment out
        /// the line with OnStartRespawn method being Unregistered 
        /// </summary>
        //void OnDisable()
        //{
        //    //if (m_RespawnEvent == null) {
        //    //    OnStartRespawn();
        //    //}
        //}

        /// <summary>
        /// RTSPrototype-OpsiveTPC-NavMeshAgentBridge: Fields m_Controller, 
        /// m_JumpAbility, and m_FallAbility are now Properties. They
        /// Use Initialization On Demand, So They Don't Need To Be Init
        /// On Awake.
        /// </summary>
        private void Awake()
        {
            //Comment Out Fields That I turned into properties
        }

        /// <summary>
        /// RTSPrototype-OpsiveTPC-AnimatorStateData: Inside AnimatorItemGroupData
        /// class, add setter inside States Property. Around Line 124
        /// </summary>
        //class AnimatorItemGroupData
        //{
        //    public AnimatorItemStateData[] States
        //    {
        //        get { return m_States; }
        //        //ADD THIS
        //        set { m_States = value; }
        //    }
        //}
        #endregion

        #region SimpleBugFixes
        /// <summary>
        /// RTSPrototype-OpsiveTPC-RigidbodyCharacterController: Before For Loop,
        /// Check If LinkedColliders Array is Equal To Null
        /// </summary>
        //void EnableCollider(bool _enable)
        //{
        //    if (m_LinkedColliders == null) return;
        //}

        /// <summary>
        /// RTSPrototype-OpsiveTPC-Die (Ability): Before For Loop,
        /// Check If LinkedColliders Array is Equal To Null
        /// </summary>
        //void Awake()
        //{
        //    if (m_IgnoreColliders != null)
        //    {
        //        for (int j = 0; j < m_IgnoreColliders.Length; ++j)
        //        {
        //            ......
        //        }
        //    }
        //}
        #endregion
    }
}