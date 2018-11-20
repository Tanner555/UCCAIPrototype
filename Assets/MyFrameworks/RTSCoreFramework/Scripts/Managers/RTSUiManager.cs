using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BaseFramework;

namespace RTSCoreFramework
{
    public class RTSUiManager : UiManager
    {
        #region Properties      
        public IGBPI_DataHandler dataHandler { get { return IGBPI_DataHandler.thisInstance; } }
        public RTSSaveManager saveManager { get { return RTSSaveManager.thisInstance; } }
        public RTSStatHandler statHandler { get { return RTSStatHandler.thisInstance; } }

        //UGBPI
        public bool isBehaviorUIOn { get { return IGBPIUi != null && IGBPIUi.activeSelf; } }
        public List<IGBPI_UI_Panel> UI_Panel_Members { get; protected set; }
        public IGBPI_UI_Panel UIPanelSelection
        {
            get; protected set;
        }
        public IGBPI_UI_Panel PreviousPanelSelection
        {
            get; protected set;
        }

        public override bool AllUiCompsAreValid
        {
            get { return IGBPICompsAreValid && CharacterStatsPanels &&
                    CharacterStatsPrefab && MenuUiPanel && WinnerUiPanel &&
                    NextLevelButton && GameOverUiPanel; }
        }

        public bool IGBPICompsAreValid
        {
            get
            {
                return UI_Panel_Prefab &&
ButtonChoicePrefab && behaviorContentTransform &&
choiceMenuTransform && choiceIndicatorText &&
choiceNavigateLeft && choiceNavigateRight &&
conditionButton && actionButton && IGBPITitleText;
            }
        }

        #endregion

        #region OverrideAndHideProperties
        new public RTSGameMaster gamemaster { get { return RTSGameMaster.thisInstance; } }
        new public RTSGameMode gamemode { get { return RTSGameMode.thisInstance; } }
        new public RTSGameInstance gameInstance { get { return RTSGameInstance.thisInstance; } }

        new public RTSUiMaster uiMaster
        {
            get
            {
                if (RTSUiMaster.thisInstance != null)
                    return RTSUiMaster.thisInstance;

                return GetComponent<RTSUiMaster>();
            }
        }

        new public static RTSUiManager thisInstance
        {
            get { return UiManager.thisInstance as RTSUiManager; }
        }
        #endregion

        #region Fields
        enum choiceEditModes
        {
            condition, action, none
        }
        choiceEditModes ChoiceEditMode = choiceEditModes.none;
        string choiceFilterNameNone = "Filters";
        //Used as a parameter for adding a dropdown instance
        bool usePanelCreationValues = false;
        IGBPIPanelValue panelCreationValues;
        #endregion

        #region UIGameObjects
        [Header("IGBPI Objects")]
        public GameObject IGBPIUi;
        public GameObject UI_Panel_Prefab;
        public GameObject ButtonChoicePrefab;
        public Transform behaviorContentTransform;
        public Transform choiceMenuTransform;
        public Text choiceIndicatorText;
        public Button choiceNavigateLeft;
        public Button choiceNavigateRight;
        public Button conditionButton;
        public Button actionButton;
        //Used To Indicate Which Character Contains Tactics
        public Text IGBPITitleText;

        [Header("Character Stats Objects")]
        public GameObject CharacterStatsPanels;
        public GameObject CharacterStatsPrefab;
        #endregion

        #region UnityMessages
        protected override void OnEnable()
        {
            base.OnEnable();
            if (!AllUiCompsAreValid)
                Debug.LogError("Please drag components into their slots");

            UI_Panel_Members = new List<IGBPI_UI_Panel>();
            DisableIGBPIEditButtons();
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }
        #endregion

        #region ButtonCalls-IGBPI
        public void CallAddDropdown()
        {
            if (uiMaster != null) uiMaster.CallEventAddDropdownInstance();
        }

        public void CallRemoveDropdown()
        {
            if (uiMaster != null && UIPanelSelection != null)
            {
                uiMaster.CallEventRemoveDropdownInstance(UIPanelSelection);
            }
        }

        public void CallToggleIGBPI()
        {
            if (uiMaster != null) uiMaster.CallEventIGBPIToggle();
        }

        public void CallIGBPISave()
        {
            StartCoroutine(SaveIGBPIData());
        }

        public void CallEditCondition()
        {
            ChoiceEditMode = choiceEditModes.condition;
            EnableFilterNav();
            AddChoicesForCondition();
        }

        public void CallEditAction()
        {
            ChoiceEditMode = choiceEditModes.action;
            EnableFilterNav();
            AddChoicesForAction();
        }

        public void CallIGBPINavLeft()
        {
            if (!IGBPICompsAreValid ||
                ChoiceEditMode == choiceEditModes.none) return;

            if (ChoiceEditMode == choiceEditModes.condition)
            {
                var _filter = dataHandler.NavPreviousCondition();
                choiceIndicatorText.text = _filter.ToString();
                AddChoicesForCondition();
            }
            else if (ChoiceEditMode == choiceEditModes.action)
            {
                var _filter = dataHandler.NavPreviousAction();
                choiceIndicatorText.text = _filter.ToString();
                AddChoicesForAction();
            }
        }

        public void CallIGBPINavRight()
        {
            if (!IGBPICompsAreValid ||
                ChoiceEditMode == choiceEditModes.none) return;

            if (ChoiceEditMode == choiceEditModes.condition)
            {
                var _filter = dataHandler.NavForwardCondition();
                choiceIndicatorText.text = _filter.ToString();
                AddChoicesForCondition();
            }
            else if (ChoiceEditMode == choiceEditModes.action)
            {
                var _filter = dataHandler.NavForwardAction();
                choiceIndicatorText.text = _filter.ToString();
                AddChoicesForAction();
            }
        }
        #endregion

        #region Handlers-General/Toggles
        void ToggleInventoryUi(bool enable)
        {
            //if (InventoryUi != null)
            //    InventoryUi.SetActive(enable);
        }

        void ToggleIGBPIUi(bool enable)
        {
            if (AllUiCompsAreValid == false) return;
            if (enable == false)
            {
                uiMaster.CallEventUIPanelSelectionChanged(null);
                StartCoroutine(RemoveAllIGBPIPanelsAfterWait(0.05f));
            }
            else
            {
                StartCoroutine(LoadIGBPIDataAfterWait(0.05f));
            }

            if (IGBPIUi != null)
                IGBPIUi.SetActive(enable);

            if (IGBPIUi.activeSelf)
            {
                //Indicate Who Owns These Tactics
                IGBPITitleText.text =
                    gamemode.CurrentPlayer.CharacterType.ToString() +
                    "'s Tactics";
            }
        }
        #endregion

        #region Handlers-IGBPI
        void SelectUIPanel(IGBPI_UI_Panel _info)
        {
            if (_info && _info.gameObject && _info.AllTextAreValid)
            {
                PreviousPanelSelection = UIPanelSelection;
                UIPanelSelection = _info;
                DisableIGBPIEditButtons(true);
            }
            else if (_info != null && _info.gameObject && !_info.AllTextAreValid)
            {
                Debug.LogError("UI Info doesn't have all text objects assigned, cannot select panel");
                UIPanelSelection = null;
                DisableIGBPIEditButtons(false);
            }
            else
            {
                PreviousPanelSelection = UIPanelSelection;
                UIPanelSelection = null;
                DisableIGBPIEditButtons(false);
            }
        }

        void AddDropdownInstance()
        {
            if (!behaviorContentTransform || !UI_Panel_Prefab)
                return;

            var _dropdownInstance = Instantiate(UI_Panel_Prefab);
            _dropdownInstance.transform.SetParent(behaviorContentTransform, false);
            var _rect = _dropdownInstance.GetComponent<RectTransform>();
            var _mydropdownpanel = _dropdownInstance.GetComponent<IGBPI_UI_Panel>();

            if (_mydropdownpanel != null && _mydropdownpanel.AllTextAreValid)
            {
                RegisterDropdownMenu(_mydropdownpanel);
                if (usePanelCreationValues)
                {
                    _mydropdownpanel.conditionText.text = panelCreationValues.condition;
                    _mydropdownpanel.actionText.text = panelCreationValues.action;
                    int _panelOrder;
                    if (int.TryParse(panelCreationValues.order, out _panelOrder))
                    {
                        MoveIGBPIPanel(_mydropdownpanel, _panelOrder - 1);
                    }
                }
                else
                {
                    MoveIGBPIPanelBelowSelection(_mydropdownpanel);
                }
            }
            else
            {
                Debug.LogError("DeRegistering Panel because all requirements were not met!");
                uiMaster.CallEventRemoveDropdownInstance(_mydropdownpanel);
            }

            usePanelCreationValues = false;
        }

        void DeregisterDropdownMenu(IGBPI_UI_Panel _info)
        {
            StartCoroutine(DeleteIGBPIPanelAfterWait(_info, 0.05f));
        }

        void ReorderIGBPIPanels()
        {
            int _count = 1;
            foreach (Transform _trans in behaviorContentTransform)
            {
                if (_trans.GetComponent<IGBPI_UI_Panel>())
                {
                    var _panel = _trans.GetComponent<IGBPI_UI_Panel>();
                    _panel.orderText.text = _count.ToString();

                    _trans.gameObject.name =
                    _panel.orderText.text + ": " +
                    _panel.conditionText.text;

                    _count++;
                }
            }
        }
        #endregion

        #region Handlers-CharactersStats
        void RegisterAllyToCharacterStatMonitor(PartyManager _party, AllyMember _ally)
        {
            if (AllUiCompsAreValid == false || 
                _party == null || _ally == null) return;

            var _statGameObject = GameObject.Instantiate(CharacterStatsPrefab, 
                CharacterStatsPanels.transform) as GameObject;
            var _statsMonitor = _statGameObject.GetComponent<RTSCharacterStatsMonitor>();
            if (_statsMonitor != null &&
                _statsMonitor.bUiTargetIsSet == false)
            {
                _statsMonitor.HookAllyCharacter(_ally);
            }
        }
        #endregion

        #region BehaviorUIMethods
        public IGBPI_UI_Panel GetPanelFromOrder(int _order)
        {
            IGBPI_UI_Panel _panel = null;
            foreach (Transform _transform in behaviorContentTransform)
            {
                if (_transform.GetComponent<IGBPI_UI_Panel>())
                {
                    if (_transform.GetSiblingIndex() == _order)
                        _panel = _transform.GetComponent<IGBPI_UI_Panel>();
                }
            }
            return _panel;
        }

        public int GetOnDragEndPanelOrderIndex()
        {
            int _currentOrder = -1;
            Transform _previousTrans = null;
            foreach (Transform _transform in behaviorContentTransform)
            {
                if (_transform.GetComponent<IGBPI_UI_Panel>())
                {
                    var _panel = _transform.GetComponent<IGBPI_UI_Panel>();
                    bool _greaterY = Input.mousePosition.y > _transform.position.y;
                    bool _greaterPrevY = _previousTrans == null ||
                        Input.mousePosition.y < _previousTrans.position.y;
                    bool _belowLastSibling = behaviorContentTransform.childCount ==
                        _transform.GetSiblingIndex() + 1 && !_greaterY;

                    if (_greaterY && _greaterPrevY)
                    {
                        int _processOrder = -1;
                        if (int.TryParse(_panel.orderText.text, out _processOrder))
                        {
                            _currentOrder = _processOrder;
                        }
                    }
                    else if (_belowLastSibling)
                    {
                        _currentOrder = behaviorContentTransform.childCount + 1;
                    }
                    _previousTrans = _transform;
                }
            }
            return _currentOrder - 1;
        }

        void MoveIGBPIPanel(IGBPI_UI_Panel _panel, int _order)
        {
            if (_panel != null && _panel.AllTextAreValid)
            {
                _panel.transform.SetSiblingIndex(_order);
            }
            uiMaster.CallEventReorderIGBPIPanels();
        }

        void MoveIGBPIPanelBelowSelection(IGBPI_UI_Panel _panel)
        {
            if (_panel != null && _panel.AllTextAreValid)
            {
                if (UIPanelSelection && UIPanelSelection.AllTextAreValid)
                {
                    int _selIndex = UIPanelSelection.transform.GetSiblingIndex();
                    _panel.transform.SetSiblingIndex(_selIndex + 1);
                }
            }
            uiMaster.CallEventReorderIGBPIPanels();
        }

        IEnumerator SaveIGBPIData()
        {
            if (IGBPICompsAreValid)
            {
                List<IGBPI_UI_Panel> _saveData = new List<IGBPI_UI_Panel>();
                Queue<IGBPI_UI_Panel> _invalidData = new Queue<IGBPI_UI_Panel>();
                foreach (Transform _choice in behaviorContentTransform)
                {
                    if (_choice.GetComponent<IGBPI_UI_Panel>())
                    {
                        var _panel = _choice.GetComponent<IGBPI_UI_Panel>();
                        if (IGBPIPanelIsValidForSave(_panel))
                            _saveData.Add(_panel);
                        else
                            _invalidData.Enqueue(_panel);
                    }
                }
                while (_invalidData.Count > 0)
                {
                    var _invalidPanel = _invalidData.Dequeue();
                    Debug.Log("Deleting Invalid Data: " + _invalidPanel.transform.name);
                    uiMaster.CallEventRemoveDropdownInstance(_invalidPanel);
                }

                yield return new WaitForSecondsRealtime(0.8f);

                if (_saveData.Count > 0)
                {
                    var _cType = gamemode.CurrentPlayer.CharacterType;
                    saveManager.Save_IGBPI_PanelValues(_cType, _saveData);
                    uiMaster.CallEventOnSaveIGBPIComplete();
                    Debug.Log("Save Successful");
                }
                else
                {
                    Debug.Log("There is no IGBPI Data to save");
                }
            }
        }

        IEnumerator LoadIGBPIDataAfterWait(float _seconds)
        {
            yield return new WaitForSecondsRealtime(_seconds);
            var _cType = gamemode.CurrentPlayer.CharacterType;
            foreach (var _data in saveManager.Load_IGBPI_PanelValues(_cType))
            {
                panelCreationValues = _data;
                usePanelCreationValues = true;
                uiMaster.CallEventAddDropdownInstance();
            }
        }

        IEnumerator DeleteIGBPIPanelAfterWait(IGBPI_UI_Panel _info, float _seconds)
        {
            if (_info.gameObject != null)
            {
                if (UIPanelSelection == _info)
                {
                    UIPanelSelection = null;
                    DisableIGBPIEditButtons();
                }
                UI_Panel_Members.Remove(_info);
                Destroy(_info.gameObject);
            }
            yield return new WaitForSecondsRealtime(_seconds);
            uiMaster.CallEventReorderIGBPIPanels();
        }

        bool IGBPIPanelIsValidForSave(IGBPI_UI_Panel _panel)
        {
            bool _textValid = _panel.AllTextAreValid;
            bool _condValid = dataHandler.IGBPI_Conditions.ContainsKey
                (_panel.conditionText.text);
            bool _actionValid = dataHandler.IGBPI_Actions.ContainsKey
                (_panel.actionText.text);
            return _panel != null && _textValid &&
                _condValid && _actionValid;
        }

        void EnableFilterNav()
        {
            choiceNavigateLeft.interactable = true;
            choiceNavigateRight.interactable = true;
        }

        void AddChoicesForCondition()
        {
            ClearChoiceButtons();
            var _filter = dataHandler.conditionFilter;
            choiceIndicatorText.text = _filter.ToString();
            List<string> _filteredKeys = new List<string>();
            foreach (var _item in dataHandler.IGBPI_Conditions)
            {
                if (_item.Value.filter == _filter)
                    _filteredKeys.Add(_item.Key);
            }
            foreach (var _key in _filteredKeys)
            {
                AddChoiceButton(_key);
            }
        }

        void AddChoicesForAction()
        {
            ClearChoiceButtons();
            var _filter = dataHandler.actionFilter;
            choiceIndicatorText.text = _filter.ToString();
            List<string> _filteredKeys = new List<string>();
            foreach (var _item in dataHandler.IGBPI_Actions)
            {
                if (_item.Value.actionFilter == _filter)
                    _filteredKeys.Add(_item.Key);
            }
            foreach (var _key in _filteredKeys)
            {
                AddChoiceButton(_key);
            }
        }

        void AddChoiceButton(string _name)
        {
            var _choiceInstance = Instantiate(ButtonChoicePrefab);
            _choiceInstance.transform.SetParent(choiceMenuTransform, false);
            _choiceInstance.name = "Choice: " + _name;
            if (_choiceInstance.GetComponent<Button>() == null)
            {
                Debug.LogError("Prefab is not a button!");
                Destroy(_choiceInstance);
                return;
            }
            else
            {
                var _button = _choiceInstance.GetComponent<Button>();
                if (_button.GetComponentInChildren<Text>())
                    _button.GetComponentInChildren<Text>().text = _name;
                _button.onClick.AddListener(() => {
                    if (UIPanelSelection != null)
                    {
                        if (ChoiceEditMode == choiceEditModes.condition)
                            UIPanelSelection.conditionText.text = _name;
                        else if (ChoiceEditMode == choiceEditModes.action)
                            UIPanelSelection.actionText.text = _name;

                        DisableIGBPIEditButtons(true);
                    }
                });
            }

        }

        void ClearChoiceButtons()
        {
            foreach (Transform _transform in choiceMenuTransform)
            {
                if (_transform.GetComponent<Button>())
                {
                    var _button = _transform.GetComponent<Button>();
                    _button.onClick.RemoveAllListeners();
                    Destroy(_button);
                }
                Destroy(_transform.gameObject);
            }
        }

        void RegisterDropdownMenu(IGBPI_UI_Panel _info)
        {
            UI_Panel_Members.Add(_info);
        }

        void DisableIGBPIEditButtons(bool _enableEdits = false)
        {
            conditionButton.interactable = _enableEdits;
            actionButton.interactable = _enableEdits;
            ClearChoiceButtons();
            choiceNavigateLeft.interactable = false;
            choiceNavigateRight.interactable = false;
            choiceIndicatorText.text = choiceFilterNameNone;
        }

        IEnumerator RemoveAllIGBPIPanelsAfterWait(float _seconds)
        {
            yield return new WaitForSecondsRealtime(_seconds);
            foreach (Transform _trans in behaviorContentTransform)
            {
                if (_trans.GetComponent<IGBPI_UI_Panel>())
                {
                    var _panel = _trans.GetComponent<IGBPI_UI_Panel>();
                    uiMaster.CallEventRemoveDropdownInstance(_panel);
                }
            }
            UIPanelSelection = null;
            PreviousPanelSelection = null;
        }
        #endregion

        #region Initialization
        protected override void SubToEvents()
        {
            base.SubToEvents();
            //Toggles
            //uiMaster.EventInventoryUIToggle += ToggleInventoryUi;
            uiMaster.EventIGBPIToggle += ToggleIGBPIUi;
            //IGBPI
            uiMaster.EventAddDropdownInstance += AddDropdownInstance;
            uiMaster.EventRemoveDropdownInstance += DeregisterDropdownMenu;
            uiMaster.EventUIPanelSelectionChanged += SelectUIPanel;
            uiMaster.EventReorderIGBPIPanels += ReorderIGBPIPanels;
            uiMaster.EventMovePanelUI += MoveIGBPIPanel;
            //CharacterStats
            uiMaster.RegisterAllyToCharacterStatMonitor += RegisterAllyToCharacterStatMonitor;
        }

        protected override void UnsubEvents()
        {
            base.UnsubEvents();
            //Toggles
            //uiMaster.EventInventoryUIToggle -= ToggleInventoryUi;
            uiMaster.EventIGBPIToggle -= ToggleIGBPIUi;
            //IGBPI
            uiMaster.EventAddDropdownInstance -= AddDropdownInstance;
            uiMaster.EventRemoveDropdownInstance -= DeregisterDropdownMenu;
            uiMaster.EventUIPanelSelectionChanged -= SelectUIPanel;
            uiMaster.EventReorderIGBPIPanels -= ReorderIGBPIPanels;
            uiMaster.EventMovePanelUI -= MoveIGBPIPanel;
            //CharacterStats
            uiMaster.RegisterAllyToCharacterStatMonitor -= RegisterAllyToCharacterStatMonitor;
        }
        #endregion

    }
}