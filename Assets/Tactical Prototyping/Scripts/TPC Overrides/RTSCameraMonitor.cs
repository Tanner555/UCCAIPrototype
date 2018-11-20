using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Opsive.ThirdPersonController;
using Opsive.ThirdPersonController.Input;

namespace RTSPrototype
{
    public class RTSCameraMonitor : CameraMonitor
    {
        protected override void Start()
        {
            SharedManager.InitializeSharedFields(gameObject, this);
        }

        protected override void InitializeCharacter(GameObject character)
        {
            if (m_Character != null)
            {
                EventHandler.UnregisterEvent(m_Character, "OnRespawn", OnRespawn);
            }

            m_Character = character;

            if (m_Character == null)
            {
                m_CharacterTransform = null;
                m_CharacterController = null;
                m_PlayerInput = null;
                enabled = false;
                return;
            }

            m_CharacterTransform = character.transform;
            m_CharacterController = character.GetComponent<RigidbodyCharacterController>();
            m_PlayerInput = RTSPlayerInput.thisInstance;
            m_PrevMousePosition = Vector3.one * float.MaxValue;
            EventHandler.RegisterEvent(character, "OnRespawn", OnRespawn);
            enabled = true;
            //Execute when switching characters, rather than on start
            if (m_Character != null)
            {
                EventHandler.ExecuteEvent<GameObject>("OnCameraAttachCharacter", m_Character);
            }
        }
    }
}