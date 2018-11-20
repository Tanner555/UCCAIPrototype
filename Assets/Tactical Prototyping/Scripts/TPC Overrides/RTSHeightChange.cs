using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTSCoreFramework;
using Opsive.ThirdPersonController.Abilities;

namespace RTSPrototype
{
    public class RTSHeightChange : HeightChange
    {
        RTSGameMode gamemode
        {
            get { return RTSGameMode.thisInstance; }
        }

        private void OnEnable()
        {
            //Used To Set Essential Properties
            //I can't change in CharacterSetup Script
            m_StatePrefix = "Crouch";
            m_IdleState = "Movement";
            m_MovementState = "Movement";
            m_ColliderHeightAdjustment = -0.4f;
        }

        public override bool CanStopAbility()
        {
            return true;
            //return !Physics.Raycast(
            //    m_Transform.position, 
            //    m_Transform.up, 
            //    m_Controller.CapsuleCollider.height - 
            //    m_ColliderHeightAdjustment, 
            //    gamemode.IgnoreInvisibleLayersAndAllies, 
            //    QueryTriggerInteraction.Ignore);
        }
    }
}