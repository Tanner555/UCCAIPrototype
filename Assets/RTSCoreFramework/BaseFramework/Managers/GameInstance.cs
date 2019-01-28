using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BaseFramework
{
    public class GameInstance : DontDestroyOnLoadBaseSingleton<GameInstance>
    {
        #region Dictionaries
        //Used to Retrieve Information About a Level
        protected Dictionary<LevelIndex, LevelSettings> levelSettingsDictionary = new Dictionary<LevelIndex, LevelSettings>();
        protected Dictionary<ScenarioIndex, ScenarioSettings> currentScenarioSettingsDictionary = new Dictionary<ScenarioIndex, ScenarioSettings>();
        #endregion

        #region Properties
        public Dictionary<LevelIndex, LevelSettings> LevelSettingsDictionary
        {
            get { return levelSettingsDictionary; }
        }

        public Dictionary<ScenarioIndex, ScenarioSettings> CurrentScenarioSettingsDictionary
        {
            get { return currentScenarioSettingsDictionary; }
        }

        public LevelIndex CurrentLevel
        {
            get { return currentLevel; }
        }

        public ScenarioIndex CurrentScenario
        {
            get { return currentScenario; }
        }

        //Used To Protect Against Invalid Retrieval of CurrentScenarioSettings
        protected bool bCurrentScenarioIsValid
        {
            get { return CurrentScenario != ScenarioIndex.No_Scenario; }
        }
        //Quick Getter For Scenario Settings, Could Cause Issues
        protected ScenarioSettings CurrentScenarioSettings
        {
            get
            {
                if (bCurrentScenarioIsValid)
                    return currentScenarioSettingsDictionary[CurrentScenario];

                return new ScenarioSettings();
            }
        }

        protected virtual int RefreshRate
        {
            get
            {
                if (_refreshRate == -1)
                    _refreshRate = Screen.currentResolution.refreshRate;

                return _refreshRate;
            }
        }
        private int _refreshRate = -1;
        #endregion

        #region Fields
        [Header("Current Level Info")]
        [SerializeField]
        protected LevelIndex currentLevel = LevelIndex.Main_Menu;
        [SerializeField]
        protected ScenarioIndex currentScenario = ScenarioIndex.No_Scenario;
        [Header("Data Containing Level Settings")]
        [SerializeField]
        protected LevelSettingsData levelSettingsData;
        #endregion

        #region UnityMessages
        protected virtual void OnEnable()
        {
            //Check If There's More Than One Instance in Scene
            if(bHasInstance && thisInstance != this)
            {
                Debug.Log("More than one game instance in scene, destroying instance");
                DestroyImmediate(this.gameObject); 
            }

            InitializeDictionaryValues();
            UpdateFrameRateLimit();
        }

        protected virtual void Update()
        {
            UpdateFrameRateLimit();
        }
        #endregion

        #region LevelManagement
        public virtual void LoadLevel(LevelIndex _level, ScenarioIndex _scenario)
        {
            if (levelSettingsDictionary.ContainsKey(_level))
            {
                var _levelSettings = levelSettingsDictionary[_level];
                currentLevel = _level;
                currentScenario = _scenario;
                UpdateScenarioDictionary();
                SceneManager.LoadScene(_levelSettings.LevelBuildIndex);
            }
            else
            {
                Debug.Log("There is no key for LevelIndex " + _level.ToString());
                return;
            }
        }

        public virtual void RestartCurrentLevel()
        {
            LoadLevel(CurrentLevel, CurrentScenario);
        }

        public virtual void GoToMainMenu()
        {
            LoadLevel(LevelIndex.Main_Menu, ScenarioIndex.No_Scenario);
        }

        public virtual void GoToNextLevel()
        {
            LevelSettings _level;
            if (GetNextLevelIsSuccessful(out _level))
            {
                LoadLevel(_level.Level, ScenarioIndex.Scenario_1);
            }
            else
            {
                Debug.LogError("Couldn't Load Next Level Because Level Settings doesn't exist");
            }
        }

        public virtual void GoToNextScenario()
        {
            ScenarioSettings _scenario;
            if (GetNextScenarioIsSuccessful(out _scenario))
            {
                LoadLevel(CurrentLevel, _scenario.Scenario);
            }
            else
            {
                Debug.LogError("Couldn't Load Next Scenario Because Scenario Settings doesn't exist");
            }
        }
        #endregion

        #region Getters/Checks
        public virtual bool IsLoadingNextPermitted(out bool _nextScenario, out bool _nextLevel)
        {
            _nextScenario = IsLoadingNextScenarioPermitted();
            _nextLevel = IsLoadingNextLevelPermitted();
            return _nextScenario || _nextLevel;
        }

        //Next Scenario Getters
        protected virtual bool IsLoadingNextScenarioPermitted()
        {
            ScenarioSettings _scenario;
            return GetNextScenarioIsSuccessful(out _scenario);
        }

        protected virtual bool GetNextScenarioIsSuccessful(out ScenarioSettings _scenario)
        {
            _scenario = CurrentScenarioSettings;
            if (bCurrentScenarioIsValid == false) return false;
            var _keysDic = currentScenarioSettingsDictionary.Keys;
            var _keysList = _keysDic.ToList();
            int _keyIndex = _keysList.IndexOf(CurrentScenario);
            ScenarioIndex _nextScenario = GetScenarioIndexFromNumber(_keyIndex + 1);
            if (_keyIndex + 1 > 0 && _keyIndex + 1 <= _keysList.Count - 1 &&
                _nextScenario != ScenarioIndex.No_Scenario &&
                currentScenarioSettingsDictionary.ContainsKey(_nextScenario))
            {
                _scenario = currentScenarioSettingsDictionary[_nextScenario];
                return true;
            }
            return false;
        }

        protected virtual ScenarioIndex GetScenarioIndexFromNumber(int _index)
        {
            switch (_index)
            {
                case 0:
                    return ScenarioIndex.Scenario_1;
                case 1:
                    return ScenarioIndex.Scenario_2;
                case 2:
                    return ScenarioIndex.Scenario_3;
                case 3:
                    return ScenarioIndex.Scenario_4;
                case 4:
                    return ScenarioIndex.Scenario_5;
                case 5:
                    return ScenarioIndex.Scenario_6;
                default:
                    return ScenarioIndex.No_Scenario;
            }
        }

        //Next Level Getters
        protected virtual bool IsLoadingNextLevelPermitted()
        {
            LevelSettings _level;
            return GetNextLevelIsSuccessful(out _level);
        }

        protected virtual bool GetNextLevelIsSuccessful(out LevelSettings _level)
        {
            var _keysDic = levelSettingsDictionary.Keys;
            var _keysList = _keysDic.ToList();
            int _keyIndex = _keysList.IndexOf(CurrentLevel);
            LevelIndex _nextLevel = GetLevelIndexFromNumber(_keyIndex + 1);
            if (_keyIndex + 1 > 0 && _keyIndex + 1 <= _keysList.Count - 1 &&
                _nextLevel != LevelIndex.No_Level &&
                levelSettingsDictionary.ContainsKey(_nextLevel))
            {
                _level = levelSettingsDictionary[_nextLevel];
                return true;
            }
            _level = levelSettingsDictionary[CurrentLevel];
            return false;
        }

        protected virtual LevelIndex GetLevelIndexFromNumber(int _index)
        {
            switch (_index)
            {
                case 0:
                    return LevelIndex.Main_Menu;
                case 1:
                    return LevelIndex.Level_1;
                case 2:
                    return LevelIndex.Level_2;
                case 3:
                    return LevelIndex.Level_3;
                case 4:
                    return LevelIndex.Level_4;
                case 5:
                    return LevelIndex.Level_5;
                default:
                    return LevelIndex.No_Level;
            }
        }
        #endregion

        #region Updaters
        protected virtual void UpdateScenarioDictionary()
        {
            currentScenarioSettingsDictionary.Clear();
            var _currentLevelSettings = levelSettingsDictionary[CurrentLevel];
            foreach (var _scenarioSettings in _currentLevelSettings.ScenarioSettingsList)
            {
                currentScenarioSettingsDictionary.Add(_scenarioSettings.Scenario, _scenarioSettings);
            }
        }

        protected virtual void UpdateFrameRateLimit()
        {
            if (QualitySettings.vSyncCount != 0)
            {
                //Frame Limit Doesn't Work If VSync Is Set Above 0
                QualitySettings.vSyncCount = 0;
            }
            if (Application.targetFrameRate != RefreshRate)
            {
                Application.targetFrameRate = RefreshRate;
            }
        }
        #endregion

        #region Initialization
        protected virtual void InitializeDictionaryValues()
        {
            //Transfer Values From Serialized List To A Dictionary
            //Level Settings Data
            if (levelSettingsData == null)
            {
                Debug.LogError("No levelSettingsData on GameInstance");
                return;
            }
            foreach (var _settings in levelSettingsData.LevelSettingsList)
            {
                levelSettingsDictionary.Add(_settings.Level, _settings);
            }
        }
        #endregion

    }
}