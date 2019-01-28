using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using BaseFramework;

namespace RTSCoreFramework
{
    public class ChangeLevelTriggerHandler : MonoBehaviour
    {
        RTSGameInstance gameInstance
        {
            get { return RTSGameInstance.thisInstance; }
        }

        RTSGameMode gamemode
        {
            get { return RTSGameMode.thisInstance; }
        }

        [SerializeField]
        public LevelIndex loadLevel;
        [SerializeField]
        public ScenarioIndex scenario;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.root.tag == gamemode.AllyTag)
            {
                var _ally = other.transform.root.GetComponent<AllyMember>();
                if (_ally != null && _ally.bIsCurrentPlayer)
                {
                    gameInstance.LoadLevel(loadLevel, scenario);
                }
            }
        }
    }
}