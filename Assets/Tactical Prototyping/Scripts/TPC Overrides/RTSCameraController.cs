using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Opsive.UltimateCharacterController;
using Opsive.UltimateCharacterController.Camera;
using Opsive.UltimateCharacterController.Utility;
using Opsive.UltimateCharacterController.Character;

namespace RTSPrototype
{
    public class RTSCameraController : CameraController
    {
        /*
        #region RTSGameModeCalls
        //Handle when partymanager switches allyInCommand, rather than on awake
        public void InitializeAllyCharacter(AllyMemberWrapper ally)
        {
            //// If the character is not initialized on start then disable the controller - the controller won't function without a character.
            m_Character = ally.gameObject;
            if (m_Character == null)
            {
                Debug.LogWarning("Error: Unable to find an Ally Character. Disabling the Camera Controller.");
                //m_CameraHandler.enabled = enabled = false;
                this.enabled = false;
                return;
            }
            InitializeCharacter(m_Character);
        }

        //protected override void RegisterEvents()
        //{
            
        //}

        //protected override void Deactivate()
        //{
            
        //}
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
            base.Awake();

            m_GameObject = gameObject;
            m_Transform = transform;

            // Create the view types from the serialized data.
            DeserializeViewTypes();

            // Initialize the first and third person view types if they haven't been initialized yet.
            if (m_FirstPersonViewType == null && !string.IsNullOrEmpty(m_FirstPersonViewTypeFullName))
            {
                int index;
                if (m_ViewTypeNameMap.TryGetValue(m_FirstPersonViewTypeFullName, out index))
                {
                    m_FirstPersonViewType = m_ViewTypes[index];
                }
            }
            if (m_ThirdPersonViewType == null && !string.IsNullOrEmpty(m_ThirdPersonViewTypeFullName))
            {
                int index;
                if (m_ViewTypeNameMap.TryGetValue(m_ThirdPersonViewTypeFullName, out index))
                {
                    m_ThirdPersonViewType = m_ViewTypes[index];
                }
            }

            // Call Awake on all of the deserialized view types after the camera controller's Awake method is complete.
            if (m_ViewTypes != null)
            {
                for (int i = 0; i < m_ViewTypes.Length; ++i)
                {
                    m_ViewTypes[i].Awake();
                }
            }

            // The items need to know if they are in a first person perspective within Awake.
            if (m_Character != null)
            {
                var characterLocomotion = m_Character.GetCachedComponent<UltimateCharacterLocomotion>();
                characterLocomotion.FirstPersonPerspective = m_ViewType.FirstPersonPerspective;
            }

            //ChangeState(m_DefaultState, true);
        }

        protected override void Start()
        {

        }
        #endregion
        */
    }
}