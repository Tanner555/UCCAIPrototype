using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BaseFramework
{
    public enum LevelIndex
    {
        Main_Menu = 0,
        Level_1 = 1,
        Level_2 = 2,
        Level_3 = 3,
        Level_4 = 4,
        Level_5 = 5,
        //Used For Error Checking
        No_Level = -1
    }

    public enum ScenarioIndex
    {
        Scenario_1 = 0,
        Scenario_2 = 1,
        Scenario_3 = 2,
        Scenario_4 = 3,
        Scenario_5 = 4,
        Scenario_6 = 5,
        //Used For Error Checking
        No_Scenario = -1
    }

    [System.Serializable]
    public struct ScenarioSettings
    {
        public string ScenarioName;
        public ScenarioIndex Scenario;
    }

    [System.Serializable]
    public struct LevelSettings
    {
        public string LevelName;
        public LevelIndex Level;
        [Tooltip("Used To Load Level Since Scene Assets Cannot Be Used In Builds")]
        public int LevelBuildIndex;
        public Sprite LevelImage;
        public List<ScenarioSettings> ScenarioSettingsList;
    }

    [CreateAssetMenu(menuName = "RTSPrototype/LevelSettingsData")]
    public class LevelSettingsData : ScriptableObject
    {
        [Header("Level Settings")]
        [SerializeField]
        public List<LevelSettings> LevelSettingsList = new List<LevelSettings>();
    }
}