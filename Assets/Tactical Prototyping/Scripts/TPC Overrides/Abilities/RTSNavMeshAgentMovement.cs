using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BaseFramework;
using RTSCoreFramework;
using Opsive.UltimateCharacterController.Character.Abilities.AI;

namespace RTSPrototype
{
    public class RTSNavMeshAgentMovement : NavMeshAgentMovement
    {
        #region Fields
        bool bAllyDied = false;
        bool bIsPaused = false;
        #endregion

        #region Properties
        RTSGameMaster gamemaster => RTSGameMaster.thisInstance;

        AllyEventHandlerWrapper myEventHandler
        {
            get
            {
                if (_myEventHandler == null && m_CharacterLocomotion != null)
                {
                    _myEventHandler = m_CharacterLocomotion.GetComponent<AllyEventHandlerWrapper>();
                }
                return _myEventHandler;
            }
        }
        AllyEventHandlerWrapper _myEventHandler = null;
        #endregion

        #region Overrides
        protected override void AbilityStarted()
        {
            base.AbilityStarted();
            m_CharacterLocomotion.StartCoroutine(AbilityStartedDelayCoroutine());
        }

        IEnumerator AbilityStartedDelayCoroutine()
        {
            yield return new WaitForSeconds(0.5f);
            myEventHandler.EventAllyDied += OnAllyDeath;
            gamemaster.OnTogglebIsInPauseControlMode += HandleGamePaused;
            gamemaster.OnToggleIsGamePaused += HandleGamePaused;
        }

        //public override void Update()
        //{
        //    //Only Update NavMesh If Ally Hasn't Died
        //    if (bIsPaused == false && bAllyDied == false)
        //    {
        //        base.Update();
        //    }
        //}

        //public override void ApplyPosition()
        //{
        //    if (bIsPaused == false && bAllyDied == false)
        //    {
        //        base.ApplyPosition();
        //    }            
        //}

        //public override void ApplyRotation()
        //{
        //    if (bIsPaused == false && bAllyDied == false)
        //    {
        //        base.ApplyRotation();
        //    }
        //}

        //protected override void OnGrounded(bool grounded)
        //{
        //    if (bIsPaused == false && bAllyDied == false)
        //    {
        //        base.OnGrounded(grounded);
        //    }
        //}

        protected override void AbilityStopped(bool force)
        {
            if (m_CharacterLocomotion != null && myEventHandler != null)
            {
                myEventHandler.EventAllyDied -= OnAllyDeath;
                gamemaster.OnTogglebIsInPauseControlMode -= HandleGamePaused;
                gamemaster.OnToggleIsGamePaused -= HandleGamePaused;
            }
            base.AbilityStopped(force);
        }
        #endregion

        #region Handlers
        void HandleGamePaused(bool _paused)
        {
            bIsPaused = _paused;
        }

        private void OnAllyDeath(Vector3 position, Vector3 force, GameObject attacker)
        {
            bAllyDied = true;
        }
        #endregion

    }
}