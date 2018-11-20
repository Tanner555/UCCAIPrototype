using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTSCoreFramework
{
    public class PartyStatController : MonoBehaviour
    {
        #region Fields
        private PartyStats myPartyStats;
        #endregion

        #region Properties
        PartyManager partyManager
        {
            get
            {
                if (__partyManager == null)
                    __partyManager = GetComponent<PartyManager>();

                return __partyManager;
            }
        }
        PartyManager __partyManager = null;

        RTSStatHandler statHandler
        {
            get { return RTSStatHandler.thisInstance; }
        }
        #endregion

        #region UnityMessages
        // Use this for initialization
        void Start()
        {
            InitializePartyStats();
        }

        // Update is called once per frame
        void Update()
        {

        }
        #endregion

        void InitializePartyStats()
        {
            myPartyStats = statHandler.RetrievePartyStats(partyManager, partyManager.GeneralCommander);
        }
    }
}