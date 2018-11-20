using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RTSCoreFramework
{
    [System.Serializable]
    public class IGBPI_UI_Panel : MonoBehaviour, IPointerClickHandler,
            IPointerEnterHandler, IPointerExitHandler,
            IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        #region Fields
        [Header("UI Text")]
        [SerializeField]
        public Text orderText;
        [SerializeField] public Text conditionText;
        [SerializeField] public Text actionText;

        [Header("Colors")]
        public Color highlightColor;
        public Color selectedColor = Color.cyan;
        private Color normalColor;

        bool hasStarted = false;

        //Dragging
        private GameObject currentDragGObject = null;
        #endregion

        #region Properties
        public bool AllTextAreValid { get { return orderText && conditionText && actionText; } }
        public bool IsUISelection { get { return uiManager != null && uiManager.UIPanelSelection == this; } }
        private RTSUiManager uiManager { get { return RTSUiManager.thisInstance; } }
        private RTSUiMaster uiManagerMaster { get { return RTSUiMaster.thisInstance; } }

        #endregion

        #region CommentedCode
        //public bool AllMenusAreValid
        //{
        //    get
        //    {
        //        return (comparisontype && /*compare_class_01 && compare_class_02 && class_01_attribute & class_02_attribute && conditionaltype && classtocarryoutevent &&*/ eventfromclasstoexecute);
        //    }
        //}
        //public List<Dropdown> Dropdown_Menus { get { return UI_Dropdown_Menus; } }
        //private List<Dropdown> UI_Dropdown_Menus;

        //private IGBPI_UIHandler behaviorUIManager;
        //private IGBPI_Manager_Master behaviorManagerMaster;
        //private IGBPI_MenuSelectionHandler behaviorMenuManager;
        ////Conditional Dropdown Menus
        //[SerializeField] //Bool, String, or GameObject
        //private Dropdown comparisontype;
        ////[SerializeField] //First Class to Compare
        ////private Dropdown compare_class_01;
        ////[SerializeField] //Second Class to Compare
        ////private Dropdown compare_class_02;
        ////[SerializeField]//First Class Attribute
        ////private Dropdown class_01_attribute;
        ////[SerializeField]//Second Class Attribute
        ////private Dropdown class_02_attribute;
        ////[SerializeField] //Greater, Less, or Equal
        ////private Dropdown conditionaltype;
        ////Action Dropdown Menus
        ////[SerializeField] //ClassToCarryOutEvent
        ////private Dropdown classtocarryoutevent;
        //[SerializeField] //EventFromClassToExecute
        //private Dropdown eventfromclasstoexecute;
        ////Getters
        //public Dropdown ComparisonType { get { return comparisontype; } }
        ////public Dropdown Compare_Class_01 { get { return compare_class_01; } }
        ////public Dropdown Compare_Class_02 { get { return compare_class_02; } }
        ////public Dropdown Class_01_Attribute { get { return class_01_attribute; } }
        ////public Dropdown Class_02_Attribute { get { return class_02_attribute; } }
        ////public Dropdown ConditionalType { get { return conditionaltype; } }
        ////public Dropdown ClassToCarryOutEvent { get { return classtocarryoutevent; } }
        //public Dropdown EventFromClassToExecute { get { return eventfromclasstoexecute; } }
        #endregion

        #region UnityMessages
        // Use this for initialization
        void Start()
        {
            if (hasStarted == false)
            {
                SetupInitialReferences();
                SubToEvents();
                hasStarted = true;
            }
        }

        private void OnEnable()
        {
            if (hasStarted == true)
            {
                SubToEvents();
            }
        }

        void OnDisable()
        {
            DeSubFromEvents();
        }

        // Update is called once per frame
        void Update()
        {

        }
        #endregion

        #region ImplementationMethods
        public void OnPointerClick(PointerEventData eventData)
        {
            if (uiManagerMaster && uiManager && !IsUISelection && AllTextAreValid)
            {
                uiManagerMaster.CallEventUIPanelSelectionChanged(this);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!IsUISelection)
                GetComponent<Image>().color = highlightColor;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!IsUISelection)
                GetComponent<Image>().color = normalColor;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            uiManagerMaster.isDraggingIGBPI = true;
            if (!IsUISelection) uiManagerMaster.CallEventUIPanelSelectionChanged(this);
            if (currentDragGObject == null)
            {
                currentDragGObject = new GameObject("Drag Cursor Visual");
                currentDragGObject.transform.parent = uiManager.IGBPIUi.transform;
                var _rectTrans = currentDragGObject.AddComponent<RectTransform>();
                var _image = currentDragGObject.AddComponent<Image>();
                if (GetComponent<Image>())
                {
                    var _myImage = GetComponent<Image>();
                    _image.sprite = _myImage.sprite;
                    _image.color = new Color(_myImage.color.r,
                        _myImage.color.g, _myImage.color.b, _myImage.color.a / 3);
                }
                if (GetComponent<RectTransform>())
                {
                    var _myRectTrans = GetComponent<RectTransform>();
                    _rectTrans.localPosition = _myRectTrans.localPosition;
                    _rectTrans.localScale = _myRectTrans.localScale;
                    _rectTrans.sizeDelta = _myRectTrans.sizeDelta;
                }
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (currentDragGObject != null)
            {
                currentDragGObject.transform.position =
                    Input.mousePosition;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (currentDragGObject != null)
                Destroy(currentDragGObject);

            uiManagerMaster.isDraggingIGBPI = false;
            int _order = uiManager.GetOnDragEndPanelOrderIndex();
            if (_order != -1)
                uiManagerMaster.CallEventMovePanelUI(this, _order);
        }
        #endregion

        #region Handlers
        void ChangeUIPanelVisuals(IGBPI_UI_Panel _info)
        {
            if (_info == this)
            {
                if (GetComponent<Image>())
                    GetComponent<Image>().color = selectedColor;
            }
            else if (uiManager.PreviousPanelSelection == this || _info == null)
            {
                if (GetComponent<Image>())
                    GetComponent<Image>().color = normalColor;
            }
        }
        void ResetUIMenusIfRequired(IGBPI_UI_Panel _info)
        {
            if (_info == this)
                ResetUIMenus();
        }

        void ResetUIMenus()
        {
            //if (AllMenusAreValid)
            //{
            //    foreach (var _menu in Dropdown_Menus)
            //    {
            //        _menu.ClearOptions();
            //        _menu.onValueChanged.RemoveAllListeners();
            //        _menu.onValueChanged.AddListener(delegate { OnMenuSelectionChange(_menu); });

            //    }
            //}
        }
        #endregion

        #region Initialization
        void SetupInitialReferences()
        {
            normalColor = GetComponent<Image>().color;
            if (!uiManager)
                Debug.LogError("No ui manager could be found!");
            if (!uiManagerMaster)
                Debug.LogError("No ui master could be found!");

            if (AllTextAreValid == false)
            {
                Debug.LogError("Please drag dropdown instance into dropdown slots!");
            }

        }

        void SubToEvents()
        {
            uiManagerMaster.EventUIPanelSelectionChanged += ChangeUIPanelVisuals;
            uiManagerMaster.EventResetAllPaneUIMenus += ResetUIMenus;
            uiManagerMaster.EventResetPanelUIMenu += ResetUIMenusIfRequired;
        }

        void DeSubFromEvents()
        {
            uiManagerMaster.EventUIPanelSelectionChanged -= ChangeUIPanelVisuals;
            uiManagerMaster.EventResetAllPaneUIMenus -= ResetUIMenus;
            uiManagerMaster.EventResetPanelUIMenu -= ResetUIMenusIfRequired;
        }
        #endregion
    }
}