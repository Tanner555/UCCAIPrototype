using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BaseFramework;

namespace RTSCoreFramework
{
    public class RTSSaveManager : BaseSingleton<RTSSaveManager>
    {
        #region Properties
        protected IGBPI_DataHandler dataHandler { get { return IGBPI_DataHandler.thisInstance; } }
        protected RTSStatHandler statHandler { get { return RTSStatHandler.thisInstance; } }
        protected virtual string persistentSavePath
        {
            get { return $"{Application.persistentDataPath}"; }
        }

        protected virtual string streamingDataSavePath
        {
            get
            {
                return $"{Application.dataPath}/StreamingAssets";
            }
        }
        #endregion

        #region IGBPI

        #region IGBPIProps
        protected virtual string tacticsXMLPath
        {
            get
            {
                return $"{persistentSavePath}/tactics_data.xml";
            }
        }

        protected virtual string defaultTacticsXMLPath
        {
            get { return $"{streamingDataSavePath}/XML/default_tactics_data.xml"; }
        }
        #endregion

        #region IGBPIPublicAccessors
        public virtual List<CharacterTactics> LoadCharacterTacticsList()
        {
            List<CharacterTactics> _tacticsList = new List<CharacterTactics>();
            foreach (var _checkCharacter in LoadXMLTactics())
            {
                if (_checkCharacter.CharacterType != ECharacterType.NoCharacterType)
                {
                    _tacticsList.Add(new CharacterTactics
                    {
                        CharacterName = _checkCharacter.CharacterName,
                        CharacterType = _checkCharacter.CharacterType,
                        Tactics = ValidateIGBPIValues(_checkCharacter.Tactics)
                    });
                }
            }
            return _tacticsList;
        }

        public virtual List<IGBPIPanelValue> Load_IGBPI_PanelValues(ECharacterType _cType)
        {
            CharacterTactics _tactics;
            if (!isIGBPISavingPermitted(_cType, out _tactics)) return null;
            return ValidateIGBPIValues(_tactics.Tactics);
        }

        public virtual void Save_IGBPI_PanelValues(ECharacterType _cType, List<IGBPI_UI_Panel> _panels)
        {
            CharacterTactics _tactics;
            if (!isIGBPISavingPermitted(_cType, out _tactics)) return;
            List<IGBPIPanelValue> _saveValues = new List<IGBPIPanelValue>();
            foreach (var _panel in _panels)
            {
                _saveValues.Add(new IGBPIPanelValue(
                    _panel.orderText.text,
                    _panel.conditionText.text,
                    _panel.actionText.text
                ));
            }
            Save_IGBPI_Values(_cType, _saveValues);
        }
        #endregion

        #region IGBPISaveHelpers
        protected virtual List<IGBPIPanelValue> ValidateIGBPIValues(List<IGBPIPanelValue> _values)
        {
            List<IGBPIPanelValue> _validValues = new List<IGBPIPanelValue>();
            bool _changedSaveFile = false;
            foreach (var _data in _values)
            {
                bool _hasCondition = dataHandler.IGBPI_Conditions.ContainsKey(_data.condition);
                bool _hasAction = dataHandler.IGBPI_Actions.ContainsKey(_data.action);
                int _order = -1;
                bool _hasOrder = int.TryParse(_data.order, out _order) && _order != -1;
                if (_hasCondition && _hasAction && _hasOrder)
                    _validValues.Add(_data);
                else
                    _changedSaveFile = true;
            }
            if (_changedSaveFile) Debug.Log("Loaded Save File Contents will change on next save.");
            return _validValues;
        }

        protected virtual void Save_IGBPI_Values(ECharacterType _cType, List<IGBPIPanelValue> _values)
        {
            CharacterTactics _tactics;
            if (!isIGBPISavingPermitted(_cType, out _tactics)) return;

            List<CharacterTactics> _allCharacterTactics = LoadCharacterTacticsList();
            int _indexOf = -1;
            CharacterTactics _characterToChange = new CharacterTactics
            {
                CharacterName = "",
                CharacterType = ECharacterType.NoCharacterType,
                Tactics = new List<IGBPIPanelValue>()
            };
            foreach (var _checkCharacter in _allCharacterTactics)
            {
                if (_cType != ECharacterType.NoCharacterType &&
                    _checkCharacter.CharacterType == _cType)
                {
                    _indexOf = _allCharacterTactics.IndexOf(_checkCharacter);
                    _characterToChange.CharacterName = _checkCharacter.CharacterName;
                    _characterToChange.CharacterType = _checkCharacter.CharacterType;
                    _characterToChange.Tactics = ValidateIGBPIValues(_values);
                }
            }

            if (_characterToChange.CharacterType != ECharacterType.NoCharacterType &&
                _indexOf != -1)
            {
                _allCharacterTactics[_indexOf] = _characterToChange;
                statHandler.UpdateTacticsDictionary(_allCharacterTactics);
                SaveXMLTactics(_allCharacterTactics);
            }
        }

        protected virtual IEnumerator YieldSave_IGBPI_Values(ECharacterType _cType, List<IGBPIPanelValue> _values)
        {
            Save_IGBPI_Values(_cType, _values);
            yield return new WaitForSecondsRealtime(0.5f);
            Debug.Log("Finished Saving");
        }

        protected virtual bool isIGBPISavingPermitted(ECharacterType _cType, out CharacterTactics _tactics)
        {
            _tactics = GetTacticsFromCharacter(_cType);
            if (_tactics.CharacterType == ECharacterType.NoCharacterType)
            {
                Debug.LogError("No IGBPI Data Object on Save Manager For Character Type " + _tactics.CharacterType.ToString());
                return false;
            }
            if (dataHandler == null)
            {
                Debug.LogError("No Data Handler could be found.");
                return false;
            }
            return true;
        }

        protected virtual CharacterTactics GetTacticsFromCharacter(ECharacterType _cType)
        {
            return statHandler.RetrieveAnonymousCharacterTactics(_cType);
        }

        protected virtual List<IGBPIPanelValue> GetPanelValuesFromCharacter(ECharacterType _cType)
        {
            return GetTacticsFromCharacter(_cType).Tactics;
        }
        #endregion

        #region IGBPIXMLHelpers
        protected virtual void SaveXMLTactics(List<CharacterTactics> _cTacticsList)
        {
            MyXmlManager.SaveXML<List<CharacterTactics>>(_cTacticsList, tacticsXMLPath);
        }

        protected virtual List<CharacterTactics> LoadXMLTactics()
        {
            var _tacticsList = MyXmlManager.LoadXML<List<CharacterTactics>>(tacticsXMLPath);
            if (_tacticsList == null)
            {
                var _defaultTacticsList = MyXmlManager.LoadXML<List<CharacterTactics>>(defaultTacticsXMLPath);
                if (_defaultTacticsList != null) return _defaultTacticsList;
                return new List<CharacterTactics>();
            }
            return _tacticsList;
        }
        #endregion

        #endregion

        #region CharacterStats

        #region CStatProperties
        protected virtual string CStatXMLPath
        {
            get
            {
                return $"{persistentSavePath}/characterstats_data.xml";
            }
        }

        protected virtual string defaultCStatXMLPath
        {
            get { return $"{streamingDataSavePath}/XML/default_characterstats_data.xml"; }
        }
        #endregion 

        #region CStatsHelpers
        protected virtual CharacterStatsSimple ConvertCharacterStatsToSimple(CharacterStats _stats)
        {
            return new CharacterStatsSimple
            {
                name = _stats.name,
                CharacterType = _stats.CharacterType,
                MaxHealth = _stats.MaxHealth,
                Health = _stats.Health,
                MaxStamina = _stats.MaxStamina,
                Stamina = _stats.Stamina,
                EquippedWeapon = _stats.EquippedWeapon,
                PrimaryWeapon = _stats.PrimaryWeapon,
                SecondaryWeapon = _stats.SecondaryWeapon
            };
        }
        #endregion

        #region CStatsPublicAccessors
        public virtual void SaveCharacterStats(List<CharacterStats> _allCStats)
        {
            List<CharacterStatsSimple> _simpleStatsList = new List<CharacterStatsSimple>();
            foreach (var _stats in _allCStats)
            {
                var _simpleStats = ConvertCharacterStatsToSimple(_stats);
                _simpleStatsList.Add(_simpleStats);
            }
            SaveSimpleCharacterStats(_simpleStatsList);
        }

        public virtual void SaveSimpleCharacterStats(List<CharacterStatsSimple> _cStatsList)
        {
            SaveXMLCStats(_cStatsList);
            Debug.Log("Saved Character Stats");
        }

        public virtual List<CharacterStatsSimple> LoadCharacterStats()
        {
            return LoadXMLCStats();
        }
        #endregion

        #region CStatsXMLHelpers
        protected virtual void SaveXMLCStats(List<CharacterStatsSimple> _cStatsList)
        {
            MyXmlManager.SaveXML<List<CharacterStatsSimple>>(_cStatsList, CStatXMLPath);
        }

        protected virtual List<CharacterStatsSimple> LoadXMLCStats()
        {
            var _cStatList = MyXmlManager.LoadXML<List<CharacterStatsSimple>>(CStatXMLPath);
            if (_cStatList == null)
            {
                var _defaultCStatList = MyXmlManager.LoadXML<List<CharacterStatsSimple>>(defaultCStatXMLPath);
                if (_defaultCStatList != null) return _defaultCStatList;
                return new List<CharacterStatsSimple>();
            }
            return _cStatList;
        }
        #endregion

        #endregion

        #region UnityMessages
        protected virtual void OnEnable()
        {
            
        }

        protected virtual void Start()
        {
            Invoke("DelayStart", 0.5f);
        }

        protected virtual void DelayStart()
        {
            
        }
        #endregion
    }
}