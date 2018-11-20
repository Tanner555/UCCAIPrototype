using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Opsive.ThirdPersonController;

namespace RTSPrototype
{
    public class RTSCameraController : CameraController
    {
        #region RTSGameModeCalls
        //Handle when partymanager switches allyInCommand, rather than on awake
        public void InitializeAllyCharacter(AllyMemberWrapper ally)
        {
            //// If the character is not initialized on start then disable the controller - the controller won't function without a character.
            m_Character = ally.gameObject;
            if (m_Character == null)
            {
                Debug.LogWarning("Error: Unable to find character with the Player tag. Disabling the Camera Controller.");
                m_CameraHandler.enabled = enabled = false;
                return;
            }
            InitializeCharacter(m_Character);
        }

        protected override void RegisterEvents()
        {
            
        }

        protected override void Deactivate()
        {
            
        }
        #endregion

        #region Properties
        RTSPlayerInput myPlayerInput
        {
            get { return RTSPlayerInput.thisInstance; }
        }
        #endregion

        #region UnityMessages
        protected override void Awake()
        {
            m_Transform = transform;
            m_Camera = GetComponent<Camera>();
            m_CameraHandler = GetComponent<CameraHandler>();
            m_CameraMonitor = GetComponent<CameraMonitor>();

            SharedManager.Register(this);

            m_StartPitch = m_Pitch = m_Transform.eulerAngles.x;

            // The active state is a unique state which is layered by the additional states.
            m_ActiveState = ScriptableObject.CreateInstance<CameraState>();
            if (m_CameraStates == null || m_CameraStates.Length == 0)
            {
                m_DefaultState = ScriptableObject.CreateInstance<CameraState>();
                m_CameraStates = new CameraState[] { m_DefaultState };
            }
            else
            {
                m_DefaultState = m_CameraStates[0];
            }
            for (int i = 0; i < m_CameraStates.Length; ++i)
            {
                m_CameraStatesMap.Add(m_CameraStates[i].name, m_CameraStates[i]);
            }
            ChangeState(m_DefaultState, true);
        }
        #endregion

    }
}