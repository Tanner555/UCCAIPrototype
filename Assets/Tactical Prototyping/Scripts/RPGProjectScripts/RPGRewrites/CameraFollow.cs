using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BaseFramework;
using RTSCoreFramework;

namespace RPGPrototype
{
    public class CameraFollow : MonoBehaviour
    {
        #region Properties
        RTSGameMode gamemode
        {
            get { return RTSGameMode.thisInstance; }
        }

        RTSGameMaster gamemaster
        {
            get { return RTSGameMaster.thisInstance; }
        }
        #endregion

        GameObject player;
        // Use this for initialization
        void OnEnable()
        {
            gamemaster.OnAllySwitch += OnAllySwitch;
            //player = GameObject.FindGameObjectWithTag("Ally");
        }

        private void OnDisable()
        {
            gamemaster.OnAllySwitch -= OnAllySwitch;
        }

        // Update is called once per frame
        void LateUpdate()
        {
            if (player == null) return;

            transform.position = player.transform.position;
        }

        void OnAllySwitch(PartyManager _party, AllyMember _toSet, AllyMember _current)
        {
            player = _toSet.gameObject;
        }
    }
}