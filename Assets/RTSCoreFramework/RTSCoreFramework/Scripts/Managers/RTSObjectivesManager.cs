using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTSCoreFramework
{
    public class RTSObjectivesManager : MonoBehaviour
    {
        #region Fields
        bool bAllEnemiesAreDead = false;
        #endregion

        #region Properties
        RTSGameMaster gamemaster
        {
            get { return RTSGameMaster.thisInstance; }
        }
        #endregion

        #region UnityMessages
        // Use this for initialization
        void Start()
        {
            SubToEvents();
        }

        private void OnDisable()
        {
            UnsubFromEvents();
        }
        #endregion

        #region Handlers
        void OnAllEnemiesDead()
        {
            bAllEnemiesAreDead = true;
            CheckIfAllObjectivesAreComplete();
        }
        #endregion

        #region Helpers
        void CheckIfAllObjectivesAreComplete()
        {
            if (bAllEnemiesAreDead)
            {
                //gamemaster.CallEventAllObjectivesCompleted();
            }
        }
        #endregion
        
        #region Initialization
        void SubToEvents()
        {
            gamemaster.EventAllEnemiesAreDead += OnAllEnemiesDead;
        }

        void UnsubFromEvents()
        {
            gamemaster.EventAllEnemiesAreDead -= OnAllEnemiesDead;
        }
        #endregion
    }
}