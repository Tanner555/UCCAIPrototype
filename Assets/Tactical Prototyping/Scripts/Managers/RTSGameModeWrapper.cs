using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Opsive.UltimateCharacterController;
using Opsive.UltimateCharacterController.Camera;
using Opsive.UltimateCharacterController.Utility;
using Opsive.UltimateCharacterController.Character;
using RTSCoreFramework;

namespace RTSPrototype
{
    public class RTSGameModeWrapper : RTSGameMode
    {
        #region Properties
        public static new RTSGameModeWrapper thisInstance
        {
            get { return RTSGameMode.thisInstance as RTSGameModeWrapper; }
        }
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

        //// Use this for initialization
        protected override void Start()
        {
            base.Start();
        }
        #endregion

        #region UIAndCameraProcessing
        /// <summary>
        /// TODO: RTSPrototype Fix SetCameraCharacter Method
        /// </summary>
        /// <param name="_targetCore"></param>
        protected override void SetCameraCharacter(AllyMember _targetCore)
        {
            AllyMemberWrapper _target = (AllyMemberWrapper)_targetCore;
            base.SetCameraCharacter(_target);
            if (Camera.main && Camera.main.GetComponent<RTSCameraController>())
            {
                var _thirdPersonCamera = Camera.main.GetComponent<RTSCameraController>();
                if (_thirdPersonCamera.Character != _target.gameObject)
                {
                    _thirdPersonCamera.Character = _target.gameObject;
                    //if (_target.ChestTransform != null)
                    //    _thirdPersonCamera.FadeTransform = _target.ChestTransform;
                    //else
                    //    _thirdPersonCamera.FadeTransform = null;
                    //if (_target.HeadTransform != null)
                    //    _thirdPersonCamera.DeathAnchor = _target.HeadTransform;
                    //else
                    //    _thirdPersonCamera.DeathAnchor = null;

                    //Initialize Character on Modified Camera Controller
                    _thirdPersonCamera.InitializeAllyCharacter(_target);
                }  
            }
            else
            {
                Debug.LogWarning("Can't set camera character because camera controller cannot be found");
                return;
            }
        }
        #endregion
        
    }
}