using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Opsive.ThirdPersonController;
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

        //// Update is called once per frame
        protected override void Update()
        {
            base.Update();
        }
        #endregion

        #region UIAndCameraProcessing
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
                    if (_target.ChestTransform != null)
                        _thirdPersonCamera.FadeTransform = _target.ChestTransform;
                    else
                        _thirdPersonCamera.FadeTransform = null;
                    if (_target.HeadTransform != null)
                        _thirdPersonCamera.DeathAnchor = _target.HeadTransform;
                    else
                        _thirdPersonCamera.DeathAnchor = null;

                    //Initialize Character on Modified Camera Controller
                    _thirdPersonCamera.InitializeAllyCharacter(_target);
                }  
            }
            else
            {
                Debug.LogError("Can't set camera character because camera controller cannot be found");
                return;
            }
        }
        #endregion
        
    }
}