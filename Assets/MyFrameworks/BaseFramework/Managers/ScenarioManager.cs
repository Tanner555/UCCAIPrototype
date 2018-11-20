using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BaseFramework
{
    public class ScenarioManager : MonoBehaviour
    {
        #region Properties
        GameInstance gameInstance
        {
            get { return GameInstance.thisInstance; }
        }

        ScenarioIndex levelScenario
        {
            get { return gameInstance.CurrentScenario; }
        }
        #endregion

        #region Fields
        [Header("Independent Game Objects")]
        [SerializeField]
        GameObject RTSManagersObject;
        [SerializeField]
        GameObject RTSCoreCanvas;
        [SerializeField]
        GameObject RTSMainCamera;
        [Header("Drag Camera Comps Here to Enable Them at Runtime")]
        [SerializeField]
        List<Behaviour> CameraCompsToEnable;
        [Header("Scenario Dependent Game Objects")]
        [SerializeField]
        GameObject Scenario_1_Spawners;
        [SerializeField]
        GameObject Scenario_2_Spawners;
        [SerializeField]
        GameObject Scenario_3_Spawners;
        [SerializeField]
        GameObject Scenario_4_Spawners;
        [SerializeField]
        GameObject Scenario_5_Spawners;
        [SerializeField]
        GameObject Scenario_6_Spawners;
        #endregion

        #region UnityMessages
        private void Awake()
        {
            if (RTSMainCamera.activeSelf)
            {
                RTSMainCamera.SetActive(false);
            }
            SetupScenario();
        }
        #endregion

        #region ScenarioSetup
        void SetupScenario()
        {
            switch (levelScenario)
            {
                case ScenarioIndex.Scenario_1:
                    ActivateAllObjects(Scenario_1_Spawners);
                    break;
                case ScenarioIndex.Scenario_2:
                    ActivateAllObjects(Scenario_2_Spawners);
                    break;
                case ScenarioIndex.Scenario_3:
                    ActivateAllObjects(Scenario_3_Spawners);
                    break;
                case ScenarioIndex.Scenario_4:
                    ActivateAllObjects(Scenario_4_Spawners);
                    break;
                case ScenarioIndex.Scenario_5:
                    ActivateAllObjects(Scenario_5_Spawners);
                    break;
                case ScenarioIndex.Scenario_6:
                    ActivateAllObjects(Scenario_6_Spawners);
                    break;
                case ScenarioIndex.No_Scenario:
                    ActivateAllObjects(null);
                    break;
                default:
                    break;
            }
        }

        void ActivateAllObjects(GameObject _scenarioSpawners)
        {
            RTSManagersObject.SetActive(true);
            RTSCoreCanvas.SetActive(true);
            //Activate MainCamera if it's not currently active in scene
            if (RTSMainCamera.activeSelf == false)
                RTSMainCamera.SetActive(true);

            //Enable Camera Components
            foreach (var _comp in CameraCompsToEnable)
            {
                _comp.enabled = true;
            }

            if (_scenarioSpawners != null)
            {
                _scenarioSpawners.SetActive(true);
            }
            else
            {
                Debug.LogError(@"No Spawners Have Been Assigned For 
                Scenario " + levelScenario.ToString());
            }
        }
        #endregion

    }
}